# Notes

## Supporting Service Infrastructure

There are two APIs in the code base that facilitate cross-service interactions and synchronization:

* The [Gateway API](./services/common/Distributed.Core/Gateway/) enables the registration of an `HttpClient`-based service for accessing a sub-set of interactions available on its corresponding service.

* The [Sync API](./services/common/Distributed.Core/Sync/) enables SignalR-based services that define data synchronization endpoints that broadcast messages whenenver associated events occur. For instance, notification of whenever an entity is modified. It is the primary facilitator of the **Events** infrastructure outlined below.

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