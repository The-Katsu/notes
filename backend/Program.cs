using DataAcess;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("ToDoDatabase"));
builder.Services.AddSingleton<ToDoService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/all", async (ToDoService service) => await service.GetAsync());
app.MapPost("/add", async ([FromBody] ToDoItem item, ToDoService service) => { 
    await service.CreateAsync(item);
    return Results.Ok();
} );

app.Run();
