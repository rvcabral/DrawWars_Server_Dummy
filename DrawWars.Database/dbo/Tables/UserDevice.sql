CREATE TABLE [dbo].[UserDevice] (
    [UserId]     INT              NOT NULL,
    [DeviceUuid] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC, [DeviceUuid] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[DrawWarsUser] ([Id])
);

