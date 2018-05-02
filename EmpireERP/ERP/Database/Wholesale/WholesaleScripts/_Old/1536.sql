BEGIN TRY
    BEGIN TRAN

if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[SaleIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[SaleIndicator]

if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReturnFromClientIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReturnFromClientIndicator]

create table dbo.[ReturnFromClientIndicator] (
        Id BIGINT IDENTITY NOT NULL,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ProviderId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT null,
       ClientId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       ReturnFromClientWaybillId UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       PreviousWaybillId UNIQUEIDENTIFIER null,
       AccountingPriceSum DECIMAL(18, 6) not null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       SalePriceSum DECIMAL(18, 6) not null,
       ReturnedCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[SaleIndicator] (
        Id BIGINT IDENTITY NOT NULL,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ProviderId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT null,
       ClientId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       PreviousWaybillId UNIQUEIDENTIFIER null,
       AccountingPriceSum DECIMAL(18, 6) not null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       SalePriceSum DECIMAL(18, 6) not null,
       SoldCount DECIMAL(18, 6) not null,
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
