/*********************************************************************************************
���������: 	GetAvailableClients

��������:	��������� ������ ����� �������� � ������ ���� ������������

���������:
	@IdList	������ � ������������� ����� ��������
	@AllClients ������� ������ ���� ��������
	@UserId ������������
	
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
	@IdList VARCHAR(8000),	-- ������ ��������� ����� 
	@AllClients BIT,	-- ������� ������ ���� ��������
	@UserId INT	-- ��� ������������
)
AS

DECLARE @permissionDistributionTypeId TINYINT	-- ���������� ����� �� �������� ��������
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(3001, @UserId)

-- ���� ����� ���, �� ������� ������ ������
IF @permissionDistributionTypeId = 3
BEGIN
	-- ���� ������� ��������� ����, ��
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

