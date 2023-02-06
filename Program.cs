using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    var certPem = File.ReadAllText("/home/mikko/localhost+1.pem");
    var eccPem = File.ReadAllText("/home/mikko/localhost+1-key.pem");

    var cert = X509Certificate2.CreateFromPem(certPem, eccPem);

    serverOptions.Listen(IPAddress.Any, 5054, listenoptions => {
        listenoptions.UseHttps(cert);
    });

});

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
