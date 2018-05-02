/*********************************************************************************************
Процедура: 	GetAvailableUsers

Описание:	Получение списка кодов пользователей с учетом прав пользователя

Параметры:
	@IdList	Строка с перечислением кодов пользователей
	@AllUsers Признак выбора всех пользователей
	@UserId Пользователь
	@PermissinId Код права
	
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableUsers'
)
   DROP PROCEDURE dbo.GetAvailableUsers
GO

CREATE PROCEDURE dbo.GetAvailableUsers
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllUsers BIT,	-- Признак выбора всех клиентов
	@UserId INT,	-- Код пользователя
	@PermissinId SMALLINT -- Код права
)
AS

-- список видимых пользователю пользователей
CREATE TABLE #VisibleUsers (	
	Id INT not null,
	Name VARCHAR(100) not null
)	

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр пользователей
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(@PermissinId, @UserId)

IF @permissionDistributionTypeId = 1	-- только свои
	-- т.е. может видеть только себя
	INSERT INTO #VisibleUsers (Id, Name)
	SELECT Id, DisplayName 
	FROM [User]
	WHERE Id = @UserId

ELSE IF @permissionDistributionTypeId = 2	-- командное право
	INSERT INTO #VisibleUsers (Id, Name)
	SELECT DISTINCT u.Id, u.DisplayName
	FROM UserTeam ut
	JOIN Team t on t.Id = ut.TeamId
	JOIN UserTeam ut2 on ut2.TeamId = ut.TeamId AND ut2.UserId = @UserId
	JOIN [User] u on u.Id = ut.UserId

ELSE IF @permissionDistributionTypeId = 3	-- право "Все"
	INSERT INTO #VisibleUsers
	SELECT Id, DisplayName 
	FROM [User]

-- Если указаны коды, то
IF @AllUsers = 0
	SELECT DISTINCT vu.Id, vu.Name
	FROM dbo.SplitIntIdList(@IdList) su
	JOIN #VisibleUsers vu on su.Id = vu.Id

ELSE
	-- иначе берем все видимые
	SELECT Id, Name 
	FROM #VisibleUsers

DROP TABLE #VisibleUsers
GO


