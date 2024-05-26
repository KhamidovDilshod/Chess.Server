using System.Linq.Expressions;
using System.Text.Json;
using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Chess.Core.Persistence;

public abstract class BaseManager<T, TModel, TKey>(IMongoDatabase db) where T : Entity<TKey> where TKey : notnull
{
    protected readonly Db Database = Db.Create(db);

    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected abstract Expression<Func<T, TModel>> EntityToModel { get; }

    public Task<IEnumerable<TModel>> GetAll() => Task.Run(() => Database.Set<T>()
        .Select(EntityToModel)
        .AsEnumerable());

    public Task<TModel?> Get(TKey id) => Database
        .Set<T>()
        .Where(e => e.Id.Equals(id))
        .Select(EntityToModel)
        .FirstOrDefaultAsync();


    public async ValueTask<TModel> Add(T entity)
    {
        var entry = Database.Set<T>().Add(entity);
        await Database.SaveChangesAsync();
        return EntityToModel.Compile().Invoke(entry.Entity);
    }
}