# Distributed Design

> Design in progress

The intent of this repository is to identify ways of alleviating pain points associated with developing a monolithic on-premise application. One of the goals of this architecture is to take as few dependencies on external services as possible. All backend services are written with standard .NET APIs with SQL Server as the underlying data store.

* [services](./services/) - Contains service infrastructure
    * [api](./services/api/) - Contains REST API apps
        * [Distributed.Proposals.Api](./services/api/Distributed.Proposals.Api/) - **Proposals** service API
        * [Distributed.Workflows.Api](./services/api/Distributed.Workflows.Api/) - **Workflows** service API
    * [common](./services/common/) - Contains code libraries for shared service infrastructure
        * [Distributed.Contracts](./services/common/Distributed.Contracts/) - Contains cross-domain infrastructure to faciliate cross-service interaction
        * [Distributed.Core](./services/common/Distributed.Core/) - Contains core infrastructure used to simplify building services
            * [Controllers](./services/common/Distributed.Core/Controllers/) - Contains standardized base classes that simplify the creation of .NET Web API Controllers.
            * [Data](./services/common/Distributed.Core/Data/) - Contains classes that standardize Entity Framework Core interactions
            * [Extensions](./services/common/Distributed.Core/Extensions/) - Contains extensions that provide extended functinoality and standardized configuration
            * [Graph](./services/common/Distributed.Core/Graph/) - Contains infrastructure that facilitates service-to-service HTTP communications through a publicly exposed subset of endpoints in an HTTP client service
            * [Messages](./services/common/Distributed.Core/Messages/) - Contains classes that provide standardized message formats for return values
            * [Schema](./services/common/Distributed.Core/Schema/) - Contains classes and interfaces that simplify building entity models
            * [Sync](./services/common/Distributed.Core/Sync/) - Contains infrastructure that facilitates cross-service data synchronization via SignalR
    * [data](./services/data/) - Contains Entity Framework Core `DbContext` definitions and Migrations
        * [Distributed.Proposals.Data](./services/data/Distributed.Proposals.Data/) - **Proposals** service `ProposalsContext` and corresponding Migrations
        * [Distributed.Workflows.Data](./services/data/Distributed.Workflows.Data/) - **Workflows** service `WorkflowsContext` and corresponding Migrations
    * [logic](./services/logic/) - Contains business logic infrastructure for services
        * [Distriburted.Proposals.Logic](./services/logic/Distributed.Proposals.Logic/) - **Proposals** service business logic
        * [Distributed.Workflows.Logic](./services/logic/Distributed.Workflows.Logic/) - **Workflows** service business logic
    * [schema](./services/schema/) - Contains entity model definitions for services
        * [Distributed.Proposals.Schema](./services/schema/Distributed.Proposals.Schema/) - **Proposals** service entity models
        * [Distributed.Workflows.Schema](./services/schema/Distributed.Workflows.Schema/) - **Workflows** service entity models