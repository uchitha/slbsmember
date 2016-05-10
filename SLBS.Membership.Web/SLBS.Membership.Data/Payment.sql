CREATE TABLE [dbo].[Payment]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [MemberId] INT NOT NULL, 
    [ProcessId] INT NOT NULL, 
    [PaymentStatus] NVARCHAR(50) NULL, 
    [PaymentUntilYear] NVARCHAR(50) NULL, 
    [PaymentUntilMonth] NVARCHAR(50) NULL, 
    [IsEmailSent] BIT NULL, 
    CONSTRAINT [FK_Payment_Member] FOREIGN KEY (MemberId) REFERENCES [Member]([Id]), 
    CONSTRAINT [FK_Payment_Process] FOREIGN KEY (ProcessId) REFERENCES [ProcessCycle]([Id])
)
