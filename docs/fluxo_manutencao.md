# Fluxo de Manutenção e Gestão de Backlog

## Parte 1: Políticas de Threshold de Alertas e Erros (SRE)
Para garantir a estabilidade do ecossistema TicketPrime, estabelecemos os seguintes limites operacionais (thresholds) baseados em Golden Signals:
* **Taxa de Erro HTTP 5XX:** O limite máximo aceitável é de **1%** das requisições totais em um intervalo de 5 minutos. Caso ultrapasse, um alerta de severidade alta é disparado para o time de plantão.
* **Latência (P95):** O tempo de resposta para o endpoint de checkout de reservas (`POST /api/reservas`) não deve passar de **800ms**. Se a latência P95 atingir **1500ms**, o sistema entra em estado de atenção.

---

## Parte 2: Classificação e Triagem do Backlog de Manutenção

Abaixo estão listadas as 12 atividades e incidentes mapeados para o ciclo de sustentação do TicketPrime, classificados de acordo com a tipologia de manutenção de software:

1. **Ticket #101:** Instabilidade crítica e queda de conexão com o banco SQL Server interrompendo 100% das vendas de ingressos atuais.
   * **Classificação:** `[1] Manutenção Corretiva Imediata`
2. **Ticket #102:** Ajustar o cálculo do checkout de reservas para aplicar uma nova alíquota de imposto municipal sobre serviços exigida por nova legislação fiscal.
   * **Classificação:** `[4] Manutenção Adaptativa`
3. **Ticket #103:** Criação de uma nova rota para exportar o relatório de reservas faturadas em formato CSV para o time de contabilidade.
   * **Classificação:** `[2] Manutenção Corretiva Evolutiva`
4. **Ticket #104:** Atualização preventiva dos pacotes NuGet do Dapper e do Microsoft.Data.SqlClient para corrigir vulnerabilidades conhecidas de segurança de código.
   * **Classificação:** `[3] Manutenção Preventiva`
5. **Ticket #105:** Mensagem visual de erro de CPF inválido quebrando o layout da interface pública do usuário em navegadores mobile mais antigos.
   * **Classificação:** `[2] Manutenção Corretiva Evolutiva`
6. **Ticket #106:** Refatoração do arquivo centralizado `Program.cs` para isolar os endpoints em arquivos de rotas menores, reduzindo a complexidade ciclomática.
   * **Classificação:** `[3] Manutenção Preventiva`
7. **Ticket #107:** Falha na validação de cupons que permite que códigos expirados continuem aplicando desconto indevidamente em compras ativas.
   * **Classificação:** `[1] Manutenção Corretiva Imediata`
8. **Ticket #108:** Migração da infraestrutura da API do servidor local para um ambiente de containers Docker na nuvem para suportar o escalonamento horizontal.
   * **Classificação:** `[4] Manutenção Adaptativa`
9. **Ticket #109:** Implementação de uma camada de cache distribuído em memória para a listagem pública de eventos ativos (`GET /api/eventos`).
   * **Classificação:** `[3] Manutenção Preventiva`
10. **Ticket #110:** Adaptação da formatação de datas e fusos horários da API para suportar a venda de ingressos para eventos internacionais em múltiplos fusos (UTC).
    * **Classificação:** `[4] Manutenção Adaptativa`
11. **Ticket #111:** Inclusão de um novo campo opcional no cadastro do usuário para armazenar e validar o número de telefone/WhatsApp.
    * **Classificação:** `[2] Manutenção Corretiva Evolutiva`
12. **Ticket #112:** Vazamento de stack trace técnico contendo detalhes internos do banco de dados quando uma requisição falha por timeout.
    * **Classificação:** `[1] Manutenção Corretiva Imediata`
