using Microsoft.AspNetCore.Mvc.Filters;
using EmpleadosAPI.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace EmpleadosAPI.Filters
{
	public class AuditoriaFilter : IActionFilter
	{
		private readonly ApplicationDbContext _context;

		public AuditoriaFilter(ApplicationDbContext context)
		{
			_context = context;
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var httpContext = context.HttpContext;
			var user = httpContext.User.Identity?.Name ?? "Anónimo";
			var rol = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "Sin rol";
			var ruta = httpContext.Request.Path.ToString();
			var metodo = httpContext.Request.Method;

			_context.Database.ExecuteSqlRaw(@"
                INSERT INTO LogsAcceso (Usuario, Rol, Metodo, Ruta)
                VALUES ({0}, {1}, {2}, {3})
            ", user, rol, metodo, ruta);
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
