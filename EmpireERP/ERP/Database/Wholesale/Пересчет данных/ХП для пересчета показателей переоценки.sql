go

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('GetFinallyMovedCount') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT')) drop function GetFinallyMovedCount
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('GetAccountingPrice') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT')) drop function GetAccountingPrice
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('GetTakingAccountingPriceVariation') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT')) drop function GetTakingAccountingPriceVariation

if exists (select * from sys.procedures where Name = 'GetReceiptedWaybillRows') drop procedure GetReceiptedWaybillRows
if exists (select * from sys.procedures where Name = 'GetAcceptedAndNotReceiptedWaybillRows') drop procedure GetAcceptedAndNotReceiptedWaybillRows
if exists (select * from sys.procedures where Name = 'GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows') drop procedure GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows
if exists (select * from sys.procedures where Name = 'GetAcceptedAndNotFinalizedWaybillRows') drop procedure GetAcceptedAndNotFinalizedWaybillRows
if exists (select * from sys.procedures where Name = 'UpdateExactRevaluationIndicators') drop procedure UpdateExactRevaluationIndicators
if exists (select * from sys.procedures where Name = 'UpdateAcceptedRevaluationIndicators') drop procedure UpdateAcceptedRevaluationIndicators
if exists (select * from sys.procedures where Name = 'AccountingPriceListCameIntoEffect') drop procedure AccountingPriceListCameIntoEffect
if exists (select * from sys.procedures where Name = 'AccountingPriceListTerminated') drop procedure AccountingPriceListTerminated
if exists (select * from sys.procedures where Name = 'IncomingWaybillReceipted') drop procedure IncomingWaybillReceipted
if exists (select * from sys.procedures where Name = 'IncomingWaybillApproved') drop procedure IncomingWaybillApproved
if exists (select * from sys.procedures where Name = 'OutgoingWaybillFinalized') drop procedure OutgoingWaybillFinalized


go
create function GetFinallyMovedCount(@SourceWaybillRowId uniqueidentifier, @Date datetime)
	RETURNS decimal(18, 6)
AS
begin

declare @Cnt decimal(18, 6)

select @Cnt = isnull(SUM(Cnt), 0)
from
	(
	select Cnt = M.MovingCount
	from WaybillRowArticleMovement M
	join MovementWaybillRow WR on WR.Id = M.DestinationWaybillRowId and WR.DeletionDate is null
	join MovementWaybill W on W.Id = WR.MovementWaybillId and W.DeletionDate is null
	where M.SourceWaybillRowId = @SourceWaybillRowId and W.ReceiptDate < @Date

	union all
	select M.MovingCount
	from WaybillRowArticleMovement M
	join ChangeOwnerWaybillRow WR on WR.Id = M.DestinationWaybillRowId and WR.DeletionDate is null
	join ChangeOwnerWaybill W on W.Id = WR.ChangeOwnerWaybillId and W.DeletionDate is null
	where M.SourceWaybillRowId = @SourceWaybillRowId and W.ChangeOwnerDate < @Date

	union all
	select M.MovingCount
	from WaybillRowArticleMovement M
	join WriteoffWaybillRow WR on WR.Id = M.DestinationWaybillRowId and WR.DeletionDate is null
	join WriteoffWaybill W on W.Id = WR.WriteoffWaybillId and W.DeletionDate is null
	where M.SourceWaybillRowId = @SourceWaybillRowId and W.WriteoffDate < @Date

	union all
	select M.MovingCount
	from WaybillRowArticleMovement M
	join ExpenditureWaybillRow WR on WR.Id = M.DestinationWaybillRowId
	join SaleWaybillRow S on S.Id = WR.Id and S.DeletionDate is null
	join ExpenditureWaybill W on W.Id = S.SaleWaybillId
	join SaleWaybill SW on SW.Id = W.Id and SW.DeletionDate is null
	where M.SourceWaybillRowId = @SourceWaybillRowId and W.ShippingDate < @Date
) T

return @Cnt

end

go

create procedure GetReceiptedWaybillRows
	@AccountingPriceListId uniqueidentifier,
	@Date datetime
as	

select StorageId
into #Storages
from AccountingPriceListStorage
where AccountingPriceListId = @AccountingPriceListId

select ArticleId
into #Articles
from ArticleAccountingPrice
where AccountingPriceListId = @AccountingPriceListId and DeletionDate is null

-- приходы
select RWR.Id, WaybillTypeId = 1, RWR.ArticleId, RecipientStorageId = RW.ReceiptWaybillReceiptStorageId, 
	RecipientId = RW.AccountOrganizationId, 
	[Count] = ISNULL(RWR.ApprovedCount, 0) - dbo.GetFinallyMovedCount(RWR.Id, @Date)
from ReceiptWaybill RW
join ReceiptWaybillRow RWR on RWR.ReceiptWaybillId = RW.Id 
	and RWR.AreCountDivergencesAfterReceipt = 0 and RWR.AreSumDivergencesAfterReceipt = 0
	and ISNULL(RWR.ApprovedCount, 0) > dbo.GetFinallyMovedCount(RWR.Id, @Date) and RWR.DeletionDate is null
join #Articles A on A.ArticleId = RWR.ArticleId
join #Storages S on S.StorageId = RW.ReceiptWaybillReceiptStorageId
where RW.ReceiptDate < @Date and (RW.ApprovementDate > @Date or RW.ApprovementDate is null) and RW.DeletionDate is null

union all
select RWR.Id, WaybillTypeId = 1, RWR.ArticleId, RecipientStorageId = RW.ReceiptWaybillReceiptStorageId, 
	RecipientId = RW.AccountOrganizationId, 
	[Count] = ISNULL(RWR.ApprovedCount, 0) - dbo.GetFinallyMovedCount(RWR.Id, @Date)
