/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.40

  Что нового:
	+ Добавлены настройки использования статуса "Готово к проводке" для накладных внутреннего перемещения,
	смены собственника, списания, реализации, возврата товара.
	
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

SET @PreviousVersion = '0.9.39' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.40'			-- номер новой версии

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

alter table Setting add UseReadyToAcceptStateForMovementWaybill BIT not null
constraint DF_UseReadyToAcceptStateForMovementWaybill default 0;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting add UseReadyToAcceptStateForChangeOwnerWaybill BIT not null
constraint DF_UseReadyToAcceptStateForChangeOwnerWaybill default 0;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting add UseReadyToAcceptStateForExpenditureWaybill BIT not null
constraint DF_UseReadyToAcceptStateForExpenditureWaybill default 0;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting add UseReadyToAcceptStateForReturnFromClientWaybill BIT not null
constraint DF_UseReadyToAcceptStateForReturnFromClientWaybill default 0;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting add UseReadyToAcceptStateForWriteOffWaybill BIT not null
constraint DF_UseReadyToAcceptStateForWriteOffWaybill default 0;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting drop constraint DF_UseReadyToAcceptStateForMovementWaybill;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting drop constraint DF_UseReadyToAcceptStateForChangeOwnerWaybill;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting drop constraint DF_UseReadyToAcceptStateForExpenditureWaybill;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting drop constraint DF_UseReadyToAcceptStateForReturnFromClientWaybill;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
alter table Setting drop constraint DF_UseReadyToAcceptStateForWriteOffWaybill;
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

