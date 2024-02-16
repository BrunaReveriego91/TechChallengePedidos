# TechChallengePedidos

![TechChallenge2](https://github.com/BrunaReveriego91/TechChallengePedidos/assets/94184681/9044a10d-fbf4-42d7-a631-92ee084834fc)

   
Exemplo de request:

Endpoint: http://localhost:7290/api/ReceberPedidoFunction

Request: 
{
    "cliente": 
    {   
        "nome": "Fulano de Tal",
        "email": "teste@gmail.com",
        "cpf": "426.450.748-04",
        "contatoPrimario": "(11)99999-9999",
        "contatoSecundario": null,
        "nascimento": "1991-10-14T00:00:00Z",
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
            "valorunitario": 10.5,
            "quantidade": 999
        }
    ]
}
  

