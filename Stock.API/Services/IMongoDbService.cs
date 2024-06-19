using MongoDB.Driver;

namespace Stock.API.Services
{
    public interface IMongoDbService
    {
        IMongoCollection<T> GetCollection<T>();
    }
}
