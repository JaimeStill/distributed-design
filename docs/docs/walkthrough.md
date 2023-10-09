---
sidebar_position: 3
title: Walkthrough
---

# Walkthrough

The following guide will walk through some of the primary features of the framework, and point to some of the infrastructure that drives it.

## Setup

6. In the **Ports** tab of the bottom panel, you can open the process associated with each port in a new tab by clicking the globe icon in the **Forwarded Address** column. Open the **Workflows App** and **Proposals API**.

    > Once the URL resolves for the API, it will look like you hit a dead page. Add `/swagger` to the end of the URL and it will open the Swagger interface.

    ![running-processes](/img/walkthrough/running-processes.png)

    :::tip
    For the remainder of the walkthrough, you will want your windows positioned as shown above. This way, when Package data is mutated, you can see the effects in real time in the Workflows app.
    :::

## Reactivity and After-Effects

The following section will demonstrate how the API facilitates real time data synchronization and facilitates cross-node interactivity.

1. In the browser tab for the **Proposals API** Swagger interface, expand the `/api/Proposal/Save` endpoint. Click **Try it out** and paste the following value and click **Execute**:

    ```json
    {
        "value": "Demonstrate reactivity and after-effects",
        "title": "Reactivity Proposal"
    }
    ```

    ![proposal-success](/img/walkthrough/proposal-success.png)

2. In the **Workflows** section, expand the `/api/Workflows/SubmitPackage` endpoint. Click **Try it out**, paste the following value, and click **Execute**:

    ```json
    {
        "value": "string",
        "state": "Pending",
        "result": "Created",
        "entityId": 1,
        "entityType": "Proposals.Entities.Proposal",
        "context": "Approval for Acquisition",
        "title": "Proposal Package"
    }
    ```

    <video controls width="100%">
        <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/476bdc17-f9d4-45c4-b978-4192e695fa6c" />
    </video>

3. In the **Proposals** section, expand `/api/Proposal/Get`, click **Try it out**, and click **Execute**. You'll notice that the `Proposal` has been modified with `"packageId": 1`. This is because the [`PackageListener`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Event/Listeners/PackageEventListener.cs) intercepts `OnAdd` events from the **Workflows API** () and passes them to the [`PackageSaga.OnAdd`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Saga/PackageSaga.cs) handler.

    :::tip
    See [`PackageEventHub`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Event/PackageEventHub.cs) and [`PackageCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Services/Command/PackageCommand.cs). The [**Sync hooks**](https://github.com/JaimeStill/distributed-design/blob/main/nodes/core/Services/EntityCommand.cs#L44) in the underlying `EntityCommand` broadcast events for *Add*, *Update*, and *Remove*.
    :::

    ![proposal-with-package-id](/img/walkthrough/proposal-with-package-id.png)

4. Additionally, if a Package is removed and it is the active Package for a Proposal, the Saga will set `Proposal.PackageId = null`. Navigate to the **Workflows** section, expand `/api/Workflows/GetPackagesByType/{entityType}`, click **Try it out**, type `Proposals.Entities.Proposal` in the `entityType` field, then click **Execute**:

    ![get-packages-by-type](/img/walkthrough/get-packages-by-type.png)

5. Copy the Package object in the **Response body** section, expand `/api/Workflows/WithdrawPackage`, click **Try it out**, paste the JSON object into the **Request body** section, and click **Execute**:

    <video controls width="100%">
        <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/8fec8622-94db-463a-8cd1-70446bf253d2" />
    </video>

6. In addition to the Package disappearing in the **Workflows App**, `Proposal.PackageId` was also set to null. Verify by executing the `/api/Proposal/Get` endpoint:

    ![proposal-without-package](/img/walkthrough/proposal-without-package.png)

## Contract Cleanup with Gateways

The **Workflows** endpoints in the **Proposals API** are just a proxy to the **Workflows API**. When the endpoint is hit in the **Proposals API**, the [`WorkflowsController`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Api/Controllers/WorkflowsController.cs) receives the request and forwards it to the [`GatewayController`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/workflows/Workflows.Api/Controllers/GatewayController.cs) in the **Workflows API** through the [`WorkflowsGateway`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/contracts/workflows/WorkflowsGateway.cs) client.

Sagas and Event Listeners work great for reacting to changes in data exposed through public contracts. But how would you handle the inverse? If we delete a Proposal and there is a Package associated with it, Proposal is not exposed as a contract for the **Workflows API** to build infrastructure around. To handle this scenario, we can leverage injecting the **Gateway** client into a command service and handling cleanup inside of service hooks.

1. Create another Package using the below JSON object at the `/api/Workflows/SubmitPackage` endpoint:

    ```json
    {
        "value": "string",
        "state": "Pending",
        "result": "Created",
        "entityId": 1,
        "entityType": "Proposals.Entities.Proposal",
        "context": "Approval for Acquisition",
        "title": "Proposal Package"
    }
    ```

2. Execute the `/api/Proposals/Get` endpoint and copy the JSON object for the proposal:

    ![get-package-again](/img/walkthrough/get-package-again.png)

3. Paste the Proposal JSON object into `/api/Proposals/Remove` and Execute. You will notice that the Package in the **Workflows App** is removed along with the Proposal:

    <video controls width="100%">
        <source src="https://github.com/JaimeStill/distributed-design/assets/14102723/b7504f08-dc1b-4e3d-b446-f72688df7fdb" />
    </video>

This is because the [`ProposalCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs#L46) will clean up any active package associated with it when it is being removed in the `AfterRemove` service hook. It is able to do so because [`WorkflowsGateway` is injected into `ProposalCommand`](https://github.com/JaimeStill/distributed-design/blob/main/nodes/proposals/Proposals.Services/Command/ProposalCommand.cs#L16).