using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using WebThoiTrang.Extensions;
namespace WebThoiTrang.Filter
{
    public class UserActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            var user = context.HttpContext.Session.Get<dynamic>("User");
            if (user != null)
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.ViewData["Username"] = user.Username;
                }
            }
        }
    }
}
