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

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibDBContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDB"),
		npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
});


builder.Services.AddDbContext<AuthDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("AccountDB"),
		npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
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


builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.Name = "LibraryCookie";
	options.Cookie.SameSite = SameSiteMode.Strict;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.LoginPath = "/Account/Login";
	options.AccessDeniedPath = "/Account/AccessDenied";
});

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

app.Run();
