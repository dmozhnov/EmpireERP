/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.39

  Что нового:
	+ Добавлено поле "Команда" в платежные документы по сделке
	+ Добавлено поле "Команда" в корректировки начального сальдо по сделке
	+ Добавлено поле "Команда" в накладные возврата товара
	* Поле "Команда" в накладных реализации сделано обязательным для заполнения
	+ Выставлено значение поля "Команда" для всех выше перечисленных сущностей
	
---------------------------------------------------------------------------------------*/
--SET NOEXEC OFF	-- выполнить данную команду в случае неуспешного обновления
SET DATEFORMAT DMY
SET NOCOUNT ON
SET ARITHABORT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

DECLARE @PreviousVersion varchar(15),	-- номер предыдущей версии
		@CurrentVersion varchar(15),	-- номер текущей версии базы данных
		@NewVersion varchar(15),		-- номер новой версии
		@DataBaseName varchar(256),		-- текущая база данных
		@CurrentDate nvarchar(10),		-- текущая дата
		@CurrentTime nvarchar(10),		-- текущее время
		@BackupTarget nvarchar(100)		-- куда делать бэкап базы данных

SET @PreviousVersion = '0.9.38' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.39'			-- номер новой версии

SELECT @CurrentVersion = DataBaseVersion FROM Setting
IF @@ERROR <> 0
BEGIN
	PRINT 'Неверная база данных'
END
ELSE
BEGIN
	-- СОЗДАЕМ БЭКАП БАЗЫ ДАННЫХ
	-- Получаем текущую дату
	SET @CurrentDate = CONVERT(nvarchar(20), GETDATE(), 104)	--	dd.mm.yyyy
	SET @CurrentTime = REPLACE(CONVERT(nvarchar(20), GETDATE(), 108) , ':', '.') --	hh:mm:ss
	SET @DataBaseName = DB_NAME()

	SET @BackupTarget = N'D:\Bizpulse\Backup\Update\' + @DataBaseName + '(' + CAST(@CurrentVersion as nvarchar(20)) + ') ' + 
		@CurrentDate + ' ' + @CurrentTime + N'.bak'

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = 
		N'Обновление версии', NOFORMAT

	IF @@ERROR <> 0
	BEGIN
		PRINT 'Ошибка создания backup''а. Продолжение выполнения невозможно.'
	END
	ELSE
		BEGIN

		IF (@CurrentVersion <> @PreviousVersion)
		BEGIN
			PRINT 'Обновить базу данных ' + @DataBaseName + ' до версии ' + @NewVersion + 
				' можно только из версии  ' + @PreviousVersion +
				'. Текущая версия: ' + @CurrentVersion
		END
		ELSE
		BEGIN
			--Начинаем транзакцию
			BEGIN TRAN

			--Обновляем версию базы данных
			UPDATE Setting 
			SET DataBaseVersion = @NewVersion		
		END
	END
END
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


--***********************************************************************************************
--*********** Добавляем команду *****************************************************************
--***********************************************************************************************
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentDocument_Team]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocument]'))
alter table dbo.[DealPaymentDocument]  drop constraint FK_DealPaymentDocument_Team

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_Team]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_Team

alter table DealPaymentDocument add TeamId SMALLINT null;
alter table ReturnFromClientWaybill add TeamId SMALLINT null;

alter table dbo.[DealPaymentDocument] 
    add constraint FK_DealPaymentDocument_Team 
    foreign key (TeamId) 
    references dbo.[Team]
    
alter table dbo.[ReturnFromClientWaybill] 
    add constraint FK_ReturnFromClientWaybill_Team 
    foreign key (TeamId) 
    references dbo.[Team]
    
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


--***********************************************************************************************   
--*********** Добавляем безкомандных пользователей в основную команду ***************************    
--***********************************************************************************************
INSERT INTO UserTeam (UserId, TeamId)
SELECT 
	u.Id,
	1
FROM 
	dbo.[User] u
	left join UserTeam ut on ut.UserId = u.Id
