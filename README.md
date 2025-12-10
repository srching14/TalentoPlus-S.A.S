# 🚀 TalentoPlus S.A.S. - Sistema de Gestión de Empleados

Sistema de gestión de empleados integral desarrollado con **ASP.NET Core 8.0** que incluye aplicación MVC para administración de RRHH, API REST para autoservicio de empleados, dashboard con IA, pruebas automatizadas y soporte completo para Docker.

**🔗 Repositorio:** [https://github.com/srching14/TalentoPlus-S.A.S.git](https://github.com/srching14/TalentoPlus-S.A.S.git)

---

## ⚠️ IMPORTANTE - INFORMACIÓN RÁPIDA

### 🔗 Link del Repositorio
```
https://github.com/srching14/TalentoPlus-S.A.S.git
```

### 🚀 Pasos para Correr la Solución

**Opción 1: Con Docker (Recomendado)**
```bash
# 1. Clonar el repositorio
git clone https://github.com/srching14/TalentoPlus-S.A.S.git
cd TalentoPlus-S.A.S

# 2. Levantar todos los servicios
docker compose up -d

# 3. Acceder a las aplicaciones
# Web (Admin): http://localhost:5000
# API: http://localhost:5001
```

**Opción 2: Desarrollo Local**
```bash
# 1. Clonar el repositorio
git clone https://github.com/srching14/TalentoPlus-S.A.S.git
cd TalentoPlus-S.A.S

# 2. Configurar variables de entorno (copiar .env.example a .env)
cp .env.example .env

# 3. Ejecutar la aplicación Web
cd PruebaDeDesempeño.Web
dotnet run

# 4. (En otra terminal) Ejecutar la API
cd PruebaDeDesempeño.API
dotnet run
```

### ⚙️ Configuración de Variables de Entorno

Crear archivo `.env` en la raíz del proyecto con:

```env
# Base de datos
CONNECTION_STRING=Host=localhost;Port=5434;Database=pruebadedesempeno_db;Username=postgres;Password=Qwe.123

# JWT
JWT_KEY=SuperSecretKeyForJWTAuthenticationPruebaDeDesempeno2024!

# Email (Mailtrap para testing)
SMTP_SERVER=sandbox.smtp.mailtrap.io
SMTP_PORT=2525
SMTP_USERNAME=tu_usuario_mailtrap
SMTP_PASSWORD=tu_password_mailtrap

# Gemini AI (opcional)
GEMINI_API_KEY=tu_api_key_aqui
```

### 🔑 Credenciales de Acceso

| Rol | Email | Contraseña |
|-----|-------|------------|
| **Administrador (Web)** | `admin@talentoplusadmin.com` | `Admin123!` |
| **Empleado (API)** | Registrarse en `/api/employees/register` | Generada y enviada por email |

### 📍 URLs de Acceso

| Servicio | URL Local | URL Docker |
|----------|-----------|------------|
| **Web App (Admin)** | http://localhost:5086 | http://localhost:5000 |
| **API Swagger** | http://localhost:5001/swagger | http://localhost:5001/swagger |
| **PostgreSQL** | localhost:5433 | localhost:5434 |

---

## 📋 Tabla de Contenidos

- [Características](#características)
- [Tecnologías](#tecnologías)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Instalación y Configuración](#instalación-y-configuración)
- [Ejecución](#ejecución)
- [Credenciales de Acceso](#credenciales-de-acceso)
- [API REST](#api-rest)
- [Docker](#docker)
- [Tests](#tests)
- [Funcionalidades Principales](#funcionalidades-principales)

---

## ✨ Características

El sistema cuenta con las siguientes funcionalidades completas:

- ✅ **Aplicación Web MVC (Admin)** - Gestión integral de empleados y departamentos.
- ✅ **API REST (Autoservicio)** - Endpoints para empleados con autenticación JWT.
- ✅ **Importación Masiva** - Carga de empleados desde Excel con validación.
- ✅ **Generación de Hoja de Vida** - Descarga de CV en PDF dinámico.
- ✅ **Dashboard con IA** - KPIs y consultas en lenguaje natural (Gemini).
- ✅ **Autenticación Híbrida** - Identity para Admin, JWT para Empleados.
- ✅ **Sistema de Email** - Envío real de credenciales y notificaciones.
- ✅ **Docker Support** - Contenedorización completa de la solución.
- ✅ **Pruebas Automatizadas** - Tests unitarios e integración.

---

## 🛠️ Tecnologías

### Backend
- **ASP.NET Core 8.0 MVC** - Panel Administrativo
- **ASP.NET Core 8.0 Web API** - API REST
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL 16** - Base de datos
- **ASP.NET Identity** - Auth Admin

### Librerías y Paquetes
- **EPPlus** - Procesamiento de Excel
- **QuestPDF** - Generación de PDFs
- **MailKit** - Servicio SMTP
- **Google.GenerativeAI** - Integración con Gemini
- **Swashbuckle** - Swagger/OpenAPI
- **JWT Bearer** - Seguridad API
- **xUnit + Moq** - Testing

### DevOps
- **Docker & Docker Compose**
- **Git**

---

## 📁 Estructura del Proyecto

```
PruebaDeDesempeño/
├── PruebaDeDesempeño.Web/              # 🌐 Aplicación MVC (Admin)
│   ├── Controllers/                     # Employees, Departments, Dashboard, Chatbot
│   ├── Views/                           # Vistas Razor
│   ├── Services/                        # Excel, PDF, Email, Gemini, EmployeeService
│   ├── Models/                          # Entidades (Employee, Department)
│   └── Data/                            # DbContext
│
├── PruebaDeDesempeño.API/              # 🔌 API REST (Empleados)
│   ├── Controllers/                     # Auth, EmployeeSelfService, Departments
│   └── DTOs/                            # Data Transfer Objects
│
├── PruebaDeDesempeño.Tests/            # 🧪 Pruebas Automatizadas
│   ├── Services/                        # Unit Tests (EmployeeService)
│   └── Integration/                     # Integration Tests (API)
│
├── Dockerfile                           # Docker Web
├── Dockerfile.api                       # Docker API
├── docker-compose.yml                   # Orquestación
└── README.md                           # Documentación
```

---

## 🔧 Instalación y Configuración

### Requisitos Previos

- **.NET 8.0 SDK**
- **PostgreSQL 16**
- **Docker Desktop** (opcional)

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/srching14/TalentoPlus-S.A.S.git
   ```

2. **Configurar Base de Datos**
   Editar `appsettings.json` o usar variables de entorno.

3. **Aplicar Migraciones**
   ```bash
   cd PruebaDeDesempeño.Web
   dotnet ef database update
   ```

---

## ▶️ Ejecución

### Opción 1: Desarrollo Local

#### Web App (Admin)
```bash
cd PruebaDeDesempeño.Web
dotnet run
```
Acceder a: http://localhost:5086

#### API REST
```bash
cd PruebaDeDesempeño.API
dotnet run
```
Acceder a: http://localhost:5001

### Opción 2: Docker Compose

```bash
docker-compose up -d
```
- Web: http://localhost:5000
- API: http://localhost:5001

---

## 🔌 API REST

### Documentación Swagger
Acceder a: **http://localhost:5001**

### Endpoints Principales

#### 🔓 Públicos
- `GET /api/departments` - Listar departamentos
- `POST /api/employees/register` - Autoregistro de empleado
- `POST /api/employees/login` - Login de empleado

#### 🔐 Privados (Bearer Token)
- `GET /api/employees/me` - Ver mi información
- `GET /api/employees/me/cv` - Descargar mi Hoja de Vida (PDF)

---

## 🧪 Tests

### Ejecutar Tests
```bash
cd PruebaDeDesempeño.Tests
dotnet test
```

### Cobertura
- **Unit Tests**: Validan la lógica de negocio de `EmployeeService` (filtros, búsquedas, reglas).
- **Integration Tests**: Validan los endpoints de la API y la persistencia en base de datos (in-memory).

---

## 🎯 Funcionalidades Principales

### 1. Gestión de Empleados (Web)
- CRUD completo de empleados.
- Asignación a departamentos.
- Soft Delete (Inactivación).

### 2. Importación Masiva
- Carga de archivo Excel (`Empleados.xlsx`).
- Validación de estructura y datos obligatorios.
- Creación automática de usuarios.

### 3. Dashboard con IA
- Tarjetas de métricas (Total, Vacaciones, Activos).
- Chatbot integrado con Gemini para preguntas como:
  - "¿Cuántos empleados hay en Tecnología?"
  - "¿Cuál es el salario promedio?"

### 4. Autoservicio (API)
- Los empleados pueden registrarse y recibir su contraseña por email.
- Login seguro con JWT.
- Descarga de Hoja de Vida en PDF autogenerado.

---

## 📝 Licencia
Este proyecto es de código abierto y está disponible bajo la licencia MIT.

Desarrollado por Elias Ching – Full Stack Developer
srching23@gmail.com
srching14
Barranquilla, Atlántico
