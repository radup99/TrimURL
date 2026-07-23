# TrimURL

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![Build](https://github.com/raduplacinta99/TrimURL/actions/workflows/dotnet-desktop.yml/badge.svg)

TrimURL is a RESTful URL shortening API built with ASP.NET Core. It allows users to create, manage, and track shortened URLs while demonstrating modern backend development practices such as layered architecture, authentication, caching, unit testing, Docker, and CI/CD.

This project was built as part of my backend portfolio to showcase clean architecture and production-ready development practices.

## Live Demo

- **Swagger UI:** https://trimurlapi.azurewebsites.net/

## Features

### URL Management

- Create shortened URLs
- Redirect shortened URLs
- Set URL expiration dates
- Update existing URLs
- Delete URLs
- Retrieve URLs by creator

### Authentication

- JWT Authentication & Authorization
- User registration and login

### Security

- Rate limiting
- Role based authorization

### Quality

- Global exception handling middleware
- Redis and In-Memory caching
- Unit tests using xUnit + Moq
- GitHub Actions CI

## Tech Stack

**Backend**
- ASP.NET Core 8

**Database**
- PostgreSQL
- Entity Framework Core

**Unit Testing**
- xUnit

## Architecture

The project follows a layered architecture.

```
Controllers
      ↓
Services
      ↓
Repositories
      ↓
Entity Framework Core
      ↓
PostgreSQL
```

Responsibilities:

- Controllers handle HTTP requests.
- Services contain business logic.
- Repositories handle database access.

## Getting Started

### Prerequisites

- .NET 8 SDK

### Clone the repository

```bash
git clone https://github.com/raduplacinta99/TrimURL

cd TrimURL/TrimUrlApi
```

### Restore packages

```bash
dotnet restore
```

### Apply migrations

```bash
dotnet ef database update
```

### Run

```bash
dotnet run
```

### Running Tests

```bash
cd TrimURL/TrimUrlApi.Tests
dotnet test
```

## Future Improvements

- Frontend Web Application

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
