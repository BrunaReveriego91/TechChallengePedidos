# TechChallengePedidos

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
  

