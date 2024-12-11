using Library_MVC.Data;
using Library_MVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Library_MVC;
using Microsoft.AspNetCore.Identity.UI.Services;
using Library_MVC.Controllers;
using Library_MVC.Data.Static;

var builder = WebApplication.CreateBuilder(args);

// Получаем строки подключения из переменных окружения
var is_Docker_Env = Environment.GetEnvironmentVariable("IS_DOCKER");
string connectionString = "SongsDb";
string accountConnectionStringName = "AccountDB";

if (!string.IsNullOrEmpty(is_Docker_Env))
{
	// Мы внутри Docker
	// Отключаем использование HTTPS
	builder.WebHost.ConfigureKestrel(serverOptions =>
	{
		serverOptions.ListenAnyIP(5000);  // HTTP порт
	});
	connectionString += "_Docker";
	accountConnectionStringName += "_Docker";
}


builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MusicLibDBContext>(options =>
{
	options.UseNpgsql(
		builder.Configuration.GetConnectionString(connectionString)
	);
});


builder.Services.AddDbContext<AuthDbContext>(options =>
{
	options.UseNpgsql(
		builder.Configuration.GetConnectionString(accountConnectionStringName)
	);
});

builder.Services.AddSingleton<IEmailSender, AuthEmailSender>();

builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
{
	options.SignIn.RequireConfirmedAccount = true;
	options.User.RequireUniqueEmail = true;
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Lockout.MaxFailedAccessAttempts = 6;
	options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();


builder.Services.AddAuthorization(options =>
{
	options.AddPolicy(Policies.CreateMod, policy => policy.RequireRole(AccountController.AdminRole));
	options.AddPolicy(Policies.EditMod, policy => policy.RequireRole(AccountController.AdminRole));
	options.AddPolicy(Policies.DeleteMod, policy => policy.RequireRole(AccountController.AdminRole));

	options.AddPolicy(Policies.CreateMember, policy => policy.RequireRole(AccountController.AdminRole));
	options.AddPolicy(Policies.EditMember, policy => policy.RequireRole(AccountController.AdminRole));
	options.AddPolicy(Policies.DeleteMember, policy => policy.RequireRole(AccountController.AdminRole));
	options.AddPolicy(Policies.BlockMember, policy => policy.RequireRole(AccountController.AdminRole, AccountController.ModeratorRole));

	options.AddPolicy(Policies.CreateBook, policy => policy.RequireRole(AccountController.AdminRole, AccountController.ModeratorRole));
	options.AddPolicy(Policies.EditBook, policy => policy.RequireRole(AccountController.AdminRole, AccountController.ModeratorRole));
	options.AddPolicy(Policies.DeleteBook, policy => policy.RequireRole(AccountController.AdminRole));
	options.AddPolicy(Policies.ReadBook, policy => policy.RequireRole(AccountController.AdminRole, AccountController.ModeratorRole, AccountController.MemberRole));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
);

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	// Применяем миграции для SongsDb
	var musicLibContext = services.GetRequiredService<MusicLibDBContext>();
	musicLibContext.Database.Migrate();

	// Применяем миграции для AccountDB
	var accountContext = services.GetRequiredService<AuthDbContext>();
	accountContext.Database.Migrate();

	// Создаем роли, если они не существуют
	var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
	var roles = new[] { AccountController.AdminRole, AccountController.ModeratorRole, AccountController.MemberRole };

	foreach (var role in roles)
	{
		if (!await roleManager.RoleExistsAsync(role))
		{
			await roleManager.CreateAsync(new IdentityRole(role));
		}
	}

	// Тестируем роли: создаем пользователя администратора
	var userManager = services.GetRequiredService<UserManager<UserModel>>();
	string adminUserName = "admin";
	string adminEmail = "admin@admin.com";

	var user = await userManager.FindByNameAsync(adminUserName);
	if (user == null)
	{
		user = new UserModel { UserName = adminUserName, Email = adminEmail, EmailConfirmed = true };
		await userManager.CreateAsync(user, "Admin1@");
	}

	var userRoles = await userManager.GetRolesAsync(user);
	if (!userRoles.Contains(AccountController.AdminRole))
	{
		if (userRoles.Count > 0)
		{
			await userManager.RemoveFromRolesAsync(user, userRoles);
		}
		await userManager.AddToRoleAsync(user, AccountController.AdminRole);
	}
}


app.Run();