from ReceiptWaybill RW
join ReceiptWaybillRow RWR on RWR.ReceiptWaybillId = RW.Id  and RWR.DeletionDate is null
	and ISNULL(RWR.ApprovedCount, 0) > dbo.GetFinallyMovedCount(RWR.Id, @Date)
join #Articles A on A.ArticleId = RWR.ArticleId
join #Storages S on S.StorageId = RW.ReceiptWaybillReceiptStorageId
where RW.ApprovementDate < @Date and RW.DeletionDate is null

-- перемещения
union all
select MWR.Id, WaybillTypeId = 2, MWR.ArticleId, RecipientStorageId = MW.RecipientStorageId, 
	RecipientId = MW.RecipientId, 
	[Count] = MWR.MovingCount - dbo.GetFinallyMovedCount(MWR.Id, @Date)
from MovementWaybill MW
join MovementWaybillRow MWR on MWR.MovementWaybillId = MW.Id  and MWR.DeletionDate is null
	and MWR.MovingCount > dbo.GetFinallyMovedCount(MWR.Id, @Date)
join #Articles A on A.ArticleId = MWR.ArticleId
join #Storages S on S.StorageId = MW.RecipientStorageId
where MW.ReceiptDate < @Date and MW.DeletionDate is null

-- смены собственника
union all
select COWR.Id, WaybillTypeId = 5, COWR.ArticleId, RecipientStorageId = COW.ChangeOwnerWaybillStorageId, 
	RecipientId = COW.ChangeOwnerWaybillRecipientId, 
	[Count] = COWR.MovingCount - dbo.GetFinallyMovedCount(COWR.Id, @Date)
from ChangeOwnerWaybill COW
join ChangeOwnerWaybillRow COWR on COWR.ChangeOwnerWaybillId = COW.Id  and COWR.DeletionDate is null
	and COWR.MovingCount > dbo.GetFinallyMovedCount(COWR.Id, @Date)
join #Articles A on A.ArticleId = COWR.ArticleId
join #Storages S on S.StorageId = COW.ChangeOwnerWaybillStorageId
where COW.ChangeOwnerDate < @Date and COW.DeletionDate is null

-- возвраты от клиента
union all
select RFCWR.Id, WaybillTypeId = 6, RFCWR.ArticleId, RecipientStorageId = RFCW.ReturnFromClientWaybillRecipientStorageId, 
	RecipientId = RFCW.ReturnFromClientWaybillRecipientId, 
	[Count] = RFCWR.ReturnCount - dbo.GetFinallyMovedCount(RFCWR.Id, @Date)
from ReturnFromClientWaybill RFCW
join ReturnFromClientWaybillRow RFCWR on RFCWR.ReturnFromClientWaybillId = RFCW.Id  and RFCWR.DeletionDate is null
	and RFCWR.ReturnCount > dbo.GetFinallyMovedCount(RFCWR.Id, @Date)
join #Articles A on A.ArticleId = RFCWR.ArticleId
join #Storages S on S.StorageId = RFCW.ReturnFromClientWaybillRecipientStorageId
where RFCW.ReceiptDate < @Date and RFCW.DeletionDate is null

drop table #Storages
drop table #Articles
	
go

create procedure GetAcceptedAndNotReceiptedWaybillRows
	@AccountingPriceListId uniqueidentifier,
	@Date datetime
as	

select StorageId
into #Storages
from AccountingPriceListStorage
where AccountingPriceListId = @AccountingPriceListId

select ArticleId
into #Articles
from ArticleAccountingPrice
where AccountingPriceListId = @AccountingPriceListId and DeletionDate is null

-- приходы
select RWR.Id, WaybillTypeId = 1, RWR.ArticleId, RecipientStorageId = RW.ReceiptWaybillReceiptStorageId, 
	RecipientId = RW.AccountOrganizationId, [Count] = RWR.PendingCount
from ReceiptWaybill RW
join ReceiptWaybillRow RWR on RWR.ReceiptWaybillId = RW.Id and RWR.DeletionDate is null and RWR.PendingCount > 0 -- отсекаем добавленные при приемке позиции
join #Articles A on A.ArticleId = RWR.ArticleId
join #Storages S on S.StorageId = RW.ReceiptWaybillReceiptStorageId
where RW.AcceptanceDate < @Date and (RW.ReceiptDate > @Date or RW.ReceiptDate is null) and RW.DeletionDate is null

-- перемещения
union all
select MWR.Id, WaybillTypeId = 2, MWR.ArticleId, RecipientStorageId = MW.RecipientStorageId, 
	RecipientId = MW.RecipientId, [Count] = MWR.MovingCount
from MovementWaybill MW
join MovementWaybillRow MWR on MWR.MovementWaybillId = MW.Id  and MWR.DeletionDate is null
join #Articles A on A.ArticleId = MWR.ArticleId
join #Storages S on S.StorageId = MW.RecipientStorageId
where MW.AcceptanceDate < @Date and (MW.ReceiptDate > @Date or MW.ReceiptDate is null) and MW.DeletionDate is null

-- смены собственника
union all
select COWR.Id, WaybillTypeId = 5, COWR.ArticleId, RecipientStorageId = COW.ChangeOwnerWaybillStorageId, 
	RecipientId = COW.ChangeOwnerWaybillRecipientId, [Count] = COWR.MovingCount
from ChangeOwnerWaybill COW
join ChangeOwnerWaybillRow COWR on COWR.ChangeOwnerWaybillId = COW.Id  and COWR.DeletionDate is null
join #Articles A on A.ArticleId = COWR.ArticleId
join #Storages S on S.StorageId = COW.ChangeOwnerWaybillStorageId
where COW.AcceptanceDate < @Date and (COW.ChangeOwnerDate > @Date or COW.ChangeOwnerDate is null) and COW.DeletionDate is null

-- возвраты от клиента
union all
select RFCWR.Id, WaybillTypeId = 6, RFCWR.ArticleId, RecipientStorageId = RFCW.ReturnFromClientWaybillRecipientStorageId, 
	RecipientId = RFCW.ReturnFromClientWaybillRecipientId, [Count] = RFCWR.ReturnCount
from ReturnFromClientWaybill RFCW
join ReturnFromClientWaybillRow RFCWR on RFCWR.ReturnFromClientWaybillId = RFCW.Id  and RFCWR.DeletionDate is null
join #Articles A on A.ArticleId = RFCWR.ArticleId
join #Storages S on S.StorageId = RFCW.ReturnFromClientWaybillRecipientStorageId
where RFCW.AcceptanceDate < @Date and (RFCW.ReceiptDate > @Date or RFCW.ReceiptDate is null) and RFCW.DeletionDate is null

drop table #Storages
drop table #Articles

go

create procedure GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows
	@AccountingPriceListId uniqueidentifier,
	@Date datetime
as	

select StorageId
into #Storages
from AccountingPriceListStorage
where AccountingPriceListId = @AccountingPriceListId

select ArticleId
into #Articles
from ArticleAccountingPrice
where AccountingPriceListId = @AccountingPriceListId and DeletionDate is null

-- приходы
select RWR.Id, WaybillTypeId = 1, RWR.ArticleId, RecipientStorageId = RW.ReceiptWaybillReceiptStorageId, 
	RecipientId = RW.AccountOrganizationId, 
	[COUNT] = 0
from ReceiptWaybill RW
join ReceiptWaybillRow RWR on RWR.ReceiptWaybillId = RW.Id  and RWR.DeletionDate is null
	and (RWR.AreCountDivergencesAfterReceipt = 1 or RWR.AreSumDivergencesAfterReceipt = 1) and RWR.PendingCount > 0
join #Articles A on A.ArticleId = RWR.ArticleId
join #Storages S on S.StorageId = RW.ReceiptWaybillReceiptStorageId
where RW.ReceiptDate < @Date and (RW.ApprovementDate > @Date or RW.ApprovementDate is null) and RW.DeletionDate is null

drop table #Storages
drop table #Articles

go

create procedure GetAcceptedAndNotFinalizedWaybillRows
	@AccountingPriceListId uniqueidentifier,
	@Date datetime
as	

select StorageId
into #Storages
from AccountingPriceListStorage
where AccountingPriceListId = @AccountingPriceListId

select ArticleId
into #Articles
from ArticleAccountingPrice
where AccountingPriceListId = @AccountingPriceListId and DeletionDate is null

-- перемещения
select MWR.Id, WaybillTypeId = 2, MWR.ArticleId, SenderStorageId = MW.SenderStorageId, 
	SenderId = MW.SenderId, [Count] = MWR.MovingCount
from MovementWaybill MW
join MovementWaybillRow MWR on MWR.MovementWaybillId = MW.Id  and MWR.DeletionDate is null
join #Articles A on A.ArticleId = MWR.ArticleId
join #Storages S on S.StorageId = MW.SenderStorageId
where MW.AcceptanceDate < @Date and (MW.ReceiptDate > @Date or MW.ReceiptDate is null) and MW.DeletionDate is null

-- смены собственника
union all
select COWR.Id, WaybillTypeId = 5, COWR.ArticleId, SenderStorageId = COW.ChangeOwnerWaybillStorageId, 
	SenderId = COW.ChangeOwnerWaybillSenderId, [Count] = COWR.MovingCount
from ChangeOwnerWaybill COW
join ChangeOwnerWaybillRow COWR on COWR.ChangeOwnerWaybillId = COW.Id  and COWR.DeletionDate is null
join #Articles A on A.ArticleId = COWR.ArticleId
join #Storages S on S.StorageId = COW.ChangeOwnerWaybillStorageId
where COW.AcceptanceDate < @Date and (COW.ChangeOwnerDate > @Date or COW.ChangeOwnerDate is null) and COW.DeletionDate is null

-- реализации
union all
select SWR.Id, WaybillTypeId = 4, SWR.ArticleId, SenderStorageId = EW.ExpenditureWaybillSenderStorageId, 
	SenderId = Con.AccountOrganizationId, [Count] = SWR.SellingCount
from ExpenditureWaybill EW
join SaleWaybill SW on SW.Id = EW.id and SW.DeletionDate is null
join Deal D on D.Id = SW.DealId 
join Contract Con on Con.Id = D.ClientContractId and Con.DeletionDate is null
join SaleWaybillRow SWR on SWR.SaleWaybillId = SW.Id  and SWR.DeletionDate is null
join #Articles A on A.ArticleId = SWR.ArticleId
join #Storages S on S.StorageId = EW.ExpenditureWaybillSenderStorageId
where EW.AcceptanceDate < @Date and (EW.ShippingDate > @Date or EW.ShippingDate is null)

-- списание
union all
select WWR.Id, WaybillTypeId = 3, WWR.ArticleId, SenderStorageId = WW.WriteoffWaybillSenderStorageId, 
	SenderId = WW.WriteoffWaybillSenderId, [Count] = WWR.WritingoffCount
from WriteoffWaybill WW
join WriteoffWaybillRow WWR on WWR.WriteoffWaybillId = WW.Id  and WWR.DeletionDate is null
join #Articles A on A.ArticleId = WWR.ArticleId
join #Storages S on S.StorageId = WW.WriteoffWaybillSenderStorageId
where WW.AcceptanceDate < @Date and (WW.WriteoffDate > @Date or WW.WriteoffDate is null) and WW.DeletionDate is null

drop table #Storages
drop table #Articles

go

CREATE FUNCTION GetAccountingPrice(@StorageId smallint, @ArticleId int, @Date datetime)
	RETURNS decimal(18, 2)
AS
begin

declare @AccountingPrice numeric(18,2)

select top 1 @AccountingPrice = AccountingPrice
from ArticleAccountingPriceIndicator
where StorageId = @StorageId and ArticleId = @ArticleId and StartDate <= @Date and (EndDate > @Date or EndDate is null)
order by StartDate desc

