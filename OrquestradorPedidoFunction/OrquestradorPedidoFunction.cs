using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechChallengePedidos.Model.Model.EstoqueProdutos;
using TechChallengePedidos.Model.Model.Pedidos;
using TechChallengePedidos.Model.Model.Produtos;

namespace OrquestradorPedidoFunction
{
    public class OrquestradorPedidoFunction
    {
        public const string urlProcessarPedido = "http://localhost:7073/api/ProcessarPedidoFunction";
        private static IMongoDatabase ObterConexaoDatabase()
        {
            var client = new MongoClient(System.Environment.GetEnvironmentVariable("MongoDBAtlasConnectionString"));
            var database = client.GetDatabase("db_tech_challenge");
            return database;
        }

        [FunctionName("OrquestrarPedidoFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                PedidoModel pedido = JsonConvert.DeserializeObject<PedidoModel>(requestBody);

                if (!ValidarPedido.PedidoValido(pedido))
                    return new BadRequestObjectResult($"Pedido Inv�lido: {ValidarPedido.Mensagem}");

                var orchestrationInstanceId = await starter.StartNewAsync("OrquestradorPedido", null, pedido);

                return starter.CreateCheckStatusResponse(req, orchestrationInstanceId);
            }
            catch
            {
                return new BadRequestObjectResult("Falha ao orquestrar pedido.");
            }
        }

        [FunctionName("OrquestradorPedido")]
        public async Task<List<string>> OrquestradorPedido(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var pedido = context.GetInput<PedidoModel>();

            var estoqueItems = await context.CallActivityAsync<List<EstoqueModel>>("ObterEstoque", null);
            var produtoItems = await context.CallActivityAsync<List<CadastroProdutoModel>>("ObterProdutos", null);

            if (!estoqueItems.Any() || !produtoItems.Any())
                throw new System.Exception();

            var estoque = estoqueItems.FirstOrDefault();

            var outputs = new List<string>();

            foreach (var produtoPedido in pedido.Produtos)
            {
                var produtoEstoque = estoque.Estoque.FirstOrDefault(x => x.ProdutoId == produtoPedido.Id);

                if (produtoEstoque == null)
                    throw new System.Exception("Falha ao orquestrar pedido, existem produtos no pedido n�o cadastrados no estoque.");

                var informacaoProduto = produtoItems.SelectMany(x => x.Produtos).FirstOrDefault(x => x.ProdutoId == produtoPedido.Id);

                if (informacaoProduto == null)
                    throw new System.Exception("Falha ao orquestrar pedido, existem produtos no pedido n�o cadastrados.");

                if (produtoPedido.Quantidade > produtoEstoque.Qtde)
                    throw new System.Exception($"Falha ao orquestrar pedido, produto {informacaoProduto.Descricao} n�o possui quantidade suficiente em estoque.");

            }

            var numeroPedido = await context.CallActivityAsync<int>("ArmazenarPedidoBlobStorage", pedido);
            pedido.IdPedido = numeroPedido;

            await context.CallActivityAsync<int>("ProcessarPedido", pedido);

            outputs.Add(JsonConvert.SerializeObject(pedido));

            return outputs;
        }


        [FunctionName("ObterEstoque")]
        public async Task<List<EstoqueModel>> ObterEstoque([ActivityTrigger] IDurableActivityContext context)
        {
            IMongoDatabase database = ObterConexaoDatabase();
            var collection = database.GetCollection<EstoqueModel>("estoque");

            return await collection.Find(_ => true).ToListAsync();

        }

        [FunctionName("ObterProdutos")]
        public async Task<List<CadastroProdutoModel>> ObterProdutos([ActivityTrigger] IDurableActivityContext context)
        {
            IMongoDatabase database = ObterConexaoDatabase();
            var collection = database.GetCollection<CadastroProdutoModel>("produtos");

            return await collection.Find(_ => true).ToListAsync();

        }

        [FunctionName("ArmazenarPedidoBlobStorage")]
        public async Task<int> ArmazenarPedidoBlobStorage([ActivityTrigger] IDurableActivityContext context)
        {
            var pedido = context.GetInput<PedidoModel>();

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("pedido-blob-storage");

            await container.CreateIfNotExistsAsync();

            int numeroPedido;
            bool blobNameValido;
            do
            {
                var rnd = new Random();
                numeroPedido = rnd.Next(1, 99999999);

                var blobName = $"Pedido_{numeroPedido}_Cliente_{pedido.Cliente.CPF}";
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
                else
                {
                    blobNameValido = false;
                }

            } while (!blobNameValido);

            return numeroPedido;
        }


        [FunctionName("ProcessarPedido")]
        public async Task ProcessarPedido([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var pedido = context.GetInput<PedidoModel>();

            string requestBody = JsonConvert.SerializeObject(pedido);

            var response = await context.CallHttpAsync(HttpMethod.Post, new Uri(urlProcessarPedido), requestBody);

            if ((int)response.StatusCode < 200 || (int)response.StatusCode > 202)
                throw new Exception($"Falha ao orquestrar pedido. Status code: {response.StatusCode}");


        }
    }
}

