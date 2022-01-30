var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("ToDoDatabase"));
builder.Services.AddSingleton<ToDoService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/all", async (ToDoService service) => await service.GetAsync());
app.MapGet("/one/{id}", async (string id, ToDoService service) => await service.GetAsync(id));
app.MapPost("/add", async ([FromBody] ToDoItem item, ToDoService service) => { 
    await service.CreateAsync(item);
    return Results.Ok();
} );

app.Run();
