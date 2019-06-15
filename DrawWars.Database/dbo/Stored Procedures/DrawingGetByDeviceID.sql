
CREATE PROCEDURE [dbo].[DrawingGetByDeviceID]
	@deviceID UNIQUEIDENTIFIER
,	@page int
,	@pageSize int
AS
BEGIN

	SELECT *
	FROM [dbo].[Drawing]
	WHERE [dbo].[Drawing].[DeviceID] = @deviceID
	ORDER BY [dbo].[Drawing].[DateCreated] DESC
	OFFSET (@page * @pageSize) ROWS
	FETCH NEXT @pageSize ROWS ONLY

END