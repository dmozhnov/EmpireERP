/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.31

  ��� ������:
	- �������� ����� �� ��������� ����� ������ ��������� ����������
	* �������� ����������� ���������� � ���������� ���-�� ���������� �������� 
	
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

SET @PreviousVersion = '0.9.30' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.31'			-- ����� ����� ������

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

-- �������� ����� �� ��������� ����� ������ ��������� ����������
delete from PermissionDistribution where PermissionId = 3613
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybill_DeletionDate_DealId_DealPaymentFormId_Id')
DROP INDEX IX_SaleWaybill_DeletionDate_DealId_DealPaymentFormId_Id ON [dbo].SaleWaybill
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table SaleWaybill drop column DealPaymentFormId
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

CREATE INDEX IX_SaleWaybill_DeletionDate_DealId_Id ON [SaleWaybill] ([DeletionDate], [DealId], [Id])
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- �������� ����������� ����������
select StartDate = E.ShippingDate, SenderId = C.AccountOrganizationId, SenderStorageId = ExpenditureWaybillSenderStorageId, WaybillId = S.Id, 
	PurchaseCostSum = (
		select SUM(Round(RW.PurchaseCost * SR.SellingCount, 6))
		from ExpenditureWaybillRow ER
		join SaleWaybillRow SR on SR.Id = ER.Id and SR.DeletionDate is null
		join ReceiptWaybillRow RW on RW.Id = ER.ExpenditureWaybillRowReceiptWaybillRowId
		where SR.SaleWaybillId = S.Id
	),
	E.SenderAccountingPriceSum, E.SalePriceSum, 
	RowNumber = ROW_NUMBER() OVER (Partition by C.AccountOrganizationId, ExpenditureWaybillSenderStorageId order by E.ShippingDate), 
	Id = (select top 1 Id from ArticleMovementFactualFinancialIndicator where WaybillId = S.Id)
into #t
from SaleWaybill S
join ExpenditureWaybill E on E.Id = S.Id
join Deal D on D.Id = S.DealId
join Contract C on C.Id = D.ClientContractId
where S.DeletionDate is null and E.ShippingDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

delete from ArticleMovementFactualFinancialIndicator
where ArticleMovementOperationType in (2, 3)
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

INSERT INTO [ArticleMovementFactualFinancialIndicator]([Id],[StartDate],[EndDate],[RecipientId],[RecipientStorageId],[SenderId],[SenderStorageId],[PreviousId]
	,[WaybillId],[ArticleMovementOperationType],[AccountingPriceSum],[PurchaseCostSum],[SalePriceSum])
select t.Id, t.StartDate, 
	EndDate = (select StartDate from #t where SenderId = t.SenderId and SenderStorageId = t.SenderStorageId and RowNumber = t.RowNumber + 1),
	null, null, SenderId, SenderStorageId,
	PreviousId = (select Id from #t where SenderId = t.SenderId and SenderStorageId = t.SenderStorageId and RowNumber = t.RowNumber - 1),
	WaybillId, ArticleMovementOperationType = 2, 	
	AccountingPriceSum = (select SUM(SenderAccountingPriceSum) from #t where SenderId = t.SenderId and SenderStorageId = t.SenderStorageId and RowNumber <= t.RowNumber),
	PurchaseCostSum = (select SUM(PurchaseCostSum) from #t where SenderId = t.SenderId and SenderStorageId = t.SenderStorageId and RowNumber <= t.RowNumber),
	SalePriceSum = (select SUM(SalePriceSum) from #t where SenderId = t.SenderId and SenderStorageId = t.SenderStorageId and RowNumber <= t.RowNumber)
from #t t
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #t
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- �������� ���������� ���������� ���������� �������� ��������������
select StartDate = E.ShippingDate, SenderStorageId = ExpenditureWaybillSenderStorageId,
	RowNumber = ROW_NUMBER() OVER (Partition by ExpenditureWaybillSenderStorageId order by E.ShippingDate), 
	Id = NEWID()
into #t
from SaleWaybill S
join ExpenditureWaybill E on E.Id = S.Id
join Deal D on D.Id = S.DealId
join Contract C on C.Id = D.ClientContractId
where S.DeletionDate is null and E.ShippingDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

delete from ArticleMovementOperationCountIndicator
where ArticleMovementOperationType in (2, 3)
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

INSERT INTO [ArticleMovementOperationCountIndicator]([Id],[StartDate],[EndDate],[PreviousId],[ArticleMovementOperationType],[StorageId],[Count])
select Id, StartDate, 
	EndDate = (select StartDate from #t where SenderStorageId = t.SenderStorageId and RowNumber = t.RowNumber + 1),
	PreviousId = (select Id from #t where SenderStorageId = t.SenderStorageId and RowNumber = t.RowNumber - 1),
	2, SenderStorageId, 
	[Count] = (select count(*) from #t where SenderStorageId = t.SenderStorageId and RowNumber <= t.RowNumber)
from #t t
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #t
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

