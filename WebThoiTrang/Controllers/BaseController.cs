using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebThoiTrang.Extensions;
namespace WebThoiTrang.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var user = HttpContext.Session.Get<dynamic>("User");
            if (user != null)
            {
                ViewData["Username"] = user.Username;
            }
        }
    }
}
