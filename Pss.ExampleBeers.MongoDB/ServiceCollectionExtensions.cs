using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pss.ExampleBeers.Domain.Interfaces;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDataStoreConfig>(
                configuration.GetSection(MongoDataStoreConfig.Section));
            
            services.AddSingleton<IMongoConnection, MongoConnection>();

            services.AddTransient<IBeerRepository, BeerRepository>();
        }
    }
}