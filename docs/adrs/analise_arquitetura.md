# Análise de Arquitetura

## Parte 1: Análise de Padrões Arquiteturais

### Cenário 1: Sistema de Alta Disponibilidade e Escalabilidade Global de Ingressos
* **Padrão Arquitetural Provável:** Arquitetura de Microsserviços (Microservices Architecture).
* **Trade-off:**
  * **Positivo:** Permite isolar e escalar de forma independente o serviço de checkout de reservas durante picos de acessos de grandes eventos.
  * **Negativo:** Aumenta exponencialmente a complexidade de rede, a latência entre comunicações e exige consistência eventual de dados.

### Cenário 2: Processamento Assíncrono de Notificações e Envio de Cupons em Massa
* **Padrão Arquitetural Provável:** Arquitetura Baseada em Eventos (Event-Driven Architecture).
* **Trade-off:**
  * **Positivo:** Total desacoplamento entre o disparador do evento (Criação de Cupom) e os consumidores (Envio de Emails), garantindo alta resiliência à aplicação.
  * **Negativo:** Dificulta a rastreabilidade do fluxo completo (debugging) e exige gerenciamento complexo de filas e tratamento de falhas (Dead Letter Queues).

### Cenário 3: Painel Administrativo Interno de Baixa Complexidade e Poucos Usuários
* **Padrão Arquitetural Provável:** Arquitetura Monolítica em Camadas (Layered/Monolith Architecture).
* **Trade-off:**
  * **Positivo:** Baixa complexidade de desenvolvimento, deploy simplificado em um único servidor e facilidade para realizar consultas diretas e relatórios rápidos.
  * **Negativo:** Baixa flexibilidade tecnológica e alto risco de acoplamento de código à medida que o sistema cresce.

---

## Parte 2: Análise de Violações Arquiteturais

### Violação 1
* **Problema:** Credenciais e String de Conexão Expostas no Código (Hardcoded Connection String).
* **Evidência:** Declaração explícita de texto literal contendo o endereço e autenticação do banco de dados diretamente no arquivo `Program.cs`.
* **Impacto:** Risco crítico de segurança com vazamento de dados confidenciais no repositório de controle de versão (Git) e extrema rigidez para trocar de ambiente de execução.
* **Ação Recomendada:** Mover a string de conexão para um arquivo de configuração isolado (`appsettings.json`) e consumi-la via padrão de injeção `builder.Configuration`.

### Violação 2
* **Problema:** Acoplamento Direto e Falta de Camadas de Negócio.
* **Evidência:** Inserção de regras de validação complexas, tratamento de cupons e cálculos matemáticos executados diretamente dentro do manipulador de rota HTTP da Minimal API.
* **Impacto:** Código centralizado e inflado em um único arquivo, impedindo o reaproveitamento de regras de negócio em outras partes do sistema e inviabilizando testes unitários puramente isolados.
* **Ação Recomendada:** Isolar as regras de negócio em classes de serviço dedicadas (Application/Domain Services), deixando os endpoints focados apenas em receber e responder requisições HTTP.

### Violação 3
* **Problema:** Violação do Princípio da Inversão de Dependência (DIP).
* **Evidência:** Instanciação direta da classe concreta `new SqlConnection(connectionString)` em cada método de rota para interagir com o Dapper.
* **Impacto:** Dependência rígida de um provedor específico de banco de dados (SQL Server), impedindo a substituição por componentes simulados (mocks) durante a execução de testes automatizados.
* **Ação Recomendada:** Registrar a fábrica de conexões ou uma abstração no container de Injeção de Dependência nativo do .NET Core e injetá-la nas rotas.

### Violação 4
* **Problema:** Exposição Direta do Modelo de Persistência na Camada de Apresentação.
* **Evidência:** Uso das mesmas estruturas de dados (Records/Classes) que representam fielmente as colunas do banco de dados como parâmetros de entrada nas rotas expostas aos clientes.
* **Impacto:** Qualquer alteração na estrutura física de tabelas do banco de dados quebra imediatamente os contratos de integração com o front-end, gerando forte acoplamento externo.
* **Ação Recomendada:** Criar classes ou records específicos de transferência de dados (DTOs - Data Transfer Objects) para desacoplar as requisições externas do modelo interno do banco.

### Violação 5
* **Problema:** Ausência de Tratamento Global de Exceções.
* **Evidência:** Inexistência de blocos estruturados de captura de erros (`try-catch`) ou middlewares configurados para gerenciar falhas de persistência ou rede.
* **Impacto:** Falhas internas ou indisponibilidade de infraestrutura geram vazamento de stack traces e detalhes técnicos do banco diretamente nas respostas HTTP para o usuário, comprometendo a segurança da API.
* **Ação Recomendada:** Configurar um middleware global de tratamento de exceções (`UseExceptionHandler`) para interceptar erros inesperados e retornar mensagens amigáveis e padronizadas.
