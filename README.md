# Project Management System (PMS)

A web-based Project Management System developed using ASP.NET Core and Entity Framework Core. The system is designed to centralize project-related information, streamline administrative workflows, and provide structured access control for different user groups.

## Tech Stack

### Backend
- **ASP.NET Core** – Web application framework
- **C#** – Primary programming language
- **Entity Framework Core** – Object-relational mapping (ORM)
- **Pomelo.EntityFrameworkCore.MySql** – MySQL/MariaDB database provider

### Frontend
- **Razor Pages** – Server-rendered web interface
- **Bootstrap** – Responsive UI components
- **JavaScript** – Client-side interactions

### Database
- **MySQL / MariaDB** – Relational database management system

### Additional Libraries
- PDF generation library – Document generation and export

## Key Features

### 1. Project Management
- Create, view, update, and delete project records.
- Manage project-related information.
- Search and filter project records.
- View project information through structured listings.

### 2. Payment Management
- Manage project payment records.
- View payment information.
- Search and filter payment records.
- Generate PDF documents for payment-related information.

### 3. User Management
- Manage system user accounts.
- Organize users into different user groups.
- Control access to system functionalities.

### 4. Role-Based Access Control

The system implements permission management using three main database entities:

| Entity | Responsibility |
|---|---|
| User | Stores system user information |
| User Group | Defines user groups |
| User Group Access | Defines permissions assigned to user groups |

The permission system supports the following operations:

- Create
- Read
- Update
- Delete

This allows system administrators to control which functionalities are accessible to different user groups.

### 5. PDF Generation
- Generate PDF documents from system records.
- Support document generation for relevant modules.

## Database Design

The application uses MySQL/MariaDB as its relational database.

Entity Framework Core is used to interact with the database, while database scaffolding is used to generate entity models from the existing database schema.

## Application Architecture

The project follows the ASP.NET Core Razor Pages approach rather than the traditional MVC controller-and-view structure.

The application separates responsibilities into:

- **Pages** – Razor Pages and their corresponding PageModels.
- **Models** – Entity models and application data structures.
- **Data** – Database context and Entity Framework Core configuration.
- **Services** – Reusable business logic and application services.

## Getting Started

### Prerequisites

Make sure you have installed:

- .NET SDK compatible with the project
- Visual Studio 2022 or later
- MySQL or MariaDB
- MySQL Workbench (optional)

### 1. Clone the Repository

```bash
git clone <YOUR_REPOSITORY_URL>
```

Navigate into the project directory:

```bash
cd PMS
```

### 2. Configure the Database

Make sure your MySQL/MariaDB server is running.

Update the database connection string in your application configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=pms;User=root;Password=YOUR_PASSWORD;"
  }
}
```

Use your own database credentials. Do not commit production credentials to GitHub.

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Application

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

Open the local URL displayed in the terminal.

## Project Structure

```text
PMS/
├── Data/                 # Database context and configuration
├── Models/               # Entity models and DTOs
├── Pages/                # Razor Pages and PageModels
│   ├── Project/
│   └── ...
├── Services/             # Application services
├── wwwroot/              # Static assets (CSS, JS, images)
├── appsettings.json      # Application configuration
├── Program.cs            # Application entry point
└── README.md
```

*The directory structure is illustrative and should be adjusted to match the actual repository.*

## Security and Access Control

The application uses a group-based permission model to manage access to different system functionalities.

Database credentials and other sensitive configuration values should be maintained outside source control.

## Future Improvements

- Expand project management functionality.
- Enhance reporting and PDF export capabilities.
- Improve permission management.
- Enhance dashboard and data visualization.
- Improve application security and validation.

## License

This project is currently maintained as a personal project. License details: **TODO**.
