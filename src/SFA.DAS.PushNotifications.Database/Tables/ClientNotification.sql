CREATE TABLE [dbo].[ClientNotification]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [MessageId] UNIQUEIDENTIFIER NULL, 
    [Status] TINYINT NULL, 
    [FailureReason] NVARCHAR(2000) NULL, 
    [TimeToSend] DATETIME NULL, 
    [StatusTime] DATETIME NULL
)