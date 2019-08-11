CREATE TABLE [dbo].[Player] (
    [Id]         INT              IDENTITY (1, 1) NOT NULL,
    [GameRoomId] INT              NULL,
    [Name]       VARCHAR (32)     NULL,
    [DeviceUuid] UNIQUEIDENTIFIER NULL,
    [PlayerUuid] UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([GameRoomId]) REFERENCES [dbo].[GameRoom] ([Id])
);

