SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRY
	BEGIN TRAN
					
		delete AcceptedSaleIndicator
		delete ShippedSaleIndicator
					
		-- ПРОВЕДЕННЫЕ РЕАЛИЗАЦИИ
		select Id = newid(), EW.AcceptanceDate, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
			C.AccountOrganizationId, RW.ArticleId, ReceiptWaybillRowId = RW.Id, DealId = D.Id, 
			PurchaseCostSum = SUM(Round(RW.PurchaseCost * SR.SellingCount, 6)),
			AccountingPriceSum = SUM(Round(AAP.AccountingPrice * SR.SellingCount,2)),
			SalePriceSum = SUM(Round(SR.SalePrice * SR.SellingCount,2)),
			SellingCount = SUM(SR.SellingCount),
			RowNumber = ROW_NUMBER() OVER ( partition by EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
				C.AccountOrganizationId, RW.ArticleId, RW.Id, D.Id order by EW.AcceptanceDate),
			-- хэш-ключ для упрощения связывания таблицы самой с собой
			KeyHash = CAST(HashBytes('SHA1', 
				CAST(EW.ExpenditureWaybillSenderStorageId as varchar(255)) + '_' +
				CAST(SW.SaleWaybillCuratorId as varchar(255)) + '_' +
				CAST(R.ProviderId as varchar(255)) + '_' +
				CAST(C.ContractorOrganizationId as varchar(255)) + '_' +
				CAST(SW.TeamId as varchar(255)) + '_' +
				CAST(D.ClientId as varchar(255)) + '_' +
				CAST(C.AccountOrganizationId as varchar(255)) + '_' +
				CAST(RW.ArticleId as varchar(255)) + '_' +
				CAST(RW.Id as varchar(255)) + '_' +
				CAST(D.Id as varchar(255))
			) as varchar(500))
		into #AcceptedSaleIndicator_Data
		from SaleWaybillRow SR
		join ExpenditureWaybillRow ER on ER.Id = SR.Id
		join ExpenditureWaybill EW on EW.Id = SR.SaleWaybillId and EW.AcceptanceDate is not null
		join SaleWaybill SW on SW.Id = EW.Id
		join ReceiptWaybillRow RW on RW.Id = ER.ExpenditureWaybillRowReceiptWaybillRowId
		join ReceiptWaybill R on R.Id = RW.ReceiptWaybillId
		join Deal D on D.Id = SW.DealId
		join Contract C on C.Id = D.ClientContractId
		join ArticleAccountingPrice AAP on AAP.Id = ER.ExpenditureWaybillSenderArticleAccountingPriceId
		where SR.DeletionDate is null and SW.DeletionDate is null
		group by EW.AcceptanceDate, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
			C.AccountOrganizationId, RW.ArticleId, RW.Id, D.Id

		create index idx_AcceptedSaleIndicator_Data on #AcceptedSaleIndicator_Data (KeyHash, RowNumber)

		insert into AcceptedSaleIndicator([Id], [StartDate], [EndDate], [StorageId], [UserId], [ContractorId], [ClientOrganizationId], [TeamId], [ClientId], [AccountOrganizationId], 
			[ArticleId], [BatchId], [DealId], [PreviousId], [PurchaseCostSum], [AccountingPriceSum], [SalePriceSum], [SoldCount])
		select T.Id, T.AcceptanceDate, 
			(select AcceptanceDate from #AcceptedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber = T.RowNumber + 1),
			T.ExpenditureWaybillSenderStorageId, T.SaleWaybillCuratorId, T.ProviderId, T.ContractorOrganizationId, T.TeamId, T.ClientId,
			T.AccountOrganizationId, T.ArticleId, T.ReceiptWaybillRowId, T.DealId,
			PreviousId = (select Id from #AcceptedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber = T.RowNumber - 1),
			PurchaseCostSum = (select SUM(PurchaseCostSum) from #AcceptedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
			AccountingPriceSum = (select SUM(AccountingPriceSum) from #AcceptedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
			SalePriceSum = (select SUM(SalePriceSum) from #AcceptedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
			SoldCount = (select SUM(SellingCount) from #AcceptedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber)
		from #AcceptedSaleIndicator_Data T
				
		drop table #AcceptedSaleIndicator_Data

		-------------------------------------------------------------------------------------------------------------------------------------------
		-- ОТГРУЖЕННЫЕ РЕАЛИЗАЦИИ
		select Id = newid(), EW.ShippingDate, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
			C.AccountOrganizationId, RW.ArticleId, ReceiptWaybillRowId = RW.Id, DealId = D.Id, 
			PurchaseCostSum = SUM(Round(RW.PurchaseCost * SR.SellingCount, 6)),
			AccountingPriceSum = SUM(Round(AAP.AccountingPrice * SR.SellingCount,2)),
			SalePriceSum = SUM(Round(SR.SalePrice * SR.SellingCount,2)),
			SellingCount = SUM(SR.SellingCount),
			RowNumber = ROW_NUMBER() OVER ( partition by EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
				C.AccountOrganizationId, RW.ArticleId, RW.Id, D.Id order by EW.ShippingDate),
			-- хэш-ключ для упрощения связывания таблицы самой с собой
			KeyHash = CAST(HashBytes('SHA1', 
				CAST(EW.ExpenditureWaybillSenderStorageId as varchar(255)) + '_' +
				CAST(SW.SaleWaybillCuratorId as varchar(255)) + '_' +
				CAST(R.ProviderId as varchar(255)) + '_' +
				CAST(C.ContractorOrganizationId as varchar(255)) + '_' +
				CAST(SW.TeamId as varchar(255)) + '_' +
				CAST(D.ClientId as varchar(255)) + '_' +
				CAST(C.AccountOrganizationId as varchar(255)) + '_' +
				CAST(RW.ArticleId as varchar(255)) + '_' +
				CAST(RW.Id as varchar(255)) + '_' +
				CAST(D.Id as varchar(255))
			) as varchar(500))
		into #ShippedSaleIndicator_Data
		from SaleWaybillRow SR
		join ExpenditureWaybillRow ER on ER.Id = SR.Id
		join ExpenditureWaybill EW on EW.Id = SR.SaleWaybillId and EW.ShippingDate is not null
		join SaleWaybill SW on SW.Id = EW.Id
		join ReceiptWaybillRow RW on RW.Id = ER.ExpenditureWaybillRowReceiptWaybillRowId
		join ReceiptWaybill R on R.Id = RW.ReceiptWaybillId
		join Deal D on D.Id = SW.DealId
		join Contract C on C.Id = D.ClientContractId
		join ArticleAccountingPrice AAP on AAP.Id = ER.ExpenditureWaybillSenderArticleAccountingPriceId
		where SR.DeletionDate is null and SW.DeletionDate is null
		group by EW.ShippingDate, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, R.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId,
			C.AccountOrganizationId, RW.ArticleId, RW.Id, D.Id

		create index idx_ShippedSaleIndicator_Data on #ShippedSaleIndicator_Data (KeyHash, RowNumber)

		insert into ShippedSaleIndicator([Id], [StartDate], [EndDate], [StorageId], [UserId], [ContractorId], [ClientOrganizationId], [TeamId], [ClientId], [AccountOrganizationId], 
			[ArticleId], [BatchId], [DealId], [PreviousId], [PurchaseCostSum], [AccountingPriceSum], [SalePriceSum], [SoldCount])
		select T.Id, T.ShippingDate, 
			(select ShippingDate from #ShippedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber = T.RowNumber + 1),
			T.ExpenditureWaybillSenderStorageId, T.SaleWaybillCuratorId, T.ProviderId, T.ContractorOrganizationId, T.TeamId, T.ClientId,
			T.AccountOrganizationId, T.ArticleId, T.ReceiptWaybillRowId, T.DealId,
			PreviousId = (select Id from #ShippedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber = T.RowNumber - 1),
			PurchaseCostSum = (select SUM(PurchaseCostSum) from #ShippedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
			AccountingPriceSum = (select SUM(AccountingPriceSum) from #ShippedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
			SalePriceSum = (select SUM(SalePriceSum) from #ShippedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
			SoldCount = (select SUM(SellingCount) from #ShippedSaleIndicator_Data where KeyHash = T.KeyHash and RowNumber <= T.RowNumber)
		from #ShippedSaleIndicator_Data T
		
		drop table #ShippedSaleIndicator_Data
								
		COMMIT TRAN
END TRY
BEGIN CATCH
	IF XACT_STATE() <> 0 ROLLBACK TRAN
	RETURN
END CATCH