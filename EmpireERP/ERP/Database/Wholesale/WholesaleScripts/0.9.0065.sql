/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.65

  ��� ������:
	* ���������� ������ � �������� ������ �� ��������
		
---------------------------------------------------------------------------------------*/
--SET NOEXEC OFF	-- ��������� ������ ������� � ������ ����������� ����������
SET DATEFORMAT DMY
SET NOCOUNT ON
SET ARITHABORT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

DECLARE @PreviousVersion varchar(15),	-- ����� ���������� ������
		@CurrentVersion varchar(15),	-- ����� ������� ������ ���� ������
		@NewVersion varchar(15),		-- ����� ����� ������
		@DataBaseName varchar(256),		-- ������� ���� ������
		@CurrentDate nvarchar(10),		-- ������� ����
		@CurrentTime nvarchar(10),		-- ������� �����
		@BackupTarget nvarchar(100)		-- ���� ������ ����� ���� ������

SET @PreviousVersion = '0.9.64' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.65'			-- ����� ����� ������

SELECT @CurrentVersion = DataBaseVersion FROM Setting
IF @@ERROR <> 0
BEGIN
	PRINT '�������� ���� ������'
END
ELSE
BEGIN
	-- ������� ����� ���� ������
	-- �������� ������� ����
	SET @CurrentDate = CONVERT(nvarchar(20), GETDATE(), 104)	--	dd.mm.yyyy
	SET @CurrentTime = REPLACE(CONVERT(nvarchar(20), GETDATE(), 108) , ':', '.') --	hh:mm:ss
	SET @DataBaseName = DB_NAME()

	SET @BackupTarget = N'D:\Bizpulse\Backup\Update\' + @DataBaseName + '(' + CAST(@CurrentVersion as nvarchar(20)) + ') ' + 
		@CurrentDate + ' ' + @CurrentTime + N'.bak'

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = N'���������� ������', NOFORMAT

	IF @@ERROR <> 0
	BEGIN
		PRINT '������ �������� backup''�. ����������� ���������� ����������.'
	END
	ELSE
		BEGIN

		IF (@CurrentVersion <> @PreviousVersion)
		BEGIN
			PRINT '�������� ���� ������ ' + @DataBaseName + ' �� ������ ' + @NewVersion + 
				' ����� ������ �� ������  ' + @PreviousVersion +
				'. ������� ������: ' + @CurrentVersion
		END
		ELSE
		BEGIN
			--�������� ����������
			BEGIN TRAN

			--��������� ������ ���� ������
			UPDATE Setting 
			SET DataBaseVersion = @NewVersion		
		END
	END
END
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ��������� ������ ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAccountingPriceForArticlesOnStoragesByDate'
)
   DROP PROCEDURE dbo.GetAccountingPriceForArticlesOnStoragesByDate
GO

CREATE PROCEDURE GetAccountingPriceForArticlesOnStoragesByDate(
	@Date DATETIME	-- ����, �� ������� ���������� �������� ��
) AS

-- ��������������� ������� ��� ������������ ������� �� ��� ������� �� ��
create table #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination(
	ArticleId INT not null,
	StorageId SMALLINT not null,
	AccountingPrice DECIMAL(18,2) not null,
	RowNumber INT not null
)

-- �������� �� ��� ������� � ��������� �� �� ���� ���������� � �������� � �������� �������(�.�. ��������� ���������� � �������� ����� ������)
INSERT INTO #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination(ArticleId,StorageId,AccountingPrice,RowNumber)
SELECT aapi.ArticleId, aapi.StorageId, aapi.AccountingPrice, 
	ROW_NUMBER() OVER(PARTITION BY aapi.ArticleId, aapi.StorageId ORDER BY aapi.StartDate DESC )
FROM ArticleAccountingPriceIndicator aapi
	join #ArticleAccountingPriceForActiclesOnStoragesDetermination d on d.ArticleId = aapi.ArticleId AND d.StorageId = aapi.StorageId AND 
	(aapi.StartDate <= @Date AND (aapi.EndDate > @Date OR aapi.EndDate is null))

-- ��������� ������� ������� ���������� ��
UPDATE #ArticleAccountingPriceForActiclesOnStoragesDetermination
SET AccountingPrice = api.AccountingPrice
FROM #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination api
WHERE api.ArticleId = #ArticleAccountingPriceForActiclesOnStoragesDetermination.ArticleId AND
	api.StorageId = #ArticleAccountingPriceForActiclesOnStoragesDetermination.StorageId AND
	api.RowNumber = 1	-- ����� ��������� �� �� ���� ���������� � ��������

drop table #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination	

GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
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
	@IdList VARCHAR(8000),	-- ������ ��������� ����� 
	@AllAccountOrganizations BIT	-- ������� ������ ���� ����������� ��������
)
AS

IF @AllAccountOrganizations = 0
	SELECT o.Id, o. ShortName
	FROM dbo.SplitIntIdList(@IdList) sil
		join Organization o on sil.Id = o.Id AND o.DeletionDate is null
		join AccountOrganization ao on ao.Id = o.Id
ELSE
	SELECT ao.Id, o.ShortName 
	FROM AccountOrganization ao
		join Organization o on o.Id = ao.Id AND o.DeletionDate is null

GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableArticleGroups'
)
   DROP PROCEDURE dbo.GetAvailableArticleGroups
GO

CREATE PROCEDURE dbo.GetAvailableArticleGroups
(
	@IdList VARCHAR(8000),	-- ������ ��������� ����� 
	@AllArticleGroups BIT	-- ������� ������ ���� ����� �������
)
AS

IF @AllArticleGroups = 0 
	SELECT ag.Id, ag.Name
	FROM dbo.SplitIntIdList(@IdList) sil
		join ArticleGroup ag on ag.Id = sil.Id 
ELSE
	SELECT Id, Name 
	FROM ArticleGroup
GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

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
		join Client c on c.Id = sc.Id
		join Contractor co on co.Id = c.Id AND co.DeletionDate is null
	ELSE
		SELECT c.Id, co.Name 
		FROM Client c
			join Contractor co on co.Id = c.Id AND co.DeletionDate is null
END

GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

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
	SELECT Id FROM Deal
ELSE IF @permissionDistributionTypeId = 2	-- "���������"
	SELECT DISTINCT d.Id 
	FROM Deal d
		join TeamDeal td on td.DealId = d.Id
		join UserTeam ut on ut.TeamId = td.TeamId AND ut.UserId = @UserId
 ELSE IF @permissionDistributionTypeId = 1	-- "������ ����"
	SELECT DISTINCT d.Id 
	FROM Deal d
		join TeamDeal td on td.DealId = d.Id AND d.CuratorId = @UserId
		join UserTeam ut on ut.TeamId = td.TeamId AND ut.UserId = @UserId

GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

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
		join TeamStorage ts on ts.StorageId = s.Id	AND s.DeletionDate is null
		join Team t on t.Id = ts.TeamId AND t.DeletionDate is null
		join UserTeam ut on ut.TeamId = t.Id
		join [User] u on u.Id = ut.UserId AND u.Id = @UserId
	GROUP BY s.Id, s.Name, s.StorageTypeId
ELSE IF @permissionDistributionTypeId = 3
	INSERT INTO #VisibleStorages (Id, Name,StorageTypeId)
	SELECT s.Id, s.Name, s.StorageTypeId FROM Storage s WHERE s.DeletionDate is null

-- ���� ������� ��, �� �������� �� �� �������
IF @AllStorages = 0
	SELECT vs.Id, vs.Name, vs.StorageTypeId 
	FROM #VisibleStorages vs
		join dbo.SplitIntIdList(@IdList) ss on ss.Id = vs.Id
ELSE	-- ����� ����� ��� ������� ��
	SELECT Id, Name, StorageTypeId 
	FROM #VisibleStorages

DROP TABLE #VisibleStorages

GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

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

IF @permissionDistributionTypeId = 2	-- ��������� �����
	INSERT INTO #VisibleUsers (Id, Name)
	SELECT DISTINCT u.Id, u.DisplayName
	FROM UserTeam ut
		join Team t on t.Id = ut.TeamId
		join UserTeam ut2 on ut2.TeamId = ut.TeamId AND ut2.UserId = @UserId
		join [User] u on u.Id = ut.UserId
ELSE IF @permissionDistributionTypeId = 3	-- ����� "���"
	INSERT INTO #VisibleUsers
	SELECT Id, DisplayName FROM [User]
-- ���� ������� ����, ��
IF @AllUsers = 0
	SELECT vu.Id, vu.Name
	FROM dbo.SplitIntIdList(@IdList) su
		join #VisibleUsers vu on su.Id = vu.Id
ELSE
	-- ����� ����� ��� �������
	SELECT Id, Name FROM #VisibleUsers

DROP TABLE #VisibleUsers
GO

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'Report0002'
)
   DROP PROCEDURE Report0002
GO

CREATE PROCEDURE Report0002
(
	@StartDate DATETIME,	-- ���� ������ ���������
	@EndDate DATETIME,	-- ���� ���������� ���������
	@IsShippedExpenditureWaybills BIT,	-- ������� ������� ��������� � ��������� �������(����� �����������)
	@DevideByBatch BIT, -- ������� ���������� ������
	@GetArticleAvailability BIT, -- ������� ������������� ���������� ������� �� ��
	@InAccountingPrice BIT,	-- ������� ������ ��
	
	@ConsiderReturnFromClient BIT,	-- �������, ����� �� ��������� ��������
	@ConsiderReturnFromClientByDate BIT, -- ������� ����, ��� ����� ��������� �������� �� ���������� ��������� ���. ����� �� ��������� ���������.

	@StorageIdList VARCHAR(8000),	-- ������ ����� ��
	@AllStorages BIT,	-- ������� ������ ���� ��
	@ArticleGroupIdList VARCHAR(8000),	--  ������ ����� ����� �������
	@AllArticleGroups BIT,	-- ������� ������ ���� ����� �������
	@ClientIdList VARCHAR(8000),	--  ������ ����� ��������
	@AllClients BIT,	-- ������� ������ ���� ��������
	@UserIdList VARCHAR(8000),	--  ������ ����� ����� �������������
	@AllUsers BIT,	-- ������� ������ ���� �������������
	@AccountOrganizationIdList VARCHAR(8000),	--  ������ ����� ����������� ��������
	@AllAccountOrganizations BIT,	-- ������� ������ ���� ����������� ��������
	@UserId INT	-- ��� ������������, ������������ �����
)
AS

SET NOCOUNT ON

-- �������� ������� �������
CREATE TABLE #ResultFlatTable(
	-- �������� ������ � ������� ����������
	ArticleId INT not null,
	ArticleNumber VARCHAR(30) not null,
	ArticleName VARCHAR(200) not null,
	BatchId UNIQUEIDENTIFIER not null,
	BatchNumber VARCHAR(25) not null,
	BatchDate DateTime not null,
	Count DECIMAL(18, 6) not null,
	PackSize DECIMAL(12, 6) null,
	CountryName VARCHAR(200) not null,
	CustomsDeclarationNumber VARCHAR(33) not null,

	-- ��
	AccountingPriceSum DECIMAL (18, 2) not null,
	-- ��
	PurchaseCostSum DECIMAL (18, 6) not null,
	--��
	SalePriceSum DECIMAL (18, 2) not null,
	
	-- ������� ��
	AveragePurchaseCost DECIMAL (18, 6) null,
	-- ������� ��
	AverageSalePrice DECIMAL (18, 2) null,
	
	IsReturn BIT not null,
	-- �������������� ������ (��� �����������)
	ArticleGroupId SMALLINT not null,
	ArticleGroupName VARCHAR(200) not null,
	StorageId TINYINT not null,
	StorageTypeId TINYINT not null,
	StorageName VARCHAR(200) not null,
	AccountOrganizationId INT not null,
	AccountOrganizationName VARCHAR(100) not null,
	TeamId SMALLINT not null,
	TeamName VARCHAR(200) not null,
	UserId INT not null,
	UserName VARCHAR(100) not null,
	ClientId INT not null,
	ClientName VARCHAR(200) not null,
	ClientOrganizationId INT not null,
	ClientOrganizationName VARCHAR(100) not null,
	ProducerId INT not null,	-- �������������/���������
	ProducerName VARCHAR(200) not null,	-- �������������/���������
	ArticleAvailabilityCount DECIMAL(18,6) not null, -- ������� ������
	ArticleAvailabilityAccountingPrice DECIMAL (18, 2) null	-- �� ��������
)

CREATE INDEX IX_#ResultFlatTable ON #ResultFlatTable (
	ArticleId,
	BatchId,
	-- �������������� ������ (��� �����������)
	ArticleGroupId,
	StorageId,
	AccountOrganizationId,
	TeamId,
	UserId,
	ClientId,
	ClientOrganizationId,
	ProducerId
)

