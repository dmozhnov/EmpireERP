/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.53

  ��� ������:
	* ����c��� ����������� ������������ ���������� �� ������������ � ������� �������
	
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

SET @PreviousVersion = '0.9.52' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.53'			-- ����� ����� ������

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
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ��������� ������ ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

CREATE FUNCTION GetSourceReceiptDate
(
  @sourceWaybillRowId uniqueidentifier,	-- ��� ��������� ������� ���������
  @sourceWaybillTypeId tinyint	-- ��� ���� ��������� ���������
)
RETURNS datetime
WITH SCHEMABINDING
AS
BEGIN
	declare @sourceReceiptDate DateTime -- ���� "�������" ���������
		
	-- �������� ������ � ���� ����������� ��������� � ������ �������
	IF(@sourceWaybillTypeId = 1)	-- ��������� ���������
		SELECT TOP(1) 
			@sourceReceiptDate = 
			CASE
				WHEN rwr.AreCountDivergencesAfterReceipt = 1 OR rwr.AreSumDivergencesAfterReceipt = 1 
					THEN rw.ApprovementDate
					ELSE rw.ReceiptDate
			END
		FROM
			dbo.ReceiptWaybill rw
			join dbo.ReceiptWaybillRow rwr on rwr.ReceiptWaybillId = rw.Id
		WHERE
			rwr.Id = @sourceWaybillRowId
	ELSE IF (@sourceWaybillTypeId = 2)	-- ��������� �����������
		SELECT TOP(1) 
			@sourceReceiptDate = mw.ReceiptDate
		FROM
			dbo.MovementWaybill mw
			join dbo.MovementWaybillRow mwr on mwr.MovementWaybillId = mw.Id
		WHERE
			mwr.Id = @sourceWaybillRowId
	ELSE IF (@sourceWaybillTypeId = 5)	-- ��������� ����� ������������
		SELECT TOP(1) 
			@sourceReceiptDate = cow.ChangeOwnerDate
		FROM
			dbo.ChangeOwnerWaybill cow
			join dbo.ChangeOwnerWaybillRow cowr on cowr.ChangeOwnerWaybillId = cow.Id
		WHERE
			cowr.Id = @sourceWaybillRowId
	ELSE IF (@sourceWaybillTypeId = 6)	-- ��������� �������� ������
		SELECT TOP(1) 
			@sourceReceiptDate = rfcw.ReceiptDate
		FROM
			dbo.ReturnFromClientWaybill rfcw
			join dbo.ReturnFromClientWaybillRow rfcwr on rfcwr.ReturnFromClientWaybillId = rfcw.Id
		WHERE
			rfcwr.Id = @sourceWaybillRowId

	RETURN @sourceReceiptDate
END

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ���������� ���������� � ������� ������������
CREATE PROCEDURE AddValueToAcceptedIndicator 
(
  @date DateTime, 
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @batchId uniqueidentifier,
  @purchaseCost decimal (18,6),
  @count decimal (18,6)
)
AS
BEGIN
	declare @exists int -- ������� ������������� ���������� � ��� �� ������ � �� ���� ����
	declare @keyHash varchar(255) -- ����
	
	SET @keyHash = HashBytes('SHA1', 
			CAST(@storageId as varchar(255)) + '_' +
			CAST(@accountOrganizationId as varchar(255)) + '_' +
			CAST(@articleId as varchar(255)) + '_' +
			CAST(@batchId as varchar(255)))
	
	SET @exists = (
		SELECT 
			COUNT(*)
		FROM 
			#T_accepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
	
	IF (@exists = 0)
		-- ��������� ��������� � �����������
		INSERT INTO #T_accepted(
			Id,
			StartDate,
			EndDate,
			StorageId,
			AccountOrganizationId,
			ArticleId,
			BatchId,
			PurchaseCost,
			Count,
			KeyHash,
			PreviousId
		)
		VALUES(	
			NEWID(),	--Id,
			@date,	--StartDate,
			null,	--EndDate,
			@storageId,	--StorageId,
			@accountOrganizationId,	--AccountOrganizationId,
			@articleId,	--ArticleId,
			@batchId,	--BatchId,
			@purchaseCost,	--PurchaseCost,
			@count,	--Count,
			@keyHash,
			null	--PreviousId			
		)
	ELSE
		-- �������������� ����������
		UPDATE #T_accepted
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date

