BEGIN TRY
    BEGIN TRAN

ALTER TABLE  dbo.AccountingPriceList 
	  ADD IsUsedByAccountingPriceChanging bit NOT NULL CONSTRAINT DF_IsUsedByAccountingPriceChanging DEFAULT (0) 


    create table dbo.AccountingPriceListWaybillTaking (
        Id UNIQUEIDENTIFIER not null,
       WaybillRowId UNIQUEIDENTIFIER not null,
       WaybillTypeId TINYINT not null,
       ArticleAccountingPriceId UNIQUEIDENTIFIER not null,
       BindingDate DATETIME not null,
       Count DECIMAL(18, 6) not null,
       primary key (Id)
    )

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
    RETURN
END CATCH
