using DataAcess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("ToDoDatabase"));
builder.Services.AddSingleton<ToDoService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
