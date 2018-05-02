create table dbo.#NewDealQuota (
       Name varchar(100),
       IsPrepayment bit,
       DiscountPercent decimal,
       PostPaymentDays smallint,
       CreditLimitSum decimal,
       Id int)


BEGIN TRY
	BEGIN TRAN		
		
	PRINT 'Обновление выполнено успешно'

insert into #NewDealQuota
select case when number <> 1 then name + cast(number as varchar(100)) else Name end as Name, IsPrepayment, DiscountPercent, PostPaymentDays, CreditLimitSum,
Id = ROW_NUMBER() over (order by name)
from
	(
	select name, IsPrepayment, DiscountPercent, PostPaymentDays, CreditLimitSum, 
	number = ROW_NUMBER() over (Partition by name order by name)
	from
		(
		select name, IsPrepayment, DiscountPercent, PostPaymentDays, CreditLimitSum
		from 
			(
			select name, IsPrepayment, DiscountPercent, PostPaymentDays, CreditLimitSum, 
			NUMBER = ROW_NUMBER() OVER (partition by IsPrepayment, DiscountPercent, PostPaymentDays, CreditLimitSum order by name, IsPrepayment, DiscountPercent, PostPaymentDays, CreditLimitSum)
			from DealQuota
			) T
		where NUMBER = 1
		) TT
	) TTT

	ALTER TABLE dbo.[DealQuota]
		DROP CONSTRAINT FK_Deal_DealQuota_DealId;	

    create table dbo.DealDealQuota (
        DealId INT not null,
       DealQuotaId INT not null,
       primary key (DealId, DealQuotaId)    )    
    
 COMMIT TRAN          
 --ROLLBACK TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	
	drop table #NewDealQuota
	drop table DealDealQuota
	
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

GO

BEGIN TRY
	BEGIN TRAN		
		
	PRINT 'Обновление выполнено успешно'
    
insert into DealDealQuota(DealId, DealQuotaId) 
select T.DealId, T.NewDealQuotaId from
(
select new.Id NewDealQuotaId, old.DealId from dbo.[DealQuota] old join #NewDealQuota new
ON old.IsPrepayment = new.IsPrepayment 
AND old.DiscountPercent = new.DiscountPercent 
AND old.PostPaymentDays = new.PostPaymentDays 
AND old.CreditLimitSum = new.CreditLimitSum) T

	update SaleWaybill
    set QuotaId = (select ddq.DealQuotaId from DealDealQuota ddq where ddq.DealId = SaleWaybill.DealId)
    
    alter table dbo.SaleWaybill
    alter column QuotaId INT not null;
    
	alter table dbo.[DealQuota]
	drop column DealId;
	
	alter table dbo.[SaleWaybill] 
	drop constraint FK_SaleWaybill_Quota 
	
	delete from DealQuota

SET IDENTITY_INSERT DealQuota ON

insert into DealQuota(Id, Name, CreationDate, DeletionDate, StartDate, EndDate, IsPrepayment, DiscountPercent, PostPaymentDays, CreditLimitSum)
select q.Id, q.Name, CURRENT_TIMESTAMP CreationDate, NULL DeletionDate, CURRENT_TIMESTAMP StartDate, NULL EndDate, q.IsPrepayment, q.DiscountPercent, q.PostPaymentDays, q.CreditLimitSum
from #NewDealQuota q

SET IDENTITY_INSERT DealQuota OFF

alter table dbo.DealDealQuota 
        add constraint FK5CE03E9C5BBBD660 
        foreign key (DealQuotaId) 
        references dbo.[DealQuota]
        
alter table dbo.DealDealQuota 
        add constraint PFK_DealQuota_Deal 
        foreign key (DealId) 
        references dbo.[Deal] 
        
alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_Quota 
        foreign key (QuotaId) 
        references dbo.[DealQuota]

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3509, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3510, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3511, 1)

    
COMMIT TRAN   
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	
	drop table #NewDealQuota
	drop table DealDealQuota
	
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH



