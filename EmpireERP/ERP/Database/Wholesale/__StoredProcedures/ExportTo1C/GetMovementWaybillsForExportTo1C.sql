/*********************************************************************************************
Процедура: 	GetMovementWaybillsForExportTo1C

Описание:	Получение списка операций внутреннего перемещения товаров для выгрузки в 1С

Параметры:
	@StartDate	дата начала периода выборки
	@EndDate	дата окончания периода выборки
	@UserId		код пользователя
	@IdSenderAccountingOrganizationList     cписок выбранных кодов собственных организаций-отправителей 
	@AllSenderAccountOrganizations признак выбора всех собственных организаций-отправителей аккаунта

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
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId) -- право на просмотр списка накладных перемещения

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
		,'П' + left(mwb.[Number],24) 
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

	LEFT JOIN ArticleAccountingPrice aap ON aap.[Id] = mwbr.MovementWaybillRecipientArticleAccountingPriceId

	JOIN #AvailableAccountOrganizations ac1 ON ac1.Id = aO.Id 

	WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId]= mwb.[RecipientId] -- организации должны совпадать
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