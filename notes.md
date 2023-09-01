# Notes

## Supporting Service Infrastructure

There are two APIs in the code base that facilitate cross-service interactions and synchronization:

* The [Graph API](./services/common/Distributed.Core/Graph/) enables the registration of an `HttpClient`-based service for accessing a sub-set of interactions available on its corresponding service.

* The [Sync API](./services/common/Distributed.Core/Sync/) enables SignalR-based services that define data synchronization endpoints that broadcast messages whenenver associated events occur. For instance, notification of whenever an entity is modified. It is the primary facilitator of the **Events** infrastructure outlined below.

## Service Infrastructure Responsibilities

This section defines a localized approach to defining service infrastructure inspired by the [Command Query Responsiblity Segregation (CQRS)](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/cqrs-pattern.html), [Event Sourcing](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/service-per-team.html), and [Saga](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/saga-pattern.html) patterns.

The following terms are used to break what is the traditional role of API-based services into smaller, more focused responsiblities:

* **Queries** - Data retrieval methods; represented by the standard data accessors from traditional API services.

* **Commands** - User-based, public data mutation methods; represented by the standard **Save** and **Delete** methods of API services.

* **Events** - Web Socket listeners used for broadcasting data mutations. Clients can also listen for events to determine whether the updates affect their current data context; if so, the data can be refreshed to ensure consumers are always working with the most up-to-date information.

* **Sagas** - System-based, private data mutation methods; They isolate reactionary logic for determining how to handle effects of mutations triggered through **Events**.

This approach is important because it creates isolated boundaries around service responsiblities:
* All data retrieval concerns associated with a specific data type can be shared across the system without worrying about exposing data manipulation logic.

*  It isolates intentional and reciprocal modifications from each other. By exposing **Commands** as intentional, user-driven data mutations and **Sagas** as reciprocal, system-driven mutations:

    * **Commands** allow user-driven data mutations to be exposed without having to consider all of the after-effects that should occur as a result

    * **Sagas** provide a single place for handling after-effects that do not require the same scrutiny (authentication / authorization) as **Commands**. They are private, internally managed mutations that facilitate recursive reactions across the whole data dependency hierarchy.

## Service Workflow

1. Sagas receive event sync clients for all relevant data in its dependency tree and register listeners during initialization
2. Command methods trigger Event syncs
3. Sagas execute methods in reaction to event syncs, which trigger further Saga method executions. This step completes recursively until the full data dependency tree is complete.