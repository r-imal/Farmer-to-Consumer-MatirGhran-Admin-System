USE [FarmerToConsumerDb];
GO

-- Fresh/admin schema aligned with current sir-style code.
-- AgentProfile is removed. Agent is a real table.

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Users] PRIMARY KEY,
        [Name] nvarchar(250) NOT NULL,
        [Email] nvarchar(250) NOT NULL,
        [PasswordHash] nvarchar(50) NOT NULL,
        [Role] nvarchar(50) NOT NULL,
        [Phone] nvarchar(50) NOT NULL CONSTRAINT [DF_Users_Phone] DEFAULT (''),
        [Village] nvarchar(120) NULL,
        [District] nvarchar(120) NULL,
        [Address] nvarchar(300) NULL,
        [CreatedAt] datetime2 NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (sysdatetime())
    );
END
GO

DECLARE @DropConstraints nvarchar(max) = N'';

SELECT @DropConstraints +=
    N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id)) + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) +
    N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(13) + CHAR(10)
FROM sys.foreign_keys fk
WHERE OBJECT_SCHEMA_NAME(fk.parent_object_id) = 'dbo'
  AND (
      OBJECT_NAME(fk.parent_object_id) IN (
          'AgentProfile', 'Agent', 'AgentDeliverymanAssign', 'Farmer', 'Product', 'ProductStock',
          'CustomerOrder', 'CustomerOrderItems', 'CustomerPayment', 'AgentPayment', 'Reviews'
      )
      OR OBJECT_NAME(fk.referenced_object_id) IN (
          'AgentProfile', 'Agent', 'AgentDeliverymanAssign', 'Farmer', 'Product', 'ProductStock',
          'CustomerOrder', 'CustomerOrderItems', 'CustomerPayment', 'AgentPayment', 'Reviews'
      )
  );

IF LEN(@DropConstraints) > 0
BEGIN
    EXEC sp_executesql @DropConstraints;
END
GO

DROP TABLE IF EXISTS [dbo].[CustomerPayment];
DROP TABLE IF EXISTS [dbo].[CustomerOrderItems];
DROP TABLE IF EXISTS [dbo].[Reviews];
DROP TABLE IF EXISTS [dbo].[ProductStock];
DROP TABLE IF EXISTS [dbo].[CustomerOrder];
DROP TABLE IF EXISTS [dbo].[AgentPayment];
DROP TABLE IF EXISTS [dbo].[Product];
DROP TABLE IF EXISTS [dbo].[Farmer];
DROP TABLE IF EXISTS [dbo].[AgentDeliverymanAssign];
DROP TABLE IF EXISTS [dbo].[AgentProfile];
DROP TABLE IF EXISTS [dbo].[Agent];
GO

CREATE TABLE [dbo].[Agent]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Agent] PRIMARY KEY,
    [UserInfoID] int NULL,
    [Name] nvarchar(150) NOT NULL,
    [Phone] nvarchar(50) NOT NULL,
    [Area] nvarchar(150) NOT NULL,
    [CommissionRate] decimal(18,2) NOT NULL CONSTRAINT [DF_Agent_CommissionRate] DEFAULT (5)
);
GO

CREATE TABLE [dbo].[Farmer]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Farmer] PRIMARY KEY,
    [AgentId] int NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Village] nvarchar(150) NOT NULL,
    [District] nvarchar(150) NOT NULL,
    [Contact] nvarchar(11) NOT NULL
);
GO

CREATE TABLE [dbo].[Product]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Product] PRIMARY KEY,
    [FarmerId] int NOT NULL,
    [AddedByAgent] int NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Category] nvarchar(150) NOT NULL
);
GO

CREATE TABLE [dbo].[ProductStock]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProductStock] PRIMARY KEY,
    [ProductId] int NOT NULL,
    [Price] int NOT NULL,
    [Quantity] int NOT NULL
);
GO

CREATE TABLE [dbo].[CustomerOrder]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomerOrder] PRIMARY KEY,
    [CustomerId] int NOT NULL,
    [AgentId] int NULL,
    [TotalAmount] int NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [DeliveryAddress] nvarchar(300) NOT NULL,
    [DeliveryStatus] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [dbo].[CustomerOrderItems]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomerOrderItems] PRIMARY KEY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [Price] int NOT NULL
);
GO

