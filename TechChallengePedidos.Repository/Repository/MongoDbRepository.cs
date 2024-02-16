using MongoDB.Driver;
using TechChallengePedidos.Repository.Interfaces;

namespace TechChallengePedidos.Repository.Repository
{
    public class MongoDbRepository<T> : IMongoDbRepository<T>
    {
        public async Task<List<T>> ObterDatabaseCollection<T>(string nomeCollection)
        {
            var client = new MongoClient(System.Environment.GetEnvironmentVariable("MongoDBAtlasConnectionString"));
            var database = client.GetDatabase("db_tech_challenge");
            var collection = database.GetCollection<T>(nomeCollection);

            return await collection.Find(_ => true).ToListAsync();
        }
    }
}
