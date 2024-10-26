using DataAccess;
using FormAPI.DTO;
using FormAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using SharedAuth;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configure Serilog for more detailed logging (optional but recommended)
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole()
        .AddDebug()
        .AddEventSourceLogger()
        .SetMinimumLevel(LogLevel.Debug);
});

//Add shared configuration file
var sharedSettingsPath = Path.Combine(builder.Environment.ContentRootPath, "..", "AuthMethod", "sharedsettings.json");
builder.Configuration.AddJsonFile(sharedSettingsPath, optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("DataAccess")));

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

// Register DAOs
builder.Services.AddScoped<FormDAO>();
builder.Services.AddScoped<DepartmentDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<ResponseDAO>();
builder.Services.AddScoped<UserDAO>();

//Add Repositories
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IResponseRepository, ResponseRepository>();

builder.Services.AddTransient<IAuthentication, Authentication>();

//Add automapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