END

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ���������� ���������� � ������� ������������
CREATE PROCEDURE AddValueToExectIndicator 
(
  @date DateTime, 
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @batchId uniqueidentifier,
  @purchaseCost decimal (18,6),
  @count decimal (18,6)
)
AS
BEGIN
	declare @exists int -- ������� ������������� ���������� � ����� ������ � �� ���� ����
	declare @keyHash varchar(255) -- ����
	
	SET @keyHash = HashBytes('SHA1', 
			CAST(@storageId as varchar(255)) + '_' +
			CAST(@accountOrganizationId as varchar(255)) + '_' +
			CAST(@articleId as varchar(255)) + '_' +
			CAST(@batchId as varchar(255)))
	
	--SET @exists = 0
	SET @exists = (
		SELECT 
			COUNT(*)
		FROM 
			#T_exact t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- ��������� ��������� � ������
		INSERT INTO #T_exact(
			Id,
			StartDate,
			EndDate,
			StorageId,
			AccountOrganizationId,
			ArticleId,
			BatchId,
			PurchaseCost,
			Count,
			KeyHash,
			PreviousId
		)
		VALUES(	
			NEWID(),	--Id,
			@date,	--StartDate,
			null,	--EndDate,
			@storageId,	--StorageId,
			@accountOrganizationId,	--AccountOrganizationId,
			@articleId,	--ArticleId,
			@batchId,	--BatchId,
			@purchaseCost,	--PurchaseCost,
			@count,	--Count,
			@keyHash,
			null	--PreviousId			
		)
	ELSE
		-- �������������� ����������
		UPDATE #T_exact
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

CREATE PROCEDURE InsertIndicator 
(
  @waybillRowId uniqueidentifier,
  @acceptDate DateTime, 
  @receiptDate DateTime,
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @batchId uniqueidentifier
)
AS
BEGIN
	declare @waybillRowArticleMovementId uniqueidentifier -- ��� ������ ����� �������� ������
	declare @sourceReceiptDate DateTime -- ���� "�������" ���������
	declare @purchaseCost decimal (18,6) -- ���������� ����
	declare @minusCount decimal (18,6)
	declare @count decimal (18,6)
	declare @sourceWaybillRowId uniqueidentifier;	-- ��� ��������� ������� ���������
	declare @sourceWaybillTypeId tinyint;	-- ��� ���� ��������� ���������
	
	-- �������� ���������� ����
	SELECT
		@purchaseCost = rwr.PurchaseCost
	FROM
		ReceiptWaybillRow rwr
	WHERE
		rwr.Id = @batchId
			
	-- �������� ������ � ������ ��������� �������		 	
	SET @waybillRowArticleMovementId = (
	SELECT TOP(1)
		wram.Id		
	FROM
		dbo.WaybillRowArticleMovement wram
	WHERE
		wram.DestinationWaybillRowId = @waybillRowId
		ORDER BY wram.Id)
	
	-- �������� ������ �� ����������		
	SELECT
		@sourceWaybillRowId = wram.SourceWaybillRowId,
		@sourceWaybillTypeId = wram.SourceWaybillTypeId,
		@count = wram.MovingCount		
	FROM
		dbo.WaybillRowArticleMovement wram
	WHERE
		wram.Id = @waybillRowArticleMovementId
	
	-- ���� ������� ���� ����������
	while(@waybillRowArticleMovementId is not null)
	begin
	
		SET @minusCount = @count * -1
		 
		SET @sourceReceiptDate = dbo.GetSourceReceiptDate(@sourceWaybillRowId, @sourceWaybillTypeId)	-- �������� ���� ������� ���������

		-- ���� �������� �� ������ �� ������ �������� ���������, ��
		IF(@sourceReceiptDate is null OR @sourceReceiptDate > @acceptDate)
		begin
			-- ��������� ��������� � �����������
			exec AddValueToAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
			-- ���� �������� ��� ������ ����� ��������
			IF (@sourceReceiptDate is not null)
			begin
				-- ��������� ������ �� ������������ � ������
				-- ��������� �����������
				exec AddValueToAcceptedIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
				
				-- ����������� ������
				exec AddValueToExectIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count		
			end
		end
		ELSE
			-- ����� ��������� ��������� � ������
			exec AddValueToExectIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
		
		-- ���� ��������� ��������� �������, �� 
		IF(@receiptDate is not null)
			-- ��������� ������
			exec AddValueToExectIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
			
			
		-- �������� ������ � ������ ��������� �������		 	
		SET @waybillRowArticleMovementId = (
		SELECT TOP(1)
			wram.Id		
		FROM
			dbo.WaybillRowArticleMovement wram
		WHERE
			wram.DestinationWaybillRowId = @waybillRowId
			AND wram.Id > @waybillRowArticleMovementId
			ORDER BY wram.Id)
		
		-- �������� ������ �� ����������		
		SELECT
			@sourceWaybillRowId = wram.SourceWaybillRowId,
			@sourceWaybillTypeId = wram.SourceWaybillTypeId,
			@count = wram.MovingCount		
		FROM
			dbo.WaybillRowArticleMovement wram
		WHERE
			wram.Id = @waybillRowArticleMovementId
	end
