using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechChallengePedidos.Model.Model.Pedidos;

namespace OrquestradorPedidoFunction
{
    public static class ValidarPedido
    {
        public static StringBuilder Mensagem { get; set; } = new StringBuilder();

        public static bool PedidoValido(PedidoModel pedido)
        {
            bool valido = true;
            Mensagem.Clear();

            if (string.IsNullOrEmpty(pedido.Cliente.CPF) || !ValidarCPF(pedido.Cliente.CPF))
            {
                valido = false;
                Mensagem.AppendLine("CPF inválido.");
            }


            if (string.IsNullOrEmpty(pedido.Cliente.ContatoPrimario) || !ValidarContato(pedido.Cliente.ContatoPrimario))
            {

                valido = false;
                Mensagem.AppendLine("Contato Primário Inválido.");
            }

            if (!string.IsNullOrEmpty(pedido.Cliente.ContatoSecundario) && !ValidarContato(pedido.Cliente.ContatoSecundario.Trim()))
            {

                valido = false;
                Mensagem.AppendLine("Contato Secundário Inválido.");
            }

            if (string.IsNullOrEmpty(pedido.Cliente.Endereco.CEP) && !ValidarCEP(pedido.Cliente.Endereco.CEP.Trim()))
            {

                valido = false;
                Mensagem.AppendLine("CEP Inválido.");
            }

            if (string.IsNullOrEmpty(pedido.Cliente.Email) && !ValidarEmail(pedido.Cliente.Email.Trim()))
            {

                valido = false;
                Mensagem.AppendLine("E-mail Inválido.");
            }


            return valido;
        }

        private static bool ValidarCPF(string cpf)
        {
            // Remover caracteres não numéricos do CPF
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            // Verificar se o CPF tem 11 dígitos
            if (cpf.Length != 11)
            {
                return false;
            }

            // Verificar se todos os dígitos do CPF são iguais
            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }

            // Calcular o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            int resto = soma % 11;
            int primeiroDigito = resto < 2 ? 0 : 11 - resto;

            // Verificar se o primeiro dígito verificador está correto
            if (int.Parse(cpf[9].ToString()) != primeiroDigito)
            {
                return false;
            }

            // Calcular o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            resto = soma % 11;
            int segundoDigito = resto < 2 ? 0 : 11 - resto;

            // Verificar se o segundo dígito verificador está correto
            if (int.Parse(cpf[10].ToString()) != segundoDigito)
            {
                return false;
            }

            return true;
        }

        private static bool ValidarContato(string contato)
        {

            // Expressão regular para validar telefone no formato (xx)xxxxx-xxxx
            string regexPattern = @"^\(\d{2}\)\d{5}-\d{4}$";

            // Verificar se o telefone corresponde ao padrão da expressão regular
            if (Regex.IsMatch(contato, regexPattern))
            {
                return true;
            }

            // Se não corresponder, tentar validar como celular no formato (xx)9xxxx-xxxx
            regexPattern = @"^\(\d{2}\)9\d{4}-\d{4}$";

            // Verificar se o telefone corresponde ao padrão da expressão regular
            return (Regex.IsMatch(contato, regexPattern));
          

        }

        private static bool ValidarEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, pattern);
        }


        private static bool ValidarCEP(string cep)
        {
            // Expressão regular para validar CEP no formato xxxxx-xxx
            string regexPattern = @"^\d{5}-\d{3}$";

            // Verificar se o CEP corresponde ao padrão da expressão regular
            return Regex.IsMatch(cep, regexPattern);
        }
    }
}
