# RaceDay2
# RaceDay – Event Management API

## 1. Project Overview

RaceDay is a web-based event management API developed to support the management of running, walking and cycling events. The system provides functionality for organisers to manage events and participants, while participants can register, enrol in events and view their results.

The purpose of the project is to make event management more organised by storing information in a central database and providing API endpoints for the required operations.

## 2. Main Features

- User registration and login.
- Role-based access for Organisers and Participants.
- User profile management.
- Create, view, update and delete events, subject to user permissions.
- Create and view event categories.
- Participant enrolment in event categories.
- Recording and viewing race results.
- Route information associated with events.
- Weather records associated with events.
- API documentation through Swagger.
- Automated tests for application functionality and validation.
- Automated build and test workflow through GitHub Actions.

## 3. Technologies Used

- C#
- ASP.NET Core Web API
- .NET 9
- Entity Framework Core
- Microsoft SQL Server
- Swagger / OpenAPI
- xUnit
- Git and GitHub
- GitHub Actions

## 4. User Roles

### Organiser

An organiser can manage race events and their associated information. Organiser-only operations are protected by role-based authorisation.

### Participant

A participant can access permitted event information, enrol in event categories and view their own results and profile information.

Access to individual endpoints depends on the authentication and authorisation rules implemented in the API.

## 5. Requirements

Before running the project, install:

- Visual Studio 2022 with the ASP.NET and web development workload, or a compatible .NET development environment.
- .NET 9 SDK.
- SQL Server or a compatible SQL Server LocalDB installation.
- Git, if you want to clone the repository.

## 6. Database Configuration

RaceDay uses Entity Framework Core with SQL Server. The database context is defined in `RaceDayContext.cs`, and the database schema is managed through Entity Framework Core migrations.

Configure the connection string in `appsettings.json` or through an appropriate development configuration.

Example for SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "RaceDayConnection": "Server=(localdb)\\ProjectModels;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Your connection string may need to be changed to match your SQL Server installation.

Do not commit production passwords, private keys or other secrets to the repository.

dotnet test RaceDay.sln