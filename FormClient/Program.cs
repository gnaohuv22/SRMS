
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SharedAuth;

var builder = WebApplication.CreateBuilder(args);

//Add shared configuration file
var sharedSettingsPath = Path.Combine(builder.Environment.ContentRootPath, "..", "AuthMethod", "sharedsettings.json");
builder.Configuration.AddJsonFile(sharedSettingsPath, optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(15);
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied";
        options.LogoutPath = "/User/Logout";
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
