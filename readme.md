# Distributed Design

> This repository is a work in progress. The current APIs and documentation are subject to change. See the [GitHub Project](https://github.com/users/JaimeStill/projects/6) to track progress to initial concept.  
>  
> See [Notes](https://github.com/JaimeStill/distributed-design/blob/main/notes.md) for info that hasn't made it into the docs yet.  

[Documentation](https://jaimestill.github.io/distributed-design/)  

This repository demonstrates a .NET microservice API (each microservice is referred to as a **node**) that takes as few external dependencies as possible. SQL Server serves as the underlying storage mechanism.

In addition to standard data retrieval and mutations, this architecture also focuses on easily enabling cross-node communication and distributed real time data synchronization. It attempts to provide a minimal and easy to incorporate approach to defining microservice infrastructure inspired by the [Command Query Responsibility Segregation (CQRS)](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/cqrs-pattern.html), [Event Sourcing](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/service-per-team.html), and [Saga](https://docs.aws.amazon.com/prescriptive-guidance/latest/modernization-data-persistence/saga-pattern.html) patterns.