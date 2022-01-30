using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DataAcess
{
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
    }

    public class DbSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string CollectionName { get; set; } = null!;
    }

    public class ToDoItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Text { get; set; } = string.Empty;
        //public bool IsDone { get; set; } = false;
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}