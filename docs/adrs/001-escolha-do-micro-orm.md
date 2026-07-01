# ADR 001: Escolha do Micro-ORM para Persistência de Dados

* **Status:** Aceito

## Contexto
O sistema TicketPrime precisa processar operações de vendas de ingressos de maneira rápida e segura, exigindo consultas eficientes que correlacionem usuários, eventos e cupons de desconto. Avaliou-se a adoção de um ORM robusto e completo (como Entity Framework Core) contra uma abordagem mais leve de mapeamento direto (como Dapper).

## Decisão
Decidimos utilizar o **Dapper** como o Micro-ORM oficial para a camada de persistência de dados do projeto. A escolha justifica-se pela necessidade de controle absoluto sobre os comandos SQL executados, visando otimização de performance, baixa alocação de memória e simplicidade na implementação de consultas customizadas.

## Consequências

### Prós:
* Performance computacional extremamente alta, aproximando-se do ADO.NET puro, essencial para cenários de alta concorrência em vendas.
* Flexibilidade total para escrever e otimizar queries SQL customizadas contendo junções complexas (como INNER JOINs).
* Curva de aprendizado reduzida e mapeamento direto dos resultados do banco de dados para objetos de negócio C#.

### Contras:
* Exige que a equipe escreva manualmente todas as strings de comandos SQL, aumentando o risco de falhas de sintaxe em tempo de desenvolvimento.
* Ausência de um mecanismo nativo de controle e versionamento de banco de dados (Migrations), demandando a manutenção manual de scripts SQL externos.
