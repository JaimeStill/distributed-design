---
sidebar_position: 3
title: Walkthrough
---

# Walkthrough

The nodes and apps built out in this repository are intended to demonstrate the real-time, cross-node data synchronization patterns enabled by the underlying libraries that they are built upon. The sections that follow will walk through the scenarios that demonstrate these patterns and highlight some of the infrastructure that enables the associated functionality.

:::info
This guide assumes that you have already followed the steps in [getting-started](/getting-started).
:::

:::tip
This guide will provide a high-level overview of how the underlying functionality is enabled. The [Tutorial](/tutorial) will provide a thorough step-by-step guide for how to build a node and an app with the underlying libraries.

Also, a lot of the infrastructure demonstrated in this walkthrough is from the underlying libraries. There is a lot of complexity that is exposed here to illustrate how this functionality is enabled. Reference the actual nodes ([*Proposals*](https://github.com/JaimeStill/distributed-design/tree/main/nodes/proposals) and [*Workflows*](https://github.com/JaimeStill/distributed-design/tree/main/nodes/workflows)) and apps ([*Proposals*](https://github.com/JaimeStill/distributed-design/tree/main/apps/proposals) and [*Workflows*](https://github.com/JaimeStill/distributed-design/tree/main/apps/workflows)) to see how the libraries simplify the implementation details.
:::

## Setup

If you haven't already, make sure that all nodes and apps are running. Be sure the apps are started with the approprate command (`npm run start:dev` if running local or `npm run start:code` if running in a codespace). The sections that follow will demonstrate how the API facilitates real time data synchronization and cross-node interactivity. With this in mind, you will want to configure two browser windows side by side with each app running in tabs in both windows:

![browser-setup](/img/walkthrough/browser-setup.png)

## Internal Node Synchronization

Mutations to data within a node are executed through `Command` methods. This is typically exposed as a [`Save`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L154) method on a specific `Entity` type internally owned by the node. When a `Command` is executed, an `Event` is broadcast with a message containing the affected data. Any node or application with access to a `Listener` associated with the affected data can then react to the change in state.

If additional data mutations need to occur as a result of the command, these interactions can be defined through the use of `Hook` methods. These hooks represent points in the `Command` method where functionality can be injected. For instance, [`OnAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L34) is executed prior to the mutations implemented in the internal [`Add`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L98) method. On the other hand, [`AfterUpdate`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L40) is executed after the mutations implemented in the internal [`Update`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L127) method. [`OnSave`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L36) is executed prior to the mutations of either `Add` or `Update` via the public [`Save`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L164) method.

You'll notice that the [`Save`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L160) and [`Remove`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L190) methods are [transactional](https://learn.microsoft.com/en-us/ef/core/saving/transactions) through the use of `db.Database.BeginTransactionAsync`. If any aspect of the method fails, to include the [result of any hook calls](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L24), the entire transaction is rolled back.

To see this in action, execute the following steps with both browser windows showing the *Proposals* app:

1. Click the *Create Proposal* button indicated by the plus icon to the left of the **Home** heading.

2. Provide a `Title` and `Value` in the *Add Proposal* dialog, then click **Save Proposal**:

    <video controls width="100%">
        <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/287064bd-54de-4844-a3b5-f726afea3763" />
    </video>

There are two things to note about this transaction:

1. The `Proposal` record that was created synchronized across all connected instances of the Proposals app.

2. In addition to creating a `Proposal`, an associated `Status` was created (illustrated by the *Created* status to the top right of the Proposal card in the application).

The [`EntityCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs) class defines a [`SyncAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L44) hook that is called after the internal [`Add`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L107) transaction is successful. Every `EntityCommand` must be initialized with an [`EventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/Events/Hub/EventHub.cs), which is a [strongly-typed SignalR hub](https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0#strongly-typed-hubs) that is defined with an [`IEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/Events/Hub/IEventHub.cs) interface. When `SyncAdd` is called, it broadcasts the added entity through the [`IEventHub.OnAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/Events/Hub/IEventHub.cs#L7) method.

A TypeScript [`EventListener`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/core/src/services/events/event-listener.ts) class is provided that allows you to define interfaces to `EventHub` instances. In this case, a [`ProposalListener`](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/services/proposal-listener.ts) is defined and injected into the [`HomeRoute`](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/routes/home/home.route.ts#L64). In the [`ngOnInit`](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/routes/home/home.route.ts#L88) lifecycle hook, the listener is initialized and events are registered.

The [`ProposalCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs) provides an [`AfterAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs#L26) hook that [generates a created status](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/core/Extensions/StatusExtensions.cs#L6) and saves it to the proposal.

## Cross-Node Synchronization

There are three primary interfaces that facilitate cross-node interactivity:

* `Contract` - an object that is sent to the remote node to initiate interactivity and track progress across the service lifetime.
    * [`Package`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/Classes/Package.cs) is a *Workflows* node `Contract`.
* `Gateway` - the API interface for sending to and retrieving data from a remote node.
    * [`GatewayController`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Api/Controllers/GatewayController.cs) defines the `Gateway` API for the *Workflows* node.
    * [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/WorkflowsGateway.cs) is the `Gateway` Service for interfacing with the *Workflows* node.
* `Listener` - the event listener provided by the remote node for tracking contract events.
    * [`PackageEventListener`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Event/Listeners/PackageEventListener.cs) handles `Package` events generated by *Workflows* node.

The following scenarios show patterns for working with these interfaces.

### Submitting a Contract

The following interactions occur to facilitate submitting a contract to a remote node:

1. The data for the contract is filled out by the initiating node
2. The contract is sent through the gateway interface for the remote node
3. The remote node receives and processes the contract
4. The remote node broadcasts an event when the contract has been successfully processed

To see this in action, set the left browser tab to the *Proposals* app and the right browser tab to the *Workflows* app. Execute the following steps:

1. In the *Proposals* app on the *Documentation Proposal* card, click the middle **Submit Package** icon button represented by a blue right-facing arrow.

2. A *PackageDialog* will open. Provide entries for `Title` and `Value`, then click **Save Package**.

<video controls width="100%">
    <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/2891310b-fab6-4a75-9d90-08ab59aa9aed" />
</video>

The [`PackageDialog`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/distributed/projects/toolkit/src/dialogs/package.dialog.ts) used to submit the package, along with its associated [`PackageForm`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/distributed/projects/toolkit/src/forms/package.form.ts), are exposed through the [`@distributed/toolkit`](https://github.com/JaimeStill/distributed-design/tree/main/apps/libs/distributed/projects/toolkit) library as part of the common infrastructure for interfacing with the Workflows node. When the [`Package`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/contracts/workflows/src/interfaces/package.ts) contract is saved through the PackageForm, it uses the [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/distributed/projects/toolkit/src/gateways/workflows.gateway.ts) service to submit the package. The gateway endpoint is configured within each app whenever the [`ToolkitModule`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/distributed/projects/toolkit/src/toolkit.module.ts) is imported (see [Proposals - AppModule](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/app.module.ts#L42)).

The [`GatewayController.SubmitPackage`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Api/Controllers/GatewayController.cs#L45) method in the workflows node passes the received package to the [`PackageCommand.Submit`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Command/PackageCommand.cs#L17) method. Since this is a new package, the `Save` method will be called, which will trigger the `PackageCommand.SyncAdd` hook (in the same way as the `ProposalCommand` service when a new Proposal is created).

The [`PackageListener`](https://github.com/JaimeStill/distributed-design/blob/main/apps/libs/distributed/projects/toolkit/src/nodes/package-listener.ts) defined in `@distributed/toolkit` is injected in the home route of both the [proposals app](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/routes/home/home.route.ts#L59) and the [workflows app](https://github.com/JaimeStill/distributed-design/blob/main/apps/workflows/src/app/routes/home/home.route.ts#L37). Both routes initialize the listener in the respective `ngOnInit` lifecycle hooks: see [proposals](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/routes/home/home.route.ts#L82) and [workflows](https://github.com/JaimeStill/distributed-design/blob/main/apps/workflows/src/app/routes/home/home.route.ts#L71).

The proposals app uses an `Observable` trigger to feed into each [`ProposalCard`](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/components/proposal-card.component.ts#L54). The card component uses this trigger to [handle package events](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/components/proposal-card.component.ts#L106) if the received event is associated with its proposal.

If you retrieve the `Proposal` both before and after submitting the `Package`, you'll notice that a `packageId` is assigned when a `Package` is successfully submitted:

**Before**

```json
{
    "statusId": 1,
    "title": "Documentation Proposal",
    "id": 1,
    "type": "Proposals.Entities.Proposal",
    "value": "A demonstrative Proposoal for documentation purposes.",
    "dateCreated": "2023-10-13T12:38:52.3668313",
    "dateModified": "2023-10-13T13:11:11.73073"
}
```

**After**

```json
{
    "statusId": 1,
    "packageId": 1,
    "title": "Documentation Proposal",
    "id": 1,
    "type": "Proposals.Entities.Proposal",
    "value": "A demonstrative Proposoal for documentation purposes.",
    "dateCreated": "2023-10-13T12:38:52.3668313",
    "dateModified": "2023-10-13T13:15:11.73073"
}
```

Leveraging an [`EventListener`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/Events/Client/EventListener.cs) is not exclusive to client apps. In fact, synchronizing node data that's associated with contracts in remote nodes is exclusively handled through [`EntitySaga`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntitySaga.cs) services through events.

In this case, the proposals node defines a [`PackageListener`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Event/Listeners/PackageEventListener.cs) that reacts to `Package` events. In this case, whenever a `Package` is added, the [`OnAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Event/Listeners/PackageEventListener.cs#L18) event triggers the [`PackageSaga.OnAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Saga/PackageSaga.cs#L21) method. Internal changes spawned by a `Saga` will still broadcast update events for the affected Entities.

### Reacting to Contract Changes

The [`PackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Event/PackageEventHub.cs) defined by the workflows node provides an additional [`OnStateChanged`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Event/IPackageEventHub.cs#L7) event. This is used to isolate changes to [`Package.State`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/Classes/Package.cs#L10) from changes to the editable `Package` entity fields.

In the [`PackageCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Command/PackageCommand.cs) service, there is validation to ensure [state is not improperly modified](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Command/PackageCommand.cs#L121) as well as validation to [ensure proper state transitions](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Command/PackageCommand.cs#L103). Also, a `Package` can only be modified [if it is not in a completed state](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Command/PackageCommand.cs#L128).

The `PackageCommand` service also defines methods for [changing the state](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Command/PackageCommand.cs#L22) of a `Package`.

In the Workflows app, pending packages provide three actions for handling that package:

![pending-package-card](/img/walkthrough/pending-package-card.png)

From left to right, those actions are:

* **Reject** - permanently reject the package that was submitted
* **Return** - return the package that was submitted for corrections
* **Approve** - approve the package and change the status to the associated `Entity`

The following clips demonstrate returning, resubmitting, and approving:

**Returning a Package**

<video controls width="100%">
    <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/de84c3ef-bca0-48fe-8a09-83d7e655c3da" />
</video>

**Resubmitting a Package**

<video controls width="100%">
    <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/c8652959-d902-4c57-87dc-807d07ffdabe" />
</video>

**Approving a Package**

<video controls width="100%">
    <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/4d5314dd-b16b-4bf3-a08b-29ceb53fdcbc" />
</video>

The `Proposal` is able to detect when the associated `Package` is complete because the [`PackageListener.OnStateChanged`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Event/Listeners/PackageEventListener.cs#L30) event triggers the [`PackageSaga.OnStateChanged`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Saga/PackageSaga.cs#L49) method. The [`ProposalCardComponent`](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/components/proposal-card.component.ts#L117) is able to track whether a `Package` can be submitted via the [`canSubmit`](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/components/proposal-card.component.ts#L117) function, which is kept synchronized through the [`handlePackageEvent`](https://github.com/JaimeStill/distributed-design/blob/main/apps/proposals/src/app/components/proposal-card.component.ts#L106) trigger.

### Cleaning Up Contracts

It's easy to synchronize internal state with changes to contracts through events because the node is aware of the remote node. The inverse, however, is not true. The remote node is never aware of the nodes that consume it. The only link it ever has to its dependent nodes is through the contract data it receives. If a `Package` contract is submitted based on an associated `Proposal` and the `Proposal` is later removed, how does the corresponding `Package` get cleaned up as well?

By leveraging `Command` hooks and `Gateway` services, associated contract data can be cleaned up. The following clip demonstrates submitting a `Package`, then removing the associated `Proposal`:

<video controls width="100%">
    <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/3fca664c-1a1d-4cf0-aa56-ab80a36a1476" />
</video>

If you look at the [`ProposalCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs) service, you'll see that the [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/WorkflowsGateway.cs) service is [injected in the constructor](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs#L16). The [`AfterRemove`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs#L46) hook tries to retrieve an active `Package` associated with the `Proposal` being removed. If it is found, the `WorkflowsGateway.WithdrawPackage` method is called. If the package is unable to be withdrawn for any reason, the whole transaction will be aborted and the `Proposal` will not be removed.