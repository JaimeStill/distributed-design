---
sidebar_position: 2
title: Getting Started
---

# Getting Started

You can work with this project either locally or through a Dev Container via GitHub Codespaces. The sections that follow will walk through how to do either. Before you do so, be sure to fork the repository so you have access to it under your GitHub profile.

## Running in a Container

The following section requires Visual Studio Code to be installed and configured with the following extensions:

* GitHub Codespaces
* Remote Explorer

1. Open a new Visual Studio Code window without opening a folder or repository. This can be done from a terminal with `code -n`, or if you have Visual Studio Code pinned to the taskbar, you can right-click the icon and select *New Window*.

2. Open the *Remote Explorer* sidebar, select *GitHub Codespaces* from the drop-down, and click *Sign in to GitHub*:

    ![codespaces-sign-in](/img/getting-started/codespaces-sign-in.png)

3. Once signed in, click the *Create Codespace* button and use the following settings:

    ![create-codespace](/img/getting-started/create-codespace.png)

    * Repository: `distributed-design`
    * Branch: `main`
    * Instance Type: At least **4 cores**
    
4. After the codespace is finished configuring, click the *Extensions* sidebar and reload the extensions:

    ![reload-extensions](/img/getting-started/reload-extensions.png)

5. Open the *SQL Server Connections* sidebar and verify that the node databases have been initialized:

    ![node-databases](/img/getting-started/node-databases.png)

6. Open the bottom panel with `` Ctrl + ` `` if it's not already open. Click the *Ports* tab. Right-click the *Proposals API (5001)* and *Workflows API (5002)* ports and set *Port Visibility* to **Public**:

    ![codespace-ports](/img/getting-started/codespace-ports.png)

7. Go to the *Task Explorer* sidebar and run the [clean-and-build](https://github.com/JaimeStill/distributed-design/blob/main/.vscode/tasks.json#L32) task:

    ![apps-clean-and-build](/img/getting-started/apps-clean-and-build.png)
    
8. Open 4 terminals (`` Ctrl + Shift + ` `` opens a new terminal) and run the following in each terminal:

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

:::tip
You can open the URLs below through the bottom panel *Ports* tab by clicking the **Open in Browser** globe icon that shows up when you hover over the port in the *Forwarded Address* column:

![open-in-browser](/img/getting-started/open-in-browser.png)
:::

Process | URL
--------|----
Propsals Node | https://CODESPACE_NAME-5001.app.github.dev/swagger
Workflows Node | https://CODESPACE_NAME-5002.app.github.dev/swagger
Proposals App | https://CODESPACE_NAME-3001.app.github.dev
Workflows App | https://CODESPACE_NAME-3002.app.github.dev

## Running Locally

To run this repository locally, you will need:

* [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) configured as `.\DevSql`
* [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* [`dotnet ef`](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) tool
* [Node.js LTS](https://nodejs.org/en)
* [Visual Studio Code](https://code.visualstudio.com/) with the following extensions:
    * C# Dev Kit
    * Task Explorer

1. Open the repository in Visual Studio Code.

2. Go to the *Task Explorer* sidebar and run the [clean-and-build](https://github.com/JaimeStill/distributed-design/blob/main/.vscode/tasks.json#L32) task:

    ![apps-clean-and-build](/img/getting-started/apps-clean-and-build.png)

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