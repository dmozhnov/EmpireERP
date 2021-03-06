/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.60

  Что нового:
	+ добавлен столбец-признак  "Собственная организации реализует собственный товар"  в справочник 
	  собственных организаций 
	
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

SET @PreviousVersion = '0.9.59' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.60'			-- номер новой версии

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

ALTER TABLE dbo.AccountOrganization ADD SalesOwnArticle bit NULL
	
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

UPDATE dbo.AccountOrganization SET SalesOwnArticle = 1

IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

ALTER TABLE dbo.AccountOrganization ALTER COLUMN SalesOwnArticle bit NOT NULL

IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30201, 1)

IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


create function SplitIntIdList 
(
	@p_SourceText varchar(8000)
)
RETURNS @retTable TABLE 
(
	Id int 
)
AS
BEGIN
 DECLARE @w_Continue  int
  ,@w_StartPos  int
  ,@w_Length  int
  ,@w_Delimeter_pos int
  ,@w_tmp_int  int
  ,@w_tmp_num  numeric(18,3)
  ,@w_tmp_txt   varchar(2000)
  ,@w_Delimeter_Len tinyint
  ,@p_Delimeter varchar(100)
 
 set @p_Delimeter = '_'
 
 if len(@p_SourceText) = 0
 begin
	SET  @w_Continue = 0 -- force early exit
 end 
 else
 begin
	-- parse the original @p_SourceText array into a temp table
	SET  @w_Continue = 1
	SET @w_StartPos = 1
	SET @p_SourceText = RTRIM( LTRIM( @p_SourceText))
	SET @w_Length   = DATALENGTH( RTRIM( LTRIM( @p_SourceText)))
	SET @w_Delimeter_Len = len(@p_Delimeter)
end
WHILE @w_Continue = 1
BEGIN
	SET @w_Delimeter_pos = CHARINDEX( @p_Delimeter
      ,(SUBSTRING( @p_SourceText, @w_StartPos
      ,((@w_Length - @w_StartPos) + @w_Delimeter_Len)))
      )
 
	IF @w_Delimeter_pos > 0  -- delimeter(s) found, get the value
	BEGIN
		SET @w_tmp_txt = LTRIM(RTRIM( SUBSTRING( @p_SourceText, @w_StartPos 
			,(@w_Delimeter_pos - 1)) ))
		
		if isnumeric(@w_tmp_txt) = 1
		begin
			set @w_tmp_int = cast( cast(@w_tmp_txt as numeric) as int)
			set @w_tmp_num = cast( @w_tmp_txt as numeric(18,3))
		end
		else
		begin
			set @w_tmp_int =  null
			set @w_tmp_num =  null
		end
		SET @w_StartPos = @w_Delimeter_pos + @w_StartPos + (@w_Delimeter_Len- 1)
	END
	ELSE -- No more delimeters, get last value
	BEGIN
		SET @w_tmp_txt = LTRIM(RTRIM( SUBSTRING( @p_SourceText, @w_StartPos 
		,((@w_Length - @w_StartPos) + @w_Delimeter_Len)) ))
		
		if isnumeric(@w_tmp_txt) = 1
		begin
			set @w_tmp_int = cast( cast(@w_tmp_txt as numeric) as int)
			set @w_tmp_num = cast( @w_tmp_txt as numeric(18,3))
		end
		else
		begin
			set @w_tmp_int =  null
			set @w_tmp_num =  null
		end
		
		SELECT @w_Continue = 0
	END
  
	INSERT INTO @retTable VALUES( @w_tmp_num )
END

RETURN

END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE FUNCTION dbo.GetPermissionDistributionType
(
	@PermissionId SMALLINT,	-- Право
	@UserId INT -- Код пользователя
)
RETURNS TINYINT
AS
BEGIN
	RETURN (SELECT ISNULL(MAX(PermissionDistributionTypeId), 0)
	FROM [User] u
	join [UserRole] ur on ur.UserId = u.Id AND u.Id = @UserId
	join [PermissionDistribution] pd on pd.RoleId = ur.RoleId  AND pd.PermissionId = @PermissionId)
END

GO



