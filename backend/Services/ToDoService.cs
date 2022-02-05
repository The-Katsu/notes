public class ToDoService
{
    private readonly IMongoCollection<ToDoItem> _collection;

    public ToDoService(IOptions<DbSettings> dbSettings)
    {
        var mongoClient = new MongoClient(
                dbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<ToDoItem>(
            dbSettings.Value.CollectionName);
    }

    public async Task CreateAsync(ToDoItem item) =>
        await _collection.InsertOneAsync(item);

    public async Task UpdateAsync(ToDoItem item) =>
        await _collection.ReplaceOneAsync(x => x.Id == item.Id, item);

    public async Task<List<ToDoItem>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<ToDoItem?> GetAsync(string id) =>
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