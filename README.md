# Boilerplate .NET API

A clean and modular boilerplate for building RESTful APIs using .NET, designed to streamline development and enforce best practices.

## Features

- **.NET**: Leverages the latest features and enhancements.
- **Clean Architecture**: Organized into layers for maintainability and scalability.
- **Dependency Injection**: Built-in DI for seamless service registration.
- **Entity Framework Core**: Simplifies data access with LINQ queries and migrations.
- **Global Exception Handling**: Standardized error responses with `ProblemDetails`.
- **Validation**: Integrated validation using FluentValidation.
- **Unit Testing**: Ready-to-use test setup with xUnit and Moq.
- **JWT Authentication**: Secure APIs with JSON Web Tokens.
- **Health Check**: Endpoint that checks the health of the application, including the status of the database and other critical services. Returns whether the application is working correctly or if there are errors to be resolved.

## Prerequisites

- .NET SDK 8.0 or later
- SQL Server or SQLite (for local development)
- A code editor (e.g., Visual Studio, Visual Studio Code)

## Getting Started

Follow these steps to set up and run the project locally:

### Clone the Repository

```bash
git clone https://github.com/gcmorais/boilerplate-dotnet-api.git
cd boilerplate-dotnet-api
`````
### Setup Database

1. Configure the database connection string, sendgrid connection and JWT keys in *appsettings.Development.json*.
2. Apply migrations:

```bash
dotnet ef database update --project src/project.Infrastructure --startup-project src/project.Api
`````

### Run the API

```bash
dotnet run --project src/WebApi
`````

## Project Structure
The project follows a modular structure:
```bash
src/
├── Application/       # Business logic and service interfaces
├── Domain/            # Core entities and domain logic
├── Infrastructure/    # Data access and external services
└── WebApi/            # API controllers and entry points
tests/
└── Application.UnitTests/  # Unit tests for Application layer
`````

## Key Technologies
- .NET 8
- Entity Framework Core
- MediatR
- FluentValidation
- xUnit & Moq

## Contributing
Contributions are welcome! Feel free to open issues or submit pull requests to improve this project.

## License
This project is licensed under the [MIT License.](https://en.wikipedia.org/wiki/MIT_License)
