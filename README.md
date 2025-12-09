# 🚀 TalentoPlus S.A.S. - Sistema de Gestión de Empleados

Sistema de gestión de empleados integral desarrollado con **ASP.NET Core 8.0** que incluye aplicación MVC, API REST, pruebas unitarias y soporte completo para Docker.

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
# Web: http://localhost:5000
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
SMTP_USERNAME=45c32d48bac825
SMTP_PASSWORD=ecb729df6118f586e189bfe5a4f5b293

# Gemini AI (opcional)
GEMINI_API_KEY=tu_api_key_aqui
```

### 🔑 Credenciales de Acceso

| Rol | Email | Contraseña |
|-----|-------|------------|
| **Administrador** | `admin@talentoplusadmin.com` | `Admin123!` |
| **Cliente** | Registrarse en `/Account/Register` | Mínimo 6 caracteres |

### 📍 URLs de Acceso

| Servicio | URL Local | URL Docker |
|----------|-----------|------------|
| **Web App** | http://localhost:5087 | http://localhost:5000 |
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

- ✅ **Aplicación Web MVC** - Interfaz administrativa completa
- ✅ **API REST** - Endpoints RESTful con Swagger/OpenAPI
- ✅ **Importación/Exportación Excel** - EPPlus para manejo de archivos
- ✅ **Generación de PDF** - Facturas y reportes con QuestPDF
- ✅ **Autenticación Completa** - ASP.NET Identity con roles
- ✅ **JWT Authentication** - Para consumo de API
- ✅ **Chatbot con IA** - Asistente virtual para consultas
- ✅ **Sistema de Email** - Notificaciones automáticas con MailKit
- ✅ **Portal de Clientes** - Área personalizada para clientes
- ✅ **Soft Delete** - Eliminación lógica de registros
- ✅ **Docker Support** - Contenedorización completa
- ✅ **Unit Tests** - Pruebas con xUnit y Moq (100% passing)

---

## 🛠️ Tecnologías

### Backend
- **ASP.NET Core 8.0 MVC** - Framework web principal
- **ASP.NET Core 8.0 Web API** - API REST
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL 16** - Base de datos relacional
- **ASP.NET Identity** - Sistema de autenticación

### Librerías y Paquetes
- **EPPlus 7.0** - Importación/Exportación de Excel
- **QuestPDF 2024** - Generación de documentos PDF
- **MailKit** - Envío de correos electrónicos
- **Swashbuckle.AspNetCore** - Documentación Swagger/OpenAPI
- **JWT Bearer** - Autenticación para API
- **xUnit + Moq** - Framework de pruebas unitarias
- **Npgsql** - Driver PostgreSQL para .NET

### DevOps
- **Docker & Docker Compose** - Contenedorización
- **Git** - Control de versiones

---

## 📁 Estructura del Proyecto

```
PruebaDeDesempeño/
├── PruebaDeDesempeño.Web/              # 🌐 Aplicación MVC (Puerto 5086)
│   ├── Controllers/                     # 9 Controllers MVC
│   ├── Views/                           # 20+ Vistas Razor
│   ├── Models/                          # Entidades de dominio
│   ├── ViewModels/                      # ViewModels para vistas
│   ├── Services/                        # Lógica de negocio
│   ├── Data/                            # DbContext y configuraciones
│   └── wwwroot/                         # Archivos estáticos
│
├── PruebaDeDesempeño.API/              # 🔌 API REST (Puerto 5001)
│   ├── Controllers/                     # 3 API Controllers
│   │   ├── AuthApiController.cs         # Login/Register JWT
│   │   ├── ProductsApiController.cs     # CRUD Productos
│   │   └── ChatbotApiController.cs      # Consultas IA
│   ├── DTOs/                            # Data Transfer Objects
│   ├── Program.cs                       # Configuración API
│   └── appsettings.json                 # Configuración
│
├── PruebaDeDesempeño.Tests/            # 🧪 Tests Unitarios
│   ├── Services/                        # Tests de servicios
│   └── Controllers/                     # Tests de controllers
│
├── Dockerfile                           # Docker para Web MVC
├── Dockerfile.api                       # Docker para API
├── docker-compose.yml                   # Orquestación de servicios
└── PruebaDeDesempeño.sln               # Solución con 3 proyectos
```

---

## 🔧 Instalación y Configuración

### Requisitos Previos

- **.NET 8.0 SDK** - [Descargar](https://dotnet.microsoft.com/download)
- **PostgreSQL 16** - [Descargar](https://www.postgresql.org/download/)
- **Visual Studio 2022** o **VS Code** (opcional)
- **Docker Desktop** (opcional, solo para ejecución en contenedores)

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone <url-del-repositorio>
   cd PruebaDeDesempeño
   ```

