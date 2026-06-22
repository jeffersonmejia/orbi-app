# Guía de comandos para Orbi

Esta guía resume los comandos principales para replicar, compilar, ejecutar, migrar y preparar el proyecto Orbi. El sistema está hecho con ASP.NET Core MVC, Entity Framework Core, ASP.NET Identity y PostgreSQL usando Docker.

## 1. Requisitos

Antes de trabajar con el proyecto se debe tener instalado:

| Herramienta | Uso |
| --- | --- |
| .NET SDK 10 | Compilar, ejecutar, migrar y publicar la aplicación |
| Docker y Docker Compose | Levantar PostgreSQL y cargar datos iniciales |
| Git | Clonar el proyecto y controlar versiones |
| dotnet-ef | Ejecutar comandos de migraciones de Entity Framework Core |
| dotnet-aspnet-codegenerator | Generar controladores y vistas MVC por scaffolding |

Instalar herramientas globales de .NET:

```bash
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator
```

Si ya están instaladas, actualizarlas:

```bash
dotnet tool update --global dotnet-ef
dotnet tool update --global dotnet-aspnet-codegenerator
```

Verificar versiones:

```bash
dotnet --version
dotnet ef --version
docker --version
docker compose version
```

## 2. Clonar y entrar al proyecto

```bash
git clone git@github.com:jeffersonmejia/orbi-app.git
cd orbi-app
```

Restaurar paquetes NuGet:

```bash
dotnet restore
```

## 3. Levantar la base de datos con Docker

El archivo `docker-compose.yml` crea dos servicios:

| Servicio | Función |
| --- | --- |
| `postgres` | Levanta PostgreSQL 16 con la base `OrbiDb` |
| `seed` | Espera el esquema generado por EF Core y ejecuta scripts SQL de carga |

Levantar PostgreSQL:

```bash
docker compose up -d postgres
```

Verificar que el contenedor esté activo:

```bash
docker ps
```

Levantar todos los servicios, incluyendo el seed:

```bash
docker compose up -d
```

Ver logs del contenedor de base de datos:

```bash
docker logs orbi-postgres
```

Ver logs del contenedor de seed:

```bash
docker logs orbi-seed
```

Consultar conteo de tablas y registros con el script del proyecto:

```bash
./query.sh
```

## 4. Compilar el proyecto

Compilar toda la solución:

```bash
dotnet build
```

Compilar solo el proyecto web:

```bash
dotnet build src/Orbi.Web/Orbi.Web.csproj
```

Compilar en modo Release:

```bash
dotnet build src/Orbi.Web/Orbi.Web.csproj -c Release
```

## 5. Ejecutar el proyecto

Ejecutar la aplicación web:

```bash
dotnet run --project src/Orbi.Web
```

Ejecutar con recarga automática en desarrollo:

```bash
dotnet watch --project src/Orbi.Web run
```

También se puede usar el script incluido:

```bash
./run.sh
```

Abrir en el navegador:

```text
http://localhost:5130
```

Al iniciar, `Program.cs` aplica migraciones pendientes con `context.Database.MigrateAsync()` y ejecuta `DbSeeder.SeedAsync()` para preparar datos base como roles y usuarios.

## 6. Flujo Code First en este proyecto

Code First significa que primero se crean o modifican clases C# en `Models`, luego Entity Framework Core genera una migración y finalmente esa migración actualiza la base de datos.

Orden recomendado:

1. Crear o editar el modelo en `src/Orbi.Web/Models`.
2. Registrar el modelo como `DbSet` en `src/Orbi.Web/Data/AppDbContext.cs`.
3. Configurar relaciones, índices, filtros y reglas en `OnModelCreating`.
4. Crear el `ViewModel` si la vista no debe usar directamente la entidad.
5. Crear o ajustar el `Service` para consultas, validaciones y reglas de negocio.
6. Crear o ajustar el `Controller`.
7. Crear o ajustar las `Views`.
8. Crear la migración.
9. Aplicar la migración a PostgreSQL.
10. Ejecutar y probar el módulo.

## 7. Crear una migración

Comando base:

```bash
dotnet ef migrations add NombreDeLaMigracion --project src/Orbi.Web --startup-project src/Orbi.Web
```

Ejemplo:

```bash
dotnet ef migrations add AddCouponsModule --project src/Orbi.Web --startup-project src/Orbi.Web
```

Buenas prácticas para nombrar migraciones:

| Caso | Ejemplo |
| --- | --- |
| Crear una tabla | `AddCouponsModule` |
| Agregar campos | `AddStoreOpeningHours` |
| Crear índices | `AddOrderSearchIndexes` |
| Modificar relaciones | `UpdatePaymentOrderRelation` |
| Optimizar consultas | `OptimizeProductSearchQueries` |

## 8. Aplicar migraciones

Aplicar migraciones pendientes a PostgreSQL:

```bash
dotnet ef database update --project src/Orbi.Web --startup-project src/Orbi.Web
```

Ver migraciones existentes:

```bash
dotnet ef migrations list --project src/Orbi.Web --startup-project src/Orbi.Web
```

