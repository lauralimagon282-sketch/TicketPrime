# Correção do professor — AV2 Engenharia de Software

**Alunos:** Laura de Lima Gonçalves (06010735), Carlos Eduardo Mendes Quintella (06011992), Caio de Paiva Barragat (06012117), Arthur Martins (06012635), Emanuel de Oliveira (06010524)
**Projeto:** TicketPrime
**Data:** 14/07/2026

---

## Nota Final: 4,75 / 10,0

---

## Tabela de Avaliação (20 itens × 0,5 pontos)

| # | Item | Critério | Nota |
|---|------|----------|:----:|
| 01 | Padrão AAA nos Testes | `// Arrange`, `// Act`, `// Assert` em ≥ 3 métodos | **0,5** ✅ |
| 02 | Nomenclatura e Independência | `Metodo_Cenario_ResultadoEsperado`, sem `if`/`switch`/`for`/`while` | **0,5** ✅ |
| 03 | Análise de Padrões Arquiteturais | 3 cenários com trade-offs (Positivo/Negativo) | **0,5** ✅ |
| 04 | Análise de Violações Arquiteturais | 5 violações com Problema/Evidência/Impacto/Ação | **0,5** ✅ |
| 05 | ADR | Contexto/Decisão/Consequências, Status, Prós/Contras | **0,5** ✅ |
| 06 | Registro de Dívida Técnica | Tabela com 6+ dívidas, colunas padronizadas | **0,5** ✅ |
| 07 | Priorização de Dívida Técnica | Prioridade 1, 2 e 3, pelo menos uma de cada | **0,5** ✅ |
| 08 | Classificação de Manutenção | 12 tickets com taxonomia Swanson (Corretiva/Adaptativa/Perfectiva/Preventiva) | **0,0** ❌ |
| 09 | Pipeline de Liberação Segura | 4 passos documentados | **0,0** ❌ |
| 10 | Plano de Iteração | `/docs/plano_iteracao.md` com 5 campos obrigatórios | **0,0** ❌ |
| 11 | Quadro Visual e Limite de WIP | 4+ colunas, WIP ≤ integrantes | **0,0** ❌ |
| 12 | Matriz de Riscos | 5+ riscos, colunas padronizadas, Estratégia (Mitigar/Transferir/Aceitar/Evitar) | **0,0** ❌ |
| 13 | Gatilhos de Risco | Coluna Gatilho com ≥ 20 caracteres por linha | **0,0** ❌ |
| 14 | Métrica de Fluxo (DORA) | Ficha com 7 campos obrigatórios | **0,0** ❌ |
| 15 | Métrica de Qualidade | Ficha com 7 campos obrigatórios | **0,0** ❌ |
| 16 | SLO | SLI, Fórmula, Fonte, Janela, Alvo (%) | **0,0** ❌ |
| 17 | Error Budget Policy | 3 níveis graduados, Nível 3 com congelamento/Feature Freeze | **0,0** ❌ |
| 18 | Segurança no Código (SSDF) | Nenhuma credencial hardcoded em `.cs` | **0,5** ✅ |
| 19 | Threat Model e Gates | Ativos/Vetor/Falha/Controle + 3 Gates numerados | **0,5** ✅ |
| 20 | Topologia de Times + DoD | 4 tipos Team Topologies + release_checklist com 7 categorias | **0,25** ⚠️ |

> **Soma:** 3,5 (01-07) + 1,0 (18-19) + 0,25 (20) = **4,75 / 10,0**

---

## Detalhamento

### ✅ Itens corretos (01–07, 18–19)

| # | Arquivo | Avaliação |
|---|---------|-----------|
| 01 | `tests/UsuariosTest.cs` | 3 métodos com `// Arrange`, `// Act`, `// Assert` ✅ |
| 02 | `tests/UsuariosTest.cs` | Nomes no padrão `Metodo_Cenario_ResultadoEsperado`, zero condicionais (`if`/`switch`/`for`/`while`) ✅ |
| 03 | `docs/adrs/analise_arquitetura.md` | 3 cenários arquiteturais com trade-off documentado como **Positivo:** e **Negativo:** ✅ |
| 04 | `docs/adrs/analise_arquitetura.md` | 5 violações com **Problema:**, **Evidência:**, **Impacto:** e **Ação Recomendada:** ✅ |
| 05 | `docs/adrs/001-escolha-do-micro-orm.md` | `## Contexto`, `## Decisão`, `## Consequências`, `Status: Aceito`, listas `Prós:` e `Contras:` ✅ |
| 06 | `docs/registro_divida_tecnica.md` | 6 dívidas, colunas ID/Freq/Risco/Esforço/Decisão com valores Alto/Médio/Baixo ✅ |
| 07 | `docs/registro_divida_tecnica.md` | DT-01 = `Prioridade 1 (Imediato)`, DT-04 = `Prioridade 3 (Aceitar/Ignorar)`, demais `Prioridade 2` ✅ |
| 18 | `src/**/*.cs` | Nenhum `Password=`, `Pwd=`, `User Id=` ou `ConnectionString=` literal. Usa `builder.Configuration.GetConnectionString()` ✅ |
| 19 | `docs/seguranca_ciclo.md` | Threat Model (Ativos/Vetor/Falha/Controle) + Gate 1 (SAST), Gate 2 (SCA), Gate 3 (Revisão de Queries) ✅ |

---

### ❌ Itens zerados (08–17)

