using Microsoft.EntityFrameworkCore;
using FilmFreakApi.Models;

var builder = WebApplication.CreateBuilder(args);

// user secrets are read by default?
// and environment specific appsettings?
var connectionString = builder.Configuration["Database:ConnectionString"];

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<FilmFreakContext>(options => options.UseNpgsql(connectionString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