CREATE PROCEDURE [dbo].[GetExpenditureWaybillsForExportTo1C]
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
	,ContractId smallint -->3---
	,ContractName varchar(200)
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

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр пользователей

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(3601, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0

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
			  ,con.Name + ' №' + con.Number + ' от ' + CONVERT(varchar(10), con.Date, 104)
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
		  JOIN dbo.[Deal] d ON sw.[DealId] = d.Id
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
		  
		  JOIN #AvailableAccountOrganizationListTable ac1 ON
		  ac1.Id = aO.Id
		WHERE sw.[DeletionDate] IS NULL
			AND sw.[Date] >= @StartDate AND sw.[Date] <= @EndDate
			AND ExpenditureWaybillStateId = 7 ---статус отгруженно 
			AND (
			 @permissionDistributionTypeId = 3 OR
			(@permissionDistributionTypeId = 2 AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamDeal] td on td.TeamId = ut.TeamId
				WHERE
					u.Id = @UserId AND td.DealId = d.Id
			 )) OR
			(@permissionDistributionTypeId = 1 AND sw.SaleWaybillCuratorId = @UserId AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamDeal] td on td.TeamId = ut.TeamId
				WHERE
					u.Id = @UserId AND td.DealId = d.Id
			 ))
			)

SET @permissionDistributionTypeId = 0	-- разрешение права на просмотр пользователей

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0

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
			   ,mwb.[Number] 
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
			  ,''
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
		  aap.[Id] = mwbr.MovementWaybillSenderArticleAccountingPriceId
		 
		  JOIN #AvailableAccountOrganizationListTable ac1 ON 
		  ac1.Id = aO.Id 
		  
		  JOIN #AvailableRecipientAccountOrganizationListTable ac2 ON 
		  ac2.Id = clAO.Id 
		  
		  WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId]<> mwb.[RecipientId]
		  AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
		  AND mwb.MovementWaybillStateId in (9,11)
		  AND (
			 @permissionDistributionTypeId = 3 OR
			(@permissionDistributionTypeId = 1 AND mwb.CuratorId = @UserId) OR
			(@permissionDistributionTypeId = 2 AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamStorage] ts on ts.TeamId = ut.TeamId
					JOIN [dbo].[Storage] s on s.Id = ts.StorageId
				WHERE 
					u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
			 )))

SET @permissionDistributionTypeId = 0	-- разрешение права на просмотр пользователей

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(2001, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0
  
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
			   ,mwb.[Number] 
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
			  ,''
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
			 @permissionDistributionTypeId = 3 OR
			(@permissionDistributionTypeId = 1 AND mwb.ChangeOwnerWaybillCuratorId = @UserId AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamStorage] ts on ts.TeamId = ut.TeamId
					JOIN [dbo].[Storage] s on s.Id = ts.StorageId
				WHERE 
					ts.StorageId = mwb.ChangeOwnerWaybillStorageId)) OR
			(@permissionDistributionTypeId = 2 AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamStorage] ts on ts.TeamId = ut.TeamId
					JOIN [dbo].[Storage] s on s.Id = ts.StorageId
				WHERE 
					ts.StorageId  = mwb.ChangeOwnerWaybillStorageId
			 ))
			)
	
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
order by SaleWaybillDate,
		 SaleWaybillNumber
		 
drop table #Waybills

drop TABLE #AvailableAccountOrganizationListTable
drop TABLE #AvailableRecipientAccountOrganizationListTable

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
			   ,mwb.[Number] 
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
			 @permissionDistributionTypeId = 3 OR
			(@permissionDistributionTypeId = 1 AND mwb.CuratorId = @UserId) OR
			(@permissionDistributionTypeId = 2 AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamStorage] ts on ts.TeamId = ut.TeamId
					JOIN [dbo].[Storage] s on s.Id = ts.StorageId
				WHERE 
					u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
			 )))
			 


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
			   ,mwb.[Number] 
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
			 @permissionDistributionTypeId = 3 OR
			(@permissionDistributionTypeId = 1 AND mwb.ChangeOwnerWaybillCuratorId = @UserId AND exists
				(SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamStorage] ts on ts.TeamId = ut.TeamId
					JOIN [dbo].[Storage] s on s.Id = ts.StorageId
				WHERE 
					u.Id = @UserId AND ts.StorageId = mwb.ChangeOwnerWaybillStorageId)) OR
			(@permissionDistributionTypeId = 2 AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamStorage] ts on ts.TeamId = ut.TeamId
					JOIN [dbo].[Storage] s on s.Id = ts.StorageId
				WHERE 
					u.Id = @UserId AND ts.StorageId  = mwb.ChangeOwnerWaybillStorageId
			 )))
	
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


CREATE TABLE #AvailableAccountOrganizationListTable(
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

