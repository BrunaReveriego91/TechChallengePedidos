﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechChallengePedidos.Model.Model.EstoqueProdutos;

namespace TechChallengePedidos.Model.Model.Produtos
{
    public class CadastroProdutoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("produtos")]
        public List<ProdutoModel> Produtos { get; set; }
    }
}
