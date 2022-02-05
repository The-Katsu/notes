public class ToDoApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("/", [Authorize] async (ToDoService service) =>
            await service.GetAsync());

        app.MapGet("/{id:length(24)}", [Authorize] async (string id, ToDoService service) =>
            await service.GetAsync(id) is ToDoItem item
            ? Results.Ok(item)
            : Results.NotFound());

        app.MapPost("/", [Authorize] async ([FromBody] ToDoItem item, ToDoService service) => {
            if (item.Id == null)
                await service.CreateAsync(item);
            else await service.UpdateAsync(item);
            return Results.Created($"/{item.Id}", item);
        });

        app.MapDelete("/{id:length(24)}", [Authorize] async (string id, ToDoService service) => {
            await service.RemoveAsync(id);
            return Results.NoContent();
        });
    }
}
