using MongoDB.Driver;

namespace TechChallengePedidos.Repository.Interfaces
{
    public interface IMongoDbRepository<T>
    {
        Task<List<T>> ObterDatabaseCollection<T>(string nomeCollection);
    }
}
