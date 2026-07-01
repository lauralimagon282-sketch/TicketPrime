# Registro de Dívidas Técnicas (Requisitos Itens 06 e 07)

Abaixo estão listadas as dívidas técnicas identificadas no ecossistema TicketPrime, mapeadas com base em sua frequência de alteração, impacto operacional e esforço de mitigação.

| ID da Dívida | Descrição Técnica | Freq. Alteração | Risco | Esforço | Decisão |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **DT-01** | Regras de validação de negócio e manipulação do banco acopladas diretamente nas Minimal APIs dentro de `Program.cs`. | Alto | Alto | Médio | Prioridade 1 (Imediato) |
| **DT-02** | Acoplamento físico com o banco através do uso das entidades de persistência (`records`) atuando como DTOs de entrada nas rotas HTTP. | Médio | Médio | Médio | Prioridade 2 (Próxima Sprint) |
| **DT-03** | Instanciação direta da classe concreta `SqlConnection` nas rotas do Dapper, violando a Inversão de Dependência. | Baixo | Médio | Baixo | Prioridade 2 (Próxima Sprint) |
| **DT-04** | Strings de logs e mensagens de erro espalhadas de forma literal no código, sem um mecanismo de padronização global. | Médio | Baixo | Baixo | Prioridade 3 (Aceitar/Ignorar) |
| **DT-05** | Ausência de uma ferramenta estruturada para controle de Migrations do banco de dados, dependendo de execuções de scripts manuais. | Baixo | Médio | Alto | Prioridade 2 (Próxima Sprint) |
| **DT-06** | Falta de uma suíte de testes de integração ponta a ponta (E2E) simulando requisições HTTP completas contra o banco de dados. | Alto | Médio | Médio | Prioridade 2 (Próxima Sprint) |
