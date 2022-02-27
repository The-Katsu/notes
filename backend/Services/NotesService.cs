public class NotesService
{
    private readonly IMongoCollection<Note> _collection;

    public NotesService(IOptions<DbSettings> dbSettings)
    {
        var mongoClient = new MongoClient(
                dbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<Note>(
            dbSettings.Value.CollectionName);
    }

    public async Task CreateAsync(Note note) =>
        await _collection.InsertOneAsync(note);

    public async Task UpdateAsync(Note note) =>
        await _collection.ReplaceOneAsync(x => x.Id == note.Id, note);

    public async Task<List<Note>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Note?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task RemoveAsync(string id) => 
        await _collection.DeleteOneAsync(x => x.Id == id);
}

public class DbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}