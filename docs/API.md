# API y rutas MVC

Orbi usa controladores MVC con el patron:

```text
/{Controller}/{Action}/{id?}
```

Las listas aceptan `searchField`, `searchTerm` y `page`.

## Publico

| Metodo | Ruta | Uso |
| --- | --- | --- |
| GET | `/` | Home |
| GET | `/Home/Index` | Home |
| GET | `/Home/RecordCounts` | Conteos del grafico home |
| GET | `/Home/DbStatus` | Estado de base de datos |
| GET | `/Account/Login` | Login |
| POST | `/Account/Login` | Iniciar sesion |
| GET | `/Account/Register` | Registro |
| POST | `/Account/Register` | Crear usuario `Customer`, `DeliveryDriver` o `StoreOwner` |

`POST /Account/Logout` requiere usuario autenticado.

## Acceso por rol

| Modulo | Admin | StoreOwner | DeliveryDriver | Customer |
| --- | --- | --- | --- | --- |
| StoreCategories | CRUD | Read | Read | Read |
| Stores | CRUD | Own CRUD | Read | Read |
| Products | CRUD | Own store CRUD | Read | Read |
| Customers | CRUD | Prohibido | Prohibido | Own read/edit |
| Addresses | CRUD | Prohibido | Via pedidos asignados | Own CRUD |
| Orders | CRUD | Own store read/edit | Assigned read/edit status | Own read/create |
| OrderStatuses | CRUD | Read | Read | Read |
| DeliveryDrivers | CRUD | Read | Own read/edit | Prohibido |
| PaymentMethods | CRUD | Read | Prohibido | Read |
| Payments | CRUD | Own store read | Prohibido | Own read/create |
| Reviews | CRUD | Own store read | Prohibido | Own CRUD |

Las secciones permanecen visibles en la navegacion. El acceso no permitido responde `403` y muestra `Acceso prohibido`.

## Rutas CRUD

| Accion | Metodo | Ruta |
| --- | --- | --- |
| Index | GET | `/{Controller}` |
| Details | GET | `/{Controller}/Details/{id}` |
| Create | GET/POST | `/{Controller}/Create` |
| Edit | GET/POST | `/{Controller}/Edit/{id}` |
| Delete | POST | `/{Controller}/Delete/{id}` |

`Delete` es borrado logico con `IsActive = false`.

## Reglas de propiedad

| Rol | Regla |
| --- | --- |
| Customer | `Customers.UserId == User.Id` |
| StoreOwner | `Stores.UserId == User.Id` |
| DeliveryDriver | `DeliveryDrivers.UserId == User.Id` |

Las consultas sensibles se filtran por propietario en servicios. Las escrituras vuelven a validar propiedad antes de guardar.

## Seguridad de entrada

- Todos los POST usan antiforgery token.
- El total de pedidos se calcula en servidor.
- El registro publico no permite crear `Admin`.
- Los formularios solo cargan dropdowns dentro del alcance del usuario.
