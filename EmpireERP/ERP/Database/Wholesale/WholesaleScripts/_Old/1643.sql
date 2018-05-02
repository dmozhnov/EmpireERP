BEGIN TRY
    BEGIN TRAN

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

    alter table dbo.[ProductionOrderCustomsDeclaration] 
        add constraint FK_ProductionOrderCustomsDeclaration_ProductionOrder 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
