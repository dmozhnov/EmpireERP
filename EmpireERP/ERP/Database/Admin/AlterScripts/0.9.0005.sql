/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.5

  Что нового:
	+ новые тарифные планы
	
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

SET @PreviousVersion = '0.9.4' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.5'			-- номер новой версии

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


update Rate
set name = 'Бесплатный', 
	[ActiveUserCountLimit] = 3,
	[TeamCountLimit] = 1,
	[StorageCountLimit] = 1,
	[AccountOrganizationCountLimit] = 1,
	[GigabyteCountLimit] = 1,
	[UseExtraActiveUserOption] = 0,
	[UseExtraTeamOption] = 0,
	[UseExtraStorageOption] = 0,
	[UseExtraAccountOrganizationOption] = 0,
	[UseExtraGigabyteOption] = 0,
	[ExtraActiveUserOptionCostPerMonth] = 0,
	[ExtraTeamOptionCostPerMonth] = 0,
	[ExtraStorageOptionCostPerMonth] = 0,
	[ExtraAccountOrganizationOptionCostPerMonth] = 0,
	[ExtraGigabyteOptionCostPerMonth] = 0,
	[BaseCostPerMonth] = 0
where id = 1
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


update Rate
set name = 'Стандарт', 
	[ActiveUserCountLimit] = 3,
	[TeamCountLimit] = 1,
	[StorageCountLimit] = 1,
	[AccountOrganizationCountLimit] = 1,
	[GigabyteCountLimit] = 1,
	[UseExtraActiveUserOption] = 1,
	[UseExtraTeamOption] = 1,
	[UseExtraStorageOption] = 1,
	[UseExtraAccountOrganizationOption] = 1,
	[UseExtraGigabyteOption] = 1,
	[ExtraActiveUserOptionCostPerMonth] = 570,
	[ExtraTeamOptionCostPerMonth] = 1200,
	[ExtraStorageOptionCostPerMonth] = 315,
	[ExtraAccountOrganizationOptionCostPerMonth] = 315,
	[ExtraGigabyteOptionCostPerMonth] = 300,
	[BaseCostPerMonth] = 990
where id = 2
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


-- тарифные планы
SET IDENTITY_INSERT Rate ON
GO

INSERT INTO [Rate] ([Id], [Name],[ActiveUserCountLimit],[TeamCountLimit],[StorageCountLimit],[AccountOrganizationCountLimit],[GigabyteCountLimit]
           ,[UseExtraActiveUserOption],[UseExtraTeamOption],[UseExtraStorageOption],[UseExtraAccountOrganizationOption],[UseExtraGigabyteOption]
           ,[ExtraActiveUserOptionCostPerMonth],[ExtraTeamOptionCostPerMonth],[ExtraStorageOptionCostPerMonth],[ExtraAccountOrganizationOptionCostPerMonth]
           ,[ExtraGigabyteOptionCostPerMonth],[BaseCostPerMonth])

SELECT 
    [Id] = 3,
    [Name] = 'Бизнес', 
	[ActiveUserCountLimit] = 10,
	[TeamCountLimit] = 3,
	[StorageCountLimit] = 32767,
	[AccountOrganizationCountLimit] = 32767,
	[GigabyteCountLimit] = 3,
	[UseExtraActiveUserOption] = 1,
	[UseExtraTeamOption] = 1,
	[UseExtraStorageOption] = 0,
	[UseExtraAccountOrganizationOption] = 0,
	[UseExtraGigabyteOption] = 1,
	[ExtraActiveUserOptionCostPerMonth] = 570,
	[ExtraTeamOptionCostPerMonth] = 1200,
	[ExtraStorageOptionCostPerMonth] = 0,
	[ExtraAccountOrganizationOptionCostPerMonth] = 0,
	[ExtraGigabyteOptionCostPerMonth] = 300,
	[BaseCostPerMonth] = 10990

UNION
SELECT [Id] = 4,
    [Name] = 'Корпорация', 
	[ActiveUserCountLimit] = 250,
	[TeamCountLimit] = 32767,
	[StorageCountLimit] = 32767,
	[AccountOrganizationCountLimit] = 32767,
	[GigabyteCountLimit] = 50,
	[UseExtraActiveUserOption] = 1,
	[UseExtraTeamOption] = 0,
	[UseExtraStorageOption] = 0,
	[UseExtraAccountOrganizationOption] = 0,
	[UseExtraGigabyteOption] = 1,
	[ExtraActiveUserOptionCostPerMonth] = 570,
	[ExtraTeamOptionCostPerMonth] = 0,
	[ExtraStorageOptionCostPerMonth] = 0,
	[ExtraAccountOrganizationOptionCostPerMonth] = 0,
	[ExtraGigabyteOptionCostPerMonth] = 300,
	[BaseCostPerMonth] = 34990
GO

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

SET IDENTITY_INSERT Rate OFF
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

