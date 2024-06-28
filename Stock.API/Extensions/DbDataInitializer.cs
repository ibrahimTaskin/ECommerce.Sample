using MongoDB.Driver;
using Stock.API.Services;

namespace Stock.API.Extensions
{
    public static class DbDataInitializer
    {
        public static async Task AddDataToMongo(this IServiceCollection services)
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            IMongoDbService mongoDbService = scope.ServiceProvider.GetService<IMongoDbService>();
            var collection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
            if(!collection.FindSync<Stock.API.Models.Entities.Stock>(s => true).Any())
            {
                await collection.InsertOneAsync(new Models.Entities.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 100 });
                await collection.InsertOneAsync(new Models.Entities.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 300 });
                await collection.InsertOneAsync(new Models.Entities.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 150 });
                await collection.InsertOneAsync(new Models.Entities.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 225 });
                await collection.InsertOneAsync(new Models.Entities.Stock() { ProductId = Guid.NewGuid().ToString(), Count = 450 });
            }
        }
    }
}
