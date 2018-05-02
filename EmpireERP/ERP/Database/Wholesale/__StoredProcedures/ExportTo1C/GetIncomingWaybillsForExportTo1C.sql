/*********************************************************************************************
Процедура: 	GetIncomingWaybillsForExportTo1C

Описание:	Получение списка накладных поступления товаров для выгрузки в 1С

Параметры:
	@StartDate	дата начала периода выборки
	@EndDate	дата окончания периода выборки
	@UserId		код пользователя
	@IdSenderAccountOrganizationList     cписок выбранных кодов собственных организаций-отправителей 
	@AllSenderAccountOrganizations признак выбора всех собственных организаций-отправителей аккаунта
	@IdRecipientAccountOrganizationList     cписок выбранных кодов собственных организаций-получателей 
	@AllRecipientAccountOrganizations признак выбора всех собственных организаций-получателей аккаунта
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

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр пользователей

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
			   ,'П' + left(mwb.[Number],24) 
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
			   ,'С' + left(mwb.[Number],24) 
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