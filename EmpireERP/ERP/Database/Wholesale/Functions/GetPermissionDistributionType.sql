/*********************************************************************************************
Функция: 	GetPermissionDistributionType

Описание:	Получение разрешения по праву

Параметры:
	@PermissionId	Код права
	@UserId	Код пользователя

*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetPermissionDistributionType'
)
   DROP FUNCTION dbo.GetPermissionDistributionType
GO

CREATE FUNCTION dbo.GetPermissionDistributionType
(
	@PermissionId SMALLINT,	-- Право
	@UserId INT -- Код пользователя
)
RETURNS TINYINT
AS
BEGIN
	RETURN (SELECT ISNULL(MAX(PermissionDistributionTypeId), 0)
	FROM [User] u
	join [UserRole] ur on ur.UserId = u.Id AND u.Id = @UserId
	join [PermissionDistribution] pd on pd.RoleId = ur.RoleId  AND pd.PermissionId = @PermissionId)
END

GO