WHERE
	ut.TeamId is NULL

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--***********************************************************************************************
--********** Проставляем команду реализациям ****************************************************    
--***********************************************************************************************

-- Првоеряем, входит ли куратор реализации без команды в одну команду. Если больше чем в одну,
-- то выводим сообщение об ошибке
declare @TeamCountForSaleWaybill int;
declare @MessageForsaleWaybill varchar(8000);

set @TeamCountForSaleWaybill = 
(
	SELECT count(*)
	FROM 
		SaleWaybill sw
		join dbo.[User] u on u.Id = sw.SaleWaybillCuratorId
		join UserTeam ut on u.Id = ut.UserId
		join Team t on t.Id = ut.TeamId
	WHERE
		sw.TeamId is NULL
)

set @MessageForSaleWaybill = ''

SELECT @MessageForSaleWaybill = @MessageForSaleWaybill + case when @MessageForSaleWaybill <> '' then ', ' else ' ' end + sw.Number
FROM 
	SaleWaybill sw
	join dbo.[User] u on u.Id = sw.SaleWaybillCuratorId
	join UserTeam ut on u.Id = ut.UserId
	join Team t on t.Id = ut.TeamId
WHERE
	sw.TeamId is NULL
	
set @MessageForSaleWaybill = 'Существуют реализации, у которых не задана команда и куратор входит более чем в одну команду. Номера накладных :' + @MessageForsaleWaybill

IF @TeamCountForSaleWaybill > 1 RAISERROR(@MessageForSaleWaybill, 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


-- Проставляем команду для накладных реализации, которые не имели ее
UPDATE SaleWaybill
SET
	TeamId = d.TeamId
FROM(
	SELECT sw.Id as SaleWaybillId
		,t.Id as TeamId
	FROM 
		SaleWaybill sw
		inner join dbo.[User] u on u.Id = sw.SaleWaybillCuratorId
		inner join UserTeam ut on u.Id = ut.UserId
		inner join Team t on t.Id = ut.TeamId
	WHERE
		sw.TeamId is NULL
) d
WHERE 
	Id = d.SaleWaybillId

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--***********************************************************************************************
--*********** Проставляем команду оплатам от клиента ********************************************    
--***********************************************************************************************
CREATE TABLE #SaleWybillTeamDictionary(
	SaleWaybillId uniqueidentifier not null
	,TeamId smallint not null
)
-- Добавляем накладные, для которых команда проставлена
INSERT INTO #SaleWybillTeamDictionary
SELECT
	sw.Id as SaleWaybillId
	,sw.TeamId as TeamId
FROM 
	SaleWaybill sw
WHERE
	not(sw.TeamId is NULL);

-- Проверяем, разнесена ли оплата на накладные разных команд
declare @TeamCountForPayment int;
declare @MessageTeamCountForPayment varchar(8000);

set @TeamCountForPayment = (
	SELECT COUNT(*)
	FROM
		DealPaymentDocument dp
		join DealPaymentFromClient dpfc on dpfc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
)
set @MessageTeamCountForPayment = '';
SELECT @MessageTeamCountForPayment = @MessageTeamCountForPayment + case when @MessageTeamCountForPayment <> '' then ', ' else ' ' end + CONVERT(varchar(50), dp.Id)
	FROM
		DealPaymentDocument dp
		join DealPaymentFromClient dpfc on dpfc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
set @MessageTeamCountForPayment = 'Существую оплаты, которые разнесены на документы разных команд. Коды оплат:' + @MessageTeamCountForPayment

