using Application;
using FappLogger;
using Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .ConfigureInfrastructure()
    .ConfigureApplication()
    .ConfigureWebApi();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.Run();