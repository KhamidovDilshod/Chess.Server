using System.Linq.Expressions;
using System.Reflection;
using Chess.Core.Persistence.Entities;
using MongoDB.Driver;

namespace Chess.Core.Persistence;

public abstract class MongoDb(MongoOptions settings)
{
    private readonly IMongoDatabase _database =
        new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);

    protected IMongoCollection<TSet> Set<TSet>() =>
        _database.GetCollection<TSet>(GetCollectionName(typeof(TSet)));

    private static string? GetCollectionName(ICustomAttributeProvider type)
    {
        var attributes = type
            .GetCustomAttributes(typeof(BsonCollectionAttribute), true);

        var collectionName = ((BsonCollectionAttribute)attributes.First()).CollectionName;
        return collectionName;
    }

    public Task<IEnumerable<TSet>> GetAll<TSet>() => Task.Run(() => Set<TSet>().AsQueryable().AsEnumerable());

    public Task<TSet> Get<TSet, TKey>(TKey id) where TSet : Entity =>
        Task.Run(() => Set<TSet>().Find(e => e.Id.Equals(id)).FirstOrDefaultAsync());

    protected Task<List<TSet>> GetAll<TSet>(Expression<Func<TSet, bool>> func) where TSet : Entity =>
        Task.Run(() => Set<TSet>().Find(func).ToList());


    protected async Task Add<TSet>(TSet entity)
    {
        var collection = Set<TSet>();
        if (collection == null)
            throw new InvalidOperationException($"Collection for {typeof(TSet).Name} not found.");
        await collection.InsertOneAsync(entity);
    }

    protected async Task Update<TSet>(TSet entity) where TSet : Entity
    {
        var collection = Set<TSet>();
        if (collection == null)
            throw new InvalidOperationException($"Collection for {typeof(TSet).Name} not found.");
        await collection.ReplaceOneAsync(g => g.Id == entity.Id, entity);
    }
}