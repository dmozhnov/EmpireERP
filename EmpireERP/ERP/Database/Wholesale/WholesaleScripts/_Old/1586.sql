BEGIN TRY
    BEGIN TRAN

    create table dbo.[ProductionOrderTransportSheet] (
        Id UNIQUEIDENTIFIER not null,
       ForwarderName VARCHAR(200) not null,
       RequestDate DATETIME not null,
       ShippingDate DATETIME null,
       PendingDeliveryDate DATETIME null,
       ActualDeliveryDate DATETIME null,
       ProductionOrderTransportSheetCurrencyDeterminationTypeId TINYINT not null,
       CostInCurrency DECIMAL(18, 2) not null,
       BillOfLadingNumber VARCHAR(100) null,
       ShippingLine VARCHAR(100) null,
       PortDocumentNumber VARCHAR(100) null,
       PortDocumentDate DATETIME null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT null,
       CurrencyRateId INT null,
       primary key (Id)
    )

    alter table dbo.[ProductionOrderTransportSheet] 
        add constraint FK_ProductionOrder_ProductionOrderTransportSheet 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderTransportSheet] 
        add constraint FK_ProductionOrderTransportSheet_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[ProductionOrderTransportSheet] 
        add constraint FK_ProductionOrderTransportSheet_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]


    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
