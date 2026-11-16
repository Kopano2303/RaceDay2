
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Controllers
{
    public class BaseController : Controller
    {
        protected virtual string RequiredRole => string.Empty;

        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            string? token = HttpContext.Session.GetString("AuthToken");
            string? role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = RedirectToAction(
                    "Login", "Account");
                return;
            }

            if (!string.IsNullOrEmpty(RequiredRole) &&
                role != RequiredRole)
            {
                if (role == "Organiser")
                {
                    context.Result = RedirectToAction(
                        "Index", "Organiser");
                }
                else if (role == "Participant")
                {
                    context.Result = RedirectToAction(
                        "Index", "Participant");
                }
                else
                {
                    context.Result = RedirectToAction(
                        "Login", "Account");
                }

                return;
            }

            ViewBag.UserName =
                HttpContext.Session.GetString("UserName");

            ViewBag.UserRole = role;

            base.OnActionExecuting(context);
        }
    }
}
