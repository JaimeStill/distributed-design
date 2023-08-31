# Distributed Design

* [common](./common/) - Contains code libraries shared across apps and services
    * [Distributed.Contracts](./common/Distributed.Contracts/) - Contains classes and enumerations used across microservices.
    * [Distributed.Core](./common/Distributed.Core/) - Contains core infrastructure used to simplify building microservices.
        * [Data](./common/Distributed.Core/Data/) - Contains classes that standardize Entity Framework interactions
        * [Extensions](./common/Distributed.Core/Extensions/) - Provides extensions that provide extended functionality and standardized configuration
        * [Schema](./common/Distributed.Core/Schema/) - Provides classes and interfaces for that simplify building entity models.
* [services](./services/) - Contains Web API microservices
    * [Distributed.Proposals](./services/Distributed.Proposals/) - Create and manage stateful Proposals. Proposal state can only be modified by submitting a `Package` to the `Workflows` microservice.
    * [Distributed.Workflows](./services/Distributed.Workflows/) - Build and action collaborative organizational workflows. Centralizes the organizational decision making process.