2. **Configurar Base de Datos**
   
   Editar `appsettings.json` en ambos proyectos con tu configuración de PostgreSQL:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5433;Database=pruebadedesempeno_db;Username=postgres;Password=TU_PASSWORD"
   }
   ```

3. **Aplicar Migraciones**
   ```bash
   cd PruebaDeDesempeño.Web
   dotnet ef database update
   ```

4. **Restaurar Paquetes**
   ```bash
   dotnet restore
   ```

---

## ▶️ Ejecución

### Opción 1: Desarrollo Local (Recomendado)

#### Ejecutar Aplicación Web (MVC)
```bash
cd PruebaDeDesempeño.Web
dotnet run
```
- **URL**: http://localhost:5086
- **Funcionalidad**: Panel administrativo completo

#### Ejecutar API REST
```bash
cd PruebaDeDesempeño.API
dotnet run
```
- **URL**: http://localhost:5001
- **Swagger**: http://localhost:5001 (raíz)

#### Ejecutar Ambos Simultáneamente
Abrir dos terminales y ejecutar cada proyecto en su respectiva terminal.

### Opción 2: Con Docker

```bash
# Levantar todos los servicios
docker-compose up -d

# Ver logs en tiempo real
docker-compose logs -f

# Detener servicios
docker-compose down
```

**Servicios disponibles:**
- Web MVC: http://localhost:5000
- API REST: http://localhost:5001
- PostgreSQL: localhost:5433

---

## 🔑 Credenciales de Acceso

### Base de Datos (PostgreSQL)

```
Host: localhost
Puerto: 5433
Database: pruebadedesempeno_db
Usuario: postgres
Password: Qwe.123
```

### Aplicación Web - Usuario Administrador

```
Email: admin@pruebadedesempeno.com
Password: Admin123!
Rol: Administrador
```

**Permisos del Administrador:**
- Acceso completo al dashboard
- CRUD de Productos, Clientes y Ventas
- Importación/Exportación de Excel
- Generación de facturas PDF
- Acceso al chatbot IA

### Aplicación Web - Usuario Cliente

Los clientes deben **registrarse** en:
```
URL: http://localhost:5086/Account/Register
```

**Datos de ejemplo para registro:**
```
Nombre Completo: [Tu nombre]
Email: cliente@ejemplo.com
Password: Cliente123!
```

**Permisos del Cliente:**
- Acceso al portal de clientes
- Ver historial de compras
- Ver detalles de órdenes
- Sin permisos administrativos

### API REST - Autenticación JWT

**Endpoint de Login:**
```http
POST http://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "admin@pruebadedesempeno.com",
  "password": "Admin123!"
}
```

**Respuesta - Token JWT:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "admin@pruebadedesempeno.com",
    "fullName": "Administrador del Sistema",
    "roles": ["Administrador"],
    "expiration": "2025-12-10T12:00:00Z"
  }
}
```

**Usar el Token:**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Email (Mailtrap - Testing)

```
SMTP Server: sandbox.smtp.mailtrap.io
Puerto: 2525
Usuario: 45c32d48bac825
Password: ecb729df6118f586e189bfe5a4f5b293
```

**Ver emails enviados:**
https://mailtrap.io/inboxes

---

## 🔌 API REST

### Documentación Swagger

Acceder a: **http://localhost:5001**

Swagger UI se abre automáticamente con documentación interactiva de todos los endpoints.

### Endpoints Disponibles

#### 🔐 Autenticación (`/api/auth`)

```http
POST /api/auth/login       # Iniciar sesión (obtener JWT token)
POST /api/auth/register    # Registrar nuevo usuario
```

#### 📦 Productos (`/api/products`)

```http
GET    /api/products           # Listar todos los productos
GET    /api/products/{id}      # Obtener producto por ID
POST   /api/products           # Crear producto (Admin)
PUT    /api/products/{id}      # Actualizar producto (Admin)
DELETE /api/products/{id}      # Eliminar producto (Admin)
```

**Filtros disponibles:**
- `?search=laptop` - Buscar por nombre
- `?category=Electrónica` - Filtrar por categoría

#### 🤖 Chatbot (`/api/chatbot`)

```http
POST /api/chatbot/ask         # Enviar consulta al chatbot
```

**Ejemplo de consulta:**
```json
{
  "message": "¿Cuántos productos hay en stock?"
}
```

### Probar la API

#### Con Swagger (Recomendado)
1. Ir a http://localhost:5001
2. **Authorize**: Click en el botón "Authorize" 🔓
3. **Login**: POST `/api/auth/login` con credenciales de admin
4. **Token**: Copiar el token de la respuesta
5. **Pegar**: En el modal "Authorize", pegar `Bearer {token}`
6. **Probar**: Ahora puedes ejecutar todos los endpoints

#### Con Postman/Insomnia
```http
POST http://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "admin@pruebadedesempeno.com",
  "password": "Admin123!"
}
```

Luego usar el token en los headers:
```
Authorization: Bearer {tu-token-aqui}
```

---

