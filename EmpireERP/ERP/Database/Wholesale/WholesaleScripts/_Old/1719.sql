BEGIN TRY
	BEGIN TRAN		

-- Виталий разрешил убить таблицы грузовых таможенных деклараций, листов дополнительной оплаты, транспортных
-- листов, оплат по заказу, оплат по грузовым таможенным декларациям, оплат по листам дополнительной оплаты,
-- оплат по транспортным листам, так как в них нет ничего ценного.

-- 00. Связь между партиями заказа и ГТД : удаление
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderBatch_ProductionOrderCustomsDeclaration]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_ProductionOrderCustomsDeclaration


-- 01. Связь между этапами шаблона заказа и шаблонами заказа
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatchLifeCycleTemplateStage]'))
alter table dbo.[ProductionOrderBatchLifeCycleTemplateStage]  drop constraint FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage_TemplateId]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatchLifeCycleTemplateStage]'))
alter table dbo.[ProductionOrderBatchLifeCycleTemplateStage]  drop constraint FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage_TemplateId
    alter table dbo.[ProductionOrderBatchLifeCycleTemplateStage] 
        add constraint FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage_TemplateId 
        foreign key (TemplateId) 
        references dbo.[ProductionOrderBatchLifeCycleTemplate]

-- 02. Связь между заказом и партиями заказа
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderBatch]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrder_ProductionOrderBatch
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderBatch_ProductionOrderId]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrder_ProductionOrderBatch_ProductionOrderId
    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrder_ProductionOrderBatch_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

-- 03. Связь между партией заказа и позицией партии заказа
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderBatch_ProductionOrderBatchRow]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatch_ProductionOrderBatchRow
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderBatch_ProductionOrderBatchRow_BatchId]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatch_ProductionOrderBatchRow_BatchId
    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatch_ProductionOrderBatchRow_BatchId 
        foreign key (BatchId) 
        references dbo.[ProductionOrderBatch]


-- 04, 05, 06, 07. Связи между партией заказа и ее этапами; ГТД и заказом; ЛДР и заказом; ТЛ и заказом
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderBatch_ProductionOrderBatchStage]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatchStage]'))
alter table dbo.[ProductionOrderBatchStage]  drop constraint FK_ProductionOrderBatch_ProductionOrderBatchStage
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderBatch_ProductionOrderBatchStage_BatchId]') AND parent_object_id = OBJECT_ID('[ProductionOrderBatchStage]'))
alter table dbo.[ProductionOrderBatchStage]  drop constraint FK_ProductionOrderBatch_ProductionOrderBatchStage_BatchId
    alter table dbo.[ProductionOrderBatchStage] 
        add constraint FK_ProductionOrderBatch_ProductionOrderBatchStage_BatchId 
        foreign key (BatchId) 
        references dbo.[ProductionOrderBatch]

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderCustomsDeclaration]') AND parent_object_id = OBJECT_ID('[ProductionOrderCustomsDeclaration]'))
alter table dbo.[ProductionOrderCustomsDeclaration]  drop constraint FK_ProductionOrder_ProductionOrderCustomsDeclaration
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderCustomsDeclaration_ProductionOrderId]') AND parent_object_id = OBJECT_ID('[ProductionOrderCustomsDeclaration]'))
alter table dbo.[ProductionOrderCustomsDeclaration]  drop constraint FK_ProductionOrder_ProductionOrderCustomsDeclaration_ProductionOrderId
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderCustomsDeclaration]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
    alter table dbo.[ProductionOrderCustomsDeclaration] 
        add constraint FK_ProductionOrder_ProductionOrderCustomsDeclaration_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderExtraExpensesSheet]') AND parent_object_id = OBJECT_ID('[ProductionOrderExtraExpensesSheet]'))
alter table dbo.[ProductionOrderExtraExpensesSheet]  drop constraint FK_ProductionOrder_ProductionOrderExtraExpensesSheet
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderExtraExpensesSheet_ProductionOrderId]') AND parent_object_id = OBJECT_ID('[ProductionOrderExtraExpensesSheet]'))
alter table dbo.[ProductionOrderExtraExpensesSheet]  drop constraint FK_ProductionOrder_ProductionOrderExtraExpensesSheet_ProductionOrderId
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderExtraExpensesSheet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
    alter table dbo.[ProductionOrderExtraExpensesSheet] 
        add constraint FK_ProductionOrder_ProductionOrderExtraExpensesSheet_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderTransportSheet]') AND parent_object_id = OBJECT_ID('[ProductionOrderTransportSheet]'))
