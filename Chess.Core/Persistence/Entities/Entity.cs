using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chess.Core.Persistence.Entities;

public class Entity<T> where T : notnull
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public virtual T Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
}

public class Entity : Entity<Guid>
{
    public override  Guid Id { get; set; }=Guid.NewGuid();
}