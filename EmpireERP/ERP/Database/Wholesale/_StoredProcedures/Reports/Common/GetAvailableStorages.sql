/*********************************************************************************************
Процедура: 	GetAvailableStorages

Описание:	Получение списка кодов МХ с учетом прав пользователя

Параметры:
	@IdList	Строка с перечислением кодов мест хранения
	@AllStorages Признак выбора всех МХ
	@UserId Пользователь
	@PermissionId Код права
	
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableStorages'
)
   DROP PROCEDURE dbo.GetAvailableStorages
GO

CREATE PROCEDURE dbo.GetAvailableStorages
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllStorages BIT,	-- Признак выбора всех мест хранения
	@UserId INT,	-- Код пользователя
	@PermissionId SMALLINT	-- Код права
)
AS

CREATE TABLE #VisibleStorages (
	Id TINYINT not null,
	NAME VARCHAR(200) not null,
	StorageTypeId TINYINT not null
)

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр МХ

-- Получаем право на просмотр МХ	
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(@PermissionId, @UserId)

IF @permissionDistributionTypeId = 2
	INSERT INTO #VisibleStorages (Id, Name, StorageTypeId)
	SELECT DISTINCT s.Id, s.Name, s.StorageTypeId
	FROM Storage s
	JOIN TeamStorage ts on ts.StorageId = s.Id	AND s.DeletionDate is null
	JOIN Team t on t.Id = ts.TeamId AND t.DeletionDate is null
	JOIN UserTeam ut on ut.TeamId = t.Id
	JOIN [User] u on u.Id = ut.UserId AND u.Id = @UserId
	GROUP BY s.Id, s.Name, s.StorageTypeId

ELSE IF @permissionDistributionTypeId = 3
	INSERT INTO #VisibleStorages (Id, Name,StorageTypeId)
	SELECT s.Id, s.Name, s.StorageTypeId 
	FROM Storage s 
	WHERE s.DeletionDate is null

-- Если указаны МХ, то выбираем их из видимых
IF @AllStorages = 0
	SELECT DISTINCT vs.Id, vs.Name, vs.StorageTypeId 
	FROM #VisibleStorages vs
	JOIN dbo.SplitIntIdList(@IdList) ss on ss.Id = vs.Id
	
ELSE	-- Иначе берем все видимые МХ
	SELECT Id, Name, StorageTypeId 
	FROM #VisibleStorages

DROP TABLE #VisibleStorages

GO