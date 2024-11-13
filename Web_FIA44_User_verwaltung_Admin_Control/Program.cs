using Web_FIA44_User_verwaltung_Admin_Control.BL;
using Web_FIA44_User_verwaltung_Admin_Control.DAL;

namespace Web_FIA44_User_verwaltung_Admin_Control
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();

			var connectionString = builder.Configuration.GetConnectionString("SqlServer");

			builder.Services.AddScoped<IAccessable>(provider => new SqlDal(connectionString));

			builder.Services.AddScoped<IUserService, BusiLay>();
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.Name = "Web_FIA44_Session";
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			}
			);
			var app = builder.Build();

			app.MapControllerRoute(name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.UseHttpsRedirection();

			app.UseAuthorization();
			app.UseStaticFiles();
			app.UseSession();
			app.UseRouting();
			app.Run();
		}
	}
}
