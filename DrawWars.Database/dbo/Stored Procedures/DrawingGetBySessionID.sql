
CREATE PROCEDURE [dbo].[DrawingGetBySessionID]
	@sessionID UNIQUEIDENTIFIER
,	@page int
,	@pageSize int
AS
BEGIN

	SELECT *
	FROM [dbo].[Drawing]
	WHERE [dbo].[Drawing].[SessionID] = @sessionID
	ORDER BY [dbo].[Drawing].[DateCreated] DESC
	OFFSET (@page * @pageSize) ROWS
	FETCH NEXT @pageSize ROWS ONLY

END