END

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������� �������� ���������� ��� ���������� �� �������
create table #T_exact (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   BatchId UNIQUEIDENTIFIER not null,
   PurchaseCost DECIMAL(18, 6) not null,
   Count DECIMAL(18, 6) not null,
   PreviousId UNIQUEIDENTIFIER null,
   KeyHash varchar(255) not null,
   RowNumber int null,
   primary key (Id)
)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

create table #T_accepted (
    Id UNIQUEIDENTIFIER not null,
   StartDate DATETIME not null,
   EndDate DATETIME null,
   StorageId SMALLINT not null,
   AccountOrganizationId INT not null,
   ArticleId INT not null,
   BatchId UNIQUEIDENTIFIER not null,
   PurchaseCost DECIMAL(18, 6) not null,
   Count DECIMAL(18, 6) not null,
   PreviousId UNIQUEIDENTIFIER null,
   KeyHash varchar(255) not null,
   RowNumber int null,
   primary key (Id)
)


-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

CREATE INDEX IX_#T_accepted ON #T_accepted (KeyHash, RowNumber, Id);

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

CREATE INDEX IX_#T_exact ON #T_exact (KeyHash, RowNumber, Id);

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

declare @waybillId uniqueidentifier;	-- ��� ������� ���������
declare @waybillRowId uniqueidentifier;	-- ��� ������� ������� ���������

declare @startDate DateTime -- ���� ������� ���������
declare @storageId smallint -- ��� ��
declare @accountOrganizationId int -- ��� ����������� ��������
declare @articleId int -- ��� ������
declare @batchId uniqueidentifier -- ��� ������

declare @acceptDate DateTime -- ���� �������� ���������
declare @receiptDate DateTime -- ���� "�������" ���������
declare @sourceReceiptDate DateTime -- ���� "�������" ���������

-- ��������� �����������

-- ������ ���������
SET @waybillId = (
SELECT TOP(1) 
	mw.Id		
FROM 
	MovementWaybill mw
WHERE
	mw.DeletionDate is null
	AND mw.AcceptanceDate is not null
	ORDER BY mw.Id)
	
SELECT TOP(1) 
	@storageId = mw.SenderStorageId,
	@accountOrganizationId = mw.SenderId,
	@acceptDate = mw.AcceptanceDate,
	@receiptDate = mw.ReceiptDate
