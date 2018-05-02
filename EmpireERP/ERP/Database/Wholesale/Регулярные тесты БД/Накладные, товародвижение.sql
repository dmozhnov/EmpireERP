----------------------------------------------------------------------------------------------------
-- »з позиции зарезервировано товара больше, чем в ней есть
select *
from ReceiptWaybillRow
where DeletionDate is null and 
	PendingCount < AcceptedCount + ShippedCount + FinallyMovedCount and
	ApprovedCount < AcceptedCount + ShippedCount + FinallyMovedCount

select *
from MovementWaybillRow
where DeletionDate is null and MovingCount < AcceptedCount + ShippedCount + FinallyMovedCount

select *
from ChangeOwnerWaybillRow
where DeletionDate is null and MovingCount < AcceptedCount + ShippedCount + FinallyMovedCount

select *
from ReturnFromClientWaybillRow
where DeletionDate is null and ReturnCount < AcceptedCount + ShippedCount + FinallyMovedCount

----------------------------------------------------------------------------------------------------
-- Ќеверное доступное дл€ резервировани€ кол-во товара
select *
from ReceiptWaybillRow r
join ReceiptWaybill w on w.Id = r.ReceiptWaybillId and w.DeletionDate is null and w.AcceptanceDate is not null
where r.DeletionDate is null and 
	AvailableToReserveCount <> PendingCount - (AcceptedCount + ShippedCount + FinallyMovedCount) and
	AvailableToReserveCount <> ApprovedCount - (AcceptedCount + ShippedCount + FinallyMovedCount)

select *
from MovementWaybillRow r
join MovementWaybill w on w.id = r.MovementWaybillId and w.DeletionDate is null and w.AcceptanceDate is not null
where r.DeletionDate is null and 
	AvailableToReserveCount <> MovingCount - (AcceptedCount + ShippedCount + FinallyMovedCount)

select *
from ChangeOwnerWaybillRow r
join ChangeOwnerWaybill w on w.id = r.ChangeOwnerWaybillId and w.DeletionDate is null and w.AcceptanceDate is not null
where r.DeletionDate is null and 
	AvailableToReserveCount <> MovingCount - (AcceptedCount + ShippedCount + FinallyMovedCount)

select *
from ReturnFromClientWaybillRow rwr
join ReturnFromClientWaybill rw on rw.id = rwr.ReturnFromClientWaybillId and rw.DeletionDate is null and rw.AcceptanceDate is not null
where rwr.DeletionDate is null and 
	AvailableToReserveCount <> ReturnCount - (AcceptedCount + ShippedCount + FinallyMovedCount)

----------------------------------------------------------------------------------------------------
-- ƒве позиции в накладной по одному товару (дл€ прихода), по одной партии (дл€ перемещени€, смены собственника, списани€, реализации) и по одной позиции накладной реализации (дл€ возвратов)

select ReceiptWaybillId
from ReceiptWaybillRow
where DeletionDate is null 
group by ReceiptWaybillId, ArticleId
having COUNT(*) > 1

select MovementWaybillId
from MovementWaybillRow
where DeletionDate is null 
group by MovementWaybillId, ReceiptWaybillRowId
having COUNT(*) > 1

select ChangeOwnerWaybillId
from ChangeOwnerWaybillRow
where DeletionDate is null 
group by ChangeOwnerWaybillId, ChangeOwnerWaybillRowReceiptWaybillRowId
having COUNT(*) > 1

select WriteoffWaybillId
from WriteoffWaybillRow
where DeletionDate is null 
group by WriteoffWaybillId, WriteoffReceiptWaybillRowId
having COUNT(*) > 1

select SaleWaybillId
from SaleWaybillRow swr
join ExpenditureWaybillRow ewr on ewr.Id = swr.Id
where DeletionDate is null 
group by SaleWaybillId, ExpenditureWaybillRowReceiptWaybillRowId
having COUNT(*) > 1

select ReturnFromClientWaybillId
from ReturnFromClientWaybillRow
where DeletionDate is null 
group by ReturnFromClientWaybillId, SaleWaybillRowId
having COUNT(*) > 1

--------------------------------------------------------------------------------------------------
-- ѕроведенные и выше накладные, которые имеют позиции со статусом, отличным от 0

select *
from ExpenditureWaybill ew
join SaleWaybill sw on sw.Id = ew.Id
where DeletionDate is null and ExpenditureWaybillStateId in (1, 8) and exists (

	select *
	from ExpenditureWaybillRow er
	join SaleWaybillRow swr on swr.Id = er.Id
	where DeletionDate is null and swr.SaleWaybillId = sw.Id and OutgoingWaybillRowStateId <> 0
)


select *
from MovementWaybill mw
where DeletionDate is null and MovementWaybillStateId in (1, 12) and exists (

	select *
	from MovementWaybillRow mr
	where DeletionDate is null and mr.MovementWaybillId = mw.Id and OutgoingWaybillRowStateId <> 0
)

select *
from ChangeOwnerWaybill mw
where DeletionDate is null and ChangeOwnerWaybillStateId in (1, 9) and exists (

	select *
	from ChangeOwnerWaybillRow mr
	where DeletionDate is null and mr.ChangeOwnerWaybillId = mw.Id and OutgoingWaybillRowStateId <> 0
)

select *
from WriteoffWaybill mw
where DeletionDate is null and WriteoffWaybillStateId in (1, 9) and exists (

	select *
	from WriteoffWaybillRow mr
	where DeletionDate is null and mr.WriteoffWaybillId = mw.Id and WriteoffOutgoingWaybillRowStateId <> 0
)

--------------------------------------------------------------------------------------------------
-- Ќепроведенные накладные, которые имеют позиции со статусом, равным 0
select *
from ExpenditureWaybill ew
join SaleWaybill sw on sw.Id = ew.Id
where DeletionDate is null and ExpenditureWaybillStateId not in (1, 8) and exists (

	select *
	from ExpenditureWaybillRow er
	join SaleWaybillRow swr on swr.Id = er.Id
	where DeletionDate is null and swr.SaleWaybillId = sw.Id and OutgoingWaybillRowStateId = 0
)

select *
from MovementWaybill mw
where DeletionDate is null and MovementWaybillStateId not in (1, 12) and exists (

	select *
	from MovementWaybillRow mr
	where DeletionDate is null and mr.MovementWaybillId = mw.Id and OutgoingWaybillRowStateId = 0
)

select *
from ChangeOwnerWaybill mw
where DeletionDate is null and ChangeOwnerWaybillStateId not in (1, 9) and exists (

	select *
	from ChangeOwnerWaybillRow mr
	where DeletionDate is null and mr.ChangeOwnerWaybillId = mw.Id and OutgoingWaybillRowStateId = 0
)

select *
from WriteoffWaybill mw
where DeletionDate is null and WriteoffWaybillStateId not in (1, 9) and exists (

	select *
	from WriteoffWaybillRow mr
	where DeletionDate is null and mr.WriteoffWaybillId = mw.Id and WriteoffOutgoingWaybillRowStateId = 0
)

--------------------------------------------------------------------------------------------------
-- ƒл€ позиции накладной IsUsingManualSource <> IsManuallyCreated

select *
from MovementWaybillRow er
join WaybillRowArticleMovement w on w.DestinationWaybillRowId = er.Id
where IsUsingManualSource <> IsManuallyCreated

select *
from ChangeOwnerWaybillRow er
join WaybillRowArticleMovement w on w.DestinationWaybillRowId = er.Id
where IsUsingManualSource <> IsManuallyCreated

select *
from ExpenditureWaybillRow er
join WaybillRowArticleMovement w on w.DestinationWaybillRowId = er.Id
where IsUsingManualSource <> IsManuallyCreated

select *
from WriteoffWaybillRow er
join WaybillRowArticleMovement w on w.DestinationWaybillRowId = er.Id
where IsUsingManualSource <> IsManuallyCreated

--------------------------------------------------------------------------------------------------
-- UsageAsManualSourceCount не совпадает с кол-вом св€зей с приемниками

select r.id, r.UsageAsManualSourceCount, count(w.Id)
from ReceiptWaybillRow r
join WaybillRowArticleMovement w on w.SourceWaybillRowId = r.Id and IsManuallyCreated = 1
group by r.id, r.UsageAsManualSourceCount
having r.UsageAsManualSourceCount <> count(w.Id)

select r.id, r.UsageAsManualSourceCount, count(w.Id)
from MovementWaybillRow r
join WaybillRowArticleMovement w on w.SourceWaybillRowId = r.Id and IsManuallyCreated = 1
group by r.id, r.UsageAsManualSourceCount
having r.UsageAsManualSourceCount <> count(w.Id)

select r.id, r.UsageAsManualSourceCount, count(w.Id)
from ChangeOwnerWaybillRow r
join WaybillRowArticleMovement w on w.SourceWaybillRowId = r.Id and IsManuallyCreated = 1
group by r.id, r.UsageAsManualSourceCount
having r.UsageAsManualSourceCount <> count(w.Id)

