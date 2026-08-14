-- SeedData_Current.sql
-- Exports the currently-seeded sample data (as observed from the running API)
-- Run this in SSMS against your target SQL Server instance.

IF DB_ID(N'InsuranceManagementDb') IS NULL
BEGIN
    PRINT 'Creating database InsuranceManagementDb';
    CREATE DATABASE [InsuranceManagementDb];
END
GO

USE [InsuranceManagementDb];
GO

-- Insert the single seeded customer (observed GUID from the running API)
IF NOT EXISTS (SELECT 1 FROM Customers WHERE Id = 'ed67a004-62e1-4a02-bd93-b5441a87bcf8')
BEGIN
    INSERT INTO Customers (Id, FirstName, LastName, Email, Phone, DateOfBirth, CreatedAt)
    VALUES (
        'ed67a004-62e1-4a02-bd93-b5441a87bcf8',
        'John',
        'Doe',
        'john.doe@example.com',
        '+1-555-0100',
        '1970-01-01',
        '2026-08-14T13:43:26.477491'
    );
END
GO

-- Insert one sample policy referencing the seeded customer (use deterministic GUID)
IF NOT EXISTS (SELECT 1 FROM Policies WHERE PolicyNumber = 'POL-1001')
BEGIN
    INSERT INTO Policies (Id, PolicyNumber, CustomerId, StartDate, EndDate, Premium, Status, CreatedAt)
    VALUES (
        '3f8b5e2b-9d4b-4e5b-8a7a-5d1c2b7a9a11',
        'POL-1001',
        'ed67a004-62e1-4a02-bd93-b5441a87bcf8',
        '2026-08-14',
        '2027-08-14',
        1200.00,
        'Active',
        '2026-08-14T13:43:26.477491'
    );
END
GO

-- Insert one sample claim referencing the seeded customer and policy
IF NOT EXISTS (SELECT 1 FROM Claims WHERE ClaimNumber = 'CLM-1001')
BEGIN
    INSERT INTO Claims (Id, ClaimNumber, PolicyId, CustomerId, DateOfLoss, Amount, Status, CreatedAt)
    VALUES (
        '7c9e6679-7425-40de-944b-e07fc1f90ae7',
        'CLM-1001',
        '3f8b5e2b-9d4b-4e5b-8a7a-5d1c2b7a9a11',
        'ed67a004-62e1-4a02-bd93-b5441a87bcf8',
        DATEADD(day, -10, '2026-08-14'),
        500.00,
        'Submitted',
        '2026-08-14T13:43:26.477491'
    );
END
GO
