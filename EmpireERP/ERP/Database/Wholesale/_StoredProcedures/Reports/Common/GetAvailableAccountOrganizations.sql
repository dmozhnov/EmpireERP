/*********************************************************************************************
Процедура: 	GetAvailableAccountOrganizations

Описание:	Получение списка кодов организаций аккаунта с учетом прав пользователя

Параметры:
	@IdList	Строка с перечислением кодов организаций аккаунта
	@AllAccountOrganizations Признак выбора всех организаций аккаунта
	
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableAccountOrganizations'
)
   DROP PROCEDURE dbo.GetAvailableAccountOrganizations
GO

CREATE PROCEDURE dbo.GetAvailableAccountOrganizations
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllAccountOrganizations BIT	-- Признак выбора всех организаций аккаунта
)
AS

IF @AllAccountOrganizations = 0
	SELECT o.Id, o. ShortName
	FROM dbo.SplitIntIdList(@IdList) sil
	JOIN Organization o on sil.Id = o.Id AND o.DeletionDate is null
	JOIN AccountOrganization ao on ao.Id = o.Id
ELSE
	SELECT ao.Id, o.ShortName 
	FROM AccountOrganization ao
	JOIN Organization o on o.Id = ao.Id AND o.DeletionDate is null

GO
