using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechChallengePedidos.Model.Model.EstoqueProdutos;
using TechChallengePedidos.Model.Model.Pedidos;
using TechChallengePedidos.Model.Model.Produtos;

namespace OrquestradorPedidoFunction
{
    public static class OrquestradorPedidoFunction
    {
        [FunctionName("OrquestrarPedidoFunction")]
        public static async Task<IActionResult> ReceberPedido(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter)
        {
            try
            {
                // Ler o corpo da requisição como um JSON para obter o pedido
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                PedidoModel pedido = JsonConvert.DeserializeObject<PedidoModel>(requestBody);


                if (!ValidarPedido.PedidoValido(pedido))
                    return new BadRequestObjectResult(string.Concat("Pedido Inválido :", ValidarPedido.Mensagem));

                var client = new MongoClient(System.Environment.GetEnvironmentVariable("MongoDBAtlasConnectionString"));
                var database = client.GetDatabase("db_tech_challenge");
                var collection = database.GetCollection<EstoqueModel>("estoque");
                var estoqueItems = await collection.Find(_ => true).ToListAsync();

                if (!estoqueItems.Any())
                    throw new System.Exception();

                var collectionProduto = database.GetCollection<CadastroProdutoModel>("produtos");
                var produtoItems = await collectionProduto.Find(_ => true).ToListAsync();

                if (!produtoItems.Any())
                    throw new System.Exception();

                foreach (var estoque in estoqueItems)
                {
                    foreach (var produtoPedido in pedido.Produtos)
                    {
                        
                        var produtoEstoque = estoque.Estoque.Where(x => x.ProdutoId == produtoPedido.Id).FirstOrDefault();

                        if(produtoEstoque == null)
                            return new BadRequestObjectResult("Falha ao orquestrar pedido, existem produtos no pedido não cadastrados no estoque.");

                        var informacaoProduto = produtoItems.FirstOrDefault().Produtos.Where(x => x.ProdutoId == produtoPedido.Id).FirstOrDefault();
                        
                        if (informacaoProduto == null)
                            return new BadRequestObjectResult("Falha ao orquestrar pedido, existem produtos no pedido não cadastrados.");

                        if (produtoPedido.Quantidade > produtoEstoque.Qtde)
                            return new BadRequestObjectResult($"Falha ao orquestrar pedido, produto {informacaoProduto.Descricao} não possui quantidade suficiente em estoque.");

                    }
                }

                return new OkObjectResult(pedido);
            }
            catch
            {
                return new BadRequestObjectResult("Falha ao orquestrar pedido.");
            }
        }

    }
}