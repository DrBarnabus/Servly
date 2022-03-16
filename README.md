# Servly
#### Microservices and Multi-tenancy on .NET made simple

### [Project Goals](#goals-for-mvp) | [Getting Started](#getting-started) | [License](#license)

[![GitHub Release][github-release-badge]][github-release]
[![NuGet Downloads][nuget-downloads-badge]][nuget-downloads]
[![Build Status][gh-actions-badge]][gh-actions]
[![Codecov][codecov-badge]][codecov]

---

**Servly** is an in development open-source and cross-platform framework to assist building modern cloud-native microservices on the [.NET](https://dot.net) platform.

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

> Documentation Coming Soon

# License

Licensed under [MIT](./LICENSE)

Copyright (c) 2022 DrBarnabus

[github-release-badge]:     https://img.shields.io/github/v/release/DrBarnabus/Servly?color=g&style=for-the-badge
[github-release]:           https://github.com/DrBarnabus/Servly/releases/latest
[nuget-downloads-badge]:    https://img.shields.io/nuget/dt/Servly.Core?color=g&logo=nuget&style=for-the-badge
[nuget-downloads]:          https://www.nuget.org/packages/Servly.Core
[gh-actions-badge]:         https://img.shields.io/github/workflow/status/DrBarnabus/Servly/CI/main?logo=github&style=for-the-badge
[gh-actions]:               https://github.com/DrBarnabus/Servly/actions/workflows/ci.yml
[codecov-badge]:            https://img.shields.io/codecov/c/github/DrBarnabus/Servly/main?logo=codecov&logoColor=fff&style=for-the-badge
[codecov]:                  https://codecov.io/gh/DrBarnabus/Servly/branch/main
