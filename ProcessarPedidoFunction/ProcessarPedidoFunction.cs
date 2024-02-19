using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TechChallengePedidos.Model.Model.EstoqueProdutos;
using TechChallengePedidos.Model.Model.Pedidos;
using TechChallengePedidos.Model.Model.Produtos;

public class ProcessarPedidoFunction
{
    private static IMongoDatabase ObterConexaoDatabase()
    {
        var client = new MongoClient(System.Environment.GetEnvironmentVariable("MongoDBAtlasConnectionString"));
        var database = client.GetDatabase("db_tech_challenge");
        return database;
    }

    [FunctionName("ProcessarPedidoFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PedidoModel pedido = JsonConvert.DeserializeObject<PedidoModel>(requestBody);

            var pedidoInstanceId = await starter.StartNewAsync("EnviarEmailOrchestrator", pedido);

            return starter.CreateCheckStatusResponse(req, pedidoInstanceId);
        }
        catch
        {
            return new BadRequestObjectResult("Falha ao processar pedido.");
        }
    }

    [FunctionName("EnviarEmailOrchestrator")]
    public async Task EnviarEmailOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var pedido = context.GetInput<PedidoModel>();

        var produtoItems = await context.CallActivityAsync<List<CadastroProdutoModel>>("ObterProdutos", null);

        foreach (var produtoPedido in pedido.Produtos)
        {
            var produtoItem = produtoItems.FirstOrDefault();
            if (!produtoItem.Produtos.Any())
                throw new Exception();

            produtoPedido.DescricaoProduto = produtoItem.Produtos.Where(x => x.ProdutoId == produtoPedido.Id).FirstOrDefault().Descricao;
            produtoPedido.Valor = produtoItem.Produtos.Where(x => x.ProdutoId == produtoPedido.Id).FirstOrDefault().Valor;
        }

        await context.CallActivityAsync("EnviarEmailActivity", pedido);
    }

    [FunctionName("ObterProdutos")]
    public async Task<List<CadastroProdutoModel>> ObterProdutos([ActivityTrigger] IDurableActivityContext context)
    {
        IMongoDatabase database = ObterConexaoDatabase();
        var collection = database.GetCollection<CadastroProdutoModel>("produtos");

        return await collection.Find(_ => true).ToListAsync();

    }

    [FunctionName("AtualizarEstoqueProduto")]
    public async Task AtualizarEstoqueProduto([ActivityTrigger] IDurableActivityContext context)
    {

        var produtoPedido = context.GetInput<ProdutoPedidoModel>();

        IMongoDatabase database = ObterConexaoDatabase();
        var collection = database.GetCollection<EstoqueModel>("estoque");

        var estoqueProduto = await collection.Find(x => x.Estoque.Any(e => e.ProdutoId == produtoPedido.Id))
                                       .Project(x => x.Estoque.FirstOrDefault(e => e.ProdutoId == produtoPedido.Id))
                                       .FirstOrDefaultAsync();

        var qtdeProduto = estoqueProduto.Qtde - produtoPedido.Quantidade;


        var filter = Builders<EstoqueModel>.Filter.ElemMatch(x => x.Estoque, e => e.ProdutoId == produtoPedido.Id);

        var update = Builders<EstoqueModel>.Update.Set("estoque.$[elem].Qtde", qtdeProduto);

        var arrayFilters = new List<ArrayFilterDefinition>
        {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("elem.ProdutoId", produtoPedido.Id))
        };

        var options = new UpdateOptions { ArrayFilters = arrayFilters };

        await collection.UpdateOneAsync(filter, update, options);
    }

    [FunctionName("EnviarEmailActivity")]
    public async Task EnviarEmailActivity([ActivityTrigger] PedidoModel pedido)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var smtpUserNameLocal = config["smtpUserName"];
        var GoogleAppPassword = config["GoogleAppPassword"];

        var fromAddress = smtpUserNameLocal;
        var toAddress = pedido.Cliente.Email;
        var subject = $"Pedido {pedido.IdPedido} recebido.";
        var body = new StringBuilder();


        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromAddress, GoogleAppPassword),
            EnableSsl = true,
        };


        body.AppendLine($"<div>Olá {pedido.Cliente.Nome}, seu pedido foi recebido e em breve será postado. </div>" +
                        $"<br><div>Detalhes do pedido:</div><br>");

        foreach (var item in pedido.Produtos)
        {
            body.AppendLine($"<b>Qtd:</b> {item.Quantidade}, - <b>Descrição Produto:</b> {item.DescricaoProduto} , - <b>R$</b> {item.Valor} <br>");
        }

        var mailMessage = new MailMessage(fromAddress, toAddress, subject, body.ToString());
        mailMessage.IsBodyHtml = true;


        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception("Falha ao enviar e-mail:", ex);
        }
    }
}