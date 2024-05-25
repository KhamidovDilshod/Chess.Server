namespace Chess.Core.Persistence.Entities;

public class Entity<T> where T : notnull
{
    public T Id { get; set; }
    public DateTime Date { get; set; }
}

public class Entity : Entity<Guid>
{
}