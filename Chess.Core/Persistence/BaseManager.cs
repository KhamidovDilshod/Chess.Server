using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MongoDB.Driver;

namespace Chess.Core.Persistence;

public abstract class BaseManager(MongoOptions settings)
{
    private readonly IMongoDatabase _database =
        new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);

    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected IMongoCollection<TSet> Set<TSet>() =>
        _database.GetCollection<TSet>(GetCollectionName(typeof(TSet)));

    private string? GetCollectionName(ICustomAttributeProvider type)
    {
        var attributes = type
            .GetCustomAttributes(typeof(BsonCollectionAttribute), true);

        var collectionName = ((BsonCollectionAttribute)attributes.First()).CollectionName;
        return collectionName;
    }

    public Task<IEnumerable<TSet>> GetAll<TSet>() => Task.Run(() => Set<TSet>().AsQueryable().AsEnumerable());

    public Task<TSet> Get<TSet, TKey>(TKey id) where TSet : Entity =>
        Task.Run(() => Set<TSet>().Find(e => e.Id.Equals(id)).FirstOrDefaultAsync());


    protected Task Add<TSet>(TSet entity) => Task.Run(() => Set<TSet>().InsertOneAsync(entity));
}