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
	cookie.Cookie.SameSite = SameSiteMode.Strict;
	cookie.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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
