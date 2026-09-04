<div align="center">

<a href="https://infodev.com.np/">
  <img src="https://infodev.com.np/infodev_logo_white-2.svg" alt="InfoDevelopers" width="260"/>
</a>

# 🍽️ Restaurant Management System

### Backend Engineering Project • ASP.NET Core Web API • MySQL • Dapper • SignalR

<p>
  <img src="https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white" alt="ASP.NET Core"/>
  <img src="https://img.shields.io/badge/C%23-Backend-239120?logo=csharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/MySQL-Database-4479A1?logo=mysql&logoColor=white" alt="MySQL"/>
  <img src="https://img.shields.io/badge/Dapper-Micro%20ORM-1F2937" alt="Dapper"/>
  <img src="https://img.shields.io/badge/SignalR-Real--Time-512BD4?logo=dotnet&logoColor=white" alt="SignalR"/>
  <img src="https://img.shields.io/badge/JWT-Authentication-000000?logo=jsonwebtokens&logoColor=white" alt="JWT"/>
</p>

<img src="https://readme-typing-svg.demolab.com?font=Fira+Code&size=20&pause=900&center=true&vCenter=true&width=850&lines=RESTful+APIs+%7C+JWT+%7C+RBAC+%7C+Middleware;Dapper+%7C+MySQL+%7C+Stored+Procedures+%7C+Transactions;SignalR+%7C+API+Versioning+%7C+Rate+Limiting;Microservices+%7C+Redis+%7C+Layered+Architecture+%7C+SOLID" alt="Technology stack animation"/>

</div>

---

## 📌 Project Overview

The **Restaurant Management System** is a backend-focused application designed to manage the operational workflow of a modern restaurant.

The system covers the complete restaurant lifecycle, including **table management, QR-based table identification, dining sessions, waiter assignment, menu management, order processing, kitchen/bar coordination, real-time order tracking, billing, payment, and table availability**.

This repository focuses on the **backend/API implementation** and demonstrates practical backend engineering concepts using the Microsoft .NET ecosystem.

> **Project period:** July 2026 – September 2026  
> **Project type:** Full-Stack Project — Backend Repository  
> **Primary backend:** ASP.NET Core Web API

---

# 🎯 Objectives

- Design maintainable and scalable RESTful APIs.
- Implement secure authentication and authorization.
- Model real-world restaurant workflows as backend business processes.
- Enable real-time communication between restaurant roles and services.
- Maintain database consistency during multi-step operations.
- Optimize relational database access using Dapper and stored procedures.
- Apply layered architecture and SOLID principles.
- Build APIs that can evolve through versioning.
- Protect API resources through rate limiting and middleware-based request processing.

---

# 🏗️ Architecture

The application follows a **layered backend architecture** that separates API concerns, business logic, data access, and persistence.

```text
                         ┌──────────────────────┐
                         │      API Clients     │
                         │ Web / Mobile / POS   │
                         └──────────┬───────────┘
                                    │
                                    ▼
                    ┌──────────────────────────────┐
                    │     ASP.NET Core Web API     │
                    │ Controllers / API Versioning │
                    └──────────────┬───────────────┘
                                   │
                                   ▼
                    ┌──────────────────────────────┐
                    │       Middleware Pipeline    │
                    │                              │
                    │ Exception Handling           │
                    │ Authentication / Authorization│
                    │ Rate Limiting                │
                    │ Request Processing           │
                    │ Logging / Cross-Cutting      │
                    └──────────────┬───────────────┘
                                   │
                                   ▼
                    ┌──────────────────────────────┐
                    │       Service Layer          │
                    │                              │
                    │ Business Rules              │
                    │ Validation                   │
                    │ Workflow Orchestration       │
                    └──────────────┬───────────────┘
                                   │
                                   ▼
                    ┌──────────────────────────────┐
                    │      Repository / Data       │
                    │                              │
                    │ Dapper                       │
                    │ Stored Procedures            │
                    │ Transactions                 │
                    └──────────────┬───────────────┘
                                   │
                     ┌─────────────┴─────────────┐
                     ▼                           ▼
              ┌─────────────┐             ┌─────────────┐
              │    MySQL    │             │    Redis    │
              │ Persistence │             │ Cache / Fast│
              │             │             │ Data Access │
              └─────────────┘             └─────────────┘

                         ┌──────────────────────┐
                         │       SignalR        │
                         │ Real-Time Messaging  │
                         └──────────┬───────────┘
                                    │
                                    ▼
                     Kitchen • Bar • Waiter • Client
```

