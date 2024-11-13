using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web_FIA44_User_verwaltung_Admin_Control.Controllers
{
	public class SessionCheckFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.HttpContext.Session.GetString("LoggedIn") == null)
			{
				context.Result = new RedirectResult("/Home/Index");
			}
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
		}

	}
}
