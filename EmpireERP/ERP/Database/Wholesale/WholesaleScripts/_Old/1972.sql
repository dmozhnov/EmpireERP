BEGIN TRY
	BEGIN TRAN		
	
	    create table dbo.[ExactArticlePriceChangeIndicator] (
        Id BIGINT IDENTITY NOT NULL,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       PreviousIndicatorId BIGINT null,
       AccountingPriceListId UNIQUEIDENTIFIER not null,
       StorageId SMALLINT not null,
       AccountingPriceSum DECIMAL(18, 6) not null,
       primary key (Id)
    )
	
	PRINT '���������� ��������� �������'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH
