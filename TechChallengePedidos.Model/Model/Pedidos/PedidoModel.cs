
namespace TechChallengePedidos.Model.Model.Pedidos
{
    public class PedidoModel
    {
        public ClientePedidoModel Cliente { get; set; }
        public EnderecoClientePedidoModel Endereco { get; set; }
        public List<ProdutoPedidoModel> Produtos { get; set; }
    }
}
