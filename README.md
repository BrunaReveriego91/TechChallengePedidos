# 🚀 Tech Challenge Fase 2 - Fiap Pos Tech

Projeto de conclusão da Fase 2 da Pós Tech Fiap Curso de Arquitetura de Sistemas .NET com Azure


## 🛠️ Construído com

* [.NET CORE 7 ](https://learn.microsoft.com/pt-br/dotnet/core/whats-new/dotnet-7) - O framework web usado
* [MongoDB ](https://www.mongodb.com/docs/atlas/) - Banco de dados NoSQL 
* [Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/) - Azure Functions
* [Azure Blob Storage](https://learn.microsoft.com/en-us/azure/storage/blobs/) - Azure Blob Storage

## ✒️ Autores

* **Andre Toledo Gama** - [Dev](https://github.com/AndreTGama)
* **Bruna Reveriego** - [Dev](https://github.com/BrunaReveriego91)
* **Rodrigo Reis** - [Dev](https://github.com/RodrigoReiis)
* **Regiane Ruivo** - [Dev](https://github.com/regianeruivo)
* **Fernando Parissenti** - [Dev](https://github.com/parissenti)
![img-tech-challenge](https://github.com/BrunaReveriego91/TechChallengePedidos/assets/94184681/ba0f0cd3-b46f-426e-af0f-bd13f2bae2b2)

Exemplo de request:

Endpoint: http://localhost:7290/api/ReceberPedidoFunction

Request: 

```json
{
  "cliente": {
    "nome": "Fulano de Tal",
    "email": "teste@gmail.com",
    "cpf": "184.623.410-78",
    "contatoPrimario": "(11)99999-9999",
    "contatoSecundario": null,
    "nascimento": "1989-10-01T00:00:00Z",
    "endereco": {
      "rua": "Rua ABC",
      "numero": "123",
      "bairro": "Centro",
      "cidade": "São Paulo",
      "estado": "SP",
      "cep": "12345-678",
      "pais": "Brasil"
    }
  },
  "produtos": [
    {
      "id": 1,
      "quantidade": 999
    }
  ]
}
```

Json MongoDB:

Collection estoque:
```json
{
  "estoque": [
    {
      "produto_id": 1,
      "qtde": 212
    },
    {
      "produto_id": 2,
      "qtde": 226
    },
    {
      "produto_id": 3,
      "qtde": 230
    }
  ]
}
```

Collection produtos:
```json
{
  "produtos": [
    {
      "produto_id": 1,
      "descricao": "TV Smart 40 polegadas",
      "valor": 1289.01
    },
    {
      "produto_id": 2,
      "descricao": "Refrigerador 260L 2 Portas Classe A 220 Volts, Branco",
      "valor": 2140
    },
    {
      "produto_id": 3,
      "descricao": "Fogao 4 bocas",
      "valor": 600
    }
  ]
}
```


  

