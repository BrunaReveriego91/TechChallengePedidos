namespace TechChallengePedidos.Model.Model
{
    public class Pedido
    {
        public Cliente Cliente { get; set; }
        public EnderecoCliente Endereco { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
