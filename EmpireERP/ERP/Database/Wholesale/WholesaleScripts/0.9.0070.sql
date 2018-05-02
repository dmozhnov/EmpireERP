/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.70

  ��� ������:
	* ��������� ����������� �� �������� ��� ���������� ������ �� ����������� Report0002 (����� + ���������)
	
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

SET @PreviousVersion = '0.9.69' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.70'			-- ����� ����� ������

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
	@TakeArticlesFromArticleGroup BIT, -- ������� ����, ������ ����� ���� ������. ���� 1 , �� �� ���������� @ArticleGroupIdList ��� @AllArticleGroups.����� �� @ArticleIdList.
	@ArticleGroupIdList VARCHAR(8000),	--  ������ ����� ����� �������
	@AllArticleGroups BIT,	-- ������� ������ ���� ����� �������
	@ArticleIdList  VARCHAR(8000),--- ������ ����� �������
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
EXEC GetAvailableStorages @StorageIdList, @AllStorages, @UserId, 24102 -- ����� �������� � ������ ����������� �������
-- ���� ���������� ������� ��, �� ��������� ��, �� ������� ������������ �� ����� ������ ��
IF @InAccountingPrice = 1 AND dbo.GetPermissionDistributionType(3 /*�������� �� �� ����������� ��*/, @UserId) = 0
	DELETE FROM #AvailableStorageListTable
	WHERE not exists(
		SELECT TOP(1) ts.StorageId 
		FROM UserTeam ut
			join TeamStorage ts on ts.TeamId = ut.TeamId AND ut.UserId = @UserId AND ts.StorageId = #AvailableStorageListTable.Id)

-- �������, � ������� �������� ���� ��������� ����� ������� � ������ ����
CREATE TABLE #AvailableArticleListTable(
	Id INT not null)
	
INSERT INTO #AvailableArticleListTable (Id)
EXEC GetAvailableArticles		@TakeArticlesFromArticleGroup,
								@ArticleGroupIdList,
								@AllArticleGroups,
								@ArticleIdList

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
EXEC GetAvailableUsers @UserIdList, @AllUsers, @UserId, 24103 -- ������������ � ������ ����������� �������

-- �������, � ������� �������� ���� ���������  ����������� �������� � ������ ����
CREATE TABLE #AvailableAccountOrganizationListTable(
	Id INT not null,
	Name VARCHAR(100) not null
)
INSERT INTO #AvailableAccountOrganizationListTable (Id, Name)  
EXEC GetAvailableAccountOrganizations @AccountOrganizationIdList, @AllAccountOrganizations

-- �������, � ������� �������� ���� ������ ��� ����������� ������� ����������
CREATE TABLE #AvailableDealListForSaleTable(Id INT not null)
CREATE INDEX IX_#AvailableDealListForSaleTable ON #AvailableDealListForSaleTable (Id)
INSERT INTO #AvailableDealListForSaleTable (Id)  
EXEC GetAvailableDeals @UserId, 3601 -- �������� ������ � ������� ��������� ���������� �������

