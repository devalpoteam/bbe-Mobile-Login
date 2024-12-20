# **AuthService** 🔐

### **Un microservicio para autenticación con JWT y Google OAuth**

![AuthService](https://img.shields.io/badge/AuthService-v1.0-blue?style=flat-square&logo=.net)  
![.NET](https://img.shields.io/badge/.NET-9.0-purple?style=flat-square&logo=.net)  
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

---

## 📂 **Estructura del proyecto**

```plaintext
AuthService/
│
├── 01-Presentation/                       # Capa de presentación (API)
│   ├── Controllers/                       # Controladores principales
│   │   └── AuthController.cs              # Login local y registro
│   ├── ActionFilters/                     # Filtros de acción
│   ├── Configuration/                     # Configuración inicial
│   ├── Extensions/                        # Métodos de extensión
│   ├── Middlewares/                       # Middlewares personalizados
│   ├── Program.cs                         # Configuración de servicios y middlewares
│   └── appsettings.json                   # Configuración general (Google, JWT, DB)
│
├── 02-Application/                        # Lógica de la aplicación
│   ├── Behavior/                          # Validaciones y comportamientos
│   ├── Configuration/                     # Configuración de la aplicación
│   ├── Contracts/                         # Interfaces y contratos
│   ├── Handlers/                          # Implementaciones de contratos
│   └── Services/                          # Servicios de lógica de negocio
│
├── 03-Model/                              # Modelos del dominio
│   ├── Entities/                          # Entidades principales (AppUser, etc.)
│   ├── Exceptions/                        # Excepciones personalizadas
│   ├── Models/                            # Modelos de entrada/salida (DTOs)
│   └── ValueObject/                       # Objetos de valor
│
├── 04-Infrastructure/                     # Configuración de infraestructura y repositorios
│   ├── Configurations/                    # Configuración de entidades para EF Core
│   ├── Context/                           # DbContext y configuración de base de datos
│   └── Migrations/                        # Migraciones para la base de datos
└── README.md                              # Documentación del proyecto
```

---

## ⚙️ **Configuración**

### **Requisitos**
```plaintext
- .NET 9 SDK
- SQL Server para almacenar usuarios y tokens.
- Una cuenta de Google Developer para configurar OAuth.
```

### **Instalación**
```bash
# 1. Restaura las dependencias
dotnet restore

# 2. Configura la base de datos y claves en `appsettings.json`
# Ejemplo:
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AuthServiceDb;Trusted_Connection=True;"
    },
    "Authentication": {
        "Jwt": {
            "Issuer": "https://authservice.local",
            "Audience": "https://api.microservicio.local"
        },
        "Google": {
            "ClientId": "tu-client-id",
            "ClientSecret": "tu-client-secret"
        }
    }
}

# 3. Aplica las migraciones y crea la base de datos
dotnet ef database update
```

---

## 📤 **Endpoints principales**

### **1. Registro de usuario**
```http
POST /api/auth/register
```
**Body (JSON):**
```json
{
  "email": "usuario@ejemplo.com",
  "password": "Password123!",
  "fullName": "Usuario Ejemplo"
}
```
**Respuesta:**
```json
{
  "message": "Usuario creado exitosamente."
}
```

---

### **2. Login local**
```http
POST /api/auth/login
```
**Body (JSON):**
```json
{
  "email": "usuario@ejemplo.com",
  "password": "Password123!"
}
```
**Respuesta:**
```json
{
  "message": "Autenticación exitosa.",
  "token": "<jwt_token>",
  "expires": "2024-12-20T12:00:00Z"
}
```

---

### **3. Login con Google**
```http
GET /api/googleauth/signin
```
```plaintext
- Redirige al usuario a Google para autenticación.
- Tras autenticarse, el usuario será redirigido al callback configurado.
```

---

### **4. Validar JWT en otros microservicios**
```plaintext
- En los microservicios cliente, incluye el token en el header:
Authorization: Bearer <jwt_token>
```

---

## 🔒 **Seguridad**

### **Firma de tokens JWT**
```plaintext
- Los tokens son firmados con claves seguras.
- Los tokens se validan usando los parámetros configurados.
```

### **Protección de endpoints**
```plaintext
- Usa [Authorize] para proteger rutas.
- Configura roles y claims para control de acceso granular.
```

---

## 📜 **Licencia**

```plaintext
Este proyecto está bajo la licencia MIT.
```

---

## 🌟 **Contribuciones**

```plaintext
¡Siempre son bienvenidas! 😊
1. Haz un fork del proyecto.
2. Crea una rama para tu funcionalidad (`git checkout -b feature/nueva-funcionalidad`).
3. Realiza un pull request.
```

---

## 📬 **Contacto**

```plaintext
Autor: Daniel Toledo
GitHub: @danieloledodevalpo
Email: Daniel.Toledo@devalpo.cl
```