IF @TeamCountForPayment > 1 RAISERROR(@MessageTeamCountForPayment, 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Ошибок нет, проставляем команду оплатам от клиента
UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM( 
	SELECT 
		dp.Id as Id
		,swtd.TeamId as TeamId
	FROM
		DealPaymentDocument dp
		join DealPaymentFromClient dpfc on dpfc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.TeamId) d
WHERE
	DealPaymentDocument.Id = d.Id
	
drop table #SaleWybillTeamDictionary;
GO

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO      
        
--***********************************************************************************************        
--************ Проставляем команду кридитовым корректировкам ************************************
--***********************************************************************************************
CREATE TABLE #SaleWybillTeamDictionary(
	SaleWaybillId uniqueidentifier not null
	,TeamId smallint not null
)
-- Добавляем накладные, для которых команда проставлена
INSERT INTO #SaleWybillTeamDictionary
SELECT
	sw.Id as SaleWaybillId
	,sw.TeamId as TeamId
FROM 
	SaleWaybill sw
WHERE
	not(sw.TeamId is NULL);

-- Проверяем, разнесена ли корректировка на накладные разных команд
declare @TeamCountForCreditCorrection int;
declare @MessageTeamCountForCreditCorrection varchar(8000);

set @TeamCountForCreditCorrection = (
	SELECT COUNT(*)
	FROM
		DealPaymentDocument dp
		join DealInitialBalanceCorrection dibc on dibc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
)
set @MessageTeamCountForCreditCorrection = '';
SELECT @MessageTeamCountForCreditCorrection = @MessageTeamCountForCreditCorrection + case when @MessageTeamCountForCreditCorrection <> '' then ', ' else ' ' end + CONVERT(varchar(50), dp.Id)
	FROM
		DealPaymentDocument dp
		join DealInitialBalanceCorrection dibc on dibc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.SaleWaybillId
	having count(distinct swtd.TeamId) > 1
set @MessageTeamCountForCreditCorrection = 'Существую кредитовые корректировки, которые разнесены на документы разных команд. Коды корректировок:' + @MessageTeamCountForCreditCorrection

