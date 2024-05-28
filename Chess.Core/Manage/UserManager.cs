using System.Linq.Expressions;
using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Chess.Core.Manage;

public class UserManager(IMongoDatabase db) : BaseManager<User, UserModel, long>(db)
{
    protected override Expression<Func<User, UserModel>> EntityToModel => e =>
        new UserModel(e.Id, e.Username, e.Email, e.Date);

    public async ValueTask<UserModel> GetOrCreateUserAsync(UserCreate userCreate)
    {
        var existingUser = await Database.Users.FirstOrDefaultAsync(u => u.Email == userCreate.Email);
        if (existingUser is not null) return EntityToModel.Compile().Invoke(existingUser);

        var user = User.Create(userCreate.Email, userCreate.Username, userCreate.LogoUrl);
        return await Add(user);
    }
}