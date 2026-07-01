# Mapeamento de Topologia de Times

Este documento estabelece a estrutura organizacional das equipes de engenharia do ecossistema TicketPrime, seguindo o framework *Team Topologies* para garantir alta autonomia e alinhamento com o fluxo de valor do negócio.

---

## Mapeamento dos 4 Tipos de Times

Para suportar a arquitetura da Minimal API, a persistência com Dapper e os requisitos operacionais de segurança e SRE, a organização é dividida em quatro categorias estruturais:

### 1. Stream-aligned Team (Time Alinhado ao Fluxo)
* **Atuação no Projeto:** É o time principal de desenvolvimento de produto (Core Features). Responsável direto pela jornada de ponta a ponta do usuário no fluxo de **Reservas, Checkout e aplicação lógica de cupons**.
* **Missão:** Entregar valor contínuo na rota crítica (`POST /api/reservas`), garantindo que as regras de negócio complexas sejam validadas em memória com agilidade e sem gargalos na esteira de deploy.

### 2. Platform Team (Time de Plataforma)
* **Atuação no Projeto:** Fornece a infraestrutura de base e as ferramentas que o time alinhado ao fluxo consome para trabalhar sem fricção. 
* **Missão:** Responsável por abstrair a complexidade dos ambientes, gerenciar as instâncias do SQL Server, estruturar o barramento de Logs em JSON da Parte 1 e expor o endpoint de telemetria e Prometheus (OpenTelemetry) para o monitoramento de SRE.

### 3. Enabling Team (Time Facilitador)
* **Atuação no Projeto:** Um time de especialistas focados em pesquisa, adoção de novas tecnologias e eliminação de impedimentos técnicos ou conceituais.
* **Missão:** Responsável por capacitar as demais equipes na correta utilização do micro-ORM Dapper (evitando queries acopladas), na escrita correta das decisões arquiteturais (ADRs) e na disseminação das práticas de gerenciamento de dívidas técnicas.

### 4. Complicated-subsystem Team (Time de Subsistema Complexo)
* **Atuação no Projeto:** Equipe dedicada exclusivamente a um componente específico do sistema que exige conhecimentos matemáticos, criptográficos ou técnicos extremamente profundos e específicos.
* **Missão:** No ecossistema TicketPrime, este time cuida do **Motor de Criptografia e Integração com Gateways de Pagamento Externos** e da engine de concorrência que impede que duas pessoas comprem o mesmo assento no exato milissegundo. O isolamento desse time evita que os desenvolvedores gerais precisem dominar essa alta complexidade matemática.
