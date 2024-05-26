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

    public async ValueTask<UserModel> CreateUserAsync(UserCreate userCreate)
    {
        var isExist = await Database.Users.AnyAsync(u => u.Email == userCreate.Email);
        if (isExist) throw new Exception($"User with email:{userCreate.Email} already exisats");

        var user = User.Create(userCreate.Email, userCreate.Username, userCreate.LogoUrl);
        return await Add(user);
    }
}