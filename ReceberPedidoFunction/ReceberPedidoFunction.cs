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
        /*Alterar conforme necessidade*/

        public const string urlOrquestrador = $"http://localhost:7071/api/OrquestrarPedidoFunction";

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

                var response = await httpClient.PostAsync(urlOrquestrador, content);

                string mensagem = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return new BadRequestObjectResult(mensagem);


                return new OkObjectResult("Pedido recebido com sucesso.");

            }
            catch
            {
                return new BadRequestObjectResult("Falha ao receber pedido.");
            }
        }
    }
}