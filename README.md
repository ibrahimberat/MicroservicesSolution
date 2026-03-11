# Microservices Solution - Backend Developer Task

This project implements a microservices-based solution with Onion Architecture, following SOLID principles and 12-Factor App methodology.

## Architecture Overview

- **Auth Service**: JWT authentication with refresh token mechanism
- **Product Service**: Product management with CQRS pattern and Redis caching
- **Log Service**: Centralized logging with structured logging
- **API Gateway**: YARP-based gateway with rate limiting
- **Event-Driven Architecture**: RabbitMQ for async communication
- **SAGA Pattern**: Distributed transaction management

## Technology Stack

- **.NET 8.0**
- **C#**
- **Entity Framework Core**
- **SQL Server**
- **Redis**
- **RabbitMQ**
- **Seq**
- **YARP Reverse Proxy**
- **Docker & Docker Compose**
- **GitHub Actions (CI/CD)**

## Prerequisites

- .NET 8.0 SDK
- Docker Desktop
- Visual Studio 2022 (recommended) or VS Code
- Git

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/microservices-solution.git
cd microservices-solution