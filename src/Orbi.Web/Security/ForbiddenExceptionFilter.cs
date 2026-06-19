using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Orbi.Web.Security;

public class ForbiddenExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not UnauthorizedAccessException)
        {
            return;
        }

        context.ExceptionHandled = true;
        context.Result = new ForbidResult();
    }
}
