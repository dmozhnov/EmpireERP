CREATE TABLE #ArticleMovementFactualFinancialIndicator(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[RecipientId] [int] NULL,
	[RecipientStorageId] [smallint] NULL,
	[SenderId] [int] NULL,
	[SenderStorageId] [smallint] NULL,
	[PreviousId] [uniqueidentifier] NULL,
	[WaybillId] [uniqueidentifier] NOT NULL,
	[ArticleMovementOperationType] [tinyint] NOT NULL,
	[AccountingPriceSum] [decimal](18, 2) NOT NULL,
	[PurchaseCostSum] [decimal](18, 6) NOT NULL,
	[SalePriceSum] [decimal](18, 2) NOT NULL,
)
GO

CREATE TABLE #ArticleMovementFactualFinancialIndicator_Data(
	[StartDate] [datetime] NOT NULL,	
	[RecipientId] [int] NULL,
	[RecipientStorageId] [smallint] NULL,
	[SenderId] [int] NULL,
	[SenderStorageId] [smallint] NULL,	
	[WaybillId] [uniqueidentifier] NOT NULL,
	[ArticleMovementOperationType] [tinyint] NOT NULL,
	[AccountingPriceSum] [decimal](18, 2) NOT NULL,
	[PurchaseCostSum] [decimal](18, 6) NOT NULL,
	[SalePriceSum] [decimal](18, 2) NOT NULL,
	CreationDate datetime NOT NULL
)
GO

CREATE TABLE #ArticleMovementFactualFinancialIndicator_HashedKeyData(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[StartDate] [datetime] NOT NULL,	
	[RecipientId] [int] NULL,
	[RecipientStorageId] [smallint] NULL,
	[SenderId] [int] NULL,
	[SenderStorageId] [smallint] NULL,	
	[ArticleMovementOperationType] [tinyint] NOT NULL,
	[AccountingPriceSum] [decimal](18, 2) NOT NULL,
	[PurchaseCostSum] [decimal](18, 6) NOT NULL,
	[SalePriceSum] [decimal](18, 2) NOT NULL,
	RowNumber int NOT NULL,
	KeyHash varchar(500) NOT NULL
)
GO

CREATE FUNCTION GetReceiptWaybillRowCurrentCount
(
	@ReceiptWaybillRowId uniqueidentifier,
	@ReceiptWaybillStateId tinyint
)
RETURNS decimal(18, 6)
AS
BEGIN	
	
	declare @AreCountDivergencesAfterReceipt bit, @AreSumDivergencesAfterReceipt bit,
		@PendingCount numeric(18, 6), @ApprovedCount numeric(18, 6)
	
	select @AreCountDivergencesAfterReceipt = AreCountDivergencesAfterReceipt, @AreSumDivergencesAfterReceipt = AreSumDivergencesAfterReceipt,
		@PendingCount = PendingCount, @ApprovedCount = ApprovedCount
	from ReceiptWaybillRow 
	where Id = @ReceiptWaybillRowId
	
	if @ReceiptWaybillStateId in (1, 7)
		return @PendingCount
	
	if @ReceiptWaybillStateId in (3, 4, 5)
		if @AreCountDivergencesAfterReceipt = 1 or @AreSumDivergencesAfterReceipt = 1
			return @PendingCount
		else
			return @ApprovedCount
	
	return @ApprovedCount
END
GO


insert into #ArticleMovementFactualFinancialIndicator_Data([StartDate],[RecipientId],[RecipientStorageId],[SenderId],
	[SenderStorageId],[WaybillId],[ArticleMovementOperationType],[AccountingPriceSum],[PurchaseCostSum],[SalePriceSum],CreationDate)
