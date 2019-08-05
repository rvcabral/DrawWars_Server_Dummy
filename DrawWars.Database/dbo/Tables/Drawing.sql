CREATE TABLE [dbo].[Drawing] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [GameRoomId] INT             NULL,
    [PlayerId]   INT             NULL,
    [ThemeId]    INT             NULL,
    [Url]        NVARCHAR (2048) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([GameRoomId]) REFERENCES [dbo].[GameRoom] ([Id]),
    FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Player] ([Id]),
    FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[Theme] ([Id])
);




GO



GO


