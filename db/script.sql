USE TicketPrime;
GO

-- 1. APAGAR NA ORDEM CORRETA (Primeiro as que têm FK, depois as que são apontadas)
IF OBJECT_ID('Reservas', 'U') IS NOT NULL DROP TABLE Reservas;
IF OBJECT_ID('Eventos', 'U') IS NOT NULL DROP TABLE Eventos;
IF OBJECT_ID('Usuarios', 'U') IS NOT NULL DROP TABLE Usuarios;
IF OBJECT_ID('Cupons', 'U') IS NOT NULL DROP TABLE Cupons;
GO

-- 2. TABELA USUARIOS (Nomes exatos: Cpf, Nome, Email)
CREATE TABLE Usuarios (
    Cpf VARCHAR(14) PRIMARY KEY, -- O PDF pede Cpf como PK
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Senha VARCHAR(MAX) NOT NULL, -- Necessário para o login funcionar
    NivelAcesso INT DEFAULT 3 -- 0 para usuário comum, 1 para administrador
);

-- 3. TABELA EVENTOS (Nomes exatos: Id, Nome, CapacidadeTotal, DataEvento, PrecoPadrao)
CREATE TABLE Eventos (
    Id INT PRIMARY KEY IDENTITY,
    Nome VARCHAR(100) NOT NULL,
    Lugar VARCHAR(255) NOT NULL,
    CapacidadeTotal INT NOT NULL,
    DataEvento DATETIME NOT NULL,
    PrecoPadrao DECIMAL(10,2) NOT NULL,
    ImagemURL VARCHAR(MAX) NULL -- ADICIONE ESTA LINHA
);

-- 4. TABELA CUPONS (Nomes exatos: codigo, PorcentagemDesconto, valorMinimoregra)
CREATE TABLE Cupons (
    codigo VARCHAR(50) PRIMARY KEY,
    PorcentagemDesconto DECIMAL(5,2) NOT NULL,
    valorMinimoregra DECIMAL(10,2) NOT NULL
);

-- 5. TABELA RESERVAS (A principal da AV2, mas obrigatória no script da AV1)
CREATE TABLE Reservas (
    Id INT PRIMARY KEY IDENTITY,
    UsuarioCpf VARCHAR(14) NOT NULL FOREIGN KEY REFERENCES Usuarios(Cpf),
    EventoId INT NOT NULL FOREIGN KEY REFERENCES Eventos(Id),
    CupomUtilizado VARCHAR(50) NULL FOREIGN KEY REFERENCES Cupons(codigo),
    ValorFinalPago DECIMAL(10,2) NOT NULL
);
GO
ALTER TABLE Reservas ADD CupomAplicado VARCHAR(50) NULL;
INSERT INTO Usuarios (Nome, CPF, Email, Senha, NivelAcesso)
VALUES ('Administrador Master', '000.000.000-00', 'admin@prime.com', '123456', 1);
GO