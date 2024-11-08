using Mongo2Go;

namespace Chess.Tests.Persistence;

public class MongoDbTests
{
    private readonly MongoDbRunner _runner;

    public MongoDbTests(MongoDbRunner runner)
    {
        _runner = runner;
    }
}