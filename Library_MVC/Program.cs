using Library_MVC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibDBContext>(response =>
{
	response.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDB"));
});

builder.Services.AddDbContext<UserContext>(options =>
{
	//options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDB"));
});

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(auth =>
{
	auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	auth.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(cookie =>
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


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
