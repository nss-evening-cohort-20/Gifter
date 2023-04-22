using Gifter.Repositories;

namespace Gifter.Utils;

public static class Extensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IPostRepository, PostRepository>();
    }
}