FROM 
	MovementWaybill mw
WHERE
	mw.Id = @waybillId

-- ���� �� ��������� 	
WHILE (@waybillId is not null)
begin
	-- ������ ������� ���������
	SET @waybillRowId = (
	SELECT TOP(1) 
		mwr.Id
	FROM 
		MovementWaybillRow mwr
	WHERE
		mwr.DeletionDate is null
		AND mwr.MovementWaybillId = @waybillId
		ORDER BY mwr.Id)

	SELECT TOP(1) 
		@articleId = mwr.ArticleId,
		@batchId = mwr.ReceiptWaybillRowId
	FROM 
		MovementWaybillRow mwr
	WHERE
		mwr.Id = @waybillRowId	

	-- ���� ��������� ���� �������
	WHILE(@waybillRowId is not null)
	begin
		-- ��������� ������ �� �������			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- ��������� ������� ���������
		SET @waybillRowId = (
		SELECT TOP(1) 
			mwr.Id
		FROM 
			MovementWaybillRow mwr
		WHERE
			mwr.DeletionDate is null
			AND mwr.MovementWaybillId = @waybillId
			AND mwr.Id > @waybillRowId
			ORDER BY mwr.Id)

		SELECT TOP(1) 
			@articleId = mwr.ArticleId,
			@batchId = mwr.ReceiptWaybillRowId
		FROM 
			MovementWaybillRow mwr
		WHERE
			mwr.Id = @waybillRowId		
	end
	
	-- ��������� ���������
	SET @waybillId = (
	SELECT TOP(1) 
		mw.Id		
	FROM 
		MovementWaybill mw
	WHERE
		mw.DeletionDate is null
		AND mw.AcceptanceDate is not null
		AND mw.Id > @waybillId
		ORDER BY mw.Id)
		
	SELECT TOP(1) 
		@storageId = mw.SenderStorageId,
		@accountOrganizationId = mw.SenderId,
		@acceptDate = mw.AcceptanceDate,
		@receiptDate = mw.ReceiptDate
	FROM 
		MovementWaybill mw
	WHERE
		mw.Id = @waybillId
end


-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

declare @waybillId uniqueidentifier;	-- ��� ������� ���������
declare @waybillRowId uniqueidentifier;	-- ��� ������� ������� ���������

declare @startDate DateTime -- ���� ������� ���������
declare @storageId smallint -- ��� ��
declare @accountOrganizationId int -- ��� ����������� ��������
declare @articleId int -- ��� ������
declare @batchId uniqueidentifier -- ��� ������

declare @acceptDate DateTime -- ���� �������� ���������
declare @receiptDate DateTime -- ���� "�������" ���������
declare @sourceReceiptDate DateTime -- ���� "�������" ���������

-- ��������� ����� �������������

-- ������ ���������
SET @waybillId = (
SELECT TOP(1) 
	cow.Id		
FROM 
	ChangeOwnerWaybill cow
WHERE
	cow.DeletionDate is null
	AND cow.AcceptanceDate is not null
	ORDER BY cow.Id)
	
SELECT TOP(1) 
	@storageId = cow.ChangeOwnerWaybillStorageId,
	@accountOrganizationId = cow.ChangeOwnerWaybillSenderId,
	@acceptDate = cow.AcceptanceDate,
	@receiptDate = cow.ChangeOwnerDate
FROM 
	ChangeOwnerWaybill cow
WHERE
	cow.Id = @waybillId

