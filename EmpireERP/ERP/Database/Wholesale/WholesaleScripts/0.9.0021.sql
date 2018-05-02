/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.21

  ��� ������:
	+ ������� ��� ����� ����������� �� ��������� (ReturnFromClientBySaleAcceptanceDateIndicator � ReturnFromClientBySaleShippingDateIndicator)
	* �������� ����������� AcceptedReturnFromClientIndicator (������ �� �� ����������� ��� ������� ��������) � ReceiptedReturnFromClientIndicator
	
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

SET @PreviousVersion = '0.9.20' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.21'			-- ����� ����� ������

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

-- �������� �� ������������ ���������� � �� ��� ����������� �������
IF EXISTS 
(
	select ExpenditureWaybillSenderStorageId, SaleWaybillCuratorId, ReturnFromClientWaybillCuratorId,
		ProviderId, ContractorOrganizationId, TeamId, ClientId, DealId, AccountOrganizationId, ArticleId, SaleWaybillId, ReceiptWaybillRowId
	from 
	(
		select EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, RW.ReturnFromClientWaybillCuratorId,
			RecW.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId, DealId = D.Id, C.AccountOrganizationId, RecWR.ArticleId, SaleWaybillId = SW.Id, 
			ReceiptWaybillRowId = RWR.ReceiptWaybillRowId	
		from ReturnFromClientWaybillRow RWR
		join ReturnFromClientWaybill RW on RW.Id = RWR.ReturnFromClientWaybillId
		join SaleWaybillRow SWR on SWR.Id = RWR.SaleWaybillRowId
		join SaleWaybill SW on SW.Id = SWR.SaleWaybillId
		join ExpenditureWaybill EW on EW.Id = SW.Id
		join ExpenditureWaybillRow EWR on EWR.Id = SWR.Id
		join ReceiptWaybillRow RecWR on RecWR.Id = RWR.ReceiptWaybillRowId
		join ReceiptWaybill RecW on RecW.Id = RecWR.ReceiptWaybillId
		join Deal D on D.Id = SW.DealId
		join Contract C on C.Id = D.ClientContractId
		join ArticleAccountingPrice AAP on AAP.Id = RWR.ReturnFromClientArticleAccountingPriceId
	) T
	group by ExpenditureWaybillSenderStorageId, SaleWaybillCuratorId, ReturnFromClientWaybillCuratorId,
		ProviderId, ContractorOrganizationId, TeamId, ClientId, DealId, AccountOrganizationId, ArticleId, SaleWaybillId, ReceiptWaybillRowId
	having COUNT(*) > 1
)
BEGIN
	RAISERROR('������! � �� �� ������ ���� ��������� � ����������� ��������� �����������.',16,1)
END

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

