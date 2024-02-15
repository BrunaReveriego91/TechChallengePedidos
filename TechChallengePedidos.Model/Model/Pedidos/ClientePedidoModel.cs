namespace TechChallengePedidos.Model.Model.Pedidos
{
    public class ClientePedidoModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string ContatoPrimario { get; set; }
        public string ContatoSecundario { get; set; }
        public DateTime Nascimento { get; set; }
        public EnderecoClientePedidoModel Endereco { get; set; }
    }
}
