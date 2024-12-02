CREATE TABLE [dbo].[ClientNotification]
(
	[Id] [uniqueidentifier] NOT NULL,
	[Status] [tinyint] NULL,
	[CreatedTime] [datetime] NULL,
	[FailureReason] [nvarchar](2000) NULL,
	[TimeToSend] [datetime] NULL,
	[TimeSent] [datetime] NULL,
	[ApplicationClientId] [int] NULL,
	[Payload] [nvarchar](max) NULL
)