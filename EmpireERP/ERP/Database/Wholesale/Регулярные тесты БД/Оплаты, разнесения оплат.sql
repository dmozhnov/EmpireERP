-- ТЕСТЫ ОПЛАТ, РАЗНЕСЕНИЙ ОПЛАТ

-- Сумма разнесений больше, чем DistributedSum у платежного документа
select dpd.id, dpd.DistributedSum, isnull(sum(dpdd.sum),0)
from DealPaymentDocument dpd
left join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.id
group by dpd.id, dpd.DistributedSum
having dpd.DistributedSum < isnull(sum(dpdd.sum),0)

-- Сумма разнесений не равна DistributedSum у платежного документа
select dpd.id, dpd.DistributedSum, sum(dpdd.sum)
from DealPaymentDocument dpd
join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.id
group by dpd.id, dpd.DistributedSum
having dpd.DistributedSum <> sum(dpdd.sum)

-- Разнесенная в результате возврата сумма больше суммы возврата
select pd.id, -pd.sum, TT.sum
from DealPaymentDocumentDistribution pd
join DealPaymentDocumentDistributionToReturnFromClientWaybill pdr on pd.id = pdr.id
left join 
(
	select SourceDistributionToReturnFromClientWaybillId, sum = sum(sum)
	from
	(
		select SourceDistributionToReturnFromClientWaybillId, pd.sum
		from DealPaymentDocumentDistribution pd
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddpd on dpddpd.id = pd.id
		where SourceDistributionToReturnFromClientWaybillId is not null

		union all
		select SourceDistributionToReturnFromClientWaybillId, pd.sum
		from DealPaymentDocumentDistribution pd
		join DealPaymentDocumentDistributionToSaleWaybill dpddpd on dpddpd.id = pd.id
		where SourceDistributionToReturnFromClientWaybillId is not null
	) T
	group by SourceDistributionToReturnFromClientWaybillId
) TT on TT.SourceDistributionToReturnFromClientWaybillId = pdr.id
where -pd.sum < TT.sum


-- Сумма разнесения больше суммы по документу
select *
from DealPaymentDocument
where DistributedSum > Sum

-- Сумма разнесения не равна сумме по документа, но признак полной оплаты выставлен
select *
from DealPaymentDocument
where DistributedSum <> Sum and IsFullyDistributed = 1

-- Сумма разнесения равна сумме по документа, но признак полной оплаты не выставлен
select *
from DealPaymentDocument
where DistributedSum = Sum and IsFullyDistributed = 0

-- Накладная реализации с учетом возвратов полностью оплачена, но данный признак у нее не выставлен
select s.id, e.SalePriceSum, isnull(r.sum, 0), isnull(rp.sum, 0), p.sum
from SaleWaybill s
join ExpenditureWaybill e on s.id = e.id
left join 
(
	select SaleWaybillId, sum = isnull(sum(round(ReturnCount * SalePrice, 2)),0)
	from ReturnFromClientWaybillRow rwr
	join SaleWaybillRow swr on swr.id = rwr.SaleWaybillRowId
	join ReturnFromClientWaybill rw on rw.id = rwr.ReturnFromClientWaybillId and rw.ReceiptDate is not null
	where rwr.DeletionDate is null and swr.DeletionDate is null
	group by SaleWaybillId
) r on r.SaleWaybillId = s.id
left join
(
	select SaleWaybillId, sum = isnull(sum(sum), 0)
	from DealPaymentDocumentDistribution dpdd
	join DealPaymentDocumentDistributionToSaleWaybill sw on sw.id = dpdd.id
	group by SaleWaybillId
) p on p.SaleWaybillId = s.id
left join
(
	select SaleWaybillId, sum = isnull(sum(-sum), 0)
	from DealPaymentDocumentDistribution dpdd
	join DealPaymentDocumentDistributionToReturnFromClientWaybill r on r.id = dpdd.id
	group by SaleWaybillId
	
) rp on rp.SaleWaybillId = s.id
where e.SalePriceSum - isnull(r.sum, 0) + isnull(rp.sum, 0) - p.sum = 0 and s.IsFullyPaid = 0
		and s.deletionDate is null

