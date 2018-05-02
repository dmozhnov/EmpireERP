/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.66

  ��� ������:
	* �������� ��������� GetMovementWaybillsForExportTo1C
	* �������� ��������� GetIncomingWaybillsForExportTo1C
	* �������� ��������� GetReturnFromClientWaybillsForExportTo1C
	* �������� ��������� GetExpenditureWaybillsForExportTo1C
	
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

SET @PreviousVersion = '0.9.65' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.66'			-- ����� ����� ������

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

/*********************************************************************************************
���������: 	GetMovementWaybillsForExportTo1C

��������:	��������� ������ �������� ����������� ����������� ������� ��� �������� � 1�

���������:
	@StartDate	���� ������ ������� �������
	@EndDate	���� ��������� ������� �������
	@UserId		��� ������������
	@IdSenderAccountingOrganizationList     c����� ��������� ����� ����������� �����������-������������ 
	@AllSenderAccountOrganizations ������� ������ ���� ����������� �����������-������������ ��������

*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetMovementWaybillsForExportTo1C'
)
   DROP PROCEDURE dbo.GetMovementWaybillsForExportTo1C
GO

CREATE PROCEDURE [dbo].[GetMovementWaybillsForExportTo1C]
(
	@StartDate datetime,
	@EndDate datetime,
	@UserId int,
	@IdSenderAccountOrganizationList VARCHAR(8000),	
	@AllSenderAccountOrganizations BIT
)
AS

SET DATEFORMAT DMY
SET NOCOUNT ON

CREATE TABLE #AvailableAccountOrganizations(
	Id INT 
)

IF @AllSenderAccountOrganizations = 1
	BEGIN
		INSERT INTO #AvailableAccountOrganizations(id)
		SELECT Id
		FROM dbo.AccountOrganization
	END
ELSE
	BEGIN
						
	    INSERT INTO #AvailableAccountOrganizations(id)
		SELECT Id
		FROM [dbo].SplitIntIdList(@IdSenderAccountOrganizationList)			
	END

CREATE TABLE #Waybills(
	 SenderStorageId smallint 
	,SenderStorageName varchar(200) 
	,RecipientStorageId smallint 
	,RecipientStorageName varchar(200) 
    ,SenderShortName varchar(100) 
	,SenderFullName varchar(250)
	,SenderINN varchar(12)
	,SenderKPP varchar(9)
	,MovementWaybillNumber varchar(25)
	,MovementWaybillDate datetime
	,MovementWaybillSalePriceSum decimal(18,2)
	,ArticleCount decimal(18,6)
	,AccountingPrice decimal(18,2)
	,AccountingSum decimal(18,2) 
	,ValueAddedTax decimal(4,2)
	,ValueAddedTaxSum decimal(18,2)
	,ArticleGroupName varchar(200)
	,MeasureUnitNumericCode varchar(3)
	,MeasureUnitShortName varchar(7)
	,IsOwner bit
)

DECLARE @permissionDistributionTypeId TINYINT
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId) -- ����� �� �������� ������ ��������� �����������

IF ISNULL(@permissionDistributionTypeId,0)<> 0
	INSERT INTO #Waybills
	(
		 SenderStorageId
		,RecipientStorageId 
		,MovementWaybillNumber
		,MovementWaybillDate
		,MovementWaybillSalePriceSum
		,AccountingPrice
		,ArticleCount
		,AccountingSum
		,ValueAddedTax
		,ValueAddedTaxSum
		,ArticleGroupName
		,MeasureUnitNumericCode
		,MeasureUnitShortName
		,SenderShortName
		,SenderFullName
		,SenderINN
		,SenderKPP
		,IsOwner
	)
	SELECT 
		mwb.[SenderStorageId]
		,mwb.[RecipientStorageId]
		,'�' + left(mwb.[Number],24) 
		,case when CONVERT(varchar(10), mwb.[Date], 104) =  CONVERT(varchar(10), mwb.[CreationDate], 104) 
				then CONVERT(varchar(10), mwb.[CreationDate], 104) + ' ' + CONVERT(varchar(10), mwb.[CreationDate], 108) 
				else CONVERT(varchar(10), mwb.[Date], 104) + ' 23:59:59'
		end 
		,mwb.[SenderAccountingPriceSum] 
		,isnull(aap.AccountingPrice,0)
		,mwbr.[MovingCount]
		,Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2)
		,vatR.[Value] 
		,Round(Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) - 
			((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2) as  SaleWaybillRowSaleSumTax
		,artGr.[NameFor1C] 
		,mu.[NumericCode] 
		,mu.[ShortName] 
		,OrgForAc.[ShortName]
		,OrgForAc.[FullName]
		,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
		,AOJP.[KPP] 
		,AO.SalesOwnArticle  
	FROM [MovementWaybill] mwb
	JOIN [MovementWaybillRow] mwbr ON mwbr.[MovementWaybillId] = mwb.[Id] AND mwbr.[DeletionDate] IS NULL
	JOIN [Article] art ON mwbr.[ArticleId] = art.[Id]
	JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
	JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
	JOIN [ValueAddedTax] vatR ON mwbr.[ValueAddedTaxId] = vatR.[Id]

	LEFT JOIN [AccountOrganization] AO ON mwb.[SenderId] = AO.[Id]
	LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
	LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
	LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
	LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id

	LEFT JOIN ArticleAccountingPrice aap ON aap.[Id] = mwbr.MovementWaybillSenderArticleAccountingPriceId

	JOIN #AvailableAccountOrganizations ac1 ON ac1.Id = aO.Id 

	WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId]= mwb.[RecipientId] -- ����������� ������ ���������
		  AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
		  AND mwb.MovementWaybillStateId in (9, 11)
		  AND (
				@permissionDistributionTypeId = 3 
				OR
				(
					@permissionDistributionTypeId = 1 AND mwb.CuratorId = @UserId AND EXISTS
					(
						SELECT *
						FROM [User] u
						JOIN [UserTeam] ut on ut.UserId = u.Id
						JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
						JOIN [Storage] s on s.Id = ts.StorageId
						WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
					)
				) 
				OR
				(
					@permissionDistributionTypeId = 2 AND EXISTS
					(
						SELECT *
						FROM [User] u
						JOIN [UserTeam] ut on ut.UserId = u.Id
						JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
						JOIN [Storage] s on s.Id = ts.StorageId
						WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
					)
				)
			)

UPDATE t 
SET t.SenderStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.SenderStorageId = st.[Id] 

UPDATE t 
SET t.RecipientStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.RecipientStorageId = st.[Id] 

select
	 SenderStorageId
	,SenderStorageName
	,RecipientStorageId
	,RecipientStorageName
	,SenderShortName
	,SenderFullName
	,SenderINN 
	,SenderKPP
	,MovementWaybillNumber
	,MovementWaybillDate
	,MovementWaybillSalePriceSum
	,sum(isnull(ArticleCount,0)) as ArticleCount
	,Round(case when sum(isnull(ArticleCount,0)) = 0 then 0 
		else sum(isnull(AccountingSum,0))/sum(isnull(ArticleCount,0))
	 end,2) as	AccountingPrice
	,sum(isnull(AccountingSum,0)) as AccountingSum
	,ValueAddedTax
	,sum(isnull(ValueAddedTaxSum,0)) as ValueAddedTaxSum
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,IsOwner
from #Waybills
group by SenderStorageId
	,SenderStorageName
	,RecipientStorageId
	,RecipientStorageName
	,SenderShortName
	,SenderFullName
	,SenderINN 
	,SenderKPP
	,MovementWaybillNumber
	,MovementWaybillDate
	,MovementWaybillSalePriceSum
	,ValueAddedTax
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,IsOwner
order by MovementWaybillDate, MovementWaybillNumber
		 
drop table #Waybills
drop TABLE #AvailableAccountOrganizations

GO


-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

/*********************************************************************************************
���������: 	GetIncomingWaybillsForExportTo1C

��������:	��������� ������ ��������� ����������� ������� ��� �������� � 1�

���������:
	@StartDate	���� ������ ������� �������
	@EndDate	���� ��������� ������� �������
	@UserId		��� ������������
	@IdSenderAccountOrganizationList     c����� ��������� ����� ����������� �����������-������������ 
	@AllSenderAccountOrganizations ������� ������ ���� ����������� �����������-������������ ��������
	@IdRecipientAccountOrganizationList     c����� ��������� ����� ����������� �����������-����������� 
	@AllRecipientAccountOrganizations ������� ������ ���� ����������� �����������-����������� ��������
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetIncomingWaybillsForExportTo1C'
)
   DROP PROCEDURE dbo.GetIncomingWaybillsForExportTo1C
GO

CREATE PROCEDURE [dbo].GetIncomingWaybillsForExportTo1C
(
	@StartDate datetime,
	@EndDate datetime,
	@UserId int,
	@IdSenderAccountOrganizationList VARCHAR(8000),
	@AllSenderAccountOrganizations BIT,
	@IdRecipientAccountOrganizationList VARCHAR(8000), 
	@AllRecipientAccountOrganizations BIT	
)
AS

SET DATEFORMAT DMY
SET NOCOUNT ON

CREATE TABLE #AvailableAccountOrganizationListTable(
	Id INT 
)
CREATE TABLE #AvailableRecipientAccountOrganizationListTable(
	Id INT 
)

IF @AllSenderAccountOrganizations = 1
	BEGIN
		INSERT INTO #AvailableAccountOrganizationListTable(id)
		SELECT Id
		FROM dbo.AccountOrganization
	END
ELSE
	BEGIN
		
		INSERT INTO #AvailableAccountOrganizationListTable(id)
		SELECT Id
		FROM [dbo].SplitIntIdList(@IdSenderAccountOrganizationList)
					
	END



IF @AllRecipientAccountOrganizations = 1
	BEGIN
		INSERT INTO #AvailableRecipientAccountOrganizationListTable(id)
		SELECT Id
		FROM dbo.AccountOrganization
	END
ELSE
	BEGIN
		INSERT INTO #AvailableRecipientAccountOrganizationListTable(id)
		SELECT Id
		FROM [dbo].SplitIntIdList(@IdRecipientAccountOrganizationList)
					
	END

create table #Waybills(
	SenderStorageId smallint ---1
	,SenderStorageName varchar(200) ---2
	,RecipientStorageId smallint ---1
	,RecipientStorageName varchar(200) ---2
	,SenderShortName varchar(100) 
	,SenderFullName varchar(250)
	,SenderINN varchar(12)
	,SenderKPP varchar(9)
	,RecipientShortName varchar(100)
	,RecipientFullName varchar(250)
	,RecipientINN varchar(12)
	,RecipientKPP varchar(9)
	,IncomingWaybillNumber varchar(25)
	,IncomingWaybillDate datetime
	,IncomingWaybillSalePriceSum decimal(18,2)
	,ArticleCount decimal(18,6)
	,AccountingPrice decimal(18,2)
	,AccountingSum decimal(18,2) 
	,ValueAddedTax decimal(4,2)
	,ValueAddedTaxSum decimal(18,2)
	,ArticleGroupName varchar(200)
	,MeasureUnitNumericCode varchar(3)
	,MeasureUnitShortName varchar(7)
	,IsOwner bit
)

DECLARE @permissionDistributionTypeId TINYINT	-- ���������� ����� �� �������� �������������

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0

		insert into #Waybills
		(
			 SenderStorageId
			,RecipientStorageId 
			,IncomingWaybillNumber
			,IncomingWaybillDate
			,IncomingWaybillSalePriceSum
			,AccountingPrice
			,ArticleCount
			,AccountingSum
			,ValueAddedTax
			,ValueAddedTaxSum
			,ArticleGroupName
			,MeasureUnitNumericCode
			,MeasureUnitShortName
			,SenderShortName
			,SenderFullName
			,SenderINN
			,SenderKPP
			,RecipientShortName
			,RecipientFullName
			,RecipientINN
			,RecipientKPP
			,IsOwner
		)

		SELECT 
			    mwb.[SenderStorageId]
			   ,mwb.[RecipientStorageId]		
			   ,'�' + left(mwb.[Number],24) 
			   ,case when CONVERT(varchar(10), mwb.[Date], 104) =  CONVERT(varchar(10), mwb.[CreationDate], 104) 
						then CONVERT(varchar(10), mwb.[CreationDate], 104) + ' ' + CONVERT(varchar(10), mwb.[CreationDate], 108) 
						else CONVERT(varchar(10), mwb.[Date], 104) + ' 23:59:59'
			   end 
			  ,mwb.[RecipientAccountingPriceSum] 
			  ,isnull(aap.AccountingPrice,0)
			  ,mwbr.[MovingCount]
			  ,Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2)
			  ,vatR.[Value] 
			  ,Round(Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) - 
					((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2) 
			  ,artGr.[NameFor1C] 
			  ,mu.[NumericCode]
			  ,mu.[ShortName]
			  ,OrgForAc.[ShortName]
			  ,OrgForAc.[FullName]
			  ,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
			  ,AOJP.[KPP] 
			  ,clOrgForAc.[ShortName]
			  ,clOrgForAc.[FullName]
			  ,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
			  ,clAOJP.[KPP]
			  ,AO.SalesOwnArticle  
		  FROM [dbo].[MovementWaybill] mwb
		  JOIN [dbo].[MovementWaybillRow] mwbr ON mwbr.[MovementWaybillId] = mwb.[Id] AND mwbr.[DeletionDate] IS NULL
		  JOIN [Article] art ON mwbr.[ArticleId] = art.[Id]
		  JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
		  JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
		  JOIN [ValueAddedTax] vatR ON mwbr.[ValueAddedTaxId] = vatR.[Id]
		  
		  LEFT JOIN [AccountOrganization] AO ON mwb.[SenderId] = AO.[Id]
		  LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
		  LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
		  LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
		  LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id
		 

		  LEFT JOIN [AccountOrganization] clAO ON mwb.[RecipientId] = clAO.[Id]
		  LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
		  LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
		  LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
		  LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id
		  
		  LEFT JOIN ArticleAccountingPrice aap ON
		  aap.[Id] = mwbr.MovementWaybillRecipientArticleAccountingPriceId
		 
		  JOIN #AvailableAccountOrganizationListTable ac1 ON 
		  ac1.Id = aO.Id 
		  
		  JOIN #AvailableRecipientAccountOrganizationListTable ac2 ON 
		  ac2.Id = clAO.Id 
		  
		  WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId]<> mwb.[RecipientId]
		  AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
		  AND mwb.MovementWaybillStateId in (9,11)
		  AND (
				@permissionDistributionTypeId = 3 
				OR
				(
					@permissionDistributionTypeId = 1 AND mwb.CuratorId = @UserId AND EXISTS
					(
						SELECT *
						FROM [User] u
						JOIN [UserTeam] ut on ut.UserId = u.Id
						JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
						JOIN [Storage] s on s.Id = ts.StorageId
						WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
					)
				) 
				OR
				(
					@permissionDistributionTypeId = 2 AND EXISTS
					(
						SELECT *
						FROM [User] u
						JOIN [UserTeam] ut on ut.UserId = u.Id
						JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
						JOIN [Storage] s on s.Id = ts.StorageId
						WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
					 )
				)
			)		

SET @permissionDistributionTypeId = 0

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(2001, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0
  
		  insert into #Waybills
		(
			 SenderStorageId
			,RecipientStorageId  
			,IncomingWaybillNumber
			,IncomingWaybillDate
			,IncomingWaybillSalePriceSum
			,AccountingPrice
			,ArticleCount
			,AccountingSum
			,ValueAddedTax
			,ValueAddedTaxSum
			,ArticleGroupName
			,MeasureUnitNumericCode
			,MeasureUnitShortName
			,SenderShortName
			,SenderFullName
			,SenderINN
			,SenderKPP
			,RecipientShortName
			,RecipientFullName
			,RecipientINN
			,RecipientKPP
			,IsOwner
		)

		SELECT 
			    mwb.[ChangeOwnerWaybillStorageId]		
			   ,mwb.[ChangeOwnerWaybillStorageId]		
			   ,'�' + left(mwb.[Number],24) 
			   ,case when CONVERT(varchar(10), mwb.[Date], 104) =  CONVERT(varchar(10), mwb.[CreationDate], 104) 
						then CONVERT(varchar(10), mwb.[CreationDate], 104) + ' ' + CONVERT(varchar(10), mwb.[CreationDate], 108) 
						else CONVERT(varchar(10), mwb.[Date], 104) + ' 23:59:59'
			   end 
			  ,mwb.[AccountingPriceSum]
			  ,isnull(aap.AccountingPrice,0)
			  ,mwbr.[MovingCount]
			  ,Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2)
			  ,vatR.[Value]
			  ,Round(Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) - 
					((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2) as  SaleWaybillRowSaleSumTax
			  ,artGr.[NameFor1C]
			  ,mu.[NumericCode]
			  ,mu.[ShortName]
			  ,OrgForAc.[ShortName]
			  ,OrgForAc.[FullName]
			  ,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
			  ,AOJP.[KPP] 
			  ,clOrgForAc.[ShortName]
			  ,clOrgForAc.[FullName]
			  ,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
			  ,clAOJP.[KPP]
			  ,AO.SalesOwnArticle 
		  FROM [dbo].[ChangeOwnerWaybill] mwb
		  JOIN [dbo].[ChangeOwnerWaybillRow] mwbr ON mwbr.[ChangeOwnerWaybillId] = mwb.[Id] AND mwbr.[DeletionDate] IS NULL
		  JOIN [Article] art ON mwbr.[ArticleId] = art.[Id]
		  JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
		  JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
		  JOIN [ValueAddedTax] vatR ON mwbr.[ChangeOwnerWaybillRowValueAddedTaxId] = vatR.[Id]
		  
		  LEFT JOIN [AccountOrganization] AO ON mwb.[ChangeOwnerWaybillSenderId] = AO.[Id]
		  LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
		  LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
		  LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
		  LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id
		 
		  LEFT JOIN [AccountOrganization] clAO ON mwb.[ChangeOwnerWaybillRecipientId] = clAO.[Id]
		  LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
		  LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
		  LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
		  LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id
		  
		  LEFT JOIN ArticleAccountingPrice aap ON
		  aap.[Id] = mwbr.ChangeOwnerWaybillRowArticleAccountingPriceId
		  
		  JOIN #AvailableAccountOrganizationListTable ac1 ON
		  ac1.Id = aO.Id 
		  
		  JOIN #AvailableRecipientAccountOrganizationListTable ac2 ON 
		  ac2.Id = clAO.Id 
		  
		  WHERE mwb.[DeletionDate] IS NULL 
		  AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
		  AND mwb.ChangeOwnerWaybillStateId = 8
		  AND (
				@permissionDistributionTypeId = 3 
				OR
				(
					@permissionDistributionTypeId = 1 AND mwb.ChangeOwnerWaybillCuratorId = @UserId AND EXISTS
					(
						SELECT *
						FROM [User] u
						JOIN [UserTeam] ut on ut.UserId = u.Id
						JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
						JOIN [Storage] s on s.Id = ts.StorageId
						WHERE u.Id = @UserId AND ts.StorageId = mwb.ChangeOwnerWaybillStorageId
					)
				) 
				OR
				(
					@permissionDistributionTypeId = 2 AND EXISTS
					(
						SELECT *
						FROM [User] u
						JOIN [UserTeam] ut on ut.UserId = u.Id
						JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
						JOIN [Storage] s on s.Id = ts.StorageId
						WHERE u.Id = @UserId AND ts.StorageId  = mwb.ChangeOwnerWaybillStorageId
					)
				 )
			)
	
UPDATE t 
SET t.SenderStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.SenderStorageId = st.[Id] 

UPDATE t 
SET t.RecipientStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.RecipientStorageId = st.[Id] 
  
select 
	 SenderStorageId
	,SenderStorageName
	,RecipientStorageId
	,RecipientStorageName
	,SenderShortName
	,SenderFullName
	,SenderINN 
	,SenderKPP
	,RecipientShortName
	,RecipientFullName
	,RecipientINN	
	,RecipientKPP
	,IncomingWaybillNumber
	,IncomingWaybillDate
	,IncomingWaybillSalePriceSum
	,sum(isnull(ArticleCount,0)) as ArticleCount
	,Round(case when sum(isnull(ArticleCount,0)) = 0 then 0 
		else sum(isnull(AccountingSum,0))/sum(isnull(ArticleCount,0))
	 end,2) as	AccountingPrice
	,sum(isnull(AccountingSum,0)) as AccountingSum
	,ValueAddedTax
	,sum(isnull(ValueAddedTaxSum,0)) as ValueAddedTaxSum
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,IsOwner
from #Waybills
group by  SenderStorageId
	,SenderStorageName
	,RecipientStorageId
	,RecipientStorageName
	,SenderShortName
	,SenderFullName
	,SenderINN 
	,SenderKPP
	,RecipientShortName
	,RecipientFullName
	,RecipientINN	
	,RecipientKPP
	,IncomingWaybillNumber
	,IncomingWaybillDate
	,IncomingWaybillSalePriceSum
	,ValueAddedTax
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,IsOwner
order by IncomingWaybillDate,
		 IncomingWaybillNumber
		 
drop table #Waybills

drop TABLE #AvailableAccountOrganizationListTable
drop TABLE #AvailableRecipientAccountOrganizationListTable

GO

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

/*********************************************************************************************
���������: 	GetReturnFromClientWaybillsForExportTo1C

��������:	��������� ������ ��������� ���������� ������� ��� �������� � 1�

���������:
	@StartDate	���� ������ ������� �������
	@EndDate	���� ��������� ������� �������
	@UserId		��� ������������
	@IdSenderAccountOrganizationList     c����� ��������� ����� ����������� �����������-������������ 
	@AllSenderAccountOrganizations ������� ������ ���� ����������� �����������-������������ ��������
	@IdRecipientAccountOrganizationList     c����� ��������� ����� ����������� �����������-����������� 
	@AllRecipientAccountOrganizations ������� ������ ���� ����������� �����������-����������� ��������
	@ShowReturnsFromCommissionaires ���������� �� �������� �� �������������
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetReturnFromClientWaybillsForExportTo1C'
)
   DROP PROCEDURE dbo.GetReturnFromClientWaybillsForExportTo1C
GO

CREATE PROCEDURE [dbo].[GetReturnFromClientWaybillsForExportTo1C]
(
	@StartDate datetime,
	@EndDate datetime,
	@UserId int,
	@IdSenderAccountOrganizationList VARCHAR(8000),
	@AllSenderAccountOrganizations BIT,
	@IdRecipientAccountOrganizationList VARCHAR(8000),  
	@AllRecipientAccountOrganizations BIT,
	@ShowReturnsFromCommissionaires BIT
)
AS

SET DATEFORMAT DMY
SET NOCOUNT ON

CREATE TABLE #AvailableAccountOrganizationListTable(
	Id INT 
)

CREATE TABLE #AvailableRecipientAccountOrganizationListTable(
	Id INT 
)

IF @AllRecipientAccountOrganizations = 1
	BEGIN
		INSERT INTO #AvailableRecipientAccountOrganizationListTable(id)
		SELECT Id
		FROM dbo.AccountOrganization
	END
ELSE
	BEGIN
		INSERT INTO #AvailableRecipientAccountOrganizationListTable(id)
		SELECT Id
		FROM SplitIntIdList(@IdRecipientAccountOrganizationList)			
	END

IF @AllSenderAccountOrganizations = 1
	BEGIN
		INSERT INTO #AvailableAccountOrganizationListTable(id)
		SELECT Id
		FROM dbo.AccountOrganization
	END
ELSE
	BEGIN
		
		INSERT INTO #AvailableAccountOrganizationListTable(id)
		SELECT Id
		FROM [dbo].SplitIntIdList(@IdSenderAccountOrganizationList)
					
	END

CREATE TABLE #AvailableDeals
(
	DealId int not null
)

insert into #AvailableDeals (DealId)
exec GetAvailableDeals @UserId = @UserId, @PermissionId = 3901 -- ����� �� �������� ������ ��������� �������� �������


create table #Waybills(
	 RecipientStorageId smallint ---1
	,RecipientStorageName varchar(200) ---2
	,SenderStorageId smallint ---1
	,SenderStorageName varchar(200) ---2
	,ContractId smallint -->3---
	,ContractName varchar(100)
	,SenderShortName varchar(100) 
	,SenderFullName varchar(250)
	,SenderINN varchar(12)
	,SenderKPP varchar(9)
    ,RecipientShortName varchar(100)
	,RecipientFullName varchar(250)
	,RecipientINN varchar(12)
	,RecipientKPP varchar(9)
	,ClientOrganizationShortName varchar(100)
	,ClientOrganizationFullName varchar(250)
	,ClientOrganizationINN varchar(12)
	,ClientOrganizationKPP varchar(9)
	,ReturnWaybillNumber varchar(25)
	,ReturnWaybillDate datetime
	,ReturnWaybillSalePriceSum decimal(18,2)
	,ArticleCount decimal(18,6)
	,SalePrice decimal(18,2)
	,SaleSum decimal(18,2) 
	,ValueAddedTax decimal(4,2)
	,ValueAddedTaxSum decimal(18,2)
	,ArticleGroupName varchar(200)
	,MeasureUnitNumericCode varchar(3)
	,MeasureUnitShortName varchar(7)
	,ExpenditureWaybillNumber varchar(25)
	,ExpenditureWaybillDate datetime
	,IsOwner bit
	,IsCommission bit
)

DECLARE @permissionDistributionTypeId TINYINT	-- ���������� ����� �� �������� �������������

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(3901, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0

	insert into #Waybills
	(
		 RecipientStorageId
		,SenderStorageId
		,ReturnWaybillNumber
		,ReturnWaybillDate
		,ReturnWaybillSalePriceSum
		,SalePrice
		,ArticleCount
		,SaleSum
		,ValueAddedTax
		,ValueAddedTaxSum
		,ArticleGroupName
		,MeasureUnitNumericCode
		,MeasureUnitShortName
		,ContractId
		,ContractName
		,RecipientShortName
		,RecipientFullName
		,RecipientINN
		,RecipientKPP
		,ClientOrganizationShortName
		,ClientOrganizationFullName
		,ClientOrganizationINN
		,ClientOrganizationKPP
		,ExpenditureWaybillNumber
		,ExpenditureWaybillDate
		,IsOwner
		,IsCommission 
	)
	select
		   sw.ReturnFromClientWaybillRecipientStorageId
		  ,0	
		  ,sw.[Number]
		  ,case when CONVERT(varchar(10), sw.[Date], 104) =  CONVERT(varchar(10), sw.[CreationDate], 104) 
					then CONVERT(varchar(10), sw.[CreationDate], 104) + ' ' + CONVERT(varchar(10), sw.[CreationDate], 108) 
					else CONVERT(varchar(10), sw.[Date], 104) + ' 23:59:59'
		   end
		  ,sw.[SalePriceSum]
		  ,saleRow.[SalePrice]
		  ,Round(swr.[ReturnCount],6)
		  ,Round(swr.[ReturnCount] * saleRow.[SalePrice], 2)
		  ,vatR.[Value] as SaleWaybillRowValueAddedTax
		  ,Round(Round(swr.[ReturnCount] * saleRow.[SalePrice], 2) - 
				((Round(swr.[ReturnCount] * saleRow.[SalePrice], 2) / (1 + vatR.[Value]/100))), 2)
		  ,artGr.[NameFor1C]
		  ,mu.[NumericCode]
		  ,mu.[ShortName]
		  ,d.[ClientContractId]
		  ,case when isnull(con.[Id],0) <> 0 
					then con.Name + ' �' + con.Number + ' �� ' + CONVERT(varchar(10), con.Date, 104)
					else '������� � ������������� �1'
		   end	
		  ,OrgForAc.[ShortName]
		  ,OrgForAc.[FullName]
		  ,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
		  ,AOJP.[KPP] 
		  ,clOrgForAc.[ShortName]
		  ,clOrgForAc.[FullName]
		  ,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
		  ,clAOJP.[KPP]
		  ,sale.[Number] 
		  ,case when CONVERT(varchar(10), sale.[Date], 104) =  CONVERT(varchar(10), sale.[CreationDate], 104) 
					then CONVERT(varchar(10), sale.[CreationDate], 104) + ' ' + CONVERT(varchar(10), sale.[CreationDate], 108) 
					else CONVERT(varchar(10), sale.[Date], 104) + ' 23:59:59' 
			end 
		  ,AO.SalesOwnArticle
		  ,0
	  FROM [ReturnFromClientWaybill] sw
	  JOIN [ReturnFromClientWaybillRow] swr ON sw.[Id] = swr.[ReturnFromClientWaybillId] AND swr.[DeletionDate] IS NULL
	  
	  JOIN [Article] art ON swr.[ArticleId] = art.[Id]
	  JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
	  JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
	  JOIN dbo.[Deal] d ON sw.[ReturnFromClientWaybillDealId] = d.Id
	  JOIN #AvailableDeals AD on AD.DealId = D.Id
	  LEFT JOIN [ClientContract] clC ON d.[ClientContractId] = clC.[Id]
	  LEFT JOIN [Contract] con ON clC.[Id] = con.[Id] AND con.DeletionDate IS NULL

	  LEFT JOIN [AccountOrganization] AO ON con.[AccountOrganizationId] = AO.[Id]
	  LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
	  LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
	  LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
	  LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id
	  
	  LEFT JOIN [ClientContract] clclC ON d.[ClientContractId] = clclC.[Id]
	  LEFT JOIN [Contract] clcon ON clclC.[Id] = clcon.[Id] AND clcon.DeletionDate IS NULL

	  LEFT JOIN [ContractorOrganization] clAO ON clcon.[ContractorOrganizationId] = clAO.[Id]
	  LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
	  LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
	  LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
	  LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id
	  
	  INNER JOIN [SaleWaybillRow] saleRow ON saleRow.[Id] = swr.[SaleWaybillRowId] AND saleRow.[DeletionDate] IS NULL
	  INNER JOIN [SaleWaybill] sale ON sale.Id = saleRow.[SaleWaybillId] AND sale.[DeletionDate] IS NULL
	  JOIN [ValueAddedTax] vatR ON saleRow.[ValueAddedTaxId] = vatR.[Id]
	  
	  JOIN #AvailableRecipientAccountOrganizationListTable ac1 ON
	  ac1.Id = aO.Id
	  WHERE sw.[DeletionDate] IS NULL
		AND sw.[Date] >= @StartDate AND sw.[Date] <= @EndDate
		AND ReturnFromClientWaybillStateId = 3 


IF @ShowReturnsFromCommissionaires = 1
BEGIN		
	SET @permissionDistributionTypeId = 0	
		
	SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId)

	IF ISNULL(@permissionDistributionTypeId,0)<> 0

			insert into #Waybills
			(
				 SenderStorageId
				,RecipientStorageId
				,ContractId
				,ContractName
				,ReturnWaybillNumber
				,ReturnWaybillDate
				,ReturnWaybillSalePriceSum
				,SalePrice
				,ArticleCount
				,SaleSum
				,ValueAddedTax
				,ValueAddedTaxSum
				,ArticleGroupName
				,MeasureUnitNumericCode
				,MeasureUnitShortName
				,SenderShortName
				,SenderFullName
				,SenderINN
				,SenderKPP
				,RecipientShortName
				,RecipientFullName
				,RecipientINN
				,RecipientKPP
				,IsOwner
				,IsCommission 
			)

			SELECT 
					mwb.[SenderStorageId]
				   ,mwb.[RecipientStorageId]
				   ,0
				   ,'������� � ������������� �1'		
				   ,'�' + left(mwb.[Number],24) 
				   ,case when CONVERT(varchar(10), mwb.[Date], 104) =  CONVERT(varchar(10), mwb.[CreationDate], 104) 
							then CONVERT(varchar(10), mwb.[CreationDate], 104) + ' ' + CONVERT(varchar(10), mwb.[CreationDate], 108) 
							else CONVERT(varchar(10), mwb.[Date], 104) + ' 23:59:59'
				   end 
				  ,mwb.[RecipientAccountingPriceSum] 
				  ,isnull(aap.AccountingPrice,0)
				  ,mwbr.[MovingCount]
				  ,Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2)
				  ,vatR.[Value] 
				  ,Round(Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) - 
						((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2) 
				  ,artGr.[NameFor1C] 
				  ,mu.[NumericCode]
				  ,mu.[ShortName]
				  ,OrgForAc.[ShortName]
				  ,OrgForAc.[FullName]
				  ,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
				  ,AOJP.[KPP] 
				  ,clOrgForAc.[ShortName]
				  ,clOrgForAc.[FullName]
				  ,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
				  ,clAOJP.[KPP]
				  ,AO.SalesOwnArticle
				  ,1  
			  FROM [dbo].[MovementWaybill] mwb
			  JOIN [dbo].[MovementWaybillRow] mwbr ON mwbr.[MovementWaybillId] = mwb.[Id] AND mwbr.[DeletionDate] IS NULL
			  JOIN [Article] art ON mwbr.[ArticleId] = art.[Id]
			  JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
			  JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
			  JOIN [ValueAddedTax] vatR ON mwbr.[ValueAddedTaxId] = vatR.[Id]
			  
			  LEFT JOIN [AccountOrganization] AO ON mwb.[SenderId] = AO.[Id]
			  LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
			  LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
			  LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
			  LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id
			 

			  LEFT JOIN [AccountOrganization] clAO ON mwb.[RecipientId] = clAO.[Id]
			  LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
			  LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
			  LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
			  LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id
			  
			  LEFT JOIN ArticleAccountingPrice aap ON
			  aap.[Id] = mwbr.MovementWaybillRecipientArticleAccountingPriceId
			 
			  JOIN #AvailableAccountOrganizationListTable ac1 ON 
			  ac1.Id = aO.Id 
			  
			  JOIN #AvailableRecipientAccountOrganizationListTable ac2 ON 
			  ac2.Id = clAO.Id 
			  
			  WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId]<> mwb.[RecipientId]
			  AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
			  AND mwb.MovementWaybillStateId in (9,11)
			  AND (
					@permissionDistributionTypeId = 3 
					OR
					(
						@permissionDistributionTypeId = 1 AND mwb.CuratorId = @UserId AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
						)
					) 
					OR
					(
						@permissionDistributionTypeId = 2 AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
						)
					)
				)



	SET @permissionDistributionTypeId = 0

	SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(2001, @UserId)

	IF ISNULL(@permissionDistributionTypeId,0)<> 0
	  
			  insert into #Waybills
			(
				 SenderStorageId
				,RecipientStorageId  
				,ContractId
				,ContractName
				,ReturnWaybillNumber
				,ReturnWaybillDate
				,ReturnWaybillSalePriceSum
				,SalePrice
				,ArticleCount
				,SaleSum
				,ValueAddedTax
				,ValueAddedTaxSum
				,ArticleGroupName
				,MeasureUnitNumericCode
				,MeasureUnitShortName
				,SenderShortName
				,SenderFullName
				,SenderINN
				,SenderKPP
				,RecipientShortName
				,RecipientFullName
				,RecipientINN
				,RecipientKPP
				,IsOwner
				,IsCommission 
			)

			SELECT 
					mwb.[ChangeOwnerWaybillStorageId]		
				   ,mwb.[ChangeOwnerWaybillStorageId]
				   ,0
				   ,'������� � ������������� �1'
				   ,'�' + left(mwb.[Number],24) 
				   ,case when CONVERT(varchar(10), mwb.[Date], 104) =  CONVERT(varchar(10), mwb.[CreationDate], 104) 
							then CONVERT(varchar(10), mwb.[CreationDate], 104) + ' ' + CONVERT(varchar(10), mwb.[CreationDate], 108) 
							else CONVERT(varchar(10), mwb.[Date], 104) + ' 23:59:59'
				   end 
				  ,mwb.[AccountingPriceSum]
				  ,isnull(aap.AccountingPrice,0)
				  ,mwbr.[MovingCount]
				  ,Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2)
				  ,vatR.[Value]
				  ,Round(Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) - 
						((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2) as  SaleWaybillRowSaleSumTax
				  ,artGr.[NameFor1C]
				  ,mu.[NumericCode]
				  ,mu.[ShortName]
				  ,OrgForAc.[ShortName]
				  ,OrgForAc.[FullName]
				  ,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
				  ,AOJP.[KPP] 
				  ,clOrgForAc.[ShortName]
				  ,clOrgForAc.[FullName]
				  ,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
				  ,clAOJP.[KPP]
				  ,AO.SalesOwnArticle
				  ,1 
			  FROM [dbo].[ChangeOwnerWaybill] mwb
			  JOIN [dbo].[ChangeOwnerWaybillRow] mwbr ON mwbr.[ChangeOwnerWaybillId] = mwb.[Id] AND mwbr.[DeletionDate] IS NULL
			  JOIN [Article] art ON mwbr.[ArticleId] = art.[Id]
			  JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
			  JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
			  JOIN [ValueAddedTax] vatR ON mwbr.[ChangeOwnerWaybillRowValueAddedTaxId] = vatR.[Id]
			  
			  LEFT JOIN [AccountOrganization] AO ON mwb.[ChangeOwnerWaybillSenderId] = AO.[Id]
			  LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
			  LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
			  LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
			  LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id
			 
			  LEFT JOIN [AccountOrganization] clAO ON mwb.[ChangeOwnerWaybillRecipientId] = clAO.[Id]
			  LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
			  LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
			  LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
			  LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id
			  
			  LEFT JOIN ArticleAccountingPrice aap ON
			  aap.[Id] = mwbr.ChangeOwnerWaybillRowArticleAccountingPriceId
			  
			  JOIN #AvailableAccountOrganizationListTable ac1 ON
			  ac1.Id = aO.Id 
			  
			  JOIN #AvailableRecipientAccountOrganizationListTable ac2 ON 
			  ac2.Id = clAO.Id 
			  
			  WHERE mwb.[DeletionDate] IS NULL 
			  AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
			  AND mwb.ChangeOwnerWaybillStateId = 8
			  AND (
					@permissionDistributionTypeId = 3 
					OR
					(
						@permissionDistributionTypeId = 1 AND mwb.ChangeOwnerWaybillCuratorId = @UserId AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId = mwb.ChangeOwnerWaybillStorageId
						)
					) 
					OR
					(
						@permissionDistributionTypeId = 2 AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId  = mwb.ChangeOwnerWaybillStorageId
						)
					 )
				)
		
END	
UPDATE t 
SET t.RecipientStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.RecipientStorageId = st.[Id] 

UPDATE t 
SET t.SenderStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.SenderStorageId = st.[Id] 
  
select
	 SenderStorageId
	,SenderStorageName 
	,RecipientStorageId
	,RecipientStorageName
	,ContractId
	,ContractName
	,SenderShortName
	,SenderFullName
	,SenderINN
	,SenderKPP
    ,RecipientShortName
	,RecipientFullName
	,RecipientINN	
	,RecipientKPP
	,ClientOrganizationShortName
	,ClientOrganizationFullName
	,ClientOrganizationINN
	,ClientOrganizationKPP
	,ReturnWaybillNumber
	,ReturnWaybillDate
	,ReturnWaybillSalePriceSum
	,sum(isnull(ArticleCount,0)) as ArticleCount
	,Round(case when sum(isnull(ArticleCount,0)) = 0 then 0 
		else sum(isnull(SaleSum,0))/sum(isnull(ArticleCount,0))
	 end,2) as	SalePrice
	,sum(isnull(SaleSum,0)) as SaleSum
	,ValueAddedTax
	,sum(isnull(ValueAddedTaxSum,0)) as ValueAddedTaxSum
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,ExpenditureWaybillNumber
	,case when ExpenditureWaybillDate is null then '20000101'
		  else ExpenditureWaybillDate
	 end as ExpenditureWaybillDate		 		
	,IsOwner
	,IsCommission
from #Waybills
group by 
	 SenderStorageId
	,SenderStorageName 
	,RecipientStorageId
	,RecipientStorageName
	,ContractId
	,ContractName
	,SenderShortName
	,SenderFullName
	,SenderINN
	,SenderKPP
    ,RecipientShortName
	,RecipientFullName
	,RecipientINN	
	,RecipientKPP
	,ClientOrganizationShortName
	,ClientOrganizationFullName
	,ClientOrganizationINN
	,ClientOrganizationKPP
	,ReturnWaybillNumber
	,ReturnWaybillDate
	,ReturnWaybillSalePriceSum
	,ValueAddedTax
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,ExpenditureWaybillNumber
	,case when ExpenditureWaybillDate is null then '20000101'
		  else ExpenditureWaybillDate
	 end
	,IsOwner
	,IsCommission
order by ReturnWaybillDate,
		 ReturnWaybillNumber
		 
drop table #Waybills

drop TABLE #AvailableRecipientAccountOrganizationListTable
drop TABLE #AvailableAccountOrganizationListTable

GO

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

/*********************************************************************************************
���������: 	GetExpenditureWaybillsForExportTo1C

��������:	��������� ������ �������� ���������� ������� ��� �������� � 1�

���������:
	@StartDate	���� ������ ������� �������
	@EndDate	���� ��������� ������� �������
	@UserId		��� ������������
	@IdSenderAccountOrganizationList     c����� ��������� ����� ����������� �����������-������������ 
	@AllSenderAccountOrganizations ������� ������ ���� ����������� �����������-������������ ��������
	@IdRecipientAccountOrganizationList     c����� ��������� ����� ����������� �����������-����������� 
	@AllRecipientAccountOrganizations ������� ������ ���� ����������� �����������-����������� ��������
	@ShowTransferToCommissionaires ���������� ��������  �� ��������
	
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetExpenditureWaybillsForExportTo1C'
)
   DROP PROCEDURE dbo.GetExpenditureWaybillsForExportTo1C
GO

CREATE PROCEDURE [dbo].[GetExpenditureWaybillsForExportTo1C]
(
	@StartDate datetime,
	@EndDate datetime,
	@UserId int,
	@IdSenderAccountOrganizationList VARCHAR(8000),	
	@AllSenderAccountOrganizations BIT,
	@IdRecipientAccountOrganizationList VARCHAR(8000), 
	@AllRecipientAccountOrganizations BIT,
	@ShowTransferToCommission BIT		
)
AS

SET DATEFORMAT DMY
SET NOCOUNT ON

CREATE TABLE #AvailableSenders(
	Id INT 
)
CREATE TABLE #AvailableRecipients(
	Id INT 
)

IF @AllSenderAccountOrganizations = 1
	BEGIN
		INSERT INTO #AvailableSenders(id)
		SELECT Id
		FROM dbo.AccountOrganization
	END
ELSE
	BEGIN
		INSERT INTO #AvailableSenders(id)
		SELECT Id
		FROM [dbo].SplitIntIdList(@IdSenderAccountOrganizationList)							
	END

IF @AllRecipientAccountOrganizations = 1
	BEGIN
		INSERT INTO #AvailableRecipients(id)
		SELECT Id
		FROM dbo.AccountOrganization
	END
ELSE
	BEGIN					
	    INSERT INTO #AvailableRecipients(id)
		SELECT Id
		FROM [dbo].SplitIntIdList(@IdRecipientAccountOrganizationList)
	END

CREATE TABLE #Waybills(
	SenderStorageId smallint
	,SenderStorageName varchar(200)
	,ContractId smallint
	,ContractName varchar(100)
    ,SenderShortName varchar(100) 
	,SenderFullName varchar(250)
	,SenderINN varchar(12)
	,SenderKPP varchar(9)
	,RecipientShortName varchar(100)
	,RecipientFullName varchar(250)
	,RecipientINN varchar(12)
	,RecipientKPP varchar(9)
	,ClientOrganizationShortName varchar(100)
	,ClientOrganizationFullName varchar(250)
	,ClientOrganizationINN varchar(12)
	,ClientOrganizationKPP varchar(9)
	,SaleWaybillNumber varchar(25)
	,SaleWaybillDate datetime
	,SaleWaybillSalePriceSum decimal(18,2)
	,ArticleCount decimal(18,6)
	,SalePrice decimal(18,2)
	,SaleSum decimal(18,2) 
	,ValueAddedTax decimal(4,2)
	,ValueAddedTaxSum decimal(18,2)
	,ArticleGroupName varchar(200)
	,MeasureUnitNumericCode varchar(3)
	,MeasureUnitShortName varchar(7)
	,IsCommission bit
	,IsOwner bit
)


-- ��������� ����������
-- ��������� ������������ ������
CREATE TABLE #AvailableDeals
(
	DealId int not null
)

insert into #AvailableDeals (DealId)
exec GetAvailableDeals @UserId = @UserId, @PermissionId = 3601 -- ����� �� �������� ������ ��������� ���������� �������

insert into #Waybills
(
	 SenderStorageId
	,SaleWaybillNumber
	,SaleWaybillDate
	,SaleWaybillSalePriceSum
	,SalePrice
	,ArticleCount
	,SaleSum
	,ValueAddedTax
	,ValueAddedTaxSum
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,ContractId
	,ContractName
	,SenderShortName
	,SenderFullName
	,SenderINN
	,SenderKPP
	,ClientOrganizationShortName
	,ClientOrganizationFullName
	,ClientOrganizationINN
	,ClientOrganizationKPP
	,IsCommission
	,IsOwner
)
select
	ew.[ExpenditureWaybillSenderStorageId]	
	,sw.[Number] 
	,case when CONVERT(varchar(10), sw.[Date], 104) =  CONVERT(varchar(10), sw.[CreationDate], 104) 
		then CONVERT(varchar(10), sw.[CreationDate], 104) + ' ' + CONVERT(varchar(10), sw.[CreationDate], 108) 
		else CONVERT(varchar(10), sw.[Date], 104) + ' 23:59:59'
	end 
	,ew.[SalePriceSum] 
	,swr.[SalePrice] 
	,Round(swr.[SellingCount],6) 
	,Round(swr.[SellingCount] * swr.[SalePrice], 2)
	,vatR.[Value] 
	,Round(Round(swr.[SellingCount] * swr.[SalePrice], 2) - 
		((Round(swr.[SellingCount] * swr.[SalePrice], 2) / (1 + vatR.[Value]/100))), 2)
	,artGr.[NameFor1C] 
	,mu.[NumericCode] 
	,mu.[ShortName] 
	,d.[ClientContractId]
	,case when isnull(con.[Id], 0) <> 0 
		then con.Name + ' �' + con.Number + ' �� ' + CONVERT(varchar(10), con.Date, 104)
		else '������� � ������������� �1'
	end			
	,OrgForAc.[ShortName]
	,OrgForAc.[FullName]
	,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
	,AOJP.[KPP] 
	,clOrgForAc.[ShortName]
	,clOrgForAc.[FullName]
	,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
	,clAOJP.[KPP]
	,0 
	,AO.SalesOwnArticle 
FROM [SaleWaybill] sw
JOIN [SaleWaybillRow] swr ON sw.[Id] = swr.[SaleWaybillId] AND swr.[DeletionDate] IS NULL
JOIN [ExpenditureWaybill] ew ON sw.[Id] = ew.[Id]
JOIN [ExpenditureWaybillRow] ewr ON swr.[Id] = ewr.[Id]
JOIN [ValueAddedTax] vatR ON swr.[ValueAddedTaxId] = vatR.[Id]
JOIN [Article] art ON swr.[ArticleId] = art.[Id]
JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
JOIN [Deal] d ON sw.[DealId] = d.Id
JOIN #AvailableDeals AD on AD.DealId = D.Id
LEFT JOIN [ClientContract] clC ON d.[ClientContractId] = clC.[Id]
LEFT JOIN [Contract] con ON clC.[Id] = con.[Id] AND con.DeletionDate IS NULL

LEFT JOIN [AccountOrganization] AO ON con.[AccountOrganizationId] = AO.[Id]
LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id

LEFT JOIN [ContractorOrganization] clAO ON con.[ContractorOrganizationId] = clAO.[Id]
LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id

JOIN #AvailableSenders ac1 ON ac1.Id = aO.Id	
WHERE sw.[DeletionDate] IS NULL
	AND sw.[Date] >= @StartDate AND sw.[Date] <= @EndDate
	AND ExpenditureWaybillStateId = 7 ---������ ��������� 

IF @ShowTransferToCommission = 1
BEGIN		
	-- ����������� �������
	DECLARE @permissionDistributionTypeId TINYINT
	SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId) -- ����� �� �������� ������ ��������� �����������

	IF @permissionDistributionTypeId <> 0
		insert into #Waybills
		(
			SenderStorageId
			,SaleWaybillNumber
			,SaleWaybillDate
			,SaleWaybillSalePriceSum
			,SalePrice
			,ArticleCount
			,SaleSum
			,ValueAddedTax
			,ValueAddedTaxSum
			,ArticleGroupName
			,MeasureUnitNumericCode
			,MeasureUnitShortName
			,ContractId
			,ContractName
			,SenderShortName
			,SenderFullName
			,SenderINN
			,SenderKPP
			,RecipientShortName
			,RecipientFullName
			,RecipientINN
			,RecipientKPP
			,IsCommission
			,IsOwner
		)
		SELECT 
			mwb.[SenderStorageId]		
			,'�' + left(mwb.[Number],24) 
			,case when CONVERT(varchar(10), mwb.[Date], 104) =  CONVERT(varchar(10), mwb.[CreationDate], 104) 
				then CONVERT(varchar(10), mwb.[CreationDate], 104) + ' ' + CONVERT(varchar(10), mwb.[CreationDate], 108) 
				else CONVERT(varchar(10), mwb.[Date], 104) + ' 23:59:59'
			end as SaleWaybillDate
			,mwb.[SenderAccountingPriceSum]
			,isnull(aap.AccountingPrice,0)
			,mwbr.[MovingCount]
			,Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2)
			,vatR.[Value] 
			,Round(Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) - 
				((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2) 
			,artGr.[NameFor1C]
			,mu.[NumericCode] 
			,mu.[ShortName]
			,0
			,'������� � ������������� �1'
			,OrgForAc.[ShortName]
			,OrgForAc.[FullName]
			,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
			,AOJP.[KPP] 
			,clOrgForAc.[ShortName]
			,clOrgForAc.[FullName]
			,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
			,clAOJP.[KPP]
			,1
			,AO.SalesOwnArticle  
		FROM [MovementWaybill] mwb
		JOIN [MovementWaybillRow] mwbr ON mwbr.[MovementWaybillId] = mwb.[Id] AND mwbr.[DeletionDate] IS NULL
		JOIN [Article] art ON mwbr.[ArticleId] = art.[Id]
		JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
		JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
		JOIN [ValueAddedTax] vatR ON mwbr.[ValueAddedTaxId] = vatR.[Id]

		LEFT JOIN [AccountOrganization] AO ON mwb.[SenderId] = AO.[Id]
		LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
		LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
		LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
		LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id

		LEFT JOIN [AccountOrganization] clAO ON mwb.[RecipientId] = clAO.[Id]
		LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
		LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
		LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
		LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id

		LEFT JOIN ArticleAccountingPrice aap ON aap.[Id] = mwbr.MovementWaybillSenderArticleAccountingPriceId
		JOIN #AvailableSenders ac1 ON ac1.Id = aO.Id 
		JOIN #AvailableRecipients ac2 ON ac2.Id = clAO.Id 

		WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId] <> mwb.[RecipientId] -- ����������� � ���������� �� ������ ���������
			AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
			AND mwb.MovementWaybillStateId in (9, 11)
			AND (
					@permissionDistributionTypeId = 3 
					OR
					(
						@permissionDistributionTypeId = 1 AND mwb.CuratorId = @UserId AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
						)
					) 
					OR
					(
						@permissionDistributionTypeId = 2 AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
						)
					)
				)


	-- ����� ������������
	SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(2001, @UserId) -- ����� �� �������� ������ ��������� ����� ������������

	IF @permissionDistributionTypeId <> 0
		insert into #Waybills
		(
			 SenderStorageId
			,SaleWaybillNumber
			,SaleWaybillDate
			,SaleWaybillSalePriceSum
			,SalePrice
			,ArticleCount
			,SaleSum
			,ValueAddedTax
			,ValueAddedTaxSum
			,ArticleGroupName
			,MeasureUnitNumericCode
			,MeasureUnitShortName
			,ContractId
			,ContractName
			,SenderShortName
			,SenderFullName
			,SenderINN
			,SenderKPP
			,RecipientShortName
			,RecipientFullName
			,RecipientINN
			,RecipientKPP
			,IsCommission
			,IsOwner
		)
		SELECT 
			mwb.[ChangeOwnerWaybillStorageId]		
			,'�' + left(mwb.[Number],24) 
			,case when CONVERT(varchar(10), mwb.[Date], 104) =  CONVERT(varchar(10), mwb.[CreationDate], 104) 
				then CONVERT(varchar(10), mwb.[CreationDate], 104) + ' ' + CONVERT(varchar(10), mwb.[CreationDate], 108) 
				else CONVERT(varchar(10), mwb.[Date], 104) + ' 23:59:59'
			end 
			,mwb.[AccountingPriceSum] 
			,isnull(aap.AccountingPrice,0)
			,mwbr.[MovingCount]
			,Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2)
			,vatR.[Value] 
			,Round(Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) - 
				((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2)
			,artGr.[NameFor1C]
			,mu.[NumericCode] 
			,mu.[ShortName] 
			,0
			,'������� � ������������� �1'
			,OrgForAc.[ShortName]
			,OrgForAc.[FullName]
			,case when AOJP.[INN] is null then AOPP.[INN] else AOJP.[INN] end 
			,AOJP.[KPP] 
			,clOrgForAc.[ShortName]
			,clOrgForAc.[FullName]
			,case when clAOJP.[INN] is null then clAOPP.[INN] else clAOJP.[INN] end 
			,clAOJP.[KPP]
			,1
			,AO.SalesOwnArticle 
		FROM [ChangeOwnerWaybill] mwb
		JOIN [ChangeOwnerWaybillRow] mwbr ON mwbr.[ChangeOwnerWaybillId] = mwb.[Id] AND mwbr.[DeletionDate] IS NULL
		JOIN [Article] art ON mwbr.[ArticleId] = art.[Id]
		JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
		JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
		JOIN [ValueAddedTax] vatR ON mwbr.[ChangeOwnerWaybillRowValueAddedTaxId] = vatR.[Id]

		LEFT JOIN [AccountOrganization] AO ON mwb.[ChangeOwnerWaybillSenderId] = AO.[Id]
		LEFT JOIN [Organization] OrgForAc ON AO.[Id] = OrgForAc.[Id] AND OrgForAc.[DeletionDate] IS NULL
		LEFT JOIN [EconomicAgent] EcAForAc ON OrgForAc.[EconomicAgentId] = EcAForAc.[Id]
		LEFT JOIN JuridicalPerson AOJP ON AOJP.Id = EcAForAc.Id
		LEFT JOIN PhysicalPerson AOPP ON AOPP.Id = EcAForAc.Id

		LEFT JOIN [AccountOrganization] clAO ON mwb.[ChangeOwnerWaybillRecipientId] = clAO.[Id]
		LEFT JOIN [Organization] clOrgForAc ON clAO.[Id] = clOrgForAc.[Id] AND clOrgForAc.[DeletionDate] IS NULL
		LEFT JOIN [EconomicAgent] clEcAForAc ON clOrgForAc.[EconomicAgentId] = clEcAForAc.[Id]
		LEFT JOIN JuridicalPerson clAOJP ON clAOJP.Id = clEcAForAc.Id
		LEFT JOIN PhysicalPerson clAOPP ON clAOPP.Id = clEcAForAc.Id

		LEFT JOIN ArticleAccountingPrice aap ON aap.[Id] = mwbr.ChangeOwnerWaybillRowArticleAccountingPriceId

		JOIN #AvailableSenders ac1 ON ac1.Id = aO.Id 
		JOIN #AvailableRecipients ac2 ON ac2.Id = clAO.Id 

		WHERE mwb.[DeletionDate] IS NULL 
			AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
			AND mwb.ChangeOwnerWaybillStateId = 8
			AND (
					@permissionDistributionTypeId = 3 
					OR
					(
						@permissionDistributionTypeId = 1 AND mwb.ChangeOwnerWaybillCuratorId = @UserId AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId = mwb.ChangeOwnerWaybillStorageId
						)
					) 
					OR
					(
						@permissionDistributionTypeId = 2 AND EXISTS
						(
							SELECT *
							FROM [User] u
							JOIN [UserTeam] ut on ut.UserId = u.Id
							JOIN [TeamStorage] ts on ts.TeamId = ut.TeamId
							JOIN [Storage] s on s.Id = ts.StorageId
							WHERE u.Id = @UserId AND ts.StorageId  = mwb.ChangeOwnerWaybillStorageId
						)
					 )
				)

END
	
UPDATE t 
SET t.SenderStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.SenderStorageId = st.[Id] 
  
select 
	 SenderStorageId
	,SenderStorageName
	,ContractId
	,ContractName
    ,SenderShortName
	,SenderFullName
	,SenderINN 
	,SenderKPP
	,RecipientShortName
	,RecipientFullName
	,RecipientINN	
	,RecipientKPP
	,ClientOrganizationShortName
	,ClientOrganizationFullName
	,ClientOrganizationINN
	,ClientOrganizationKPP
	,SaleWaybillNumber
	,SaleWaybillDate
	,SaleWaybillSalePriceSum
	,sum(isnull(ArticleCount,0)) as ArticleCount
	,Round(case when sum(isnull(ArticleCount,0)) = 0 then 0 
		else sum(isnull(SaleSum,0))/sum(isnull(ArticleCount,0))
	 end,2) as	SalePrice
	,sum(isnull(SaleSum,0)) as SaleSum
	,ValueAddedTax
	,sum(isnull(ValueAddedTaxSum,0)) as ValueAddedTaxSum
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,IsCommission
	,IsOwner
from #Waybills
group by SenderStorageId
	,SenderStorageName
	,ContractId
	,ContractName
    ,SenderShortName
	,SenderFullName
	,SenderINN 
	,SenderKPP
	,RecipientShortName
	,RecipientFullName
	,RecipientINN
	,RecipientKPP
	,ClientOrganizationShortName
	,ClientOrganizationFullName
	,ClientOrganizationINN
	,ClientOrganizationKPP
	,SaleWaybillNumber
	,SaleWaybillDate
	,SaleWaybillSalePriceSum
	,ValueAddedTax
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,IsCommission
	,IsOwner
order by SaleWaybillDate, SaleWaybillNumber
		 
drop table #Waybills

drop table #AvailableSenders
drop table #AvailableRecipients

drop table #AvailableDeals

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