Generar script SQL de migraciones:

```bash
dotnet ef migrations script --project src/Orbi.Web --startup-project src/Orbi.Web -o migrations.sql
```

Eliminar la última migración si todavía no fue aplicada:

```bash
dotnet ef migrations remove --project src/Orbi.Web --startup-project src/Orbi.Web
```

Importante: si una migración ya fue aplicada en la base de datos compartida, no se debe eliminar sin revisar el impacto. En examen, explicar que primero se valida si la migración fue aplicada.

## 9. Generar modelos

En Code First, un modelo se crea como una clase C# dentro de `Models`.

Ejemplo de modelo:

```csharp
namespace Orbi.Web.Models;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DateTime ExpirationDate { get; set; }
}
```

Registrar el modelo en `AppDbContext`:

```csharp
public DbSet<Coupon> Coupons => Set<Coupon>();
```

Configurar reglas en `OnModelCreating`:

```csharp
modelBuilder.Entity<Coupon>(entity =>
{
    entity.HasIndex(c => c.Code).IsUnique();
    entity.HasQueryFilter(c => c.IsActive);
});
```

Crear la migración:

```bash
dotnet ef migrations add AddCouponsModule --project src/Orbi.Web --startup-project src/Orbi.Web
```

Aplicarla:

```bash
dotnet ef database update --project src/Orbi.Web --startup-project src/Orbi.Web
```

## 10. Generar controladores y vistas MVC

El proyecto incluye `Microsoft.VisualStudio.Web.CodeGeneration.Design`, por eso se puede usar scaffolding.

Comando general:

```bash
dotnet aspnet-codegenerator controller \
  -name CouponsController \
  -m Coupon \
  -dc AppDbContext \
  --relativeFolderPath Controllers \
  --useDefaultLayout \
  --referenceScriptLibraries \
  --project src/Orbi.Web
```

Después de generar, revisar:

| Archivo o carpeta | Qué revisar |
| --- | --- |
| `Controllers/CouponsController.cs` | Acciones `Index`, `Details`, `Create`, `Edit` y `Delete` |
| `Views/Coupons` | Formularios y tablas generadas |
| `AppDbContext` | `DbSet` y configuración del modelo |
| `Services` | Mover lógica sensible desde el controlador hacia servicios |
| `Security` | Validar acceso por rol si el módulo debe estar restringido |

En este proyecto, lo ideal es no dejar toda la lógica en el controlador. La lógica de consultas, filtros por propietario, validaciones y paginación debe vivir en `Services`.

## 11. Crear un módulo completo en Orbi

Orden recomendado para agregar un módulo nuevo:

1. Crear el modelo en `Models`.
2. Agregar `DbSet` en `AppDbContext`.
3. Configurar relaciones, índices y `HasQueryFilter`.
4. Crear migración con `dotnet ef migrations add`.
5. Aplicar migración con `dotnet ef database update`.
6. Crear `ViewModel`.
7. Crear `Service`.
8. Registrar el servicio en `Program.cs` con `builder.Services.AddScoped`.
9. Crear `Controller`.
10. Crear vistas Razor.
11. Agregar permisos en `RoleAccessFilter` si aplica.
12. Probar con cada rol: `Admin`, `StoreOwner`, `DeliveryDriver` y `Customer`.

## 12. Comandos útiles de Entity Framework Core

| Acción | Comando |
| --- | --- |
| Ver migraciones | `dotnet ef migrations list --project src/Orbi.Web --startup-project src/Orbi.Web` |
| Crear migración | `dotnet ef migrations add Nombre --project src/Orbi.Web --startup-project src/Orbi.Web` |
| Aplicar migraciones | `dotnet ef database update --project src/Orbi.Web --startup-project src/Orbi.Web` |
| Quitar última migración no aplicada | `dotnet ef migrations remove --project src/Orbi.Web --startup-project src/Orbi.Web` |
| Generar SQL | `dotnet ef migrations script --project src/Orbi.Web --startup-project src/Orbi.Web -o migrations.sql` |
| Ver DbContext detectado | `dotnet ef dbcontext list --project src/Orbi.Web --startup-project src/Orbi.Web` |

## 13. Comandos útiles de Docker

| Acción | Comando |
| --- | --- |
| Levantar contenedores | `docker compose up -d` |
| Detener contenedores | `docker compose down` |
| Ver contenedores activos | `docker ps` |
| Ver logs de PostgreSQL | `docker logs orbi-postgres` |
| Ver logs del seed | `docker logs orbi-seed` |
| Entrar a PostgreSQL | `docker exec -it orbi-postgres psql -U postgres -d OrbiDb` |
| Ejecutar conteo del proyecto | `./query.sh` |

Consulta rápida dentro de PostgreSQL:

```sql
\dt
SELECT COUNT(*) FROM "Customers";
SELECT COUNT(*) FROM "Orders";
```

## 14. Preparar para IIS

Para desplegar en IIS se debe publicar la aplicación en modo Release.

Publicar:

```bash
dotnet publish src/Orbi.Web/Orbi.Web.csproj -c Release -o ./publish
```