-- ���� �� ��������� 	
WHILE (@waybillId is not null)
begin
	-- ������ ������� ���������
	SET @waybillRowId = (
	SELECT TOP(1) 
		cowr.Id
	FROM 
		ChangeOwnerWaybillRow cowr
	WHERE
		cowr.DeletionDate is null
		AND cowr.ChangeOwnerWaybillId = @waybillId
		ORDER BY cowr.Id)

	SELECT TOP(1) 
		@articleId = cowr.ArticleId,
		@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId
	FROM 
		ChangeOwnerWaybillRow cowr
	WHERE
		cowr.Id = @waybillRowId	

	-- ���� ��������� ���� �������
	WHILE(@waybillRowId is not null)
	begin		
		-- ��������� ������ �� �������			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- ��������� ������� ���������
		SET @waybillRowId = (
		SELECT TOP(1) 
			cowr.Id
		FROM 
			ChangeOwnerWaybillRow cowr
		WHERE
			cowr.DeletionDate is null
			AND cowr.ChangeOwnerWaybillId = @waybillId
			AND cowr.Id > @waybillRowId
			ORDER BY cowr.Id)

		SELECT TOP(1) 
			@articleId = cowr.ArticleId,
			@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId
		FROM 
			ChangeOwnerWaybillRow cowr
		WHERE
			cowr.Id = @waybillRowId			
	end
	
	-- ��������� ���������
	SET @waybillId = (
	SELECT TOP(1) 
		cow.Id		
	FROM 
		ChangeOwnerWaybill cow
	WHERE
		cow.DeletionDate is null
		AND cow.AcceptanceDate is not null
		AND cow.Id > @waybillId
		ORDER BY cow.Id)
		
	SELECT TOP(1) 
		@storageId = cow.ChangeOwnerWaybillStorageId,
		@accountOrganizationId = cow.ChangeOwnerWaybillSenderId,
		@acceptDate = cow.AcceptanceDate,
		@receiptDate = cow.ChangeOwnerDate
	FROM 
		ChangeOwnerWaybill cow
	WHERE
		cow.Id = @waybillId
end

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


declare @waybillId uniqueidentifier;	-- ��� ������� ���������
declare @waybillRowId uniqueidentifier;	-- ��� ������� ������� ���������

declare @startDate DateTime -- ���� ������� ���������
declare @storageId smallint -- ��� ��
declare @accountOrganizationId int -- ��� ����������� ��������
declare @articleId int -- ��� ������
declare @batchId uniqueidentifier -- ��� ������

declare @acceptDate DateTime -- ���� �������� ���������
declare @receiptDate DateTime -- ���� "�������" ���������
declare @sourceReceiptDate DateTime -- ���� "�������" ���������

-- ��������� ��������

-- ������ ���������
SET @waybillId = (
SELECT TOP(1) 
	ww.Id		
FROM 
	WriteoffWaybill ww
WHERE
	ww.DeletionDate is null
	AND ww.AcceptanceDate is not null
	ORDER BY ww.Id)
	
SELECT TOP(1) 
	@storageId = ww.WriteoffWaybillSenderStorageId,
	@accountOrganizationId = ww.WriteoffWaybillSenderId,
	@acceptDate = ww.AcceptanceDate,
	@receiptDate = ww.WriteoffDate
FROM 
	WriteoffWaybill ww
WHERE
	ww.Id = @waybillId

