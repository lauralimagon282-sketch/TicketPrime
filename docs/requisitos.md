# Requisitos do Sistema TicketPrime

## Histórias de Usuário 
1. Como **Organizador**, Quero **cadastrar um novo evento**, Para **disponibilizar ingressos para venda**.
2. Como **Administrador**, Quero **cadastrar cupons de desconto**, Para **promover eventos específicos**.
3. Como **Cliente**, Quero **realizar meu cadastro**, Para **poder comprar ingressos**.

## Critérios de Aceitação 
**Cenário: Cadastro de Usuário com CPF Duplicado**
- **Dado que** o CPF "123.456.789-00" já está cadastrado no sistema;
- **Quando** eu tentar cadastrar um novo usuário com este mesmo CPF;
- **Então** a API deve retornar o Status Code 400 (Bad Request).
