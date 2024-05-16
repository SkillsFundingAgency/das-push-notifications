CREATE TABLE [dbo].[Applications]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [ApplicationType] NVARCHAR(50) NULL, 
    [NotificationsEnabled] BIT NOT NULL DEFAULT 1
)