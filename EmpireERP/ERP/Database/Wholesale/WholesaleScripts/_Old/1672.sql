BEGIN TRY
	BEGIN TRAN		
	
	if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK63944DB72B40A237]') AND parent_object_id = OBJECT_ID('TeamProductionOrder'))
		alter table dbo.TeamProductionOrder  drop constraint FK63944DB72B40A237


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[PFK_ProductionOrder_Team]') AND parent_object_id = OBJECT_ID('TeamProductionOrder'))
		alter table dbo.TeamProductionOrder  drop constraint PFK_ProductionOrder_Team

	if exists (select * from dbo.sysobjects where id = object_id(N'dbo.TeamProductionOrder') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.TeamProductionOrder

	create table dbo.TeamProductionOrder (
			TeamId SMALLINT not null,
		   ProductionOrderId UNIQUEIDENTIFIER not null,
		   primary key (TeamId, ProductionOrderId)
	)
	    
	alter table dbo.TeamProductionOrder 
		add constraint FK63944DB72B40A237 
		foreign key (ProductionOrderId) 
		references dbo.[ProductionOrder]
        
	alter table dbo.TeamProductionOrder 
		add constraint PFK_ProductionOrder_Team 
		foreign key (TeamId) 
		references dbo.[Team]
	
	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH
