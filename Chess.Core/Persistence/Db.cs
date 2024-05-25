using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chess.Core.Persistence;

public class Db : DbContext
{
    public DbSet<User> Users { get; set; }
}