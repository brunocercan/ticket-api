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
* Scalar / Swagger
* Dependency Injection
* Repository Pattern
* FluentValidation
* JWT Authentication
* BCrypt.NET
* Docker
* xUnit / Moq (Testes)

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
├── Services/
├── Validators/
└── Tests/
```

## 🎫 Endpoints

### Autenticação

| Método | Endpoint         | Descrição              |
| ------ | ---------------- | ---------------------- |
| POST   | `/api/auth/login`    | Login de usuário       |
| POST   | `/api/auth/register` | Registro de usuário    |

### Tickets

| Método | Endpoint              | Descrição                                   |
| ------ | --------------------- | ------------------------------------------- |
| GET    | `/api/tickets`        | Lista tickets (com paginação)               |
| GET    | `/api/tickets/{id}`   | Busca ticket por ID                         |
| GET    | `/api/tickets/details`| Consulta tickets com detalhes e comentários |
| POST   | `/api/tickets`        | Cria um ticket                              |
| PUT    | `/api/tickets/{id}`   | Atualiza um ticket                          |
| DELETE | `/api/tickets/{id}`   | Remove um ticket                            |
| POST   | `/api/tickets/comment`| Adiciona comentário ao ticket               |

### Usuários

| Método | Endpoint            | Descrição                         |
| ------ | ------------------- | --------------------------------- |
| GET    | `/api/users`        | Lista usuários (com paginação)    |
| GET    | `/api/users/{id}`   | Busca usuário por ID              |
| POST   | `/api/users`        | Cria um usuário                   |
| PUT    | `/api/users/{id}`   | Atualiza um usuário               |
| DELETE | `/api/users/{id}`   | Remove um usuário                 |

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
User 1 ─────── N Tickets (Requester)
User 1 ─────── N Tickets (AssignedTo)
Category 1 ─── N Tickets
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

## ⚙️ Configuração do Ambiente

### 1. Backend (.NET API)

#### Arquivos de configuração

O projeto utiliza arquivos `appsettings.json` para configuração.

#### Passos para configurar:

1. **Copie o arquivo de exemplo:**
   ```bash
   cd TicketApi
   cp appsettings.Example.json appsettings.json
   ```

2. **Edite o `appsettings.json` com suas credenciais:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TicketApiDb;User Id=sa;Password=SUA_SENHA_AQUI;TrustServerCertificate=True;"
     },
     "JwtSettings": {
       "SecretKey": "SUA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES!",
       "Issuer": "TicketApi",
       "Audience": "TicketApiUsers",
       "ExpirationInMinutes": 60
     }
   }
   ```

   > ⚠️ **Importante:** A `SecretKey` deve ter **pelo menos 32 caracteres** para segurança do JWT.

3. **Para desenvolvimento local com SQL Server local:**
   - Certifique-se de ter o SQL Server rodando localmente
   - Use a senha do seu usuário `sa` do SQL Server

4. **Para usar User Secrets (recomendado para desenvolvimento):**
   ```bash
   cd TicketApi
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TicketApiDb;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;"
   dotnet user-secrets set "JwtSettings:SecretKey" "SUA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES!"
   ```

### 2. Frontend (Angular)

#### Configuração:

Edite `FrontEnd/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:8080'  // URL da API backend
};
```

Para produção, edite `environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://sua-api-producao.com'
};
```

### 3. Docker Compose

#### Variáveis de ambiente

O `docker-compose.yml` usa variáveis de ambiente definidas no arquivo `.env`.

1. **Copie o arquivo de exemplo:**
   ```bash
   cp .env.example .env
   ```

2. **Edite o `.env` com suas credenciais:**
   ```env
   # SQL Server SA Password (mínimo 8 chars: maiúscula, minúscula, número, especial)
   SA_PASSWORD=SuaSenhaForte123!

   # JWT Secret Key (mínimo 32 caracteres)
   JWT_SECRET_KEY=SuaChaveSecretaSuperSeguraComPeloMenos32Caracteres!
   ```

3. **Suba os containers:**
   ```bash
   docker-compose up -d
   ```

   Isso irá subir:
   - **SQL Server** na porta 1433
   - **API** nas portas 8080 (HTTP) e 8081 (HTTPS)

4. **Verifique os logs:**
   ```bash
   docker-compose logs -f api
   docker-compose logs -f sqlserver
   ```

5. **Pare os containers:**
   ```bash
   docker-compose down
   ```

   > Para remover os volumes (dados do banco):
   > ```bash
   > docker-compose down -v
   > ```

## ▶️ Executando Localmente (Sem Docker)

### Pré-requisitos

* .NET 9 SDK
* SQL Server (LocalDB, Express, ou Developer Edition)
* Node.js 18+ (para o Frontend)

### Backend

```bash
# 1. Clone o repositório
git clone https://github.com/brunocercan/ticket-api.git
cd ticket-api/TicketApi

# 2. Configure o appsettings.json (veja seção acima)

# 3. Restaure dependências e compile
dotnet restore
dotnet build

# 4. Execute as migrações do EF Core (se aplicável)
# dotnet ef database update

# 5. Execute a API
dotnet run
```

A documentação da API estará disponível através do **Swagger** em `http://localhost:8080/swagger` ou **Scalar** em `http://localhost:8080/scalar`.

### Frontend

```bash
cd FrontEnd

# Instale dependências
npm install

# Execute em modo desenvolvimento
npm start
```

O frontend estará disponível em `http://localhost:4200`.

## 🐳 Dockerfile da API

O `TicketApi/Dockerfile` está configurado com multi-stage build:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TicketApi/ticket-api.csproj", "TicketApi/"]
RUN dotnet restore "TicketApi/ticket-api.csproj"
COPY . .
WORKDIR "/src/TicketApi"
RUN dotnet build "ticket-api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ticket-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ticket-api.dll"]
```

### Build manual da imagem:

```bash
docker build -t ticket-api -f TicketApi/Dockerfile .
docker run -p 8080:8080 -p 8081:8081 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=TicketApiDb;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;" \
  -e JwtSettings__SecretKey="SUA_CHAVE_SECRETA_32_CHARS!" \
  ticket-api
```

## 🧪 Testes

```bash
cd TicketApi.Tests
dotnet test
```

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
* [x] CRUD de Usuários
* [x] Validação (FluentValidation)
* [x] Paginação
* [x] Autenticação e autorização (JWT)
* [x] Testes automatizados (xUnit + Moq)
* [x] Docker
* [x] Configuração segura com variáveis de ambiente

## 👨‍💻 Autor

**Bruno Cercan Garcia**

Desenvolvedor Back-End .NET

[GitHub](https://github.com/brunocercan)