RETURN isnull(@AccountingPrice,0)

end

go

CREATE FUNCTION GetTakingAccountingPriceVariation(@TakingId uniqueidentifier)
	RETURNS decimal(18, 2)
AS
begin

declare @PrevAccountingPrice numeric(18,2), @NewAccountingPrice numeric(18,2)
declare @StorageId smallint, @ArticleId int, @IsOnAccountingPriceListStart bit, @AccountingPrice numeric(18,2), @TakingDate datetime

select @StorageId = StorageId, @ArticleId = ArticleId, @IsOnAccountingPriceListStart = IsOnAccountingPriceListStart, 
	@AccountingPrice = AccountingPrice, @TakingDate = TakingDate
from AccountingPriceListWaybillTaking
where Id = @TakingId

if(@IsOnAccountingPriceListStart = 1)
begin	
	set @PrevAccountingPrice = dbo.GetAccountingPrice(@StorageId, @ArticleId, Dateadd(SECOND, -1, @TakingDate))
	set @NewAccountingPrice = @AccountingPrice
end
else
begin
	set @NewAccountingPrice = dbo.GetAccountingPrice(@StorageId, @ArticleId, Dateadd(SECOND, 1, @TakingDate))
	set @PrevAccountingPrice = @AccountingPrice
end

return @NewAccountingPrice - @PrevAccountingPrice

end

go

create procedure UpdateExactRevaluationIndicators
	@StartDate datetime,
	@StorageId smallint,
	@AccountOrganizationID int, 
	@Sum numeric(18,2)
as	

select *
into #T
from ExactArticleRevaluationIndicator
where StorageId = @StorageId and AccountOrganizationId = @AccountOrganizationID and (EndDate > @StartDate or EndDate is null) 

if not exists (select * from #T)
begin
	INSERT INTO [ExactArticleRevaluationIndicator]([Id],[StartDate],[EndDate],[StorageId],[AccountOrganizationId],[RevaluationSum],[PreviousId])
	select NEWID(), @StartDate, null, @StorageId, @AccountOrganizationID, @Sum, null
end
else
begin

	if exists (select * from #T where StartDate = @StartDate)
	begin
		
		update [ExactArticleRevaluationIndicator]
		set [RevaluationSum] = [RevaluationSum] + @Sum
		from [ExactArticleRevaluationIndicator]
		where StorageId = @StorageId and AccountOrganizationId = @AccountOrganizationID and StartDate = @StartDate		
	end
	else
	begin
		declare @curId uniqueidentifier
		select top 1 @curId = Id from #T order by StartDate
		
		update [ExactArticleRevaluationIndicator]
		set EndDate = @StartDate
		where Id = @curId
		
		INSERT INTO [ExactArticleRevaluationIndicator]([Id],[StartDate],[EndDate],[StorageId],[AccountOrganizationId],[RevaluationSum],[PreviousId])
		select NEWID(), @StartDate, null, @StorageId, @AccountOrganizationID, 
		(select RevaluationSum + @Sum from #T where Id = @curId), @curId
		
	end
end

drop table #T

go

create procedure UpdateAcceptedRevaluationIndicators
	@StartDate datetime,
	@StorageId smallint,
	@AccountOrganizationID int, 
	@Sum numeric(18,2)
as	

select *
into #T
from AcceptedArticleRevaluationIndicator
where StorageId = @StorageId and AccountOrganizationId = @AccountOrganizationID and (EndDate > @StartDate or EndDate is null) 

if not exists (select * from #T)
begin
	INSERT INTO [AcceptedArticleRevaluationIndicator]([Id],[StartDate],[EndDate],[StorageId],[AccountOrganizationId],[RevaluationSum],[PreviousId])
	select NEWID(), @StartDate, null, @StorageId, @AccountOrganizationID, @Sum, null
end
else
begin

	if exists (select * from #T where StartDate = @StartDate)
	begin
		
		update [AcceptedArticleRevaluationIndicator]
		set [RevaluationSum] = [RevaluationSum] + @Sum
		from [AcceptedArticleRevaluationIndicator]
		where StorageId = @StorageId and AccountOrganizationId = @AccountOrganizationID and StartDate = @StartDate		
	end
	else
	begin
		declare @curId uniqueidentifier
		select top 1 @curId = Id from #T order by StartDate
		
		update [AcceptedArticleRevaluationIndicator]
		set EndDate = @StartDate
		where Id = @curId
		
		INSERT INTO [AcceptedArticleRevaluationIndicator]([Id],[StartDate],[EndDate],[StorageId],[AccountOrganizationId],[RevaluationSum],[PreviousId])
		select NEWID(), @StartDate, null, @StorageId, @AccountOrganizationID, 
		(select RevaluationSum + @Sum from #T where Id = @curId), @curId
		
	end
end

drop table #T

go

create procedure AccountingPriceListCameIntoEffect
	@AccountingPriceListId uniqueidentifier
as

declare @StartDate datetime
set @StartDate = (select StartDate from AccountingPriceList where id = @AccountingPriceListId)

create table #ReceiptedWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, RecipientStorageId smallint, RecipientId int, 
	[Count] numeric(18, 6)
)
create table #AcceptedAndNotReceiptedWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, RecipientStorageId smallint, RecipientId int, 
	[Count] numeric(18, 6)
)
create table #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, RecipientStorageId smallint, RecipientId int, 
	[Count] numeric(18, 6)
)
create table #AcceptedAndNotFinalizedWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, SenderStorageId smallint, SenderId int, 
	[Count] numeric(18, 6)
)

-- получение списка позиций накладных для построения связей
insert #ReceiptedWaybillRows
exec GetReceiptedWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @StartDate

insert #AcceptedAndNotReceiptedWaybillRows
exec GetAcceptedAndNotReceiptedWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @StartDate

insert #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows
exec GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @StartDate

insert #AcceptedAndNotFinalizedWaybillRows
exec GetAcceptedAndNotFinalizedWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @StartDate

-- формирование связей
INSERT INTO [AccountingPriceListWaybillTaking]([Id],[TakingDate],[IsWaybillRowIncoming],[ArticleAccountingPriceId],[WaybillTypeId],
	[WaybillRowId],	[ArticleId],[StorageId],[AccountOrganizationId],[AccountingPrice],[IsOnAccountingPriceListStart],[Count],[RevaluationDate])
select NEWID(), @StartDate, 1, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.RecipientStorageId, R.RecipientId, AAP.AccountingPrice, 1, 
	R.[Count], @StartDate
from ArticleAccountingPrice AAP
join #ReceiptedWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId  and AAP.DeletionDate is null

union all
select NEWID(), @StartDate, 1, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.RecipientStorageId, R.RecipientId, AAP.AccountingPrice, 1, 
	R.[Count], NULL
from ArticleAccountingPrice AAP
join #AcceptedAndNotReceiptedWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId  and AAP.DeletionDate is null

union all
select NEWID(), @StartDate, 1, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.RecipientStorageId, R.RecipientId, AAP.AccountingPrice, 1, 
	0, NULL
from ArticleAccountingPrice AAP
join #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId  and AAP.DeletionDate is null

union all
select NEWID(), @StartDate, 0, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.SenderStorageId, R.SenderId, AAP.AccountingPrice, 1, 
	R.[Count], NULL
from ArticleAccountingPrice AAP
join #AcceptedAndNotFinalizedWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId  and AAP.DeletionDate is null


declare @StorageId smallint, @AccountOrganizationId int, @Sum numeric(18,2)

-- изменение показателей точной переоценки
DECLARE curExact CURSOR FAST_FORWARD FOR

	select RecipientStorageId, RecipientId, cast(SUM([COUNT]) as numeric(18,2))
	from 
	(
		select ArticleId, RecipientStorageId, RecipientId,
			[COUNT] = SUM([COUNT]) * 
			(isnull((select AccountingPrice from ArticleAccountingPrice where AccountingPriceListId = @AccountingPriceListId and ArticleId = R.ArticleId and DeletionDate is null),0) -
			isnull(dbo.GetAccountingPrice(RecipientStorageId, ArticleId, Dateadd(second, -1, @StartDate)), 0))
		from #ReceiptedWaybillRows R
		group by ArticleId, RecipientStorageId, RecipientId
	) TT
	group by RecipientStorageId, RecipientId
		
OPEN curExact
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
WHILE @@FETCH_STATUS = 0
BEGIN
			
	if(@Sum <> 0)
		exec UpdateExactRevaluationIndicators @StartDate, @StorageId, @AccountOrganizationID, @Sum
				 
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
END
CLOSE curExact
DEALLOCATE curExact


-- изменение показателей проведенной переоценки
DECLARE curExact CURSOR FAST_FORWARD FOR

	select RecipientStorageId, RecipientId, cast(SUM([COUNT]) as numeric(18,2))
	from 
	(
		select ArticleId, RecipientStorageId, RecipientId,
			[COUNT] = SUM([COUNT]) * 
			(isnull((select AccountingPrice from ArticleAccountingPrice where AccountingPriceListId = @AccountingPriceListId and ArticleId = T.ArticleId and DeletionDate is null),0) -
			isnull(dbo.GetAccountingPrice(RecipientStorageId, ArticleId, Dateadd(second, -1, @StartDate)),0))
		from 
		(
			select *
			from #ReceiptedWaybillRows

			union all
			select *
			from #AcceptedAndNotReceiptedWaybillRows

			union all
			select Id, WaybillTypeId, ArticleId, SenderStorageId, SenderId, -[Count]
			from #AcceptedAndNotFinalizedWaybillRows
		) T
		group by ArticleId, RecipientStorageId, RecipientId
	) TT
	group by RecipientStorageId, RecipientId
		
OPEN curExact
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
WHILE @@FETCH_STATUS = 0
BEGIN
			
	if(@Sum <> 0)
		exec UpdateAcceptedRevaluationIndicators @StartDate, @StorageId, @AccountOrganizationID, @Sum
				 
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
END
CLOSE curExact
DEALLOCATE curExact

update AccountingPriceList
set IsRevaluationOnStartCalculated = 1
where Id = @AccountingPriceListId

drop table #ReceiptedWaybillRows
drop table #AcceptedAndNotReceiptedWaybillRows
drop table #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows
drop table #AcceptedAndNotFinalizedWaybillRows

go

create procedure AccountingPriceListTerminated
	@AccountingPriceListId uniqueidentifier
as

declare @EndDate datetime, @StartDate datetime
set @StartDate = (select StartDate from AccountingPriceList where id = @AccountingPriceListId)
set @EndDate = (select EndDate from AccountingPriceList where id = @AccountingPriceListId)

select StorageId
into #Storages
from AccountingPriceListStorage
where AccountingPriceListId = @AccountingPriceListId

select ArticleId
into #Articles
from ArticleAccountingPrice
where AccountingPriceListId = @AccountingPriceListId and DeletionDate is null

create table #ReceiptedWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, RecipientStorageId smallint, RecipientId int, 
	[Count] numeric(18, 6)
)
create table #AcceptedAndNotReceiptedWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, RecipientStorageId smallint, RecipientId int, 
	[Count] numeric(18, 6)
)
create table #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, RecipientStorageId smallint, RecipientId int, 
	[Count] numeric(18, 6)
)
create table #AcceptedAndNotFinalizedWaybillRows (
	Id uniqueidentifier, WaybillTypeId int, ArticleId int, SenderStorageId smallint, SenderId int, 
	[Count] numeric(18, 6)
)


-- получение списка позиций накладных для построения связей
insert #ReceiptedWaybillRows
exec GetReceiptedWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @EndDate

