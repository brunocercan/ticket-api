/*
    ============================================================
    HelpDesk API - Database Script
    Database: SQL Server
    ============================================================
*/

-- ============================================================
-- 1. DATABASE
-- ============================================================

IF DB_ID('HelpDeskDb') IS NULL
BEGIN
    CREATE DATABASE HelpDeskDb;
END
GO

USE HelpDeskDb;
GO


-- ============================================================
-- 2. CLEANUP
--    Permite executar o script novamente durante o desenvolvimento
-- ============================================================

IF OBJECT_ID('TicketComments', 'U') IS NOT NULL
    DROP TABLE TicketComments;

IF OBJECT_ID('Tickets', 'U') IS NOT NULL
    DROP TABLE Tickets;

IF OBJECT_ID('Categories', 'U') IS NOT NULL
    DROP TABLE Categories;

IF OBJECT_ID('Users', 'U') IS NOT NULL
    DROP TABLE Users;
GO


-- ============================================================
-- 3. USERS
-- ============================================================

CREATE TABLE Users
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(30) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Users PRIMARY KEY (Id),

    CONSTRAINT UQ_Users_Email UNIQUE (Email),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN ('Admin', 'Support', 'User'))
);
GO


-- ============================================================
-- 4. CATEGORIES
-- ============================================================

CREATE TABLE Categories
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,

    CONSTRAINT PK_Categories PRIMARY KEY (Id),

    CONSTRAINT UQ_Categories_Name UNIQUE (Name)
);
GO


-- ============================================================
-- 5. TICKETS
-- ============================================================

CREATE TABLE Tickets
(
    Id INT IDENTITY(1,1) NOT NULL,

    Title NVARCHAR(200) NOT NULL,

    Description NVARCHAR(MAX) NOT NULL,

    Priority NVARCHAR(20) NOT NULL,

    Status NVARCHAR(20) NOT NULL,

    CategoryId INT NOT NULL,

    RequesterId INT NOT NULL,

    AssignedToId INT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Tickets_CreatedAt DEFAULT SYSUTCDATETIME(),

    UpdatedAt DATETIME2 NULL,

    ClosedAt DATETIME2 NULL,

    CONSTRAINT PK_Tickets PRIMARY KEY (Id),

    CONSTRAINT FK_Tickets_Category
        FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id),

    CONSTRAINT FK_Tickets_Requester
        FOREIGN KEY (RequesterId)
        REFERENCES Users(Id),

    CONSTRAINT FK_Tickets_AssignedTo
        FOREIGN KEY (AssignedToId)
        REFERENCES Users(Id),

    CONSTRAINT CK_Tickets_Priority
        CHECK (Priority IN ('Low', 'Medium', 'High', 'Critical')),

    CONSTRAINT CK_Tickets_Status
        CHECK (Status IN ('Open', 'InProgress', 'Resolved', 'Closed'))
);
GO


-- ============================================================
-- 6. TICKET COMMENTS
-- ============================================================