CREATE TABLE [dbo].[CustomerPayment]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomerPayment] PRIMARY KEY,
    [OrderId] int NOT NULL,
    [Amount] int NOT NULL,
    [Method] nvarchar(50) NOT NULL CONSTRAINT [DF_CustomerPayment_Method] DEFAULT ('CashOnDelivery'),
    [Status] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [dbo].[AgentPayment]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AgentPayment] PRIMARY KEY,
    [AgentId] int NOT NULL,
    [Amount] int NOT NULL,
    [PaidAt] datetime2 NOT NULL CONSTRAINT [DF_AgentPayment_PaidAt] DEFAULT (sysdatetime())
);
GO

CREATE TABLE [dbo].[Reviews]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Reviews] PRIMARY KEY,
    [ProductId] int NOT NULL,
    [CustomerId] int NOT NULL,
    [Rating] int NOT NULL,
    [Comment] nvarchar(500) NOT NULL
);
GO

CREATE TABLE [dbo].[AgentDeliverymanAssign]
(
    [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AgentDeliverymanAssign] PRIMARY KEY,
    [Name] nvarchar(120) NOT NULL,
    [Phone] nvarchar(50) NOT NULL CONSTRAINT [DF_AgentDeliverymanAssign_Phone] DEFAULT (''),
    [AgentUserId] int NOT NULL,
    [AgentWorkVillage] nvarchar(120) NOT NULL,
    [MonthlyDeliveredAmount] decimal(18,2) NOT NULL
);
GO

ALTER TABLE [dbo].[Agent]
ADD CONSTRAINT [FK_Agent_Users_UserInfoID]
FOREIGN KEY ([UserInfoID]) REFERENCES [dbo].[Users]([ID]);
GO

ALTER TABLE [dbo].[Farmer]
ADD CONSTRAINT [FK_Farmer_Agent_AgentId]
FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Agent]([ID]);
GO

ALTER TABLE [dbo].[Product]
ADD CONSTRAINT [FK_Product_Farmer_FarmerId]
FOREIGN KEY ([FarmerId]) REFERENCES [dbo].[Farmer]([ID]);
GO

ALTER TABLE [dbo].[Product]
ADD CONSTRAINT [FK_Product_Agent_AddedByAgent]
FOREIGN KEY ([AddedByAgent]) REFERENCES [dbo].[Agent]([ID]);
GO

ALTER TABLE [dbo].[ProductStock]
ADD CONSTRAINT [FK_ProductStock_Product_ProductId]
FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([ID]);
GO

ALTER TABLE [dbo].[CustomerOrder]
ADD CONSTRAINT [FK_CustomerOrder_Users_CustomerId]
FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users]([ID]);
GO

ALTER TABLE [dbo].[CustomerOrder]
ADD CONSTRAINT [FK_CustomerOrder_Agent_AgentId]
FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Agent]([ID]);
GO

ALTER TABLE [dbo].[CustomerOrderItems]
ADD CONSTRAINT [FK_CustomerOrderItems_CustomerOrder_OrderId]
FOREIGN KEY ([OrderId]) REFERENCES [dbo].[CustomerOrder]([ID]);
GO

ALTER TABLE [dbo].[CustomerOrderItems]
ADD CONSTRAINT [FK_CustomerOrderItems_Product_ProductId]
FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([ID]);
GO

ALTER TABLE [dbo].[CustomerPayment]
ADD CONSTRAINT [FK_CustomerPayment_CustomerOrder_OrderId]
FOREIGN KEY ([OrderId]) REFERENCES [dbo].[CustomerOrder]([ID]);
GO

ALTER TABLE [dbo].[AgentPayment]
ADD CONSTRAINT [FK_AgentPayment_Users_AgentId]
FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Users]([ID]);
GO

ALTER TABLE [dbo].[Reviews]
ADD CONSTRAINT [FK_Reviews_Product_ProductId]
FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([ID]);
GO

ALTER TABLE [dbo].[Reviews]
ADD CONSTRAINT [FK_Reviews_Users_CustomerId]
FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users]([ID]);
GO

ALTER TABLE [dbo].[AgentDeliverymanAssign]
ADD CONSTRAINT [FK_AgentDeliverymanAssign_Agent_AgentUserId]
FOREIGN KEY ([AgentUserId]) REFERENCES [dbo].[Agent]([ID]);
GO

PRINT 'Admin schema updated successfully.';
GO
