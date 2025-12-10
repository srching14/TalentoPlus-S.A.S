# TalentoPlus S.A.S. - Employee Management System

A comprehensive employee management system developed with **ASP.NET Core 8.0** that includes an MVC application for HR administration, REST API for employee self-service, AI-powered dashboard, automated tests, and full Docker support.

**Repository:** [https://github.com/srching14/TalentoPlus-S.A.S.git](https://github.com/srching14/TalentoPlus-S.A.S.git)

---

## IMPORTANT - QUICK INFORMATION

### Repository Link
```
https://github.com/srching14/TalentoPlus-S.A.S.git
```

### Steps to Run the Solution

**Option 1: With Docker (Recommended)**
```bash
# 1. Clone the repository
git clone https://github.com/srching14/TalentoPlus-S.A.S.git
cd TalentoPlus-S.A.S

# 2. Start all services
docker compose up -d

# 3. Access the applications
# Web (Admin): http://localhost:5000
# API: http://localhost:5001
```

**Option 2: Local Development**
```bash
# 1. Clone the repository
git clone https://github.com/srching14/TalentoPlus-S.A.S.git
cd TalentoPlus-S.A.S

# 2. Configure environment variables (copy .env.example to .env)
cp .env.example .env

# 3. Run the Web application
cd PruebaDeDesempeño.Web
dotnet run

# 4. (In another terminal) Run the API
cd PruebaDeDesempeño.API
dotnet run
```

### Environment Variables Configuration

Create a `.env` file in the project root with:

```env
# Database
CONNECTION_STRING=Host=localhost;Port=5434;Database=pruebadedesempeno_db;Username=postgres;Password=Qwe.123

# JWT
JWT_KEY=SuperSecretKeyForJWTAuthenticationPruebaDeDesempeno2024!

# Email (Mailtrap for testing)
SMTP_SERVER=sandbox.smtp.mailtrap.io
SMTP_PORT=2525
SMTP_USERNAME=your_mailtrap_username
SMTP_PASSWORD=your_mailtrap_password

# Gemini AI (optional)
GEMINI_API_KEY=your_api_key_here
```

### Access Credentials

| Role | Email | Password |
|-----|-------|----------|
| **Administrator (Web)** | `admin@talentoplusadmin.com` | `Admin123!` |
| **Employee (API)** | Register at `/api/employees/register` | Generated and sent by email |

### Access URLs

| Service | Local URL | Docker URL |
|---------|-----------|------------|
| **Web App (Admin)** | http://localhost:5086 | http://localhost:5000 |
| **API Swagger** | http://localhost:5001/swagger | http://localhost:5001/swagger |
| **PostgreSQL** | localhost:5433 | localhost:5434 |

---

## Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Project Structure](#project-structure)
- [Installation and Configuration](#installation-and-configuration)
- [Execution](#execution)
- [Access Credentials](#access-credentials)
- [REST API](#rest-api)
- [Docker](#docker)
- [Tests](#tests)
- [Main Features](#main-features)

---

## Features

The system includes the following complete functionalities:

- **MVC Web Application (Admin)** - Comprehensive employee and department management.
- **REST API (Self-Service)** - Endpoints for employees with JWT authentication.
- **Bulk Import** - Employee upload from Excel with validation.
- **Resume Generation** - Dynamic CV download in PDF format.
- **AI Dashboard** - KPIs and natural language queries (Gemini).
- **Hybrid Authentication** - Identity for Admin, JWT for Employees.
- **Email System** - Real sending of credentials and notifications.
- **Docker Support** - Complete containerization of the solution.
- **Automated Tests** - Unit and integration tests.

---

## Technologies

### Backend
- **ASP.NET Core 8.0 MVC** - Admin Panel
- **ASP.NET Core 8.0 Web API** - REST API
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL 16** - Database
- **ASP.NET Identity** - Admin Auth

### Libraries and Packages
- **EPPlus** - Excel Processing
- **QuestPDF** - PDF Generation
- **MailKit** - SMTP Service
- **Google.GenerativeAI** - Gemini Integration
- **Swashbuckle** - Swagger/OpenAPI
- **JWT Bearer** - API Security
- **xUnit + Moq** - Testing

### DevOps
- **Docker & Docker Compose**
- **Git**

---

## Project Structure

```
PruebaDeDesempeño/
├── PruebaDeDesempeño.Web/              # MVC Application (Admin)
│   ├── Controllers/                     # Employees, Departments, Dashboard, Chatbot
│   ├── Views/                           # Razor Views
│   ├── Services/                        # Excel, PDF, Email, Gemini, EmployeeService
│   ├── Models/                          # Entities (Employee, Department)
│   └── Data/                            # DbContext
│
├── PruebaDeDesempeño.API/              # REST API (Employees)
│   ├── Controllers/                     # Auth, EmployeeSelfService, Departments
│   └── DTOs/                            # Data Transfer Objects
│
├── PruebaDeDesempeño.Tests/            # Automated Tests
│   ├── Services/                        # Unit Tests (EmployeeService)
│   └── Integration/                     # Integration Tests (API)
│
├── Dockerfile                           # Docker Web
├── Dockerfile.api                       # Docker API
├── docker-compose.yml                   # Orchestration
└── README.md                           # Documentation
```

---

## Installation and Configuration

### Prerequisites

- **.NET 8.0 SDK**
- **PostgreSQL 16**
- **Docker Desktop** (optional)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/srching14/TalentoPlus-S.A.S.git
   ```

2. **Configure Database**
   Edit `appsettings.json` or use environment variables.

3. **Apply Migrations**
   ```bash
   cd PruebaDeDesempeño.Web
   dotnet ef database update
   ```

---

## Execution

### Option 1: Local Development

#### Web App (Admin)
```bash
cd PruebaDeDesempeño.Web
dotnet run
```
Access at: http://localhost:5086

#### REST API
```bash
cd PruebaDeDesempeño.API
dotnet run
```
Access at: http://localhost:5001

### Option 2: Docker Compose

```bash
docker-compose up -d
```
- Web: http://localhost:5000
- API: http://localhost:5001

---

## REST API

### Swagger Documentation
Access at: **http://localhost:5001**

### Main Endpoints

#### Public
- `GET /api/departments` - List departments
- `POST /api/employees/register` - Employee self-registration
- `POST /api/employees/login` - Employee login

#### Private (Bearer Token)
- `GET /api/employees/me` - View my information
- `GET /api/employees/me/cv` - Download my Resume (PDF)

---

## Tests

### Run Tests
```bash
cd PruebaDeDesempeño.Tests
dotnet test
```

### Coverage
- **Unit Tests**: Validate business logic of `EmployeeService` (filters, searches, rules).
- **Integration Tests**: Validate API endpoints and database persistence (in-memory).

---

## Main Features

### 1. Employee Management (Web)
- Complete CRUD for employees.
- Department assignment.
- Soft Delete (Deactivation).

### 2. Bulk Import
- Excel file upload (`Empleados.xlsx`).
- Structure and required data validation.
- Automatic user creation.

### 3. AI Dashboard
- Metrics cards (Total, On Vacation, Active).
- Chatbot integrated with Gemini for questions like:
  - "How many employees are in Technology?"
  - "What is the average salary?"

### 4. Self-Service (API)
- Employees can register and receive their password by email.
- Secure login with JWT.
- Auto-generated Resume download in PDF.

---

# License

This project is open source and available under the MIT license.

---

**Developed by Elias Ching – Full Stack Developer**

- Email: srching23@gmail.com
- GitHub: srching14
- Location: Barranquilla, Atlántico

**Developed for TalentoPlus S.A.S.**
