/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.49

  ��� ������:
	+ ���� SourceDistributionToReturnFromClientWaybillId � ������� DealPaymentDocumentDistributionToDealPaymentDocument � DealPaymentDocumentDistributionToSaleWaybill
	* �������������� ����� 
		
---------------------------------------------------------------------------------------*/
--SET NOEXEC OFF	-- ��������� ������ ������� � ������ ����������� ����������
SET DATEFORMAT DMY
SET NOCOUNT ON
SET ARITHABORT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

DECLARE @PreviousVersion varchar(15),	-- ����� ���������� ������
		@CurrentVersion varchar(15),	-- ����� ������� ������ ���� ������
		@NewVersion varchar(15),		-- ����� ����� ������
		@DataBaseName varchar(256),		-- ������� ���� ������
		@CurrentDate nvarchar(10),		-- ������� ����
		@CurrentTime nvarchar(10),		-- ������� �����
		@BackupTarget nvarchar(100)		-- ���� ������ ����� ���� ������

SET @PreviousVersion = '0.9.48' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.49'			-- ����� ����� ������

SELECT @CurrentVersion = DataBaseVersion FROM Setting
IF @@ERROR <> 0
BEGIN
	PRINT '�������� ���� ������'
END
ELSE
BEGIN
	-- ������� ����� ���� ������
	-- �������� ������� ����
	SET @CurrentDate = CONVERT(nvarchar(20), GETDATE(), 104)	--	dd.mm.yyyy
	SET @CurrentTime = REPLACE(CONVERT(nvarchar(20), GETDATE(), 108) , ':', '.') --	hh:mm:ss
	SET @DataBaseName = DB_NAME()

	SET @BackupTarget = N'D:\Bizpulse\Backup\Update\' + @DataBaseName + '(' + CAST(@CurrentVersion as nvarchar(20)) + ') ' + 
		@CurrentDate + ' ' + @CurrentTime + N'.bak'

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = N'���������� ������', NOFORMAT

	IF @@ERROR <> 0
	BEGIN
		PRINT '������ �������� backup''�. ����������� ���������� ����������.'
	END
	ELSE
		BEGIN

		IF (@CurrentVersion <> @PreviousVersion)
		BEGIN
			PRINT '�������� ���� ������ ' + @DataBaseName + ' �� ������ ' + @NewVersion + 
				' ����� ������ �� ������  ' + @PreviousVersion +
				'. ������� ������: ' + @CurrentVersion
		END
		ELSE
		BEGIN
			--�������� ����������
			BEGIN TRAN

			--��������� ������ ���� ������
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
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ��������� ������� ��� ������
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
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ���� ������������� ������� �� ����������
CREATE TABLE DistributionPlan(
	DistributionId uniqueidentifier NOT NULL,
	SourceId uniqueIdentifier NULL,
	Sum decimal(18,2) NOT NULL,
	Date DateTime NOT NULL,
	Id int IDENTITY(1,1) not null,
	primary key (Id)
);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ��������� ����������
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
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
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
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

create table TMP_DealPaymentDocumentDistributionToDealPaymentDocument (
	   Id UNIQUEIDENTIFIER not null,
	   DestinationDealPaymentDocumentId UNIQUEIDENTIFIER not null,
	   SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null,
	   --primary key (Id)
	);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO  

create table TMP_DealPaymentDocumentDistributionToSaleWaybill (
	   Id UNIQUEIDENTIFIER not null,
	   SaleWaybillId UNIQUEIDENTIFIER not null,
	   SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null,
	   --primary key (Id)
	);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table DealPaymentDocumentDistributionToDealPaymentDocument add SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER NULL
