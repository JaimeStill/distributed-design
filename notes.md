# Notes

## Supporting Service Infrastructure

There are two APIs in the code base that facilitate cross-service interactions and synchronization:

* The [Graph API](./services/common/Distributed.Core/Graph/) enables the registration of an `HttpClient`-based service for accessing a sub-set of interactions available on its corresponding service.

* The [Sync API](./services/common/Distributed.Core/Sync/) enables SignalR-based services that define data synchronization endpoints that broadcast messages whenenver associated events occur. For instance, notification of whenever an entity is modified. It is the primary facilitator of the **Events** infrastructure outlined below.

## Service Infrastructure Responsibilities

This section defines a localized approach to defining service infrastructure inspired by the [Command Query Responsiblity Segregation (CQRS)](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/cqrs-pattern.html), [Event Sourcing](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/service-per-team.html), and [Saga](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/saga-pattern.html) patterns.

The following terms are used to break what is the traditional role of API-based services into smaller, more focused responsiblities:

* **Queries** - Data retrieval methods; represented by the standard data accessors from traditional API services. Exist as [Service classes](./services/common/Distributed.Core/Services/EntityQuery.cs).

* **Commands** - User-based, public data mutation methods; represented by the standard **Save** and **Delete** methods of API services. Exist as [Service classes](./services/common/Distributed.Core/Services/EntityCommand.cs).

* **Events** - [SignalR hubs](./services/common/Distributed.Core/Hubs/EntityEventHub.cs) used for broadcasting data mutations. Injected into **Commands**. Clients can also listen for events to determine whether the updates affect their current data context; if so, the data can be refreshed to ensure consumers are always working with the most up-to-date information.

* **Sagas** - System-based, private data mutation methods; they isolate reactionary logic for determining how to handle effects of mutations triggered through **Events**.

This approach is important because it creates isolated boundaries around service responsiblities:
* All data retrieval concerns associated with a specific data type can be shared across the system without worrying about exposing data manipulation logic.

*  It isolates intentional and reciprocal modifications from each other. By exposing **Commands** as intentional, user-driven data mutations and **Sagas** as reciprocal, system-driven mutations:

    * **Commands** allow user-driven data mutations to be exposed without having to consider all of the after-effects that should occur as a result

    * **Sagas** provide a single place for handling after-effects that do not require the same scrutiny (authentication / authorization) as **Commands**. They are private, internally managed mutations that facilitate recursive reactions across the whole data dependency hierarchy.
    
## Sagas

The concept of the Saga needs to be developed further. Initially, I thought I might be able to build out the Saga as a `SyncClient` with embedded functionality, but it seems that there ought to be a separation of those responsibilities.

Final ideas for this before the weekend:

* There should be an `EventListener` that intercepts sync events originated from an `EntityEventHub`. This is the `SyncClient` that alerts the service when data has been mutated that affects the state of the service.

* The `Saga` should be defined as a standard service class that defines data mutation methods. The `Saga` should be aware of how to execute all direct mutations resulting from the event and trigger additional events as a result of these mutations. The process completes until the generated events require no more data mutations. 

***The `Saga` should only be concerned with data encapsulated in the service that defines it. All services should be self-sufficient with reacting to events.***