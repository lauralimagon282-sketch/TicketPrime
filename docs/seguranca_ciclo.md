# Modelo de Ameaças e Ciclo de Segurança

Este documento descreve o mapeamento de riscos de segurança e os controles estabelecidos para o ciclo de desenvolvimento do ecossistema TicketPrime.

---

## Parte 1: Threat Model (Modelo de Ameaças)

**Rota Avaliada:** `POST /api/reservas` (Rota de maior risco devido ao processamento de transações de ingressos, aplicação de cupons financeiros e validação de acessos).

### Mapeamento Técnico de Riscos:
* **Ativos Protegidos:** Banco de dados SQL Server (tabela de Reservas e Cupons), integridade financeira do faturamento dos ingressos e dados cadastrais de CPF dos usuários.
* **Vetor de Ataque Provável:** Manipulação de parâmetros na requisição HTTP (Injeção de payloads nocivos ou tentativa de aplicar cupons sem atingir o valor mínimo através de ferramentas de interceptação de requisições como Postman/Burp Suite).
* **Falha Arquitetural Potencial:** Acoplamento excessivo de validações complexas e regras de cupons diretamente nas rotas HTTP sem uma camada isolada de higienização de dados, aumentando a superfície de exposição a fraudes ou travamentos de concorrência (Race Conditions).
* **Controle de Engenharia (Mitigação):** Utilização estrita de consultas parametrizadas com Dapper (`@Parametro`) anulando qualquer risco de SQL Injection, isolamento da string de conexão fora do código fonte via variáveis de ambiente/configuração e validação lógica tripla executada inteiramente em memória no backend C# antes de qualquer persistência.

---

## Parte 2: Gates de Segurança do Ciclo de Desenvolvimento (DevSecOps)

Para garantir a conformidade com as práticas do SSDF (Secure Software Development Framework), a equipe adota obrigatoriamente os três gates de segurança numerados abaixo antes de qualquer liberação de código para produção:

### Gate 1: Análise Estática de Segurança (SAST)
* **Descrição:** Execução automatizada de ferramentas de varredura de código fonte no pipeline de Integração Contínua (CI).
* **Critério de Aceite:** O build será bloqueado e falhará imediatamente se for detectada qualquer credencial, senha ou string de conexão literal (*hardcoded*) exposta em arquivos `.cs`, ou vulnerabilidades classificadas como de severidade "Alta" ou "Crítica".

### Gate 2: Verificação de Dependências e Vulnerabilidades (SCA)
* **Descrição:** Análise automática dos pacotes NuGet importados no arquivo `TicketPrime.csproj` (como Dapper e Microsoft.Data.SqlClient).
* **Critério de Aceite:** Fica proibida a aprovação de Pull Requests caso alguma biblioteca de terceiros possua vulnerabilidades conhecidas de segurança (CVEs) sem correção. O pipeline exige a atualização preventiva para versões estáveis e seguras.

### Gate 3: Revisão de Arquitetura e Higienização de Queries
* **Descrição:** Revisão por pares (*Peer Review*) obrigatória focada na camada de persistência de dados do Dapper.
* **Critério de Aceite:** Validação manual e visual de que nenhuma query SQL utiliza concatenação ou interpolação de strings. 100% das consultas que interagem com parâmetros externos devem usar mapeamento de objetos nativos do Dapper para mitigar riscos de injeção de comandos.
