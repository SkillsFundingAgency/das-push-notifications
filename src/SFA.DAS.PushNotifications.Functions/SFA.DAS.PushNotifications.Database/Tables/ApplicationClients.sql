CREATE TABLE [dbo].[ApplicationClients]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL, 
    [PushProtocol]INT NULL DEFAULT 1, 
    [Status]INT NOT NULL, 
    [EndPoint] NVARCHAR(255) NOT NULL, 
    [SubscriptionPublicKey] NVARCHAR(1000) NULL, 
    [SubscriptionAuthenticationSecret] NVARCHAR(1000) NULL, 
    [Handle] NVARCHAR(50) NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [ExpiryDate] DATETIME NULL, 
    [ApplicationId]INT NOT NULL,
    CONSTRAINT [FK_ApplicationClient_ToTable] FOREIGN KEY ([ApplicationId]) REFERENCES [Application]([Id]),
    CONSTRAINT [UK_ApplicationClient_EndPoint] UNIQUE NONCLUSTERED ([EndPoint] ASC)
)