insert #AcceptedAndNotReceiptedWaybillRows
exec GetAcceptedAndNotReceiptedWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @EndDate

insert #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows
exec GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @EndDate

insert #AcceptedAndNotFinalizedWaybillRows
exec GetAcceptedAndNotFinalizedWaybillRows @AccountingPriceListId = @AccountingPriceListId, @Date = @EndDate

-- перекрывающие РЦ
select APLS.StorageId, AAP.ArticleId
into #OverlappingPriceLists
from AccountingPriceList APL
join ArticleAccountingPrice AAP on AAP.AccountingPriceListId = APL.Id and AAP.DeletionDate is null
join AccountingPriceListStorage APLS on APLS.AccountingPriceListId = APL.Id
join #Storages S on S.StorageId = APLS.StorageId
join #Articles A on A.ArticleId = AAP.ArticleId
where APL.StartDate < @EndDate and APL.StartDate > @StartDate and (APL.EndDate > @EndDate or APL.EndDate is null) and APL.AcceptanceDate is not null and APL.DeletionDate is null

-- помечаем перекрытые позиции РЦ
update AAP
set IsOverlappedOnEnd = 1
from ArticleAccountingPrice AAP
join #OverlappingPriceLists O on O.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId and AAP.DeletionDate is null

-- удаляем позиции накладных, связанные с перекрытыми позициями РЦ 
delete R
from #ReceiptedWaybillRows R
join #OverlappingPriceLists O on O.ArticleId = R.ArticleId and O.StorageId = R.RecipientStorageId

delete R
from #AcceptedAndNotReceiptedWaybillRows R
join #OverlappingPriceLists O on O.ArticleId = R.ArticleId and O.StorageId = R.RecipientStorageId

delete R
from #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows R
join #OverlappingPriceLists O on O.ArticleId = R.ArticleId and O.StorageId = R.RecipientStorageId

delete R
from #AcceptedAndNotFinalizedWaybillRows R
join #OverlappingPriceLists O on O.ArticleId = R.ArticleId and O.StorageId = R.SenderStorageId


-- формирование связей
INSERT INTO [AccountingPriceListWaybillTaking]([Id],[TakingDate],[IsWaybillRowIncoming],[ArticleAccountingPriceId],[WaybillTypeId],
	[WaybillRowId],	[ArticleId],[StorageId],[AccountOrganizationId],[AccountingPrice],[IsOnAccountingPriceListStart],[Count],[RevaluationDate])
select NEWID(), @EndDate, 1, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.RecipientStorageId, R.RecipientId, AAP.AccountingPrice, 0, 
	R.[Count], @EndDate
from ArticleAccountingPrice AAP
join #ReceiptedWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId  and AAP.DeletionDate is null

union all
select NEWID(), @EndDate, 1, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.RecipientStorageId, R.RecipientId, AAP.AccountingPrice, 0, 
	R.[Count], NULL
from ArticleAccountingPrice AAP
join #AcceptedAndNotReceiptedWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId   and AAP.DeletionDate is null

union all
select NEWID(), @EndDate, 1, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.RecipientStorageId, R.RecipientId, AAP.AccountingPrice, 0, 
	0, NULL
from ArticleAccountingPrice AAP
join #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId   and AAP.DeletionDate is null

union all
select NEWID(), @EndDate, 0, AAP.Id, R.WaybillTypeId, R.Id, R.ArticleId, R.SenderStorageId, R.SenderId, AAP.AccountingPrice, 0, 
	R.[Count], NULL
from ArticleAccountingPrice AAP
join #AcceptedAndNotFinalizedWaybillRows R on R.ArticleId = AAP.ArticleId
where AAP.AccountingPriceListId = @AccountingPriceListId   and AAP.DeletionDate is null

declare @StorageId smallint, @AccountOrganizationId int, @Sum numeric(18,2)

-- изменение показателей точной переоценки
DECLARE curExact CURSOR FAST_FORWARD FOR

	select RecipientStorageId, RecipientId, cast(SUM([COUNT]) as numeric(18,2))
	from 
	(
		select ArticleId, RecipientStorageId, RecipientId,
			[COUNT] = SUM([COUNT]) * 
			(isnull(dbo.GetAccountingPrice(RecipientStorageId, ArticleId, Dateadd(second, 1, @EndDate)),0) -
			isnull((select AccountingPrice from ArticleAccountingPrice where AccountingPriceListId = @AccountingPriceListId and ArticleId = R.ArticleId and DeletionDate is null),0))
		from #ReceiptedWaybillRows R
		group by ArticleId, RecipientStorageId, RecipientId
	) TT
	group by RecipientStorageId, RecipientId
		
OPEN curExact
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
WHILE @@FETCH_STATUS = 0
BEGIN
	if(@Sum <> 0)	
		exec UpdateExactRevaluationIndicators @EndDate, @StorageId, @AccountOrganizationID, @Sum
				 
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
END
CLOSE curExact
DEALLOCATE curExact

-- изменение показателей проведенной переоценки
DECLARE curExact CURSOR FAST_FORWARD FOR

	select RecipientStorageId, RecipientId, cast(SUM([COUNT]) as numeric(18,2))
	from 
	(
		select ArticleId, RecipientStorageId, RecipientId,
			[COUNT] = SUM([COUNT]) * 
			(isnull(dbo.GetAccountingPrice(RecipientStorageId, ArticleId, Dateadd(second, 1, @EndDate)),0) -			
			isnull((select AccountingPrice from ArticleAccountingPrice where AccountingPriceListId = @AccountingPriceListId and ArticleId = T.ArticleId and DeletionDate is null),0))
		from 
		(
			select *
			from #ReceiptedWaybillRows

			union all
			select *
			from #AcceptedAndNotReceiptedWaybillRows

			union all
			select Id, WaybillTypeId, ArticleId, SenderStorageId, SenderId, -[Count]
			from #AcceptedAndNotFinalizedWaybillRows
		) T
		group by ArticleId, RecipientStorageId, RecipientId
	) TT
	group by RecipientStorageId, RecipientId
		
