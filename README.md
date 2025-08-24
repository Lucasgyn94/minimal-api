# 🚀 API de Gerenciamento de Veículos
## 📖 Sobre o Projeto
* Esta API RESTful foi desenvolvida como parte do bootcamp GFT Start #7 .NET. O projeto inicial, uma Minimal API funcional, foi o ponto de partida para uma jornada de aprimoramento e aplicação de boas práticas de arquitetura e desenvolvimento de software, transformando-a em uma solução robusta, segura e escalável.

* O objetivo principal desta evolução foi demonstrar, na prática, a aplicação de conceitos de Clean Architecture, segurança, testabilidade e organização de código no ecossistema .NET.

## ✨ Principais Funcionalidades
* __CRUD completo__ para gerenciamento de veículos.

* __Sistema de Autenticação__ com Tokens JWT (JSON Web Tokens).

* __Sistema de Autorização__ baseado em perfis (Roles), como Adm e Editor.

* __Endpoints documentados__ e prontos para teste com Swagger (OpenAPI).

* __Testes de Integração e de Serviço__.

## 🌱 Evolução: De um Projeto de Bootcamp a uma API mais Robusta
* A seguir, os principais passos da evolução que fiz no projeto:

1. __Fundação com Minimal API__: O projeto nasceu utilizando o padrão Minimal APIs do .NET, focando em simplicidade e performance.

2. __Segurança em Primeiro Lugar__: A primeira e mais crítica melhoria que optei por fazer, foi a implementação de um sistema de hashing de senhas com BCrypt.Net. Com isso as senhas não são mais armazenadas em texto puro, garantindo a segurança dos dados dos usuários.

3. __Modernização da Arquitetura__:

  * O **Program.cs** e **Startup.cs** foram unificados, adotando o modelo de hospedagem moderno e mais limpo do .NET 8.

  * As responsabilidades foram divididas em camadas, seguindo os princípios da __Clean Architecture__, com uma separação clara entre:

    * __Dominio__: O coração do negócio, com as entidades puras.

    * __Application__: A camada de orquestração, contendo os serviços e a lógica dos casos de uso.

    * __Infraestrutura__: Os detalhes de implementação, como o acesso ao banco de dados com EF Core.

4. __Código Organizado e Escalável (Princípios SOLID)__:

* Os endpoints foram extraídos do __Program.cs__ e organizados em classes de extensão (__Endpoint Definitions__), aplicando o Princípio da Responsabilidade Única (SRP).

* A lógica de geração de tokens JWT foi abstraída para um serviço dedicado (__ITokenService__), tornando o código mais limpo e reutilizável.

* Refatoração de Testes: A suíte de testes foi adaptada para a nova arquitetura, sendo implementado como parte do desafio __testes de integração e de serviço__ para classe de __Veículo__.

## 🛠️ Tecnologias e Ferramentas Utilizadas
* __Framework__: .NET 8

* __Arquitetura__: Minimal API, Clean Architecture

* __Banco de Dados__: MySQL

* __ORM__: Entity Framework Core 8

* __Autenticação/Autorização__: JWT (JSON Web Tokens)

* __Segurança__: BCrypt.Net (Hashing de Senhas)

* __Testes__: MSTest, WebApplicationFactory para testes de integração

* __Documentação__: Swagger (OpenAPI)

## 🏛️ Conceitos de Arquitetura Demonstrados
* Neste projeto foi utilizado os seguintes conceitos de arquiterura:

  - RESTful API Design

  - Clean Architecture

  - Injeção de Dependência (DI)

  - Princípios SOLID (especialmente SRP)

  - Test-Driven Development (TDD): O projeto possui uma cobertura de testes que valida desde a lógica de negócio até as requisições HTTP.

  - Gerenciamento de Configuração: Uso de appsettings.json e User Secrets para proteger dados sensíveis.

## 🏁 Como Executar o Projeto
* Siga os passos abaixo para executar a aplicação localmente.

### Pré-requisitos
.NET 8 SDK