La carpeta `publish` contendrá los archivos listos para copiar al servidor IIS. ASP.NET Core genera un `web.config` para que IIS pueda redirigir las solicitudes hacia la aplicación.

Pasos generales en Windows Server con IIS:

1. Instalar el .NET Hosting Bundle compatible con la versión del proyecto.
2. Crear un sitio en IIS apuntando a la carpeta publicada.
3. Configurar el Application Pool en `No Managed Code`.
4. Configurar la cadena de conexión de producción mediante variables de entorno, User Secrets o configuración segura del servidor.
5. Asegurar que PostgreSQL sea accesible desde el servidor.
6. Dar permisos de lectura y ejecución al usuario del Application Pool sobre la carpeta publicada.
7. Activar HTTPS con certificado válido.
8. Reiniciar el sitio y revisar logs si ocurre un error.

Comandos útiles para publicar:

```bash
dotnet restore
dotnet build src/Orbi.Web/Orbi.Web.csproj -c Release
dotnet publish src/Orbi.Web/Orbi.Web.csproj -c Release -o ./publish
```

En producción no se recomienda dejar credenciales reales en `appsettings.json`. Lo correcto es usar variables de entorno o secretos del servidor.

Ejemplo de variable de entorno para la conexión:

```bash
ConnectionStrings__DefaultConnection="Host=servidor;Port=5432;Database=OrbiDb;Username=usuario;Password=clave"
```

## 15. Qué explicar en el examen

Si piden explicar el proyecto, una respuesta técnica breve puede seguir este orden:

1. Orbi es una aplicación ASP.NET Core MVC para delivery.
2. Usa PostgreSQL como base de datos y EF Core con enfoque Code First.
3. Los modelos representan clientes, tiendas, productos, pedidos, pagos, reseñas y repartidores.
4. `AppDbContext` configura tablas, relaciones, índices y filtros globales.
5. `Program.cs` registra servicios, Identity, filtros, cookies, seguridad, rutas, migraciones y seed.
6. Los controladores reciben solicitudes y los servicios aplican lógica de negocio.
7. Las vistas Razor muestran formularios, tablas y detalles usando ViewModels.
8. Docker levanta PostgreSQL y ejecuta scripts de carga inicial.
9. Identity maneja autenticación y roles.
10. El acceso se controla por roles y propiedad de datos.

## 16. Errores comunes y solución

| Error | Posible causa | Solución |
| --- | --- | --- |
| No conecta a PostgreSQL | Docker no está levantado | Ejecutar `docker compose up -d postgres` |
| `dotnet ef` no existe | Herramienta no instalada | Ejecutar `dotnet tool install --global dotnet-ef` |
| No encuentra el `DbContext` | Falta indicar proyecto | Usar `--project src/Orbi.Web --startup-project src/Orbi.Web` |
| La tabla no aparece | Migración no aplicada | Ejecutar `dotnet ef database update` |
| Error de permisos en una pantalla | Rol sin autorización | Revisar `RoleAccessFilter` y reglas del servicio |
| Datos eliminados siguen en BD | El proyecto usa eliminación lógica | Revisar el campo `IsActive` |
| Seed no corre | El esquema EF todavía no existe | Ejecutar primero la app o aplicar migraciones |

## 17. Vocabulario clave

| Término | Significado |
| --- | --- |
| MVC | Patrón Modelo-Vista-Controlador. Separa datos, interfaz y flujo de solicitudes. |
| Model | Clase que representa una tabla o entidad del negocio. |
| View | Pantalla Razor que muestra información al usuario. |
| Controller | Clase que recibe solicitudes HTTP y devuelve vistas o respuestas. |
| ViewModel | Objeto usado para enviar a la vista solo los datos necesarios. |
| Service | Clase donde se coloca lógica de negocio, consultas y validaciones. |
| DbContext | Clase central de EF Core que representa la conexión entre C# y la base de datos. |
| DbSet | Propiedad del `DbContext` que representa una tabla consultable. |
| Migration | Archivo generado por EF Core para transformar cambios de modelos en cambios de base de datos. |
| Code First | Enfoque donde primero se crean clases C# y luego se genera la base de datos. |
| Identity | Sistema de ASP.NET Core para usuarios, roles, contraseñas y autenticación. |
| Role | Perfil de acceso del usuario, como `Admin`, `StoreOwner`, `DeliveryDriver` o `Customer`. |
| Authorization | Validación de qué puede hacer un usuario autenticado. |
| Authentication | Proceso de iniciar sesión y comprobar identidad. |
| Soft Delete | Eliminación lógica usando `IsActive = false` en vez de borrar físicamente. |
| IQueryable | Consulta que EF Core traduce a SQL para filtrar y paginar en la base de datos. |
| Eager Loading | Carga explícita de relaciones con `Include` para evitar consultas repetidas. |
| Seed | Carga inicial de datos para catálogos, usuarios o registros de prueba. |
| Docker Compose | Herramienta para levantar servicios como PostgreSQL usando un archivo YAML. |
| IIS | Servidor web de Microsoft usado para publicar aplicaciones ASP.NET Core en Windows Server. |
