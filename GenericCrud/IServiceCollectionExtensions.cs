using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericCrud
{
    public static class IServiceCollectionExtensions
    {
        public static void AddGenericCrud<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddScoped<DbContext, TDbContext>();
            services.AddScoped(typeof(ICrudService<,,>), typeof(CrudService<,,>));
        }
    }
}
