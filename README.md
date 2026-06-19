# Orbi

Aplicacion ASP.NET Core MVC para delivery multi-servicio: tiendas, productos, pedidos, pagos, repartidores y resenas.

## Stack

| Componente | Uso |
| --- | --- |
| ASP.NET Core MVC | Web app Razor/MVC |
| Entity Framework Core | Acceso a datos y migraciones |
| PostgreSQL | Base de datos |
| ASP.NET Identity | Usuarios, roles y sesiones |
| Bootstrap | UI base |

## Inicio

```bash
docker compose up -d
dotnet run --project src/Orbi.Web
```

URL local: `http://localhost:5130`.

La app aplica migraciones pendientes y ejecuta el seed al iniciar.

## Roles

Los roles vienen de `docs/ROLS.MD` y se aplican como roles de ASP.NET Identity:

| Rol | Acceso principal |
| --- | --- |
| `Admin` | CRUD total sobre entidades de negocio. |
| `StoreOwner` | Su tienda, productos, pedidos, pagos y resenas de su tienda. |
| `DeliveryDriver` | Su perfil y pedidos asignados. |
| `Customer` | Su perfil, direcciones, pedidos, pagos y resenas. |

La navegacion no oculta secciones por rol. Si un usuario entra a una seccion no permitida, recibe `403` y la pantalla indica `Acceso prohibido`.

## Seguridad Aplicada

- Filtro global de acceso por rol para controladores MVC de negocio.
- Filtros de propiedad por `UserId` en servicios sensibles.
- Validacion server-side para impedir acceso por IDs adivinados.
- Excepciones de acceso convertidas a `403`.
- Login con bloqueo por intentos fallidos.
- Registro publico limitado a `Customer`, `DeliveryDriver` y `StoreOwner`.
- Cookies `HttpOnly`, `SameSite=Strict` y `Secure` en produccion.
- Headers de seguridad: CSP, `X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy` y `Permissions-Policy`.
- HSTS fuera de desarrollo y redireccion HTTPS.
- Cache JS del grafico del home para no re-renderizar si los conteos no cambiaron.

## Optimizaciones Aplicadas

- Paginacion server-side con `Skip`/`Take`.
- Proyecciones a ViewModels en servicios.
- `AsNoTracking` en lecturas.
- Indices compuestos para filtros frecuentes.
- Indices trigram GIN para busquedas de texto.
- Dropdowns grandes limitados a 200 registros.
- Cache en memoria para catalogos pequenos.

## Pendiente

- Tests automatizados de acceso por rol y propiedad.
- Flujos dedicados para cancelar pedidos y cambios operativos de estado.
- Auditoria de acciones sensibles.
- Confirmacion de email y recuperacion de contrasena.
- Politicas de seguridad por ambiente para dominios reales de produccion.

## Docs

- `docs/API.md`: rutas MVC y permisos.
- `docs/ROLS.MD`: matriz de roles.
- `SECURITY.md`: politica y controles de seguridad.
- `docs/SEED.md`: datos iniciales.
