/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.49

  Что нового:
	+ поле SourceDistributionToReturnFromClientWaybillId в таблицы DealPaymentDocumentDistributionToDealPaymentDocument и DealPaymentDocumentDistributionToSaleWaybill
	* переразнесение оплат 
		
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

SET @PreviousVersion = '0.9.48' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.49'			-- номер новой версии

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

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = N'Обновление версии', NOFORMAT

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

go
CREATE FUNCTION dbo.MaxDate3
(
  @Date1 datetime,
  @Date2 datetime,
  @Date3 datetime
)
RETURNS datetime
WITH SCHEMABINDING
AS
BEGIN
  DECLARE @MaxDate datetime
  SET @MaxDate = COALESCE(@Date1, @Date2, @Date3)

  IF @MaxDate IS NULL RETURN NULL

  IF @MaxDate < ISNULL(@Date1, '1900-01-01')
	  SET @MaxDate = @Date1

  IF @MaxDate < ISNULL(@Date2, '1900-01-01')
	  SET @MaxDate = @Date2

  IF @MaxDate < ISNULL(@Date3, '1900-01-01')
	  SET @MaxDate = @Date3

  RETURN @MaxDate
END
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- источники средств для оплаты
create table PaymentSource(
	PaymentId uniqueIdentifier NOT NULL,
	SourceId uniqueIdentifier NULL,
	Sum decimal(18,2) NOT NULL,
	Date DateTime NOT NULL,
	RowNumber int NOT NULL,
	Id int NOT NULL IDENTITY (1,1)
)
GO
CREATE INDEX IX_PaymentSource ON PaymentSource (PaymentId, SourceId, RowNumber, Id);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- План распределения средств на разнесения
CREATE TABLE DistributionPlan(
	DistributionId uniqueidentifier NOT NULL,
	SourceId uniqueIdentifier NULL,
	Sum decimal(18,2) NOT NULL,
	Date DateTime NOT NULL,
	Id int IDENTITY(1,1) not null,
	primary key (Id)
);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- Имеющиеся разнесения
CREATE TABLE Distributions(
	DistributionId uniqueidentifier NOT NULL,
	PaymentId uniqueidentifier NOT NULL,
	Sum decimal(18,2) NOT NULL,
	Date DateTime NOT NULL,
	RowNumber int NOT NULL
);
GO
CREATE INDEX IX_Distributions ON Distributions (RowNumber);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table TMP_DealPaymentDocumentDistribution (
		Id UNIQUEIDENTIFIER not null,
	   Sum DECIMAL(18, 2) not null,
	   CreationDate DATETIME not null,
	   DistributionDate DATETIME not null,
	   OrdinalNumber INT not null,
	   SourceDealPaymentDocumentId UNIQUEIDENTIFIER not null,
	   --primary key (Id)
	);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

create table TMP_DealPaymentDocumentDistributionToDealPaymentDocument (
	   Id UNIQUEIDENTIFIER not null,
	   DestinationDealPaymentDocumentId UNIQUEIDENTIFIER not null,
	   SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null,
	   --primary key (Id)
	);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO  

create table TMP_DealPaymentDocumentDistributionToSaleWaybill (
	   Id UNIQUEIDENTIFIER not null,
	   SaleWaybillId UNIQUEIDENTIFIER not null,
	   SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null,
	   --primary key (Id)
	);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table DealPaymentDocumentDistributionToDealPaymentDocument add SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER NULL
