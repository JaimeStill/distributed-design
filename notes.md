# Notes

## Todo

* DevContainer Setup
    * `appsettings.Codespaces.json`
    * **.devcontainer**
    * Procedures for once the container is built and the post create command has finished executing
* JsonExceptionHandler Middleware
* Docker
* Command Hooks and Transactions
* npm link
* SignalR Diagnostics
* Gateway Example
* Saga + EventListener Example
* Saga EventHub Synchronization

## Saga EventHub Synchronization

Trigger events from within a Saga if internal data is mutated in reaction to external events. See PackageSaga in the proposals node.

## Library-based Contract Event Listeners

Document the process of creating contract-based event listeners within a library that can be imported within separate apps.

## Gateway Example

> When writing a `GatewayController`, methods should return `GatewayResult` vs. `ApiResult`. This will ensure that the intended value is delivered and the result can be processed by the node executing the endpoint.

Demonstrated withdrawing the active `Package` for a `Proposal` whenever the `Proposal` is removed.

See: `ProposalCommand.AfterRemove`.

## Saga + EventListener Example

Set the `PackageId` of a `Proposal` after an associated `Package` has been added or removed.

See:

* `PackageSaga`
* `Registrant`
* `appsettings.*.json`
* `IPackageEventListener`
* `PackageEventListener`
* `PackageEventStartup`
* `EventRegistration`
* `Program`

## SignalR Diagnostics

* appsettings.json for Server
* HubConnectionBuilder.ConfigureLogging in EventClient

Make sure the types of the provided values match the types of the hub method being invoked. ***Deserialization of interface types is not supported!***

## npm link

```bash
# from source code dist
npm link

# in consuming project
npm link [package]
npm i [path-to-package-dist] # ../libs/distributed/dist/toolkit/

# list all globally linked packages
npm ls -g --depth=0 --link=true

# unlink a package
npm unlink -g [package]
```

In `tsconfig.json`, ensure all linked non-Angular libraries have `path` values that point to their `dist` folders to ensure proper change detection:

```json
"paths": {
    "@distributed/core": [
        "../libs/core/dist/*"
    ]
}
```

## Integrate Local NPM Modules With Standalone Local Angular Libraries

