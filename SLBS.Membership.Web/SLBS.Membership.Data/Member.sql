﻿CREATE TABLE [dbo].[Member]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [MemberId] NVARCHAR(50) NOT NULL, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Email] NVARCHAR(MAX) NULL, 
    [Phone] NVARCHAR(MAX) NULL 
)
