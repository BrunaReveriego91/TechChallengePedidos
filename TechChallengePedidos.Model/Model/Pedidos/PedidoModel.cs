
namespace TechChallengePedidos.Model.Model.Pedidos
{
    public class PedidoModel
    {
        public int IdPedido { get; set; }
        public ClientePedidoModel Cliente { get; set; }
        public List<ProdutoPedidoModel> Produtos { get; set; }
    }
}
