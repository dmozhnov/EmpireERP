BEGIN TRY
	BEGIN TRAN		

	ALTER TABLE dbo.[ProducerOrganization] ADD ManufacturerId SMALLINT null;
    alter table dbo.[ProducerOrganization] 
        add constraint FK_ProducerOrganization_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.[Manufacturer];

	alter table dbo.[Producer] drop constraint FK_Producer_Manufacturer;
    ALTER TABLE dbo.[Producer] DROP COLUMN ManufacturerId;

	if exists(select * from sys.indexes where name = 'IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId')
		drop index [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] on [ReceiptWaybillRow]
	
	alter table [ReceiptWaybillRow] alter column ManufacturerId SMALLINT not null;

	CREATE INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [ReceiptWaybillRow] ([DeletionDate], [ReceiptWaybillId]) 
	INCLUDE ([Id], [ArticleMeasureUnitScale], [PendingCount], [PendingSum], [ReceiptedCount], [ProviderCount], [ApprovedCount], [ApprovedSum], [PurchaseCost],		[CustomsDeclarationNumber], [CreationDate], [FinallyMovementDate], [ReservedCount], [AcceptedCount], [ShippedCount], [FinallyMovedCount],	[RecipientArticleAccountingPriceId], [ArticleId], [PendingValueAddedTaxId], [ApprovedValueAddedTaxId], [CountryId], [ManufacturerId])

	PRINT 'Обновление выполнено успешно'
		
	--ROLLBACK TRAN -- Очень помогает при отладке, а COMMIT TRAN при отладке комментируем
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
