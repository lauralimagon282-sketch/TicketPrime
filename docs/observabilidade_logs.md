# Plano de Observabilidade, Telemetria e Logs (Requisitos de Operação)

Este documento estabelece os padrões de observabilidade, estrutura de logs e políticas de confiabilidade (SRE) aplicadas ao ecossistema TicketPrime.

---

## Parte 1: Padrão de Log Estruturado (Structured Logging)

Para garantir que os logs possam ser indexados, pesquisados e agregados por ferramentas de APM (como ElasticStack, Grafana Loki ou Datadog), a API adota o formato padronizado **JSON** em todas as saídas de console.

### Esquema Padrão do Objeto JSON de Log
Todo log gerado pela aplicação deve conter obrigatoriamente os seguintes campos estruturados:
* `Timestamp`: Data e hora exata do evento no formato ISO 8601 (UTC).
* `LogLevel`: Severidade do log (`INFO`, `WARNING`, `ERROR`, `CRITICAL`).
* `TraceId`: Identificador único da requisição para rastreamento distribuído.
* `Message`: Texto descritivo legível por humanos sobre o evento.
* `Component`: O nome do módulo ou rota que gerou o log.
* `Exception`: Detalhes técnicos e Stack Trace (presente apenas se `LogLevel` for `ERROR` ou `CRITICAL`).

### Exemplo de Log de Sucesso (JSON)
json
{
  "Timestamp": "2026-07-01T10:15:30.123Z",
  "LogLevel": "INFO",
  "TraceId": "4bf92f3577b34da6a3ce929d0e0e4736",
  "Message": "Reserva realizada com sucesso para o evento ID 12",
  "Component": "POST /api/reservas",
  "Exception": null
}
```

### Exemplo de Log de Erro de Infraestrutura (JSON)
json
{
  "Timestamp": "2026-07-01T10:17:05.456Z",
  "LogLevel": "ERROR",
  "TraceId": "9af12e4588c44db2b3ce929d0e0e9812",
  "Message": "Falha de conexão com a instância do SQL Server ao tentar processar checkout.",
  "Component": "POST /api/reservas",
  "Exception": "Microsoft.Data.SqlClient.SqlException: A network-related or instance-specific error occurred... at Microsoft.Data.SqlClient.SqlConnection.OnError..."
}

---

## Parte 2: Rastreamento Distribuído (Distributed Tracing)

Para correlacionar eventos que cruzam as fronteiras do cliente (Front-end) e do servidor (Minimal API), a aplicação implementa o padrão de **Trace ID baseado no cabeçalho HTTP W3C**.

1. **Geração do Identificador:** Toda nova requisição que entra no ecossistema sem um identificador prévio recebe um `TraceId` exclusivo gerado na camada de entrada (Middleware).
2. **Propagação de Contexto:** Esse identificador é injetado no contexto de execução do .NET (`Activity.Current.TraceId`) e incluído automaticamente em todos os logs gerados durante o ciclo de vida daquela requisição específica.
3. **Resposta HTTP:** O `TraceId` é devolvido no cabeçalho de resposta HTTP (`X-Trace-Id`) para o cliente. Se um usuário relatar uma falha, o time de suporte pode isolar toda a jornada daquela transação usando esse código no painel de observabilidade.

---

## Parte 3: Telemetria e Métricas de Negócio (OpenTelemetry)

A aplicação expõe métricas operacionais e de negócio através de um endpoint compatível com o Prometheus, utilizando os três tipos principais de instrumentos do OpenTelemetry:

1. **Counter (Contador Acumulativo):**
   * **Nome da Métrica:** `ticketprime_reservas_criadas_total`
   * **Descrição:** Conta a quantidade total de reservas efetuadas desde a inicialização do serviço.
   * **Tags/Labels:** `status` (success, failed), `evento_id`.
2. **Gauge (Medidor Instantâneo):**
   * **Nome da Métrica:** `ticketprime_usuarios_ativos_gauge`
   * **Descrição:** Monitora o volume atual de conexões simultâneas ativas consumindo a API.
3. **Histogram (Distribuição Estatística):**
   * **Nome da Métrica:** `ticketprime_http_request_duration_seconds`
   * **Descrição:** Mede a latência e tempo de resposta das requisições HTTP para mapear os percentis P95 e P99.

---

## Parte 4: Alinhamento com Negócio e Gestão de Erros (SRE)

Para equilibrar a velocidade de entrega de código da equipe com a estabilidade operacional exigida pelos clientes, adotamos formalmente as seguintes premissas de Confiabilidade:

### Acordo de Nível de Serviço (SLA) e Orçamento de Erro (Error Budget)
* **SLO Operacional:** Estabelecemos que **99%** de todas as requisições HTTP da API de reservas nos últimos 30 dias devem responder com sucesso (Status Code inferior a 500).
* **Orçamento de Erro:** Isso confere à equipe um **Orçamento de Erro (Error Budget) de 1%**. Este orçamento representa a margem de falha tolerável para o negócio para que possamos correr riscos inovando ou atualizando a infraestrutura.

### Política de Esgotamento do Orçamento de Erro (Feature Freeze)
Caso o monitoramento aponte que o **Orçamento de Erro** acumulado do mês se esgotou (ou seja, a taxa de erros HTTP 5XX ultrapassou o limite acumulado de 1% devido a instabilidades ou bugs em produção), a seguinte política rígida entra em vigor imediatamente:

* **Ativação do Feature Freeze:** O time de engenharia entra em estado de bloqueio de funcionalidades.
* **Zero novas funcionalidades:** Fica estritamente proibido realizar o deploy de qualquer nova feature, tela ou endpoint no ambiente de produção.
* **Redirecionamento de Esforço:** 100% da capacidade técnica do time de desenvolvimento é desviada exclusivamente para atividades de estabilização, correção de bugs estruturais, otimização de queries no Dapper e melhorias de infraestrutura.
* **Duração do Bloqueio:** O estado de *Feature Freeze* só será revogado e as entregas de negócio retomadas quando os indicadores de confiabilidade retornarem aos limites seguros definidos no SLO.
