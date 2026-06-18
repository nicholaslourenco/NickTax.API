# 📊 NickTax — Sistema de Gestão Fiscal e Automação de NF-e `[Em Desenvolvimento]`

O **NickTax** é uma Web API robusta desenvolvida para centralizar, gerenciar e automatizar processos fiscais de empresas, com foco no cálculo inteligente de impostos e taxas. O sistema foi projetado seguindo padrões arquiteturais modernos para garantir alta performance, segurança e escalabilidade.

---

## 🚀 Status do Projeto

> ⚠️ **Nota:** O projeto encontra-se atualmente em desenvolvimento ativo. A fundação de infraestrutura, arquitetura, segurança e testes de integração já está 100% concluída. As regras de negócio para gestão de empresas e a automação de leitura de notas fiscais (NF-e) estão sendo implementadas.

---

## 🛠️ Tecnologias e Práticas Utilizadas

### **Backend & Infraestrutura**
* **Ambiente:** .NET 10 & C#
* **Arquitetura:** Clean Architecture (Domain-Driven Design principles)
* **Banco de Dados:** PostgreSQL (Ambiente de Testes/Desenvolvimento) com suporte a Migrations via Entity Framework Core
* **Segurança:** Autenticação e Autorização baseadas em **JWT (JSON Web Tokens)**
* **Logs Estruturados:** Serilog para monitoramento e rastreabilidade

### **Qualidade de Código & Testes**
* **Testes de Integração:** Cobertura automatizada utilizando **xUnit** e `WebApplicationFactory` para simular o ciclo de vida completo da API em memória.

### **Documentação da API**
* **OpenAPI / Scalar:** Substituindo o Swagger tradicional, utilizamos o **Scalar** nativo do .NET 10 para uma interface de documentação moderna e interativa. A configuração é customizada via `IOpenApiDocumentTransformer` para injetar esquemas de segurança e travar rotas protegidas no painel de testes.

---

## 🛡️ Funcionalidades Concluídas (Core & Infra)

* **Autenticação JWT:** Fluxo completo de login e geração de tokens seguros.
* **Recuperação de Senha Segura (OTP):** Sistema de recuperação de conta via código numérico de 6 dígitos com validação estrita de tempo de expiração.
* **Tratamento Global de Erros:** Middleware centralizado para captura de exceções, garantindo respostas padronizadas e segurança de dados em produção.
* **Pipeline de Testes Automatizados:** Suíte de testes de integração que valida fluxos de ponta a ponta (banco de dados, rotas e autenticação) em menos de 5 segundos.

---

## 🗺️ Próximos Passos (Roadmap)

- [ ] **Módulo de Empresas:** Cadastro e gerenciamento (CRUD) de organizações e seus perfis fiscais.
- [ ] **Módulo de Notas Fiscais:** Registro de movimentações financeiras.
- [ ] **Integração com NF-e (Diferencial Core):** Engine de leitura e processamento automático de arquivos XML de Notas Fiscais Eletrônicas para automação do cálculo de impostos.

---

## 🔧 Como Executar o Projeto (Ambiente de Desenvolvimento)

### **Pré-requisitos**
* SDK do .NET 10 instalado.

### **Passo a Passo**

1. Clone o repositório

2. Restaure as dependências e compile a solução a partir da raiz:
   ```bash
   dotnet build

3. Execute a suíte de testes de integração para garantir que o ambiente está íntegro:
   ```bash
   dotnet test

4. Inicie a API:
   ```bash
   dotnet run --project NickTax.API
