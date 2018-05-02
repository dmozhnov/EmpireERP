-- ��������� ���� �������� ���������
CREATE FUNCTION GetWaybillReceiptDate
(
  @waybillRowId uniqueidentifier,	-- ��� ��������� ������� ���������
  @waybillTypeId tinyint	-- ��� ���� ��������� ���������
)
RETURNS datetime
WITH SCHEMABINDING
AS
BEGIN
	declare @receiptDate DateTime -- ���� "�������" ���������
		
	-- �������� ������ � ���� ����������� ��������� � ������ �������
	IF(@waybillTypeId = 1)	-- ��������� ���������
		SELECT TOP(1) 
			@receiptDate = 
			CASE
				WHEN rwr.AreCountDivergencesAfterReceipt = 1 OR rwr.AreSumDivergencesAfterReceipt = 1 
					THEN rw.ApprovementDate
					ELSE rw.ReceiptDate
			END
		FROM
			dbo.ReceiptWaybill rw
			join dbo.ReceiptWaybillRow rwr on rwr.ReceiptWaybillId = rw.Id
		WHERE
			rwr.Id = @waybillRowId
	ELSE IF (@waybillTypeId = 2)	-- ��������� �����������
		SELECT TOP(1) 
			@receiptDate = mw.ReceiptDate
		FROM
			dbo.MovementWaybill mw
			join dbo.MovementWaybillRow mwr on mwr.MovementWaybillId = mw.Id
		WHERE
			mwr.Id = @waybillRowId
	ELSE IF (@waybillTypeId = 5)	-- ��������� ����� ������������
		SELECT TOP(1) 
			@receiptDate = cow.ChangeOwnerDate
		FROM
			dbo.ChangeOwnerWaybill cow
			join dbo.ChangeOwnerWaybillRow cowr on cowr.ChangeOwnerWaybillId = cow.Id
		WHERE
			cowr.Id = @waybillRowId
	ELSE IF (@waybillTypeId = 6)	-- ��������� �������� ������
		SELECT TOP(1) 
			@receiptDate = rfcw.ReceiptDate
		FROM
			dbo.ReturnFromClientWaybill rfcw
			join dbo.ReturnFromClientWaybillRow rfcwr on rfcwr.ReturnFromClientWaybillId = rfcw.Id
		WHERE
			rfcwr.Id = @waybillRowId

	RETURN @receiptDate
END
GO

-- ���������� ���������� � ������� ������������ ���������� �� ������������
CREATE PROCEDURE AddValueToOutgoingFromAcceptedIndicator 
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
	
	SET @exists = (
		SELECT 
			COUNT(*)
		FROM 
			#T_OutgoingFromAccepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
	
	IF (@exists = 0)
		-- ��������� ��������� � �����������
		INSERT INTO #T_OutgoingFromAccepted(
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
		UPDATE #T_OutgoingFromAccepted
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date

END
GO

-- ���������� ���������� � ������� ���������� ������������ �� �������
CREATE PROCEDURE AddValueToOutgoingFromExectIndicator 
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
	declare @exists int -- ������� ������������� ���������� � ��� �� ������ � �� �� �� ����
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
			#T_OutgoingFromExact t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- ��������� ��������� � ������
		INSERT INTO #T_OutgoingFromExact(
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
		UPDATE #T_OutgoingFromExact
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END
GO


-- ���������� ���������� � ������� ������� �������
CREATE PROCEDURE AddValueToExactIndicator 
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
	declare @exists int -- ������� ������������� ���������� � ��� �� ������ � �� �� �� ����
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
			#T_Exact t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- ��������� ��������� � ������ �������
		INSERT INTO #T_Exact(
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
		UPDATE #T_Exact
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END
GO


-- ���������� ���������� � ������� ������������ ��������� �������
CREATE PROCEDURE AddValueToIncomingAcceptedIndicator 
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
	declare @exists int -- ������� ������������� ���������� � ��� �� ������ � �� �� �� ����
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
			#T_IncomingAccepted t
		WHERE
			t.KeyHash = @keyHash
			AND t.StartDate = @date
	)
		
	IF (@exists = 0)
		-- ��������� ��������� � ����������� ��������
		INSERT INTO #T_IncomingAccepted(
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
		UPDATE #T_IncomingAccepted
			SET Count = Count + @count
		WHERE
			KeyHash = @keyHash
			AND StartDate = @date
