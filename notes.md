# Notes

## Supporting Service Infrastructure

There are two APIs in the code base that facilitate cross-service interactions and synchronization:

* The [Gateway API](./services/common/Distributed.Core/Gateway/) enables the registration of an `HttpClient`-based service for accessing a sub-set of interactions available on its corresponding service.

* The [Events API](./services/common/Distributed.Core/Services/Events) enables SignalR-based services that define data synchronization endpoints that broadcast messages whenenver associated events occur. For instance, notification of whenever an entity is modified.

## Configuration - Options Pattern

When you have a standardized JSON configuration pattern, you can implement the [Options Pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0) to simplify the process of reading configuration. This way, instead of injecting `IConfiguration` and trying to read the configuration directly, you can just inject the configuration directly.

The **Gateway API** uses this pattern via the [`GatewayOptions`](./services/common/Distributed.Core/Gateway/GatewayOptions.cs) record and the [`ConfigureGatewayOptions`](./services/common/Distributed.Core/Extensions/ConfigurationExtensions.cs#L62) extension, as implemented in the [Proposals](./services/api/Distributed.Proposals.Api/Program.cs#L11) and [Workflows](./services/api/Distributed.Workflows.Api/Program.cs#L11) services.

### Configuration - Events

Because the configuration pattern for the **Events API** is not standard (the names of the services in the configuration section will differ depending on the service being configured), it cannot implement the Options pattern. Instead, a [`GetEventEndpoint`](./services/common/Distributed.Core/Extensions/ConfigurationExtensions.cs#L67) extension method has been written against `IConfiguration` that standardizes the process of retrieving an EventHub endpoint.

## Global Enum String Conversion Configuration

To simplify the storage and transport of [enum-based actions and states](./services/common/Distributed.Contracts/Enums/), [EntityContext](./services/common/Distributed.Core/Data/EntityContext.cs#L27) and [JsonSerializerOptions](./services/common/Distributed.Core/Extensions/ConfigurationExtensions.cs#L19) are globally configured to convert enums to and from strings.

**EntityContext**

```cs
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configuretionBuilder.Properties<Enum>()
        .HasConversion<string>();
}
```

**JsonSerializerOptions**

```cs
public static JsonSerializerOptions ConfigureJsonOptions(JsonSerializerOptions options)
{
    options.Converters.Add(new JsonStringEnumConverter());
    // other JsonSerializerOptions settings
}
```

## Service Infrastructure Responsibilities

This section defines a localized approach to defining service infrastructure inspired by the [Command Query Responsiblity Segregation (CQRS)](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/cqrs-pattern.html), [Event Sourcing](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/service-per-team.html), and [Saga](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/saga-pattern.html) patterns.

The following terms are used to break what is the traditional role of API-based services into smaller, more focused responsiblities:

* [**Queries**](./services/common/Distributed.Core/Services/EntityQuery.cs) - Data retrieval methods; represented by the standard data accessors from traditional API services.

* [**Commands**](./services/common/Distributed.Core/Services/EntityCommand.cs) - User-based, public data mutation methods; represented by the standard **Save** and **Delete** methods of API services.

* [**Sagas**](./services/common/Distributed.Core/Services/EntitySaga.cs) - System-based, private data mutation methods; they isolate reactionary logic for determining how to handle effects of mutations triggered through **Events**. The **Saga** should only be concerned with the data encapsulated in the service that defines it. All services should be self-sufficient with reacting to events.

* [**Events**](./services/common/Distributed.Core/Services/Events/) - Events are comprised of two components: the [EventHub](./services/common/Distributed.Core/Services/Events/Hub/EntityEventHub.cs) which is used to broadcast when data has been mutated, and the [EventListener](./services/common/Distributed.Core/Services/Events/Client/EntityEventListener.cs) which is used to execute corresponding **Saga** methods to handle the follow on effects of the data mutation. **EventHubs** are injected into **Commands** and broadcast messages within *Sync{x}* events on the **Command** service.

This approach is important because it creates isolated boundaries around service responsiblities:
* All data retrieval concerns associated with a specific data type can be shared across the system without worrying about exposing data manipulation logic.

*  It isolates intentional and reciprocal modifications from each other. By exposing **Commands** as intentional, user-driven data mutations and **Sagas** as reciprocal, system-driven mutations:

    * **Commands** allow user-driven data mutations to be exposed without having to consider all of the after-effects that should occur as a result

    * **Sagas** provide a single place for handling after-effects that do not require the same scrutiny (authentication / authorization) as **Commands**. They are private, internally managed mutations that facilitate recursive reactions across the whole data dependency hierarchy.

## Service Class API Definition Order

**EventQuery** implementation

```cs
public class DataQuery : EntityQuery<Data, DataContext>
{
    public DataQuery(DataContext db) : base(db)
    { }

    // query data specific to the Data entity
}
```

**IEventHub**

Defines the interface for the broadcast events available to the **EventHub**.

```cs
public interface IDataEventHub : IEventHub<Data>
{
    Task OnEvent(IEventMessage<T> message);
}
```

**EventHub**

```cs
public class DataEventHub : EventHub<Data, IDataEventHub>
{ }
```

**Command**

```cs
public class DataCommand : EntityCommand<Data, DataEventHub, IDataEventHub, DataContext>
{
    public DataCommand(DataContext db, IHubContext<DataEventHub, IDataEventHub> events)
    : base(db, events)
    { }

    Func<Data, Task> SyncEvent => async (Data data) =>
    {
        EventMessage<Data> message = GenerateMessage(data, "event");

        await events
            .Clients
            .All
            .OnEvent(message);
    };

    public async Task<ApiMessage<Data>> Process(Data data)
    {
        // process data in some way
        await SyncEvent(data);
        // return
    }
}
```

**IEventListener**

```cs
public interface IDataEventListener : IEventListener<Data>
{
    EventAction OnEvent { get; }
}
```

**EventListener**

Event listeners will be defined for every entity that requires a **Saga** and needs to react to data mutations that generated outside of the control of the service.

You will only define an event listeners for entities that either:

1. Do not provide a direct interface to the entity and need to react to internal system changes affecting the entity
2. Interface with a contract to an external service

```cs
public class DataEventListener : EventListener<Data, DataSaga, DataContext>
{
    readonly string EVENT_ENDPOINT = "Data";

    public EventAction OnEvent { get; }

    public DataEventListener(DataSaga saga, IConfiguration config)
    : base(
        saga,
        config.GetEventEndpoint(EVENT_ENDPOINT)
    )
    {
        OnEvent = new(nameof(OnEvent), connection);

        OnEvent.Set<IEventMessage<Data>>(HandleEvent);
    }

    Task HandleEvent(IEventMessage<Data> message) =>
        saga.OnEvent(message.Data);
}
```

**Saga**

Sagas will typically be defined in services for entities that either:

1. Do not provide a direct interface to the entity and need to react to internal system changes affecting the entity
2. Interface with a contract to an external service

For instance, in the **Proposals** service in this repository, the only Saga defined will be for the `Package` and will simply check for when the `Package` has been completed through a `Workflow`. This way, it can generate the new `Status` for the `Proposal` based on the result of the `Workflow`.

```cs
public  class DataSaga  : EntitySaga<Data,DataContext>
{
    public DataSaga(DataContext db) : base(db)
    { }

    public Task<ApiMessage<Data>> OnEvent(Data data)
    {
        // Respond to an event associated with Data
    }
}
```

## Service Registration

To keep API configuration clean and simplify the process of registering standard API services, instances of the [ServiceRegistrant](./services/common/Distributed.Core/Services/ServiceRegistrant.cs) class handle API service registration.

The library defining API service classes will also define a [Registrant](./services/logic/Distributed.Workflows.Logic/Registrant.cs) that specifies how to register the services.

```cs
using Distributed.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Workflows.Logic;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddScoped<PackageQuery>();
        services.AddScoped<PackageCommand>();

        services.AddScoped<ProcessQuery>();
        services.AddScoped<ProcessCommand>();

        services.AddScoped<WorkflowQuery>();
        services.AddScoped<WorkflowCommand>();

        services.AddScoped<ProcessTemplateQuery>();
        services.AddScoped<ProcessTemplateCommand>();

        services.AddScoped<WorkflowTemplateQuery>();
        services.AddScoped<WorkflowTemplateCommand>();
    }
}
```

In [Program.cs](./services/api/Distributed.Workflows.Api/Program.cs#L17), calling the [`AddAppServices()`](./services/common/Distributed.Core/Extensions/ConfigurationExtensions.cs) extension method will identify concrete instances of `ServiceRegistrant` and execute its `Register()` method.

**AddAppServices**

```cs
public static void AddAppServices(this IServiceCollection services)
{
    Assembly? entry = Assembly.GetEntryAssembly();

    if (entry is not null)
    {
        IEnumerable<Assembly> assemblies = entry
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Append(entry)
            .Where(x =>
                x.GetTypes()
                    .Any(IsValidServiceRegistrant)
            );

        IEnumerable<Type>? registrants = assemblies
            .SelectMany(x =>
                x.GetTypes()
                    .Where(IsValidServiceRegistrant)
            );

        if (registrants is not null)
            foreach (Type registrant in registrants)
                ((ServiceRegistrant?)Activator.CreateInstance(registrant, services))?.Register();
    }
}
```

**Program.cs**

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppServices();
```

## Entity Controllers

[`EntityController`](./services/common/Distributed.Core/Controllers/EntityController.cs) is an abstract Web API Controller that exposes standard `EntityQuery<T>` and `EntityCommand<T>` methods:

```cs
using Distributed.Core.Schema;
using Distributed.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Core.Controllers;
public abstract class EntityController<T,TQuery,TCommand> : ApiController
where T : Entity
where TQuery : IQuery<T>
where TCommand : ICommand<T>
{
    protected IQuery<T> baseQuery;
    protected ICommand<T> baseCommand;

    public EntityController(IQuery<T> query, ICommand<T> command)
    {
        baseQuery = query;
        baseCommand = command;
    }

    [HttpGet("[action]")]
    public virtual async Task<IActionResult> Get() =>
        ApiResult(await baseQuery.Get());

    [HttpGet("[action]/{id:int}")]
    public virtual async Task<IActionResult> GetFromId([FromRoute]int id) =>
        ApiResult(await baseQuery.GetById(id));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> ValidateValue([FromBody]T entity) =>
        ApiResult(await baseCommand.ValidateValue(entity));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Validate([FromBody]T entity) =>
        ApiResult(await baseCommand.Validate(entity));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Save([FromBody]T entity) =>
        ApiResult(await baseCommand.Save(entity));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Remove([FromBody]T entity) =>
        ApiResult(await baseCommand.Remove(entity));
}
```

Instances of `EntityController` just need to specify the generic types associated with the instance, inject the `Query` and `Command` services and pass them to `base`, and define any additional methods that need to be exposed:

**ProposalController**

```cs
using Distributed.Core.Controllers;
using Distributed.Proposals.Logic;
using Distributed.Proposals.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Proposals.Api.Controllers;

[Route("api/[controller]")]
public class ProposalController : EntityController<Proposal, ProposalQuery, ProposalCommand>
{
    readonly ProposalQuery query;

    public ProposalController(ProposalQuery query, ProposalCommand command)
    : base(query, command)
    {
        this.query = query;
    }

    [HttpGet("[action]/{id:int}")]
    public async Task<IActionResult> GetStatus([FromRoute]int id) =>
        ApiResult(await query.GetStatus(id));
}
```

## Gateways

A gateway represents a public interface into a service that facilitates cross-service interaction. The Gateway API provides infrastructure for defining the exposed Gateway and interfacing with the exposed Gateway.

Common Gateway Infrastructure:

* A [GatewayOptions](./services/common/Distributed.Core/Gateway/GatewayOptions.cs) Options pattern record for structuring Gateway configuration metadata. At a minimum, `Gateway.Id` must be provided.
    * Gateway configuration is defined in [`appsettings`](./services/api/Distributed.Workflows.Api/appsettings.Development.json).
* A [GatewayService](./services/common/Distributed.Core/Gateway/GatewayService.cs) class that exposes gateway configuration information.

Defining Gateway Infrastructure:

* An abstract [Controller](./services/common/Distributed.Core/Gateway/GatewayControllerBase.cs) that exposes public interface for the service.

Interfacing Gateway Infrastructure:

* An abstract [GatewayClient](./services/common/Distributed.Core/Gateway/GatewayClient.cs) HTTP service that defines client calls to Gateway controller endpoints.

### Implementing a Gateway

The **Workflows** service defines a [`GatewayController`](./services/api/Distributed.Workflows.Api/Controllers/GatewayController.cs) that allows interactions from external services through the [`Package`](./services/common/Distributed.Contracts/Classes/Package.cs) contract entity.

```cs
using Distributed.Contracts;
using Distributed.Core.Gateway;
using Distributed.Workflows.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Workflows.Api.Controllers;
public class GatewayController : GatewayControllerBase
{
    readonly PackageQuery packageQuery;
    readonly PackageCommand packageCommand;

    public GatewayController(
        GatewayService gateway,
        PackageQuery packageQuery,
        PackageCommand packageCommand
    )
    : base(gateway)
    {
        this.packageQuery = packageQuery;
        this.packageCommand = packageCommand;
    }

    [HttpGet("[action]/{id:int}/{type}")]
    public async Task<IActionResult> GetPackage(
        [FromRoute] int id,
        [FromRoute] string type
    ) => ApiResult(await packageQuery.GetByEntity(id, type));

    [HttpPost("[action]")]
    public async Task<IActionResult> ValidatePackage([FromBody] Package package) =>
        ApiResult(await packageCommand.Validate(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> SubmitPackage([FromBody] Package package) =>
        ApiResult(await packageCommand.Save(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> WithdrawPackage([FromBody] Package package) =>
        ApiResult(await packageCommand.Remove(package));
}
```

To provide external access to this API, a `GatewayClient` instance is defined at [`./services/common/Distributed.Contracts/Gateways/WorkflowsGateway.cs`](./services/common/Distributed.Contracts/Gateways/WorkflowsGateway.cs):

```cs
using Distributed.Core.Gateway;
using Distributed.Core.Messages;

namespace Distributed.Contracts.Gateways;
public class WorkflowsGateway : GatewayClient
{
    public WorkflowsGateway(GatewayService gateway)
    : base(gateway, "Workflows")
    { }

    public async Task<Package?> GetPackage(int id, string type) =>
        await Get<Package?>($"getPackage/{id}/{type}");

    public async Task<ValidationMessage?> ValidatePackage(Package package) =>
        await Post<ValidationMessage, Package>(package, "validatePackage");

    public async Task<ApiMessage<Package>?> SubmitPackage(Package package) =>
        await Post<ApiMessage<Package>, Package>(package, "submitPackage");

    public async Task<ApiMessage<int>?> WithdrawPackage(Package package) =>
        await Delete<ApiMessage<int>, Package>(package, "withdrawPackage");
}
```

Gateway clients can be registered by interfacing services by defining a [`Registrant`](./services/api/Distributed.Proposals.Api/Registrant.cs) that registers all `GatewayClient` services:

```cs
using Distributed.Contracts.Gateways;
using Distributed.Core.Services;

namespace Distributed.Proposals.Api;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddSingleton<WorkflowsGateway>();
    }
}
```