create table #Waybills(
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

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр пользователей

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1401, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0

		insert into #Waybills
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
			   ,mwb.[Number] 
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
		 
		  LEFT JOIN ArticleAccountingPrice aap ON
		  aap.[Id] = mwbr.MovementWaybillSenderArticleAccountingPriceId
		 
		  JOIN #AvailableAccountOrganizationListTable ac1 ON 
		  ac1.Id = aO.Id 
		  
		  WHERE mwb.[DeletionDate] IS NULL AND mwb.[SenderId]= mwb.[RecipientId]
		  AND mwb.[Date] >= @StartDate AND mwb.[Date] <= @EndDate
		  AND mwb.MovementWaybillStateId in (9,11)
		  AND (
			 @permissionDistributionTypeId = 3 OR
			(@permissionDistributionTypeId = 1 AND mwb.CuratorId = @UserId) OR
			(@permissionDistributionTypeId = 2 AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamStorage] ts on ts.TeamId = ut.TeamId
					JOIN [dbo].[Storage] s on s.Id = ts.StorageId
				WHERE 
					u.Id = @UserId AND ts.StorageId in (mwb.RecipientStorageId, mwb.SenderStorageId)
			 )))
  
  	
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
group by  SenderStorageId
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
order by MovementWaybillDate,
		 MovementWaybillNumber
		 
drop table #Waybills

drop TABLE #AvailableAccountOrganizationListTable

GO


CREATE PROCEDURE [dbo].[GetReturnFromClientWaybillsForExportTo1C]
(
	@StartDate datetime,
	@EndDate datetime,
	@UserId int,
	@IdRecipientAccountOrganizationList VARCHAR(8000),  
	@AllRecipientAccountOrganizations BIT	
)
AS

SET DATEFORMAT DMY
SET NOCOUNT ON


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

create table #Waybills(
	RecipientStorageId smallint ---1
	,RecipientStorageName varchar(200) ---2
	,ContractId smallint -->3---
	,ContractName varchar(200)
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
)

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр пользователей

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(3901, @UserId)

IF ISNULL(@permissionDistributionTypeId,0)<> 0

	insert into #Waybills
	(
		 RecipientStorageId
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
	)
	select
		   sw.ReturnFromClientWaybillRecipientStorageId	
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
		  ,con.Name + ' №' + con.Number + ' от ' + CONVERT(varchar(10), con.Date, 104)
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
	  FROM [ReturnFromClientWaybill] sw
	  JOIN [ReturnFromClientWaybillRow] swr ON sw.[Id] = swr.[ReturnFromClientWaybillId] AND swr.[DeletionDate] IS NULL
	  
	  JOIN [Article] art ON swr.[ArticleId] = art.[Id]
	  JOIN [ArticleGroup] artGr ON art.[ArticleGroupId] = artGr.[Id]
	  JOIN [MeasureUnit] mu ON art.[MeasureUnitId] = mu.[Id]
	  JOIN dbo.[Deal] d ON sw.[ReturnFromClientWaybillDealId] = d.Id
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
		AND (
			@permissionDistributionTypeId = 3 OR
			(@permissionDistributionTypeId = 2 AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamDeal] td on td.TeamId = ut.TeamId
				WHERE 
					u.Id = @UserId AND td.DealId = sw.ReturnFromClientWaybillDealId
				) 
			) OR 
			(@permissionDistributionTypeId = 1 AND sw.ReturnFromClientWaybillCuratorId = @UserId AND exists(
				SELECT *
				FROM [dbo].[User] u
					JOIN [dbo].[UserTeam] ut on ut.UserId = u.Id
					JOIN [dbo].[TeamDeal] td on td.TeamId = ut.TeamId
				WHERE 
					u.Id = @UserId AND td.DealId = sw.ReturnFromClientWaybillDealId
				) 
			)				
		)
	
UPDATE t 
SET t.RecipientStorageName = st.[Name]
FROM #Waybills t 
JOIN [Storage] st ON t.RecipientStorageId = st.[Id] 
  
select 
	 RecipientStorageId
	,RecipientStorageName
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
	,ExpenditureWaybillDate
	,IsOwner
from #Waybills
group by RecipientStorageId
	,RecipientStorageName
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
	,ReturnWaybillNumber
	,ReturnWaybillDate
	,ReturnWaybillSalePriceSum
	,ValueAddedTax
	,ArticleGroupName
	,MeasureUnitNumericCode
	,MeasureUnitShortName
	,ExpenditureWaybillNumber
	,ExpenditureWaybillDate
	,IsOwner
order by ReturnWaybillDate,
		 ReturnWaybillNumber
		 
drop table #Waybills

drop TABLE #AvailableRecipientAccountOrganizationListTable


GO




-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO
	