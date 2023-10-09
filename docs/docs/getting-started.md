---
sidebar_position: 2
title: Getting Started
---

# Getting Started

You can work with this project either locally or through a Dev Container via GitHub Codespaces. The sections that follow will walk through how to do either.

## Running in a Container

1. Fork this repository and create a Codespace with at least a **4-core** machine type:

    ![codespace-with-options](/img/getting-started/codespace-with-options.png)

    ![codespace-settings](/img/getting-started/codespace-settings.png)

2. Once the Codespace has loaded and the [`postCreateCommand`](https://github.com/JaimeStill/distributed-design/blob/main/.devcontainer/devcontainer.json#L79) has finished executing, be sure to go to extensions and reload to finalize the extension settings:

    :::info
    You will get a Language Server client error on initial load. This is because the extensions are still installing and need to be reloaded per this step.
    :::

    ![extension-reload](/img/getting-started/extension-reload.png)

3. Set the forwarded API ports (labeled *Proposals API (5001)* and *Workflows API (5002)*) to **Public**. If the bottom panel isn't already open, type `` Ctrl + ` ``. Click the *Ports* tab. Right-click the port and set to **Public**:

    ![port-visibility](/img/getting-started/port-visibility.png)

4. Open the *Task Explorer* panel in the side bar and run the `distributed-design/vscode/apps/build-and-install` task:

    > Close all of the task terminals when they are complete

    ![build-and-install](/img/getting-started/build-and-install.png)
    
5. Open 4 terminals (`` Ctrl + Shift + ` `` opens a new terminal) and run the following in each terminal:

    **Terminal 1**

    ```bash
    cd ./nodes/workflows/Workflows.Api
    dotnet run
    ```

    **Terminal 2**

    ```bash
    cd ./nodes/proposals/Proposals.Api
    dotnet run
    ```

    **Terminal 3**

    ```bash
    cd ./apps/proposals
    npm run start:code
    ```

    **Terminal 4**

    ```bash
    cd ./apps/workflows
    npm run start:code
    ```

Access the nodes and apps with the following:

Process | URL
--------|----
Propsals Node | https://CODESPACE_NAME-5001.app.github.dev/swagger
Workflows Node | https://CODESPACE_NAME-5002.app.github.dev/swagger
Proposals App | https://CODESPACE_NAME-3001.app.github.dev
Workflows App | https://CODESPACE_NAME-3002.app.github.dev

## Running Locally

To run this repository locally, you will need:

* [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) configured as `.\DevSql`.
* [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
* [`dotnet ef`](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) tool.
* [Node.js LTS](https://nodejs.org/en).
* [Visual Studio Code](https://code.visualstudio.com/).

### Steps

1. Run the [apps-clean-and-build](https://github.com/JaimeStill/distributed-design/blob/main/.vscode/tasks.json#L32) task.
2. Assuming you have a local `.\DevSql` instance of SQL Server, apply Entity Framework migrations for both the `proposals` and `workflows` nodes:
    ```bash
    dotnet ef database update -p ./nodes/proposals/Proposals.Data
    ```

    ```bash    
    dotnet ef database update -p ./nodes/workflows/Workflows.Data
    ```
3. From 4 different terminals (`` Ctrl + Shift + ` `` opens a new terminal), start the nodes and apps in the order demonstrated below:

    **Terminal 1**

    ```bash
    cd ./nodes/workflows/Workflows.Api
    dotnet run
    ```

    **Terminal 2**

    ```bash
    cd ./nodes/proposals/Proposals.Api
    dotnet run
    ```

    **Terminal 3**

    ```bash
    cd ./apps/proposals
    npm run start:dev
    ```

    **Terminal 4**

    ```bash
    cd ./apps/workflows
    npm run start:dev
    ```

Access the nodes and apps with the following:

Process | URL
--------|----
Propsals Node | [http://localhost:5001/swagger](http://localhost:5001/swagger)
Workflows Node | [http://localhost:5002/swagger](http://localhost:5002/swagger)
Proposals App | [http://localhost:3001](http://localhost:3001)
Workflows App | [http://localhost:3002](http://localhost:3002)