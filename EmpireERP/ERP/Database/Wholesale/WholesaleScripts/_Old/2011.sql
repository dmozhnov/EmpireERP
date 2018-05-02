BEGIN TRY
	BEGIN TRAN		
	
	alter table dbo.AccountingPriceList drop constraint DF_IsUsedByAccountingPriceChanging
	
	if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ReturnFromClientWaybill_Client]') AND 
	parent_object_id = OBJECT_ID('[ReturnFromClientWaybill]'))
	alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_Client
	
	alter table [ReturnFromClientWaybill] drop column ReturnFromClientWaybillClientId
	
	alter table [ReturnFromClientWaybill] add ReturnFromClientWaybillDealId INT not null
	
	alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_Deal 
        foreign key (ReturnFromClientWaybillDealId) 
        references dbo.[Deal]
	
	alter table dbo.[ReturnFromClientIndicator] add DealId INT not null
	
	alter table [ReturnFromClientWaybill] add ReturnFromClientWaybillCreatedById INT not null constraint _tmp default 1
	
	alter table dbo.[ReturnFromClientWaybill]  drop constraint _tmp
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
