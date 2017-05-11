CREATE TABLE [dbo].[ProcessCycle]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [StartDateTime] DATETIME NOT NULL, 
    [StartedBy] NVARCHAR(MAX) NULL
)
