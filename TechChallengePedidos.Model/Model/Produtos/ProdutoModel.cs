using MongoDB.Bson.Serialization.Attributes;

namespace TechChallengePedidos.Model.Model.Produtos
{
    public class ProdutoModel
    {
        [BsonElement("produto_id")]
        public int ProdutoId { get; set; }
        [BsonElement("descricao")]
        public string Descricao { get; set; }
        [BsonElement("valor")]
        public double Valor { get; set; }
    }
}