-- ���� �� ��������� 	
WHILE (@waybillId is not null)
begin
	-- ������ ������� ���������
	SET @waybillRowId = (
	SELECT TOP(1) 
		wwr.Id
	FROM 
		WriteoffWaybillRow wwr
	WHERE
		wwr.DeletionDate is null
		AND wwr.WriteoffWaybillId = @waybillId
		ORDER BY wwr.Id)

	SELECT TOP(1) 
		@articleId = wwr.ArticleId,
		@batchId = wwr.WriteoffReceiptWaybillRowId
	FROM 
		WriteoffWaybillRow wwr
	WHERE
		wwr.Id = @waybillRowId	

	-- ���� ��������� ���� �������
	WHILE(@waybillRowId is not null)
	begin		
		-- ��������� ������ �� �������			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- ��������� ������� ���������
		SET @waybillRowId = (
		SELECT TOP(1) 
			wwr.Id
		FROM 
			WriteoffWaybillRow wwr
		WHERE
			wwr.DeletionDate is null
			AND wwr.WriteoffWaybillId = @waybillId
			AND wwr.Id > @waybillRowId
			ORDER BY wwr.Id)

		SELECT TOP(1) 
			@articleId = wwr.ArticleId,
			@batchId = wwr.WriteoffReceiptWaybillRowId
		FROM 
			WriteoffWaybillRow wwr
		WHERE
			wwr.Id = @waybillRowId			
	end
	
	-- ��������� ���������
	SET @waybillId = (
	SELECT TOP(1) 
		ww.Id		
	FROM 
		WriteoffWaybill ww
	WHERE
		ww.DeletionDate is null
		AND ww.AcceptanceDate is not null
		AND ww.id > @waybillId
		ORDER BY ww.Id)
		
	SELECT TOP(1) 
		@storageId = ww.WriteoffWaybillSenderStorageId,
		@accountOrganizationId = ww.WriteoffWaybillSenderId,
		@acceptDate = ww.AcceptanceDate,
		@receiptDate = ww.WriteoffDate
	FROM 
		WriteoffWaybill ww
	WHERE
		ww.Id = @waybillId
end

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


declare @waybillId uniqueidentifier;	-- ��� ������� ���������
declare @waybillRowId uniqueidentifier;	-- ��� ������� ������� ���������

declare @startDate DateTime -- ���� ������� ���������
declare @storageId smallint -- ��� ��
declare @accountOrganizationId int -- ��� ����������� ��������
declare @articleId int -- ��� ������
declare @batchId uniqueidentifier -- ��� ������

declare @acceptDate DateTime -- ���� �������� ���������
declare @receiptDate DateTime -- ���� "�������" ���������
declare @sourceReceiptDate DateTime -- ���� "�������" ���������

-- ��������� ����������

-- ������ ���������
SET @waybillId = (
SELECT TOP(1) 
	ew.Id		
FROM 
	ExpenditureWaybill ew
	join SaleWaybill sw on sw.Id = ew.Id
WHERE
	sw.DeletionDate is null
	AND ew.AcceptanceDate is not null
	ORDER BY ew.Id)
	
SELECT TOP(1) 
	@storageId = ew.ExpenditureWaybillSenderStorageId,
	@accountOrganizationId = c.AccountOrganizationId,
	@acceptDate = ew.AcceptanceDate,
	@receiptDate = ew.ShippingDate
FROM 
	ExpenditureWaybill ew
	join SaleWaybill sw on sw.Id = ew.Id
	join Deal d on d.Id = sw.DealId
	join Contract c on c.Id = d.ClientContractId
WHERE
	ew.Id = @waybillId