---

# 🔄 Restaurant Business Workflow

The backend represents the restaurant operation as a controlled workflow:

```text
Customer Arrives
       │
       ▼
Scan Table QR
       │
       ▼
Login / Guest
       │
       ▼
Start Dining Session
       │
       ▼
Notify Waiter
       │
       ▼
Waiter Assigned
       │
       ▼
Browse Menu
       │
       ▼
Place Order
       │
       ▼
Kitchen / Bar Notification
       │
       ▼
Preparing
       │
       ▼
Ready
       │
       ▼
Waiter Picks Up
       │
       ▼
Order Served
       │
       ▼
Request Bill
       │
       ▼
Payment
       │
       ▼
Table Cleaning
       │
       ▼
Table Available
```

This workflow is implemented through API operations, business rules, database state changes, and real-time SignalR notifications.

---

# 👥 Roles & RBAC

The system uses **JWT authentication** together with **Role-Based Access Control (RBAC)**.

| Role | Responsibilities |
|---|---|
| 👑 **Admin** | Administrative and restaurant-level management |
| 🧑‍💼 **Waiter** | Table service, waiter assignment, order pickup, serving, customer requests |
| 👨‍🍳 **Chef** | Kitchen order processing and preparation status |
| 🍹 **Bar Attendant** | Beverage/bar order processing |
| 🧑 **Customer / Guest** | Table session, menu browsing, ordering, and billing requests |

Authorization is enforced at the API layer so protected operations are available only to permitted roles.

---

# 🔐 Authentication & Authorization

## JWT Authentication

The backend uses **JSON Web Tokens (JWT)** for stateless authentication.

```text
┌──────────┐       Login        ┌──────────────┐
│  Client  │ ─────────────────► │ Auth API     │
└──────────┘                    └──────┬───────┘
                                       │
                                  Generate JWT
                                       │
                                       ▼
┌──────────┐     Bearer Token    ┌──────────────┐
│  Client  │ ─────────────────►  │ ASP.NET Core │
└──────────┘                     │ API          │
                                 └──────┬───────┘
                                        │
                              Validate Token
                                        │
                                        ▼
                               Check Role / Policy
                                        │
                                        ▼
                                 Business Logic
```

### Security concepts

- JWT authentication
- Role-Based Access Control
- Authorization policies
- Protected endpoints
- Stateless authentication
- Secure request pipeline

---

# 🧩 Middleware Pipeline

The backend uses the ASP.NET Core **middleware pipeline** to handle cross-cutting concerns before requests reach application endpoints.

Typical request flow:

```text
HTTP Request
     │
     ▼
┌───────────────────────┐
│ Exception Middleware  │
└───────────┬───────────┘
            ▼
┌───────────────────────┐
│ Rate Limiting         │
└───────────┬───────────┘
            ▼
┌───────────────────────┐
│ Authentication        │
└───────────┬───────────┘
            ▼
┌───────────────────────┐
│ Authorization         │
└───────────┬───────────┘
            ▼
┌───────────────────────┐
│ Endpoint Routing      │
└───────────┬───────────┘
            ▼
       Controller
            │
            ▼
         Service
            │
            ▼
       Repository
            │
            ▼
         Database
```

Middleware provides a clean mechanism for handling **cross-cutting concerns** without duplicating logic across controllers.

---

# ⚡ SignalR — Real-Time Communication

**ASP.NET Core SignalR** is used for real-time communication between restaurant services and connected clients.

Instead of repeatedly polling the API, important events can be pushed to connected clients.

### Example

```text
                       ┌─────────────┐
                       │  SignalR Hub│
                       └──────┬──────┘
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
          👨‍🍳 Chef       🧑‍💼 Waiter      🧑 Customer
              │               │               │
              └───────────────┼───────────────┘
                              │
                       Real-Time Events
```