OPEN curExact
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
WHILE @@FETCH_STATUS = 0
BEGIN
	if(@Sum <> 0)
		exec UpdateAcceptedRevaluationIndicators @EndDate, @StorageId, @AccountOrganizationID, @Sum
				 
FETCH NEXT FROM curExact INTO @StorageId, @AccountOrganizationId, @Sum
END
CLOSE curExact
DEALLOCATE curExact

update AccountingPriceList
set IsRevaluationOnEndCalculated = 1
where Id = @AccountingPriceListId

drop table #Storages
drop table #Articles

drop table #ReceiptedWaybillRows
drop table #AcceptedAndNotReceiptedWaybillRows
drop table #ReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows
drop table #AcceptedAndNotFinalizedWaybillRows
drop table #OverlappingPriceLists

go


create procedure IncomingWaybillReceipted
	@WaybillId uniqueidentifier,
	@WaybillType int,
	@StorageId smallint,
	@AccountOrganizationId int,
	@Date datetime
as

create table #Takings
(
	Id uniqueidentifier, WaybillRowId uniqueidentifier, [Count] numeric(18,6), AccountingPrice numeric(18,2), ArticleId int, IsOnAccountingPriceListStart bit
)
create table #RowsWithoutDivergences ( Id uniqueidentifier )
create table #RowsWithDivergences ( Id uniqueidentifier )

-- приход
if(@WaybillType = 1)
begin
	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join ReceiptWaybillRow WR on WR.Id = T.WaybillRowId and WR.ReceiptWaybillId = @WaybillId and WR.DeletionDate is null
	join ReceiptWaybill W on W.Id = WR.ReceiptWaybillId and W.ReceiptWaybillReceiptStorageId = T.StorageId and 
		W.AccountOrganizationId = T.AccountOrganizationId and W.DeletionDate is null
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId

	insert into #RowsWithoutDivergences
	select Id	
	from ReceiptWaybillRow 
	where ReceiptWaybillId = @WaybillId and AreCountDivergencesAfterReceipt = 0 and AreSumDivergencesAfterReceipt = 0 and 
		PendingCount > 0 and DeletionDate is null
	
	insert into #RowsWithDivergences
	select Id	
	from ReceiptWaybillRow 
	where ReceiptWaybillId = @WaybillId and (AreCountDivergencesAfterReceipt = 1 or AreSumDivergencesAfterReceipt = 1) and 
		PendingCount > 0 and DeletionDate is null
end

-- перемещение
if(@WaybillType = 2)
begin
	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join MovementWaybillRow WR on WR.Id = T.WaybillRowId and WR.MovementWaybillId = @WaybillId and WR.DeletionDate is null
	join MovementWaybill W on W.Id = WR.MovementWaybillId and W.RecipientStorageId = T.StorageId and 
		W.RecipientId = T.AccountOrganizationId and W.DeletionDate is null
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId
	
	insert into #RowsWithoutDivergences
	select Id	
	from MovementWaybillRow 
	where MovementWaybillId = @WaybillId and DeletionDate is null
end

-- смена собственника
if(@WaybillType = 5)
begin
	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join ChangeOwnerWaybillRow WR on WR.Id = T.WaybillRowId and WR.ChangeOwnerWaybillId = @WaybillId and WR.DeletionDate is null
	join ChangeOwnerWaybill W on W.Id = WR.ChangeOwnerWaybillId and W.ChangeOwnerWaybillStorageId = T.StorageId and 
		W.ChangeOwnerWaybillRecipientId = T.AccountOrganizationId and W.DeletionDate is null
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId
	
	insert into #RowsWithoutDivergences
	select Id	
	from ChangeOwnerWaybillRow 
	where ChangeOwnerWaybillId = @WaybillId	 and DeletionDate is null
end

-- возвраты от клиента
if(@WaybillType = 6)
begin
	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join ReturnFromClientWaybillRow WR on WR.Id = T.WaybillRowId and WR.ReturnFromClientWaybillId = @WaybillId and WR.DeletionDate is null
	join ReturnFromClientWaybill W on W.Id = WR.ReturnFromClientWaybillId and W.ReturnFromClientWaybillRecipientStorageId  = T.StorageId and 
		W.ReturnFromClientWaybillRecipientId = T.AccountOrganizationId and W.DeletionDate is null
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId
	
	insert into #RowsWithoutDivergences
	select Id	
	from ReturnFromClientWaybillRow 
	where ReturnFromClientWaybillId = @WaybillId and DeletionDate is null
end


-- позиции без расхождений
update A
set RevaluationDate = @Date
from #RowsWithoutDivergences R
join #Takings T on T.WaybillRowId = R.Id
join AccountingPriceListWaybillTaking A on A.Id = T.Id

declare @ExactArticleRevaluationCorrection numeric(18,2)

select @ExactArticleRevaluationCorrection = ISNULL(SUM(Round(T.Count * dbo.GetTakingAccountingPriceVariation(T.Id), 2)), 0)
from #RowsWithoutDivergences R
join #Takings T on T.WaybillRowId = R.Id

if(@ExactArticleRevaluationCorrection <> 0)
	exec UpdateExactRevaluationIndicators @Date, @StorageId, @AccountOrganizationID, @ExactArticleRevaluationCorrection

-- позиции с расхождениями
update A
set Count = 0
from #RowsWithDivergences R
join #Takings T on T.WaybillRowId = R.Id
join AccountingPriceListWaybillTaking A on A.Id = T.Id

declare @AcceptedArticleRevaluationCorrection numeric(18,2)

