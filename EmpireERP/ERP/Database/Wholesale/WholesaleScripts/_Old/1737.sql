BEGIN TRY
	BEGIN TRAN		
	
	create table dbo.[PaymentDistributionToSale] (
        Id UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
       
    create table dbo.[PaymentDistributionToPayment] (
        Id UNIQUEIDENTIFIER not null,
       PaidPaymentId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
    
    create table dbo.[PaymentDistributionToSalesReturn] (
        Id UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       ReturnWaybillId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
    
    alter table dbo.[PaymentDistributionToSale] 
        add constraint PFK_PaymentDistributionToSale 
        foreign key (Id) 
        references dbo.[PaymentDistribution]

    alter table dbo.[PaymentDistributionToSale] 
        add constraint FK_PaymentDistributionToSale_SaleWaybill 
        foreign key (SaleWaybillId) 
        references dbo.[SaleWaybill]
        
    alter table dbo.[PaymentDistributionToPayment] 
        add constraint PFK_PaymentDistributionToPayment 
        foreign key (Id) 
        references dbo.[PaymentDistribution]

    alter table dbo.[PaymentDistributionToPayment] 
        add constraint FK_PaymentDistributionToPayment_PaidPayment 
        foreign key (PaidPaymentId) 
        references dbo.[Payment] 
         
    alter table dbo.[PaymentDistributionToSalesReturn] 
        add constraint PFK_PaymentDistributionToSalesReturn 
        foreign key (Id) 
        references dbo.[PaymentDistribution]

    alter table dbo.[PaymentDistributionToSalesReturn] 
        add constraint FK_PaymentDistributionToSalesReturn_Sale 
        foreign key (SaleWaybillId) 
        references dbo.[SaleWaybill]

    alter table dbo.[PaymentDistributionToSalesReturn] 
        add constraint FK_PaymentDistributionToSalesReturn_ReturnWaybill 
        foreign key (ReturnWaybillId) 
        references dbo.[ReturnFromClientWaybill]
        
    /* Перенос данных из тарых разнесений в новые разнесения по продажам */
    INSERT INTO dbo.[PaymentDistributionToSale] ([Id], [SaleWaybillId]) 
		SELECT [Id], [SaleWaybillId] FROM dbo.[PaymentDistribution]
        
    /* Удаление столбца накладной продажи из абстрактного предка разнесения платежей */    
    ALTER TABLE dbo.[PaymentDistribution] DROP CONSTRAINT [FK_SaleWaybill_PaymentDistribution_SaleWaybillId]
    ALTER TABLE dbo.[PaymentDistribution] DROP COLUMN [SaleWaybillId]
        
    /* Убираем признак оплаты от клиента из платежа */
    ALTER TABLE dbo.[Payment] DROP COLUMN [IsFromClient]
    
    /* Добавляем поле "Тип оплаты" */
    ALTER TABLE dbo.[Payment] ADD PaymentTypeId tinyint NOT NULL CONSTRAINT DF_Payment_PaymentTypeId DEFAULT 1
	ALTER TABLE dbo.[Payment] DROP CONSTRAINT  DF_Payment_PaymentTypeId  
    
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