-- приемка приходов без расхождений
select StartDate = w.ReceiptDate, RecipientId = AccountOrganizationId, RecipientStorageId = ReceiptWaybillReceiptStorageId, 
	SenderId = null, SenderStorageId = null, WaybillId = w.Id, ArticleMovementOperationType = 1,
	AccountingPriceSum = sum(round(a.AccountingPrice * dbo.GetReceiptWaybillRowCurrentCount(r.id, ReceiptWaybillStateId), 2)),
	PurchaseCostSum = sum(round(r.PurchaseCost * dbo.GetReceiptWaybillRowCurrentCount(r.id, ReceiptWaybillStateId), 6)),
	SalePriceSum = 0, w.CreationDate
from ReceiptWaybill w
join ReceiptWaybillRow r on r.ReceiptWaybillId = w.id and r.DeletionDate is null and
	r.AreCountDivergencesAfterReceipt = 0 and r.AreSumDivergencesAfterReceipt = 0 and r.PendingCount > 0
join ArticleAccountingPrice a on a.id = r.RecipientArticleAccountingPriceId
where w.DeletionDate is null and w.ReceiptDate is not null
group by w.ReceiptDate, AccountOrganizationId, ReceiptWaybillReceiptStorageId, w.Id, w.CreationDate


-- согласование расхождений после приемки приходной накладной
union all
select StartDate = w.ApprovementDate, RecipientId = AccountOrganizationId, RecipientStorageId = ReceiptWaybillReceiptStorageId, 
	SenderId = null, SenderStorageId = null, WaybillId = w.Id, ArticleMovementOperationType = 1,
	AccountingPriceSum = sum(round(a.AccountingPrice * dbo.GetReceiptWaybillRowCurrentCount(r.id, ReceiptWaybillStateId), 2)),
	PurchaseCostSum = sum(round(r.PurchaseCost * dbo.GetReceiptWaybillRowCurrentCount(r.id, ReceiptWaybillStateId), 6)),
	SalePriceSum = 0, w.CreationDate
from ReceiptWaybill w
join ReceiptWaybillRow r on r.ReceiptWaybillId = w.id and r.DeletionDate is null and
	(r.AreCountDivergencesAfterReceipt = 1 or r.AreSumDivergencesAfterReceipt = 1 or r.PendingCount = 0)
join ArticleAccountingPrice a on a.id = r.RecipientArticleAccountingPriceId
where w.DeletionDate is null and w.ApprovementDate is not null
group by w.ApprovementDate, AccountOrganizationId, ReceiptWaybillReceiptStorageId, w.Id, w.CreationDate

-- приход при приемке накладной перемещения
union all
select StartDate = w.ReceiptDate, RecipientId = w.RecipientId, RecipientStorageId = w.RecipientStorageId, 
	SenderId = w.SenderId, SenderStorageId = w.SenderStorageId, WaybillId = w.Id, ArticleMovementOperationType = 4,
	AccountingPriceSum = sum(round(a.AccountingPrice * r.MovingCount, 2)),
	PurchaseCostSum = sum(round(rr.PurchaseCost * r.MovingCount, 6)),
	SalePriceSum = 0, w.CreationDate
from MovementWaybill w
join MovementWaybillRow r on r.MovementWaybillId = w.id and r.DeletionDate is null
join ArticleAccountingPrice a on a.id = r.MovementWaybillRecipientArticleAccountingPriceId
join ReceiptWaybillRow rr on rr.id = r.ReceiptWaybillRowId
where w.DeletionDate is null and w.ReceiptDate is not null
group by w.ReceiptDate, w.RecipientId, w.RecipientStorageId, w.SenderId, w.SenderStorageId, w.Id, w.CreationDate

-- расход при приемке накладной перемещения
union all
select StartDate = w.ReceiptDate, RecipientId = w.RecipientId, RecipientStorageId = w.RecipientStorageId, 
	SenderId = w.SenderId, SenderStorageId = w.SenderStorageId, WaybillId = w.Id, ArticleMovementOperationType = 5,
	AccountingPriceSum = sum(round(a.AccountingPrice * r.MovingCount, 2)),
	PurchaseCostSum = sum(round(rr.PurchaseCost * r.MovingCount, 6)),
	SalePriceSum = 0, w.CreationDate
from MovementWaybill w
join MovementWaybillRow r on r.MovementWaybillId = w.id and r.DeletionDate is null
join ArticleAccountingPrice a on a.id = r.MovementWaybillSenderArticleAccountingPriceId
join ReceiptWaybillRow rr on rr.id = r.ReceiptWaybillRowId
where w.DeletionDate is null and w.ReceiptDate is not null
group by w.ReceiptDate, w.RecipientId, w.RecipientStorageId, w.SenderId, w.SenderStorageId, w.Id, w.CreationDate

-- отгрузка накладной списания
union all
select StartDate = w.WriteoffDate, RecipientId = null, RecipientStorageId = null, 
	SenderId = w.WriteoffWaybillSenderId, SenderStorageId = w.WriteoffWaybillSenderStorageId, WaybillId = w.Id, ArticleMovementOperationType = 6,
	AccountingPriceSum = sum(round(a.AccountingPrice * r.WritingoffCount, 2)),
	PurchaseCostSum = sum(round(rr.PurchaseCost * r.WritingoffCount, 6)),
	SalePriceSum = 0, w.CreationDate
from WriteoffWaybill w
join WriteoffWaybillRow r on r.WriteoffWaybillId = w.id and r.DeletionDate is null
join ArticleAccountingPrice a on a.id = r.WriteoffSenderArticleAccountingPriceId
join ReceiptWaybillRow rr on rr.id = r.WriteoffReceiptWaybillRowId
where w.DeletionDate is null and w.WriteoffDate is not null
group by w.WriteoffDate, w.WriteoffWaybillSenderId, w.WriteoffWaybillSenderStorageId, w.Id, w.CreationDate

-- отгрузка накладной реализации товаров
union all
select StartDate = ew.ShippingDate, RecipientId = null, RecipientStorageId = null, 
	SenderId = c.AccountOrganizationId, SenderStorageId = ew.ExpenditureWaybillSenderStorageId, WaybillId = w.Id, ArticleMovementOperationType = 2,
	AccountingPriceSum = sum(round(a.AccountingPrice * r.SellingCount, 2)),
	PurchaseCostSum = sum(round(rr.PurchaseCost * r.SellingCount, 6)),
	SalePriceSum = sum(round(r.SalePrice * r.SellingCount, 2)),
	w.CreationDate
from SaleWaybill w
join ExpenditureWaybill ew on ew.id = w.id
join SaleWaybillRow r on r.SaleWaybillId = w.id and r.DeletionDate is null
join ExpenditureWaybillRow er on er.id = r.id
join ArticleAccountingPrice a on a.id = er.ExpenditureWaybillSenderArticleAccountingPriceId
join ReceiptWaybillRow rr on rr.id = er.ExpenditureWaybillRowReceiptWaybillRowId
join Deal d on d.id = w.DealId
join Contract c on c.id = d.ClientContractId
where w.DeletionDate is null and ew.ShippingDate is not null
group by ew.ShippingDate, c.AccountOrganizationId, ew.ExpenditureWaybillSenderStorageId, w.Id, w.CreationDate

-- приемка возврата товара от клиента
union all
select StartDate = w.ReceiptDate, RecipientId = w.ReturnFromClientWaybillRecipientId, RecipientStorageId = w.ReturnFromClientWaybillRecipientStorageId, 
	SenderId = null, SenderStorageId = null, WaybillId = w.Id, ArticleMovementOperationType = 7,
	AccountingPriceSum = sum(round(a.AccountingPrice * r.ReturnCount, 2)),
	PurchaseCostSum = sum(round(rr.PurchaseCost * r.ReturnCount, 6)),
	SalePriceSum = sum(round(sr.SalePrice * r.ReturnCount, 2)),
	w.CreationDate
from ReturnFromClientWaybill w
join ReturnFromClientWaybillRow r on r.ReturnFromClientWaybillId = w.id and r.DeletionDate is null
join ArticleAccountingPrice a on a.id = r.ReturnFromClientArticleAccountingPriceId
join ReceiptWaybillRow rr on rr.id = r.ReceiptWaybillRowId
join SaleWaybillRow sr on sr.id = r.SaleWaybillRowId
where w.DeletionDate is null and w.ReceiptDate is not null
group by w.ReceiptDate, w.ReturnFromClientWaybillRecipientId, w.ReturnFromClientWaybillRecipientStorageId, w.Id, w.CreationDate


create index idx_ArticleMovementFactualFinancialIndicator_Data on #ArticleMovementFactualFinancialIndicator_Data ([StartDate],[RecipientId],[RecipientStorageId],[SenderId],
	[SenderStorageId],[WaybillId],[ArticleMovementOperationType],CreationDate)


insert into #ArticleMovementFactualFinancialIndicator_HashedKeyData (StartDate,RecipientId,RecipientStorageId,SenderId,
	SenderStorageId,ArticleMovementOperationType,AccountingPriceSum,PurchaseCostSum,SalePriceSum, RowNumber, KeyHash)
select StartDate, RecipientId, RecipientStorageId, SenderId, SenderStorageId, ArticleMovementOperationType,
	AccountingPriceSum = sum(AccountingPriceSum),
	PurchaseCostSum = sum(PurchaseCostSum),
	SalePriceSum = sum(SalePriceSum),
	RowNumber = ROW_NUMBER() OVER ( partition by RecipientId, RecipientStorageId, SenderId, SenderStorageId, 
		ArticleMovementOperationType order by StartDate),
	-- хэш-ключ для упрощения связывания таблицы самой с собой
	KeyHash = CAST(HashBytes('SHA1', 
		ISNULL(CAST(RecipientId as varchar(255)), '') + '_' +
		ISNULL(CAST(RecipientStorageId as varchar(255)), '') + '_' +
		ISNULL(CAST(SenderId as varchar(255)), '') + '_' +
		ISNULL(CAST(SenderStorageId as varchar(255)), '') + '_' +
		CAST(ArticleMovementOperationType as varchar(255))
	) as varchar(500))
from #ArticleMovementFactualFinancialIndicator_Data
group by StartDate, RecipientId, RecipientStorageId, SenderId, SenderStorageId,  ArticleMovementOperationType


create index idx_ArticleMovementFactualFinancialIndicator_HashedKeyData on #ArticleMovementFactualFinancialIndicator_HashedKeyData (KeyHash, RowNumber)


insert into #ArticleMovementFactualFinancialIndicator(Id,StartDate,EndDate,RecipientId,RecipientStorageId,SenderId,PreviousId,
	SenderStorageId,WaybillId,ArticleMovementOperationType,AccountingPriceSum,PurchaseCostSum,SalePriceSum)
select T.Id, T.StartDate, 
	(select StartDate from #ArticleMovementFactualFinancialIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber = T.RowNumber + 1),
	RecipientId,RecipientStorageId,SenderId,
	PreviousId = (select Id from #ArticleMovementFactualFinancialIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber = T.RowNumber - 1),
	SenderStorageId,
	WaybillId = 
		(
			select top 1 WaybillId 
			from #ArticleMovementFactualFinancialIndicator_Data
			where StartDate	= T.StartDate and			
			(RecipientId = T.RecipientId or (RecipientId is null and T.RecipientId is null)) and
			(RecipientStorageId = T.RecipientStorageId or (RecipientStorageId is null and T.RecipientStorageId is null)) and
			(SenderId = T.SenderId or (SenderId is null and T.SenderId is null)) and
			(SenderStorageId = T.SenderStorageId or (SenderStorageId is null and T.SenderStorageId is null)) and
			ArticleMovementOperationType = T.ArticleMovementOperationType
			order by CreationDate
		),
	ArticleMovementOperationType,	
	AccountingPriceSum = (select SUM(AccountingPriceSum) from #ArticleMovementFactualFinancialIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
	PurchaseCostSum = (select SUM(PurchaseCostSum) from #ArticleMovementFactualFinancialIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),
	SalePriceSum = (select SUM(SalePriceSum) from #ArticleMovementFactualFinancialIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber <= T.RowNumber)
from #ArticleMovementFactualFinancialIndicator_HashedKeyData T


-- группируем показатели с одинаковыми значениями
select *,
	WaybillId = 
	(
		select top 1 WaybillId 
		from ArticleMovementFactualFinancialIndicator
		where StartDate	= T.StartDate and			
		(RecipientId = T.RecipientId or (RecipientId is null and T.RecipientId is null)) and
		(RecipientStorageId = T.RecipientStorageId or (RecipientStorageId is null and T.RecipientStorageId is null)) and
		(SenderId = T.SenderId or (SenderId is null and T.SenderId is null)) and
		(SenderStorageId = T.SenderStorageId or (SenderStorageId is null and T.SenderStorageId is null)) and
		ArticleMovementOperationType = T.ArticleMovementOperationType
	)
into #GroupedArticleMovementFactualFinancialIndicator
from 
(
	select StartDate = min(StartDate), EndDate = case when max(isnull(EndDate, '9999-01-01')) = '9999-01-01 00:00:00' then null else max(isnull(EndDate, '9999-01-01')) end, 
		RecipientId, RecipientStorageId, SenderId, SenderStorageId, ArticleMovementOperationType, AccountingPriceSum, PurchaseCostSum, SalePriceSum
	from ArticleMovementFactualFinancialIndicator
	group by RecipientId, RecipientStorageId, SenderId, SenderStorageId, ArticleMovementOperationType, AccountingPriceSum, PurchaseCostSum, SalePriceSum
)T

-- проверка
select *
from #GroupedArticleMovementFactualFinancialIndicator A
full join #ArticleMovementFactualFinancialIndicator AA on
	A.StartDate	= AA.StartDate	and
	(A.EndDate = AA.EndDate or (A.EndDate is null and AA.EndDate is null)) and
	(A.RecipientId = AA.RecipientId or (A.RecipientId is null and AA.RecipientId is null)) and
	(A.RecipientStorageId = AA.RecipientStorageId or (A.RecipientStorageId is null and AA.RecipientStorageId is null)) and
	(A.SenderId = AA.SenderId or (A.SenderId is null and AA.SenderId is null)) and
	(A.SenderStorageId = AA.SenderStorageId or (A.SenderStorageId is null and AA.SenderStorageId is null)) and
	-- при наличии нескольких накладных с одинаковыми параметрами на одну и ту же дату определить накладную, которая попала в показатель, невозможно
	-- A.WaybillId	= AA.WaybillId and
	A.ArticleMovementOperationType	= AA.ArticleMovementOperationType and
	A.AccountingPriceSum = AA.AccountingPriceSum and
	A.PurchaseCostSum = AA.PurchaseCostSum and
	A.SalePriceSum = AA.SalePriceSum
where A.SalePriceSum is null or AA.SalePriceSum is null and 
	not (A.AccountingPriceSum = 0 and A.PurchaseCostSum = 0 and A.SalePriceSum = 0) -- отсекаем нулевые количества и совпадение дат
order by A.RecipientId, A.RecipientStorageId, A.SenderId, A.SenderStorageId, A.ArticleMovementOperationType, A.StartDate,
	AA.RecipientId, AA.RecipientStorageId, AA.SenderId, AA.SenderStorageId, AA.ArticleMovementOperationType, AA.StartDate


drop function GetReceiptWaybillRowCurrentCount
drop table #ArticleMovementFactualFinancialIndicator_Data
drop table #ArticleMovementFactualFinancialIndicator
drop table #ArticleMovementFactualFinancialIndicator_HashedKeyData
drop table #GroupedArticleMovementFactualFinancialIndicator