namespace Chess.Core.Extensions;

public static class SignalRExtensions
{
    public static void RegisterSignalRDependencies(this IServiceProvider serviceProvider)
    {
        // GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyIdProvider());
    } 
}