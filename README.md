# 📊 NickTax — Sistema de Gestão Fiscal e Inteligência de NF-e `[Em Desenvolvimento]`

O **NickTax** é uma Web API robusta desenvolvida para centralizar, gerenciar e automatizar processos fiscais de empresas, com foco no tratamento de notas recebidas e preparação de dados para classificação inteligente. O sistema foi projetado seguindo padrões arquiteturais modernos (Clean Architecture) para garantir isolamento de regras de negócio, segurança e alta escalabilidade.

---

## 🚀 Status do Projeto

> 🛠️ **Fase Atual:** A fundação de infraestrutura, autenticação e o núcleo do domínio estão 100% concluídos. Os fluxos de gerenciamento de **Empresas** (com validação local de certificados digitais e sincronização de dados) está operacional e o ecossistema de **Notas Fiscais** integrado à API da **Focus NFe** já está em fase final de desenvolvimento. O projeto prepara o terreno para o motor de Inteligência Artificial.

---

## 🛠️ Tecnologias e Práticas Utilizadas

### **Backend & Infraestrutura**
* **Ambiente:** .NET 10 & C# (Utilizando padrões modernos como `X509CertificateLoader`)
* **Arquitetura:** Clean Architecture (Domain-Driven Design principles)
* **Banco de Dados:** PostgreSQL estruturado via Migrations com Entity Framework Core
* **Segurança:** Autenticação e Autorização baseadas em **JWT (JSON Web Tokens)** com isolamento de dados por Usuário Logado.
* **Logs Estruturados:** Serilog para monitoramento e rastreabilidade

### **Qualidade de Código & Testes**
* **Testes de Integração:** Cobertura automatizada utilizando **xUnit** e `WebApplicationFactory` para simular o ciclo de vida completo da API em memória.
* **Validações de Entrada:** Camada de DTOs blindada com **FluentValidation** (com regras condicionais para atualizações parciais de certificados).

### **Documentação da API**
* **OpenAPI / Scalar:** Substituindo o Swagger tradicional, utilizamos o **Scalar** nativo do .NET 10 para uma interface de documentação moderna e interativa. A configuração é customizada via `IOpenApiDocumentTransformer` para injetar esquemas de segurança e travar rotas protegidas no painel de testes.

---

## 🛡️ Funcionalidades Concluídas (Core, Infra & Negócio)

* **Autenticação JWT & Isolamento de Escopo:** Fluxo completo de login e proteção de rotas. Cada usuário só consegue visualizar, editar ou excluir as Empresas e Notas Fiscais atreladas estritamente ao seu próprio token.
* **Módulo de Empresas (CRUD):** Cadastro completo de organizações com upload de certificado digital (`.pfx`) e senha.
* **Validação Offline de Certificados:** Engine local (`CertificadoService`) que valida a estrutura criptográfica, prazo de validade (passado e futuro) e extrai o CNPJ interno do certificado usando Expressões Regulares (`Regex`) para bater com o dado digitado.
* **Módulo de Notas Fiscais & Sincronização:** Endpoint de sincronização automatizada que orquestra a busca de lotes na Focus NFe, executa o parser do XML do governo federal (`NfeParserService`) isolando metadados como chave de acesso, emitente, data de emissão e valor total, e armazena os registros no banco evitando duplicidade.
* **Tratamento Global de Erros:** Middleware centralizado para captura de exceções, garantindo respostas padronizadas e segurança de dados em produção.

---

## 🗺️ Próximos Passos (Roadmap)

- [ ] **Módulo de Classificação por IA:** Implementação do motor de inteligência artificial para ler o conteúdo bruto do XML armazenado (`ConteudoXML`), mapear os itens/produtos comprados e categorizar os custos automaticamente.
- [ ] **Dashboard Analítico:** Endpoints agregadores para expor métricas de despesas fiscais baseadas nas notas mineradas.
- [ ] **Integração com Focus NFe (`FocusNfeClient`):** Arquitetura para espelhar dados na API da Focus NFe (com suporte a parâmetros de simulação de ambiente como `dry_run=1`), consumindo o endpoint de notas recebidas (`/v2/nfes_recebidas`) de forma otimizada por NSU.

---

## 🔧 Como Executar o Projeto (Ambiente de Desenvolvimento)

### **Pré-requisitos**
* SDK do .NET 10 instalado.
* PostgreSQL instalado.

### **Passo a Passo**

1. Clone o repositório.

2. Restaure as dependências e compile a solução a partir da raiz:
   ```bash
   dotnet build

3. Execute a suíte de testes de integração para garantir que o ambiente está íntegro:
   ```bash
   dotnet test

4. Inicie a API:
   ```bash
   dotnet run --project NickTax.API
