/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--Add Required Client Push Status Values

IF NOT EXISTS (SELECT * FROM ClientPushStatus WHERE Id = 1)
INSERT INTO ClientPushStatus(Id, StatusName)
VALUES (1, 'Active')

IF NOT EXISTS (SELECT * FROM ClientPushStatus WHERE Id = 2)
INSERT INTO ClientPushStatus(Id, StatusName)
VALUES (2, 'Inactive')

IF NOT EXISTS (SELECT * FROM ClientPushStatus WHERE Id = 3)
INSERT INTO ClientPushStatus(Id, StatusName)
VALUES (3, 'Expired')

IF NOT EXISTS (SELECT * FROM ClientPushStatus WHERE Id = 4)
INSERT INTO ClientPushStatus(Id, StatusName)
VALUES (4, 'Unsubscribed')

--Add Required Push Protocol values

IF NOT EXISTS (SELECT * FROM PushProtocol WHERE Id = 1)
INSERT INTO PushProtocol(Id, ProtocolName)
VALUES (1, 'WebPush')

IF NOT EXISTS (SELECT * FROM PushProtocol WHERE Id = 2)
INSERT INTO PushProtocol(Id, ProtocolName)
VALUES (2, 'SafariPushNotificationService')

--Add Apprentice App Application Row
CREATE TABLE #TempApplication
(
    [Id] INT , 
    [Name] NVARCHAR(50), 
    [ApplicationType] NVARCHAR(50), 
    [NotificationsEnabled] BIT
)

IF NOT EXISTS (SELECT * FROM Applications WHERE Id = 1)
INSERT INTO Applications ([Id], [Name], [ApplicationType], [NotificationsEnabled])
VALUES (1, 'ApprenticeApp', 'PWA', 1)