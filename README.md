# Form Management System

## Introduction
The Form Management System is a web application designed to manage forms, categories, departments, and users. It provides a comprehensive solution for handling form submissions, user management, and categorization. The system is built using ASP.NET Core and Entity Framework Core, ensuring a robust and scalable architecture.

## Table of Contents
- [Features](#features)
- [Technology Used](#technology-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features
- User authentication and authorization
- Form submission and management
- Category and department management
- Role-based access control
- Responsive user interface

## Technology Used
- **Backend**: ASP.NET Core 8.0, Entity Framework Core 8.0
- **Frontend**: Razor Pages, Bootstrap
- **Database**: SQL Server
- **Others**: AutoMapper, ILogger

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 or later

### Installation
1. Clone the repository:
```
git clone https://github.com/gnaohuv/SRMS.git
```

2. Navigate to the solution directory:
```
cd SFMS
```

3. Restore the dependencies:
```
dotnet restore
```

4. Update the database connection string in `appsettings.json`:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
}
```

5. Apply the migrations to create the database schema:
```
dotnet ef database update
```

6. Run the application:
```
dotnet run
```


## Usage
- Navigate to `http://localhost:7295` in your web browser. `https://localhost:7066/swagger/index.html` for Swagger API documentation.
- Use the provided UI to manage forms, categories, departments, and users.
- Admin users can create, update, and delete entities, while regular users have restricted access based on their roles.

## Contributing
Contributions are welcome! Please fork the repository and create a pull request with your changes. Ensure that your code follows the project's coding standards and includes appropriate tests.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
