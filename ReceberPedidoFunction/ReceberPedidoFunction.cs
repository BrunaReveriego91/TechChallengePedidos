using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TechChallengePedidos.Model.Model;

namespace ReceberPedidoFunction
{
    public static class ReceberPedidoFunction
    {
        [FunctionName("ReceberPedidoFunction")]
        public static async Task<IActionResult> ReceberPedido(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter)
        {
            try
            {
                // Ler o corpo da requisição como um JSON para obter o pedido
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Pedido pedido = JsonConvert.DeserializeObject<Pedido>(requestBody);

                // Iniciar a instância do orquestrador
                string instanceId = await starter.StartNewAsync("RunOrchestrator", pedido);
                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            catch
            {
                return new BadRequestObjectResult("Falha ao receber pedido.");
            }
        }

        [FunctionName("RunOrchestrator")]
        public static async Task<Pedido> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var pedido = context.GetInput<Pedido>();

            return pedido;
        }
    }
}