CREATE PROCEDURE [dbo].[DrawingInsert]
	@url NVARCHAR(2048)
,	@deviceID UNIQUEIDENTIFIER
,	@sessionID UNIQUEIDENTIFIER
AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO [dbo].[Drawing] ([Url], [DeviceID], [SessionID])
	OUTPUT
		[inserted].[ID]
	,	[inserted].[Url]
	,	[inserted].[DeviceID]
	,	[inserted].[SessionID]
	,	[inserted].[DateCreated]
	VALUES (
		@url
	,	@deviceID
	,	@sessionID
	)

END