select @AcceptedArticleRevaluationCorrection = ISNULL(SUM(Round(-T.Count * dbo.GetTakingAccountingPriceVariation(T.Id), 2)), 0)
from #RowsWithDivergences R
join #Takings T on T.WaybillRowId = R.Id

if(@AcceptedArticleRevaluationCorrection <> 0)
	exec UpdateAcceptedRevaluationIndicators @Date, @StorageId, @AccountOrganizationID, @AcceptedArticleRevaluationCorrection


drop table #Takings
drop table #RowsWithoutDivergences
drop table #RowsWithDivergences

go

create procedure IncomingWaybillApproved
	@WaybillId uniqueidentifier,	
	@StorageId smallint,
	@AccountOrganizationId int,
	@Date datetime
as

update T
set Count = WR.ApprovedCount, RevaluationDate = @Date
from AccountingPriceListWaybillTaking T
join ReceiptWaybillRow WR on WR.Id = T.WaybillRowId and WR.ReceiptWaybillId = @WaybillId and (AreCountDivergencesAfterReceipt = 1 or AreSumDivergencesAfterReceipt = 1)		and PendingCount > 0
join ReceiptWaybill W on W.Id = WR.ReceiptWaybillId and W.ReceiptWaybillReceiptStorageId = T.StorageId and W.AccountOrganizationId = T.AccountOrganizationId
where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId

select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
into #Takings
from AccountingPriceListWaybillTaking T
join ReceiptWaybillRow WR on WR.Id = T.WaybillRowId and WR.ReceiptWaybillId = @WaybillId and (AreCountDivergencesAfterReceipt = 1 or AreSumDivergencesAfterReceipt = 1)		and PendingCount > 0
join ReceiptWaybill W on W.Id = WR.ReceiptWaybillId and W.ReceiptWaybillReceiptStorageId = T.StorageId and W.AccountOrganizationId = T.AccountOrganizationId
where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId

declare @ArticleRevaluationCorrection numeric(18,2)

select @ArticleRevaluationCorrection = ISNULL(SUM(Round(T.Count * dbo.GetTakingAccountingPriceVariation(T.Id), 2)), 0)
from #Takings T

if(@ArticleRevaluationCorrection <> 0)
begin
	exec UpdateAcceptedRevaluationIndicators @Date, @StorageId, @AccountOrganizationID, @ArticleRevaluationCorrection
	exec UpdateExactRevaluationIndicators @Date, @StorageId, @AccountOrganizationID, @ArticleRevaluationCorrection

end

drop table #Takings

go

create procedure OutgoingWaybillFinalized
	@WaybillId uniqueidentifier,
	@WaybillType int,
	@StorageId smallint,
	@AccountOrganizationId int,
	@Date datetime
as

create table #Takings
(
	Id uniqueidentifier, WaybillRowId uniqueidentifier, [Count] numeric(18,6), AccountingPrice numeric(18,2), ArticleId int, IsOnAccountingPriceListStart bit
)

-- перемещение
if(@WaybillType = 2)
begin
	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join MovementWaybillRow WR on WR.Id = T.WaybillRowId and WR.MovementWaybillId = @WaybillId and WR.DeletionDate is null
	join MovementWaybill W on W.Id = WR.MovementWaybillId and W.SenderStorageId = T.StorageId and W.SenderId = T.AccountOrganizationId and W.DeletionDate is null	
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId
end

-- смена собственника
if(@WaybillType = 5)
begin
	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join ChangeOwnerWaybillRow WR on WR.Id = T.WaybillRowId and WR.ChangeOwnerWaybillId = @WaybillId and WR.DeletionDate is null
	join ChangeOwnerWaybill W on W.Id = WR.ChangeOwnerWaybillId and W.ChangeOwnerWaybillStorageId = T.StorageId and 
		W.ChangeOwnerWaybillSenderId = T.AccountOrganizationId and W.DeletionDate is null
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId
end

-- реализации
if(@WaybillType = 4)
begin

	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join SaleWaybillRow WR on WR.Id = T.WaybillRowId and WR.SaleWaybillId = @WaybillId and WR.DeletionDate is null
	join SaleWaybill SW on SW.Id = WR.SaleWaybillId and SW.DeletionDate is null
	join Deal D on D.Id = SW.DealId
	join Contract Con on Con.Id = D.ClientContractId and Con.DeletionDate is null
	join ExpenditureWaybill W on W.Id = WR.SaleWaybillId and W.ExpenditureWaybillSenderStorageId = T.StorageId and 
		Con.AccountOrganizationId = T.AccountOrganizationId
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId
end

-- списания
if(@WaybillType = 3)
begin
	insert into #Takings
	select T.Id, T.WaybillRowId, T.Count, T.AccountingPrice, T.ArticleId, T.IsOnAccountingPriceListStart	
	from AccountingPriceListWaybillTaking T
	join WriteoffWaybillRow WR on WR.Id = T.WaybillRowId and WR.WriteoffWaybillId = @WaybillId and WR.DeletionDate is null
	join WriteoffWaybill W on W.Id = WR.WriteoffWaybillId and W.WriteoffWaybillSenderStorageId = T.StorageId and 
		W.WriteoffWaybillSenderId = T.AccountOrganizationId and W.DeletionDate is null
	where T.StorageId = @StorageId and T.AccountOrganizationId = @AccountOrganizationId
end

update A
set RevaluationDate = @Date
from #Takings T
join AccountingPriceListWaybillTaking A on A.Id = T.Id

declare @ExactArticleRevaluationCorrection numeric(18,2)

select @ExactArticleRevaluationCorrection = ISNULL(SUM(-Round(T.Count * dbo.GetTakingAccountingPriceVariation(T.Id), 2)), 0)
from #Takings T

if(@ExactArticleRevaluationCorrection <> 0)
	exec UpdateExactRevaluationIndicators @Date, @StorageId, @AccountOrganizationID, @ExactArticleRevaluationCorrection

drop table #Takings

go