-- Накладная реализации с учетом возвратов оплачена не полностью, но данный признак у нее выставлен
select s.id, 
	[Сумма накладной реализации] = e.SalePriceSum, 
	[Сумма возвратов товаров] = isnull(r.sum, 0), 
	[Сумма возвратов оплат в результате возврата товаров] = isnull(rp.sum, 0), 
	[Сумма платежей на накладную реализации] = p.sum
from SaleWaybill s
join ExpenditureWaybill e on s.id = e.id
left join 
(
	select SaleWaybillId, sum = isnull(sum(round(ReturnCount * SalePrice, 2)),0)
	from ReturnFromClientWaybillRow rwr
	join SaleWaybillRow swr on swr.id = rwr.SaleWaybillRowId
	join ReturnFromClientWaybill rw on rw.id = rwr.ReturnFromClientWaybillId and rw.ReceiptDate is not null
	where rwr.DeletionDate is null and swr.DeletionDate is null
	group by SaleWaybillId
) r on r.SaleWaybillId = s.id
left join
(
	select SaleWaybillId, sum = isnull(sum(sum), 0)
	from DealPaymentDocumentDistribution dpdd
	join DealPaymentDocumentDistributionToSaleWaybill sw on sw.id = dpdd.id
	group by SaleWaybillId
) p on p.SaleWaybillId = s.id
left join
(
	select SaleWaybillId, sum = isnull(sum(-sum), 0)
	from DealPaymentDocumentDistribution dpdd
	join DealPaymentDocumentDistributionToReturnFromClientWaybill r on r.id = dpdd.id
	group by SaleWaybillId
	
) rp on rp.SaleWaybillId = s.id
where 
	e.SalePriceSum - isnull(r.sum, 0) + isnull(rp.sum, 0) - p.sum <> 0 and s.IsFullyPaid = 1	
	and s.deletionDate is null
	
-- Проверяем, что возвраты средств в оплату (по конктретной реализации)не превышают сумму разнесений с этой оплаты (на эту реализацию)
SELECT [Оплата]	= s.Payment
	,[Реализация] = s.SaleWaybillId
	,[Сумма разнесений] = s.SaleSum
	,[Сумма возвратов] = r.ReturnSum
FROM
	(SELECT 
		d.SourceDealPaymentDocumentId Payment,
		sd.SaleWaybillId,
		SUM(d.Sum) SaleSum
	FROM DealPaymentDocumentDistributionToSaleWaybill sd
	join DealPaymentDocumentDistribution d on d.Id=sd.Id
	GROUP BY sd.SaleWaybillId, d.SourceDealPaymentDocumentId) s
join
	(SELECT 
		d.SourceDealPaymentDocumentId Payment,
		rd.SaleWaybillId,
		SUM(d.Sum) ReturnSum
	FROM DealPaymentDocumentDistributionToReturnFromClientWaybill rd
	join DealPaymentDocumentDistribution d on d.Id=rd.Id
	GROUP BY rd.SaleWaybillId,d.SourceDealPaymentDocumentId) r
	on s.SaleWaybillId = r.SaleWaybillId and s.Payment = r.Payment
WHERE
	s.SaleSum + r.ReturnSum < 0
	
	
-- Проверяем, что оплата не разнесена на непроведенную накладную реализации
SELECT DISTINCT [Код оплаты] = dpd.Id, [Код реализации] = ew.Id
FROM DealPaymentDocumentDistributionToSaleWaybill d
join ExpenditureWaybill ew on ew.Id = d.SaleWaybillId
join DealPaymentDocumentDistribution dpdd on dpdd.Id = d.Id
join DealPaymentDocument dpd on dpd.Id = dpdd.SourceDealPaymentDocumentId
WHERE ew.AcceptanceDate is null

-- Разнесения с нулевой суммой
SELECT Count(*)
--SELECT [Код оплаты] = dpd.Id, [Код разнесения] = dpdd.Id
FROM DealPaymentDocumentDistribution dpdd
join DealPaymentDocument dpd on dpd.Id = dpdd.SourceDealPaymentDocumentId
WHERE dpdd.Sum = 0



	