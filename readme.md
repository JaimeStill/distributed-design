# Distributed Design

* [services](./services/) - Contains service infrastructure
    * [api](./services/api/) - Contains REST API apps
        * [Distributed.Proposals.Api](./services/api/Distributed.Proposals.Api/) - **Proposals** service API
        * [Distributed.Workflows.Api](./services/api/Distributed.Workflows.Api/) - **Workflows** service API
    * [common](./services/common/) - Contains code libraries for shared service infrastructure
        * [Distributed.Contracts](./services/common/Distributed.Contracts/) - Contains cross-domain infrastructure to faciliate cross-service interaction
        * [Distributed.Core](./services/common/Distributed.Core/) - Contains core infrastructure used to simplify building services
            * [Data](./services/common/Distributed.Core/Data/) - Contains classes that standardize Entity Framework Core interactions
            * [Extensions](./services/common/Distributed.Core/Extensions/) - Contains extensions that provide extended functinoality and standardized configuration
            * [Schema](./services/common/Distributed.Core/Schema/) - Provides classes and interfaces that simplify building entity models
    * [data](./services/data/) - Contains Entity Framework Core `DbContext` definitions and Migrations
        * [Distributed.Proposals.Data](./services/data/Distributed.Proposals.Data/) - **Proposals** service `ProposalsContext` and corresponding Migrations
        * [Distributed.Workflows.Data](./services/data/Distributed.Workflows.Data/) - **Workflows** service `WorkflowsContext` and corresponding Migrations
    * [logic](./services/logic/) - Contains business logic infrastructure for services
        * [Distriburted.Proposals.Logic](./services/logic/Distributed.Proposals.Logic/) - **Proposals** service business logic
        * [Distributed.Workflows.Logic](./services/logic/Distributed.Workflows.Logic/) - **Workflows** service business logic
    * [schema](./services/schema/) - Contains entity model definitions for services
        * [Distributed.Proposals.Schema](./services/schema/Distributed.Proposals.Schema/) - **Proposals** service entity models
        * [Distributed.Workflows.Schema](./services/schema/Distributed.Workflows.Schema/) - **Workflows** service entity models