**08 — Classificação de Manutenção (0,0):**
Os 12 tickets em `fluxo_manutencao.md` usam termos **fora da taxonomia de Swanson** exigida:
- `Manutenção Corretiva Imediata` → deveria ser **Corretiva**
- `Manutenção Corretiva Evolutiva` → deveria ser **Perfectiva**
- `Manutenção Adaptativa` → ok, mas misturada com termos inválidos
A taxonomia oficial é: **Corretiva, Adaptativa, Perfectiva, Preventiva**. Os termos "Corretiva Imediata" e "Corretiva Evolutiva" não existem nessa classificação.

**09 — Pipeline de Liberação (0,0):**
`fluxo_manutencao.md` não contém os 4 passos exigidos: (1) Análise de Impacto, (2) Teste como Instrumento Cirúrgico, (3) Feature Toggle, (4) Estratégia de Release e Regressão.

**10 — Plano de Iteração (0,0):**
Arquivo `/docs/plano_iteracao.md` **não existe**. Deveria conter: `Objetivo da Iteração:`, `Escopo (Backlog Selecionado):`, `Entregáveis (Evidências):`, `Risco Principal do Ciclo:`, `Definição de Pronto (DoD):`.

**11 — Quadro Visual e WIP (0,0):**
Depende do `plano_iteracao.md` (inexistente). Deveria ter 4+ colunas nomeadas e WIP numérico ≤ 5 integrantes.

**12 — Matriz de Riscos (0,0):**
Arquivo `/docs/operacao.md` **não existe**. Deveria conter 5+ riscos com Risco/Probabilidade/Impacto/Estratégia/Ação Planejada. Estratégia deve usar: Mitigar/Transferir/Aceitar/Evitar.

**13 — Gatilhos de Risco (0,0):**
Depende do `operacao.md` (inexistente). Cada linha precisava de gatilho com ≥ 20 caracteres.

**14 — Métrica DORA (0,0):**
Depende do `operacao.md` (inexistente). Ficha com 7 campos: Nome/O que Mede/Fórmula/Fonte/Frequência/Limites de Saúde/Ação se Violado. Deve ser métrica de fluxo (Deploy/Lead Time/Throughput/DORA).

**15 — Métrica de Qualidade (0,0):**
Depende do `operacao.md` (inexistente). Mesmos 7 campos, mas métrica de qualidade (Falha/Erro/Teste/Change Failure Rate/Cobertura).

**16 — SLO (0,0):**
Depende do `operacao.md` (inexistente). Conteúdo relacionado aparece em `observabilidade_logs.md`, mas não no arquivo correto e sem os campos obrigatórios: SLI (Indicador), Fórmula de Coleta, Fonte do Dado, Janela de Medição (com valor + dias/horas), Alvo SLO (%).

**17 — Error Budget Policy (0,0):**
Depende do `operacao.md` (inexistente). Conteúdo de Feature Freeze aparece em `observabilidade_logs.md`, mas sem os 3 níveis graduados (Nível 1, Nível 2, Nível 3) exigidos.

---

### ⚠️ Parcial (20)

**20 — Topologia de Times + DoD (0,25/0,5):**
- `/docs/topologia_times.md`: ✅ Mapeia 4 tipos (Stream-aligned, Platform, Enabling, Complicated-Subsystem) para o contexto do projeto
- `release_checklist_final.md`: ❌ As 7 caixas `[x]` não usam as categorias exigidas. O enunciado pede: **Fundamentos, Produto Mínimo, Evidência de Qualidade, Decisões Documentadas, Evidência de Requisitos, Governança e Segurança**. O arquivo atual usa descrições que não correspondem a essas 7 categorias.

---

## Requisitos de Código (itens implícitos)

| Requisito | Situação |
|-----------|:--------:|
| 2+ endpoints com regras de negócio (não CRUD simples) | ✅ `POST /api/reservas` + `POST /api/login` |
| 1+ endpoint com JOIN | ✅ `GET /api/reservas/usuario/{cpf}` com INNER JOIN em Usuarios e Eventos |
| 1+ endpoint com 3+ validações + 400 Bad Request | ✅ `POST /api/reservas`: usuário existe, evento existe, cupom válido, preço ≥ valor mínimo |
| Dapper 100% parametrizado (`@Parametro`) | ✅ Nenhuma concatenação/interpolação |

---

## Pontos Fortes

- **Itens 01 a 07 impecáveis:** testes AAA com nomenclatura correta, análise arquitetural completa (3 cenários + 5 violações), ADR com trade-offs, 6 dívidas técnicas bem priorizadas
- **Segurança exemplar:** zero credenciais em `.cs`, threat model com 3 gates DevSecOps
- **Código sólido:** endpoints com regras de negócio, JOIN, múltiplas validações com 400, Dapper 100% parametrizado

## Pontos a Melhorar

1. **Corrigir taxonomia** no `fluxo_manutencao.md`: "Corretiva Imediata" → **Corretiva**, "Corretiva Evolutiva" → **Perfectiva**
2. **Adicionar Pipeline de Liberação** com os 4 passos no `fluxo_manutencao.md`
3. **Criar `/docs/plano_iteracao.md`** com os 5 campos + quadro visual com WIP
4. **Criar `/docs/operacao.md`** com: matriz de riscos (5+ riscos com gatilhos ≥20 chars), métrica DORA (7 campos), métrica de qualidade (7 campos), SLO (5 campos) e Error Budget Policy (3 níveis)
5. **Corrigir `release_checklist_final.md`** para usar as 7 categorias: Fundamentos, Produto Mínimo, Evidência de Qualidade, Decisões Documentadas, Evidência de Requisitos, Governança e Segurança
