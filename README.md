<h1 align="center">Servly</h1>

<div align="center">
    <strong>Microservices and Multitenancy on .NET made simple</strong>
</div>

<div align="center">
    <h3>
        <a href="#goals-for-mvp">Project Goals</a>
        <span> | </span>
        <a href="#getting-started">Getting Started</a>
        <span> | </span>
        <a href="#license">License</a>
    </h3>
</div>

---

Servly is an in development open-source and cross-platform framework to assist building modern cloud-native microservices on the [.NET](https://dot.net) platform.

It consist of a number of configurable and mixable modules to give you flexibility while constructing your microservices including support for [multi-tenancy](https://en.wikipedia.org/wiki/Multitenancy) scenarios.

# :construction: Active Development

Servly is still under heavy initial development and the functionality and/or interfaces that it provides are subject to change without warning between package versions until the `v1` release.

Once `v1` has been solidified for release, full [semantic versioning](https://semver.org/) will be utilized alerting of any breaking changes.

## Goals for MVP

The goals for this project are still in flux, currently the following is being targeted to be implemented for the `v1` release.

### General

- **Contextual Authentication**: Access a contextual state of current authentication while abstracting the authentication method, intended for use in DDD scenarios.
- **Data Persistence Providers**: Providers for common persistence methods such as [Redis](https://redis.io/) and [EF Core](https://docs.microsoft.com/en-us/ef/core/) with wrappers for common scenarios.
- **Configuration**: Standardized and low effort configuration helpers to reduce setup cost of new services.
- **Logging**: Standardized and low effort logging configuration to reduce setup cost of new services.
- **CQRS**: Either an integration into an existing package or a greenfield implementation of Command Query Responsibility Segregation pattern.
- **Integration Events**: Full featured events system with support for both [RabbitMQ](https://www.rabbitmq.com/) and [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/).

### Multi-tenancy

- **Tenant Identification and Configuration**: Implementation of a fully customizable Tenant identification and configuration scheme to allow handling tenant specific requests with specific changes.
- **Data Persistence**: Multi-tenancy aware persistence providers. [EF Core](https://docs.microsoft.com/en-us/ef/core/) specifically with support for Database-per-Tenant, Single Database and Sharded Database scenarios.
- **Caching**: Multi-tenant compatible caching with a choice of backing methods; Memory, Distributed ([Redis](https://redis.io/)) or [FusionCache](https://github.com/jodydonetti/ZiggyCreatures.FusionCache).

### AspNetCore

- **Fully Integrated**: Fully integrated with the general and Multi-tenancy features listed above.
- **Request Idempotency**: Middleware to enable idempotent http request handling and prevent duplicate execution of sensitive requests.
- **Service Hosting**: Helpers for setting up services and application pipeline to reduce setup effort for new services.

# Getting Started

> Coming Soon

# License

Licensed under [MIT](./LICENSE)

Copyright (c) 2022 DrBarnabus
