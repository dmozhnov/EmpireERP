/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.3

  Что нового:
	+ дополнительные поля в Rate и ServiceSet
	
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

SET @PreviousVersion = '0.9.2' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.3'			-- номер новой версии

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

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = 
		N'Обновление версии', NOFORMAT

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

alter table dbo.[ServiceSet] drop constraint FK_ServiceSetConfiguration_Rate 
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete Rate
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add ActiveUserCountLimit SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add TeamCountLimit SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add StorageCountLimit SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add AccountOrganizationCountLimit SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add GigabyteCountLimit SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add UseExtraActiveUserOption BIT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add UseExtraTeamOption BIT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add UseExtraStorageOption BIT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add UseExtraAccountOrganizationOption BIT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add UseExtraGigabyteOption BIT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add ExtraActiveUserOptionCostPerMonth DECIMAL(18, 2) not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add ExtraTeamOptionCostPerMonth DECIMAL(18, 2) not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add ExtraStorageOptionCostPerMonth DECIMAL(18, 2) not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add ExtraAccountOrganizationOptionCostPerMonth DECIMAL(18, 2) not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add ExtraGigabyteOptionCostPerMonth DECIMAL(18, 2) not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table [Rate] add BaseCostPerMonth DECIMAL(18, 2) not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

EXEC sp_rename
    @objname = 'ServiceSet.UserCountLimit',
    @newname = 'ExtraActiveUserCount',
    @objtype = 'COLUMN'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] add ExtraTeamCount SMALLINT null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] add ExtraStorageCount SMALLINT null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] add ExtraAccountOrganizationCount SMALLINT null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] add ExtraGigabyteCount SMALLINT null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update ServiceSet 
set ExtraTeamCount = 0, ExtraStorageCount = 0, ExtraAccountOrganizationCount = 0, ExtraGigabyteCount = 0
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] alter column ExtraTeamCount SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] alter column ExtraStorageCount SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] alter column ExtraAccountOrganizationCount SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table dbo.[ServiceSet] alter column ExtraGigabyteCount SMALLINT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

SET IDENTITY_INSERT Rate ON
GO

INSERT INTO [Rate] (id, [Name],[ActiveUserCountLimit],[TeamCountLimit],[StorageCountLimit],[AccountOrganizationCountLimit],[GigabyteCountLimit]
           ,[UseExtraActiveUserOption],[UseExtraTeamOption],[UseExtraStorageOption],[UseExtraAccountOrganizationOption],[UseExtraGigabyteOption]
           ,[ExtraActiveUserOptionCostPerMonth],[ExtraTeamOptionCostPerMonth],[ExtraStorageOptionCostPerMonth],[ExtraAccountOrganizationOptionCostPerMonth]
           ,[ExtraGigabyteOptionCostPerMonth],[BaseCostPerMonth])
SELECT 1, 'Бесплатный', 5, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 UNION
SELECT 2, 'Бизнес', 10, 3, 5, 5, 10, 1, 1, 1, 1, 1, 310, 470, 160, 160, 300, 6700 

SET IDENTITY_INSERT Rate OFF
GO

alter table dbo.[ServiceSet] 
	add constraint FK_ServiceSetConfiguration_Rate 
    foreign key (RateId) 
    references dbo.[Rate]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

