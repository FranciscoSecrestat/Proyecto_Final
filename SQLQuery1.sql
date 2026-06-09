CREATE DATABASE ProyectoFinal;
GO

USE ProyectoFinal;
GO


CREATE TABLE CryptoCurrencies (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(10) NOT NULL UNIQUE,
    CurrentPrice DECIMAL(18,2),
    LastUpdated DATETIME
);


CREATE TABLE TransactionTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Name NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Transactions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    CryptoCurrencyId INT NOT NULL,
    TransactionTypeId INT NOT NULL,
    Amount DECIMAL(18,8) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    TransactionDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CryptoCurrencyId) REFERENCES CryptoCurrencies(Id),
    FOREIGN KEY (TransactionTypeId) REFERENCES TransactionTypes(Id)
);