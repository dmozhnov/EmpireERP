BEGIN TRY
	BEGIN TRAN		

	create table dbo.[ArticleMovementFactualFinancialIndicator] (
        Id BIGINT IDENTITY NOT NULL,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       RecipientId INT null,
       RecipientStorageId SMALLINT null,
       SenderId INT null,
       SenderStorageId SMALLINT null,
       PreviousIndicatorId BIGINT null,
       WaybillId UNIQUEIDENTIFIER not null,
       ArticleMovementOperationType TINYINT not null,
       AccountingPriceSum DECIMAL(18, 6) not null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       SalePriceSum DECIMAL(18, 6) not null,
       primary key (Id)
    )

    
create table dbo.[ArticleMovementOperationCountIndicator] (
        Id BIGINT IDENTITY NOT NULL,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       PreviousIndicatorId BIGINT null,
       WaybillId UNIQUEIDENTIFIER not null,
       ArticleMovementOperationType TINYINT not null,
       StorageId SMALLINT not null,
       Count BIGINT not null,
       primary key (Id)
    )

    
create table dbo.[ArticlePriceChangeIndicator] (
        Id BIGINT IDENTITY NOT NULL,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       PreviousIndicatorId BIGINT null,
       AccountingPriceListId UNIQUEIDENTIFIER not null,
       AccountOrganizationId INT not null,
       StorageId SMALLINT not null,
       NeedToRecalculate BIT not null,
       ChangeSum DECIMAL(18, 6) not null,
       primary key (Id)
    )

	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
