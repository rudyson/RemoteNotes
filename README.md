# dnu-fpecs-master-istk
ДНУ ФФЕКС 5.1_2.1_ОКП - Інформаційна стійкість комп'ютерних технологій 9 варіант

## Summary

This application is the showcase of WPF MVVM & ASP.NET Web API knowledge.

| Aspect | Description |
| ------ | ----------- |
| Core functionality | CRUD operations on Notes (Create, Read, Update, Delete) |
| Technology | [.NET 8](https://dotnet.microsoft.com/en-us/)|
| Database | [PostgreSQL](https://www.postgresql.org/)|
| Security |  [JWT](https://jwt.io/) Authorization|
| Authentication | Login/Logout system with user session management (see `IUserStore`) |
| Features | - User registration & authentication |
|          | - CRUD operations for Notes |
|          | - Advanced search and filtering of Notes (by title, content & creation date) |
|          | - Data binding with MVVM pattern |
|          | - Client-server communication using asynchronous requests |
| Testing | - Unit and integration tests following TDD and BDD principles |
| Development approach | SOLID, DRY, and clean code principles |
| Client | Implements robust error handling and network request cancellation |

## Installation

### Clone this repository:
```bash
   git clone https://github.com/rudyson/dnu-fpecs-master-istk.git
```

### Set up the backend:
Ensure you have PostgreSQL installed.
Set up the database using the provided schema.
Apply database migration.

### Set up the frontend:
Open the solution in Visual Studio (or another compatible IDE).
Restore the NuGet packages.

### Configure the app:
Set the backend API URL and JWT authentication in the app configuration files.

### Run the app:
Start the backend API using Visual Studio or `dotnet run`.
Start the WPF application in the IDE.

## API Endpoints

| Method | Path | Description |
| - | - | - |
| POST | /api/auth/login |  Log in and get a JWT token & user id. |
| POST | /api/auth/register |  Create an account. |
| GET | /api/notes |  Get all notes for the logged-in user. |
| GET | /api/notes/{id} |  Get specific note. |
| POST | /api/notes |  Create a new note.|
| PUT | /api/notes/{id} | Update an existing note.|
| DELETE | /api/notes/{id} | Delete a note.|

## Running Tests

To run the unit and integration tests, you can use the following commands in the solution root:

```bash
dotnet test
```

Ensure that your backend API is running if you need to run integration tests.