-- ���� �� ��������� 	
WHILE (@waybillId is not null)
begin
	-- ������ ������� ���������
	SET @waybillRowId = (
	SELECT TOP(1) 
		ewr.Id
	FROM 
		ExpenditureWaybillRow ewr
		join SalewaybillRow swr on swr.Id = ewr.Id
	WHERE
		swr.DeletionDate is null
		AND swr.SaleWaybillId = @waybillId
		ORDER BY ewr.Id)

	SELECT TOP(1) 
		@articleId = swr.ArticleId,
		@batchId = ewr.ExpenditureWaybillRowReceiptWaybillRowId
	FROM 
		ExpenditureWaybillRow ewr
		join SalewaybillRow swr on swr.Id = ewr.Id
	WHERE
		ewr.Id = @waybillRowId	

	-- ���� ��������� ���� �������
	WHILE(@waybillRowId is not null)
	begin		
		-- ��������� ������ �� �������			
		exec InsertIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
		
		-- ��������� ������� ���������
		SET @waybillRowId = (
		SELECT TOP(1) 
			ewr.Id
		FROM 
			ExpenditureWaybillRow ewr
			join SalewaybillRow swr on swr.Id = ewr.Id
		WHERE
			swr.DeletionDate is null
			AND swr.SaleWaybillId = @waybillId
			AND ewr.Id > @waybillRowId
			ORDER BY ewr.Id)

		SELECT TOP(1) 
			@articleId = swr.ArticleId,
			@batchId = ewr.ExpenditureWaybillRowReceiptWaybillRowId
		FROM 
			ExpenditureWaybillRow ewr
			join SalewaybillRow swr on swr.Id = ewr.Id
		WHERE
			ewr.Id = @waybillRowId			
	end
	
	-- ��������� ���������
	SET @waybillId = (
	SELECT TOP(1) 
		ew.Id		
	FROM 
		ExpenditureWaybill ew
		join SaleWaybill sw on sw.Id = ew.Id
	WHERE
		sw.DeletionDate is null
		AND ew.AcceptanceDate is not null
		AND ew.Id > @waybillId
		ORDER BY ew.Id)
		
	SELECT TOP(1) 
		@storageId = ew.ExpenditureWaybillSenderStorageId,
		@accountOrganizationId = c.AccountOrganizationId,
		@acceptDate = ew.AcceptanceDate,
		@receiptDate = ew.ShippingDate
	FROM 
		ExpenditureWaybill ew
		join SaleWaybill sw on sw.Id = ew.Id
		join Deal d on d.Id = sw.DealId
		join Contract c on c.Id = d.ClientContractId
	WHERE
		ew.Id = @waybillId
end

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ����������� ���������� ������ ����� � �����������
UPDATE #T_accepted
	SET RowNumber = d.rn
