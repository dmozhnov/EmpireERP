/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.3

  ��� ������:
	+ InitialPurchaseCost � �������� �������
	+ ProviderSum � �������� �������
	+ ApprovedPurchaseCost � �������� �������
	* DiscountSum � ��������� ������� ������� �� 2 ������ � ������������� � PendingDiscountSum
	* PendingSum � ������� ��������� ������� ������� �� 2 ������
	* ApprovedSum � ������� ��������� ������� ������� �� 2 ������
	- ����� DiscountPercent �� ��������� �������
	- ������ ReceiptedSum �� ��������� �������
	
	!!! ���� ���������, ����� ���������� ������� ������� SELECT � �����, 0 - ������ ��� �������
	
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

SET @PreviousVersion = '0.9.2' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.3'			-- ����� ����� ������

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

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = 
		N'���������� ������', NOFORMAT

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

	alter table dbo.[ReceiptWaybill] DROP column DiscountPercent;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 2 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybill] alter column DiscountSum DECIMAL(18, 2) not null;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 3 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    EXEC sp_rename 'ReceiptWaybill.DiscountSum', 'PendingDiscountSum';

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 4 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] ADD InitialPurchaseCost DECIMAL(18, 6) not null
	CONSTRAINT DF_ReceiptWaybillRow_InitialPurchaseCost DEFAULT 0

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 5 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] DROP CONSTRAINT DF_ReceiptWaybillRow_InitialPurchaseCost

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 6 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    -- ���� InitialPurchaseCost ����� ��������� ����� �������, �������� �� ���������� (���� ��� �� ����
    -- ��������� ��� �������)
	update RR
	set RR.InitialPurchaseCost =
		CASE WHEN RR.PendingCount <> 0 THEN
			RR.PendingSum / RR.PendingCount
		ELSE
			0
		END
	from
    dbo.[ReceiptWaybillRow] RR;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 7 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] WITH ( ONLINE = OFF )

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 8 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybillRow] alter column PendingSum DECIMAL(18, 2) not null;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 9 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] ADD ProviderSum DECIMAL(18, 2) null;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 10 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	CREATE NONCLUSTERED INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow]
	(
		[DeletionDate] ASC,
		[ReceiptWaybillId] ASC
	)
	INCLUDE ( [Id],[ArticleMeasureUnitScale],[PendingCount],[PendingSum],[ReceiptedCount],[ProviderCount],[ProviderSum],
	[ApprovedCount],[ApprovedSum],[PurchaseCost],[CustomsDeclarationNumber],[CreationDate],[FinallyMovementDate],
	[ReservedCount],[AcceptedCount],[ShippedCount],[FinallyMovedCount],[RecipientArticleAccountingPriceId],
	[ArticleId],[PendingValueAddedTaxId],[ApprovedValueAddedTaxId],[CountryId],[ManufacturerId])
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF,
	DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 11 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    -- ���� ProviderSum ����� ����� ��������� ����� �������, ���� ��������� ��� ������� �� �����
	update RR
	set RR.[ProviderSum] = RR.PendingSum
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId <> 1 AND R.ReceiptWaybillStateId <> 7;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 12 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] WITH ( ONLINE = OFF )

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 13 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybillRow] alter column ApprovedSum DECIMAL(18, 2) null;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 14 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	CREATE NONCLUSTERED INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow]
	(
		[DeletionDate] ASC,
		[ReceiptWaybillId] ASC
	)
	INCLUDE ( [Id],[ArticleMeasureUnitScale],[PendingCount],[PendingSum],[ReceiptedCount],[ProviderCount],[ProviderSum],
	[ApprovedCount],[ApprovedSum],[PurchaseCost],[CustomsDeclarationNumber],[CreationDate],[FinallyMovementDate],
	[ReservedCount],[AcceptedCount],[ShippedCount],[FinallyMovedCount],[RecipientArticleAccountingPriceId],
	[ArticleId],[PendingValueAddedTaxId],[ApprovedValueAddedTaxId],[CountryId],[ManufacturerId])
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF,
	DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 15 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	alter table dbo.[ReceiptWaybill] DROP column ReceiptedSum;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 16 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    -- ���� ProviderSum � ������������� ��������� (� ������������� � ���) ����� ����� ������������� ����� �������
    -- ������, ��� �����, ��� ���� ������ ��� "����� �����������", � ����.�� �-�� ���, � ����� ��� ����. ������� �
    -- ���������� ������� (� �� ����� ����� ������������ �������� "����� �� ���������"), �� ������������ �����
    -- ����� ��������� ��������� �� �� ����� :)
	update RR
	set RR.[ProviderSum] = RR.[ApprovedSum]
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 2 OR R.ReceiptWaybillStateId = 6;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 17 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    -- ���� ProviderSum � ���������, �������� �� �����, �� �� ������������� (� ������������� "�� �����" ��� "�� ����� �
    -- ����������") ����� ����� ��������� ����� ������� + 1 ����� :) ������� ������, ��� ��� ����� �� ���� ��������
    -- �� ������������ (�����, ��� ���������� �����, ��������� � ���� ReceiptedSum ���� ��������� � ������ ����).
	update RR
	set RR.[ProviderSum] = RR.[PendingSum] + 1
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 3 OR R.ReceiptWaybillStateId = 5;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 18 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	ALTER TABLE dbo.[ReceiptWaybillRow] ADD ApprovedPurchaseCost DECIMAL(18, 6) null

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 19 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    -- ���� ApprovedPurchaseCost � ������������� ��������� (� ������������� � ���) ����� ����� ������������� ����������
    -- ���� (PurchaseCost). � ������ ��������� ��� ����� ����� null (��� �������� � ��������� ���������� � ���
    -- ���������, ������� ��� �������� �����������, � �� ���������� Approved-��������).
    -- ������, ��� �����, ��� ���� ��� ���� ���������� ��������� Approved-��������, ��� ����������
	update RR
	set RR.[ApprovedPurchaseCost] = RR.[PurchaseCost]
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE R.ReceiptWaybillStateId = 2 OR R.ReceiptWaybillStateId = 6;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 20 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    -- ����� ���������� ApprovedPurchaseCost � �������� ������� �������� �������� ���������
	update RR
	set RR.[ApprovedPurchaseCost] = RR.[PurchaseCost]
	from
    dbo.[ReceiptWaybillRow] RR
    JOIN [ReceiptWaybill] R on R.Id = RR.ReceiptWaybillId
    WHERE (R.ReceiptWaybillStateId = 3 OR R.ReceiptWaybillStateId = 4 OR R.ReceiptWaybillStateId = 5)
		AND RR.PendingCount = RR.ReceiptedCount AND RR.PendingCount = RR.ProviderCount AND
		RR.PendingSum = RR.[ProviderSum];

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 21 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    -- � ����� � ���������� ������� ������������� ���. ���� � ������������� �����, �-��, ���� ����
    -- �� �������� ������� ��� ����������� ����� null, � ������ �� null, ���������� ����������
    -- ��� ������� ������ ������������. ������ �� ������ ����
	update RR
	set RR.ApprovedCount = null,
		RR.ApprovedPurchaseCost = null,
		RR.ApprovedSum = null
	from
    dbo.[ReceiptWaybillRow] RR
    WHERE RR.ApprovedCount is null OR
		RR.ApprovedPurchaseCost is null OR
		RR.ApprovedSum is null;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 22 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    PRINT '���� ���� ������ ������� ������ 0 �����, ���� ����� (���� �������������� �����). �� ����� �� ������ ����'
    DECLARE @count_i INT
	SELECT @count_i = COUNT(*)
		FROM dbo.ReceiptWaybillRow where Round(PendingCount * InitialPurchaseCost, 2) <> PendingSum;
    PRINT @count_i

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

