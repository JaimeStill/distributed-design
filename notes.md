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

## Todo

* Document EF Core Enum Configuration
    * [ConfigureConventions](https://github.com/JaimeStill/DistributedDesign/commit/2f810160cfee4ac1bea68502f0f4a07aa0194ff1#diff-e58b26a4e69dee8a555436129fc258bc9cec011d337ba22e309d43792abf5a4c)
    * [Status.OnSaving](https://github.com/JaimeStill/DistributedDesign/commit/2f810160cfee4ac1bea68502f0f4a07aa0194ff1#diff-f02680279535cf9c6ecd0c89c8dd992278da2b8962df776df4aecf5cc61e5332)
* Document Service Registration
    * [ServiceRegistrant](https://github.com/JaimeStill/DistributedDesign/commit/2f810160cfee4ac1bea68502f0f4a07aa0194ff1#diff-0a8486889f9e5b0cc4049b711e00a3bf21d3e4f88bf97b282e322104ac4d4bda)
    * [AddAppServices](https://github.com/JaimeStill/DistributedDesign/commit/2f810160cfee4ac1bea68502f0f4a07aa0194ff1#diff-e131aa38cff2a77aaf59e4ac33a0f024ad35fe769aa5f582163800cbadfb2b1c)
    * [Registrant](https://github.com/JaimeStill/DistributedDesign/commit/2f810160cfee4ac1bea68502f0f4a07aa0194ff1#diff-a4ff448e1728f82d577cbf54f1a90fe9bc545dd38b586922c19554f12b1785c7)
* Document Entity Controller
    * [EntityController](https://github.com/JaimeStill/DistributedDesign/commit/2f810160cfee4ac1bea68502f0f4a07aa0194ff1#diff-901a41bad148d5b20815b8d0e87dd0b2e4c2c084c180516834e50657baca023c)
    * [ProposalController](https://github.com/JaimeStill/DistributedDesign/commit/2f810160cfee4ac1bea68502f0f4a07aa0194ff1#diff-12b8baaf39725801084ba0add78d1a0c3914f06c1ec72ce8f618133f43213610)