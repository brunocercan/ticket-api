# 🎫 Ticket API

API REST para gerenciamento de tickets de Help Desk, desenvolvida com **C# e .NET 9**.

Projeto de portfólio com foco em desenvolvimento de APIs, separação de responsabilidades e acesso a dados utilizando **Entity Framework Core** e **Dapper**.

## 🚀 Tecnologias

* C#
* .NET 9
* ASP.NET Core Web API
* Entity Framework Core
* Dapper
* SQL Server
* Scalar
* Dependency Injection
* Repository Pattern

## 🏗️ Estrutura

```text
TicketApi/
├── Controllers/
├── CustomExceptions/
├── Data/
│   ├── Dapper/
│   └── EntityFramework/
├── DataTransferObjects/
├── Helpers/
├── Interfaces/
├── Middleware/
├── Models/
├── ScriptsDB/
└── Services/
```

## 🎫 Endpoints

### Tickets

| Método | Endpoint              | Descrição                                   |
| ------ | --------------------- | ------------------------------------------- |
| GET    | `/api/ticket`         | Lista tickets                               |
| GET    | `/api/ticket/details` | Consulta tickets com detalhes e comentários |
| POST   | `/api/ticket`         | Cria um ticket                              |
| PUT    | `/api/ticket/{id}`    | Atualiza um ticket                          |
| DELETE | `/api/ticket/{id}`    | Remove um ticket                            |
| POST   | `/api/ticket/comment` | Adiciona comentário ao ticket               |

### Usuários

O `UserController` já possui sua estrutura inicial e está em desenvolvimento.

## 💾 Acesso a dados

O projeto utiliza duas abordagens:

**Entity Framework Core**

* CRUD e persistência de dados.

**Dapper**

* Consultas específicas e com múltiplos `JOINs`.
* Multi-Mapping para retornar tickets com seus comentários.

Relacionamento:

```text
Ticket 1 ─────── N TicketComments
```

## 🗄️ Banco de dados

SQL Server com as principais entidades:

```text
Users
Categories
Tickets
TicketComments
```

Os scripts estão disponíveis em:

```text
ScriptsDB/
```

## ▶️ Executando

### Pré-requisitos

* .NET 9 SDK
* SQL Server

### Clone

```bash
git clone https://github.com/brunocercan/ticket-api.git
cd ticket-api/TicketApi
```

Configure a `DefaultConnection` no `appsettings.json` e execute:

```bash
dotnet restore
dotnet build
dotnet run
```

A documentação da API estará disponível através do **Scalar**.

## 📌 Status

* [x] Estrutura da Web API
* [x] Dependency Injection
* [x] Service Layer
* [x] Repository Pattern
* [x] Entity Framework Core
* [x] Dapper
* [x] CRUD de Tickets
* [x] CRUD de Comentários
* [x] Dapper Multi-Mapping
* [x] Tratamento global de exceções
* [ ] CRUD de Usuários
* [ ] Validação
* [ ] Paginação
* [ ] Autenticação e autorização
* [ ] Testes automatizados
* [ ] Docker

## 👨‍💻 Autor

**Bruno Cercan Garcia**

Desenvolvedor Back-End .NET

[GitHub](https://github.com/brunocercan)