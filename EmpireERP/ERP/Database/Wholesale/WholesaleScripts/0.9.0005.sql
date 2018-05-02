/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.5

  ��� ������:
	* ���������� ��������	
	
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

SET @PreviousVersion = '0.9.4' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.5'			-- ����� ����� ������

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

--*** ReceiptWaybillRow *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId')
DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] 

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


CREATE INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [ReceiptWaybillRow] ([DeletionDate], [ReceiptWaybillId]) 
INCLUDE ([Id], [ArticleMeasureUnitScale], [PendingCount], [PendingSum], [ReceiptedCount], [ProviderCount], [ApprovedCount], [ApprovedSum], [PurchaseCost], [CustomsDeclarationNumber], [CreationDate], [FinallyMovementDate], [ReservedCount], [AcceptedCount], [ShippedCount], [FinallyMovedCount], [RecipientArticleAccountingPriceId], [ArticleId], [PendingValueAddedTaxId], [ApprovedValueAddedTaxId], [CountryId], [ManufacturerId], [InitialPurchaseCost], [ProviderSum], [ApprovedPurchaseCost])

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


--*** ArticleAccountingPriceIndicator *******************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ArticleAccountingPriceIndicator_StorageId_StartDate_EndDate')
DROP INDEX [IX_ArticleAccountingPriceIndicator_StorageId_StartDate_EndDate] ON [dbo].[ArticleAccountingPriceIndicator]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


CREATE INDEX [IX_ArticleAccountingPriceIndicator_StorageId_StartDate_EndDate] ON [ArticleAccountingPriceIndicator] ([StorageId], [StartDate], [EndDate]) 
INCLUDE ([Id], [ArticleId], [AccountingPrice], [AccountingPriceListId])

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


--*** ArticleAccountingPrice ***********************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId')
DROP INDEX [IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId] ON [dbo].[ArticleAccountingPrice]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


CREATE INDEX [IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId] ON [ArticleAccountingPrice] ([DeletionDate], [AccountingPriceListId], [ArticleId]) 
INCLUDE ([Id], [AccountingPrice], [CreationDate], [UsedDefaultAccountingPriceCalcRule], [UsedDefaultLastDigitRule], [OrdinalNumber])

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


--***  ExactArticleAvailabilityIndicator ****************************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId')
DROP INDEX [IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId] ON [dbo].[ExactArticleAvailabilityIndicator]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


CREATE INDEX [IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId] ON [ExactArticleAvailabilityIndicator] ([StorageId], [AccountOrganizationId], [ArticleId], [BatchId])

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

