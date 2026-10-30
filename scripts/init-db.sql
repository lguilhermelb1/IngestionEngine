IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IngestionDb')
BEGIN
    CREATE DATABASE IngestionDb;
END
GO

USE IngestionDb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'IngestionRecords')
BEGIN
    CREATE TABLE IngestionRecords (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        DocumentNumber VARCHAR(11) NOT NULL,
        Name VARCHAR(150) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        ProcessedAt DATETIME2 NOT NULL
    );

    CREATE INDEX IX_IngestionRecords_DocumentNumber ON IngestionRecords (DocumentNumber);
END
GO