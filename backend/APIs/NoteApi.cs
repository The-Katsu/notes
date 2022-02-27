public class NoteApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("/", [Authorize] async (NotesService service) =>
            await service.GetAsync());

        app.MapGet("/{id:length(24)}", [Authorize] async (string id, NotesService service) =>
            await service.GetAsync(id) is Note item
            ? Results.Ok(item)
            : Results.NotFound());

        app.MapPost("/", [Authorize] async ([FromBody] Note item, NotesService service) => {
            if (item.Id == null)
                await service.CreateAsync(item);
            else await service.UpdateAsync(item);
            return Results.Created($"/{item.Id}", item);
        });

        app.MapDelete("/{id:length(24)}", [Authorize] async (string id, NotesService service) => {
            await service.RemoveAsync(id);
            return Results.NoContent();
        });
    }
}
