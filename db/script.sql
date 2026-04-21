-- SCRIPT DE CRIAÇÃO DO BANCO DE DADOS TICKETPRIME

-- 1. Criar o Banco de Dados
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TicketPrime')
BEGIN
    CREATE DATABASE TicketPrime;
END
GO

USE TicketPrime;
GO

-- 2. Limpar tabelas antigas (para evitar erros ao rodar o script várias vezes)
IF OBJECT_ID('Eventos', 'U') IS NOT NULL DROP TABLE Eventos;
IF OBJECT_ID('Cupons', 'U') IS NOT NULL DROP TABLE Cupons;
IF OBJECT_ID('Usuarios', 'U') IS NOT NULL DROP TABLE Usuarios;
GO

-- 3. Criar Tabela de Usuários (Admin, Vendedor e Cliente)
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY,
    Nome VARCHAR(100) NOT NULL,
    CPF VARCHAR(14) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Senha VARCHAR(MAX) NOT NULL,
    NivelAcesso INT DEFAULT 3 -- 1: Admin, 2: Vendedor, 3: Cliente
);

-- 4. Criar Tabela de Cupons
CREATE TABLE Cupons (
    Id INT PRIMARY KEY IDENTITY,
    Codigo VARCHAR(50) UNIQUE NOT NULL,
    PorcentagemDesconto DECIMAL(5,2) NOT NULL,
    ValorMinimo DECIMAL(10,2) DEFAULT 0
);

-- 5. Criar Tabela de Eventos (ATUALIZADA COM QUANTIDADE)
CREATE TABLE Eventos (
    Id INT PRIMARY KEY IDENTITY,
    Nome VARCHAR(100) NOT NULL,
    Local VARCHAR(100) NOT NULL,
    Data DATETIME NOT NULL,
    Preco DECIMAL(10,2) NOT NULL,
    Quantidade INT NOT NULL DEFAULT 1, -- ADICIONE ESTA LINHA AQUI
    ImagemURL VARCHAR(MAX),
    VendedorId INT FOREIGN KEY REFERENCES Usuarios(Id)
);
GO

-- 6. Inserir o Administrador Padrão (Login que você vai usar)
INSERT INTO Usuarios (Nome, CPF, Email, Senha, NivelAcesso)
VALUES ('Administrador Master', '000.000.000-00', 'admin@prime.com', '123456', 1);
GO