BEGIN TRY
	BEGIN TRAN		

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderPlannedPayment_ProductionOrderPayment_ProductionOrderPlannedPaymentId]') AND parent_object_id = OBJECT_ID('[ProductionOrderPayment]'))
alter table dbo.[ProductionOrderPayment]  drop constraint FK_ProductionOrderPlannedPayment_ProductionOrderPayment_ProductionOrderPlannedPaymentId
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderPlannedPayment_ProductionOrderId]') AND parent_object_id = OBJECT_ID('[ProductionOrderPlannedPayment]'))
alter table dbo.[ProductionOrderPlannedPayment]  drop constraint FK_ProductionOrder_ProductionOrderPlannedPayment_ProductionOrderId
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderPlannedPayment_Currency]') AND parent_object_id = OBJECT_ID('[ProductionOrderPlannedPayment]'))
alter table dbo.[ProductionOrderPlannedPayment]  drop constraint FK_ProductionOrderPlannedPayment_Currency
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderPlannedPayment_CurrencyRate]') AND parent_object_id = OBJECT_ID('[ProductionOrderPlannedPayment]'))
alter table dbo.[ProductionOrderPlannedPayment]  drop constraint FK_ProductionOrderPlannedPayment_CurrencyRate

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderPlannedPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
    drop table dbo.[ProductionOrderPlannedPayment]

	IF EXISTS
	(
		SELECT [name]
		FROM syscolumns
		WHERE id = object_id(N'[dbo].[ProductionOrderPayment]') 
		AND OBJECTPROPERTY(id, N'IsUserTable') = 1
		AND [name] = N'ProductionOrderPlannedPaymentId'
	)
	BEGIN
		ALTER TABLE dbo.[ProductionOrderPayment] DROP COLUMN ProductionOrderPlannedPaymentId;
		print 'Столбец ProductionOrderPlannedPaymentId найден.'
	END

	alter table dbo.[ProductionOrderPayment]
	add ProductionOrderPlannedPaymentId UNIQUEIDENTIFIER not null
	
	-- Отсюда раскомментировать тем, кто будет накатывать на свою базу с существующими оплатами по заказам
	
	--constraint DF_ProductionOrderPlannedPaymentId default ('00000000-0000-0000-0000-000000000000');
	
	--alter table dbo.[ProductionOrderPayment]
	--drop constraint DF_ProductionOrderPlannedPaymentId;

	--ALTER TABLE dbo.[ProductionOrderPayment]
	--ALTER COLUMN ProductionOrderPlannedPaymentId UNIQUEIDENTIFIER null

	-- Досюда раскомментировать тем, кто будет накатывать на свою базу

    create table dbo.[ProductionOrderPlannedPayment] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME not null,
       SumInCurrency DECIMAL(18, 2) not null,
       Purpose VARCHAR(50) not null,
       ProductionOrderPaymentTypeId TINYINT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT not null,
       CurrencyRateId INT null,
       primary key (Id)
    )

	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20011, 1)
	
	--ROLLBACK TRAN -- Очень помогает при отладке, а COMMIT TRAN при отладке комментируем
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

GO

BEGIN TRY
	BEGIN TRAN

	-- Отсюда раскомментировать тем, кто будет накатывать на свою базу с существующими оплатами по заказам

	--DECLARE myCursor CURSOR
	--	LOCAL FAST_FORWARD READ_ONLY
	--	FOR SELECT ProductionOrderPaymentTypeId, CurrencyRateId, ProductionOrderId, SumInCurrency, Id
	--	FROM dbo.[ProductionOrderPayment];
	--DECLARE @_ProductionOrderPaymentTypeId tinyint, @currencyRateId int, @currencyId smallint,
	--	@_ProductionOrderId UNIQUEIDENTIFIER, @sum decimal(18, 2),
	--	@productionOrderPlannedPaymentId UNIQUEIDENTIFIER, @productionOrderPaymentId UNIQUEIDENTIFIER
	--OPEN myCursor
	--FETCH myCursor
	--	INTO @_ProductionOrderPaymentTypeId, @currencyRateId, @_ProductionOrderId, @sum, @productionOrderPaymentId
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
	--	set @productionOrderPlannedPaymentId = NEWID();
	--	SET @currencyId = (SELECT [CurrencyId] FROM dbo.[CurrencyRate] WHERE Id = @currencyRateId);
	--	insert into [ProductionOrderPlannedPayment] (Id, CreationDate, StartDate, EndDate,
	--		CurrencyId, CurrencyRateId, ProductionOrderId, ProductionOrderPaymentTypeId,
	--		Purpose, SumInCurrency, Comment)
	--	select @productionOrderPlannedPaymentId, GETDATE(), GETDATE(), GETDATE(), @currencyId, @currencyRateId, 
	--		@_ProductionOrderId, @_ProductionOrderPaymentTypeId, 'Назначение', @sum, 'Комментарий';

	--	UPDATE P
	--	SET P.ProductionOrderPlannedPaymentId = @productionOrderPlannedPaymentId
	--	FROM dbo.[ProductionOrderPayment] P
	--	WHERE Id = @productionOrderPaymentId;

	--	FETCH NEXT FROM myCursor
	--		INTO @_ProductionOrderPaymentTypeId, @currencyRateId, @_ProductionOrderId, @sum, @productionOrderPaymentId
	--END
	--CLOSE myCursor
	--DEALLOCATE myCursor

	--ALTER TABLE dbo.[ProductionOrderPayment]
	--ALTER COLUMN ProductionOrderPlannedPaymentId UNIQUEIDENTIFIER not null

	-- Досюда раскомментировать тем, кто будет накатывать на свою базу

	--ROLLBACK TRAN -- Очень помогает при отладке, а COMMIT TRAN при отладке комментируем
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

GO

BEGIN TRY
	BEGIN TRAN

    alter table dbo.[ProductionOrderPayment] 
        add constraint FK_ProductionOrderPlannedPayment_ProductionOrderPayment_ProductionOrderPlannedPaymentId 
        foreign key (ProductionOrderPlannedPaymentId) 
        references dbo.[ProductionOrderPlannedPayment];
    alter table dbo.[ProductionOrderPlannedPayment] 
        add constraint FK_ProductionOrder_ProductionOrderPlannedPayment_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder];
    alter table dbo.[ProductionOrderPlannedPayment] 
        add constraint FK_ProductionOrderPlannedPayment_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency];
    alter table dbo.[ProductionOrderPlannedPayment] 
        add constraint FK_ProductionOrderPlannedPayment_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate];

	PRINT 'Обновление выполнено успешно'

	--ROLLBACK TRAN -- Очень помогает при отладке, а COMMIT TRAN при отладке комментируем
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