### 1. Clone o Repositório
```
git clone https://github.com/Lucasgyn94/minimal-api.git
cd minimal-api/Api
```

### 2. Configure os User Secrets
* Para proteger nossas credenciais, o projeto utiliza User Secrets. Execute os comandos abaixo na pasta Api/ para configurar sua string de conexão e a chave JWT.
```
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=minimal_api;Uid=SEU_USUARIO;Pwd=SUA_SENHA"

dotnet user-secrets set "Jwt" "SUA_CHAVE_SECRETA_DE_NO_MINIMO_32_CARACTERES_E_SEGURA_AQUI_12345"
```

**OBS**: Lembrar de criar o banco de dados __minimal_api__ no __MySQL__ antes de prosseguir.

### 3. Aplique as Migrations
* O comando abaixo irá criar a estrutura do banco de dados e popular os dados iniciais.
```
dotnet ef database update
```

### 4. Execute a API
```
dotnet run
```
* A API estará disponível em http://localhost:5062 (ou outra porta definida no launchSettings.json). A documentação do Swagger pode ser acessada em http://localhost:5062/swagger.

### 5. Execute os Testes
* Para rodar a suíte de testes, navegue para a pasta raiz do projeto e execute:
```
dotnet test
```

## 📍 Endpoints da API
<table style="width:100%; border-collapse: collapse; border: 1px solid #444;">
  <thead style="background-color: #2c3e50; color: #ecf0f1;">
    <tr>
      <th style="padding: 12px; text-align: left; border-bottom: 2px solid #3498db;">Verbo</th>
      <th style="padding: 12px; text-align: left; border-bottom: 2px solid #3498db;">Rota</th>
      <th style="padding: 12px; text-align: left; border-bottom: 2px solid #3498db;">Descrição</th>
      <th style="padding: 12px; text-align: left; border-bottom: 2px solid #3498db;">Autorização</th>
    </tr>
  </thead>
  <tbody>
    <tr style="background-color: #34495e;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #2980b9;">GET</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Exibe a mensagem de boas-vindas da API.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Nenhuma</td>
    </tr>
    <tr style="background-color: #2c3e50;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #27ae60;">POST</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/administradores/login</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Autentica um usuário e retorna um token JWT.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Nenhuma</td>
    </tr>
    <tr style="background-color: #34495e;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #2980b9;">GET</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/administradores</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Lista todos os administradores.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>Adm</code></td>
    </tr>
    <tr style="background-color: #2c3e50;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #27ae60;">POST</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/administradores</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Cria um novo administrador.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>Adm</code></td>
    </tr>
    <tr style="background-color: #34495e;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #2980b9;">GET</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/veiculos</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Lista todos os veículos.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>Adm</code>, <code>Editor</code></td>
    </tr>
    <tr style="background-color: #2c3e50;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #2980b9;">GET</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/veiculos/{id}</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Busca um veículo por ID.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>Adm</code>, <code>Editor</code></td>
    </tr>
    <tr style="background-color: #34495e;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #27ae60;">POST</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/veiculos</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Cria um novo veículo.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>Adm</code>, <code>Editor</code></td>
    </tr>
    <tr style="background-color: #2c3e50;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: #2c3e50; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #f39c12;">PUT</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/veiculos/{id}</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Atualiza um veículo existente.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>Adm</code></td>
    </tr>
    <tr style="background-color: #34495e;">
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><span style="display: inline-block; padding: 4px 10px; color: white; border-radius: 4px; font-weight: bold; font-size: 0.9em; background-color: #c0392b;">DELETE</span></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>/veiculos/{id}</code></td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;">Apaga um veículo.</td>
      <td style="padding: 10px; vertical-align: middle; border-bottom: 1px solid #444;"><code>Adm</code></td>
    </tr>
  </tbody>
</table>

## Link do Repositório Base Original Sem Modificações
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/digitalinnovationone/minimal-api)

## 📫 Contato
**Lucas Ferreira da Silva**
<br>
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/lucas-ferreira-55053412a/)
<br>
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Lucasgyn94)
