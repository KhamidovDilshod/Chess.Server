using Chess.Core.Models;
using Chess.Core.Persistence;
using Chess.Core.Persistence.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Chess.Core.Manage;

public class UserManager(IOptions<MongoOptions> options) : BaseManager(options.Value), IManager
{
    public async ValueTask<UserModel> GetOrCreateUserAsync(UserCreate userCreate)
    {
        var existingUser = await Set<User>().Find(u => u.Email == userCreate.Email).FirstOrDefaultAsync();
        if (existingUser is not null) return ToModel(existingUser);

        var user = User.Create(userCreate.Email, userCreate.Username, userCreate.LogoUrl);
        await Add(user);
        return ToModel(await Get<User, Guid>(user.Id));
    }

    private static UserModel ToModel(User user) => new(user.Id, user.Username, user.Email, user.Date);
}