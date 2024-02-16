# TechChallengePedidos

![TechChallenge2](https://github.com/BrunaReveriego91/TechChallengePedidos/assets/94184681/124f6b60-04a0-496f-afdf-1e96b40cffb4)

   
Exemplo de request:

Endpoint: http://localhost:7290/api/ReceberPedidoFunction

Request: 
{
    "cliente": 
    {   
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
            "valorunitario": 10.5,
            "quantidade": 999
        }
    ]
}
  