select r.id, r.UsageAsManualSourceCount, count(w.Id)
from ReturnFromClientWaybillRow r
join WaybillRowArticleMovement w on w.SourceWaybillRowId = r.Id and IsManuallyCreated = 1
group by r.id, r.UsageAsManualSourceCount
having r.UsageAsManualSourceCount <> count(w.Id)

--------------------------------------------------------------------------------------------------
-- св€зь операций над накладными с показател€ми 

-- приходы
select *
from ReceiptWaybill w
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from IncomingAcceptedArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from ReceiptWaybill w
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from AcceptedPurchaseIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from ReceiptWaybill w
join ReceiptWaybillRow r on r.ReceiptWaybillId = w.Id and r.DeletionDate is null and r.AreCountDivergencesAfterReceipt = 0 and r.AreSumDivergencesAfterReceipt = 0
where w.DeletionDate is null and ReceiptDate is not null and not exists
(
	select *
	from ExactArticleAvailabilityIndicator
	where StartDate = w.ReceiptDate 
)

select *
from ReceiptWaybill w
join ReceiptWaybillRow r on r.ReceiptWaybillId = w.Id and r.DeletionDate is null and r.AreCountDivergencesAfterReceipt = 0 and r.AreSumDivergencesAfterReceipt = 0
where w.DeletionDate is null and ReceiptDate is not null and not exists
(
	select *
	from ApprovedPurchaseIndicator
	where StartDate = w.ReceiptDate 
)

select *
from ReceiptWaybill w
join ReceiptWaybillRow r on r.ReceiptWaybillId = w.Id and r.DeletionDate is null and (r.AreCountDivergencesAfterReceipt = 1 or r.AreSumDivergencesAfterReceipt = 1)
where w.DeletionDate is null and ApprovementDate is not null and not exists
(
	select *
	from ExactArticleAvailabilityIndicator
	where StartDate = w.ApprovementDate 
)

select *
from ReceiptWaybill w
join ReceiptWaybillRow r on r.ReceiptWaybillId = w.Id and r.DeletionDate is null and (r.AreCountDivergencesAfterReceipt = 1 or r.AreSumDivergencesAfterReceipt = 1)
where w.DeletionDate is null and ApprovementDate is not null and not exists
(
	select *
	from ApprovedPurchaseIndicator
	where StartDate = w.ApprovementDate 
)

-- перемещени€
select *
from MovementWaybill w
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from IncomingAcceptedArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from MovementWaybill w
where DeletionDate is null and ReceiptDate is not null and not exists
(
	select *
	from ExactArticleAvailabilityIndicator
	where StartDate = w.ReceiptDate 
)

-- смены собственника
select *
from ChangeOwnerWaybill w
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from IncomingAcceptedArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from ChangeOwnerWaybill w
where DeletionDate is null and ChangeOwnerDate is not null and not exists
(
	select *
	from ExactArticleAvailabilityIndicator
	where StartDate = w.ChangeOwnerDate 
)

-- списани€
select *
from WriteoffWaybill w
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)
and not exists
(
	select *
	from OutgoingAcceptedFromExactArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from WriteoffWaybill w
where DeletionDate is null and WriteOffDate is not null and not exists
(
	select *
	from ExactArticleAvailabilityIndicator
	where StartDate = w.WriteOffDate 
)

-- реализации
select *
from ExpenditureWaybill w
join SaleWaybill s on s.Id = w.Id
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)
and not exists
(
	select *
	from OutgoingAcceptedFromExactArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from ExpenditureWaybill w
join SaleWaybill s on s.Id = w.Id
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from AcceptedSaleIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from ExpenditureWaybill w
join SaleWaybill s on s.Id = w.Id
where DeletionDate is null and ShippingDate is not null and not exists
(
	select *
	from ExactArticleAvailabilityIndicator
	where StartDate = w.ShippingDate 
)

select *
from ExpenditureWaybill w
join SaleWaybill s on s.Id = w.Id
where DeletionDate is null and ShippingDate is not null and not exists
(
	select *
	from ShippedSaleIndicator
	where StartDate = w.ShippingDate 
)

-- возвраты от клиента
select *
from ReturnFromClientWaybill w
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from IncomingAcceptedArticleAvailabilityIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from ReturnFromClientWaybill w
where DeletionDate is null and AcceptanceDate is not null and not exists
(
	select *
	from AcceptedReturnFromClientIndicator
	where StartDate = w.AcceptanceDate 
)

select *
from ReturnFromClientWaybill w
where w.DeletionDate is null and ReceiptDate is not null and not exists
(
	select *
	from ExactArticleAvailabilityIndicator
	where StartDate = w.ReceiptDate 
)

select *
from ReturnFromClientWaybill w
where DeletionDate is null and ReceiptDate is not null and not exists
(
	select *
	from ReceiptedReturnFromClientIndicator
	where StartDate = w.ReceiptDate 
)

--------------------------------------------------------------------------------------------------
-- Ќакладна€ проведена, а параметры проводки не проставлены или наоборот

select *
from SaleWaybill sw
join SaleWaybillRow sr on sr.SaleWaybillId = sw.Id and sr.DeletionDate is null
join ExpenditureWaybill ew on ew.id = sw.id
join ExpenditureWaybillRow er on er.id = sr.id
where (ExpenditureWaybillStateId in (1, 8) and (SaleWaybillAcceptedById is not null or AcceptanceDate is not null or SalePrice is not null or 
	ExpenditureWaybillSenderArticleAccountingPriceId is not null or SalePriceSum <> 0 or SenderAccountingPriceSum <> 0))
	or
	(ExpenditureWaybillStateId not in (1, 8) and (SaleWaybillAcceptedById is null or AcceptanceDate is null or SalePrice is null or 
	ExpenditureWaybillSenderArticleAccountingPriceId is null))


--------------------------------------------------------------------------------------------------
-- Ѕольше одной св€зи между источником и приемником
select DestinationWaybillRowId,	SourceWaybillRowId
from WaybillRowArticleMovement
group by DestinationWaybillRowId,	SourceWaybillRowId
having count(*) > 1

--------------------------------------------------------------------------------------------------
-- Ќе хватает св€зей между источником и приемником

-- исход€щие

-- реализации
select sr.Id, SellingCount
from SaleWaybill sw
join SaleWaybillRow sr on sr.SaleWaybillId = sw.Id and sr.DeletionDate is null
join ExpenditureWaybill ew on ew.id = sw.id
join ExpenditureWaybillRow er on er.id = sr.id
where AcceptanceDate is not null and SellingCount <> isnull(
	(
		select sum(MovingCount)
		from WaybillRowArticleMovement
		where DestinationWaybillRowId = sr.Id
	),0)
	
-- перемещени€
select r.Id, MovingCount
from MovementWaybill w
join MovementWaybillRow r on r.MovementWaybillId = w.Id and r.DeletionDate is null
where AcceptanceDate is not null and MovingCount <> isnull(
	(
		select sum(MovingCount)
		from WaybillRowArticleMovement
		where DestinationWaybillRowId = r.Id
	),0)
	
-- смены собственника
select r.Id, MovingCount
from ChangeOwnerWaybill w
join ChangeOwnerWaybillRow r on r.ChangeOwnerWaybillId = w.Id and r.DeletionDate is null
where AcceptanceDate is not null and MovingCount <> isnull(
	(
		select sum(MovingCount)
		from WaybillRowArticleMovement
		where DestinationWaybillRowId = r.Id
	),0)
	
-- списани€
select r.Id, WritingoffCount
from WriteoffWaybill w
join WriteoffWaybillRow r on r.WriteoffWaybillId = w.Id and r.DeletionDate is null
where AcceptanceDate is not null and WritingoffCount <> isnull(
	(
		select sum(MovingCount)
		from WaybillRowArticleMovement
		where DestinationWaybillRowId = r.Id
	),0)
	
--------------------------------------------------------------------------------------------------
-- Ќакладна€ реализации не прин€та, но доступное дл€ возврата кол-во не равно 0 

select *
from SaleWaybill sw
join SaleWaybillRow sr on sr.SaleWaybillId = sw.Id and sr.DeletionDate is null
join ExpenditureWaybill ew on ew.id = sw.id
join ExpenditureWaybillRow er on er.id = sr.id
where sw.DeletionDate is null and ew.ShippingDate is null and sr.AvailableToReturnCount <> 0

--------------------------------------------------------------------------------------------------
-- Ќакладна€ реализации не прин€та, но по ее позици€м имеютс€ возвраты
select *
from SaleWaybill sw
join SaleWaybillRow sr on sr.SaleWaybillId = sw.Id and sr.DeletionDate is null
join ExpenditureWaybill ew on ew.id = sw.id
join ExpenditureWaybillRow er on er.id = sr.id
join ReturnFromClientWaybillRow rr on rr.SaleWaybillRowId = sr.Id and rr.DeletionDate is null
where sw.DeletionDate is null and ew.ShippingDate is null