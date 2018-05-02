BEGIN TRY
	BEGIN TRAN		
	
	select *
	into #T
	from ArticleAccountingPriceIndicator
	
	drop table ArticleAccountingPriceIndicator
	
	create table dbo.[ArticleAccountingPriceIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       ArticleId INT not null,
       AccountingPrice DECIMAL(18, 2) not null,
       AccountingPriceListId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )
	
	insert into [ArticleAccountingPriceIndicator] (Id, StartDate, EndDate, StorageId, ArticleId, AccountingPrice, AccountingPriceListId)
	select NEWID(), StartDate, EndDate, StorageId, ArticleId, AccountingPrice, AccountingPriceListId
	from #T
	
	drop table #T
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
