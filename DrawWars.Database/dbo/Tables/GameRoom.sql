CREATE TABLE [dbo].[GameRoom] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [Code]         VARCHAR (8)      NULL,
    [CreationDate] DATETIME         DEFAULT (getdate()) NULL,
    [SessionUuid]  UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