END
GO

-- ������� �������� �����������
CREATE PROCEDURE InsertIncomingIndicator 
(
  @waybillRowId uniqueidentifier,
  @acceptDate DateTime, 
  @receiptDate DateTime,
  @storageId smallint,
  @accountOrganizationId int,
  @articleId int,
  @count decimal(18,6),
  @batchId uniqueidentifier
)
AS
BEGIN
	declare @waybillRowArticleMovementId uniqueidentifier -- ��� ������ ����� �������� ������
	declare @destinationReceiptDate DateTime -- ���� "�������" ���������
	declare @purchaseCost decimal (18,6) -- ���������� ����
	declare @destinationCount decimal (18,6)
	declare @destinationWaybillRowId uniqueidentifier;	-- ��� ��������� �������
	declare @destinationWaybillTypeId tinyint;	-- ��� ���� ��������� ���������
	declare @minusCount decimal(18,6)	-- ������������� �������� ���������� ������
	-- �������� ���������� ����
	SELECT
		@purchaseCost = rwr.PurchaseCost
	FROM
		ReceiptWaybillRow rwr
	WHERE
		rwr.Id = @batchId

	-- ����������� �� �������� ����������� �������
	exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
	IF (@receiptDate is not null)	-- ���� ������� �������
	BEGIN
		SET @minusCount = @count * -1
		-- ��������� �� ������������ ��������� � ������
		exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@minusCount
		exec AddValueToExactIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
	END
END
GO

-- ������� ��������� �����������
CREATE PROCEDURE InsertOutgoingIndicator 
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
	
	-- �������� ������ �� ���������		
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
		 
		SET @sourceReceiptDate = dbo.GetWaybillReceiptDate(@sourceWaybillRowId, @sourceWaybillTypeId)	-- �������� ���� ������� ���������

		-- ���� �������� �� ������ �� ������ �������� ���������, ��
		IF(@sourceReceiptDate is null OR @sourceReceiptDate > @acceptDate)
		begin
			-- ��������� ��������� � �����������
			exec AddValueToOutgoingFromAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
			-- ���� �������� ��� ������ ����� ��������
			IF (@sourceReceiptDate is not null)
			begin
				-- ��������� ������ �� ������������ � ������
				-- ��������� �����������
				exec AddValueToOutgoingFromAcceptedIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
				
				-- ����������� ������
				exec AddValueToOutgoingFromExectIndicator @sourceReceiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count		
			end
		end
		ELSE
			-- ����� ��������� ��������� � ������
			exec AddValueToOutgoingFromExectIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
		
		-- ���� ��������� ��������� �������, �� 
		IF(@receiptDate is not null)
		BEGIN
			-- ��������� ������
			exec AddValueToOutgoingFromExectIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
			exec AddValueToExactIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost, @minusCount
		END
			
			
		-- �������� ������ � ��������� ��������� �������		 	
		SET @waybillRowArticleMovementId = (
		SELECT TOP(1)
			wram.Id		
		FROM
			dbo.WaybillRowArticleMovement wram
		WHERE
			wram.DestinationWaybillRowId = @waybillRowId
			AND wram.Id > @waybillRowArticleMovementId
			ORDER BY wram.Id)
		
		-- �������� ������ �� ���������		
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
GO

-- ������� �������� ���������� ��� ���������� �� �������
create table #T_OutgoingFromExact (
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
GO
-- ������� �������� ���������� ��� ���������� �� ������������
create table #T_OutgoingFromAccepted (
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
GO

-- ������� �������� ���������� ��� ������� �������
create table #T_Exact (
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
GO

-- ������� �������� ���������� ��� ������������ ���������
create table #T_IncomingAccepted (
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
GO

-- �������, ���������� ���� ������������ �����������
CREATE TABLE #Diff(
	OldId UNIQUEIDENTIFIER not null,
	NewId UNIQUEIDENTIFIER not null
)

CREATE INDEX IX_#T_OutgoingFromAccepted ON #T_OutgoingFromAccepted (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

