# 🎫 Ticket API

API REST para gerenciamento de chamados de um sistema de Help Desk, desenvolvida com **C# e .NET 9**, utilizando **Entity Framework Core** e **Dapper** para acesso e consulta aos dados.

O projeto está sendo desenvolvido como um projeto de portfólio com foco em boas práticas de desenvolvimento de APIs REST, separação de responsabilidades, injeção de dependência, acesso a dados e aplicação de diferentes estratégias de consulta ao banco.

---

## 🚀 Tecnologias

* **C#**
* **.NET 9**
* **ASP.NET Core Web API**
* **Entity Framework Core 9**
* **Dapper**
* **SQL Server**
* **Scalar** — documentação da API
* **Dependency Injection**
* **Repository Pattern**
* **DTOs**
* **Middleware para tratamento de exceções**

---

## 📌 Objetivo

O objetivo da aplicação é disponibilizar uma API para gerenciamento de tickets de suporte, permitindo controlar:

* Tickets
* Usuários
* Categorias
* Comentários dos tickets
* Prioridade
* Status
* Responsável pelo atendimento
* Solicitante do chamado

Além da implementação dos endpoints, o projeto busca demonstrar diferentes abordagens de acesso a dados utilizando **Entity Framework Core** e **Dapper**.

---

## 🏗️ Arquitetura

A aplicação utiliza uma separação entre **Controllers, Services, Repositories e Data Access**, seguindo uma abordagem simples de camadas.

```text
                    ┌──────────────────┐
                    │    Controller    │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │     Service      │
                    └────────┬─────────┘
                             │
                ┌────────────┴────────────┐
                │                         │
                ▼                         ▼
       ┌─────────────────┐       ┌────────────────────┐
       │    Repository   │       │ Query Repository   │
       │   EntityFramework│      │      Dapper        │
       └────────┬────────┘       └──────────┬─────────┘
                │                           │
                ▼                           ▼
       ┌────────────────────────────────────────────┐
       │                  SQL Server                │
       └────────────────────────────────────────────┘
```

### Entity Framework Core

O **Entity Framework Core** é utilizado principalmente para operações de persistência e operações CRUD.

### Dapper

O **Dapper** é utilizado para consultas mais específicas, principalmente quando existe a necessidade de trabalhar com:

* SQL personalizado
* múltiplos `JOINs`
* projeções específicas
* consultas envolvendo relacionamentos
* retorno de DTOs específicos

Um dos exemplos atuais é a consulta de detalhes dos tickets, que utiliza Dapper para retornar um ticket juntamente com sua lista de comentários.

---

## 📂 Estrutura do projeto

```text
TicketApi/
│
├── Controllers/
│   ├── TicketController.cs
│   └── UserController.cs
│
├── CustomExceptions/
│
├── Data/
│   ├── Dapper/
│   │   └── TicketQueryRepository.cs
│   │
│   ├── EntityFramework/
│   │   ├── TicketRepository.cs
│   │   ├── TicketCommentRepository.cs
│   │   └── UserRepository.cs
│   │
│   └── AppDbContext.cs
│
├── DataTransferObjects/
│
├── Helpers/
│
├── Interfaces/
│   ├── ITicketRepository.cs
│   ├── ITicketQueryRepository.cs
│   ├── ITicketService.cs
│   ├── ITicketCommentRepository.cs
│   ├── IUserRepository.cs
│   └── IUserService.cs
│
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
│
├── Models/
│   └── Tickets/
│
├── ScriptsDB/
│
├── Services/
│   ├── TicketService.cs
│   └── UserService.cs
│
├── Program.cs
└── ticket-api.csproj
```

---

## 🎫 Tickets

A API trabalha com tickets contendo informações como:

* Título
* Descrição
* Prioridade
* Status
* Categoria
* Solicitante
* Responsável
* Data de criação
* Data de atualização
* Data de fechamento

Os tickets possuem relacionamento com:

```text
User
  │
  ├── Requester
  │
  └── Support

Category
  │
  └── Ticket

Ticket
  │
  └── N Comments
```

---

## 💬 Comentários

Cada ticket pode possuir vários comentários.

A relação utilizada é:

```text
Ticket 1 ──────────── N TicketComments
```

A consulta de detalhes utiliza **Dapper Multi-Mapping** para transformar o resultado do `JOIN` em um objeto de ticket contendo uma coleção de comentários.

Exemplo conceitual:

```json
{
  "id": 1,
  "titulo": "Problema no acesso",
  "descricao": "Usuário não consegue acessar o sistema",
  "prioridade": "High",
  "status": "Open",
  "detalhesTicket": [
    {
      "idComentario": 1,
      "idTicket": 1,
      "idUsuario": 5,
      "conteudo": "Usuário entrou em contato.",
      "dataCriacao": "2026-09-09T10:00:00"
    },
    {
      "idComentario": 2,
      "idTicket": 1,
      "idUsuario": 8,
      "conteudo": "Chamado encaminhado para o suporte.",
      "dataCriacao": "2026-09-09T11:30:00"
    }
  ]
}
```

Para evitar a duplicação do ticket causada pelo relacionamento 1:N no resultado do SQL, a consulta utiliza um `Dictionary<int, ConsultaDetalheTicketResponse>` para agrupar os comentários pertencentes a cada ticket.

A implementação também utiliza `LEFT JOIN`, permitindo que tickets sem comentários continuem sendo retornados.

---

## 🔎 Consultas com Dapper

A consulta de detalhes dos tickets utiliza SQL parametrizado e permite aplicar filtros como:

* ID
* Prioridade
* Status
* Título

Exemplo simplificado:

```sql
SELECT
    t.Id,
    t.Title,
    t.Description,
    t.Priority,
    t.Status,
    tc.Id AS IdComentario,
    tc.TicketId AS IdTicket,
    tc.UserId AS IdUsuario,
    tc.Content AS Conteudo,
    tc.CreatedAt AS DataCriacao
FROM Tickets AS t
LEFT JOIN TicketComments AS tc
    ON tc.TicketId = t.Id
```

O Dapper utiliza Multi-Mapping para separar os dados do ticket e dos comentários:

```text
ConsultaDetalheTicketResponse
              +
        DetalheTicket
              ↓
     Ticket + List<DetalheTicket>
```

---

## 🔌 Endpoints

### Tickets

| Método   | Endpoint              | Descrição                       |
| -------- | --------------------- | ------------------------------- |
| `GET`    | `/api/ticket`         | Consulta tickets                |
| `GET`    | `/api/ticket/details` | Consulta detalhes dos tickets   |
| `POST`   | `/api/ticket`         | Cadastro de ticket              |
| `PUT`    | `/api/ticket`         | Atualização de ticket           |
| `DELETE` | `/api/ticket`         | Remoção de ticket               |
| `POST`   | `/api/ticket/comment` | Adiciona comentário a um ticket |

> Alguns endpoints de criação, edição e remoção ainda estão em desenvolvimento.

### Usuários

A estrutura inicial do `UserController` já foi criada e está sendo desenvolvida.

---

## 🧩 Dependency Injection

As principais dependências são registradas no `Program.cs`.

Exemplo:

```csharp
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<ITicketQueryRepository, TicketQueryRepository>();
```

O `AppDbContext` é utilizado pelo Entity Framework:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

Enquanto o Dapper utiliza uma conexão SQL Server registrada como `IDbConnection`:

```csharp
builder.Services.AddScoped<IDbConnection>(_ =>
    new SqlConnection(connectionString));
```

Dessa forma, o projeto consegue utilizar as duas abordagens de acesso a dados de maneira independente.

---

## 🛡️ Tratamento de exceções

A aplicação possui um middleware dedicado ao tratamento de exceções:

```text
Middleware/
└── ExceptionHandlingMiddleware.cs
```

O middleware é registrado no pipeline:

```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

A ideia é centralizar o tratamento de erros da aplicação em vez de espalhar `try/catch` pelos controllers.

---

## 📖 Documentação da API

A aplicação utiliza **Scalar** para disponibilizar a documentação dos endpoints durante o ambiente de desenvolvimento.

```csharp
app.MapOpenApi();
app.MapScalarApiReference();
```

Com a aplicação em execução, a documentação pode ser acessada pela interface disponibilizada pelo Scalar.

---

## ⚙️ Configuração

A aplicação utiliza uma connection string chamada:

```text
DefaultConnection
```

Exemplo de configuração:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HelpDeskDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

> Não versionar senhas ou outras credenciais no repositório.

---

## 🗄️ Banco de Dados

O projeto utiliza **SQL Server**.

O banco possui entidades relacionadas ao gerenciamento do Help Desk, incluindo:

```text
Users
Categories
Tickets
TicketComments
```

Relacionamento principal:

```text
Users
  │
  ├───────────────┐
  │               │
  ▼               ▼
Tickets       TicketComments
  │               ▲
  │               │
  └───────────────┘
        1 : N
```

Scripts relacionados ao banco estão disponíveis no diretório:

```text
ScriptsDB/
```

---

## ▶️ Executando o projeto

### Pré-requisitos

* .NET 9 SDK
* SQL Server
* IDE de sua preferência, como Visual Studio ou JetBrains Rider

### Clone o repositório

```bash
git clone https://github.com/brunocercan/ticket-api.git
```

### Acesse o projeto

```bash
cd ticket-api/TicketApi
```

### Configure a connection string

Configure a `DefaultConnection` no arquivo de configuração da aplicação.

### Execute a API

```bash
dotnet restore
dotnet build
dotnet run
```

Após iniciar a aplicação, utilize a documentação disponibilizada pelo Scalar para consultar e testar os endpoints.

---

## 🧪 Status atual do projeto

### Implementado

* [x] Estrutura inicial da Web API
* [x] Controllers
* [x] Services
* [x] Repository Pattern
* [x] Dependency Injection
* [x] Entity Framework Core
* [x] SQL Server
* [x] CRUD inicial de Tickets
* [x] DTOs
* [x] Middleware de tratamento de exceções
* [x] Documentação com Scalar
* [x] Repository específico para consultas com Dapper
* [x] Consultas parametrizadas com Dapper
* [x] Filtros de consulta de tickets
* [x] `LEFT JOIN` entre Tickets e TicketComments
* [x] Dapper Multi-Mapping
* [x] Relacionamento 1:N entre Ticket e Comentários
* [x] Retorno de lista de comentários dentro do detalhe do Ticket
* [x] Endpoint para cadastro de comentário
* [x] Estrutura inicial de UserController/UserService

### 🚧 Em desenvolvimento

* [ ] Finalizar CRUD de Tickets
* [ ] Finalizar CRUD de Usuários
* [ ] Consulta individual de Ticket
* [ ] Melhorar retorno dos comentários
* [ ] Paginação
* [ ] Validação de entrada
* [ ] Autenticação e autorização
* [ ] Testes automatizados
* [ ] Docker
* [ ] Melhorias de documentação
* [ ] Melhorias de arquitetura conforme evolução do projeto

---

## 🗺️ Roadmap

A evolução planejada para o projeto inclui:

```text
[x] Web API
 │
 ├── [x] Entity Framework Core
 │
 ├── [x] Repository Pattern
 │
 ├── [x] Service Layer
 │
 ├── [x] Dependency Injection
 │
 ├── [x] Dapper
 │
 ├── [x] Consultas com JOIN
 │
 ├── [x] Relacionamento 1:N
 │
 ├── [x] Dapper Multi-Mapping
 │
 ├── [ ] CRUD completo
 │
 ├── [ ] Validação
 │
 ├── [ ] Paginação
 │
 ├── [ ] JWT / Authentication
 │
 ├── [ ] Authorization
 │
 ├── [ ] Unit Tests
 │
 ├── [ ] Integration Tests
 │
 ├── [ ] Docker
 │
 └── [ ] CI/CD
```

---

## 🎯 Objetivos de aprendizado

Este projeto está sendo desenvolvido com o objetivo de praticar e demonstrar conhecimentos em:

* Desenvolvimento de APIs REST com ASP.NET Core
* C#
* .NET
* Entity Framework Core
* Dapper
* SQL Server
* SQL e relacionamentos
* Repository Pattern
* Service Layer
* Dependency Injection
* DTOs
* Middleware
* Tratamento de exceções
* Consultas SQL complexas
* Dapper Multi-Mapping
* Boas práticas de organização de código

---

## 📌 Próximos passos

O próximo ciclo de desenvolvimento deve priorizar:

1. Finalizar o cadastro de Tickets utilizando Entity Framework.
2. Implementar consulta individual de Ticket.
3. Implementar atualização de Ticket.
4. Implementar exclusão de Ticket.
5. Evoluir os endpoints de comentários.
6. Implementar paginação e filtros.
7. Adicionar validações.
8. Implementar autenticação com JWT.
9. Criar testes automatizados.
10. Containerizar a aplicação com Docker.

---

## 👨‍💻 Autor

**Bruno Cercan Garcia**

Desenvolvedor Back-End .NET

[GitHub](https://github.com/brunocercan)

---

## 📄 Licença

Este projeto está sendo desenvolvido para fins de estudo e portfólio.