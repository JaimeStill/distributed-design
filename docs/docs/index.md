---
sidebar_position: 1
slug: /
title: Overview
---

# Distributed Design

This repository demonstrates a minimal .NET microservice framework (each microservice is referred to as a **node**) that takes as few external dependencies as possible. In addition to standard data retrieval and mutations, this architecture also focuses on easily enabling cross-node communication and distributed real time data synchronization. SQL Server serves as the underlying storage mechanism through Entity Framework. SignalR is used for real time data synchronization.

## Terminology

This project uses the following terminology to describe different facets of its infrastructure:

Term | Definition | Example
-----|------------|--------
**API** | A .NET Web API project that provides a configuration point and establishes HTTP and Web Socket endpoints for a **Node**. | [`Proposals.Api`](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals/Proposals.Api)
**App** | A client that interfaces with one or more **Nodes**. | [`proposals`](https://github.com/JaimeStill/distributed-design/tree/main/apps/proposals)
**Command** | A **Service** that defines user-driven public data mutation methods. | [`ProposalCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs)
**Contract** | An **Entity** that facilitates interaction between **Nodes**. A **Contract** can be thought of as a *public* **Entity**. | [`Package`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/Classes/Package.cs)
**Entity** | A data model represented by an underlying store table that encapsulates an aspect of the associated **Node** domain. | [`Proposal`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Entities/Proposal.cs)
**EventHub** | A strongly-typed SignalR `Hub` that broadcasts **Entity**-based mutation events. | <ul><li>Interface: [`IPackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Event/IPackageEventHub.cs)</li><li>Hub: [`PackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Event/PackageEventHub.cs)</li></ul>
**EventListener** | A SignalR client that facilitates reacting to **Contract**-based mutation events broadcast by an **EventHub**. | <ul><li>.NET: [`PackageListener`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Event/Listeners/PackageEventListener.cs)</li><li>TypeScript: [`PackageListener`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/distributed/projects/toolkit/src/nodes/package-listener.ts)</li></ul>
**Gateway** | The publicly-exposed API interface defined by a **Node** that facilitates cross-node interaction. | <ul><li>API: [`GatewayController`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Api/Controllers/GatewayController.cs)</li><li>Client: [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/WorkflowsGateway.cs)</li><li>Configuration: [`appsettings.json`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Api/appsettings.Development.json#L14)</li></ul>
**Hook** | An asynchronous delegate function that allows logic to be executed at critical moments within a **Command** action. | <ul><li>Definition: [`AfterAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L39)</li><li>Implementation: [`AfterAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs#L26)</li></ul>
**Node** | A microservice backend that encapsulates a single micro domain. This term was selected in place of ***Service*** to avoid confusion. **Service** is an overloaded term that already carries a different definition in this architecture. | [`proposals`](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals)
**Query** | A **Service** that facilitates retrieval of **Entity** data. | [`ProposalQuery`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Query/ProposalQuery.cs)
**Saga** | A **Service** that defines system-driven private data mutation methods; they isolate reactionary logic for determining how to handle effects of mutations triggered through external node **Events**. Typically, a Saga is defined when you want to adjust internal **Entities** in response to changes in associated external **Contracts**. | [`PackageSaga`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Saga/PackageSaga.cs)
**Service** | A class that exposes functionality and is registered with the .NET Dependency Injection service container. **Query**, **Command**, **EventListener**, and **Saga** are all examples of services with a targeted intent. | [`GatewayService`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Gateway/GatewayService.cs)

## Directory Structure

The layout of the repository is expressed as follows:

* [.devcontainer](https://github.com/JaimeStill/distributed-design/tree/main/.devcontainer) - [Dev Container](https://containers.dev/) configuration for running this project in [GitHub Codespaces](https://github.com/features/codespaces).
* [.github/workflows](https://github.com/JaimeStill/distributed-design/tree/main/.github/workflows) - [GitHub Actions](https://github.com/features/actions) workflows for deploying docs.
* [.vscode](https://github.com/JaimeStill/distributed-design/tree/main/.vscode) - [Tasks](https://code.visualstudio.com/docs/editor/tasks) and [Debug](https://code.visualstudio.com/docs/editor/debugging) configurations for VS Code.
* [apps](https://github.com/JaimeStill/distributed-design/tree/main/apps) - All app-related infrastructure.
    * [libs](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs) - Contains local npm libraries that define common infrastructure for app development.
        * [contracts](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs/contracts) - Libraries that define publicly available entity classes and their underlying infrastructure.
            * [core](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs/contracts/core) - Core contracts not associated with a specific node.
            * [workflows](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs/contracts/workflows) - ContractsS associated with the [workflows](https://github.com/JaimeStill/distributed-design/tree/main/nodes/workflows) node.
        * [core](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs/core) - Library that contains all of the core infrastructure for building apps that interface with nodes.
        * [distributed](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs/distributed) - Angular workspace that defines the [`@distributed/toolkit`](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs/distributed/projects/toolkit) library. Contains Angular-specific infrastructure that can be shared between Angular apps.
    * [proposals](https://github.com/JaimeStill/distributed-design/tree/main/apps/proposals) - Angular app interface for the [`proposals`](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals) node.
    * [workflows](https://github.com/JaimeStill/distributed-design/tree/main/apps/workflows) - Angular app interface for the [`workflows`](https://github.com/JaimeStill/distributed-design/tree/main/nodes/workflows) node.
* [docs](https://github.com/JaimeStill/distributed-design/tree/main/docs) - Repository documentation, hosted at [https://jaimestill.github.io/distributed-design](https://jaimestill.github.io/distributed-design).
* [nodes](https://github.com/JaimeStill/distributed-design/tree/main/nodes) - All node-related infrastructure.
    * [contracts](https://github.com/JaimeStill/distributed-design/tree/main/nodes/contracts) - Projects that define publicly available entity classes and their underlying infrastructure.
        * [core](https://github.com/JaimeStill/distributed-design/tree/main/nodes/contracts/core) - Core contracts not associated with a specific node.
        * [workflows](https://github.com/JaimeStill/distributed-design/tree/main/nodes/contracts/workflows) - Contracts associated with the [workflows](https://github.com/JaimeStill/distributed-design/tree/main/nodes/workflows) node.
    * [core](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core) - Class library that contains all of the core infrastructure for building out a node.
        * [Controllers](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Controllers) - Abstract [Controller](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-7.0#controllerbase-class) definitions.
        * [Data](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Data) - Infrastructure for building out Entity Framework Core related features.
        * [Entities](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Entities) - Core classes and interfaces for building entity classes for a node.
        * [Extensions](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Extensions) - Extension methods that provide enhanced functionality and / or standardizes node configuration.
        * [Gateway](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Gateway) - Infrastructure for building a gateway within a node.
        * [Messages](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Messages) - Return types that wrap a data type with metadata about the results of the operation that was performed.
        * [Middleware](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Middleware) - Contains node specific [middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0) infrastructure.
        * [Services](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Services) - Core classes and interfaces for building command, query, event, and saga services for a node.
            * [Events](https://github.com/JaimeStill/distributed-design/tree/main/nodes/core/Services/Events) - Infrastructure for building SignalR-based event hubs and clients.
    * [proposals](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals) - A simple demonstration node that facilitates the development of proposals.
    * [workflows](https://github.com/JaimeStill/distributed-design/tree/main/nodes/workflows) - A simple demonstration node that facilitates the staffing of packages through pre-defined workflows. Packages are generated from external services through the [`Package`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/Classes/Package.cs) contract via the [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/WorkflowsGateway.cs).
* [scripts](https://github.com/JaimeStill/distributed-design/tree/main/scripts) - PowerShell scripts that facilitate management of Azure cloud deployment (see [deploy-azure.ps1](https://github.com/JaimeStill/distributed-design/blob/main/scripts/deploy-azure.ps1)) and local npm libraries (see [clean-apps.ps1](https://github.com/JaimeStill/distributed-design/blob/main/scripts/clean-apps.ps1)).
* [`deploy.json`](https://github.com/JaimeStill/distributed-design/blob/main/deploy.json) - Contains metadata for simplifying Azure cloud deployment.