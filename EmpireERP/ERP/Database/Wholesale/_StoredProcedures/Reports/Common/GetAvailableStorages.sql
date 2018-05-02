/*********************************************************************************************
���������: 	GetAvailableStorages

��������:	��������� ������ ����� �� � ������ ���� ������������

���������:
	@IdList	������ � ������������� ����� ���� ��������
	@AllStorages ������� ������ ���� ��
	@UserId ������������
	@PermissionId ��� �����
	
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
	@IdList VARCHAR(8000),	-- ������ ��������� ����� 
	@AllStorages BIT,	-- ������� ������ ���� ���� ��������
	@UserId INT,	-- ��� ������������
	@PermissionId SMALLINT	-- ��� �����
)
AS

CREATE TABLE #VisibleStorages (
	Id TINYINT not null,
	NAME VARCHAR(200) not null,
	StorageTypeId TINYINT not null
)

DECLARE @permissionDistributionTypeId TINYINT	-- ���������� ����� �� �������� ��

-- �������� ����� �� �������� ��	
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

-- ���� ������� ��, �� �������� �� �� �������
IF @AllStorages = 0
	SELECT DISTINCT vs.Id, vs.Name, vs.StorageTypeId 
	FROM #VisibleStorages vs
	JOIN dbo.SplitIntIdList(@IdList) ss on ss.Id = vs.Id
	
ELSE	-- ����� ����� ��� ������� ��
	SELECT Id, Name, StorageTypeId 
	FROM #VisibleStorages

DROP TABLE #VisibleStorages

GO