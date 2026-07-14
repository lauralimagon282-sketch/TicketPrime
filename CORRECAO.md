# Correção do professor — AV2 Engenharia de Software

**Alunos:** Laura de Lima Gonçalves (06010735), Carlos Eduardo Mendes Quintella (06011992), Caio de Paiva Barragat (06012117), Arthur Martins (06012635), Emanuel de Oliveira (06010524)
**Projeto:** TicketPrime
**Data:** 14/07/2026

---

## Nota Final: 4,9 / 10,0

---

## Avaliação por Item

| # | Item | Pontos | Nota | Situação |
|---|------|:------:|:----:|:--------:|
| 01 | `/docs/requisitos.md` com 10+ histórias + BDD | 1,0 | **0,3** | ❌ |
| 02 | `README.md` executável | 0,7 | **0,3** | ⚠️ |
| 03 | `/db` com script PostgreSQL | 0,7 | **0,0** | ❌ |
| 04 | Endpoints obrigatórios | 1,0 | **0,8** | ⚠️ |
| 05 | Validações e status codes | 1,0 | **0,4** | ❌ |
| 06 | Dapper parametrizado, sem SQL Injection | 1,0 | **1,0** | ✅ |
| 07 | Testes xUnit com Assert | 1,0 | **0,3** | ⚠️ |
| 08 | ADR com prós/contras | 1,0 | **1,0** | ✅ |
| 09 | Riscos, métricas, SLO, Error Budget | 1,2 | **0,6** | ⚠️ |
| 10 | Frontend Blazor + MudBlazor integrado | 1,0 | **0,0** | ❌ |
| 11 | `release_checklist_final.md` | 0,4 | **0,2** | ⚠️ |
| **Total** | | **10,0** | **4,9** | |

---

## Detalhamento

### 🔴 Violações críticas da Stack Obrigatória

O enunciado exige:
- **Banco de dados: PostgreSQL** com Npgsql
- **Frontend: Blazor com MudBlazor**

O projeto entregue utiliza:
- **SQL Server** (`Microsoft.Data.SqlClient`) — banco de dados diferente do exigido
- **HTML estático** (`wwwroot/*.html`) — sem Blazor, sem MudBlazor

Isso impacta diretamente os itens 03, 10 e parcialmente o 04.

---

### ✅ Itens atendidos

**06 — Dapper parametrizado (1,0/1,0):** Todas as consultas usam `@Parametro`. Nenhuma concatenação de SQL. Uso correto do Dapper.

**08 — ADR (1,0/1,0):** `/docs/adrs/001-escolha-do-micro-orm.md` com `## Contexto`, `## Decisão`, `## Consequências` (Prós e Contras). Arquivo adicional `analise_arquitetura.md` com 3 padrões arquiteturais + trade-offs e 5 violações arquiteturais mapeadas com evidências e ações.

---

### ⚠️ Parcialmente atendido

**02 — README (0,3/0,7):** Lista os 5 integrantes com matrículas. Tem comando `dotnet run`. Mas não explica como configurar banco (SQL Server), não lista URLs da aplicação, não tem comandos detalhados para cada etapa.

**04 — Endpoints (0,8/1,0):** 5 dos 6 endpoints obrigatórios implementados:
- `POST /api/usuarios` ✅, `POST /api/eventos` ✅, `GET /api/eventos` ✅, `POST /api/cupons` ✅, `POST /api/reservas` ✅
- `GET /api/reservas/{cpf}` → implementado como `/api/reservas/usuario/{cpf}` — rota divergente
- `POST /api/reservas` definido **duas vezes** (linhas 95 e 108), a segunda sobrescreve a primeira

**07 — Testes (0,3/1,0):** Arquivo `tests/UsuariosTest.cs` existe com `[Fact]` e `Assert` no formato AAA. Porém:
- Não há projeto `.csproj` de testes — o arquivo não compila sozinho
- Testes apenas validam construção de objetos (Usuário, Cupom, Evento), não cobrem **nenhuma regra de negócio** (CPF duplicado, limite de reservas, capacidade, cupom)
- Classes testadas (`Usuario`, `Cupom`, `Evento`) são `record` definidos no `Program.cs` — os testes referenciam tipos que não existem em um assembly separado