GO
alter table DealPaymentDocumentDistributionToSaleWaybill add SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
	

	
	-- Обновляем даты разнесений для возвратов оплаты
	UPDATE DealPaymentDocumentDistribution
	SET 
		DistributionDate = s.ReceiptDate
	FROM
	(
		SELECT 
			pd.Id
			,r.ReceiptDate
		FROM
			DealPaymentDocumentDistribution pd
			join DealPaymentDocumentDistributionToReturnFromClientWaybill pds on pd.id = pds.id
			join ReturnFromclientWaybill r on r.Id = pds.ReturnFromclientWaybillId
	) s
	WHERE
		DealPaymentDocumentDistribution.Id = s.Id
		

	-- Добавлем в источники средств саму оплату
	INSERT INTO PaymentSource (PaymentId,SourceId,Sum,Date,RowNumber)
	SELECT
		pd.Id
		,NULL
		,pd.Sum
		,pd.Date
		,0
	FROM
		DealPaymentDocument pd
		join DealPaymentFromClient pfc on pd.Id = pfc.Id
	WHERE
		pd.DeletionDate is NULL
		
	-- 	Добавлем в источники средств корректировку
	INSERT INTO PaymentSource (PaymentId,SourceId,Sum,Date,RowNumber)
	SELECT
		pd.Id
		,NULL
		,pd.Sum
		,pd.Date
		,0
	FROM
		DealPaymentDocument pd
		join DealCreditInitialBalanceCorrection cbc on pd.Id = cbc.Id
	WHERE
		pd.DeletionDate is NULL	

	-- Добавляем в качестве источников возвраты оплаты	
	INSERT INTO PaymentSource (PaymentId, SourceId, Sum, Date,RowNumber)
	SELECT
		pdd.SourceDealPaymentDocumentId
		,pdd.Id
		,-pdd.Sum
		,pdd.DistributionDate
		,0
	FROM
		DealPaymentDocumentDistribution pdd
		join DealPaymentDocumentDistributionToReturnFromClientWaybill rfc on pdd.Id = rfc.Id
	WHERE
		pdd.SourceDealPaymentDocumentId in (SELECT PaymentId FROM PaymentSource)

	UPDATE PaymentSource
	SET RowNumber = s.rn
	FROM
	(SELECT
		 ROW_NUMBER() OVER(ORDER BY Date) rn
		 ,ps.Id
	FROM
		PaymentSource ps) s
	WHERE
		s.Id = PaymentSource.Id
		
	--***************************
	INSERT INTO Distributions(DistributionId,PaymentId,Sum,Date,RowNumber)
	SELECT
		pdd.Id
		,pdd.SourceDealPaymentDocumentId
		,pdd.Sum
		,pdd.DistributionDate
		,0
	FROM
		DealPaymentDocumentDistribution pdd
		join DealPaymentDocumentDistributionToSaleWaybill dsw on pdd.Id = dsw.Id
		

	INSERT INTO Distributions(DistributionId,PaymentId,Sum,Date,RowNumber)
	SELECT
		pdd.Id
		,pdd.SourceDealPaymentDocumentId
		,pdd.Sum
		,pdd.DistributionDate
		,0
	FROM
		DealPaymentDocumentDistribution pdd
		join DealPaymentDocumentDistributionToDealPaymentDocument dpdd on pdd.Id = dpdd.Id	
		
	UPDATE Distributions
	SET RowNumber = s.rn
	FROM
	(SELECT
		 ROW_NUMBER() OVER(ORDER BY Date) rn
		 ,d.DistributionId
	FROM
		Distributions d) s
	WHERE
		s.DistributionId = Distributions.DistributionId

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	-- Part 2

	declare @distributionNumber int -- номер очередного разнесения
	declare @distributionId uniqueidentifier	-- код очередного разнесения
	declare @distributionSum decimal(18,2)	-- сумма очередного разнесения

	declare @paymentSourceNumber int -- номер очередного источника
	declare @paymentSourceSum decimal(18,2)	-- сумма очередного источника

	declare @partSum decimal(18,2)	-- разносимая сумма с очередного источника на очередное разнесение
	declare @sourceId uniqueidentifier
	declare @date DateTime

	--***************************************************************************************************************
	SELECT @distributionNumber = min(RowNumber) FROM Distributions

	SELECT
		@distributionSum = d.Sum,
		@distributionId = d.DistributionId
	FROM 
		Distributions d
	WHERE 
		d.RowNumber = @distributionNumber

	-- Цикл прохода по всем разнесениям оплаты
	while (@distributionNumber is not null)
	begin
		--++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		 -- Номер первого источника средств
		 SELECT @paymentSourceNumber = 
			min(RowNumber) 
			FROM 
				PaymentSource ps
			WHERE 
				exists (
					SELECT *
					FROM 
						Distributions d
					WHERE
						d.PaymentId = ps.PaymentId
						AND d.DistributionId = @distributionId
				)
				AND ps.Sum > 0
				 
		-- Доступная сумма источника
		SELECT 
			@paymentSourceSum = ps.Sum,
			@sourceId = ps.SourceId,
			@date = ps.Date
		FROM	
			PaymentSource ps
		WHERE 
			ps.RowNumber = @paymentSourceNumber

		
		-- Цикл распределения источников на разнесения средств
		while (@paymentSourceSum is not null AND @distributionSum > 0)
		begin
			-- PRINT 'SourceNumber: '+CONVERT(varchar,@paymentSourceNumber);
			
			IF ( @paymentSourceSum > 0)
			begin
				--////////////////////////////////////////////////////////////////////
				IF @distributionSum <= @paymentSourceSum
				-- Источника хватает для разнесения
				SET @partSum = @distributionSum
				-- Источника не хватает для разнесения, забираем все средства с этого источника
				ELSE SET @partSum = @paymentSourceSum;
				
				-- Добавлем распределение средств на разнесение
				INSERT INTO DistributionPlan (
					DistributionId
					,SourceId
					,Sum
					,Date)
				VALUES (
					@distributionId
					,@sourceId
					,@partSum
					,@date
				)
				
				-- Уменьшаем сумму разнесения
				SET @distributionSum = @distributionSum - @partSum;

				-- Уменьшаем сумму источника
				UPDATE PaymentSource
				SET 
					Sum = Sum - @partSum
				WHERE
					PaymentSource.RowNumber = @paymentSourceNumber		
				--////////////////////////////////////////////////////////////////////
			end
			-- Номер очередного источника средств
			SET @paymentSourceNumber = (
				SELECT TOP(1) ps.RowNumber
				FROM 
					PaymentSource ps
				WHERE 
					ps.RowNumber > @paymentSourceNumber 
					AND ps.Sum > 0
					AND exists (
						SELECT *
						FROM 
							Distributions d
						WHERE
							d.PaymentId = ps.PaymentId
							AND d.DistributionId = @distributionId
				)
				ORDER BY ps.RowNumber
			)
			-- Доступная сумма очередного источника
			SELECT 
				@paymentSourceSum = ps.Sum,
				@sourceId = ps.SourceId,
				@date = ps.Date
			FROM	
				PaymentSource ps
			WHERE 
				ps.RowNumber = @paymentSourceNumber
		end
		
		-- Уменьшаем сумму к разнесению
		UPDATE Distributions
		SET 
			Sum = @distributionSum
		WHERE
			Distributions.RowNumber = @distributionNumber
		
		--++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		
		-- Получаем номер очередного разнесения
		SET @distributionNumber = (
			SELECT TOP(1) d.RowNumber
			FROM Distributions d
			WHERE d.RowNumber > @distributionNumber
			ORDER BY d.RowNumber
		)
		
		SELECT
			@distributionSum = d.Sum,
			@distributionId = d.DistributionId
		FROM 
			Distributions d
		WHERE 
			d.RowNumber = @distributionNumber
	end
	--***************************************************************************************************************

	-- Part 3

	-- Создаем разнесения
	declare @Id uniqueidentifier	-- ID
	--declare @DistributionId uniqueidentifier	-- код разнесения
	--declare @SourceId uniqueIdentifier	-- код источника средств разнесения
	declare @Sum decimal(18,2)	-- сумма средств разнесения
	--declare @Date DateTime	-- дата разнесения
	declare @PlanId int	-- код текущего элемента в плане разнесений
	declare @CreationDate DateTime	-- дата создания разнесения
	declare @SourceDealPaymentDocumentId uniqueIdentifier	-- код оплаты, из которой делается разнесение

	declare @IsDistributionToSaleWaybill int	-- Признак разнесения на реализацию
	declare @SaleWaybillId uniqueIdentifier	-- код реализации, на которую делается разнесение
	declare @DestinationDealPaymentDocumentId uniqueIdentifier	-- код документа, на который делается разнесение

	SELECT @PlanId = min(Id) FROM DistributionPlan;

	SELECT
		@DistributionId = d.DistributionId,
		@SourceId = d.SourceId,
		@Sum = d.Sum,
		@Date = d.Date
	FROM (
		SELECT *
		FROM 
			DistributionPlan dp
		WHERE
			dp.Id = @PlanId) d

	while (@PlanId is not null)
	begin
		-- Получаем данные из разнесения
		SELECT 
			@CreationDate = pdd.CreationDate,
			@SourceDealPaymentDocumentId = pdd.SourceDealPaymentDocumentId
		FROM
			DealPaymentDocumentDistribution pdd
		WHERE
			pdd.Id = @DistributionId;
		
		
		-- Определяем тип разнесения
		SELECT @IsDistributionToSaleWaybill = Count(*)
		FROM 
			DealPaymentDocumentDistributionToSaleWaybill swd
		WHERE
			swd.Id = @DistributionId
			
		
		-- Генерируем Id
		SET @Id = NEWID();
		
		-- Добавляем разнесение
		INSERT INTO TMP_DealPaymentDocumentDistribution(	
			Id
			,Sum 
			,CreationDate 
			,DistributionDate 
			,OrdinalNumber 
			,SourceDealPaymentDocumentId)
		VALUES(
			@Id
			,@Sum
			,@CreationDate
			,@Date
			,999	-- Как выставлять? После отдельным UPDATE?
			,@SourceDealPaymentDocumentId
		)
		
		IF @IsDistributionToSaleWaybill > 0
		begin
			SELECT 
				@SaleWaybillId = d.SaleWaybillId
			FROM
				DealPaymentDocumentDistributionToSaleWaybill d
			WHERE
				d.Id = @DistributionId
				
			INSERT INTO TMP_DealPaymentDocumentDistributionToSaleWaybill(
				Id,
				SaleWaybillId,
				SourceDistributionToReturnFromClientWaybillId)
			VALUES(
				@Id
				,@SaleWaybillId
				,@SourceId
			)
		end
		ELSE
		begin
			SELECT 
				@DestinationDealPaymentDocumentId = d.DestinationDealPaymentDocumentId
			FROM
				DealPaymentDocumentDistributionToDealPaymentDocument d
			WHERE
				d.Id = @DistributionId
				
			INSERT INTO TMP_DealPaymentDocumentDistributionToDealPaymentDocument(
				Id,
			   DestinationDealPaymentDocumentId,
			   SourceDistributionToReturnFromClientWaybillId)
			VALUES(
				@Id
				,@DestinationDealPaymentDocumentId
				,@SourceId
			)
		end	
		
		-- переходим к следующему элементу
		SELECT @PlanId = min(Id) FROM DistributionPlan WHERE Id > @PlanId;
		
		SELECT
			@DistributionId = d.DistributionId,
			@SourceId = d.SourceId,
			@Sum = d.Sum,
			@Date = d.Date
		FROM (
			SELECT *
			FROM 
				DistributionPlan dp
			WHERE
				dp.Id = @PlanId) d
	end

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	-- Part 4
	-- Проставляем правильные даты разнесений

	-- Обновляем дату разнесения для разнесений на реализацию
	UPDATE TMP_DealPaymentDocumentDistribution
	SET
		DistributionDate = d.maxDate
	FROM (
		SELECT 
			dbo.MaxDate3(sw.AcceptanceDate, dp.Date, rd.DistributionDate) as maxDate,
			pd.Id
		FROM
			TMP_DealPaymentDocumentDistribution pd
			join TMP_DealPaymentDocumentDistributionToSaleWaybill swd on swd.Id = pd.Id
			join ExpenditureWaybill sw on swd.SaleWaybillId = sw.Id
			join DealPaymentDocument dp on dp.Id = pd.SourceDealPaymentDocumentId
			left join TMP_DealPaymentDocumentDistribution rd on rd.Id = swd.SourceDistributionToReturnFromClientWaybillId
	) d
	WHERE
		d.Id = TMP_DealPaymentDocumentDistribution.Id 

	-- Обновляем дату разнесения для разнесений на документы
	UPDATE TMP_DealPaymentDocumentDistribution
	SET
		DistributionDate = d.maxDate
	FROM (
		SELECT 
			dbo.MaxDate3(dd.Date, dp.Date, rd.DistributionDate) as maxDate,
			pd.Id
		FROM
			TMP_DealPaymentDocumentDistribution pd
			join TMP_DealPaymentDocumentDistributionToDealPaymentDocument dpd on dpd.Id = pd.Id
			join DealPaymentDocument dd on dpd.DestinationDealPaymentDocumentId = dd.Id
			join DealPaymentDocument dp on dp.Id = pd.SourceDealPaymentDocumentId
			left join TMP_DealPaymentDocumentDistribution rd on rd.Id = dpd.SourceDistributionToReturnFromClientWaybillId
	) d
	WHERE
		d.Id = TMP_DealPaymentDocumentDistribution.Id 


	--********************************************************
	-- Выставляем OrderNumber для разнесений

	declare @paymentId uniqueidentifier
	SELECT TOP(1)
		@paymentId = d.Id
	FROM 
		DealPaymentDocument d
	ORDER BY d.Id

	-- Цикл прохода 
	while (@paymentId is not null)
	begin
		UPDATE TMP_DealPaymentDocumentDistribution
		SET OrdinalNumber = s.rn
		FROM
			(SELECT
				 ROW_NUMBER() OVER(ORDER BY CreationDate) rn
				 ,d.Id
				 ,d.SourceDealPaymentDocumentId
			FROM
				TMP_DealPaymentDocumentDistribution d
			WHERE
				@paymentId = d.SourceDealPaymentDocumentId
			) s
		WHERE
			s.Id = TMP_DealPaymentDocumentDistribution.Id
			AND TMP_DealPaymentDocumentDistribution.SourceDealPaymentDocumentId = s.SourceDealPaymentDocumentId
			
			
		--------------
		SET @paymentId = (
			SELECT TOP(1)d.Id
			FROM 
				DealPaymentDocument d
			WHERE
				d.Id > @paymentId
			ORDER BY d.Id
		)
	end

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

	--****************************************************************************************
	-- Перенос данных в основные таблицы
	DELETE FROM DealPaymentDocumentDistributionToSaleWaybill;
	DELETE FROM DealPaymentDocumentDistributionToDealPaymentDocument;
	DELETE FROM DealPaymentDocumentDistribution
	WHERE
		not exists(
			SELECT *
			FROM 
				DealPaymentDocumentDistributionToReturnFromClientWaybill rfc
			WHERE
				rfc.Id = DealPaymentDocumentDistribution.Id
		);
		
	INSERT INTO DealPaymentDocumentDistribution(
		Id,
		Sum,
		CreationDate,
		DistributionDate,
		OrdinalNumber,
		SourceDealPaymentDocumentId)
	SELECT 
		Id,
		Sum,
		CreationDate,
		DistributionDate,
		OrdinalNumber,
		SourceDealPaymentDocumentId
	FROM TMP_DealPaymentDocumentDistribution;

	INSERT INTO DealPaymentDocumentDistributionToDealPaymentDocument(
		Id,
		DestinationDealPaymentDocumentId,
		SourceDistributionToReturnFromClientWaybillId)
	SELECT
		Id,
		DestinationDealPaymentDocumentId,
		SourceDistributionToReturnFromClientWaybillId
	FROM TMP_DealPaymentDocumentDistributionToDealPaymentDocument


	INSERT INTO DealPaymentDocumentDistributionToSaleWaybill(
		Id,
		SaleWaybillId,
		SourceDistributionToReturnFromClientWaybillId)
	SELECT
		Id,
		SaleWaybillId,
		SourceDistributionToReturnFromClientWaybillId
	FROM TMP_DealPaymentDocumentDistributionToSaleWaybill
	--****************************************************************************************

	-- Исправления ошибок
	update DealPaymentDocument
	set IsFullyDistributed = 0
	where DistributedSum <> Sum and IsFullyDistributed = 1

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop function dbo.MaxDate3

drop INDEX IX_PaymentSource on PaymentSource;
drop INDEX IX_Distributions on Distributions;

drop table PaymentSource;
drop table DistributionPlan;
drop table Distributions;

drop table TMP_DealPaymentDocumentDistribution;
drop table TMP_DealPaymentDocumentDistributionToDealPaymentDocument;
drop table TMP_DealPaymentDocumentDistributionToSaleWaybill;

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

