/*********************************************************************************************
���������: 	GetAvailableUsers

��������:	��������� ������ ����� ������������� � ������ ���� ������������

���������:
	@IdList	������ � ������������� ����� �������������
	@AllUsers ������� ������ ���� �������������
	@UserId ������������
	@PermissinId ��� �����
	
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
	@IdList VARCHAR(8000),	-- ������ ��������� ����� 
	@AllUsers BIT,	-- ������� ������ ���� ��������
	@UserId INT,	-- ��� ������������
	@PermissinId SMALLINT -- ��� �����
)
AS

-- ������ ������� ������������ �������������
CREATE TABLE #VisibleUsers (	
	Id INT not null,
	Name VARCHAR(100) not null
)	

DECLARE @permissionDistributionTypeId TINYINT	-- ���������� ����� �� �������� �������������
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(@PermissinId, @UserId)

IF @permissionDistributionTypeId = 1	-- ������ ����
	-- �.�. ����� ������ ������ ����
	INSERT INTO #VisibleUsers (Id, Name)
	SELECT Id, DisplayName 
	FROM [User]
	WHERE Id = @UserId

ELSE IF @permissionDistributionTypeId = 2	-- ��������� �����
	INSERT INTO #VisibleUsers (Id, Name)
	SELECT DISTINCT u.Id, u.DisplayName
	FROM UserTeam ut
	JOIN Team t on t.Id = ut.TeamId
	JOIN UserTeam ut2 on ut2.TeamId = ut.TeamId AND ut2.UserId = @UserId
	JOIN [User] u on u.Id = ut.UserId

ELSE IF @permissionDistributionTypeId = 3	-- ����� "���"
	INSERT INTO #VisibleUsers
	SELECT Id, DisplayName 
	FROM [User]

-- ���� ������� ����, ��
IF @AllUsers = 0
	SELECT DISTINCT vu.Id, vu.Name
	FROM dbo.SplitIntIdList(@IdList) su
	JOIN #VisibleUsers vu on su.Id = vu.Id

ELSE
	-- ����� ����� ��� �������
	SELECT Id, Name 
	FROM #VisibleUsers

DROP TABLE #VisibleUsers
GO


