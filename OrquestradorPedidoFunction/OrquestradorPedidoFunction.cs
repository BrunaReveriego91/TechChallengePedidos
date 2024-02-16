using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
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

                var estoque = estoqueItems.FirstOrDefault();

                foreach (var produtoPedido in pedido.Produtos)
                {

                    var produtoEstoque = estoque.Estoque.Where(x => x.ProdutoId == produtoPedido.Id).FirstOrDefault();

                    if (produtoEstoque == null)
                        return new BadRequestObjectResult("Falha ao orquestrar pedido, existem produtos no pedido não cadastrados no estoque.");

                    var informacaoProduto = produtoItems.FirstOrDefault().Produtos.Where(x => x.ProdutoId == produtoPedido.Id).FirstOrDefault();

                    if (informacaoProduto == null)
                        return new BadRequestObjectResult("Falha ao orquestrar pedido, existem produtos no pedido não cadastrados.");

                    if (produtoPedido.Quantidade > produtoEstoque.Qtde)
                        return new BadRequestObjectResult($"Falha ao orquestrar pedido, produto {informacaoProduto.Descricao} não possui quantidade suficiente em estoque.");

                }

                var numeroPedido = await ArmazenarPedidoBlobStorage(pedido);

                pedido.IdPedido = numeroPedido;

                return new OkObjectResult(pedido);
            }
            catch
            {
                return new BadRequestObjectResult("Falha ao orquestrar pedido.");
            }
        }

        private async static Task<int> ArmazenarPedidoBlobStorage(PedidoModel pedido)
        {
            bool blobNameValido = false;
            int numeroPedido = 1;

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("pedido-blob-storage");

            await container.CreateIfNotExistsAsync();


            do
            {
                var rnd = new Random();
                numeroPedido = rnd.Next(1, 99999999);

                var blobName = string.Concat("Pedido_", numeroPedido, "_Cliente_", pedido.Cliente.CPF);
                var blobExistente = container.GetBlockBlobReference(blobName);

                if (!await blobExistente.ExistsAsync())
                {
                    var json = JsonConvert.SerializeObject(pedido);

                    using (var stream = new MemoryStream())
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(json);
                        writer.Flush();
                        stream.Position = 0;
                        await blobExistente.UploadFromStreamAsync(stream);
                    }

                    blobNameValido = true;
                }

            } while (!blobNameValido);

            return numeroPedido;
        }
    }
}