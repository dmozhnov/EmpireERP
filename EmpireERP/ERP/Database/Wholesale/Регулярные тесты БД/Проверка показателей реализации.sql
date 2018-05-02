-- ПРОВЕДЕННЫЕ РЕАЛИЗАЦИИ
CREATE TABLE #AcceptedSaleIndicator(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[StorageId] [smallint] NOT NULL,
	[UserId] [int] NOT NULL,
	[ContractorId] [int] NOT NULL,
	[ClientOrganizationId] [int] NOT NULL,
	[TeamId] [smallint] NOT NULL,
	[ClientId] [int] NOT NULL,
	[AccountOrganizationId] [int] NOT NULL,
	[ArticleId] [int] NOT NULL,
	[BatchId] [uniqueidentifier] NOT NULL,
	[DealId] [int] NOT NULL,
	[PreviousId] [uniqueidentifier] NULL,
	[PurchaseCostSum] [decimal](18, 6) NOT NULL,
	[AccountingPriceSum] [decimal](18, 2) NOT NULL,
	[SalePriceSum] [decimal](18, 2) NOT NULL,
	[SoldCount] [decimal](18, 6) NOT NULL,
)
GO

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

insert into #AcceptedSaleIndicator([Id], [StartDate], [EndDate], [StorageId], [UserId], [ContractorId], [ClientOrganizationId], [TeamId], [ClientId], [AccountOrganizationId], 
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

-- группируем показатели с одинаковыми значениями
select StartDate = min(StartDate), EndDate = case when max(isnull(EndDate, '9999-01-01')) = '9999-01-01 00:00:00' then null else max(isnull(EndDate, '9999-01-01')) end, 
	StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId, ArticleId, BatchId, DealId, PurchaseCostSum, AccountingPriceSum, 
	SalePriceSum, SoldCount
into #GroupedAcceptedSaleIndicator
from AcceptedSaleIndicator 
group by StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId, ArticleId, BatchId, DealId, PurchaseCostSum, AccountingPriceSum, SalePriceSum, SoldCount

-- проверка
select *
from #GroupedAcceptedSaleIndicator A
full join #AcceptedSaleIndicator AA on
	A.StartDate	= AA.StartDate	and
	(A.EndDate = AA.EndDate or (A.EndDate is null and AA.EndDate is null)) and
	A.StorageId	= AA.StorageId and
	A.UserId = AA.UserId and 
	A.ContractorId = AA.ContractorId and
	A.ClientOrganizationId = AA.ClientOrganizationId and
	A.TeamId = AA.TeamId and
	A.ClientId = AA.ClientId and
	A.AccountOrganizationId = AA.AccountOrganizationId and
	A.ArticleId = AA.ArticleId and
	A.BatchId = AA.BatchId and
	A.DealId = AA.DealId and
	A.PurchaseCostSum = AA.PurchaseCostSum and
	A.AccountingPriceSum = AA.AccountingPriceSum and 
	A.SalePriceSum = AA.SalePriceSum and
	A.SoldCount = AA.SoldCount	
where A.StartDate is null or AA.StartDate is null and A.SoldCount <> 0 and A.StartDate <> A.EndDate -- отсекаем нулевые количества и совпадение дат
order by A.StorageId, A.UserId, A.ContractorId, A.ClientOrganizationId, A.TeamId, A.ClientId, A.AccountOrganizationId, A.ArticleId, A.BatchId, A.DealId, A.StartDate,
	AA.StorageId, AA.UserId, AA.ContractorId, AA.ClientOrganizationId, AA.TeamId, AA.ClientId, AA.AccountOrganizationId, AA.ArticleId, AA.BatchId, AA.DealId, AA.StartDate

drop table #AcceptedSaleIndicator
drop table #AcceptedSaleIndicator_Data
drop table #GroupedAcceptedSaleIndicator

-------------------------------------------------------------------------------------------------------------------------------------------
-- ОТГРУЖЕННЫЕ РЕАЛИЗАЦИИ
CREATE TABLE #ShippedSaleIndicator(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[StorageId] [smallint] NOT NULL,
	[UserId] [int] NOT NULL,
	[ContractorId] [int] NOT NULL,
	[ClientOrganizationId] [int] NOT NULL,
	[TeamId] [smallint] NOT NULL,
	[ClientId] [int] NOT NULL,
	[AccountOrganizationId] [int] NOT NULL,
	[ArticleId] [int] NOT NULL,
	[BatchId] [uniqueidentifier] NOT NULL,
	[DealId] [int] NOT NULL,
	[PreviousId] [uniqueidentifier] NULL,
	[PurchaseCostSum] [decimal](18, 6) NOT NULL,
	[AccountingPriceSum] [decimal](18, 2) NOT NULL,
	[SalePriceSum] [decimal](18, 2) NOT NULL,
	[SoldCount] [decimal](18, 6) NOT NULL,
)
GO

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

insert into #ShippedSaleIndicator([Id], [StartDate], [EndDate], [StorageId], [UserId], [ContractorId], [ClientOrganizationId], [TeamId], [ClientId], [AccountOrganizationId], 
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

-- группируем показатели с одинаковыми значениями
select StartDate = min(StartDate), EndDate = case when max(isnull(EndDate, '9999-01-01')) = '9999-01-01 00:00:00' then null else max(isnull(EndDate, '9999-01-01')) end, 
	StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId, ArticleId, BatchId, DealId, PurchaseCostSum, AccountingPriceSum, 
	SalePriceSum, SoldCount
into #GroupedShippedSaleIndicator
from ShippedSaleIndicator 
group by StorageId, UserId, ContractorId, ClientOrganizationId, TeamId, ClientId, AccountOrganizationId, ArticleId, BatchId, DealId, PurchaseCostSum, AccountingPriceSum, SalePriceSum, SoldCount

-- проверка
select *
from #GroupedShippedSaleIndicator A
full join #ShippedSaleIndicator AA on
	A.StartDate	= AA.StartDate	and
	(A.EndDate = AA.EndDate or (A.EndDate is null and AA.EndDate is null)) and
	A.StorageId	= AA.StorageId and
	A.UserId = AA.UserId and 
	A.ContractorId = AA.ContractorId and
	A.ClientOrganizationId = AA.ClientOrganizationId and
	A.TeamId = AA.TeamId and
	A.ClientId = AA.ClientId and
	A.AccountOrganizationId = AA.AccountOrganizationId and
	A.ArticleId = AA.ArticleId and
	A.BatchId = AA.BatchId and
	A.DealId = AA.DealId and
	A.PurchaseCostSum = AA.PurchaseCostSum and
	A.AccountingPriceSum = AA.AccountingPriceSum and 
	A.SalePriceSum = AA.SalePriceSum and
	A.SoldCount = AA.SoldCount	
where A.StartDate is null or AA.StartDate is null and A.SoldCount <> 0 and A.StartDate <> A.EndDate -- отсекаем нулевые количества и совпадение дат
order by A.StorageId, A.UserId, A.ContractorId, A.ClientOrganizationId, A.TeamId, A.ClientId, A.AccountOrganizationId, A.ArticleId, A.BatchId, A.DealId, A.StartDate,
	AA.StorageId, AA.UserId, AA.ContractorId, AA.ClientOrganizationId, AA.TeamId, AA.ClientId, AA.AccountOrganizationId, AA.ArticleId, AA.BatchId, AA.DealId, AA.StartDate

drop table #ShippedSaleIndicator
drop table #ShippedSaleIndicator_Data
drop table #GroupedShippedSaleIndicator