IF @TeamCountForCreditCorrection > 1 RAISERROR(@MessageTeamCountForCreditCorrection, 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Ошибок нет, проставляем команду кредитовым корректировкам
UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM( 
	SELECT 
		dp.Id as Id
		,swtd.TeamId as TeamId
	FROM
		DealPaymentDocument dp
		join DealInitialBalanceCorrection dibc on dibc.Id = dp.Id
		join DealPaymentDocumentDistribution dpdd on dp.Id = dpdd.SourceDealPaymentDocumentId
		join DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = dpdd.Id
		join #SaleWybillTeamDictionary swtd on swtd.SaleWaybillId = swd.SaleWaybillId
	GROUP BY
		dp.Id
		,swtd.TeamId) d
WHERE
	DealPaymentDocument.Id = d.Id
	
drop table #SaleWybillTeamDictionary;
GO

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO      

--***********************************************************************************************
--************* Проставляем команду возвратам оплаты клиентам ***********************************    
--***********************************************************************************************
-- Проверяем, не оплачен ли возврат оплаты документами разных команд
declare @TeamCountForPaymentToClient int;
declare @MessageTeamCountForPaymentToClient varchar(8000);

set @TeamCountForPaymentToClient = (
	SELECT COUNT(*)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealPaymentToClient dptc on dptc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	
	GROUP BY dptc.Id
	having count(distinct dpd.TeamId) > 1
)
set @MessageTeamCountForPaymentToClient = '';
SELECT @MessageTeamCountForPaymentToClient = @MessageTeamCountForPaymentToClient + case when @MessageTeamCountForPaymentToClient <> '' then ', ' else ' ' end + CONVERT(varchar(50), dptc.Id)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealPaymentToClient dptc on dptc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY dptc.Id
	having count(distinct dpd.TeamId) > 1
set @MessageTeamCountForPaymentToClient = 'Существуют возвраты оплаты клиенту, на которые разнесены документы разных команд. Коды возвратов оплаты:' + @MessageTeamCountForPaymentToClient

IF @TeamCountForPaymentToClient > 1 RAISERROR(@MessageTeamCountForPaymentToClient, 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- ошибок нет. Проставляем команду для возврата оплаты
UPDATE DealPaymentDocument
SET TeamId = d.TeamId
FROM(
	SELECT
		dptc.Id
		,dpd.TeamId
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealPaymentToClient dptc on dptc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	) d
WHERE
	DealPaymentDocument.Id = d.Id

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--***********************************************************************************************
--************* Проставляем команду дебитовой корректировке *************************************
--***********************************************************************************************    

-- Проверяем, не оплачена ли корректировка документами разных команд
declare @TeamCountForDebitCorrection int;
declare @MessageTeamCountForDebitCorrection varchar(8000);

set @TeamCountForDebitCorrection = (
	SELECT COUNT(*)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealDebitInitialBalanceCorrection ddibc on ddibc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY ddibc.Id
	having count(distinct dpd.TeamId) > 1
)
set @MessageTeamCountForDebitCorrection = '';
SELECT @MessageTeamCountForDebitCorrection = @MessageTeamCountForDebitCorrection + case when @MessageTeamCountForDebitCorrection <> '' then ', ' else ' ' end + CONVERT(varchar(50), ddibc.Id)
	FROM 
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealDebitInitialBalanceCorrection ddibc on ddibc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY ddibc.Id
	having count(distinct dpd.TeamId) > 1
set @MessageTeamCountForDebitCorrection = 'Существую дебитовые корректировки, на которые разнесены документы разных команд. Коды корректировок:' + @MessageTeamCountForDebitCorrection

IF @TeamCountForDebitCorrection > 1 RAISERROR(@MessageTeamCountForDebitCorrection, 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Ошибок нет, проставляем команду кредитовым корректировкам
UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM( 
	SELECT 
		ddibc.Id as Id
		,dpd.TeamId as TeamId
	FROM
		DealPaymentDocument dpd
		join DealPaymentDocumentDistribution dpdd on dpdd.SourceDealPaymentDocumentId = dpd.Id
		join DealPaymentDocumentDistributionToDealPaymentDocument dpddtdpd on dpddtdpd.Id = dpdd.Id
		join DealDebitInitialBalanceCorrection ddibc on ddibc.Id = dpddtdpd.DestinationDealPaymentDocumentId
	GROUP BY
		ddibc.Id
		,dpd.TeamId) d
WHERE
	DealPaymentDocument.Id = d.Id	


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--***********************************************************************************************
--******** Проставляем команду оплатам, которые разнесены только на платежные документы *********
--***********************************************************************************************
-- Проставляем таким документам команду куратора сделки. Сейчас NULL вместо команды только 
-- у таких документов

UPDATE DealPaymentDocument
SET
	TeamId = d.TeamId
FROM(
	SELECT
		dpd.Id as Id
		,MIN(t.Id) as TeamId
	FROM
		DealPaymentDocument dpd
		join Deal d on dpd.DealId = d.Id
		join dbo.[User] u on u.Id = d.CuratorId
		join UserTeam ut on ut.UserId = u.Id
		join Team t on t.Id = ut.TeamId
	GROUP BY dpd.Id
	)d
WHERE
	DealPaymentDocument.Id = d.Id 
	AND DealPaymentDocument.TeamId is NULL
    

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--***********************************************************************************************
--********** Проставляем команду для возвратов товаров ******************************************
--***********************************************************************************************    

-- Проверяем, не сделан ли возврат по реализация разных команд
declare @TeamCountForRetrunFromClientWaybill int;
declare @MessageTeamCountForRetrunFromClientWaybill varchar(8000);

set @TeamCountForRetrunFromClientWaybill = (
	SELECT COUNT(*)
	FROM
		ReturnFromClientWaybill rfcw
		join ReturnFromClientWaybillRow rfcwr on rfcw.Id = rfcwr.ReturnFromClientWaybillId
		join SaleWaybillRow swr on rfcwr.SaleWaybillRowId = swr.Id
		join SaleWaybill sw on sw.Id = swr.SaleWaybillId
	GROUP BY rfcw.Id
	having count(distinct sw.TeamId) > 1
)
set @MessageTeamCountForRetrunFromClientWaybill = '';
SELECT @MessageTeamCountForRetrunFromClientWaybill = @MessageTeamCountForRetrunFromClientWaybill + case when @MessageTeamCountForRetrunFromClientWaybill <> '' then ', ' else ' ' end + CONVERT(varchar(50), rfcw.Id)
	FROM
		ReturnFromClientWaybill rfcw
		join ReturnFromClientWaybillRow rfcwr on rfcw.Id = rfcwr.ReturnFromClientWaybillId
		join SaleWaybillRow swr on rfcwr.SaleWaybillRowId = swr.Id
		join SaleWaybill sw on sw.Id = swr.SaleWaybillId
	GROUP BY rfcw.Id
	having count(distinct sw.TeamId) > 1
set @MessageTeamCountForRetrunFromClientWaybill = 'Существуют возвраты товара, которые сделаны по реализациям разных команд. Коды возвратов:' + @MessageTeamCountForRetrunFromClientWaybill

IF @TeamCountForRetrunFromClientWaybill > 1 RAISERROR(@MessageTeamCountForRetrunFromClientWaybill, 16, 1)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Ошибок нет, проставляем команду возвратам
UPDATE ReturnFromClientWaybill
SET
	TeamId = d.TeamId
FROM(
	SELECT rfcw.Id as Id
		,Min(sw.TeamId) as TeamId
	FROM
		ReturnFromClientWaybill rfcw
		join ReturnFromClientWaybillRow rfcwr on rfcw.Id = rfcwr.ReturnFromClientWaybillId
		join SaleWaybillRow swr on rfcwr.SaleWaybillRowId = swr.Id
		join SaleWaybill sw on sw.Id = swr.SaleWaybillId
	GROUP BY rfcw.Id
	) d
WHERE
	ReturnFromClientWaybill.Id = d.Id


-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--***********************************************************************************************
--********** Проставляем команду для возвратов товаров без позиций ******************************
--*********************************************************************************************** 
-- Выставляем команду по куратору накладной

UPDATE ReturnFromClientWaybill
SET
	TeamId = d.TeamId
FROM(
	SELECT
		rfcw.Id as Id
		,MIN(t.Id) as TeamId
	FROM
		ReturnFromClientWaybill rfcw
		join dbo.[User] u on u.Id = rfcw.ReturnFromClientWaybillCuratorId
		join UserTeam ut on ut.UserId = u.Id
		join Team t on t.Id = ut.TeamId
	GROUP BY rfcw.Id
	)d
WHERE
	ReturnFromClientWaybill.Id = d.Id 
	AND ReturnFromClientWaybill.TeamId is NULL

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

--***********************************************************************************************
--************ Делаем поле команда обязательным для заполнения **********************************
--***********************************************************************************************
alter table DealPaymentDocument alter column TeamId SMALLINT NOT null;
alter table ReturnFromClientWaybill alter column TeamId SMALLINT NOT null;
alter table SaleWaybill alter column TeamId SMALLINT NOT null;

IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId')
DROP INDEX IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId ON [dbo].[AcceptedSaleIndicator]
GO
alter table AcceptedSaleIndicator alter column TeamId SMALLINT NOT null;
GO
CREATE INDEX IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId ON [AcceptedSaleIndicator] ([StorageId], [UserId], [TeamId], [DealId])
GO
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId')
DROP INDEX IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId ON [dbo].[ShippedSaleIndicator]
GO
alter table ShippedSaleIndicator alter column TeamId SMALLINT NOT null;
GO
CREATE INDEX IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId ON [ShippedSaleIndicator] ([StorageId], [UserId], [TeamId], [DealId])
GO

alter table AcceptedReturnFromClientIndicator alter column TeamId SMALLINT NOT null;
alter table ReceiptedReturnFromClientIndicator alter column TeamId SMALLINT NOT null;
alter table ReturnFromClientBySaleAcceptanceDateIndicator alter column TeamId SMALLINT NOT null;
alter table ReturnFromClientBySaleShippingDateIndicator alter column TeamId SMALLINT NOT null;

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO