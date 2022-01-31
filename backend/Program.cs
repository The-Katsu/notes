var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("ToDoDatabase"));
builder.Services.AddSingleton<ToDoService>();

var app = builder.Build();

app.MapGet("/", async (ToDoService service) => 
    await service.GetAsync());

app.MapGet("/{id:length(24)}", async (string id, ToDoService service) => 
    await service.GetAsync(id) is ToDoItem item 
    ? Results.Ok(item)
    : Results.NotFound());

app.MapPost("/", async ([FromBody] ToDoItem item, ToDoService service) => { 
    if (item.Id == null)
        await service.CreateAsync(item);
    else await service.UpdateAsync(item);
    return Results.Created($"/{item.Id}", item);
});

app.MapDelete("/{id:length(24)}", async (string id, ToDoService service) => {
    await service.RemoveAsync(id);
    return Results.NoContent();
});

app.Run();