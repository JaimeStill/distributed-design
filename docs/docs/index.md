---
sidebar_position: 1
slug: /
title: Overview
---

# Distributed Design

:::caution

This repository is a work in progress. The current APIs and documentation are subject to change. See the [GitHub Project](https://github.com/users/JaimeStill/projects/6) to track progress to initial concept.

See [Notes](https://github.com/JaimeStill/distributed-design/blob/main/notes.md) for info that hasn't made it into the docs yet.

:::

This repository demonstrates a .NET microservice API (each microservice is referred to as a **node**) that takes as few external dependencies as possible. SQL Server serves as the underlying storage mechanism.

In addition to standard data retrieval and mutations, this architecture also focuses on easily enabling cross-node communication and distributed real time data synchronization. It attempts to provide a minimal and easy to incorporate approach to defining microservice infrastructure inspired by the [Command Query Responsibility Segregation (CQRS)](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/cqrs-pattern.html), [Event Sourcing](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/service-per-team.html), and [Saga](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/saga-pattern.html) patterns.

## Terminology

This project uses the following terminology to describe different facets of its infrastructure:

Term | Definition | Example
-----|------------|--------
**API** | A .NET Web API project that establishes HTTP and Web Socket endpoints for a **Node**. | [Proposals.Api](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals/Proposals.Api)
**App** | A client that interfaces with one or more **Nodes**. | Pending
**Command** | A **Service** that defines user-driven public data mutation methods. | [`ProposalCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs)
**Contract** | An **Entity** that facilitates interaction between **Nodes**. A **Contract** can be thought of as a *public* **Entity**. | [`Package`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/Classes/Package.cs)
**Entity** | A data model represented by an underlying store table that encapsulates an aspect of the associated **Node** domain. | [`Proposal`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Entities/Proposal.cs)
**EventHub** | A strongly-typed SignalR `Hub` that broadcasts **Entity**-based mutation events. | <ul><li>Interface: [`IPackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Event/IPackageEventHub.cs)</li><li>Hub: [`PackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Event/PackageEventHub.cs)</li></ul>
**EventListener** | A SignalR client that facilitates reacting to  **Entity**-based mutation events broadcast by an **EventHub**. | Pending
**Gateway** | The publicly-exposed API interface defined by a **Node** that facilitates cross-node interaction. | <ul><li>API: [`GatewayController`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Api/Controllers/GatewayController.cs)</li><li>Client: [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/WorkflowsGateway.cs)</li><li>Configuration: [`appsettings.json`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Api/appsettings.Development.json#L11)</li></ul>
**Hook** | An asynchronous delegate function that allows logic to be executed at critical moments within a **Command** action. | <ul><li>Definition: [`AfterAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L39)</li><li>Implementation: [`AfterAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs#L17)</li></ul>
**Node** | A microservice backend that encapsulates a single micro domain. This term was selected in place of ***Service*** to avoid confusion. **Service** is an overloaded term that already carries a different definition in this architecture. | [proposals](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals)
**Query** | A **Service** that facilitates retrieval of **Entity** data. | [`ProposalQuery`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Query/ProposalQuery.cs)
**Saga** | A **Service** that defines system-drive private data mutation methods; they isolate reactionary logic for determining how to handle effects of mutations triggered through **Events**. | Pending
**Service** | A class that exposes functionality and is registered with the .NET Dependency Injection service container. **Query**, **Command**, **EventListener**, and **Saga** are all examples of services with a targeted intent. | [`GatewayService`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Gateway/GatewayService.cs)

## Layout

The layout of the repository is expressed as follows:

* [docs](https://github.com/JaimeStill/distributed-design/tree/main/docs) - Repository documentation, hosted at [https://jaimestill.github.io/distributed-design](https://jaimestill.github.io/distributed-design).
* [nodes](https://github.com/JaimeStill/distributed-design/tree/main/nodes) - All **Node**-related infrastructure.
    * [contracts](https://github.com/JaimeStill/distributed-design/tree/main/nodes/contracts) - Projects that define publicly available **Entity** classes and their underlying infrastructure.
        * [core](https://github.com/JaimeStill/distributed-design/tree/main/nodes/contracts/core) - Core contracts not associated with a specific **Node**.
        * [workflows](https://github.com/JaimeStill/distributed-design/tree/main/nodes/contracts/workflows) - Contracts associated with the [workflows](https://github.com/JaimeStill/distributed-design/tree/main/nodes/workflows) **Node**.
    * [core](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core) - Class library that contains all of the core infrastructure for building out a **Node**.
        * [Controllers](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Controllers) - Abstract [Controller](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-7.0#controllerbase-class) definitions.
        * [Data](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Data) - Infrastructure for building out Entity Framework Core related features.
        * [Entities](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Entities) - Core classes and interfaces for building **Entity** classes for a **Node**.
        * [Extensions](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Extensions) - Extension methods that provide enhanced functionality and / or standardizes **Node** configuration.
        * [Gateway](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Gateway) - Infrastructure for building a **Gateway** within a **Node**.
        * [Messages](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Messages) - Return types that wrap a data type with metadata about the results of the operation that was performed.
        * [Middleware](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Middleware) - Contains **Node** specific [middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0).
        * [Services](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Services) - Core classes and interfaces for building **Command**, **Query**, **Event**, and **Saga** services for a **Node**.
    * [proposals](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals) - A simple demonstration **Node** that facilitates the development of proposals.
    * [workflows](https://github.com/JaimeStill/distributed-design/tree/main/nodes/workflows) - A simple demonstration **Node** that facilitates the staffing of packages through pre-defined workflows. Packages are generated from external services through the [`Package`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/Classes/Package.cs) contract via the [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/WorkflowsGateway.cs).