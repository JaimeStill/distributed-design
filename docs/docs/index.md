---
sidebar_position: 1
slug: /
title: Overview
---

# Distributed Design

This repository demonstrates a .NET microservice API (each microservice is referred to as a **node**) that takes as few external dependencies as possible. SQL Server serves as the underlying storage mechanism.

In addition to standard data retrieval and mutations, this architecture also focuses on easily enabling cross-node communication and distributed real time data synchronization.

## Terminology

This project uses the following terminology to describe different facets of its infrastructure:

Term | Definition | Example
-----|------------|--------
**API** | A .NET Web API project that establishes HTTP and Web Socket endpoints for a **Node**. | [Proposals API](https://github.com/JaimeStill/distributed-design/tree/main/services/api/Distributed.Proposals.Api)
**App** | A client that interfaces with one or more **Nodes**. | Pending
**Command** | A **Service** that defines user-driven public data mutation methods. | [`ProposalCommand`](https://github.com/JaimeStill/distributed-design/blob/main/services/logic/Distributed.Proposals.Logic/Command/ProposalCommand.cs)
**Contract** | An **Entity** that facilitates interaction between **Nodes**. A **Contract** can be thought of as a *public* **Entity**. | [`Package`](https://github.com/JaimeStill/distributed-design/blob/main/services/common/Distributed.Contracts/Classes/Package.cs)
**Entity** | A data model represented by an underlying store table that encapsulates an aspect of the associated **Node** domain. | [`Proposal`](https://github.com/JaimeStill/distributed-design/blob/main/services/schema/Distributed.Proposals.Schema/Proposal.cs)
**EventHub** | A strongly-typed SignalR `Hub` that broadcasts **Entity**-based mutation events. | <ul><li>Interface: [`IPackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/services/logic/Workflows.Services/Event/IPackageEventHub.cs)</li><li>Hub: [`PackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/services/logic/Workflows.Services/Event/PackageEventHub.cs)</li></ul>
**EventListener** | A SignalR client that facilitates reacting to  **Entity**-based mutation events broadcast by an **EventHub**. | Pending
**Gateway** | The publicly-exposed API interface defined by a **Node** that facilitates cross-node interaction. | <ul><li>API: [`GatewayController`](https://github.com/JaimeStill/distributed-design/blob/main/services/api/Workflows.Api/Controllers/GatewayController.cs)</li><li>Client: [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/services/common/Distributed.Contracts/Gateways/WorkflowsGateway.cs)</li><li>Configuration: [`appsettings.json`](https://github.com/JaimeStill/distributed-design/blob/main/services/api/Workflows.Api/appsettings.Development.json#L11)</li></ul>
**Hook** | An asynchronous delegate function that allows logic to be executed at critical moments within a **Command** action. | <ul><li>Definition: [`AfterAdd`](https://github.com/JaimeStill/distributed-design/blob/main/services/common/Distributed.Core/Services/EntityCommand.cs#L39)</li><li>Implementation: [`AfterAdd`](https://github.com/JaimeStill/distributed-design/blob/main/services/logic/Distributed.Proposals.Logic/Command/ProposalCommand.cs#L17)</li></ul>
**Node** | A microservice backend that encapsulates a single micro domain. This term was selected in place of ***Service*** to avoid confusion. **Service** is an overloaded term that already carries a different definition in this architecture. | 
**Query** | A **Service** that facilitates retrieval of **Entity** data. | 
**Saga** | A **Service** that defines system-drive private data mutation methods; they isolate reactionary logic for determining how to handle effects of mutations triggered through **Events**.
**Service** | A class that exposes functionality and is registered with the .NET Dependency Injection service container.

## Layout

The layout of the 