When I wrote the [Local npm Packages](https://jaimestill.github.io/azure-dev-resources/npm.html#local-npm-packages) documentation, I did not consider the additional details that would be needed for integration into a standalone local Angular library.

The following section will outline how to setup an npm package, an Angular library in an isolated workspace that uses the package, and an Angular app that uses the Angular library as an npm package.

### npm Package Setup

```bash
mkdir [package]
cd [package]
npm init
```

**package.json**

```jsonc
{
    "name": "@scope/[package]",
    "version": "0.0.1",
    "description": "Package description",
    "main": "index.ts",
    "type": "module",
    "types": "dist/index.d.ts",
    "scripts": {
        "build": "tsc",
        "watch": "tsc --watch"
    },
    // additional configuration
}
```

**tsconfig.json**

```jsonc
{
    "compileOnSave": false,
    "compilerOptions": {
        "moduleResolution": "node",
        "target": "ES2022",
        "module": "ES2022",
        "rootDir": "./src",
        "outDir": "./dist",
        "declaration": true,
        "esModuleInterop": false,
        "forceConsistentCasingInFileNames": true,
        "strict": true
    }
}
```

**Build**

```bash
npm i
npm run build
```

### Angular Library Setup

> It is important to note that while the library within the Angular workspace has a package.json, you should not run `npm install` for the library. It is purely there to generate the ES module build with **ng-packagr**.

```bash
npm i -g @angular/cli

ng new [workspace] `
    --package-manager npm `
    --minimal `
    --no-create-application `
    --skip-git

cd [workspace]

npm i ../[package] -s

ng generate library [name] --entry-file index.ts
```

**Workspace - package.json**

```jsonc
{
    "name": "[workspace]",
    "version": "0.0.0",
    "private": "true",
    "dependencies": {
        "@scope/[package]": "file../[package]",
        // additional deps
    },
    // additional config
}
```

**Workspace - tsconfig.json**

```jsonc
{
    "compileOnSave": false,
    "compilerOptions": {
        "baseUrl": "./",
        "outDir": "./dist/out-tsc",
        "forceConsistentCasingInFileNames": true,
        "strict": true,
        "noImplicitOverride": true,
        "noPropertyAccessFromIndexSignature": true,
        "noImplicitReturns": true,
        "noFallthroughCasesInSwitch": true,
        "sourceMap": true,
        "declaration": false,
        "downlevelIteration": true,
        "experimentalDecorators": true,
        "moduleResolution": "node",
        "importHelpers": true,
        "target": "ES2022",
        "module": "ES2022",
        "useDefineForClassFields": false,
        "paths": {
            "[name]": [
                "dist/[name]"
            ]
        },
        "lib": [
            "ES2022",
            "dom"
        ]
    },
    "angularCompilerOptions": {
        "enableI18nLegacyMessageIdFormat": false,
        "strictInjectionParameters": true,
        "strictInputAccessModifiers": true,
        "strictTemplates": true
    }
}
```

**Workspace - angular.json**

```jsonc
{
    "$schema": "./node_modules/@angular/cli/lib/config/schema.json",
    "version": 1,
    "cli": {
        "packageManager": "npm"
    },
    "newProjectRoot": "projects",
    "projects": {
        "[name]": {
            "projectType": "library",
            "root": "projects/[name]",
            "sourceRoot": "projects/[name]/src",
            "prefix": "lib",
            "architect": {
                "build": {
                    "builder": "@angular-devkit/build-angular:ng-packagr",
                    "options": {
                        "project": "projects/[name]/ng-package.json"
                    },
                    "configurations": {
                        "production": {
                        "tsConfig": "projects/[name]/tsconfig.lib.prod.json"
                        },
                        "development": {
                        "tsConfig": "projects/[name]/tsconfig.lib.json"
                        }
                    },
                    "defaultConfiguration": "production"
                }
            }
        }
    }
}
```

**Library - ng-package.json**

```jsonc
{
    "$schema": "../../node_modules/ng-packagr/ng-package.schema.json",
    "dest": "../../dist/[name]",
    "lib": {
        "entryFile": "src/index.ts"
    }
}
```

**Library - package.json**

```jsonc
{
    "name": "@scope/[name]",
    "version": "0.0.1",
    "peerDependencies": {
        "@scope/[package]": "file:../../../[package]",
        // additional dependencies
    },
    "dependencies": {
        "tslib": "[version]"
    },
    "sideEffects": false
}
```

**Build**

> Conducted from workspace root

```bash
npm i
npm run build
```

### Angular App Integration

The following steps must be conducted on every Angular application that intends to use the Angular library.

Within the Angular app workspace:

```bash
# install NPM package @scope/[package]
# listed as peerDependency for 
# Angular library @scope/[name]
npm i [path-to-@scope/[package]] -s

# install Angular library @scope/[name]
npm i [path-to-@scope/[name]] -s
```

Configure the following settings in the following files:

**tsconfig.json**

```jsonc
{
    "paths": {
        "@scope/[package]": [
            "node_modules/@scope/[package]/dist/*"
        ]
    },
    // additional configuration
}
```

**angular.json**

```jsonc
{
    "projects": {
        "[app]" {
            "architect": {
                "build": {
                    "options": {
                        "preserveSymlinks": true,
                        "sourceMap": {
                            "scripts": true,
                            "styles": true,
                            "vendor": true
                        }
                    }
                }
            }
        }
    }
    // additional configuration
}
```

**Build**

```bash
npm run build
# or
npm run start
```

## Docker

```bash
# proposals node - run from ./nodes
docker build -t proposals-api -f ./proposals/Proposals.Api/Dockerfile .
docker run -it --rm -p 5001:80 propsals-api
docker rmi proposals-api

# workflows node - run from ./nodes
docker build -t workflows-api -f ./workflows/Workflows.Api/Dockerfile .
docker run -it --rm -p 5002:80 workflows-api
docker rmi workflows-api
```

## Configuration - Options Pattern

When you have a standardized JSON configuration pattern, you can implement the [Options Pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0) to simplify the process of reading configuration. This way, instead of injecting `IConfiguration` and trying to read the configuration directly, you can just inject the configuration directly.

The **Gateway API** uses this pattern via the [`GatewayOptions`](./nodes/common/Distributed.Core/Gateway/GatewayOptions.cs) record and the [`ConfigureGatewayOptions`](./nodes/common/Distributed.Core/Extensions/ConfigurationExtensions.cs#L62) extension, as implemented in the [Proposals](./nodes/api/Distributed.Proposals.Api/Program.cs#L11) and [Workflows](./nodes/api/Workflows.Api/Program.cs#L11) services.

### Configuration - Events

Because the configuration pattern for the **Events API** is not standard (the names of the services in the configuration section will differ depending on the service being configured), it cannot implement the Options pattern. Instead, a [`GetEventEndpoint`](./nodes/common/Distributed.Core/Extensions/ConfigurationExtensions.cs#L67) extension method has been written against `IConfiguration` that standardizes the process of retrieving an EventHub endpoint.

## Global Enum String Conversion Configuration

To simplify the storage and transport of [enum-based actions and states](./nodes/common/Distributed.Contracts/Enums/), [EntityContext](./nodes/common/Distributed.Core/Data/EntityContext.cs#L27) and [JsonSerializerOptions](./nodes/common/Distributed.Core/Extensions/ConfigurationExtensions.cs#L19) are globally configured to convert enums to and from strings.

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

* [**Queries**](./nodes/common/Distributed.Core/Services/EntityQuery.cs) - Data retrieval methods; represented by the standard data accessors from traditional API services.

* [**Commands**](./nodes/common/Distributed.Core/Services/EntityCommand.cs) - User-based, public data mutation methods; represented by the standard **Save** and **Delete** methods of API services.

* [**Sagas**](./nodes/common/Distributed.Core/Services/EntitySaga.cs) - System-based, private data mutation methods; they isolate reactionary logic for determining how to handle effects of mutations triggered through **Events**. The **Saga** should only be concerned with the data encapsulated in the service that defines it. All services should be self-sufficient with reacting to events.

* [**Events**](./nodes/common/Distributed.Core/Services/Events/) - Events are comprised of two components: the [EventHub](./nodes/common/Distributed.Core/Services/Events/Hub/EntityEventHub.cs) which is used to broadcast when data has been mutated, and the [EventListener](./nodes/common/Distributed.Core/Services/Events/Client/EntityEventListener.cs) which is used to execute corresponding **Saga** methods to handle the follow on effects of the data mutation. **EventHubs** are injected into **Commands** and broadcast messages within *Sync{x}* events on the **Command** service.

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

To keep API configuration clean and simplify the process of registering standard API services, instances of the [ServiceRegistrant](./nodes/common/Distributed.Core/Services/ServiceRegistrant.cs) class handle API service registration.

The library defining API service classes will also define a [Registrant](./nodes/logic/Workflows.Services/Registrant.cs) that specifies how to register the services.

```cs
using Distributed.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Workflows.Services;
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

In [Program.cs](./nodes/api/Workflows.Api/Program.cs#L17), calling the [`AddAppServices()`](./nodes/common/Distributed.Core/Extensions/ConfigurationExtensions.cs) extension method will identify concrete instances of `ServiceRegistrant` and execute its `Register()` method.

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

[`EntityController`](./nodes/common/Distributed.Core/Controllers/EntityController.cs) is an abstract Web API Controller that exposes standard `EntityQuery<T>` and `EntityCommand<T>` methods:

```cs
using Distributed.Core.Entities;
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
using Distributed.Proposals.Services;
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

* A [GatewayOptions](./nodes/common/Distributed.Core/Gateway/GatewayOptions.cs) Options pattern record for structuring Gateway configuration metadata. At a minimum, `Gateway.Id` must be provided.
    * Gateway configuration is defined in [`appsettings`](./nodes/api/Workflows.Api/appsettings.Development.json).
* A [GatewayService](./nodes/common/Distributed.Core/Gateway/GatewayService.cs) class that exposes gateway configuration information.

Defining Gateway Infrastructure:

* An abstract [Controller](./nodes/common/Distributed.Core/Gateway/GatewayControllerBase.cs) that exposes public interface for the service.

Interfacing Gateway Infrastructure:

* An abstract [GatewayClient](./nodes/common/Distributed.Core/Gateway/GatewayClient.cs) HTTP service that defines client calls to Gateway controller endpoints.

### Implementing a Gateway

The **Workflows** service defines a [`GatewayController`](./nodes/api/Workflows.Api/Controllers/GatewayController.cs) that allows interactions from external services through the [`Package`](./nodes/common/Distributed.Contracts/Classes/Package.cs) contract entity.

```cs
using Distributed.Contracts;
using Distributed.Core.Gateway;
using Workflows.Services;
using Microsoft.AspNetCore.Mvc;

namespace Workflows.Api.Controllers;
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

To provide external access to this API, a `GatewayClient` instance is defined at [`./nodes/common/Distributed.Contracts/Gateways/WorkflowsGateway.cs`](./nodes/common/Distributed.Contracts/Gateways/WorkflowsGateway.cs):

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

Gateway clients can be registered by interfacing services by defining a [`Registrant`](./nodes/api/Distributed.Proposals.Api/Registrant.cs) that registers all `GatewayClient` services:

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

