-- SeedData_MoreSamples.sql
-- Adds a set of additional realistic sample customers, policies and claims
-- Run in SSMS against InsuranceManagementDb after schema creation.

USE [InsuranceManagementDb];
GO

-- Add 5 customers
DECLARE @now DATETIME2 = SYSUTCDATETIME();

IF NOT EXISTS (SELECT 1 FROM Customers WHERE Email = 'alice.smith@example.com')
BEGIN
    INSERT INTO Customers (Id, FirstName, LastName, Email, Phone, DateOfBirth, CreatedAt)
    VALUES
    (NEWID(), 'Alice', 'Smith', 'alice.smith@example.com', '+1-555-0101', '1985-04-12', @now),
    (NEWID(), 'Bob', 'Johnson', 'bob.johnson@example.com', '+1-555-0102', '1979-09-23', @now),
    (NEWID(), 'Carol', 'Williams', 'carol.williams@example.com', '+1-555-0103', '1990-02-02', @now),
    (NEWID(), 'David', 'Brown', 'david.brown@example.com', '+1-555-0104', '1968-07-30', @now),
    (NEWID(), 'Eva', 'Davis', 'eva.davis@example.com', '+1-555-0105', '1995-11-11', @now);
END
GO

-- Add policies for the newly added customers
IF NOT EXISTS (SELECT 1 FROM Policies WHERE PolicyNumber = 'POL-2001')
BEGIN
    INSERT INTO Policies (Id, PolicyNumber, CustomerId, StartDate, EndDate, Premium, Status, CreatedAt)
    SELECT NEWID(), CONCAT('POL-', CAST(2000 + ROW_NUMBER() OVER (ORDER BY Email) AS VARCHAR(10))), Id, @now, DATEADD(year,1,@now), ROUND(500 + (ROW_NUMBER() OVER (ORDER BY Email) * 150),2), 'Active', @now
    FROM Customers WHERE Email IN ('alice.smith@example.com','bob.johnson@example.com','carol.williams@example.com','david.brown@example.com','eva.davis@example.com');
END
GO

-- Add claims (one or two per some policies)
IF NOT EXISTS (SELECT 1 FROM Claims WHERE ClaimNumber = 'CLM-2001')
BEGIN
    INSERT INTO Claims (Id, ClaimNumber, PolicyId, CustomerId, DateOfLoss, Amount, Status, CreatedAt)
    SELECT NEWID(), CONCAT('CLM-', CAST(2000 + ROW_NUMBER() OVER (ORDER BY p.PolicyNumber) AS VARCHAR(10))), p.Id, p.CustomerId, DATEADD(day, - (ROW_NUMBER() OVER (ORDER BY p.PolicyNumber) * 5), @now), ROUND(100 + (ROW_NUMBER() OVER (ORDER BY p.PolicyNumber) * 250),2), 'Submitted', @now
    FROM Policies p
    WHERE p.PolicyNumber LIKE 'POL-2%';
END
GO