CREATE INDEX IX_#T_OutgoingFromExact ON #T_OutgoingFromExact (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

CREATE INDEX IX_#T_Exact ON #T_Exact (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

CREATE INDEX IX_#T_IncomingAccepted ON #T_IncomingAccepted (KeyHash, RowNumber, Id, storageId, accountOrganizationId, articleId, batchId);
GO

BEGIN TRY
	BEGIN TRAN
	
	SET NOCOUNT ON
					
	declare @waybillId uniqueidentifier;	-- ��� ������� ���������
	declare @waybillRowId uniqueidentifier;	-- ��� ������� ������� ���������

	declare @startDate DateTime -- ���� ������� ���������
	declare @storageId smallint -- ��� ��
	declare @accountOrganizationId int -- ��� ����������� ��������
	declare @recipientAccountOrganizationId int -- ��� ����������� ��������
	declare @articleId int -- ��� ������
	declare @articleCount decimal(18,6) -- ���������� ������
	declare @batchId uniqueidentifier -- ��� ������

	declare @acceptDate DateTime -- ���� �������� ���������
	declare @receiptDate DateTime -- ���� "�������" ���������
	declare @approvedDate DateTime -- ���� "������������" ��������� �������
	declare @sourceReceiptDate DateTime -- ���� "�������" ���������
	declare @count decimal (18,6) -- ���������� ������
	declare @recipientStorageId smallint -- ��� ��
	
	PRINT('������ ����� ��������')
	
	-- 1) ��������� �����������
	PRINT('   1) ��������� �����������')
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
		@recipientStorageId = mw.RecipientStorageId,
		@accountOrganizationId = mw.SenderId,
		@recipientAccountOrganizationId = mw.RecipientId,
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
			@articleCount = mwr.MovingCount,
			@batchId = mwr.ReceiptWaybillRowId
		FROM 
			MovementWaybillRow mwr
		WHERE
			mwr.Id = @waybillRowId	

		-- ���� ��������� ���� �������
		WHILE(@waybillRowId is not null)
		begin
			-- ��������� ������ �� �������			
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			exec InsertIncomingIndicator @waybillRowId,@acceptDate,@receiptDate,@recipientStorageId,@recipientAccountOrganizationId,@articleId,@articleCount,@batchId

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
				@articleCount = mwr.MovingCount,
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
			@recipientStorageId = mw.RecipientStorageId,
			@accountOrganizationId = mw.SenderId,
			@recipientAccountOrganizationId = mw.RecipientId,
			@acceptDate = mw.AcceptanceDate,
			@receiptDate = mw.ReceiptDate
		FROM 
			MovementWaybill mw
		WHERE
			mw.Id = @waybillId
	end
	
	-- 2) ��������� ����� ������������
	PRINT('   2) ��������� ����� ������������')
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
		@recipientAccountOrganizationId = cow.ChangeOwnerWaybillRecipientId,
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
			@articleCount = cowr.MovingCount,
			@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId,
			@count = cowr.MovingCount
		FROM 
			ChangeOwnerWaybillRow cowr
		WHERE
			cowr.Id = @waybillRowId	

		-- ���� ��������� ���� �������
		WHILE(@waybillRowId is not null)
		begin		
			-- ��������� ������ �� �������			
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			exec InsertIncomingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@recipientAccountOrganizationId,@articleId,@articleCount,@batchId
			
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
				@articleCount = cowr.MovingCount,
				@batchId = cowr.ChangeOwnerWaybillRowReceiptWaybillRowId,
				@count = cowr.MovingCount
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
			@recipientAccountOrganizationId = cow.ChangeOwnerWaybillRecipientId,
			@acceptDate = cow.AcceptanceDate,
			@receiptDate = cow.ChangeOwnerDate
		FROM 
			ChangeOwnerWaybill cow
		WHERE
			cow.Id = @waybillId
	end


	-- 3) ��������� ��������
	PRINT('   3) ��������� ��������')
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
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			
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


	-- 4) ��������� ����������
	PRINT('   4) ��������� ����������')
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
			exec InsertOutgoingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId
			
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

	-- 5) ��������� �������� ������
	PRINT('   5) ��������� �������� ������')
	-- ������ ���������
	SET @waybillId = (
	SELECT TOP(1) 
		Id
	FROM 
		ReturnFromClientWaybill
	WHERE
		DeletionDate is null
		AND AcceptanceDate is not null
		ORDER BY Id)
		
	SELECT TOP(1) 
		@storageId = rfcw.ReturnFromClientWaybillRecipientStorageId,
		@accountOrganizationId = c.AccountOrganizationId,
		@acceptDate = rfcw.AcceptanceDate,
		@receiptDate = rfcw.ReceiptDate
	FROM 
		ReturnFromClientWaybill rfcw
		join Deal d on d.Id = rfcw.ReturnFromClientWaybillDealId
		join Contract c on c.Id = d.ClientContractId
	WHERE
		rfcw.Id = @waybillId

	-- ���� �� ��������� 	
	WHILE (@waybillId is not null)
	begin
		-- ������ ������� ���������
		SET @waybillRowId = (
		SELECT TOP(1) Id
		FROM ReturnFromClientWaybillRow
		WHERE
			DeletionDate is null
			AND ReturnFromClientWaybillId = @waybillId
			ORDER BY Id)

		SELECT TOP(1) 
			@articleId = ArticleId,
			@articleCount = ReturnCount,
			@batchId = ReceiptWaybillRowId
		FROM 
			ReturnFromClientWaybillRow
		WHERE
			Id = @waybillRowId	

		-- ���� ��������� ���� �������
		WHILE(@waybillRowId is not null)
		begin		
			-- ��������� ������ �� �������			
			exec InsertIncomingIndicator @waybillRowId,@acceptDate,@receiptDate,@storageId,@accountOrganizationId,@articleId,@articleCount,@batchId
			
			-- ��������� ������� ���������
			SET @waybillRowId = (
			SELECT TOP(1) 
				Id
			FROM ReturnFromClientWaybillRow
			WHERE
				DeletionDate is null
				AND ReturnFromClientWaybillId = @waybillId
				AND Id > @waybillRowId
				ORDER BY Id)

			SELECT TOP(1) 
				@articleId = ArticleId,
				@articleCount = ReturnCount,
				@batchId = ReceiptWaybillRowId
			FROM 
				ReturnFromClientWaybillRow
			WHERE
				Id = @waybillRowId			
		end
		
		-- ��������� ���������
		SET @waybillId = (
		SELECT TOP(1) 
			Id
		FROM 
			ReturnFromClientWaybill
		WHERE
			DeletionDate is null
			AND AcceptanceDate is not null
			AND Id > @waybillId
			ORDER BY Id)
			
		SELECT TOP(1) 
			@storageId = rfcw.ReturnFromClientWaybillRecipientStorageId,
			@accountOrganizationId = c.AccountOrganizationId,
			@acceptDate = rfcw.AcceptanceDate,
			@receiptDate = rfcw.ReceiptDate
		FROM 
			ReturnFromClientWaybill rfcw
			join Deal d on d.Id = rfcw.ReturnFromClientWaybillDealId
			join Contract c on c.Id = d.ClientContractId
		WHERE
			rfcw.Id = @waybillId
	end
	
	-- 6) ��������� ���������
	PRINT('   6) ��������� ���������')
	declare @areDivergencesAfterReceipt bit -- ������� ����������� ����� �������
	declare @added bit -- ������� ���������� ������� ��� ��������
	declare @purchaseCost decimal(18,6)
	declare @pendingCount decimal(18,6)
	declare @receiptedCount decimal(18,6)
	declare @approvedCount decimal(18,6)
	
	-- ������ ���������
	SET @waybillId = (
	SELECT TOP(1) 
		Id
	FROM 
		ReceiptWaybill
	WHERE
		DeletionDate is null
		AND AcceptanceDate is not null
		ORDER BY Id)
		
	SELECT TOP(1) 
		@storageId = ReceiptWaybillReceiptStorageId,
		@accountOrganizationId = AccountOrganizationId,
		@acceptDate = AcceptanceDate,
		@receiptDate = ReceiptDate,
		@approvedDate = ApprovementDate
	FROM 
		ReceiptWaybill
	WHERE
		Id = @waybillId

	-- ���� �� ��������� 	
	WHILE (@waybillId is not null)
	begin
		-- ������ ������� ���������
		SET @waybillRowId = (
		SELECT TOP(1) Id
		FROM ReceiptWaybillRow
		WHERE
			DeletionDate is null AND ReceiptWaybillId = @waybillId
			ORDER BY Id)

		SELECT TOP(1) 
			@articleId = ArticleId,
			@pendingCount = PendingCount,
			@receiptedCount = ReceiptedCount,
			@approvedCount = ApprovedCount,
			@areDivergencesAfterReceipt = CASE WHEN (AreCountDivergencesAfterReceipt = 1 OR AreSumDivergencesAfterReceipt = 1) THEN 1 ELSE 0 END,
			@added = CASE WHEN PendingCount = 0 THEN 1 ELSE 0 END,
			@batchId = Id
		FROM 
			ReceiptWaybillRow
		WHERE
			Id = @waybillRowId	
		
		-- ���� ��������� ���� �������
		WHILE @waybillRowId is not null
		begin		
			-- ��������� ������ �� �������			
		
			-- �������� ���������� ����
			SELECT
				@purchaseCost = rwr.PurchaseCost
			FROM
				ReceiptWaybillRow rwr
			WHERE
				rwr.Id = @waybillRowId
				
			IF @approvedDate is not null AND @added = 0 AND @areDivergencesAfterReceipt = 1
			BEGIN
				SET @count = @pendingCount * -1
				-- ��������� � �����������
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
				-- ������� �� ������������ ���������, �.�. ���� �����������
				exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
				-- ��������� � ������ ����� ������������
				exec AddValueToExactIndicator @approvedDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@approvedCount
			END
			IF @approvedDate is not null AND @added = 1
				-- ��������� � ������ ����������� �������
				exec AddValueToExactIndicator @approvedDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@approvedCount
			IF @approvedDate is null AND @receiptDate is not null AND @added = 0 AND @areDivergencesAfterReceipt = 1
			BEGIN
				SET @count = @pendingCount * -1
				-- ��������� � �����������
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
				-- ������� �� ������������ ���������, �.�. ���� �����������
				exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
			END
			IF @receiptDate is not null AND @added = 0 AND @areDivergencesAfterReceipt = 0
			BEGIN
				SET @count = @pendingCount * -1
				-- ��������� � �����������
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
				-- ������� �� ������������ ��������� � ���������� � ������
				exec AddValueToIncomingAcceptedIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@count
				exec AddValueToExactIndicator @receiptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount
			END
			IF @receiptDate is null
				-- ��������� � �����������
				exec AddValueToIncomingAcceptedIndicator @acceptDate,@storageId,@accountOrganizationId,@articleId,@batchId,@purchaseCost,@pendingCount				
			
			
			-- ��������� ������� ���������
			SET @waybillRowId = (
			SELECT TOP(1) Id
			FROM ReceiptWaybillRow
			WHERE
				DeletionDate is null
				AND ReceiptWaybillId = @waybillId
				AND Id > @waybillRowId
				ORDER BY Id)

			SELECT TOP(1) 
				@articleId = ArticleId,
				@pendingCount = PendingCount,
				@receiptedCount = ReceiptedCount,
				@approvedCount = ApprovedCount,
				@areDivergencesAfterReceipt = CASE WHEN (AreCountDivergencesAfterReceipt = 1 OR AreSumDivergencesAfterReceipt = 1) THEN 1 ELSE 0 END,
				@added = CASE WHEN PendingCount = 0 THEN 1 ELSE 0 END,
				@batchId = Id
			FROM 
				ReceiptWaybillRow
			WHERE
				Id = @waybillRowId				
		end
		
		-- ��������� ���������
		SET @waybillId = (
		SELECT TOP(1) 
			Id
		FROM 
			ReceiptWaybill
		WHERE
			DeletionDate is null
			AND AcceptanceDate is not null
			AND Id > @waybillId
			ORDER BY Id)
			
		SELECT TOP(1) 
			@storageId = ReceiptWaybillReceiptStorageId,
			@accountOrganizationId = AccountOrganizationId,
			@acceptDate = AcceptanceDate,
			@receiptDate = ReceiptDate,
			@approvedDate = ApprovementDate
		FROM 
			ReceiptWaybill
		WHERE
			Id = @waybillId
	end

