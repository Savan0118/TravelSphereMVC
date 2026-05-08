using Microsoft.EntityFrameworkCore;
using TravelSphereMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var googleClientId     = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

var authBuilder = builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath       = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan    = TimeSpan.FromHours(8);
    });

bool googleEnabled = !string.IsNullOrWhiteSpace(googleClientId)
                  && googleClientId != "258259180885-klcsf42e64bl5kqhoirqtptke7gruvob.apps.googleusercontent.com"
                  && !string.IsNullOrWhiteSpace(googleClientSecret)
                  && googleClientSecret != "GOCSPX-abc123xyz";

if (googleEnabled)
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId     = googleClientId!;
        options.ClientSecret = googleClientSecret!;
    });
}

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
    name:    "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await TravelSphereMVC.Data.DbSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database seeding failed. The application will still start.");
    }
}

app.Run();
