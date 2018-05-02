BEGIN TRY
    BEGIN TRAN

	ALTER TABLE [dbo].[ProductionOrderTransportSheet]
		ALTER COLUMN BillOfLadingNumber VARCHAR(100) not null

	ALTER TABLE [dbo].[ProductionOrderTransportSheet]
		ALTER COLUMN ShippingLine VARCHAR(100) not null

	ALTER TABLE [dbo].[ProductionOrderTransportSheet]
		ALTER COLUMN PortDocumentNumber VARCHAR(100) not null

    create table dbo.[ProductionOrderExtraExpensesSheet] (
        Id UNIQUEIDENTIFIER not null,
       ExtraExpensesContractorName VARCHAR(200) not null,
       Date DATETIME not null,
       ProductionOrderTransportSheetCurrencyDeterminationTypeId TINYINT not null,
       CostInCurrency DECIMAL(18, 2) not null,
       ExtraExpensesPurpose VARCHAR(200) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT null,
       CurrencyRateId INT null,
       primary key (Id)
    )

    alter table dbo.[ProductionOrderExtraExpensesSheet] 
        add constraint FK_ProductionOrder_ProductionOrderExtraExpensesSheet 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderExtraExpensesSheet] 
        add constraint FK_ProductionOrderExtraExpensesSheet_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[ProductionOrderExtraExpensesSheet] 
        add constraint FK_ProductionOrderExtraExpensesSheet_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]

	EXEC sp_rename 'ProductionOrderTransportSheet.ProductionOrderTransportSheetCurrencyDeterminationTypeId',
		'ProductionOrderCurrencyDeterminationTypeId';

	EXEC sp_rename 'ProductionOrderExtraExpensesSheet.ProductionOrderTransportSheetCurrencyDeterminationTypeId',
		'ProductionOrderCurrencyDeterminationTypeId';

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
