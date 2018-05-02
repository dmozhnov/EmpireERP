SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRY
	BEGIN TRAN
			
		-- сохраняем текущие данные
		select *
		into AccountingPriceListWaybillTaking_old
		from AccountingPriceListWaybillTaking

		select *
		into ExactArticleRevaluationIndicator_Old
		from ExactArticleRevaluationIndicator

		select *
		into AcceptedArticleRevaluationIndicator_Old
		from AcceptedArticleRevaluationIndicator

		select *
		into AccountingPriceList_Old
		from AccountingPriceList

		-- удаляем существующие показатели
		delete AccountingPriceListWaybillTaking
		delete ExactArticleRevaluationIndicator
		delete AcceptedArticleRevaluationIndicator

		update AccountingPriceList
		set IsRevaluationOnStartCalculated = 0, IsRevaluationOnEndCalculated = 0

		update ArticleAccountingPrice
		set IsOverlappedOnEnd = 0
		where IsOverlappedOnEnd = 1

		/*
		типы и коды событий, влияющих на переоценку
		1 - вступление РЦ в действие
		2 - прекращение действия РЦ
		3 - приемка входящей накладной
		4 - согласование расхождений после приемки входящей накладной
		5 - перевод исходящей накладной в финальный статус
		*/

		declare @Id uniqueidentifier, @Date datetime, @EventType int, @WaybillType int, @StorageId smallint, @AccountOrganizationId int

		DECLARE curMain CURSOR FAST_FORWARD FOR
			
			select Id, Date, Type, WaybillType, StorageId, AccountOrganizationId
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
		FETCH NEXT FROM curMain INTO @Id, @Date, @EventType, @WaybillType, @StorageId, @AccountOrganizationId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			if @EventType = 1
			begin
				print 'РЦ вступил'
				exec AccountingPriceListCameIntoEffect @AccountingPriceListId = @Id
			end
			
			if @EventType = 2
			begin
				print 'РЦ завершил действие'
				exec AccountingPriceListTerminated @AccountingPriceListId = @Id
			end
			
			if @EventType = 3
			begin
				print 'Входящая накладная принята'		
				exec IncomingWaybillReceipted @Id, @WaybillType, @StorageId, @AccountOrganizationId, @Date
			end		 
			
			if @EventType = 4
			begin
				print 'Расхождения согласованы'		
				exec IncomingWaybillApproved @Id, @StorageId, @AccountOrganizationId, @Date				 
			end
						 
			if @EventType = 5
			begin
				print 'Исходящая накладная отгружена'		
				exec OutgoingWaybillFinalized @Id, @WaybillType, @StorageId, @AccountOrganizationId, @Date
			end			 
						 
			FETCH NEXT FROM curMain INTO @Id, @Date, @EventType, @WaybillType, @StorageId, @AccountOrganizationId
		END
		CLOSE curMain
		DEALLOCATE curMain

		-- проверки
		select *
		from AccountingPriceListWaybillTaking A
		full join AccountingPriceListWaybillTaking_Old AA on
			A.TakingDate = AA.TakingDate and
			A.ArticleAccountingPriceId = AA.ArticleAccountingPriceId and
			A.WaybillTypeId	 = AA.WaybillTypeId and
			A.WaybillRowId	 = AA.WaybillRowId and
			A.ArticleId	 = AA.ArticleId and
			A.StorageId	 = AA.StorageId and
			A.AccountOrganizationId	 = AA.AccountOrganizationId and
			A.AccountingPrice	 = AA.AccountingPrice and
			A.IsOnAccountingPriceListStart	 = AA.IsOnAccountingPriceListStart and	
			A.Count	 = AA.Count and
			(A.RevaluationDate = AA.RevaluationDate or (A.RevaluationDate is null and AA.RevaluationDate is null)) and
			A.IsWaybillRowIncoming	  = AA.IsWaybillRowIncoming
		where A.Id is null or AA.Id is null
		order by A.TakingDate, A.ArticleAccountingPriceId, A.WaybillRowId, A.ArticleId, A.StorageId, A.AccountOrganizationId,
		AA.TakingDate, AA.ArticleAccountingPriceId, AA.WaybillRowId, AA.ArticleId, AA.StorageId, AA.AccountOrganizationId

		select *
		from AcceptedArticleRevaluationIndicator E
		full join AcceptedArticleRevaluationIndicator_Old EE on
			E.StartDate	= EE.StartDate	and
			(E.EndDate = EE.EndDate or (E.EndDate is null and EE.EndDate is null)) and
			E.StorageId	 = EE.StorageId and
			E.AccountOrganizationId	 = EE.AccountOrganizationId and
			E.RevaluationSum	 = EE.RevaluationSum
		where E.id is null or EE.id is null
		order by E.StorageId, E.AccountOrganizationId, E.StartDate, EE.StorageId, EE.AccountOrganizationId, EE.StartDate

		select *
		from ExactArticleRevaluationIndicator E
		full join ExactArticleRevaluationIndicator_Old EE on
			E.StartDate	= EE.StartDate	and
			(E.EndDate = EE.EndDate or (E.EndDate is null and EE.EndDate is null)) and
			E.StorageId	 = EE.StorageId and
			E.AccountOrganizationId	 = EE.AccountOrganizationId and
			E.RevaluationSum	 = EE.RevaluationSum
		where E.id is null or EE.id is null
		order by E.StorageId, E.AccountOrganizationId, E.StartDate, EE.StorageId, EE.AccountOrganizationId, EE.StartDate

		/*
		select *
		from AcceptedArticleRevaluationIndicator
		order by StorageId, AccountOrganizationId, StartDate

		select *
		from ExactArticleRevaluationIndicator
		order by StorageId, AccountOrganizationId, StartDate

		select *
		from AccountingPriceListWaybillTaking
		*/

		-- восстановление прежних данных
		delete AccountingPriceListWaybillTaking
		delete ExactArticleRevaluationIndicator
		delete AcceptedArticleRevaluationIndicator

		insert into AccountingPriceListWaybillTaking
		select *
		from AccountingPriceListWaybillTaking_Old

		insert into ExactArticleRevaluationIndicator
		select *
		from ExactArticleRevaluationIndicator_Old

		insert into AcceptedArticleRevaluationIndicator
		select *
		from AcceptedArticleRevaluationIndicator_Old

		update A
		set IsRevaluationOnStartCalculated = AA.IsRevaluationOnStartCalculated, IsRevaluationOnEndCalculated = AA.IsRevaluationOnEndCalculated
		from AccountingPriceList A
		join AccountingPriceList_Old AA on AA.Id = A.Id

		drop table AccountingPriceListWaybillTaking_Old
		drop table ExactArticleRevaluationIndicator_Old
		drop table AcceptedArticleRevaluationIndicator_Old
		drop table AccountingPriceList_Old

	COMMIT TRAN
END TRY
BEGIN CATCH
	IF XACT_STATE() <> 0 ROLLBACK TRAN
	RETURN
END CATCH