SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRY
	BEGIN TRAN
			
-- ������� ������������ ����������
delete AccountingPriceListWaybillTaking
delete ExactArticleRevaluationIndicator
delete AcceptedArticleRevaluationIndicator

update AccountingPriceList
set IsRevaluationOnStartCalculated = 0, IsRevaluationOnEndCalculated = 0

update ArticleAccountingPrice
set IsOverlappedOnEnd = 0
where IsOverlappedOnEnd = 1

/*
���� � ���� �������, �������� �� ����������
1 - ���������� �� � ��������
2 - ����������� �������� ��
3 - ������� �������� ���������
4 - ������������ ����������� ����� ������� �������� ���������
5 - ������� ��������� ��������� � ��������� ������
*/

declare @Id uniqueidentifier, @Date datetime, @EventType int, @WaybillType int, @StorageId smallint, @AccountOrganizationId int, @RowNumber int

DECLARE curMain CURSOR FAST_FORWARD FOR
	
	select Id, Date, Type, WaybillType, StorageId, AccountOrganizationId, RowNumber = ROW_NUMBER() OVER (order by Date)
	from
	(
		select Id, Date = StartDate, Type = 1, WaybillType = 0, StorageId = 0, AccountOrganizationId = 0
		from AccountingPriceList
		where StartDate <= GETDATE() and AcceptanceDate is not null and DeletionDate is null

		union
		select Id, EndDate, 2, 0, 0, 0
		from AccountingPriceList
		where EndDate <= GETDATE() and AcceptanceDate is not null and DeletionDate is null

		union
		select Id, ReceiptDate, 3, 1, ReceiptWaybillReceiptStorageId, AccountOrganizationId
		from ReceiptWaybill
		where ReceiptDate is not null and DeletionDate is null

		union
		select Id, ReceiptDate, 3, 2, RecipientStorageId, RecipientId
		from MovementWaybill
		where ReceiptDate is not null and DeletionDate is null

		union
		select Id, ChangeOwnerDate, 3, 5, ChangeOwnerWaybillStorageId, ChangeOwnerWaybillRecipientId
		from ChangeOwnerWaybill
		where ChangeOwnerDate is not null and DeletionDate is null

		union
		select Id, ReceiptDate, 3, 6, ReturnFromClientWaybillRecipientStorageId, ReturnFromClientWaybillRecipientId
		from ReturnFromClientWaybill
		where ReceiptDate is not null and DeletionDate is null

		union
		select Id, ApprovementDate, 4, 1, ReceiptWaybillReceiptStorageId, AccountOrganizationId
		from ReceiptWaybill
		where ApprovementDate is not null and ReceiptDate <> ApprovementDate and DeletionDate is null

		union
		select Id, ReceiptDate, 5, 2, SenderStorageId, SenderId
		from MovementWaybill
		where ReceiptDate is not null and DeletionDate is null

		union
		select Id, ChangeOwnerDate, 5, 5, ChangeOwnerWaybillStorageId, ChangeOwnerWaybillSenderId
		from ChangeOwnerWaybill
		where ChangeOwnerDate is not null and DeletionDate is null

		union
		select EW.Id, ShippingDate, 5, 4, ExpenditureWaybillSenderStorageId, Con.AccountOrganizationId
		from ExpenditureWaybill EW
		join SaleWaybill SW on SW.Id = EW.id and SW.DeletionDate is null
		join Deal D on D.Id = SW.DealId		
		join Contract Con on Con.Id = D.ClientContractId and Con.DeletionDate is null
		where ShippingDate is not null

		union
		select Id, WriteoffDate, 5, 3, WriteoffWaybillSenderStorageId, WriteoffWaybillSenderId
		from WriteoffWaybill
		where WriteoffDate is not null and DeletionDate is null
	) T
	--where Date <= '2012-03-01 23:59:59.000'
	order by Date
	
OPEN curMain
FETCH NEXT FROM curMain INTO @Id, @Date, @EventType, @WaybillType, @StorageId, @AccountOrganizationId, @RowNumber
WHILE @@FETCH_STATUS = 0
BEGIN
	
	if @EventType = 1
	begin
		print '�� �������'
		exec AccountingPriceListCameIntoEffect @AccountingPriceListId = @Id
	end
	
	if @EventType = 2
	begin
		print '�� �������� ��������'
		exec AccountingPriceListTerminated @AccountingPriceListId = @Id
	end
	
	if @EventType = 3
	begin
		print '�������� ��������� �������'		
		exec IncomingWaybillReceipted @Id, @WaybillType, @StorageId, @AccountOrganizationId, @Date
	end		 
	
	if @EventType = 4
	begin
		print '����������� �����������'		
		exec IncomingWaybillApproved @Id, @StorageId, @AccountOrganizationId, @Date				 
	end
				 
	if @EventType = 5
	begin
		print '��������� ��������� ���������'		
		exec OutgoingWaybillFinalized @Id, @WaybillType, @StorageId, @AccountOrganizationId, @Date
	end			 
				 
	if @RowNumber % 1000 = 0
	begin
		exec sp_updatestats
	end
				 
	FETCH NEXT FROM curMain INTO @Id, @Date, @EventType, @WaybillType, @StorageId, @AccountOrganizationId, @RowNumber
END
CLOSE curMain
DEALLOCATE curMain


	COMMIT TRAN
END TRY
BEGIN CATCH
	IF XACT_STATE() <> 0 ROLLBACK TRAN
	
	RETURN
END CATCH

go
drop function GetFinallyMovedCount
drop procedure GetReceiptedWaybillRows
drop procedure GetAcceptedAndNotReceiptedWaybillRows
drop procedure GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows
drop procedure GetAcceptedAndNotFinalizedWaybillRows
drop function GetAccountingPrice
drop function GetTakingAccountingPriceVariation
drop procedure UpdateExactRevaluationIndicators
drop procedure UpdateAcceptedRevaluationIndicators
drop procedure AccountingPriceListCameIntoEffect
drop procedure AccountingPriceListTerminated
drop procedure IncomingWaybillReceipted
drop procedure IncomingWaybillApproved
drop procedure OutgoingWaybillFinalized