CREATE TABLE TicketComments
(
    Id INT IDENTITY(1,1) NOT NULL,

    TicketId INT NOT NULL,

    UserId INT NOT NULL,

    Content NVARCHAR(MAX) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_TicketComments_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_TicketComments PRIMARY KEY (Id),

    CONSTRAINT FK_TicketComments_Ticket
        FOREIGN KEY (TicketId)
        REFERENCES Tickets(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_TicketComments_User
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
);
GO


-- ============================================================
-- 7. INDEXES
-- ============================================================

CREATE INDEX IX_Tickets_Status
    ON Tickets(Status);
GO

CREATE INDEX IX_Tickets_Priority
    ON Tickets(Priority);
GO

CREATE INDEX IX_Tickets_CategoryId
    ON Tickets(CategoryId);
GO

CREATE INDEX IX_Tickets_RequesterId
    ON Tickets(RequesterId);
GO

CREATE INDEX IX_Tickets_AssignedToId
    ON Tickets(AssignedToId);
GO

CREATE INDEX IX_Tickets_CreatedAt
    ON Tickets(CreatedAt DESC);
GO

CREATE INDEX IX_TicketComments_TicketId
    ON TicketComments(TicketId);
GO


-- ============================================================
-- 8. CATEGORIES - SEED DATA
-- ============================================================

INSERT INTO Categories
(
    Name,
    Description
)
VALUES
(
    'Hardware',
    'Problemas relacionados a computadores, notebooks, monitores e periféricos.'
),
(
    'Software',
    'Problemas relacionados a sistemas, aplicações e softwares.'
),
(
    'Network',
    'Problemas relacionados à rede, internet, VPN e conectividade.'
),
(
    'Access',
    'Problemas relacionados a acesso, autenticação e permissões.'
),
(
    'Email',
    'Problemas relacionados a e-mail e comunicação corporativa.'
);
GO


-- ============================================================
-- 9. USERS - SEED DATA
--
-- PasswordHash abaixo é apenas um valor fictício para os dados
-- iniciais. Na aplicação real, as senhas deverão ser armazenadas
-- utilizando um algoritmo apropriado de hashing.
-- ============================================================

INSERT INTO Users
(
    Name,
    Email,
    PasswordHash,
    Role
)
VALUES
(
    'Admin Sistema',
    'admin@helpdesk.local',
    'DEMO_HASH_ADMIN',
    'Admin'
),
(
    'Carlos Oliveira',
    'carlos.oliveira@helpdesk.local',
    'DEMO_HASH_CARLOS',
    'Support'
),
(
    'Mariana Santos',
    'mariana.santos@helpdesk.local',
    'DEMO_HASH_MARIANA',
    'Support'
),
(
    'Bruno Almeida',
    'bruno.almeida@helpdesk.local',
    'DEMO_HASH_BRUNO',
    'User'
),
(
    'Fernanda Costa',
    'fernanda.costa@helpdesk.local',
    'DEMO_HASH_FERNANDA',
    'User'
),
(
    'Rafael Souza',
    'rafael.souza@helpdesk.local',
    'DEMO_HASH_RAFAEL',
    'User'
),
(
    'Juliana Martins',
    'juliana.martins@helpdesk.local',
    'DEMO_HASH_JULIANA',
    'User'
);
GO


-- ============================================================
-- 10. TICKETS - SEED DATA
-- ============================================================

INSERT INTO Tickets
(
    Title,
    Description,
    Priority,
    Status,
    CategoryId,
    RequesterId,
    AssignedToId,
    CreatedAt,
    UpdatedAt,
    ClosedAt
)
VALUES
(
    'Notebook não conecta ao Wi-Fi',
    'O notebook não consegue se conectar à rede corporativa desde o início da manhã.',
    'High',
    'InProgress',
    3,
    4,
    2,
    DATEADD(DAY, -1, SYSUTCDATETIME()),
    DATEADD(HOUR, -3, SYSUTCDATETIME()),
    NULL
),
(
    'Erro ao abrir sistema financeiro',
    'O sistema financeiro apresenta erro inesperado ao tentar realizar o login.',
    'Critical',
    'Open',
    2,
    5,
    NULL,
    DATEADD(HOUR, -8, SYSUTCDATETIME()),
    NULL,
    NULL
),
(
    'Solicitação de acesso à VPN',
    'Necessário liberar acesso à VPN corporativa para trabalho remoto.',
    'Medium',
    'Resolved',
    4,
    6,
    3,
    DATEADD(DAY, -3, SYSUTCDATETIME()),
    DATEADD(DAY, -2, SYSUTCDATETIME()),
    NULL
),
(
    'Monitor apresenta tela preta',
    'Monitor externo não apresenta imagem quando conectado ao notebook.',
    'Medium',
    'Open',
    1,
    7,
    2,
    DATEADD(DAY, -2, SYSUTCDATETIME()),
    NULL,
    NULL
),
(
    'Problema com e-mail corporativo',
    'Usuário não consegue enviar mensagens para destinatários externos.',
    'High',
    'InProgress',
    5,
    4,
    3,
    DATEADD(DAY, -1, SYSUTCDATETIME()),
    DATEADD(HOUR, -5, SYSUTCDATETIME()),
    NULL
),
(
    'Instalação de software',
    'Solicitação de instalação de ferramenta necessária para as atividades do usuário.',
    'Low',
    'Closed',
    2,
    5,
    2,
    DATEADD(DAY, -10, SYSUTCDATETIME()),
    DATEADD(DAY, -9, SYSUTCDATETIME()),
    DATEADD(DAY, -9, SYSUTCDATETIME())
),
(
    'Senha expirada',
    'Usuário não consegue acessar o sistema devido à senha expirada.',
    'High',
    'Resolved',
    4,
    6,
    2,
    DATEADD(DAY, -5, SYSUTCDATETIME()),
    DATEADD(DAY, -4, SYSUTCDATETIME()),
    NULL
),
(
    'Internet instável',
    'Conexão com a internet apresenta quedas frequentes durante o expediente.',
    'High',
    'Open',
    3,
    7,
    NULL,
    DATEADD(HOUR, -12, SYSUTCDATETIME()),
    NULL,
    NULL
),
(
    'Erro ao imprimir documento',
    'Impressora não está sendo reconhecida pela estação de trabalho.',
    'Medium',
    'Closed',
    1,
    4,
    2,
    DATEADD(DAY, -15, SYSUTCDATETIME()),
    DATEADD(DAY, -14, SYSUTCDATETIME()),
    DATEADD(DAY, -14, SYSUTCDATETIME())
),
(
    'Acesso ao sistema de RH',
    'Usuário precisa de acesso ao sistema interno de recursos humanos.',
    'Low',
    'InProgress',
    4,
    5,
    3,
    DATEADD(DAY, -2, SYSUTCDATETIME()),
    DATEADD(DAY, -1, SYSUTCDATETIME()),
    NULL
);
GO


-- ============================================================
-- 11. TICKET COMMENTS - SEED DATA
-- ============================================================

INSERT INTO TicketComments
(
    TicketId,
    UserId,
    Content,
    CreatedAt
)
VALUES
(
    1,
    4,
    'O problema começou após uma atualização do sistema operacional.',
    DATEADD(HOUR, -20, SYSUTCDATETIME())
),
(
    1,
    2,
    'Estamos verificando as configurações do adaptador de rede.',
    DATEADD(HOUR, -18, SYSUTCDATETIME())
),
(
    2,
    5,
    'O erro acontece somente no sistema financeiro.',
    DATEADD(HOUR, -7, SYSUTCDATETIME())
),
(
    3,
    6,
    'Solicitação de acesso realizada conforme procedimento.',
    DATEADD(DAY, -3, SYSUTCDATETIME())
),
(
    3,
    3,
    'Acesso à VPN liberado. Favor realizar novo teste.',
    DATEADD(DAY, -2, SYSUTCDATETIME())
),
(
    4,
    7,
    'Já testei outro cabo HDMI e o problema continua.',
    DATEADD(DAY, -2, SYSUTCDATETIME())
),
(
    5,
    4,
    'O problema ocorre somente ao enviar mensagens externas.',
    DATEADD(DAY, -1, SYSUTCDATETIME())
),
(
    5,
    3,
    'Estamos verificando as regras de segurança do servidor de e-mail.',
    DATEADD(HOUR, -4, SYSUTCDATETIME())
),
(
    6,
    2,
    'Software instalado e validado com o usuário.',
    DATEADD(DAY, -9, SYSUTCDATETIME())
),
(
    7,
    2,
    'Senha redefinida e acesso validado.',
    DATEADD(DAY, -4, SYSUTCDATETIME())
);
GO


-- ============================================================
-- 12. TEST QUERIES
-- ============================================================

-- Tickets
SELECT
    t.Id,
    t.Title,
    t.Priority,
    t.Status,
    c.Name AS Category,
    requester.Name AS Requester,
    support.Name AS AssignedTo,
    t.CreatedAt
FROM Tickets t
INNER JOIN Categories c
    ON c.Id = t.CategoryId
INNER JOIN Users requester
    ON requester.Id = t.RequesterId
LEFT JOIN Users support
    ON support.Id = t.AssignedToId
ORDER BY t.CreatedAt DESC;
GO


-- Comments
SELECT
    tc.Id,
    tc.TicketId,
    u.Name AS UserName,
    tc.Content,
    tc.CreatedAt
FROM TicketComments tc
INNER JOIN Users u
    ON u.Id = tc.UserId
ORDER BY tc.CreatedAt;
GO


-- Tickets por status
SELECT
    Status,
    COUNT(*) AS Total
FROM Tickets
GROUP BY Status;
GO


-- Tickets por prioridade
SELECT
    Priority,
    COUNT(*) AS Total
FROM Tickets
GROUP BY Priority;
GO