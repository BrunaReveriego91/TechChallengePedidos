using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TechChallengePedidos.Model.Model.EstoqueProdutos
{
    public class EstoqueModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("estoque")]
        public List<EstoqueProdutoModel> Estoque { get; set; }
    }
}
