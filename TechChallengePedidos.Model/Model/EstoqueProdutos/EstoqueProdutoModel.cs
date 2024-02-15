using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TechChallengePedidos.Model.Model.EstoqueProdutos
{
    public class EstoqueProdutoModel
    {
        [BsonElement("produto_id")]
        public int ProdutoId { get; set; }
        [BsonElement("qtde")]
        public int Qtde { get; set; }
    }
}
