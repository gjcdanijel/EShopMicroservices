# EShopMicroservices

A production-grade **e-commerce backend** built with **.NET 8** and **C# 12**, implementing a full microservices architecture. The system covers the complete shopping lifecycle — from product catalog browsing to basket management, discount application, and order processing — following modern software engineering patterns and cloud-native principles.

---

## Architecture Overview

The application is composed of five independently deployable microservices, a shared API gateway, and a client web application:

| Service | Role | Database | Architecture |
|---|---|---|---|
| **Catalog.API** | Product management (CRUD) | PostgreSQL (via Marten as Document DB) | Vertical Slice + CQRS |
| **Basket.API** | Shopping cart management | Redis (distributed cache) | Clean Architecture + CQRS |
| **Discount.gRPC** | Coupon/discount engine | SQLite (via EF Core) | gRPC server |
| **Ordering.API** | Order lifecycle management | SQL Server (via EF Core) | DDD + Clean Architecture + CQRS |
| **YarpApiGateway** | Single entry point for clients | — | Reverse Proxy (YARP) |
| **Shopping.Web** | Razor Pages frontend client | — | Refit HTTP client |

All services communicate asynchronously over **RabbitMQ** (via MassTransit) and synchronously via **gRPC** where low-latency inter-service calls are required.

---

## Services

### Catalog.API
- ASP.NET Core Minimal APIs with **Carter** for endpoint mapping
- **Vertical Slice Architecture** — each feature lives in its own self-contained folder
- **CQRS** with **MediatR** — commands and queries fully separated
- **Marten** library turns PostgreSQL into a transactional document database (JSON/JSONB)
- **Mapster** for object mapping, **FluentValidation** for input validation
- Cross-cutting pipeline behaviors: logging, validation, exception handling
- Database seeding via Marten's `IInitialData` interface
- Pagination support via `ToPagedListAsync`

### Basket.API
- RESTful CRUD API for shopping cart operations
- **Redis** as distributed cache — implements **Cache-Aside pattern**
- **Proxy and Decorator patterns** for repository abstraction
- Consumes **Discount.gRPC** synchronously to calculate final product prices at cart insertion
- Publishes `BasketCheckout` event to RabbitMQ via **MassTransit** when user checks out

### Discount.gRPC
- High-performance **gRPC** server using Protobuf message contracts
- **SQLite** database via **Entity Framework Core** with auto-migration on startup
- **Dapper** micro-ORM for lightweight, high-performance queries
- Serves as a synchronous dependency for Basket.API during price computation

### Ordering.API
- Implements **Domain-Driven Design (DDD)** — rich domain models, value objects, domain events
- **Clean Architecture** with strict layer separation (Domain → Application → Infrastructure → API)
- **CQRS** via MediatR with **FluentValidation** and **AutoMapper**
- Subscribes to `BasketCheckout` events from RabbitMQ to create new orders
- **SQL Server** with **Entity Framework Core** — auto-migrates schema on startup

### YarpApiGateway
- Built on **YARP (Yet Another Reverse Proxy)**
- Routes all client HTTP traffic to the appropriate downstream microservice
- Implements **Gateway Routing** and **Rate Limiting** patterns

### Shopping.Web
- Razor Pages client consuming the API Gateway
- Uses **Refit** typed HTTP client for strongly-typed service calls

---

## Technology Stack

**Runtime & Language**
- .NET 8, C# 12, ASP.NET Core Minimal APIs

**Architecture & Patterns**
- Microservices, Vertical Slice Architecture, Clean Architecture, DDD
- CQRS, Repository pattern, Decorator pattern, Cache-Aside, Gateway Routing

**Libraries**
- MediatR, Carter, Marten, Mapster, FluentValidation, AutoMapper, Refit, Dapper

**Databases**
- PostgreSQL (Catalog — document mode via Marten)
- Redis (Basket — distributed cache)
- SQLite (Discount — via EF Core)
- SQL Server (Ordering — via EF Core)

**Messaging & Communication**
- RabbitMQ — async publish/subscribe
- MassTransit — abstraction over RabbitMQ
- gRPC — sync inter-service communication (Basket ↔ Discount)

**Infrastructure**
- Docker & Docker Compose
- YARP Reverse Proxy

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

> **Recommended Docker resources:** at least 4 GB RAM and 2 CPUs (configure under Docker Desktop → Settings → Resources).

### Running the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/mehmetozkaya/EShopMicroservices.git
   cd EShopMicroservices
   ```

2. Start all services with Docker Compose:
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
   ```

3. All backing services (PostgreSQL, Redis, SQL Server, RabbitMQ) and microservices will start automatically.

### Service URLs (after Docker Compose)

| Service | URL |
|---|---|
| Catalog API | `http://localhost:6000` |
| Basket API | `http://localhost:6001` |
| Discount gRPC | `http://localhost:6002` |
| Ordering API | `http://localhost:6003` |
| API Gateway | `http://localhost:6004` |
| Shopping Web | `http://localhost:6005` |
| RabbitMQ Management | `http://localhost:15672` |
| Portainer (Docker UI) | `http://localhost:9000` |

> On **macOS**, replace `host.docker.internal` with `docker.for.mac.localhost` in the `.env` file.

---

## Key Flows

**Checkout Flow:**
1. User adds items to basket → Basket.API calls Discount.gRPC to apply discounts and updates Redis
2. User checks out → Basket.API publishes `BasketCheckout` event to RabbitMQ
3. Ordering.API consumes the event and creates a new order in SQL Server

**Discount Application:**
- On every `StoreBasket` command, Basket.API iterates cart items and calls Discount.gRPC synchronously
- Discount.gRPC returns the current discount for the product; Basket.API adjusts the item price before persistence

---

## Project Structure

```
src/
├── Services/
│   ├── Catalog/
│   │   └── Catalog.API/
│   ├── Basket/
│   │   └── Basket.API/
│   ├── Discount/
│   │   └── Discount.gRPC/
│   └── Ordering/
│       ├── Ordering.API/
│       ├── Ordering.Application/
│       ├── Ordering.Domain/
│       └── Ordering.Infrastructure/
├── ApiGateways/
│   └── YarpApiGateway/
├── WebApps/
│   └── Shopping.Web/
└── BuildingBlocks/
    └── BuildingBlocks/       ← Shared CQRS interfaces, behaviors, exceptions
```

---

## License

This project is licensed under the [MIT License](LICENSE).
