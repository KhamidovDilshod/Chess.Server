using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Chess.Core.Persistence;

public class Db : DbContext
{
    public Db(DbContextOptions options) : base(options)
    {
    }

    public static Db Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<Db>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameState> GameStates { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<GamePlayer> Players { get; set; }
    public DbSet<Board> Boards { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasOne(g => g.Board)
            .WithOne(b => b.Game)
            .HasForeignKey<Board>(b => b.GameId);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Players)
            .WithOne(p => p.Game)
            .HasForeignKey(p => p.GameId);

        modelBuilder.Entity<Board>()
            .HasOne(b=>b.Game)
            .WithOne(g=>g.Board)
            .HasForeignKey<Board>(b=>b.GameId);
        
        base.OnModelCreating(modelBuilder);
    }
}