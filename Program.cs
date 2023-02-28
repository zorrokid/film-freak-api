using Microsoft.EntityFrameworkCore;
using FilmFreakApi.Models;
using FilmFreakApi.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FilmFreakApi.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var dbOptions = configuration.GetSection(DatabaseOptions.Database).Get<DatabaseOptions>();
if (dbOptions == null)
{
    throw new Exception("Database options not configured.");
}

// Add services to the container.

var corsOptions = configuration.GetSection(CorsOptions.CORS).Get<CorsOptions>();
if (corsOptions == null)
{
    throw new Exception("CORS options not configured.");
}
builder.Services.AddCors(options => options.AddDefaultPolicy(
    policy =>
    {
        policy
            .WithOrigins(corsOptions.AllowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    }
));

builder.Services.AddControllers();


builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(dbOptions.AuthDbConnectionString));
builder.Services.AddDbContext<FilmFreakContext>(options => options.UseNpgsql(dbOptions.ConnectionString));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Auth

var jwtOptions = configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>();

if (jwtOptions == null)
{
    throw new Exception("JWT options not configured.");
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
        ValidateLifetime = true,
        ValidAudience = jwtOptions.ValidAudience,
        ValidIssuer = jwtOptions.ValidIssuer,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.\r\n\r\n 
            Enter 'Bearer' and token separated with space in the text input below."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// Note: if multiple instances of this app is run simultaniously, something like this needs to be used:
// https://www.thereformedprogrammer.net/how-to-safely-apply-an-ef-core-migrate-on-asp-net-core-startup/
builder.Services.AddHostedService<ConsumeAuthDbInitializationService>();
builder.Services.AddHostedService<ConsumeFilmFreakDbInitializationService>();
builder.Services.AddScoped<IAuthDbInitializationService, AuthDbInitializationService>();
builder.Services.AddScoped<IFilmFreakDbInitializationService, FilmFreakDbInitializationService>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
