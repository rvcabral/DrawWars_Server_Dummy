CREATE TABLE [dbo].[PlayerScore] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [GameRoomId] INT NULL,
    [PlayerId]   INT NULL,
    [Score]      INT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([GameRoomId]) REFERENCES [dbo].[GameRoom] ([Id]),
    FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Player] ([Id]),
    UNIQUE NONCLUSTERED ([GameRoomId] ASC, [PlayerId] ASC)
);