**09 — Operação (0,6/1,2):** O conteúdo de SRE/operação está distribuído em 3 arquivos:
- `observabilidade_logs.md`: ✅ SLO (99%), Error Budget Policy com Feature Freeze, structured logging, OpenTelemetry, distributed tracing
- `fluxo_manutencao.md`: ✅ 12 tickets de manutenção classificados, thresholds de SRE
- `registro_divida_tecnica.md`: ✅ 6 dívidas técnicas com priorização (ID, Descrição, Freq, Risco, Esforço, Decisão)
- Porém: **não existe o arquivo `operacao.md`** (nome obrigatório). Falta a matriz de riscos com as 5 colunas exigidas (Risco, Probabilidade, Impacto, Gatilho, Ação). Falta métrica com Fórmula, Fonte de Dados, Frequência e Ação se Violado.

**11 — Release Checklist (0,2/0,4):** Arquivo presente com checkboxes `[x]`. Mas os itens do checklist não correspondem aos entregáveis reais do projeto — referenciam conceitos abstratos em vez de verificar artefatos concretos (ex.: "script SQL entregue", "testes criados", etc.).

---

### ❌ Itens ausentes ou insuficientes

**01 — Requisitos (0,3/1,0):** Apenas **3 histórias** de usuário (exigido: mínimo 10). Apenas **1 critério de aceitação** BDD (CPF duplicado). O arquivo tem só 7 linhas de conteúdo — completamente insuficiente para os 1,0 ponto.

**03 — Script SQL (0,0/0,7):** O script usa sintaxe **SQL Server** (`USE`, `GO`, `OBJECT_ID`, `IDENTITY`, `VARCHAR(MAX)`) em vez de **PostgreSQL** (`BIGSERIAL`, sem `GO`, sem `USE`). O banco de dados está errado — o script não funciona no PostgreSQL exigido.

**05 — Validações (0,4/1,0):** Implementa apenas 2 das 4 regras de negócio:
- ✅ R1: Usuário e evento devem existir
- ❌ R2: Limite de 2 reservas por CPF no mesmo evento — **não implementado**
- ❌ R3: Bloqueio por capacidade do evento — **não implementado**
- ✅ R4: Cupom exige valor mínimo

**10 — Frontend (0,0/1,0):** Frontend em **HTML estático** (`index.html`, `admin.html`, `cliente.html`, `vendedor.html`). Zero Blazor, zero MudBlazor. A stack obrigatória exige Blazor WebAssembly + MudBlazor.

---

## Pontos Fortes

- **Documentação de engenharia de software extensa e de qualidade**: ADR com trade-offs, análise arquitetural com 3 cenários + 5 violações, observabilidade completa (SLO, Error Budget, OpenTelemetry, distributed tracing), 12 tickets de manutenção classificados, 6 dívidas técnicas priorizadas, threat model, DevSecOps gates, topologia de times
- Dapper 100% parametrizado, sem risco de SQL Injection
- `INNER JOIN` corretamente utilizado na consulta de reservas
- `release_checklist_final.md` presente
- 5 integrantes identificados com matrículas

## Pontos a Melhorar

1. **Migrar para PostgreSQL + Npgsql** — o uso de SQL Server viola a stack obrigatória
2. **Substituir HTML estático por Blazor + MudBlazor** — violação crítica da stack
3. **Implementar regras R2 e R3** (limite de 2 reservas por CPF, bloqueio por capacidade)
4. **Expandir requisitos.md** para mínimo de 10 US com BDD
5. **Criar projeto xUnit real** com `.csproj`, testando regras de negócio (não só construtores)
6. **Criar `docs/operacao.md`** com matriz de riscos (5 colunas) e métrica operacional
7. **Corrigir rota** `GET /api/reservas/{cpf}` e remover endpoint duplicado
8. **Melhorar README** com instruções completas de setup e URLs
