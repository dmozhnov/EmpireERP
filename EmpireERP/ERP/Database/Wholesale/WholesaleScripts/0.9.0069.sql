/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.69

  Что нового:	
	* правильные учетные цены цены в хранимых процедурах для выгрузки в 1С
		
---------------------------------------------------------------------------------------*/
--SET NOEXEC OFF	-- выполнить данную команду в случае неуспешного обновления
SET DATEFORMAT DMY
SET NOCOUNT ON
SET ARITHABORT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

DECLARE @PreviousVersion varchar(15),	-- номер предыдущей версии
		@CurrentVersion varchar(15),	-- номер текущей версии базы данных
		@NewVersion varchar(15),		-- номер новой версии
		@DataBaseName varchar(256),		-- текущая база данных
		@CurrentDate nvarchar(10),		-- текущая дата
		@CurrentTime nvarchar(10),		-- текущее время
		@BackupTarget nvarchar(100)		-- куда делать бэкап базы данных

SET @PreviousVersion = '0.9.68' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.69'			-- номер новой версии

SELECT @CurrentVersion = DataBaseVersion FROM Setting
IF @@ERROR <> 0
BEGIN
	PRINT 'Неверная база данных'
END
ELSE
BEGIN
	-- СОЗДАЕМ БЭКАП БАЗЫ ДАННЫХ
	-- Получаем текущую дату
	SET @CurrentDate = CONVERT(nvarchar(20), GETDATE(), 104)	--	dd.mm.yyyy
	SET @CurrentTime = REPLACE(CONVERT(nvarchar(20), GETDATE(), 108) , ':', '.') --	hh:mm:ss
	SET @DataBaseName = DB_NAME()

	SET @BackupTarget = N'D:\Bizpulse\Backup\Update\' + @DataBaseName + '(' + CAST(@CurrentVersion as nvarchar(20)) + ') ' + 
		@CurrentDate + ' ' + @CurrentTime + N'.bak'

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = N'Обновление версии', NOFORMAT

	IF @@ERROR <> 0
	BEGIN
		PRINT 'Ошибка создания backup''а. Продолжение выполнения невозможно.'
	END
	ELSE
		BEGIN

		IF (@CurrentVersion <> @PreviousVersion)
		BEGIN
			PRINT 'Обновить базу данных ' + @DataBaseName + ' до версии ' + @NewVersion + 
				' можно только из версии  ' + @PreviousVersion +
				'. Текущая версия: ' + @CurrentVersion
		END
		ELSE
		BEGIN
			--Начинаем транзакцию
			BEGIN TRAN

			--Обновляем версию базы данных
			UPDATE Setting 
			SET DataBaseVersion = @NewVersion		
		END
	END
END
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


/*********************************************************************************************
Процедура: 	GetExpenditureWaybillsForExportTo1C

Описание:	Получение списка операций реализации товаров для выгрузки в 1С

Параметры:
	@StartDate	дата начала периода выборки
	@EndDate	дата окончания периода выборки
	@UserId		код пользователя
	@IdSenderAccountOrganizationList     cписок выбранных кодов собственных организаций-отправителей 
	@AllSenderAccountOrganizations признак выбора всех собственных организаций-отправителей аккаунта
	@IdRecipientAccountOrganizationList     cписок выбранных кодов собственных организаций-получателей 
	@AllRecipientAccountOrganizations признак выбора всех собственных организаций-получателей аккаунта
	@ShowTransferToCommissionaires показывать передачу  на коммисию
	
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


-- НАКЛАДНЫЕ РЕАЛИЗАЦИИ
-- доступные пользователю сделки
CREATE TABLE #AvailableDeals
(
	DealId int not null
)

insert into #AvailableDeals (DealId)
exec GetAvailableDeals @UserId = @UserId, @PermissionId = 3601 -- право на просмотр списка накладных реализации товаров

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
		then con.Name + ' №' + con.Number + ' от ' + CONVERT(varchar(10), con.Date, 104)
		else 'Договор с комиссионером №1'
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
	AND ExpenditureWaybillStateId = 7 ---статус отгружено 

IF @ShowTransferToCommission = 1
BEGIN		
	-- ПЕРЕМЕЩЕНИЯ ТОВАРОВ
	DECLARE @permissionDistributionTypeId TINYINT
	SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId) -- право на просмотр списка накладных перемещения

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
			,'П' + left(mwb.[Number],24) 
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
			,'Договор с комиссионером №1'
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

		LEFT JOIN ArticleAccountingPrice aap ON aap.[Id] = mwbr.MovementWaybillRecipientArticleAccountingPriceId
		JOIN #AvailableSenders ac1 ON ac1.Id = aO.Id 
		JOIN #AvailableRecipients ac2 ON ac2.Id = clAO.Id 

		WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId] <> mwb.[RecipientId] -- отправитель и получатель не должны совпадать
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


	-- СМЕНЫ СОБСТВЕННИКА
	SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(2001, @UserId) -- право на просмотр списка накладных смены собственника

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
				((Round(isnull(aap.AccountingPrice,0) * mwbr.[MovingCount],2) / (1 + vatR.[Value]/100))), 2)
			,artGr.[NameFor1C]
			,mu.[NumericCode] 
			,mu.[ShortName] 
			,0
			,'Договор с комиссионером №1'
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


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

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


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

