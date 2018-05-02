/*********************************************************************************************
Процедура: 	GetAvailableClients

Описание:	Получение списка кодов клиентов с учетом прав пользователя

Параметры:
	@IdList	Строка с перечислением кодов клиентов
	@AllClients Признак выбора всех клиентов
	@UserId Пользователь
	
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableClients'
)
   DROP PROCEDURE dbo.GetAvailableClients
GO

CREATE PROCEDURE dbo.GetAvailableClients
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllClients BIT,	-- Признак выбора всех клиентов
	@UserId INT	-- Код пользователя
)
AS

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр клиентов
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(3001, @UserId)

-- Если права нет, то выборка всегда пустая
IF @permissionDistributionTypeId = 3
BEGIN
	-- Если указаны выбранные коды, то
	IF @AllClients = 0
		SELECT c.Id, co.Name
		FROM dbo.SplitIntIdList(@IdList) sc
		JOIN Client c on c.Id = sc.Id
		JOIN Contractor co on co.Id = c.Id AND co.DeletionDate is null
	ELSE
		SELECT c.Id, co.Name 
		FROM Client c
		JOIN Contractor co on co.Id = c.Id AND co.DeletionDate is null
END

GO

