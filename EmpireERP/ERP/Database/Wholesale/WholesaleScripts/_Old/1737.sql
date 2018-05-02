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
        
    /* ������� ������ �� ����� ���������� � ����� ���������� �� �������� */
    INSERT INTO dbo.[PaymentDistributionToSale] ([Id], [SaleWaybillId]) 
		SELECT [Id], [SaleWaybillId] FROM dbo.[PaymentDistribution]
        
    /* �������� ������� ��������� ������� �� ������������ ������ ���������� �������� */    
    ALTER TABLE dbo.[PaymentDistribution] DROP CONSTRAINT [FK_SaleWaybill_PaymentDistribution_SaleWaybillId]
    ALTER TABLE dbo.[PaymentDistribution] DROP COLUMN [SaleWaybillId]
        
    /* ������� ������� ������ �� ������� �� ������� */
    ALTER TABLE dbo.[Payment] DROP COLUMN [IsFromClient]
    
    /* ��������� ���� "��� ������" */
    ALTER TABLE dbo.[Payment] ADD PaymentTypeId tinyint NOT NULL CONSTRAINT DF_Payment_PaymentTypeId DEFAULT 1
	ALTER TABLE dbo.[Payment] DROP CONSTRAINT  DF_Payment_PaymentTypeId  
    
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