create table dbo.[ReturnFromClientBySaleAcceptanceDateIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       SaleWaybillCuratorId INT not null,
       ReturnFromClientWaybillCuratorId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT null,
       ClientId INT not null,
       DealId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       BatchId UNIQUEIDENTIFIER not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       ReturnedCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into [ReturnFromClientBySaleAcceptanceDateIndicator] (Id, StartDate, EndDate, StorageId, SaleWaybillCuratorId, ReturnFromClientWaybillCuratorId,
       ContractorId, ClientOrganizationId, TeamId, ClientId, DealId, AccountOrganizationId, ArticleId, SaleWaybillId, BatchId, PreviousId,
       PurchaseCostSum, AccountingPriceSum, SalePriceSum, ReturnedCount)

select NEWID(), EW.AcceptanceDate, null, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, RW.ReturnFromClientWaybillCuratorId,
	RecW.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId, DealId = D.Id, C.AccountOrganizationId, RecWR.ArticleId, SW.Id, 
	ReceiptWaybillRowId = RWR.ReceiptWaybillRowId, null, 
	PurchaseCostSum = Round(RecWR.PurchaseCost * RWR.ReturnCount, 6),
	AccountingPriceSum = Round(AAP.AccountingPrice * RWR.ReturnCount,2),
	SalePriceSum = Round(SWR.SalePrice * RWR.ReturnCount,2),
	RWR.ReturnCount
from ReturnFromClientWaybillRow RWR
join ReturnFromClientWaybill RW on RW.Id = RWR.ReturnFromClientWaybillId
join SaleWaybillRow SWR on SWR.Id = RWR.SaleWaybillRowId
join SaleWaybill SW on SW.Id = SWR.SaleWaybillId
join ExpenditureWaybill EW on EW.Id = SW.Id
join ExpenditureWaybillRow EWR on EWR.Id = SWR.Id
join ReceiptWaybillRow RecWR on RecWR.Id = RWR.ReceiptWaybillRowId
join ReceiptWaybill RecW on RecW.Id = RecWR.ReceiptWaybillId
join Deal D on D.Id = SW.DealId
join Contract C on C.Id = D.ClientContractId
join ArticleAccountingPrice AAP on AAP.Id = RWR.ReturnFromClientArticleAccountingPriceId
where RW.ReturnFromClientWaybillStateId in (2, 3)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

create table dbo.[ReturnFromClientBySaleShippingDateIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       SaleWaybillCuratorId INT not null,
       ReturnFromClientWaybillCuratorId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT null,
       ClientId INT not null,
       DealId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       BatchId UNIQUEIDENTIFIER not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       ReturnedCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into [ReturnFromClientBySaleShippingDateIndicator] (Id, StartDate, EndDate, StorageId, SaleWaybillCuratorId, ReturnFromClientWaybillCuratorId,
       ContractorId, ClientOrganizationId, TeamId, ClientId, DealId, AccountOrganizationId, ArticleId, SaleWaybillId, BatchId, PreviousId,
       PurchaseCostSum, AccountingPriceSum, SalePriceSum, ReturnedCount)

select NEWID(), EW.ShippingDate, null, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, RW.ReturnFromClientWaybillCuratorId,
	RecW.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId, DealId = D.Id, C.AccountOrganizationId, RecWR.ArticleId, SW.Id, 
	ReceiptWaybillRowId = RWR.ReceiptWaybillRowId, null, 
	PurchaseCostSum = Round(RecWR.PurchaseCost * RWR.ReturnCount, 6),
	AccountingPriceSum = Round(AAP.AccountingPrice * RWR.ReturnCount,2),
	SalePriceSum = Round(SWR.SalePrice * RWR.ReturnCount,2),
	RWR.ReturnCount
from ReturnFromClientWaybillRow RWR
join ReturnFromClientWaybill RW on RW.Id = RWR.ReturnFromClientWaybillId
join SaleWaybillRow SWR on SWR.Id = RWR.SaleWaybillRowId
join SaleWaybill SW on SW.Id = SWR.SaleWaybillId
join ExpenditureWaybill EW on EW.Id = SW.Id
join ExpenditureWaybillRow EWR on EWR.Id = SWR.Id
join ReceiptWaybillRow RecWR on RecWR.Id = RWR.ReceiptWaybillRowId
join ReceiptWaybill RecW on RecW.Id = RecWR.ReceiptWaybillId
join Deal D on D.Id = SW.DealId
join Contract C on C.Id = D.ClientContractId
join ArticleAccountingPrice AAP on AAP.Id = RWR.ReturnFromClientArticleAccountingPriceId
where RW.ReturnFromClientWaybillStateId in (2, 3)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

delete AcceptedReturnFromClientIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into AcceptedReturnFromClientIndicator (Id, StartDate, EndDate, StorageId, SaleWaybillCuratorId, ReturnFromClientWaybillCuratorId,
       ContractorId, ClientOrganizationId, TeamId, ClientId, DealId, AccountOrganizationId, ArticleId, SaleWaybillId, BatchId, PreviousId,
       PurchaseCostSum, AccountingPriceSum, SalePriceSum, ReturnedCount)

select NEWID(), RW.AcceptanceDate, null, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, RW.ReturnFromClientWaybillCuratorId,
	RecW.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId, DealId = D.Id, C.AccountOrganizationId, RecWR.ArticleId, SW.Id, 
	ReceiptWaybillRowId = RWR.ReceiptWaybillRowId, null, 
	PurchaseCostSum = Round(RecWR.PurchaseCost * RWR.ReturnCount, 6),
	AccountingPriceSum = Round(AAP.AccountingPrice * RWR.ReturnCount,2),
	SalePriceSum = Round(SWR.SalePrice * RWR.ReturnCount,2),
	RWR.ReturnCount
from ReturnFromClientWaybillRow RWR
join ReturnFromClientWaybill RW on RW.Id = RWR.ReturnFromClientWaybillId
join SaleWaybillRow SWR on SWR.Id = RWR.SaleWaybillRowId
join SaleWaybill SW on SW.Id = SWR.SaleWaybillId
join ExpenditureWaybill EW on EW.Id = SW.Id
join ExpenditureWaybillRow EWR on EWR.Id = SWR.Id
join ReceiptWaybillRow RecWR on RecWR.Id = RWR.ReceiptWaybillRowId
join ReceiptWaybill RecW on RecW.Id = RecWR.ReceiptWaybillId
join Deal D on D.Id = SW.DealId
join Contract C on C.Id = D.ClientContractId
join ArticleAccountingPrice AAP on AAP.Id = RWR.ReturnFromClientArticleAccountingPriceId
where RW.ReturnFromClientWaybillStateId in (2, 3)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

delete ReceiptedReturnFromClientIndicator

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into ReceiptedReturnFromClientIndicator (Id, StartDate, EndDate, StorageId, SaleWaybillCuratorId, ReturnFromClientWaybillCuratorId,
       ContractorId, ClientOrganizationId, TeamId, ClientId, DealId, AccountOrganizationId, ArticleId, SaleWaybillId, BatchId, PreviousId,
       PurchaseCostSum, AccountingPriceSum, SalePriceSum, ReturnedCount)

select NEWID(), RW.ReceiptDate, null, EW.ExpenditureWaybillSenderStorageId, SW.SaleWaybillCuratorId, RW.ReturnFromClientWaybillCuratorId,
	RecW.ProviderId, C.ContractorOrganizationId, SW.TeamId, D.ClientId, DealId = D.Id, C.AccountOrganizationId, RecWR.ArticleId, SW.Id, 
	ReceiptWaybillRowId = RWR.ReceiptWaybillRowId, null, 
	PurchaseCostSum = Round(RecWR.PurchaseCost * RWR.ReturnCount, 6),
	AccountingPriceSum = Round(AAP.AccountingPrice * RWR.ReturnCount,2),
	SalePriceSum = Round(SWR.SalePrice * RWR.ReturnCount,2),
	RWR.ReturnCount
from ReturnFromClientWaybillRow RWR
join ReturnFromClientWaybill RW on RW.Id = RWR.ReturnFromClientWaybillId
join SaleWaybillRow SWR on SWR.Id = RWR.SaleWaybillRowId
join SaleWaybill SW on SW.Id = SWR.SaleWaybillId
join ExpenditureWaybill EW on EW.Id = SW.Id
join ExpenditureWaybillRow EWR on EWR.Id = SWR.Id
join ReceiptWaybillRow RecWR on RecWR.Id = RWR.ReceiptWaybillRowId
join ReceiptWaybill RecW on RecW.Id = RecWR.ReceiptWaybillId
join Deal D on D.Id = SW.DealId
join Contract C on C.Id = D.ClientContractId
join ArticleAccountingPrice AAP on AAP.Id = RWR.ReturnFromClientArticleAccountingPriceId
where RW.ReturnFromClientWaybillStateId = 3

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

