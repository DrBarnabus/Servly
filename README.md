# Servly
#### Microservices and Multi-tenancy on .NET made simple

### [Project Goals](#goals-for-mvp) | [Getting Started](#getting-started) | [License](#license)

[![GitHub Release][github-release-badge]][github-release]
[![NuGet Downloads][nuget-downloads-badge]][nuget-downloads]
[![Build Status][gh-actions-badge]][gh-actions]
[![Codecov][codecov-badge]][codecov]

---

**Servly** is an in development open-source and cross-platform framework to assist building modern cloud-native microservices on the [.NET](https://dot.net) platform.

It will consist of a number of configurable and mixable modules to give you flexibility while constructing your microservices including support for [multi-tenancy](https://en.wikipedia.org/wiki/Multitenancy) scenarios.

# 🚧 Active Development

Servly is still under heavy initial development and the functionality and/or interfaces that it provides are subject to change without warning between package versions until the `v1` release.

Once `v1` has been solidified for release, full [semantic versioning](https://semver.org/) will be utilized alerting of any breaking changes.

## Goals for MVP

The goals for this project are still in flux, currently the following is being targeted to be implemented for the `v1` release.

```
Planned       = 🚧
Work Started  = 🏗
v1 Completed  = ✔
Considering   = ❓
```

### General

- **Contextual Authentication (✔)**: Access a contextual state of current authentication while abstracting the authentication method, intended for use in DDD scenarios.
- **Data Persistence Providers (🏗)**: Persistence Providers (used internally initially) for common persistence solutions. (Redis, EF Core)
  - **Redis (✔)**: A provider for [Redis](https://redis.io/) for use with other Services.
  - **EF Core (❓)**: A provider for [EF Core](https://docs.microsoft.com/en-us/ef/core/) for use with other Services.
- **Service Hosting (✔)**: Helpers for setting up services and application pipeline to reduce setup effort for new services.
  - **Configuration (🚧)**: Standardized and low effort configuration setup to reduce boilerplate in service startup.
  - **Logging (🚧)**: Standardized and low effort logging setup to reduce boilerplate in service startup.
- **Request Idempotency (✔)**: Middleware to enable idempotent http request handling and prevent duplicate execution of sensitive requests.
- **Message Bus (🚧)**: Full featured events message bus system for inter service communication.
  - **RabbitMQ (🚧)**: Implementation of Message Bus using [RabbitMQ](https://www.rabbitmq.com/) as a provider.
  - **Azure Service Bus (❓)**: Implementation of Message Bus using [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/) as a provider.
- **Caching (❓)**: Full featured caching implementation providing; Memory, Distributed and Tiered caching.
- **CQRS (🚧)**: Integration into an existing package or a new implementation of a Command Query Responsibility pattern.

### Multi-tenancy

- **Tenant Identification and Specific Configuration (❓)**: Implementation of a fully customizable Tenant identification and configuration scheme to allow handling tenant specific requests with specific configuration.
- **Tenant Aware Data Persistence Providers (❓)**: Extension to Persistence Providers with support for Tenant Isolation.
- **Tenant Aware Caching (❓)**: Extension to Caching with support for Tenant Isolation.

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
