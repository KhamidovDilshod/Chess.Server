namespace Chess.Core.Persistence;

public class MongoOptions
{
    public required string DatabaseName { get; set; }
    public required string ConnectionString { get; set; }
}