/*********************************************************************************************
���������: 	GetAvailableDeals

��������:	��������� ������ ����� ������� ������

���������:
	@UserId ������������
	@PermissionId ��� ����� ��� ����������� ��������� ������
	
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
	@UserId INT,	-- ��� ������������
	@PermissionId SMALLINT	-- ��� �����
)
AS

DECLARE @permissionDistributionTypeId TINYINT	-- ���������� ����� �� �������� ������
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(@PermissionId, @UserId)

IF @permissionDistributionTypeId = 3	-- "���"
	SELECT Id 
	FROM Deal

ELSE IF @permissionDistributionTypeId = 2	-- "���������"
	SELECT DISTINCT d.Id 
	FROM Deal d
	JOIN TeamDeal td on td.DealId = d.Id
	JOIN UserTeam ut on ut.TeamId = td.TeamId AND ut.UserId = @UserId

ELSE IF @permissionDistributionTypeId = 1	-- "������ ����"
	SELECT DISTINCT d.Id 
	FROM Deal d
	JOIN TeamDeal td on td.DealId = d.Id AND d.CuratorId = @UserId
	JOIN UserTeam ut on ut.TeamId = td.TeamId AND ut.UserId = @UserId

GO