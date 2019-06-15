CREATE PROCEDURE [dbo].[DrawingGetRandom]
AS
BEGIN

	SELECT TOP 10 *
	FROM [dbo].[Drawing]
	ORDER BY NEWID()

END