-- �������, � ������� �������� ���� ������ ��� ����������� ������� ���������
CREATE TABLE #AvailableDealListForReturnTable(Id INT not null)
CREATE INDEX IX_#AvailableDealListForReturnTable ON #AvailableDealListForReturnTable (Id)
INSERT INTO #AvailableDealListForReturnTable (Id)  
EXEC GetAvailableDeals @UserId, 3901 -- ���������� �� �������� ������ ��������� ��������
	

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
		ag.Id,	-- ArticleGroupId 
		ag.Name,	-- ArticleGroupName
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
		join ArticleGroup ag on ag.Id = a.ArticleGroupId 		
		join Deal d on d.Id = ssi.DealId
		join Contract c on c.Id = d.ClientContractId
		join Country country on country.Id = rwr.CountryId
		join Team t on t.Id = ssi.TeamId
		join Organization o on o.Id = ssi.ClientOrganizationId
		join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
		join Contractor contractor on contractor.Id = rw.ProviderId
		
		-- ����������� �� ���������
		join #AvailableDealListForSaleTable ad on ad.Id = ssi.DealId
		-- ����������� �� ���������� ������
		join #AvailableStorageListTable _st on _st.Id = ssi.StorageId	-- ����������� �� ��
		join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
		join #AvailableClientListTable _ct on _ct.Id = ssi.ClientId	-- ����������� �� ��������
		join #AvailableUserListTable _ut on _ut.Id = ssi.UserId	-- ����������� �� �������������
		join #AvailableAccountOrganizationListTable _aot on _aot.Id = ssi.AccountOrganizationId	-- ����������� �� ����������� ��������
	GROUP BY a.Id, a.Number,a.FullName,ssi.BatchId,rw.Number,rw.Date,a.PackSize,country.Name,rwr.CustomsDeclarationNumber,		
		-- �������������� ������ (��� �����������)
	    ag.Id,ag.Name,_st.Id,_st.Name,_st.StorageTypeId,_aot.Id,_aot.Name,t.Id,t.Name,_ut.Id,_ut.Name,_ct.Id,_ct.Name,o.Id,
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
			
			join #AvailableDealListForSaleTable ad on ad.Id = ssi.DealId
			-- ����������� �� ���������� ������
			join #AvailableStorageListTable _st on _st.Id = ssi.StorageId	-- ����������� �� ��
			join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
		ag.Id,	-- ArticleGroupId 
		ag.Name,	-- ArticleGroupName
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
		join ArticleGroup ag on ag.Id = a.ArticleGroupId 				
		join Deal d on d.Id = asi.DealId
		join Contract c on c.Id = d.ClientContractId
		join Country country on country.Id = rwr.CountryId
		join Team t on t.Id = asi.TeamId
		join Organization o on o.Id = asi.ClientOrganizationId
		join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
		join Contractor contractor on contractor.Id = rw.ProviderId
		-- ����������� �� ������
		join #AvailableDealListForSaleTable ad on ad.Id = asi.DealId
		-- ����������� �� ���������� ������
		join #AvailableStorageListTable _st on _st.Id = asi.StorageId	-- ����������� �� ��
		join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
		join #AvailableClientListTable _ct on _ct.Id = asi.ClientId	-- ����������� �� ��������
		join #AvailableUserListTable _ut on _ut.Id = asi.UserId	-- ����������� �� �������������
		join #AvailableAccountOrganizationListTable _aot on _aot.Id = asi.AccountOrganizationId	-- ����������� �� ����������� ��������
	GROUP BY a.Id, a.Number,a.FullName,asi.BatchId,rw.Number,rw.Date,a.PackSize,country.Name,rwr.CustomsDeclarationNumber,		
		-- �������������� ������ (��� �����������)
		ag.Id,ag.Name,_st.Id,_st.Name,_st.StorageTypeId,_aot.Id,_aot.Name,t.Id,t.Name,_ut.Id,_ut.Name,_ct.Id,_ct.Name,o.Id,
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
			
			join #AvailableDealListForSaleTable ad on ad.Id = asi.DealId
			-- ����������� �� ���������� ������
			join #AvailableStorageListTable _st on _st.Id = asi.StorageId	-- ����������� �� ��
			join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
				-- ����������� �� ������
				join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId 
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
					-- ����������� �� ������
					join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
					join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ����������� �� ��������
					join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId -- ����������� �� �������������
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
				ag.Id,	-- ArticleGroupId 
				ag.Name,	-- ArticleGroupName
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
				join ArticleGroup ag on ag.Id = a.ArticleGroupId 
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
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
				-- ����������� �� ������
				join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
					-- ����������� �� ������
					join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
				ag.Id,	-- ArticleGroupId 
				ag.Name,	-- ArticleGroupName
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
				join ArticleGroup ag on ag.Id = a.ArticleGroupId 
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
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
				-- ����������� �� ������
				join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId
				join #AvailableDealListForSaleTable ad on ad.Id = rfci.DealId
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
					-- ����������� �� ������
					join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId
					join #AvailableDealListForSaleTable ad on ad.Id = rfci.DealId
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
				ag.Id,	-- ArticleGroupId 
				ag.Name,	-- ArticleGroupName
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
				join ArticleGroup ag on ag.Id = a.ArticleGroupId 
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Country country on country.Id = rwr.CountryId		
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				join #AvailableDealListForSaleTable ad on ad.Id = rfci.DealId
				-- ����������� �� ���������� ������
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
				-- ����������� �� ������
				join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId
				-- ����������� �� ���������� ������
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
					-- ����������� �� ������
					join #AvailableDealListForReturnTable adl on adl.Id = rfci.DealId
					-- ����������� �� ���������� ������
					join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
					join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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
				ag.Id,	-- ArticleGroupId 
				ag.Name,	-- ArticleGroupName
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
				join ArticleGroup ag on ag.Id = a.ArticleGroupId 
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				-- ����������� �� ���������� ������
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ����������� �� ��
				join #AvailableArticleListTable _at on _at.Id = a.Id	-- ����������� �� ����� �������
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

	-- ��������� �������������� ������ �� ������ �� ��������� �������
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
DROP INDEX IX_#AvailableDealListForSaleTable on #AvailableDealListForSaleTable
DROP INDEX IX_#AvailableClientListTable ON #AvailableClientListTable
DROP INDEX IX_#AvailableDealListForReturnTable ON #AvailableDealListForReturnTable

DROP TABLE #AvailableStorageListTable
DROP TABLE #AvailableArticleListTable
DROP TABLE #AvailableClientListTable
DROP TABLE #AvailableUserListTable
DROP TABLE #AvailableAccountOrganizationListTable
DROP TABLE #AvailableDealListForSaleTable
DROP TABLE #AvailableDealListForReturnTable
DROP TABLE #ResultFlatTable

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

