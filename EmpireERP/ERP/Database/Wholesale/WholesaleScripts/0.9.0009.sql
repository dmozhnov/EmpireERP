/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.9

  Что нового:
	* Изменены права на этапы заказа, план оплат заказа, финансовый план заказа
	Дерево прав выглядит так:
		Просмотр этапов заказа (зависит от Заказ(Список и детали))
		Ввод и редактирование этапов заказа (зависит от Просмотр этапов заказа)
		Перевод на следующий этап заказа (зависит от Просмотр этапов заказа)
		Перевод на предыдущий этап заказа (зависит от Просмотр этапов заказа)
		Перевод на этап заказа «Неуспешное закрытие» (зависит от Просмотр этапов заказа)

		Просмотр плана оплат (зависит от Заказ(Список и детали))
		Добавить платеж в план (зависит от Просмотр плана оплат)
		Редактирование платежа в плане (зависит от Просмотр плана оплат)
		Удаление платежа в плане (зависит от Просмотр плана оплат)

		Просмотр фин. плана (зависит от Заказ(Список и детали))
		Ввод и редактирование фин. плана (зависит от Просмотр фин. плана)
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

SET @PreviousVersion = '0.9.8' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.9'			-- номер новой версии

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

	-- Вставляем нулевые значения для создаваемых (новых) прав
	DECLARE roleCursor CURSOR
		LOCAL FAST_FORWARD READ_ONLY
		FOR SELECT Id
		FROM [Role];
	DECLARE @roleId smallint
	OPEN roleCursor
	FETCH roleCursor
		INTO @roleId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 20901, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21002, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21003, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21004, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21005, @roleId;

		FETCH NEXT FROM roleCursor
			INTO @roleId
	END
	CLOSE roleCursor
	DEALLOCATE roleCursor

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 2 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Ставим администратору (роль 1) распространение "Все" на все новые права
UPDATE P
	SET P.[PermissionDistributionTypeId] = 3
	FROM dbo.[PermissionDistribution] P
	WHERE [RoleId] = 1 AND (
		[PermissionId] = 20901 OR
		[PermissionId] = 21002 OR
		[PermissionId] = 21003 OR
		[PermissionId] = 21004 OR
		[PermissionId] = 21005);
-- Можно было бы без проверки на [PermissionId], но побоялись

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 7 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Просто перенумеруем права
UPDATE P
	SET P.[PermissionId] = 20902
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20003;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 8 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Просто перенумеруем права
UPDATE P
	SET P.[PermissionId] = 20903
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20008;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 9 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Просто перенумеруем права
UPDATE P
	SET P.[PermissionId] = 20904
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20009;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 10 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Просто перенумеруем права
UPDATE P
	SET P.[PermissionId] = 20905
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20010;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 11 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Просто перенумеруем права
UPDATE P
	SET P.[PermissionId] = 21001
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20011;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 12 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Просто перенумеруем права
UPDATE P
	SET P.[PermissionId] = 21006
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20004;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 13 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Проходим в цикле все роли,
-- и если 21001 право (просмотр плана оплат) имеет распространение 0 или нет ее в базе,
-- а выставлено право 20601 (просмотр списка и деталей оплат по заказам), переписываем ее значение
-- в право 21001.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
DECLARE @permissionDistributionTypeIdProductionOrder smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Проверяем, есть ли запись для данной роли и права "просм. плана оплат" (если нет, то -1)
    set @permissionDistributionTypeId = -1;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 21001;

    -- Проверяем, есть ли запись для данной роли и права "обычные оплаты: список" (если нет, то 0)
    set @permissionDistributionTypeIdPayments = 0;
	SELECT TOP 1 @permissionDistributionTypeIdPayments = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 20601;

    -- Проверяем, есть ли запись для данной роли и права "просм. деталей заказа" (если нет, то 0)
    set @permissionDistributionTypeIdProductionOrder = 0;
	SELECT TOP 1 @permissionDistributionTypeIdProductionOrder = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 20001;

	IF (@permissionDistributionTypeId <= 0 AND @permissionDistributionTypeIdPayments > 0)
	BEGIN
	    DECLARE @newDistributionTypeId smallint;
	    IF (@permissionDistributionTypeIdPayments < @permissionDistributionTypeIdProductionOrder)
			SET @newDistributionTypeId = @permissionDistributionTypeIdPayments
		ELSE
			SET @newDistributionTypeId = @permissionDistributionTypeIdProductionOrder;
		
		IF (@permissionDistributionTypeId = -1)
		BEGIN
			--print 'Осуществляем вставку.';
			insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
				select @newDistributionTypeId, 21001, @roleId;
		END
		ELSE
		BEGIN
			--print 'Осуществляем апдейт.';
			UPDATE P
				SET P.PermissionDistributionTypeId = @newDistributionTypeId
				FROM [PermissionDistribution] P
				WHERE [PermissionId] = 21001 AND [RoleId] = @roleId;
		END
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 14 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Проходим в цикле все роли,
-- и если 20001 право (просмотр заказов) имеет распространение больше чем "Ничего",
-- переписываем ее значение в право 20901 "просмотр этапов заказа" (оно уже создано), для админа 3 заменит 3.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Проверяем, есть ли запись для данной роли и права "просмотр заказов" (если нет, то 0)
    set @permissionDistributionTypeId = 0;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 20001;

	IF (@permissionDistributionTypeId > 0)
	BEGIN
		--print 'Осуществляем апдейт.';
		UPDATE P
			SET P.PermissionDistributionTypeId = @permissionDistributionTypeId
			FROM [PermissionDistribution] P
			WHERE [PermissionId] = 20901 AND [RoleId] = @roleId;
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 15 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Проходим в цикле все роли,
-- и если 21006 право (ввод и ред. фин. плана) имеет распространение больше чем "Ничего",
-- переписываем ее значение в право 21005 "просмотр фин. плана" (оно уже создано), для админа 3 заменит 3.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Проверяем, есть ли запись для данной роли и права "ввод и ред. фин. плана" (если нет, то 0)
    set @permissionDistributionTypeId = 0;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 21006;

	IF (@permissionDistributionTypeId > 0)
	BEGIN
		--print 'Осуществляем апдейт.';
		UPDATE P
			SET P.PermissionDistributionTypeId = @permissionDistributionTypeId
			FROM [PermissionDistribution] P
			WHERE [PermissionId] = 21005 AND [RoleId] = @roleId;
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 16 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Проходим в цикле все роли,
-- и если 21001 право (просм. плана оплат) имеет распространение больше чем "Ничего",
-- переписываем ее значение в права 21002 21003 "созд-ред план. оплат" (они уже созданы), для админа 3 заменит 3.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Проверяем, есть ли запись для данной роли и права "ввод и ред. фин. плана" (если нет, то 0)
    set @permissionDistributionTypeId = 0;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 21001;

	IF (@permissionDistributionTypeId > 0)
	BEGIN

		UPDATE P
			SET P.PermissionDistributionTypeId = @permissionDistributionTypeId
			FROM [PermissionDistribution] P
			WHERE ([PermissionId] = 21002 OR [PermissionId] = 21003) AND [RoleId] = @roleId;
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг 17 окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