-- �������, � ������� �������� ���� ��������� �� � ������ ����
CREATE TABLE #AvailableStorageListTable(
	Id TINYINT not null,
	Name VARCHAR(200) not null,
	StorageTypeId TINYINT not null
)
-- ��������� ������� "�����������" ������
INSERT INTO #AvailableStorageListTable(Id, Name, StorageTypeId) 
EXEC GetAvailableStorages @StorageIdList, @AllStorages, @UserId, 24102
-- ���� ���������� ������� ��, �� ��������� ��, �� ������� ������������ �� ����� ������ ��
IF @InAccountingPrice = 1 AND dbo.GetPermissionDistributionType(3 /*�������� �� �� ����������� ��*/, @UserId) = 0
	DELETE FROM #AvailableStorageListTable
	WHERE not exists(
		SELECT TOP(1) ts.StorageId 
		FROM UserTeam ut
			join TeamStorage ts on ts.TeamId = ut.TeamId AND ut.UserId = @UserId AND ts.StorageId = #AvailableStorageListTable.Id)

-- �������, � ������� �������� ���� ��������� ����� ������� � ������ ����
CREATE TABLE #AvailableArticleGroupListTable(
	Id TINYINT not null,
	Name VARCHAR(200) not null
)
INSERT INTO #AvailableArticleGroupListTable (Id, Name)
EXEC GetAvailableArticleGroups @ArticleGroupIdList, @AllArticleGroups

-- �������, � ������� �������� ���� ��������� �������� � ������ ����
CREATE TABLE #AvailableClientListTable(
	Id INT not null,
	Name VARCHAR(200) not null
)
CREATE INDEX IX_#AvailableClientListTable ON #AvailableClientListTable (Id)

INSERT INTO #AvailableClientListTable (Id, Name) 
EXEC GetAvailableClients @ClientIdList, @AllClients, @UserId

-- �������, � ������� �������� ���� ��������� ������������� � ������ ����
CREATE TABLE #AvailableUserListTable(
	Id INT not null,
	Name VARCHAR(100) not null
)
INSERT INTO #AvailableUserListTable (Id, Name)  
EXEC GetAvailableUsers @UserIdList, @AllUsers, @UserId, 24103

-- �������, � ������� �������� ���� ���������  ����������� �������� � ������ ����
CREATE TABLE #AvailableAccountOrganizationListTable(
	Id INT not null,
	Name VARCHAR(100) not null
)
INSERT INTO #AvailableAccountOrganizationListTable (Id, Name)  
EXEC GetAvailableAccountOrganizations @AccountOrganizationIdList, @AllAccountOrganizations

-- �������, � ������� �������� ���� ������ ��� ����������� ������� ����������
CREATE TABLE #AvailableDealListTable(Id INT not null)
CREATE INDEX IX_#AvailableDealListTable ON #AvailableDealListTable (Id)
INSERT INTO #AvailableDealListTable (Id)  
EXEC GetAvailableDeals @UserId, 3601
	

-- ���� ���������� ����� ������ ��������� � ��������� �������
IF @IsShippedExpenditureWaybills = 1
BEGIN
	-- ��������� ����������� �� ����� �������
	INSERT INTO #ResultFlatTable (ArticleId, ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
		AccountingPriceSum, PurchaseCostSum, SalePriceSum,
		ArticleAvailabilityCount, IsReturn,
		-- �������������� ������ (��� �����������)
		ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
		TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
	SELECT
		a.Id,	-- ArticleId
		a.Number,	-- ArticleNumber
		a.FullName,	-- ArticleName 
		ssi.BatchId,	-- BatchId 
		rw.Number, -- BatchNumber
		rw.Date, -- BatchDate
		a.PackSize,	-- PackSize
		SUM(ssi.SoldCount) SoldCount,	-- Count 
		country.Name,	-- CountryName
		rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
		
		SUM(ssi.AccountingPriceSum) AccountingPriceSum,	--AccountingPriceSum
		SUM(ssi.PurchaseCostSum) PurchaseCostSum,	--PurchaseCostSum
		SUM(ssi.SalePriceSum) SalePriceSum,	--SalePriceSum
		
		0,  -- ArticleAvailabilityCount
		0,	-- IsReturn
		-- �������������� ������ (��� �����������)
		_agt.Id,	-- ArticleGroupId 
		_agt.Name,	-- ArticleGroupName
		_st.Id,	-- StorageId 
		_st.Name,	-- StorageName
		_st.StorageTypeId, -- StorageTypeId
		_aot.Id,	-- AccountOrganizationId 
		_aot.Name,	-- AccountOrganizationName
		t.Id,	-- TeamId 
		t.Name,	-- TeamName 
		_ut.Id,	-- UserId 
		_ut.Name,	-- UserName 
		_ct.Id,	-- ClientId 
		_ct.Name,	-- ClientName 
		o.Id,	-- ClientOrganizationId 
		o.ShortName,	-- ClientOrganizationName 
		contractor.Id,	-- ProducerId 
		contractor.Name	-- ProducerName 
	FROM 
		ShippedSaleIndicator ssi
		join ReceiptWaybillRow rwr on rwr.Id = ssi.BatchId AND ssi.StartDate <= @EndDate AND (ssi.EndDate > @EndDate OR ssi.EndDate is null)
		join Article a on a.Id = ssi.ArticleId		
		join Deal d on d.Id = ssi.DealId
		join Contract c on c.Id = d.ClientContractId
		join Country country on country.Id = rwr.CountryId
		join Team t on t.Id = ssi.TeamId
		join Organization o on o.Id = ssi.ClientOrganizationId
		join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
		join Contractor contractor on contractor.Id = rw.ProviderId
		
		-- ����������� �� ���������
		join #AvailableDealListTable ad on ad.Id = ssi.DealId
		-- ����������� �� ���������� ������
		join #AvailableStorageListTable _st on _st.Id = ssi.StorageId	-- ����������� �� ��
		join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
		join #AvailableClientListTable _ct on _ct.Id = ssi.ClientId	-- ����������� �� ��������
		join #AvailableUserListTable _ut on _ut.Id = ssi.UserId	-- ����������� �� �������������
		join #AvailableAccountOrganizationListTable _aot on _aot.Id = ssi.AccountOrganizationId	-- ����������� �� ����������� ��������
	GROUP BY a.Id, a.Number,a.FullName,ssi.BatchId,rw.Number,rw.Date,a.PackSize,country.Name,rwr.CustomsDeclarationNumber,		
		-- �������������� ������ (��� �����������)
		_agt.Id,_agt.Name,_st.Id,_st.Name,_st.StorageTypeId,_aot.Id,_aot.Name,t.Id,t.Name,_ut.Id,_ut.Name,_ct.Id,_ct.Name,o.Id,
		o.ShortName,contractor.Id,contractor.Name
		
	-- �������� ���������� �� ������ �������
	UPDATE #ResultFlatTable
	SET Count = #ResultFlatTable.Count - ind.SoldCount,
		AccountingPriceSum = #ResultFlatTable.AccountingPriceSum - ind.AccountingPriceSum,
		PurchaseCostSum = #ResultFlatTable.PurchaseCostSum - ind.PurchaseCostSum,
		SalePriceSum = #ResultFlatTable.SalePriceSum - ind.SalePriceSum
	FROM(
		SELECT ssi.UserId, ssi.StorageId, ssi.TeamId, ssi.BatchId, ssi.ClientId, ssi.ClientOrganizationId, ssi.AccountOrganizationId,
			SUM(ssi.SoldCount) SoldCount, SUM(ssi.AccountingPriceSum) AccountingPriceSum, 
			SUM(ssi.PurchaseCostSum) PurchaseCostSum,SUM(ssi.SalePriceSum) SalePriceSum
		FROM
			ShippedSaleIndicator ssi
			join Article a on a.Id = ssi.ArticleId AND ssi.StartDate <= @StartDate AND (ssi.EndDate > @StartDate OR ssi.EndDate is null)
			
			join #AvailableDealListTable ad on ad.Id = ssi.DealId
			-- ����������� �� ���������� ������
			join #AvailableStorageListTable _st on _st.Id = ssi.StorageId	-- ����������� �� ��
			join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
			join #AvailableClientListTable _ct on _ct.Id = ssi.ClientId	-- ����������� �� ��������
			join #AvailableUserListTable _ut on _ut.Id = ssi.UserId	-- ����������� �� �������������
			join #AvailableAccountOrganizationListTable _aot on _aot.Id = ssi.AccountOrganizationId	-- ����������� �� ����������� ��������
		GROUP BY ssi.UserId, ssi.StorageId, ssi.TeamId, ssi.BatchId, ssi.ClientId, ssi.ClientOrganizationId, ssi.AccountOrganizationId) ind
	WHERE ind.UserId = #ResultFlatTable.UserId AND ind.StorageId = #ResultFlatTable.StorageId AND
		ind.TeamId = #ResultFlatTable.TeamId AND ind.BatchId = #ResultFlatTable.BatchId	AND ind.ClientId = #ResultFlatTable.ClientId AND
		ind.ClientOrganizationId = 	#ResultFlatTable.ClientOrganizationId AND ind.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId
END
ELSE	-- ����� ����� �����������
BEGIN
	-- ��������� ���� ����������� ������� ����������
	INSERT INTO #ResultFlatTable (ArticleId, ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
		AccountingPriceSum, PurchaseCostSum, SalePriceSum,
		ArticleAvailabilityCount, IsReturn,
		-- �������������� ������ (��� �����������)
		ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
		TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
	SELECT
		a.Id,	-- ArticleId
		a.Number,	-- ArticleNumber
		a.FullName,	-- ArticleName 
		asi.BatchId,	-- BatchId 
		rw.Number, -- BatchNumber
		rw.Date, -- BatchDate
		a.PackSize,	-- PackSize
		SUM(asi.SoldCount),	-- Count 
		country.Name,	-- CountryName
		rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
		
		SUM(asi.AccountingPriceSum),	--AccountingPriceSum
		SUM(asi.PurchaseCostSum),	--PurchaseCostSum
		SUM(asi.SalePriceSum),	--SalePriceSum
		
		0,  -- ArticleAvailabilityCount
		0,	-- IsReturn
		-- �������������� ������ (��� �����������)
		_agt.Id,	-- ArticleGroupId 
		_agt.Name,	-- ArticleGroupName
		_st.Id,	-- StorageId 
		_st.Name,	-- StorageName
		_st.StorageTypeId, -- StorageTypeId
		_aot.Id,	-- AccountOrganizationId 
		_aot.Name,	-- AccountOrganizationName
		t.Id,	-- TeamId 
		t.Name,	-- TeamName 
		_ut.Id,	-- UserId 
		_ut.Name,	-- UserName 
		_ct.Id,	-- ClientId 
		_ct.Name,	-- ClientName 
		o.Id,	-- ClientOrganizationId 
		o.ShortName,	-- ClientOrganizationName 
		contractor.Id,	-- ProducerId 
		contractor.Name	-- ProducerName 
	FROM 
		AcceptedSaleIndicator asi
		join ReceiptWaybillRow rwr on rwr.Id = asi.BatchId AND asi.StartDate <= @EndDate AND (asi.EndDate > @EndDate OR asi.EndDate is null)
		join Article a on a.Id = asi.ArticleId		
		join Deal d on d.Id = asi.DealId
		join Contract c on c.Id = d.ClientContractId
		join Country country on country.Id = rwr.CountryId
		join Team t on t.Id = asi.TeamId
		join Organization o on o.Id = asi.ClientOrganizationId
		join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
		join Contractor contractor on contractor.Id = rw.ProviderId

		join #AvailableDealListTable ad on ad.Id = asi.DealId
		-- ����������� �� ���������� ������
		join #AvailableStorageListTable _st on _st.Id = asi.StorageId	-- ����������� �� ��
		join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
		join #AvailableClientListTable _ct on _ct.Id = asi.ClientId	-- ����������� �� ��������
		join #AvailableUserListTable _ut on _ut.Id = asi.UserId	-- ����������� �� �������������
		join #AvailableAccountOrganizationListTable _aot on _aot.Id = asi.AccountOrganizationId	-- ����������� �� ����������� ��������
	GROUP BY a.Id, a.Number,a.FullName,asi.BatchId,rw.Number,rw.Date,a.PackSize,country.Name,rwr.CustomsDeclarationNumber,		
		-- �������������� ������ (��� �����������)
		_agt.Id,_agt.Name,_st.Id,_st.Name,_st.StorageTypeId,_aot.Id,_aot.Name,t.Id,t.Name,_ut.Id,_ut.Name,_ct.Id,_ct.Name,o.Id,
		o.ShortName,contractor.Id,contractor.Name
		
	-- �������� ���������� �� ������ �������
	UPDATE #ResultFlatTable
	SET Count = #ResultFlatTable.Count - ind.SoldCount,
		AccountingPriceSum = #ResultFlatTable.AccountingPriceSum - ind.AccountingPriceSum,
		PurchaseCostSum = #ResultFlatTable.PurchaseCostSum - ind.PurchaseCostSum,
		SalePriceSum = #ResultFlatTable.SalePriceSum - ind.SalePriceSum
	FROM(
		SELECT asi.UserId, asi.StorageId, asi.TeamId, asi.BatchId, asi.ClientId, asi.ClientOrganizationId, asi.AccountOrganizationId,
			SUM(asi.SoldCount) SoldCount, SUM(asi.AccountingPriceSum) AccountingPriceSum, 
			SUM(asi.PurchaseCostSum) PurchaseCostSum,SUM(asi.SalePriceSum) SalePriceSum
		FROM
			AcceptedSaleIndicator asi
			join Article a on a.Id = asi.ArticleId AND asi.StartDate <= @StartDate AND (asi.EndDate > @StartDate OR asi.EndDate is null)
			
			join #AvailableDealListTable ad on ad.Id = asi.DealId
			-- ����������� �� ���������� ������
			join #AvailableStorageListTable _st on _st.Id = asi.StorageId	-- ����������� �� ��
			join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
			join #AvailableClientListTable _ct on _ct.Id = asi.ClientId	-- ����������� �� ��������
			join #AvailableUserListTable _ut on _ut.Id = asi.UserId	-- ����������� �� �������������
			join #AvailableAccountOrganizationListTable _aot on _aot.Id = asi.AccountOrganizationId	-- ����������� �� ����������� ��������
		GROUP BY asi.UserId, asi.StorageId, asi.TeamId, asi.BatchId, asi.ClientId, asi.ClientOrganizationId, asi.AccountOrganizationId) ind
	WHERE ind.UserId = #ResultFlatTable.UserId AND ind.StorageId = #ResultFlatTable.StorageId AND
		ind.TeamId = #ResultFlatTable.TeamId AND ind.BatchId = #ResultFlatTable.BatchId	AND ind.ClientId = #ResultFlatTable.ClientId AND
		ind.ClientOrganizationId = 	#ResultFlatTable.ClientOrganizationId AND ind.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId
END

-- ������� ������� ������
DELETE FROM #ResultFlatTable WHERE Count = 0
	
IF @ConsiderReturnFromClient = 1	-- ���� ����� ������ ��������
BEGIN
	IF @ConsiderReturnFromClientByDate = 1	-- ��������� �� ����?
	BEGIN
		IF 	@IsShippedExpenditureWaybills = 1	-- ���� ���������� ����� ������ ��������� � ��������� �������
		BEGIN
			-- �������� �� ����� �������
			SELECT rfci.* INTO #tmp_GetReceiptedReturnFromClientWaybillRowsByDate
			FROM ReceiptedReturnFromClientIndicator rfci
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������

			-- �������� �������� �� ������ �������		
			UPDATE #tmp_GetReceiptedReturnFromClientWaybillRowsByDate
			SET 
				ReturnedCount = #tmp_GetReceiptedReturnFromClientWaybillRowsByDate.ReturnedCount - p.ReturnedCount,
				AccountingPriceSum = #tmp_GetReceiptedReturnFromClientWaybillRowsByDate.AccountingPriceSum - p.AccountingPriceSum,
				PurchaseCostSum = #tmp_GetReceiptedReturnFromClientWaybillRowsByDate.PurchaseCostSum - p.PurchaseCostSum,
				SalePriceSum = #tmp_GetReceiptedReturnFromClientWaybillRowsByDate.SalePriceSum - p.SalePriceSum
			FROM (
				SELECT rfci.*
				FROM ReceiptedReturnFromClientIndicator rfci
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
					join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
					join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
					join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������
				) p
			WHERE
				#tmp_GetReceiptedReturnFromClientWaybillRowsByDate.SaleWaybillId = p.SaleWaybillId AND 
				#tmp_GetReceiptedReturnFromClientWaybillRowsByDate.BatchId = p.BatchId AND 
				#tmp_GetReceiptedReturnFromClientWaybillRowsByDate.ReturnFromClientWaybillCuratorId = p.ReturnFromClientWaybillCuratorId
				
			-- ������� ������� ������
			DELETE FROM #tmp_GetReceiptedReturnFromClientWaybillRowsByDate WHERE ReturnedCount = 0

			-- ��������� ���� ����������� ������� ���������
			INSERT INTO #ResultFlatTable (ArticleId, ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
				AccountingPriceSum, PurchaseCostSum, SalePriceSum,
				ArticleAvailabilityCount, IsReturn,
				-- �������������� ������ (��� �����������)
				ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
				TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
			SELECT
				a.Id,	-- ArticleId
				a.Number,	-- ArticleNumber
				a.FullName,	-- ArticleName 
				rfci.BatchId,	-- BatchId 
				rw.Number, --BatchNumber
				rw.Date, --BatchDate
				a.PackSize,	-- PackSize
				rfci.ReturnedCount,	-- Count 
				country.Name,	-- CountryName 
				rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
				
				rfci.AccountingPriceSum,	--AccountingPriceSum
				rfci.PurchaseCostSum,	--PurchaseCostSum
				rfci.SalePriceSum,	--SalePriceSum
				
				0,  -- ArticleAvailabilityCount
				1,	-- IsReturn
				-- �������������� ������ (��� �����������)
				_agt.Id,	-- ArticleGroupId 
				_agt.Name,	-- ArticleGroupName
				_st.Id,	-- StorageId 
				_st.Name,	-- StorageName
				_st.StorageTypeId, -- StorageTypeId
				_aot.Id,	-- AccountOrganizationId 
				_aot.Name,	-- AccountOrganizationName
				t.Id,	-- TeamId 
				t.Name,	-- TeamName 
				_ut.Id,	-- UserId 
				_ut.Name,	-- UserName 
				_ct.Id,	-- ClientId 
				_ct.Name,	-- ClientName 
				o.Id,	-- ClientOrganizationId 
				o.ShortName,	-- ClientOrganizationName 
				contractor.Id,	-- ProducerId 
				contractor.Name	-- ProducerName 
			FROM #tmp_GetReceiptedReturnFromClientWaybillRowsByDate rfci
				join Article a on a.Id = rfci.ArticleId
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Country country on country.Id = rwr.CountryId
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				-- ����������� �� ���������� ������
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������

			drop table #tmp_GetReceiptedReturnFromClientWaybillRowsByDate
		END
		ELSE
		BEGIN
			-- �������� �� ����� �������
			SELECT rfci.* INTO #tmp_GetAcceptedReturnFromClientWaybillRowsByDate
			FROM AcceptedReturnFromClientIndicator rfci
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������

			-- �������� �������� �� ������ �������		
			UPDATE #tmp_GetAcceptedReturnFromClientWaybillRowsByDate
			SET 
				ReturnedCount = #tmp_GetAcceptedReturnFromClientWaybillRowsByDate.ReturnedCount - p.ReturnedCount,
				AccountingPriceSum = #tmp_GetAcceptedReturnFromClientWaybillRowsByDate.AccountingPriceSum - p.AccountingPriceSum,
				PurchaseCostSum = #tmp_GetAcceptedReturnFromClientWaybillRowsByDate.PurchaseCostSum - p.PurchaseCostSum,
				SalePriceSum = #tmp_GetAcceptedReturnFromClientWaybillRowsByDate.SalePriceSum - p.SalePriceSum
			FROM (
				SELECT rfci.*
				FROM AcceptedReturnFromClientIndicator rfci
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
					join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
					join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
					join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������
				) p
			WHERE
				#tmp_GetAcceptedReturnFromClientWaybillRowsByDate.SaleWaybillId = p.SaleWaybillId AND 
				#tmp_GetAcceptedReturnFromClientWaybillRowsByDate.BatchId = p.BatchId AND 
				#tmp_GetAcceptedReturnFromClientWaybillRowsByDate.ReturnFromClientWaybillCuratorId = p.ReturnFromClientWaybillCuratorId
				
			-- ������� ������� ������
			DELETE FROM #tmp_GetAcceptedReturnFromClientWaybillRowsByDate WHERE ReturnedCount = 0

			-- ��������� ���� ����������� ������� ���������
			INSERT INTO #ResultFlatTable (ArticleId, ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
				AccountingPriceSum, PurchaseCostSum, SalePriceSum,
				ArticleAvailabilityCount, IsReturn,
				-- �������������� ������ (��� �����������)
				ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
				TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
			SELECT
				a.Id,	-- ArticleId
				a.Number,	-- ArticleNumber
				a.FullName,	-- ArticleName 
				rfci.BatchId,	-- BatchId 
				rw.Number, --BatchNumber
				rw.Date, --BatchDate
				a.PackSize,	-- PackSize
				rfci.ReturnedCount,	-- Count 
				country.Name,	-- CountryName 
				rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
				
				rfci.AccountingPriceSum,	--AccountingPriceSum
				rfci.PurchaseCostSum,	--PurchaseCostSum
				rfci.SalePriceSum,	--SalePriceSum
				
				0,  -- ArticleAvailabilityCount
				1,	-- IsReturn
				-- �������������� ������ (��� �����������)
				_agt.Id,	-- ArticleGroupId 
				_agt.Name,	-- ArticleGroupName
				_st.Id,	-- StorageId 
				_st.Name,	-- StorageName
				_st.StorageTypeId, -- StorageTypeId
				_aot.Id,	-- AccountOrganizationId 
				_aot.Name,	-- AccountOrganizationName
				t.Id,	-- TeamId 
				t.Name,	-- TeamName 
				_ut.Id,	-- UserId 
				_ut.Name,	-- UserName 
				_ct.Id,	-- ClientId 
				_ct.Name,	-- ClientName 
				o.Id,	-- ClientOrganizationId 
				o.ShortName,	-- ClientOrganizationName 
				contractor.Id,	-- ProducerId 
				contractor.Name	-- ProducerName 
			FROM #tmp_GetAcceptedReturnFromClientWaybillRowsByDate rfci
				join Article a on a.Id = rfci.ArticleId
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Country country on country.Id = rwr.CountryId
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				-- ����������� �� ���������� ������
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������

			drop table #tmp_GetAcceptedReturnFromClientWaybillRowsByDate
		END
	END
	ELSE	-- ����� �� ��� �� ����������
	BEGIN

		IF 	@IsShippedExpenditureWaybills = 1	-- ���� ���������� ����� ������ ��������� � ��������� �������
		BEGIN
			-- �������� �� ����� �������
			SELECT rfci.* INTO #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills
			FROM ReturnFromClientBySaleShippingDateIndicator rfci
				join #AvailableDealListTable ad on ad.Id = rfci.DealId
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������

			-- �������� �������� �� ������ �������		
			UPDATE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills
			SET 
				ReturnedCount = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.ReturnedCount - p.ReturnedCount,
				AccountingPriceSum = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.AccountingPriceSum - p.AccountingPriceSum
			FROM (
				SELECT rfci.*
				FROM ReturnFromClientBySaleShippingDateIndicator rfci
					join #AvailableDealListTable ad on ad.Id = rfci.DealId
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
					join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
					join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
					join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������
					) p
			WHERE
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.SaleWaybillId = p.SaleWaybillId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.BatchId = p.BatchId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.ReturnFromClientWaybillCuratorId = p.ReturnFromClientWaybillCuratorId
				
			-- ������� ������� ������
			DELETE FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills WHERE ReturnedCount <= 0
			
			-- ��������� ���� ����������� ������� ��������� �� ���� �������� ����������
			INSERT INTO #ResultFlatTable (ArticleId,ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
				AccountingPriceSum, PurchaseCostSum, SalePriceSum,
				ArticleAvailabilityCount, IsReturn,
				-- �������������� ������ (��� �����������)
				ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId,  AccountOrganizationId, AccountOrganizationName, TeamId, 
				TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName)
			SELECT
				a.Id,	-- ArticleId
				a.Number,	-- ArticleNumber
				a.FullName,	-- ArticleName 
				rfci.BatchId,	-- BatchId 
				rw.Number, -- BatchNumber
				rw.Date, -- BatchDate
				a.PackSize,	-- PackSize
				rfci.ReturnedCount,	-- Count 
				country.Name,	-- CountryName 
				rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
				
				rfci.AccountingPriceSum,	--AccountingPriceSum
				rfci.PurchaseCostSum,	--PurchaseCostSum
				rfci.SalePriceSum,	--SalePriceSum
				
				0,  -- ArticleAvailabilityCount
				1,	-- IsReturn
				
				-- �������������� ������ (��� �����������)
				_agt.Id,	-- ArticleGroupId 
				_agt.Name,	-- ArticleGroupName
				_st.Id,	-- StorageId 
				_st.Name,	-- StorageName
				_st.StorageTypeId, -- StorageTypeId
				_aot.Id,	-- AccountOrganizationId 
				_aot.Name,	-- AccountOrganizationName
				t.Id,	-- TeamId 
				t.Name,	-- TeamName 
				_ut.Id,	-- UserId 
				_ut.Name,	-- UserName 
				_ct.Id,	-- ClientId 
				_ct.Name,	-- ClientName 
				o.Id,	-- ClientOrganizationId 
				o.ShortName,	-- ClientOrganizationName 
				contractor.Id,	-- ProducerId 
				contractor.Name	-- ProducerName 
			FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills rfci
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate >= @StartDate AND rfci.StartDate <= @EndDate
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Country country on country.Id = rwr.CountryId		
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				join #AvailableDealListTable ad on ad.Id = rfci.DealId
				-- ����������� �� ���������� ������
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������
				
			DROP TABLE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills
			
		END
		ELSE
		BEGIN
			-- �������� �� ����� �������
			SELECT rfci.* INTO #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2
			FROM ReturnFromClientBySaleAcceptanceDateIndicator rfci
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������

			-- �������� �������� �� ������ �������		
			UPDATE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2
			SET 
				ReturnedCount = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.ReturnedCount - p.ReturnedCount,
				AccountingPriceSum = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.AccountingPriceSum - p.AccountingPriceSum
			FROM (
				SELECT rfci.*
				FROM ReturnFromClientBySaleAcceptanceDateIndicator rfci
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
					join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
					join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
					join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������
				 ) p
			WHERE
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.SaleWaybillId = p.SaleWaybillId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.BatchId = p.BatchId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.ReturnFromClientWaybillCuratorId = p.ReturnFromClientWaybillCuratorId
				
			-- ������� ������� ������
			DELETE FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2 WHERE ReturnedCount <= 0
			
			-- ��������� ���� ����������� ������� ��������� �� ���� �������� ����������
			INSERT INTO #ResultFlatTable (ArticleId,ArticleNumber, ArticleName, BatchId, BatchNumber,BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
				AccountingPriceSum, PurchaseCostSum, SalePriceSum,
				ArticleAvailabilityCount, IsReturn,
				-- �������������� ������ (��� �����������)
				ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
				TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
			SELECT
				a.Id,	-- ArticleId
				a.Number,	-- ArticleNumber
				a.FullName,	-- ArticleName 
				rfci.BatchId,	-- BatchId 
				rw.Number, -- BatchNumber
				rw.Date, -- BatchDate
				a.PackSize,	-- PackSize
				rfci.ReturnedCount,	-- Count 
				rwr.CountryId,	-- CountryName 
				rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
				
				rfci.AccountingPriceSum,	--AccountingPriceSum
				rfci.PurchaseCostSum,	--PurchaseCostSum
				rfci.SalePriceSum,	--SalePriceSum
				
				0,  --ArticleAvailabilityCount
				1,	-- IsReturn
				
				-- �������������� ������ (��� �����������)
				_agt.Id,	-- ArticleGroupId 
				_agt.Name,	-- ArticleGroupName
				_st.Id,	-- StorageId 
				_st.Name,	-- StorageName
				_st.StorageTypeId, -- StorageTypeId
				_aot.Id,	-- AccountOrganizationId 
				_aot.Name,	-- AccountOrganizationName
				t.Id,	-- TeamId 
				t.Name,	-- TeamName 
				_ut.Id,	-- UserId 
				_ut.Name,	-- UserName 
				_ct.Id,	-- ClientId 
				_ct.Name,	-- ClientName 
				o.Id,	-- ClientOrganizationId 
				o.ShortName,	-- ClientOrganizationName 
				contractor.Id,	-- ProducerId 
				contractor.Name	-- ProducerName 
			FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2 rfci
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate >= @StartDate AND rfci.StartDate <= @EndDate
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				-- ����������� �� ���������� ������
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ����������� �� ������� �������
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ����������� �� �������������
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ����������� �� ����������� ��������
				
			DROP TABLE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2
			
		END
	END