## 🐳 Docker

### Servicios Containerizados

El `docker-compose.yml` orquesta 3 servicios:

1. **PostgreSQL** (Base de datos)
   - Puerto: 5433
   - Volumen persistente

2. **Web MVC** (Aplicación principal)
   - Puerto: 5000
   - Conectada a PostgreSQL

3. **API REST** (API independiente)
   - Puerto: 5001
   - Swagger en raíz
   - Conectada a PostgreSQL

### Comandos Docker

```bash
# Iniciar todos los servicios
docker-compose up -d

# Ver estado de contenedores
docker-compose ps

# Ver logs
docker-compose logs -f webapp    # Logs del MVC
docker-compose logs -f api       # Logs del API
docker-compose logs -f postgres  # Logs de la BD

# Detener servicios
docker-compose stop

# Eliminar servicios y volúmenes
docker-compose down -v
```

### Acceder a Servicios Docker

Una vez levantados los contenedores:

- **Web MVC**: http://localhost:5000
- **API + Swagger**: http://localhost:5001
- **PostgreSQL**: localhost:5433

---

## 🧪 Tests

### Ejecutar Tests

```bash
cd PruebaDeDesempeño.Tests
dotnet test
```

### Cobertura de Tests

- **Total**: 9 pruebas unitarias
- **Estado**: ✅ 9/9 passing (100%)
- **Frameworks**: xUnit + Moq + EF Core InMemory

**Tests implementados:**

#### ChatbotServiceTests (4 tests)
- ✅ Consulta de cantidad de productos
- ✅ Detección de productos con bajo stock
- ✅ Manejo de consultas inválidas
- ✅ Estadísticas de ventas totales

#### ProductsControllerTests (5 tests)
- ✅ Index muestra solo productos activos
- ✅ Crear producto correctamente
- ✅ Visualizar detalles de producto
- ✅ Búsqueda y filtrado
- ✅ Soft delete de productos

---

## 🎯 Funcionalidades Principales

### 1. Gestión de Productos
- CRUD completo
- Importación masiva desde Excel
- Exportación a Excel
- Categorización
- Control de stock
- Soft delete

### 2. Gestión de Clientes
- CRUD completo
- Importación desde Excel
- Exportación a Excel
- Tipos de documento (CC, CE, Pasaporte)
- Historial de compras

### 3. Gestión de Ventas
- Crear ventas con múltiples productos
- Cálculo automático de IVA (19%)
- Generación de facturas PDF
- Exportación de ventas a Excel
- Estados de venta

### 4. Dashboard Administrativo
- KPIs en tiempo real:
  - Total de productos
  - Total de clientes
  - Ventas del mes
  - Ingresos del mes
- Gráficos y estadísticas
- Productos con bajo stock

### 5. Portal de Clientes
- Acceso personalizado para clientes
- Historial de compras
- Detalles de cada orden
- Estadísticas personales

### 6. Chatbot con IA
- Consultas en lenguaje natural
- Respuestas sobre:
  - Inventario de productos
  - Ventas y estadísticas
  - Clientes frecuentes
  - Stock bajo

### 7. Sistema de Email
- Email de bienvenida automático
- Configuración SMTP con MailKit
- Integración con Mailtrap (testing)
- Plantillas HTML personalizadas

### 8. Autenticación y Autorización
- ASP.NET Identity
- Roles: Administrador y Cliente
- Protección de rutas
- JWT para API

---

## 📊 Arquitectura

### Patrón de Diseño

- **MVC** (Model-View-Controller) para la aplicación web
- **RESTful** para la API
- **Repository Pattern** con Entity Framework
- **Dependency Injection** nativo de ASP.NET Core

### Separación en Capas

```
Presentación (Views/Controllers)
        ↓
Lógica de Negocio (Services)
        ↓
Acceso a Datos (Repository/DbContext)
        ↓
Base de Datos (PostgreSQL)
```

---

## 🔒 Seguridad

- ✅ Contraseñas hasheadas con Identity
- ✅ Tokens JWT con expiración
- ✅ Protección CSRF (AntiForgery tokens)
- ✅ Autorización basada en roles
- ✅ Validación de datos (Data Annotations)
- ✅ HTTPS recomendado en producción
- ✅ SQL Injection protection (EF Core)

---

## 📝 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

---

## 👨‍💻 Desarrollo

### Tecnologías de Desarrollo
- Visual Studio 2022 / VS Code
- .NET 8.0 SDK
- PostgreSQL 16
- Docker Desktop (opcional) docker-compose up -d


### Extensiones Recomendadas (VS Code)
- C# Dev Kit
- Docker
- PostgreSQL Explorer

---

## 📞 Soporte

Para problemas o consultas:
1. Revisar la documentación de Swagger
2. Verificar logs de la aplicación
3. Consultar el código fuente

---

**Desarrollado con ❤️ usando ASP.NET Core 8.0**
