BEGIN TRY
    BEGIN TRAN

    create table dbo.[ProductionOrderMaterialsPackage] (
        Id UNIQUEIDENTIFIER not null,
       Name VARCHAR(250) not null,
       Description VARCHAR(250) not null,
       Comment VARCHAR(4000) not null,
       CreationDate DATETIME not null,
       LastChangeDate DATETIME not null,
       DeletionDate DATETIME null,
       ProductionOrderMaterialsPackageSize DECIMAL(18, 6) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

	create table dbo.[ProductionOrderMaterialDocument] (
        Id UNIQUEIDENTIFIER not null,
       FileName VARCHAR(250) not null,
       Description VARCHAR(250) not null,
       CreationDate DATETIME not null,
       LastChangeDate DATETIME not null,
       DeletionDate DATETIME null,
       Size DECIMAL(18, 6) not null,
       CreatedById INT not null,
       MaterialsPackageId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
	
    alter table dbo.[ProductionOrderMaterialsPackage] 
        add constraint FK_ProductionOrderMaterialsPackage_ProductionOrder 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

	alter table dbo.[ProductionOrderMaterialDocument] 
        add constraint FK_ProductionOrderMaterialDocument_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]
		
    alter table dbo.[ProductionOrderMaterialDocument] 
        add constraint FK_ProductionOrderMaterialsPackage_ProductionOrderMaterialDocument_MaterialsPackageId 
        foreign key (MaterialsPackageId) 
        references dbo.[ProductionOrderMaterialsPackage]


    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH