/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.44

  Что нового:
	* Выставлен признак полной оплаты для реализаций (исправлен баг)
	
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

SET @PreviousVersion = '0.9.43' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.44'			-- номер новой версии

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

-- Получаем стоимость накладной в ОЦ без возвратов
SELECT 
	SW.Id,
	SUM(swr.SalePrice * swr.SellingCount) as Sum 
	INTO #SaleSum
FROM
	SaleWaybill sw
	join ExpenditureWaybill ew on ew.Id = sw.Id
	join SaleWaybillRow swr on swr.SaleWaybillId = sw.Id
WHERE
	sw.DeletionDate is NULL
	AND swr.DeletionDate is NULL
	AND NOT(ew.AcceptanceDate is NULL)
	AND sw.IsFullyPaid = 0
	GROUP BY sw.Id
	
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Сумма возвратов
SELECT 
	SW.Id,
	SUM(rfcwr.ReturnCount * swr.SalePrice) as Sum 
	INTO #ReturnSum
FROM
	SaleWaybill sw
	join ExpenditureWaybill ew on ew.Id = sw.Id
	join SaleWaybillRow swr on swr.SaleWaybillId = sw.Id
	join ReturnFromClientWaybillRow rfcwr on rfcwr.SaleWaybillRowId = swr.Id
	join ReturnFromClientWaybill rfcw on rfcw.Id = rfcwr.ReturnFromClientWaybillId
WHERE
	sw.DeletionDate is NULL
	AND swr.DeletionDate is NULL
	AND rfcwr.DeletionDate is NULL
	AND sw.IsFullyPaid = 0
	AND NOT(ew.AcceptanceDate is NULL)
	AND NOT(rfcw.ReceiptDate is NULL)
	GROUP BY sw.Id
	
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Сумма оплат
SELECT 
	SW.Id,
	SUM(
		CASE
			WHEN NOT(dd.Sum is NULL) THEN dd.Sum ELSE 0
		END
	) as Sum
	INTO #PaymentSum
FROM
	SaleWaybill sw
	join ExpenditureWaybill ew on ew.Id = sw.Id
	join DealPaymentDocumentDistributionToSaleWaybill ddsw on ddsw.SaleWaybillId = sw.Id
	join DealPaymentDocumentDistribution dd on dd.Id = ddsw.Id
WHERE
	sw.DeletionDate is NULL
	AND NOT(ew.AcceptanceDate is NULL)
	AND sw.IsFullyPaid = 0
	GROUP BY sw.Id

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Сумма возвращенной оплаты (т.е. когда товар оплачен и его возвращают, то денежные средства переходят в нераспределенные оплаты)
SELECT 
	SW.Id,
	SUM(
		CASE
			WHEN NOT(dd.Sum is NULL) THEN dd.Sum ELSE 0
		END
	) as Sum
	INTO #ReturnPaymentSum
FROM
	SaleWaybill sw
	join ExpenditureWaybill ew on ew.Id = sw.Id
	join DealPaymentDocumentDistributionToReturnFromClientWaybill ddr on ddr.SaleWaybillId = sw.Id
	join DealPaymentDocumentDistribution dd on dd.Id = ddr.Id
WHERE
	sw.DeletionDate is NULL
	AND NOT(ew.AcceptanceDate is NULL)
	AND sw.IsFullyPaid = 0
	GROUP BY sw.Id	

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Получаем неоплаченный остаток по реализациям
SELECT 
	sw.Id,
	(saleSum.Sum
		- CASE WHEN paymentSum.Sum is NULL THEN 0 ELSE paymentSum.Sum END 
		+ CASE WHEN returnPaymentSum.Sum  is NULL THEN 0 ELSE returnPaymentSum.Sum  END
		- CASE WHEN returnSum.Sum is NULL THEN 0 ELSE returnSum.Sum  END) Remainder
	INTO #Remainder
FROM
	SaleWaybill sw
	left join #SaleSum saleSum on saleSum.Id = sw.Id
	left join #ReturnSum returnSum on returnSum.Id = sw.Id
	left join #PaymentSum paymentSum on paymentSum.Id = sw.Id
	left join #ReturnPaymentSum returnPaymentSum on returnPaymentSum .Id = sw.Id
WHERE
	sw.DeletionDate is NULL
	AND sw.IsFullyPaid = 0
	
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Выставляем флаг полной оплаты
UPDATE SaleWaybill
SET IsFullyPaid = 1
WHERE
	exists(	
		SELECT *
		FROM 
			#Remainder r
		WHERE
			r.Id = SaleWaybill.Id 
			AND r.Remainder = 0
	)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #SaleSum
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #ReturnSum
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #PaymentSum
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #ReturnPaymentSum
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #Remainder
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

