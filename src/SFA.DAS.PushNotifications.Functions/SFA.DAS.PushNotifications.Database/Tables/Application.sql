CREATE TABLE [dbo].[Application]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [ApplicationType] NVARCHAR(50) NULL, 
    [NotificationsEnabled] BIT NOT NULL
)