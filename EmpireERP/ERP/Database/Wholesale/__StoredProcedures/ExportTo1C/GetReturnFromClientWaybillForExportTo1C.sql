/*********************************************************************************************
ѕроцедура: 	GetReturnFromClientWaybillsForExportTo1C

ќписание:	ѕолучение списка накладных реализации товаров дл€ выгрузки в 1—

ѕараметры:
	@StartDate	дата начала периода выборки
	@EndDate	дата окончани€ периода выборки
	@UserId		код пользовател€
	@IdSenderAccountOrganizationList     cписок выбранных кодов собственных организаций-отправителей 
	@AllSenderAccountOrganizations признак выбора всех собственных организаций-отправителей аккаунта
	@IdRecipientAccountOrganizationList     cписок выбранных кодов собственных организаций-получателей 
	@AllRecipientAccountOrganizations признак выбора всех собственных организаций-получателей аккаунта
	@ShowReturnsFromCommissionaires показывать ли возвраты от комиссионеров
	@IdRecipientCommissionaireOrganizationList     cписок выбранных кодов организаций комиссионеров(получатели возвратов) по которым необходимо учитывать возвраты от клиентов 
	@AllRecipientCommissionaireOrganizations признак выбора всех организаций комиссионеров(получатели возвратов) по которым необходимо учитывать возвраты от клиентов
	@ShowReturnsAcceptedByCommissionaires показывать ли возвраты прин€тые комиссионерами(получатели возвратов) от клиентов
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
	@ShowReturnsFromCommissionaires BIT,
	@IdRecipientCommissionaireOrganizationList VARCHAR(8000),  
	@AllRecipientCommissionaireOrganizations BIT,
	@ShowReturnsAcceptedByCommissionaires BIT
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

CREATE TABLE #AvailableCommissionaireOrganizationListTable(
	Id INT 
)

CREATE TABLE #AvailableRecipientAndCommissionaireAccountOrganizationListTable(
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

IF @ShowReturnsAcceptedByCommissionaires = 1
BEGIN
	IF @AllRecipientCommissionaireOrganizations = 1
		BEGIN
			INSERT INTO #AvailableCommissionaireOrganizationListTable(id)
			SELECT Id
			FROM dbo.AccountOrganization
		END
	ELSE
		BEGIN
			INSERT INTO #AvailableCommissionaireOrganizationListTable(id)
			SELECT Id
			FROM SplitIntIdList(@IdRecipientCommissionaireOrganizationList)			
		END
END

INSERT INTO #AvailableRecipientAndCommissionaireAccountOrganizationListTable(id)
			SELECT id
			FROM #AvailableRecipientAccountOrganizationListTable

IF @ShowReturnsAcceptedByCommissionaires = 1
BEGIN
	INSERT INTO #AvailableRecipientAndCommissionaireAccountOrganizationListTable(id)
	SELECT id
	FROM #AvailableCommissionaireOrganizationListTable
	WHERE id NOT IN (
					 SELECT id FROM #AvailableRecipientAndCommissionaireAccountOrganizationListTable
					 )

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
exec GetAvailableDeals @UserId = @UserId, @PermissionId = 3901 -- право на просмотр списка накладных возврата товаров


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

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр пользователей

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
					then con.Name + ' є' + con.Number + ' от ' + CONVERT(varchar(10), con.Date, 104)
					else 'ƒоговор с комиссионером є1'
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
	  
	  JOIN #AvailableRecipientAndCommissionaireAccountOrganizationListTable ac1 ON
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
				   ,'ƒоговор с комиссионером є1'		
				   ,'ѕ' + left(mwb.[Number],24) 
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
				   ,'ƒоговор с комиссионером є1'
				   ,'—' + left(mwb.[Number],24) 
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
			  AND mwb.ChangeOwnerWaybillSenderId <> mwb.ChangeOwnerWaybillRecipientId
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
drop TABLE #AvailableCommissionaireOrganizationListTable
drop TABLE #AvailableRecipientAndCommissionaireAccountOrganizationListTable
	


GO