alter table dbo.[ProductionOrderTransportSheet]  drop constraint FK_ProductionOrder_ProductionOrderTransportSheet
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderTransportSheet_ProductionOrderId]') AND parent_object_id = OBJECT_ID('[ProductionOrderTransportSheet]'))
alter table dbo.[ProductionOrderTransportSheet]  drop constraint FK_ProductionOrder_ProductionOrderTransportSheet_ProductionOrderId
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderTransportSheet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
    alter table dbo.[ProductionOrderTransportSheet] 
        add constraint FK_ProductionOrder_ProductionOrderTransportSheet_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

-- 08. Связь между заказом и оплатой (базовой) : удаление
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderPayment]') AND parent_object_id = OBJECT_ID('[ProductionOrderPayment]'))
alter table dbo.[ProductionOrderPayment]  drop constraint FK_ProductionOrder_ProductionOrderPayment
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrder_ProductionOrderPayment_ProductionOrderId]') AND parent_object_id = OBJECT_ID('[ProductionOrderPayment]'))
alter table dbo.[ProductionOrderPayment]  drop constraint FK_ProductionOrder_ProductionOrderPayment_ProductionOrderId


-- 09. Связи между базовой оплатой и курсом валюты,
-- PK оплаты по ГТД, ГТД и оплатой по ГТД,
-- PK оплаты по ЛДР, ЛДР и оплатой по ЛДР,
-- PK оплаты по ТЛ, ТД и оплатой по ТЛ:
-- удаление на всякий случай
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderPayment_CurrencyRate]') AND parent_object_id = OBJECT_ID('[ProductionOrderPayment]'))
alter table dbo.[ProductionOrderPayment]  drop constraint FK_ProductionOrderPayment_CurrencyRate
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[PFK_ProductionOrderCustomsDeclarationPayment]') AND parent_object_id = OBJECT_ID('[ProductionOrderCustomsDeclarationPayment]'))
alter table dbo.[ProductionOrderCustomsDeclarationPayment]  drop constraint PFK_ProductionOrderCustomsDeclarationPayment
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationPayment_CustomsDeclarationId]') AND parent_object_id = OBJECT_ID('[ProductionOrderCustomsDeclarationPayment]'))
alter table dbo.[ProductionOrderCustomsDeclarationPayment]  drop constraint FK_ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationPayment_CustomsDeclarationId
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[PFK_ProductionOrderExtraExpensesSheetPayment]') AND parent_object_id = OBJECT_ID('[ProductionOrderExtraExpensesSheetPayment]'))
alter table dbo.[ProductionOrderExtraExpensesSheetPayment]  drop constraint PFK_ProductionOrderExtraExpensesSheetPayment
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetPayment_ExtraExpensesSheetId]') AND parent_object_id = OBJECT_ID('[ProductionOrderExtraExpensesSheetPayment]'))
alter table dbo.[ProductionOrderExtraExpensesSheetPayment]  drop constraint FK_ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetPayment_ExtraExpensesSheetId
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[PFK_ProductionOrderTransportSheetPayment]') AND parent_object_id = OBJECT_ID('[ProductionOrderTransportSheetPayment]'))
alter table dbo.[ProductionOrderTransportSheetPayment]  drop constraint PFK_ProductionOrderTransportSheetPayment
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ProductionOrderTransportSheet_ProductionOrderTransportSheetPayment_TransportSheetId]') AND parent_object_id = OBJECT_ID('[ProductionOrderTransportSheetPayment]'))
alter table dbo.[ProductionOrderTransportSheetPayment]  drop constraint FK_ProductionOrderTransportSheet_ProductionOrderTransportSheetPayment_TransportSheetId

-- 10. Удаление таблиц оплат по ГТД, по ЛДР, по ТЛ
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderCustomsDeclarationPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderCustomsDeclarationPayment]
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderExtraExpensesSheetPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderExtraExpensesSheetPayment]
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderTransportSheetPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderTransportSheetPayment]
-- 11. Удаление таблицы базовой оплаты
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderPayment]
-- 12. Удаление таблиц ГТД, ЛДР, ТЛ
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderCustomsDeclaration]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderCustomsDeclaration]
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderExtraExpensesSheet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderExtraExpensesSheet]
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderTransportSheet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderTransportSheet]

	PRINT 'Удаление выполнено успешно'