---------------------------------------------------------------------------------------------------------------------------
	PRINT('����������� ���������� ������')
	-- ����������� ���������� ������ ����� � �����������
	UPDATE #T_OutgoingFromAccepted
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_OutgoingFromAccepted t) d
	WHERE
		#T_OutgoingFromAccepted.KeyHash = d.KeyHash
		AND #T_OutgoingFromAccepted.Id = d.Id
		
	-- ����������� ���������� ������ ����� � �����������
	UPDATE #T_OutgoingFromExact
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_OutgoingFromExact t) d
	WHERE
		#T_OutgoingFromExact.KeyHash = d.KeyHash
		AND #T_OutgoingFromExact.Id = d.Id
		
	-- ����������� ���������� ������ ����� � �����������
	UPDATE #T_Exact
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_Exact t) d
	WHERE
		#T_Exact.KeyHash = d.KeyHash
		AND #T_Exact.Id = d.Id
	
	-- ����������� ���������� ������ ����� � �����������
	UPDATE #T_IncomingAccepted
		SET RowNumber = d.rn
	FROM (
		SELECT 
			ROW_NUMBER() over (partition by t.KeyHash order by t.StartDate) rn, 
			t.KeyHash,
			t.Id
		FROM  #T_IncomingAccepted t) d
	WHERE
		#T_IncomingAccepted.KeyHash = d.KeyHash
		AND #T_IncomingAccepted.Id = d.Id
---------------------------------------------------------------------------------------------------------------------------
	-- ��������� ���������� �� ������
	declare @indicatorId uniqueidentifier --	��� ����������
	declare @startDateNext DateTime	--	���� ������ �������� ����������
	declare @countNext decimal(18,6)	--	���������� ����������
	declare @indicatorNextId uniqueidentifier -- ��� ����������
	declare @keyHash varchar(255) -- ���� �������� ����������
	declare @rowNumber int -- ���������� ����� �������� ����������

	-- ��������� ���������� ���������� ������������ �� ������������
	PRINT('��������� ������ ����������: ���������� ������������ �� ������������')
	
	-- ������ ����
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_OutgoingFromAccepted t
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
			#T_OutgoingFromAccepted t
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
				#T_OutgoingFromAccepted t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- ��������� ����������
			UPDATE #T_OutgoingFromAccepted
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- ��������� ���������
			UPDATE #T_OutgoingFromAccepted
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
				#T_OutgoingFromAccepted t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		-- ��������� ����
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_OutgoingFromAccepted t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end
-------------------------------------------------------------------
	-- ��������� ���������� ���������� ������������ �� �������
	PRINT('��������� ������ ����������: ���������� ������������ �� �������')
	-- ������ ����
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_OutgoingFromExact t
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
			#T_OutgoingFromExact t
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
				#T_OutgoingFromExact t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- ��������� ����������
			UPDATE #T_OutgoingFromExact
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- ��������� ���������
			UPDATE #T_OutgoingFromExact
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
				#T_OutgoingFromExact t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		
		-- ��������� ����
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_OutgoingFromExact t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end

---------------------------------
	-- ��������� ���������� �������
	PRINT('��������� ������ ����������: ������ �������')
	
	-- ������ ����
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_Exact t
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
			#T_Exact t
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
				#T_Exact t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- ��������� ����������
			UPDATE #T_Exact
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- ��������� ���������
			UPDATE #T_Exact
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
				#T_Exact t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		
		-- ��������� ����
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_Exact t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end

---------------------------------
	-- ��������� ���������� ������������ ���������
	PRINT('��������� ������ ����������: ������������ ���������')
	
	-- ������ ����
	SET @keyHash =(
	SELECT TOP(1)
		t.KeyHash
	FROM 
		#T_IncomingAccepted t
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
			#T_IncomingAccepted t
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
				#T_IncomingAccepted t 
			WHERE 
				t.KeyHash = @keyHash 
				AND t.RowNumber = @rowNumber + 1
			
			-- ��������� ����������
			UPDATE #T_IncomingAccepted
				SET EndDate = @startDateNext
			WHERE Id = @indicatorId
			
			-- ��������� ���������
			UPDATE #T_IncomingAccepted
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
				#T_IncomingAccepted t
			WHERE
				t.KeyHash = @keyHash
				AND t.RowNumber = @rowNumber + 1
		end
		
		-- ��������� ����
		SET @keyHash =(
		SELECT TOP(1)
			t.KeyHash
		FROM 
			#T_IncomingAccepted t
		WHERE
			t.KeyHash > @keyHash
			ORDER BY t.KeyHash)
	end
--*****************************************************************************************
--	��������� ������ ������� � ������
--*****************************************************************************************
	
	DECLARE @oldId UNIQUEIDENTIFIER	-- ��� �������� ������� ����������
	DECLARE @newId UNIQUEIDENTIFIER	-- ��� �������� ������ ����������
	DECLARE @oldCount DECIMAL(18,6) -- ������ ����������
	DECLARE @newCount DECIMAL(18,6) -- ����� ����������
	DECLARE @oldStartDate DateTime -- ���� ������ ��������
	
	--------------------------------------------------
	-- ���������� ��������� �� �������
	PRINT('���������� ��������� �� �������')
	
	SELECT Count(*) '����������� � ��������� �� �������'
	--SELECT *
	FROM #T_OutgoingFromExact t
	full JOIN  OutgoingAcceptedFromExactArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)

	--------------------------------------------------
	-- ���������� ��������� �� ������������
	PRINT('���������� ��������� �� ������������')
	
	SELECT Count(*) '����������� � ��������� �� ������������'
	--SELECT *
	FROM #T_OutgoingFromAccepted t
	full JOIN  OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)
    
	--------------------------------------------------
	-- ���������� ������ �������
	PRINT('���������� ������ �������')
	
	SELECT Count(*) '����������� � ������ �������'
	--SELECT *
	FROM #T_Exact t
	full JOIN  ExactArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)
    
	--------------------------------------------------
	-- ���������� �������� �����������
	PRINT('���������� �������� �����������')
	
	SELECT Count(*) '����������� �� �������� �����������'
	--SELECT *
	FROM #T_IncomingAccepted t
	full JOIN  IncomingAcceptedArticleAvailabilityIndicator a on 
		a.StartDate >= t.StartDate AND (a.StartDate < t.EndDate OR t.EndDate is null)
		AND a.StorageId = t.StorageId 
		AND a.AccountOrganizationId = t.AccountOrganizationId 
		AND a.ArticleId = t.ArticleId 
		AND a.BatchId = t.BatchId 
		AND a.PurchaseCost = t.PurchaseCost 
		AND a.Count = t.Count
    WHERE (a.Id is null OR t.Id is null) AND (a.Count <> 0 OR t.Count <> 0)
    
	--COMMIT TRAN
	ROLLBACK TRAN
END TRY
BEGIN CATCH
	IF XACT_STATE() <> 0 ROLLBACK TRAN
		
	RETURN
END CATCH

DROP INDEX IX_#T_OutgoingFromAccepted on #T_OutgoingFromAccepted
DROP INDEX IX_#T_OutgoingFromExact on #T_OutgoingFromExact
DROP INDEX IX_#T_Exact ON #T_Exact
DROP INDEX IX_#T_IncomingAccepted ON #T_IncomingAccepted

DROP TABLE #T_OutgoingFromExact
DROP TABLE #T_OutgoingFromAccepted
DROP TABLE #T_Exact
DROP TABLE #T_IncomingAccepted
DROP TABLE #Diff

DROP FUNCTION dbo.GetWaybillReceiptDate
DROP PROCEDURE dbo.AddValueToOutgoingFromAcceptedIndicator
DROP PROCEDURE dbo.AddValueToOutgoingFromExectIndicator
DROP PROCEDURE dbo.InsertOutgoingIndicator
DROP PROCEDURE dbo.AddValueToExactIndicator
DROP PROCEDURE dbo.AddValueToIncomingAcceptedIndicator
DROP PROCEDURE dbo.InsertIncomingIndicator