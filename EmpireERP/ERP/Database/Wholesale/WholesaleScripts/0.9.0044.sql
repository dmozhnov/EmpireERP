/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.44

  ��� ������:
	* ��������� ������� ������ ������ ��� ���������� (��������� ���)
	
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

SET @PreviousVersion = '0.9.43' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.44'			-- ����� ����� ������

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

-- �������� ��������� ��������� � �� ��� ���������
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
	
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ����� ���������
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
	
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ����� �����
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

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ����� ������������ ������ (�.�. ����� ����� ������� � ��� ����������, �� �������� �������� ��������� � ���������������� ������)
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

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- �������� ������������ ������� �� �����������
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
	
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ���������� ���� ������ ������
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

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #SaleSum
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #ReturnSum
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #PaymentSum
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #ReturnPaymentSum
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #Remainder
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