-- 12. Создание таблиц ГТД, ЛДР, ТЛ
    create table dbo.[ProductionOrderCustomsDeclaration] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(50) not null,
       Date DATETIME not null,
       ImportCustomsDutiesSum DECIMAL(18, 2) not null,
       ExportCustomsDutiesSum DECIMAL(18, 2) not null,
       ValueAddedTaxSum DECIMAL(18, 2) not null,
       ExciseSum DECIMAL(18, 2) not null,
       CustomsFeesSum DECIMAL(18, 2) not null,
       CustomsValueCorrection DECIMAL(18, 2) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
    create table dbo.[ProductionOrderExtraExpensesSheet] (
        Id UNIQUEIDENTIFIER not null,
       ExtraExpensesContractorName VARCHAR(200) not null,
       Date DATETIME not null,
       ProductionOrderCurrencyDeterminationTypeId TINYINT not null,
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
    create table dbo.[ProductionOrderTransportSheet] (
        Id UNIQUEIDENTIFIER not null,
       ForwarderName VARCHAR(200) not null,
       RequestDate DATETIME not null,
       ShippingDate DATETIME null,
       PendingDeliveryDate DATETIME null,
       ActualDeliveryDate DATETIME null,
       ProductionOrderCurrencyDeterminationTypeId TINYINT not null,
       CostInCurrency DECIMAL(18, 2) not null,
       BillOfLadingNumber VARCHAR(100) not null,
       ShippingLine VARCHAR(100) not null,
       PortDocumentNumber VARCHAR(100) not null,
       PortDocumentDate DATETIME null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT null,
       CurrencyRateId INT null,
       primary key (Id)
    )

-- 11. Создание таблицы базовой оплаты	
    create table dbo.[ProductionOrderPayment] (
        Id UNIQUEIDENTIFIER not null,
       ProductionOrderPaymentTypeId TINYINT not null,
       PaymentDocumentNumber VARCHAR(255) not null,
       Date DATETIME not null,
       SumInCurrency DECIMAL(18, 2) not null,
       ProductionOrderPaymentFormId TINYINT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyRateId INT null,
       primary key (Id)
    )
-- 10. Создание таблиц оплат по ГТД, по ЛДР, по ТЛ
    create table dbo.[ProductionOrderCustomsDeclarationPayment] (
        Id UNIQUEIDENTIFIER not null,
       DeletionDate DATETIME null,
       CustomsDeclarationId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
    create table dbo.[ProductionOrderExtraExpensesSheetPayment] (
        Id UNIQUEIDENTIFIER not null,
       DeletionDate DATETIME null,
       ExtraExpensesSheetId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
    create table dbo.[ProductionOrderTransportSheetPayment] (
        Id UNIQUEIDENTIFIER not null,
       DeletionDate DATETIME null,
       TransportSheetId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

-- 00. Связь между партиями заказа и ГТД : создание
    --alter table dbo.[ProductionOrderBatch] 
    --    add constraint FK_ProductionOrderBatch_ProductionOrderCustomsDeclaration 
    --    foreign key (ProductionOrderCustomsDeclarationId) 
    --    references dbo.[ProductionOrderCustomsDeclaration]
-- НЕ работал, wtf
--Msg 547, Level 16, State 0, Line 212
--The ALTER TABLE statement conflicted with the FOREIGN KEY constraint "FK_ProductionOrderBatch_ProductionOrderCustomsDeclaration". The conflict occurred in database "wholesale_000001", table "dbo.ProductionOrderCustomsDeclaration", column 'Id'.

-- 08. Связь между заказом и оплатой (базовой) : создание
    alter table dbo.[ProductionOrderPayment] 
        add constraint FK_ProductionOrder_ProductionOrderPayment_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]
	
-- 09. Связи между базовой оплатой и курсом валюты,
-- PK оплаты по ГТД, ГТД и оплатой по ГТД,
-- PK оплаты по ЛДР, ЛДР и оплатой по ЛДР,
-- PK оплаты по ТЛ, ТД и оплатой по ТЛ:
-- создание
    alter table dbo.[ProductionOrderPayment] 
        add constraint FK_ProductionOrderPayment_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]
    alter table dbo.[ProductionOrderCustomsDeclarationPayment] 
        add constraint PFK_ProductionOrderCustomsDeclarationPayment 
        foreign key (Id) 
        references dbo.[ProductionOrderPayment]
    alter table dbo.[ProductionOrderCustomsDeclarationPayment] 
        add constraint FK_ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationPayment_CustomsDeclarationId 
        foreign key (CustomsDeclarationId) 
        references dbo.[ProductionOrderCustomsDeclaration]
    alter table dbo.[ProductionOrderExtraExpensesSheetPayment] 
        add constraint PFK_ProductionOrderExtraExpensesSheetPayment 
        foreign key (Id) 
        references dbo.[ProductionOrderPayment]
    alter table dbo.[ProductionOrderExtraExpensesSheetPayment] 
        add constraint FK_ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetPayment_ExtraExpensesSheetId 
        foreign key (ExtraExpensesSheetId) 
        references dbo.[ProductionOrderExtraExpensesSheet]
    alter table dbo.[ProductionOrderTransportSheetPayment] 
        add constraint PFK_ProductionOrderTransportSheetPayment 
        foreign key (Id) 
        references dbo.[ProductionOrderPayment]
    alter table dbo.[ProductionOrderTransportSheetPayment] 
        add constraint FK_ProductionOrderTransportSheet_ProductionOrderTransportSheetPayment_TransportSheetId 
        foreign key (TransportSheetId) 
        references dbo.[ProductionOrderTransportSheet]


	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
