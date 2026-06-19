using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Orbi.Web.Security;

public class RoleAccessFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> PublicControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Account",
        "Home"
    };

    private readonly CurrentUserAccess _access;

    public RoleAccessFilter(CurrentUserAccess access)
    {
        _access = access;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
        var action = context.RouteData.Values["action"]?.ToString() ?? "";

        if (PublicControllers.Contains(controller))
        {
            await next();
            return;
        }

        if (!_access.IsAuthenticated)
        {
            context.Result = new ChallengeResult();
            return;
        }

        if (!CanAccess(controller, action, context.HttpContext.Request.Method))
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }

    private bool CanAccess(string controller, string action, string method)
    {
        if (_access.IsAdmin) return true;

        var isRead = HttpMethods.IsGet(method) && action is "Index" or "Details";
        var isCreate = action == "Create";
        var isEdit = action == "Edit";
        var isDelete = action == "Delete";

        return controller switch
        {
            "StoreCategories" => isRead,
            "OrderStatuses" => isRead,
            "PaymentMethods" => (_access.IsCustomer || _access.IsStoreOwner) && isRead,

            "Stores" => isRead ||
                (_access.IsStoreOwner && !isDelete && (isCreate || isEdit)),

            "Products" => isRead ||
                (_access.IsStoreOwner && (isCreate || isEdit || isDelete)),

            "Customers" => _access.IsCustomer && !isCreate && !isDelete && (isRead || isEdit),

            "Addresses" => _access.IsCustomer && (isRead || isCreate || isEdit || isDelete),

            "Orders" => (_access.IsStoreOwner || _access.IsDeliveryDriver) && (isRead || isEdit) ||
                _access.IsCustomer && (isRead || isCreate),

            "DeliveryDrivers" => _access.IsDeliveryDriver && !isCreate && !isDelete && (isRead || isEdit) ||
                _access.IsStoreOwner && isRead,

            "Payments" => _access.IsStoreOwner && isRead ||
                _access.IsCustomer && (isRead || isCreate),

            "Reviews" => _access.IsStoreOwner && isRead ||
                _access.IsCustomer && (isRead || isCreate || isEdit || isDelete),

            _ => false
        };
    }
}
