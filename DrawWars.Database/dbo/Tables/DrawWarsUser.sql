CREATE TABLE [dbo].[DrawWarsUser] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Username] VARCHAR (256)  NULL,
    [PassHash] VARCHAR (1024) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

