using Microsoft.EntityFrameworkCore;
using FilmFreakApi.Models;
using FilmFreakApi.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// user secrets are read by default?
// and environment specific appsettings?
var connectionString = configuration["Database:ConnectionString"];
var authDbConnectionString = configuration["Database:AuthDbConnectionString"];

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(authDbConnectionString));
builder.Services.AddDbContext<FilmFreakContext>(options => options.UseNpgsql(connectionString));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Auth

var validIssuer = configuration["JWT:ValidIssuer"];
var validAudience = configuration["JWT:ValidAudience"];
var secret = configuration["JWT:Secret"];
if (string.IsNullOrEmpty(validIssuer) || string.IsNullOrEmpty(validAudience) || string.IsNullOrEmpty(secret))
{
    throw new Exception("One or more of the JWT token settings are missing.");
}

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = validIssuer,
        ValidIssuer = validAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<ConsumeAuthDbInitializationService>();
builder.Services.AddScoped<IAuthDbInitializationService, AuthDbInitializationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