### Example events

- New order created
- Kitchen order notification
- Bar order notification
- Order status changed
- Order ready
- Waiter notification
- Customer service request
- Order pickup/serving updates

This makes SignalR particularly useful for **restaurant operational coordination and live order tracking**.

---

# 🚦 API Versioning

API versioning allows the backend to evolve without unnecessarily breaking existing clients.

Example:

```http
GET /api/v1/orders
GET /api/v2/orders
```

Benefits:

- Backward compatibility
- Controlled API evolution
- Safer introduction of breaking changes
- Multiple client versions can coexist

---

# 🛡️ Rate Limiting

Rate limiting protects API resources by controlling how frequently clients can send requests.

```text
             Incoming Requests
                    │
                    ▼
             ┌─────────────┐
             │ Rate Limiter│
             └──────┬──────┘
                    │
             ┌──────┴──────┐
             ▼             ▼
          Allowed        Rejected
             │             │
             ▼             ▼
           API       HTTP 429
```

This helps improve API stability and reduces excessive or abusive traffic.

---

# 🗄️ Database & Data Access

## MySQL + Dapper

The backend uses **MySQL** as the relational database and **Dapper** as the micro-ORM.

### Why Dapper?

- Lightweight
- High-performance data access
- Explicit SQL control
- Minimal abstraction
- Simple object mapping
- Good fit for stored-procedure-driven systems

```text
Service
   │
   ▼
Repository
   │
   ▼
 Dapper
   │
   ▼
Stored Procedure / SQL
   │
   ▼
 MySQL
```

---

# 🔒 Atomic Database Transactions

Critical workflows use database transactions to prevent partial updates.

For example, order creation can involve:

```text
BEGIN TRANSACTION
        │
        ├── Validate Dining Session
        │
        ├── Validate Menu Items
        │
        ├── Create / Update Order
        │
        ├── Insert Order Items
        │
        ├── Update Required State
        │
        └── COMMIT
              │
              ▼
           SUCCESS
```

If an operation fails:

```text
Error
  │
  ▼
ROLLBACK
  │
  ▼
Database remains consistent
```

This is particularly important for **order creation, quantity updates, billing, and other multi-step operations**.

---

# 🧠 Stored Procedures

MySQL stored procedures are used for database operations where encapsulating SQL/database logic provides a clear and controlled data-access boundary.

Advantages include:

- Centralized database operations
- Reduced application-side query duplication
- Controlled database execution
- Reusable SQL logic
- Efficient execution of complex database operations

---

# 🚀 Redis

**Redis** was used during backend engineering work to support high-performance data workflows and caching-oriented use cases.

```text
                 Application
                     │
                     ▼
              ┌─────────────┐
              │ Redis Cache │
              └──────┬──────┘
                     │
             Cache Hit │ Cache Miss
                     │       │
                     │       ▼
                     │    MySQL
                     │
                     ▼
                  Response
```

Redis can reduce unnecessary database reads and improve response performance for suitable frequently accessed data.

> Redis is included here as part of the backend engineering experience developed during the internship. It should only be described as a component of this specific restaurant application if Redis is actually configured and used in this repository.

---

# 🧱 Microservices Experience

During backend engineering work at **InfoDevelopers**, I also worked with **microservice-oriented systems** and learned how independently deployable services can communicate and operate within a larger distributed architecture.

A simplified service architecture:

```text
                         API Gateway / Entry Point
                                  │
                ┌─────────────────┼─────────────────┐
                ▼                 ▼                 ▼
        ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
        │ Auth Service │  │ Order Service│  │ Menu Service │
        └──────┬───────┘  └──────┬───────┘  └──────┬───────┘
               │                 │                 │
               ▼                 ▼                 ▼
          Data Layer        Data Layer        Data Layer
               │                 │                 │
               └─────────────────┼─────────────────┘
                                 ▼
                           Shared Infrastructure
                       Redis • Database • Messaging
```

### Concepts covered

- Service boundaries
- Inter-service communication
- Layered architecture within services
- Independent business capabilities
- Distributed data access
- Scalability considerations
- Fault isolation

