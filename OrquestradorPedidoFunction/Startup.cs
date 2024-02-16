using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TechChallengePedidos.Repository.Interfaces;
using TechChallengePedidos.Repository.Repository;

[assembly: FunctionsStartup(typeof(OrquestradorPedidoFunction.Startup))]

namespace OrquestradorPedidoFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(typeof(IMongoDbRepository<>), typeof(MongoDbRepository<>));
        }
    }
}
