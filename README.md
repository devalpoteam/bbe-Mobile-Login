# **AuthService** 🔐

### **Un microservicio para autenticación con JWT y Google OAuth**

![AuthService](https://img.shields.io/badge/AuthService-v1.0-blue?style=flat-square&logo=.net)
![.NET](https://img.shields.io/badge/.NET-6.0-purple?style=flat-square&logo=.net)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

## **📚 Descripción**

**AuthService** es un microservicio construido con **ASP.NET Core 6** que proporciona un sistema de autenticación basado en **JWT (JSON Web Tokens)** y **Google OAuth**.  
Este servicio permite:
- Registrar usuarios con email y contraseña.
- Iniciar sesión mediante credenciales locales.
- Iniciar sesión mediante Google OAuth.
- Generar y validar tokens JWT.

## **🚀 Características**

✅ Registro de usuarios mediante email y contraseña.  
✅ Autenticación local con JWT.  
✅ Inicio de sesión con Google OAuth.  
✅ Protección de endpoints con `[Authorize]`.  
✅ Escalable para sistemas distribuidos.

## **📂 Estructura del proyecto**

AuthService/
│
├── Controllers/           # Controladores (manejan las solicitudes HTTP)
│   ├── AuthController.cs  # Login local y registro
│   └── GoogleAuthController.cs  # Login con Google
│
├── Domain/                # Entidades y modelos del dominio
│   ├── AppUser.cs         # Clase de usuario
│   ├── UserToken.cs       # Modelo para tokens JWT
│   └── Models/            # Modelos de entrada (RegisterModel, LoginModel)
│
├── Infrastructure/        # Configuración de base de datos y repositorios
│   ├── Context/           # DbContext y migraciones
│   └── Repositories/      # Lógica de usuarios y tokens
│
├── Program.cs             # Configuración de servicios y middlewares
├── appsettings.json       # Configuración del servicio (Google, JWT, DB)
└── README.md              # Documentación del proyecto

## **⚙️ Configuración**

### **Requisitos**
- .NET 6 SDK
- SQL Server para almacenar usuarios y tokens.
- Una cuenta de Google Developer para configurar OAuth.

### **Instalación**
# 1. Clona este repositorio
git clone https://github.com/tu-usuario/AuthService.git
cd AuthService

# 2. Restaura las dependencias
dotnet restore

# 3. Configura la base de datos y claves en `appsettings.json`
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

# 4. Aplica las migraciones y crea la base de datos
dotnet ef database update

## **📤 Endpoints principales**

### **1. Registro de usuario**
POST /api/auth/register

# Body (JSON):
{
  "email": "usuario@ejemplo.com",
  "password": "Password123!",
  "fullName": "Usuario Ejemplo"
}

# Respuesta:
{
  "message": "Usuario creado exitosamente."
}

### **2. Login local**
POST /api/auth/login

# Body (JSON):
{
  "email": "usuario@ejemplo.com",
  "password": "Password123!"
}

# Respuesta:
{
  "message": "Autenticación exitosa.",
  "token": "<jwt_token>",
  "expires": "2024-12-20T12:00:00Z"
}

### **3. Login con Google**
GET /api/googleauth/signin

# Redirige al usuario a Google para autenticación.
# Tras autenticarse, el usuario será redirigido al callback configurado.

### **4. Validar JWT en otros microservicios**
# En los microservicios cliente, incluye el token en el header:
Authorization: Bearer <jwt_token>

## **🔒 Seguridad**

### Firma de tokens JWT
- Los tokens son firmados con claves seguras.
- Los tokens se validan usando los parámetros configurados.

### Protección de endpoints
- Usa [Authorize] para proteger rutas.
- Configura roles y claims para control de acceso granular.

## **📜 Licencia**

Este proyecto está bajo la licencia MIT.

## **🌟 Contribuciones**

¡Siempre son bienvenidas! 😊  
1. Haz un fork del proyecto.  
2. Crea una rama para tu funcionalidad (`git checkout -b feature/nueva-funcionalidad`).  
3. Realiza un pull request.

## **📬 Contacto**

Autor: Daniel Toledo  
GitHub: @DanielToledo  
Email: Daniel.Toledo@devalpo.cl