GO
alter table DealPaymentDocumentDistributionToSaleWaybill add SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
	

	
	-- ��������� ���� ���������� ��� ��������� ������
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
		

	-- �������� � ��������� ������� ���� ������
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
		
	-- 	�������� � ��������� ������� �������������
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

	-- ��������� � �������� ���������� �������� ������	
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
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	-- Part 2

	declare @distributionNumber int -- ����� ���������� ����������
	declare @distributionId uniqueidentifier	-- ��� ���������� ����������
	declare @distributionSum decimal(18,2)	-- ����� ���������� ����������

	declare @paymentSourceNumber int -- ����� ���������� ���������
	declare @paymentSourceSum decimal(18,2)	-- ����� ���������� ���������

	declare @partSum decimal(18,2)	-- ���������� ����� � ���������� ��������� �� ��������� ����������
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

	-- ���� ������� �� ���� ����������� ������
	while (@distributionNumber is not null)
	begin
		--++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		 -- ����� ������� ��������� �������
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
				 
		-- ��������� ����� ���������
		SELECT 
			@paymentSourceSum = ps.Sum,
			@sourceId = ps.SourceId,
			@date = ps.Date
		FROM	
			PaymentSource ps
		WHERE 
			ps.RowNumber = @paymentSourceNumber

		
		-- ���� ������������� ���������� �� ���������� �������
		while (@paymentSourceSum is not null AND @distributionSum > 0)
		begin
			-- PRINT 'SourceNumber: '+CONVERT(varchar,@paymentSourceNumber);
			
			IF ( @paymentSourceSum > 0)
			begin
				--////////////////////////////////////////////////////////////////////
				IF @distributionSum <= @paymentSourceSum
				-- ��������� ������� ��� ����������
				SET @partSum = @distributionSum
				-- ��������� �� ������� ��� ����������, �������� ��� �������� � ����� ���������
				ELSE SET @partSum = @paymentSourceSum;
				
				-- �������� ������������� ������� �� ����������
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
				
				-- ��������� ����� ����������
				SET @distributionSum = @distributionSum - @partSum;

				-- ��������� ����� ���������
				UPDATE PaymentSource
				SET 
					Sum = Sum - @partSum
				WHERE
					PaymentSource.RowNumber = @paymentSourceNumber		
				--////////////////////////////////////////////////////////////////////
			end
			-- ����� ���������� ��������� �������
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
			-- ��������� ����� ���������� ���������
			SELECT 
				@paymentSourceSum = ps.Sum,
				@sourceId = ps.SourceId,
				@date = ps.Date
			FROM	
				PaymentSource ps
			WHERE 
				ps.RowNumber = @paymentSourceNumber
		end
		
		-- ��������� ����� � ����������
		UPDATE Distributions
		SET 
			Sum = @distributionSum
		WHERE
			Distributions.RowNumber = @distributionNumber
		
		--++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		
		-- �������� ����� ���������� ����������
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

	-- ������� ����������
	declare @Id uniqueidentifier	-- ID
	--declare @DistributionId uniqueidentifier	-- ��� ����������
	--declare @SourceId uniqueIdentifier	-- ��� ��������� ������� ����������
	declare @Sum decimal(18,2)	-- ����� ������� ����������
	--declare @Date DateTime	-- ���� ����������
	declare @PlanId int	-- ��� �������� �������� � ����� ����������
	declare @CreationDate DateTime	-- ���� �������� ����������
	declare @SourceDealPaymentDocumentId uniqueIdentifier	-- ��� ������, �� ������� �������� ����������

	declare @IsDistributionToSaleWaybill int	-- ������� ���������� �� ����������
	declare @SaleWaybillId uniqueIdentifier	-- ��� ����������, �� ������� �������� ����������
	declare @DestinationDealPaymentDocumentId uniqueIdentifier	-- ��� ���������, �� ������� �������� ����������

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
		-- �������� ������ �� ����������
		SELECT 
			@CreationDate = pdd.CreationDate,
			@SourceDealPaymentDocumentId = pdd.SourceDealPaymentDocumentId
		FROM
			DealPaymentDocumentDistribution pdd
		WHERE
			pdd.Id = @DistributionId;
		
		
		-- ���������� ��� ����������
		SELECT @IsDistributionToSaleWaybill = Count(*)
		FROM 
			DealPaymentDocumentDistributionToSaleWaybill swd
		WHERE
			swd.Id = @DistributionId
			
		
		-- ���������� Id
		SET @Id = NEWID();
		
		-- ��������� ����������
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
			,999	-- ��� ����������? ����� ��������� UPDATE?
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
		
		-- ��������� � ���������� ��������
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
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	-- Part 4
	-- ����������� ���������� ���� ����������

	-- ��������� ���� ���������� ��� ���������� �� ����������
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

	-- ��������� ���� ���������� ��� ���������� �� ���������
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
	-- ���������� OrderNumber ��� ����������

	declare @paymentId uniqueidentifier
	SELECT TOP(1)
		@paymentId = d.Id
	FROM 
		DealPaymentDocument d
	ORDER BY d.Id

	-- ���� ������� 
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
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	--****************************************************************************************
	-- ������� ������ � �������� �������
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

	-- ����������� ������
	update DealPaymentDocument
	set IsFullyDistributed = 0
	where DistributedSum <> Sum and IsFullyDistributed = 1

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
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

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