---

# 🏢 Internship Engineering Experience — InfoDevelopers

This project also reflects the backend technologies and engineering practices I worked with during my **Software Engineering Internship at InfoDevelopers Pvt. Ltd., Sanepa, Lalitpur, Nepal**.

urlInfoDevelopers official websitehttps://infodev.com.np/

### Internship period

**June 2026 – Present**

### Backend engineering responsibilities

- Engineered RESTful APIs using **ASP.NET Core Web API**
- Implemented **JWT authentication**
- Implemented **Role-Based Access Control (RBAC)**
- Worked with **API versioning**
- Implemented **rate limiting**
- Applied **SOLID principles**
- Designed and worked with **layered architecture**
- Implemented and worked with **middleware**
- Optimized data access using **Dapper**
- Worked with **MySQL stored procedures**
- Implemented **atomic database transactions**
- Used **Redis** for efficient backend workflows and caching-oriented scenarios
- Implemented **SignalR** for real-time communication
- Worked with **microservice-based backend architecture**
- Developed maintainable RESTful backend services

---

# 🧰 Technology Stack

| Area | Technologies |
|---|---|
| Language | C# |
| Framework | ASP.NET Core Web API |
| API | RESTful APIs |
| Authentication | JWT |
| Authorization | RBAC / Authorization Policies |
| Middleware | ASP.NET Core Middleware Pipeline |
| Real-Time | SignalR |
| API Management | API Versioning, Rate Limiting |
| Architecture | Layered Architecture, Microservices |
| Database | MySQL |
| Data Access | Dapper |
| Database Logic | Stored Procedures |
| Transactions | Atomic Database Transactions |
| Caching / Performance | Redis |
| Design Principles | SOLID |
| Dependency Management | Dependency Injection |

---

# 📦 Core Backend Modules

### 👤 Authentication & Authorization
- User authentication
- JWT token generation/validation
- Role-based authorization
- Protected API endpoints

### 🪑 Table Management
- Table management
- QR-based table identification
- Table availability
- Table lifecycle management

### 🍽️ Dining Sessions
- Start dining sessions
- Track active sessions
- Associate sessions with tables
- Manage session lifecycle

### 📖 Menu
- Menu browsing
- Menu item management
- Menu availability

### 🛒 Orders
- Create orders
- Add order items
- Update quantities
- Track order status
- Coordinate kitchen/bar processing

### 👨‍🍳 Kitchen & Bar
- Receive order notifications
- Process relevant order items
- Update preparation status
- Notify waiters when orders are ready

### 🧑‍💼 Waiter Operations
- Waiter assignment
- Customer requests
- Order pickup
- Serving workflow

### 💵 Billing
- Bill generation/request
- Payment workflow
- Billing state management

---

# 🧪 REST API Design

The backend follows resource-oriented REST conventions.

Example API structure:

```http
# Tables
GET    /api/v1/tables
GET    /api/v1/tables/{id}
POST   /api/v1/tables
PUT    /api/v1/tables/{id}
DELETE /api/v1/tables/{id}

# Orders
GET    /api/v1/orders
GET    /api/v1/orders/{id}
POST   /api/v1/orders
PUT    /api/v1/orders/{id}/status

# Dining Sessions
POST   /api/v1/dining-sessions
GET    /api/v1/dining-sessions/{id}

# Bills
POST   /api/v1/bills
GET    /api/v1/bills/{id}
```

> Update these examples to match the exact routes implemented in the repository.

---

# 📁 Recommended Backend Structure

```text
RestaurantManagementSystem
│
├── Controllers/
│   ├── AuthController.cs
│   ├── TableController.cs
│   ├── DiningController.cs
│   ├── MenuController.cs
│   ├── OrderController.cs
│   └── BillController.cs
│
├── Services/
│   ├── Interfaces/
│   └── Implementations/
│
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
│
├── Models/
│
├── DTOs/
│
├── Hubs/
│   └── RestaurantHub.cs
│
├── Middleware/
│
├── Helpers/
│
├── Configuration/
│
└── Database/
    └── StoredProcedures/
```