FROM (
	SELECT 
		ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
		t.KeyHash,
		t.Id
	FROM  #T_accepted t) d
WHERE
	#T_accepted.KeyHash = d.KeyHash
	AND #T_accepted.Id = d.Id
	
-- ����������� ���������� ������ ����� � �����������
UPDATE #T_exact
	SET RowNumber = d.rn
FROM (
	SELECT 
		ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
		t.KeyHash,
		t.Id
	FROM  #T_exact t) d
WHERE
	#T_exact.KeyHash = d.KeyHash
	AND #T_exact.Id = d.Id
	

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ��������� ���������� �� ������
declare @indicatorId uniqueidentifier --	��� ����������
declare @startDateNext DateTime	--	���� ������ �������� ����������
declare @countNext decimal(18,6)	--	���������� ����������
declare @indicatorNextId uniqueidentifier -- ��� ����������
declare @keyHash varchar(255) -- ���� �������� ����������
declare @rowNumber int -- ���������� ����� �������� ����������
declare @count decimal (18,6) -- ���������� ������

-- ��������� ���������� ������������
-- ������ ����
SET @keyHash =(
SELECT TOP(1)
	t.KeyHash
FROM 
	#T_accepted t
	ORDER BY t.KeyHash)
	
WHILE(@keyHash is not null)
begin
	SET @indicatorId = null
	-- ������ ���������
	SELECT TOP(1) 
		@indicatorId = t.Id,
		@count = t.Count,
		@keyHash = t.KeyHash,
		@rowNumber = t.RowNumber
	FROM 
		#T_accepted t
	WHERE 
		t.KeyHash = @keyHash
	ORDER BY t.RowNumber

	WHILE( @indicatorId is not null)
	begin
		-- �������� ����������
		SET @startDateNext = null
		SET @countNext = null
		SET @indicatorNextId = null
		
		-- �������� ������ �� ���������� ����������
		SELECT 
			@startDateNext = t.StartDate,
			@countNext = t.Count,
			@indicatorNextId = t.Id
		FROM 
			#T_accepted t 
		WHERE 
			t.KeyHash = @keyHash 
			AND t.RowNumber = @rowNumber + 1
		
		-- ��������� ����������
		UPDATE #T_accepted
			SET EndDate = @startDateNext
		WHERE Id = @indicatorId
		
		-- ��������� ���������
		UPDATE #T_accepted
			SET Count = @countNext + @count,
				PreviousId = @indicatorId
		WHERE Id = @indicatorNextId
		
		-- ��������� � ����������
		SET @indicatorId = null	-- �� ������, ����� ������� ����� ������
		-- ������ ���������
		SELECT TOP(1) 
			@indicatorId = t.Id,
			@count = t.Count,
			@keyHash = t.KeyHash,
			@rowNumber = t.RowNumber
		FROM 
			#T_accepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.RowNumber = @rowNumber + 1
	end
	-- ��������� ����
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_accepted t
	WHERE
		t.KeyHash > @keyHash
		ORDER BY t.KeyHash)
end

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ��������� ���������� �� ������
declare @indicatorId uniqueidentifier --	��� ����������
declare @startDateNext DateTime	--	���� ������ �������� ����������
declare @countNext decimal(18,6)	--	���������� ����������
declare @indicatorNextId uniqueidentifier -- ��� ����������
declare @keyHash varchar(255) -- ���� �������� ����������
declare @rowNumber int -- ���������� ����� �������� ����������
declare @count decimal (18,6) -- ���������� ������

-- ��������� ���������� �������

-- ������ ����
SET @keyHash =(
SELECT TOP(1)
	t.KeyHash
FROM 
	#T_exact t
	ORDER BY t.KeyHash)
	
WHILE(@keyHash is not null)
begin
	SET @indicatorId = null
	-- ������ ���������
	SELECT TOP(1) 
		@indicatorId = t.Id,
		@count = t.Count,
		@rowNumber = t.RowNumber
	FROM 
		#T_exact t
	WHERE
		t.KeyHash = @keyHash
	ORDER BY t.RowNumber

	WHILE( @indicatorId is not null)
	begin
		-- �������� ����������
		SET @startDateNext = null
		SET @countNext = null
		SET @indicatorNextId = null
		
		-- �������� ������ �� ���������� ����������
		SELECT 
			@startDateNext = t.StartDate,
			@countNext = t.Count,
			@indicatorNextId = t.Id
		FROM 
			#T_exact t 
		WHERE 
			t.KeyHash = @keyHash 
			AND t.RowNumber = @rowNumber + 1
		
		-- ��������� ����������
		UPDATE #T_exact
			SET EndDate = @startDateNext
		WHERE Id = @indicatorId
		
		-- ��������� ���������
		UPDATE #T_exact
			SET Count = @countNext + @count,
				PreviousId = @indicatorId
		WHERE Id = @indicatorNextId
		
		-- ��������� � ����������
		SET @indicatorId = null	-- �� ������, ����� ������� ����� ������
		-- ������ ���������
		SELECT TOP(1) 
			@indicatorId = t.Id,
			@count = t.Count,
			@rowNumber = t.RowNumber
		FROM 
			#T_exact t
		WHERE
			t.KeyHash = @keyHash
			AND t.RowNumber = @rowNumber + 1
	end
	
	-- ��������� ����
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_exact t
	WHERE
		t.KeyHash > @keyHash
		ORDER BY t.KeyHash)
end

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ��������� ������ �� ��������� ������ � ��������

DELETE FROM OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

INSERT INTO OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator (
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
)
SELECT 
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
FROM #T_accepted
		
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

DELETE FROM OutgoingAcceptedFromExactArticleAvailabilityIndicator

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

INSERT INTO OutgoingAcceptedFromExactArticleAvailabilityIndicator (
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
)
SELECT 
	Id,
	StartDate,
	EndDate,
	StorageId,
	AccountOrganizationId,
	ArticleId,
	BatchId,
	PurchaseCost,
	Count,
	PreviousId
FROM #T_exact

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop index IX_#T_accepted on #T_accepted

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop index IX_#T_exact on #T_exact

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #T_exact

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #T_accepted

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop function dbo.GetSourceReceiptDate

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop procedure dbo.AddValueToAcceptedIndicator

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop procedure dbo.AddValueToExectIndicator

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop procedure dbo.InsertIndicator

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO