/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.26

  ��� ������:
	+ ��������� ���� OrdinalNumber � ������� ������� ������� � �������� � ���. ��������� INT_MAX
	* ��������� ���� OrdinalNumber � ������ ������� ��������
	+ ��������� ���� WorkDaysPlanScheme � ������ � ���. ��������� (�� �� �� �� ��)
	+ ��������� ���� InWorkDays � ������� ������ ������ � ������ ������� ������
	
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

SET @PreviousVersion = '0.9.25' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.26'			-- ����� ����� ������

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

alter table dbo.ProductionOrderBatchRow
add OrdinalNumber INT not null
constraint DF_ProductionOrderBatchRow_OrdinalNumber default (2147483647)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	alter table ProductionOrderBatchRow
	drop constraint DF_ProductionOrderBatchRow_OrdinalNumber

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.ReceiptWaybillRow
add OrdinalNumber INT not null
constraint DF_ReceiptWaybillRow_OrdinalNumber default (2147483647)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

	alter table ReceiptWaybillRow
	drop constraint DF_ReceiptWaybillRow_OrdinalNumber

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId')
DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] 

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

CREATE INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [ReceiptWaybillRow] ([DeletionDate], [ReceiptWaybillId], [ArticleId]) INCLUDE ([Id], [ArticleMeasureUnitScale], [PendingCount], [PendingSum], [ReceiptedCount], [ProviderCount], [ApprovedCount], [ApprovedSum], [PurchaseCost], [CustomsDeclarationNumber], [CreationDate], [AcceptedCount], [ShippedCount], [FinallyMovedCount], [RecipientArticleAccountingPriceId], [PendingValueAddedTaxId], [ApprovedValueAddedTaxId], [CountryId], [ManufacturerId], [InitialPurchaseCost], [ProviderSum], [ApprovedPurchaseCost], [UsageAsManualSourceCount], [AvailableToReserveCount], [OrdinalNumber]);

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ProductionOrder]
ADD ArticleTransportingPrimeCostCalculationTypeId TINYINT not null
CONSTRAINT DF_ArticleTransportingPrimeCostCalculationTypeId DEFAULT 3

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ProductionOrder]  drop constraint DF_ArticleTransportingPrimeCostCalculationTypeId

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ProductionOrder]
ADD WorkDaysPlanScheme TINYINT not null
CONSTRAINT DF_WorkDaysPlanScheme DEFAULT 124

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ProductionOrder]  drop constraint DF_WorkDaysPlanScheme

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ProductionOrderBatchLifeCycleTemplateStage]
ADD InWorkDays BIT not null
CONSTRAINT DF_ProductionOrderBatchLifeCycleTemplateStage_InWorkDays DEFAULT 0

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ProductionOrderBatchLifeCycleTemplateStage]
	drop constraint DF_ProductionOrderBatchLifeCycleTemplateStage_InWorkDays

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

ALTER TABLE dbo.[ProductionOrderBatchStage]
ADD InWorkDays BIT not null
CONSTRAINT DF_ProductionOrderBatchStage_InWorkDays DEFAULT 0

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ProductionOrderBatchStage]
	drop constraint DF_ProductionOrderBatchStage_InWorkDays

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

