USE [emd]
GO

/****** Object:  StoredProcedure [dbo].[LogError]    Script Date: 08/12/2010 11:54:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =========================================================
-- 08/12/2010		atanis		error logging for Panther
-- EXEC LogError '2-1-2010', 'test', 'test exception info'
-- SELECT TOP 100 * FROM error ORDER BY date DESC
-- =========================================================
CREATE PROCEDURE [dbo].[LogError]
	@date datetime,
	@application varchar(100),
	@exception varchar(MAX)		
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO error
	(
		[date],
		[application],
		[exception]
	)
	VALUES
	(
		@date,
		@application,
		@exception
	)
END

GO


