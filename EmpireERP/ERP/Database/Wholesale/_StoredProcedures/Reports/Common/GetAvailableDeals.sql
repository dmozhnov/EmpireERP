/*********************************************************************************************
Процедура: 	GetAvailableDeals

Описание:	Получение списка кодов видимых сделок

Параметры:
	@UserId Пользователь
	@PermissionId Код права для определения видимости сделок
	
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableDeals'
)
   DROP PROCEDURE dbo.GetAvailableDeals
GO

CREATE PROCEDURE dbo.GetAvailableDeals
(
	@UserId INT,	-- Код пользователя
	@PermissionId SMALLINT	-- Код права
)
AS

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр сделок
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(@PermissionId, @UserId)

IF @permissionDistributionTypeId = 3	-- "Все"
	SELECT Id 
	FROM Deal

ELSE IF @permissionDistributionTypeId = 2	-- "командные"
	SELECT DISTINCT d.Id 
	FROM Deal d
	JOIN TeamDeal td on td.DealId = d.Id
	JOIN UserTeam ut on ut.TeamId = td.TeamId AND ut.UserId = @UserId

ELSE IF @permissionDistributionTypeId = 1	-- "только свои"
	SELECT DISTINCT d.Id 
	FROM Deal d
	JOIN TeamDeal td on td.DealId = d.Id AND d.CuratorId = @UserId
	JOIN UserTeam ut on ut.TeamId = td.TeamId AND ut.UserId = @UserId

GO