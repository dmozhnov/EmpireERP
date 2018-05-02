-- ����� �����, ���������� �����

-- ����� ���������� ������, ��� DistributedSum � ���������� ���������
select dpd.id, dpd.DistributedSum, isnull(sum(dpdd.sum),0)
from DealPaymentDocument dpd
left join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.id
group by dpd.id, dpd.DistributedSum
having dpd.DistributedSum < isnull(sum(dpdd.sum),0)

-- ����� ���������� �� ����� DistributedSum � ���������� ���������
select dpd.id, dpd.DistributedSum, sum(dpdd.sum)
from DealPaymentDocument dpd
join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.id
group by dpd.id, dpd.DistributedSum
having dpd.DistributedSum <> sum(dpdd.sum)

-- ����������� � ���������� �������� ����� ������ ����� ��������
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


-- ����� ���������� ������ ����� �� ���������
select *
from DealPaymentDocument
where DistributedSum > Sum

-- ����� ���������� �� ����� ����� �� ���������, �� ������� ������ ������ ���������
select *
from DealPaymentDocument
where DistributedSum <> Sum and IsFullyDistributed = 1

-- ����� ���������� ����� ����� �� ���������, �� ������� ������ ������ �� ���������
select *
from DealPaymentDocument
where DistributedSum = Sum and IsFullyDistributed = 0

-- ��������� ���������� � ������ ��������� ��������� ��������, �� ������ ������� � ��� �� ���������
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

-- ��������� ���������� � ������ ��������� �������� �� ���������, �� ������ ������� � ��� ���������
select s.id, 
	[����� ��������� ����������] = e.SalePriceSum, 
	[����� ��������� �������] = isnull(r.sum, 0), 
	[����� ��������� ����� � ���������� �������� �������] = isnull(rp.sum, 0), 
	[����� �������� �� ��������� ����������] = p.sum
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
	
-- ���������, ��� �������� ������� � ������ (�� ����������� ����������)�� ��������� ����� ���������� � ���� ������ (�� ��� ����������)
SELECT [������]	= s.Payment
	,[����������] = s.SaleWaybillId
	,[����� ����������] = s.SaleSum
	,[����� ���������] = r.ReturnSum
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
	
	
-- ���������, ��� ������ �� ��������� �� ������������� ��������� ����������
SELECT DISTINCT [��� ������] = dpd.Id, [��� ����������] = ew.Id
FROM DealPaymentDocumentDistributionToSaleWaybill d
join ExpenditureWaybill ew on ew.Id = d.SaleWaybillId
join DealPaymentDocumentDistribution dpdd on dpdd.Id = d.Id
join DealPaymentDocument dpd on dpd.Id = dpdd.SourceDealPaymentDocumentId
WHERE ew.AcceptanceDate is null

-- ���������� � ������� ������
SELECT Count(*)
--SELECT [��� ������] = dpd.Id, [��� ����������] = dpdd.Id
FROM DealPaymentDocumentDistribution dpdd
join DealPaymentDocument dpd on dpd.Id = dpdd.SourceDealPaymentDocumentId
WHERE dpdd.Sum = 0



	