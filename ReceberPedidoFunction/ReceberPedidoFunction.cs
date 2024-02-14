using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();

                await httpClient.PostAsync("http://localhost:7071/api/OrquestrarPedidoFunction", content);

                return new OkObjectResult("Pedido recebido com sucesso.");

            }
            catch
            {
                return new BadRequestObjectResult("Falha ao receber pedido.");
            }
        }
    }
}