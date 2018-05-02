SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

CREATE TABLE #ArticleMovementOperationCountIndicator_Data(	
	[StartDate] [datetime] NOT NULL,		
	[ArticleMovementOperationType] [tinyint] NOT NULL,
	[StorageId] [smallint] NOT NULL,
	[Count] [int] NOT NULL
)
GO

CREATE TABLE #ArticleMovementOperationCountIndicator_HashedKeyData(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[StartDate] [datetime] NOT NULL,	
	[ArticleMovementOperationType] [tinyint] NOT NULL,
	[StorageId] [smallint] NOT NULL,
	[Count] [int] NOT NULL,
	RowNumber int NOT NULL,
	KeyHash varchar(500) NOT NULL
)
GO

BEGIN TRY
	BEGIN TRAN
			
		insert into #ArticleMovementOperationCountIndicator_Data([StartDate], [StorageId], [ArticleMovementOperationType],[Count])

		-- ������� ��������� ���������
		select StartDate = w.ReceiptDate, StorageId = ReceiptWaybillReceiptStorageId, ArticleMovementOperationType = 1, [Count] = COUNT(*)
		from ReceiptWaybill w
		where w.DeletionDate is null and w.ReceiptDate is not null
		group by w.ReceiptDate, ReceiptWaybillReceiptStorageId

		-- ������ ��� ������� ��������� �����������
		union all
		select StartDate = w.ReceiptDate, w.RecipientStorageId, ArticleMovementOperationType = 4, [Count] = COUNT(*)
		from MovementWaybill w
		where w.DeletionDate is null and w.ReceiptDate is not null
		group by w.ReceiptDate, w.RecipientStorageId

		-- ������ ��� ������� ��������� �����������
		union all
		select StartDate = w.ReceiptDate, w.SenderStorageId, ArticleMovementOperationType = 5, [Count] = COUNT(*)
		from MovementWaybill w
		where w.DeletionDate is null and w.ReceiptDate is not null
		group by w.ReceiptDate, w.SenderStorageId

		-- �������� ��������� ��������
		union all
		select StartDate = w.WriteoffDate, w.WriteoffWaybillSenderStorageId, ArticleMovementOperationType = 6, [Count] = COUNT(*)
		from WriteoffWaybill w
		where w.DeletionDate is null and w.WriteoffDate is not null
		group by w.WriteoffDate, w.WriteoffWaybillSenderStorageId

		-- �������� ��������� ���������� �������
		union all
		select StartDate = ew.ShippingDate, ew.ExpenditureWaybillSenderStorageId, ArticleMovementOperationType = 2, [Count] = COUNT(*)
		from SaleWaybill w
		join ExpenditureWaybill ew on ew.id = w.id
		where w.DeletionDate is null and ew.ShippingDate is not null
		group by ew.ShippingDate, ew.ExpenditureWaybillSenderStorageId

		-- ������� �������� ������ �� �������
		union all
		select StartDate = w.ReceiptDate, w.ReturnFromClientWaybillRecipientStorageId, ArticleMovementOperationType = 7, [Count] = COUNT(*)
		from ReturnFromClientWaybill w
		where w.DeletionDate is null and w.ReceiptDate is not null
		group by w.ReceiptDate, w.ReturnFromClientWaybillRecipientStorageId

		create index idx_ArticleMovementOperationCountIndicator_Data on #ArticleMovementOperationCountIndicator_Data ([StartDate], [StorageId], [ArticleMovementOperationType])

		insert into #ArticleMovementOperationCountIndicator_HashedKeyData (StartDate, StorageId, ArticleMovementOperationType,[Count],RowNumber, KeyHash)
		select StartDate, StorageId, ArticleMovementOperationType,
			[Count] = sum([Count]),
			RowNumber = ROW_NUMBER() OVER ( partition by StorageId, ArticleMovementOperationType order by StartDate),
			-- ���-���� ��� ��������� ���������� ������� ����� � �����
			KeyHash = CAST(HashBytes('SHA1', 
				CAST(StorageId as varchar(255)) + '_' +
				CAST(ArticleMovementOperationType as varchar(255))
			) as varchar(500))
		from #ArticleMovementOperationCountIndicator_Data
		group by StartDate, StorageId,  ArticleMovementOperationType

		create index idx_ArticleMovementOperationCountIndicator_HashedKeyData on #ArticleMovementOperationCountIndicator_HashedKeyData (KeyHash, RowNumber)

		delete ArticleMovementOperationCountIndicator

		insert into ArticleMovementOperationCountIndicator(Id,StartDate,EndDate,PreviousId, StorageId,ArticleMovementOperationType,[Count])
		select T.Id, T.StartDate, 
			(select StartDate from #ArticleMovementOperationCountIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber = T.RowNumber + 1),	
			PreviousId = (select Id from #ArticleMovementOperationCountIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber = T.RowNumber - 1),
			StorageId,
			ArticleMovementOperationType,	
			[Count] = (select SUM([Count]) from #ArticleMovementOperationCountIndicator_HashedKeyData where KeyHash = T.KeyHash and RowNumber <= T.RowNumber)
		from #ArticleMovementOperationCountIndicator_HashedKeyData T
									
	COMMIT TRAN
END TRY
BEGIN CATCH
	IF XACT_STATE() <> 0 ROLLBACK TRAN
	RETURN
END CATCH


DROP TABLE #ArticleMovementOperationCountIndicator_Data
DROP TABLE #ArticleMovementOperationCountIndicator_HashedKeyData