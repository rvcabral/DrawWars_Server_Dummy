CREATE TABLE [dbo].[Drawing] (
    [ID]          INT                IDENTITY (1, 1) NOT NULL,
    [Url]         NVARCHAR (2048)    NOT NULL,
    [DeviceID]    UNIQUEIDENTIFIER   NOT NULL,
    [SessionID]   UNIQUEIDENTIFIER   NOT NULL,
    [DateCreated] DATETIMEOFFSET (7) DEFAULT (sysdatetimeoffset()) NULL,
    CONSTRAINT [PK__Drawing] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Drawing_SessionID]
    ON [dbo].[Drawing]([SessionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Drawing_DeviceID]
    ON [dbo].[Drawing]([DeviceID] ASC);