END

-- ���� ��� ���������� �� �������, �� ������� ��
IF @DevideByBatch = 0
BEGIN
	-- �������� ������� �������
	CREATE TABLE #tmp_ResultFlatTable(
		-- �������� ������ � ������� ����������
		ArticleId INT  null,
		ArticleNumber VARCHAR(30)  null,
		ArticleName VARCHAR(200)  null,
		BatchId UNIQUEIDENTIFIER  null,
		BatchNumber VARCHAR(25)  null,
		BatchDate DateTime  null,
		Count DECIMAL(18, 6)  null,
		PackSize DECIMAL(12, 6) null,
		CountryName VARCHAR(200)  null,
		CustomsDeclarationNumber VARCHAR(33)  null,

		-- ��
		AccountingPriceSum DECIMAL (18, 2) null,
		-- ��
		PurchaseCostSum DECIMAL (18, 6) null,
		--��
		SalePriceSum DECIMAL (18, 2) null,
		
		-- ������� ��
		AveragePurchaseCost DECIMAL (18, 6) null,
		-- ������� ��
		AverageSalePrice DECIMAL (18, 2) null,
		
		IsReturn BIT  null,
		-- �������������� ������ (��� �����������)
		ArticleGroupId SMALLINT  null,
		ArticleGroupName VARCHAR(200)  null,
		StorageId TINYINT  null,
		StorageTypeId TINYINT  null,
		StorageName VARCHAR(200)  null,
		AccountOrganizationId INT  null,
		AccountOrganizationName VARCHAR(100)  null,
		TeamId SMALLINT  null,
		TeamName VARCHAR(200)  null,
		UserId INT  null,
		UserName VARCHAR(100)  null,
		ClientId INT  null,
		ClientName VARCHAR(200)  null,
		ClientOrganizationId INT  null,
		ClientOrganizationName VARCHAR(100)  null,
		ProducerId INT  null,	-- �������������/���������
		ProducerName VARCHAR(200)  null,	-- �������������/���������
		ArticleAvailabilityCount DECIMAL(18,6)  null, -- ������� ������
		ArticleAvailabilityAccountingPrice DECIMAL (18, 2) null	-- �� ��������
	)

	-- �������� �������������� ������ �� ������ �� ��������� �������
	INSERT INTO #tmp_ResultFlatTable (ArticleId, Count, AccountingPriceSum, PurchaseCostSum, SalePriceSum,
		IsReturn,
		-- �������������� ������ (��� �����������)
		ArticleGroupId,  StorageId,  StorageTypeId,  AccountOrganizationId,  TeamId, 
		UserId, ClientId,  ClientOrganizationId,  ProducerId)
	SELECT
		rft.ArticleId, 
		SUM(rft.Count), 
		SUM(rft.AccountingPriceSum), 
		SUM(rft.PurchaseCostSum), 
		SUM(rft.SalePriceSum),
		rft.IsReturn,
		-- �������������� ������ (��� �����������)
		rft.ArticleGroupId, 
		rft.StorageId, 
		rft.StorageTypeId,  
		rft.AccountOrganizationId, 
		rft.TeamId, 
		rft.UserId, 
		rft.ClientId, 
		rft.ClientOrganizationId, 
		rft.ProducerId
	FROM #ResultFlatTable rft
	GROUP BY ArticleId, ArticleGroupId, StorageId, StorageTypeId, AccountOrganizationId, TeamId, UserId, ClientId, ClientOrganizationId, ProducerId, IsReturn

	UPDATE #tmp_ResultFlatTable
	SET
		ArticleNumber = tmp. ArticleNumber,
		ArticleName = tmp. ArticleName,
		BatchId = tmp.BatchId,
		BatchNumber = tmp. BatchNumber,
		BatchDate = tmp. BatchDate,
		PackSize = tmp. PackSize,
		CountryName = tmp. CountryName,
		CustomsDeclarationNumber = tmp. CustomsDeclarationNumber,
		ArticleAvailabilityCount = tmp. ArticleAvailabilityCount,
		ArticleAvailabilityAccountingPrice = tmp.ArticleAvailabilityAccountingPrice,
		ArticleGroupName = tmp. ArticleGroupName,
		StorageName = tmp. StorageName,
		AccountOrganizationName = tmp. AccountOrganizationName,
		TeamName = tmp. TeamName,
		UserName = tmp. UserName,
		ClientName = tmp.ClientName ,
		ClientOrganizationName = tmp. ClientOrganizationName,
		ProducerName  = tmp.ProducerName
	FROM (
		SELECT *
		FROM #ResultFlatTable) tmp
	WHERE
		tmp.ArticleId = #tmp_ResultFlatTable.ArticleId AND tmp.ArticleGroupId = #tmp_ResultFlatTable.ArticleGroupId AND tmp.StorageId = #tmp_ResultFlatTable.StorageId AND 
		tmp.AccountOrganizationId = #tmp_ResultFlatTable.AccountOrganizationId AND tmp.TeamId = #tmp_ResultFlatTable.TeamId AND tmp.UserId = #tmp_ResultFlatTable.UserId AND 
		tmp.ClientId = #tmp_ResultFlatTable.ClientId AND tmp.ClientOrganizationId = #tmp_ResultFlatTable.ClientOrganizationId AND tmp.ProducerId = #tmp_ResultFlatTable.ProducerId
		AND #tmp_ResultFlatTable.IsReturn = tmp.IsReturn
		
	-- ���������� ������ � �������� �������
	DELETE FROM #ResultFlatTable
	INSERT INTO #ResultFlatTable SELECT * FROM #tmp_ResultFlatTable
			
	-- ������� ������������� �������
	DROP TABLE #tmp_ResultFlatTable
END	

-- ���� ����� �������� �������, �� ...
IF @GetArticleAvailability = 1
BEGIN
	-- �������� ������� �������
	-- ����� �� ��������� ������?
	IF @DevideByBatch = 0	
	BEGIN
		-- ���, �� ��������� ������ ������� ��� ��������� ��������
		CREATE TABLE #tmp(
			ArticleId INT not null,
			StorageId TINYINT not null,
			AccountOrganizationId INT not null,
			Count DECIMAL(18,6) not null -- ������� ������
		)
		-- �������� ������� ������ ��� ����� ������
		INSERT INTO #tmp (ArticleId, StorageId , AccountOrganizationId, Count)
		SELECT ArticleId, StorageId, AccountOrganizationId, SUM(Count)
		FROM ExactArticleAvailabilityIndicator
		WHERE StartDate <= @EndDate AND (EndDate > @EndDate OR EndDate is null)
		GROUP BY ArticleId, StorageId, AccountOrganizationId
		-- ��������� ������� � �������� �������
		UPDATE #ResultFlatTable 
		SET ArticleAvailabilityCount = eaa.Count
		FROM #tmp eaa
		WHERE
			eaa.StorageId = #ResultFlatTable.StorageId AND eaa.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId AND
			eaa.ArticleId = #ResultFlatTable.ArticleId
		
		DROP TABLE #tmp
	END
	ELSE
		-- ���������� ������� � ������ ������
		UPDATE #ResultFlatTable 
		SET ArticleAvailabilityCount = eaa.Count
		FROM (
			SELECT *
			FROM ExactArticleAvailabilityIndicator
			WHERE
				StartDate <= @EndDate AND (EndDate > @EndDate OR EndDate is null)) eaa
		WHERE
			eaa.StorageId = #ResultFlatTable.StorageId AND eaa.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId AND
			eaa.ArticleId = #ResultFlatTable.ArticleId AND eaa.BatchId = #ResultFlatTable.BatchId
	
	-- ������� ������� ������ ��� ��������� ��
	CREATE TABLE #ArticleAccountingPriceForActiclesOnStoragesDetermination(
		ArticleId INT not null,
		StorageId SMALLINT not null,
		AccountingPrice DECIMAL(18,2) null
	)
	-- ��������� ������� �������� � ��, ��� ������� ����� �������� ��
	INSERT INTO #ArticleAccountingPriceForActiclesOnStoragesDetermination(ArticleId, StorageId, AccountingPrice)
	SELECT rft.ArticleId, rft.StorageId, -1
	FROM #ResultFlatTable rft
	GROUP BY rft.ArticleId, rft.StorageId
	
	-- �������� ��
	EXEC GetAccountingPriceForArticlesOnStoragesByDate @EndDate
	
	-- ���������� ��
	UPDATE #ResultFlatTable
	SET ArticleAvailabilityAccountingPrice = t.AccountingPrice
	FROM #ArticleAccountingPriceForActiclesOnStoragesDetermination t
	WHERE t.ArticleId = #ResultFlatTable.ArticleId AND t.StorageId = #ResultFlatTable.StorageId
		
	DROP TABLE #ArticleAccountingPriceForActiclesOnStoragesDetermination
END

-- ���������� �������������� ������� ������� ������
SELECT * FROM #ResultFlatTable 

-- ������� ��������� �������
DROP INDEX IX_#ResultFlatTable on #ResultFlatTable
DROP INDEX IX_#AvailableDealListTable on #AvailableDealListTable
DROP INDEX IX_#AvailableClientListTable ON #AvailableClientListTable

DROP TABLE #AvailableStorageListTable
DROP TABLE #AvailableArticleGroupListTable
DROP TABLE #AvailableClientListTable
DROP TABLE #AvailableUserListTable
DROP TABLE #AvailableAccountOrganizationListTable
DROP TABLE #AvailableDealListTable
DROP TABLE #ResultFlatTable

GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'Report0002_GetAvailableSaleIndicatorByUser'
)
   DROP PROCEDURE dbo.Report0002_GetAvailableSaleIndicatorByUser
GO
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