> This is a conceptual structure. Rename directories to match the actual repository.

---

# 🧠 SOLID & Clean Code Practices

The backend follows principles intended to keep the system maintainable as functionality grows.

### S — Single Responsibility
Controllers, services, repositories, and middleware have separate responsibilities.

### O — Open/Closed
Business components are structured to allow extension without unnecessary modification.

### L — Liskov Substitution
Abstractions and implementations are designed to remain interchangeable where applicable.

### I — Interface Segregation
Interfaces are kept focused around specific responsibilities.

### D — Dependency Inversion
High-level services depend on abstractions and use dependency injection rather than tightly coupling implementations.

---

# 📊 Engineering Highlights

| Area | Implementation |
|---|---|
| API Architecture | RESTful ASP.NET Core Web API |
| Security | JWT + RBAC |
| Request Pipeline | Middleware |
| API Evolution | Versioning |
| API Protection | Rate Limiting |
| Real-Time | SignalR |
| Data Access | Dapper |
| Database | MySQL |
| Database Logic | Stored Procedures |
| Consistency | Atomic Transactions |
| Performance | Redis |
| Architecture | Layered Architecture |
| Distributed Systems | Microservices |
| Code Quality | SOLID + Dependency Injection |

---

# 🚀 Getting Started

## Prerequisites

- .NET SDK
- MySQL Server
- Redis — if enabled by the repository configuration
- Git

Check .NET:

```bash
dotnet --version
```

Check MySQL:

```bash
mysql --version
```

Check Redis if installed:

```bash
redis-cli ping
```

Expected:

```text
PONG
```

---

## Clone

```bash
git clone <repository-url>
cd <backend-project-directory>
```

## Restore

```bash
dotnet restore
```

## Configure Database

Example configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=restaurant_management;User=root;Password=YOUR_PASSWORD;"
  }
}
```

Never commit production credentials or secrets.

## Configure JWT

```json
{
  "Jwt": {
    "Key": "YOUR_DEVELOPMENT_SECRET",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
  }
}
```

## Run

```bash
dotnet build
dotnet run
```

---

# 📚 API Documentation

If Swagger/OpenAPI is enabled, run the application and open the configured Swagger endpoint.

Swagger can be used to:

- Explore endpoints
- Inspect request/response models
- Test APIs
- Provide authentication tokens
- Understand API contracts

---

# 🔭 Future Improvements

Potential improvements include:

- Comprehensive unit and integration testing
- Docker containerization
- CI/CD automation
- Centralized logging and observability
- Distributed tracing
- Message broker integration
- More advanced Redis caching strategies
- Health checks and readiness probes
- API gateway integration
- Automated database deployment
- Advanced restaurant analytics

---

# 👨‍💻 Skills Demonstrated

```text
Backend Engineering
│
├── ASP.NET Core Web API
├── C#
├── RESTful API Design
├── JWT Authentication
├── RBAC
├── Middleware
├── API Versioning
├── Rate Limiting
├── SignalR
├── Dependency Injection
├── SOLID Principles
│
├── Data Access
│   ├── Dapper
│   ├── MySQL
│   ├── Stored Procedures
│   └── Transactions
│
├── Performance
│   └── Redis
│
└── Architecture
    ├── Layered Architecture
    └── Microservices
```

---

# 🏢 Internship Context

**Software Engineering Intern — InfoDevelopers Pvt. Ltd.**  
**Sanepa, Lalitpur, Nepal**  
**June 2026 – Present**

The internship provided practical exposure to enterprise backend development, including REST API development, authentication and authorization, middleware, database optimization, distributed service architecture, real-time communication, caching, and software design principles.

InfoDevelopers describes itself as a software development and IT consultancy company with a strong focus on financial technology and enterprise solutions. citeturn0search3turn0search0

---

<div align="center">

### 🍽️ Restaurant Management System

**Designed to demonstrate real-world backend engineering with ASP.NET Core.**

<br/>

**ASP.NET Core • C# • Dapper • MySQL • SignalR • JWT • Redis • Microservices**

<br/>

⭐ If this project helped you understand backend engineering, consider giving the repository a star.

</div>
