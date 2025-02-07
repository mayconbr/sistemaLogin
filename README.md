#Projeto ASP.NET MVC

Este projeto foi desenvolvido com o objetivo de centralizar os sistemas de login de uma empresa em uma única API, mantendo o visual da marca da empresa e simplificando o processo de autenticação para os usuários. Com essa solução, um único usuário pode acessar todos os sistemas da empresa de forma segura e integrada, sem a necessidade de realizar múltiplos logins.

## Tecnologias Utilizadas

- **ASP.NET MVC**: Framework principal utilizado para o desenvolvimento do sistema.
- **HTML**: Linguagem de marcação para estruturação do conteúdo da página.
- **CSS**: Utilizado para estilização e layout das páginas.
- **Razor**: Engine de templates utilizada para gerar HTML dinâmico.
- **Claims-based Authentication**: Método de autenticação utilizado para identificar usuários através de *claims* (informações sobre o usuário), como nome, e-mail e permissões.

## Objetivo do Projeto

O projeto visa permitir que todos os sistemas da empresa possuam uma autenticação centralizada. Ao usar uma única API de login, a experiência do usuário é melhorada, garantindo que ele não precise se autenticar repetidamente para acessar diferentes sistemas. Além disso, o design das páginas de login e autenticação mantém a identidade visual da marca da empresa.

### Benefícios

- **Autenticação Centralizada**: Um único login para todos os sistemas.
- **Facilidade de Integração**: Outros sistemas da empresa podem facilmente integrar com a API de autenticação.
- **Segurança Aprimorada**: Utilização de Claims-based Authentication para garantir sessões seguras e confiáveis.
- **Manutenção Simplificada**: Alterações no processo de login podem ser feitas centralizadamente na API.
- **Consistência Visual**: Todos os sistemas seguem o mesmo padrão visual, garantindo a uniformidade da marca.

## Como Funciona a Autenticação

A autenticação é realizada utilizando **Claims-based Authentication**. Em vez de usar tokens como o JWT, o sistema utiliza *claims* para gerenciar a identidade do usuário durante a autenticação.

### O Processo de Autenticação:

1. **O usuário acessa o sistema** e é redirecionado para a página de login.
2. **As credenciais fornecidas** (nome de usuário e senha) são enviadas para a API de autenticação.
3. **A API valida as credenciais** e, se forem corretas, ela retorna um conjunto de *claims* sobre o usuário, como:
   - Nome completo
   - E-mail
   - Roles ou permissões associadas ao usuário
4. **Os *claims* são armazenados** no sistema de autenticação. Estes *claims* são então usados para identificar o usuário em cada requisição subsequente.
5. **A aplicação utiliza esses *claims*** para controlar o acesso do usuário aos recursos da aplicação, como páginas ou funcionalidades específicas.
6. O usuário é redirecionado para o sistema ou página principal de acordo com suas permissões.

Esses *claims* são armazenados de forma segura no *cookie* de autenticação, que é gerenciado pelo ASP.NET para garantir que o usuário permaneça autenticado durante a sessão.

### Vantagens do Claims-based Authentication:

- **Flexibilidade**: Permite associar informações personalizadas sobre o usuário (ex.: permissões, roles, preferências).
- **Segurança**: As informações são armazenadas no servidor, e não no cliente, o que minimiza o risco de manipulação.
- **Integração**: Sistemas e aplicativos podem facilmente utilizar as informações de autenticação e autorização baseadas em *claims*.

## Estrutura do Projeto

- **Controllers**:
  - `LoginController`: Controlador responsável pelas ações relacionadas ao login de usuários.
  - `AccountController`: Controlador que gerencia a criação e gerenciamento de contas de usuários.
  
- **Views**:
  - `Login.cshtml`: Página de login onde o usuário insere suas credenciais.
  - `Register.cshtml`: Página para registro de novos usuários, caso seja necessário.
  - `Error.cshtml`: Página de erro para exibir mensagens caso o login ou processo de autenticação falhe.
  
- **Modelos**:
  - `UserModel`: Representa os dados de um usuário, incluindo informações de login e autenticação.
  - `LoginModel`: Modelo usado para validar as credenciais de login.

- **Serviços**:
  - `AuthService`: Serviço responsável pela comunicação com a API de autenticação e gerenciamento de *claims* de usuário.

## Como Rodar o Projeto

### Pré-requisitos

- Visual Studio ou qualquer editor de código compatível com ASP.NET MVC.
- .NET SDK (versão 6 ou superior).
- Um banco de dados SQL Server configurado.
- A API de autenticação já configurada e funcionando.
