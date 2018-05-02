/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.22

  ��� ������:
	+ ��������� ��� ����� ������� ����������� ������� - AcceptedPurchaseIndicator, ApprovedPurchaseIndicator
	+ ��������� ���������������� �������
	* ��������, �� ����� �� � �����-������ ���� ����� "�������� �������" ����, ��� � "�������� ���������� ��� � �������" � ���������� ���, ����� ��� ���� �����, ���� ������� �������.
	* ��������, ��� � ������� ��� ����� ��� ���������, ��� �������� ���� ������������ ��������� �������� ��������, ������� ��� ������������.
	
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

SET @PreviousVersion = '0.9.21' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.22'			-- ����� ����� ������

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

	declare @SimultaneousCount int;

	set @SimultaneousCount = (select COUNT(*) from ReceiptWaybill R1 join ReceiptWaybill R2
	on 
	(
		R1.AcceptanceDate = R2.AcceptanceDate or
		R1.AcceptanceDate = R2.ReceiptDate or
		R1.AcceptanceDate = R2.ApprovementDate or	
		
		R1.ReceiptDate = R2.ReceiptDate or
		R1.ReceiptDate = R2.ApprovementDate or	
		
		R1.ApprovementDate = R2.ApprovementDate)
		
	and R1.Id <> R2.Id)

IF @SimultaneousCount > 0
RAISERROR('���������� ���� ���������, ��� �������� ���� ������������ ��������� �������� ��������, ������� ��� ������������. ���������� � ������������.', 16, 1)

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    create table dbo.[AcceptedPurchaseIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ContractorId INT not null,
       ContractorOrganizationId INT not null,
       ContractId INT null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       primary key (Id)
    )
    
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

    create table dbo.[ApprovedPurchaseIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ContractorId INT not null,
       ContractorOrganizationId INT not null,
       ContractId INT null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       primary key (Id)
    )

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

create table #T(
	Number varchar(25) not null,
	AcceptanceDate DATETIME not null,
	ReceiptDate DATETIME null,
	ApprovementDate DATETIME null,
	ReceiptWaybillReceiptStorageId SMALLINT not null,
	CuratorId INT not null,
	ProviderId INT null,
	ContractorOrganizationId INT not null,
	ProviderContractId INT null,
	AccountOrganizationId INT not null,
	ArticleId INT not null,
	IsDivergence bit not null,
	IsReceipted bit not null,
	IsApproved bit not null, 
	AcceptedIndicatorAcceptedCount DECIMAL(18, 6) null,
	AcceptedIndicatorAcceptedSum DECIMAL(18, 6) null,
	AcceptedIndicatorReceiptedCount DECIMAL(18, 6) null,
	AcceptedIndicatorReceiptedSum DECIMAL(18, 6) null,
	AcceptedIndicatorApprovedCount DECIMAL(18, 6) null,
	AcceptedIndicatorApprovedSum DECIMAL(18, 6) null,
	
	ApprovedIndicatorReceiptedCount DECIMAL(18, 6) null,
	ApprovedIndicatorReceiptedSum DECIMAL(18, 6) null,
	ApprovedIndicatorApprovedCount DECIMAL(18, 6) null,
	ApprovedIndicatorApprovedSum DECIMAL(18, 6) null,
	PendingCount DECIMAL(18, 6) null,
	ApprovedCount DECIMAL(18, 6) null,
	ProviderCount DECIMAL(18, 6) null,
	ReceiptedCount DECIMAL(18, 6) null,
	PendingSum DECIMAL(18, 6) null,
	ApprovedSum DECIMAL(18, 6) null,
	ProviderSum DECIMAL(18, 6) null	
)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into #T
select 
   RW.Number, RW.AcceptanceDate, RW.ReceiptDate, RW.ApprovementDate, RW.ReceiptWaybillReceiptStorageId, RW.CuratorId, RW.ProviderId, C.ContractorOrganizationId,
	 RW.ProviderContractId, RW.AccountOrganizationId, RR.ArticleId, 
	 
	 IsDivergence = case when 
	 ((RR.ReceiptedCount is not null and RR.ProviderCount is not null) 
		and (RR.PendingCount <> RR.ReceiptedCount or RR.PendingCount <> RR.ProviderCount))
	 or
	 (ProviderSum is not null and ROUND(RR.InitialPurchaseCost * RR.PendingCount, 2) <> ProviderSum)	 
	 then 1 else 0 end,
	 	 
	 IsReceipted = case when RW.ReceiptDate is not null then 1 else 0 end,
	 IsApproved = case when RW.ApprovementDate is not null then 1 else 0 end,
	 
	 AcceptedIndicatorAcceptedCount = null,	 
	 AcceptedIndicatorAcceptedSum = null,
	 AcceptedIndicatorReceiptedCount = null,	 
	 AcceptedIndicatorReceiptedSum = null,
	 AcceptedIndicatorApprovedCount = null,	 
	 AcceptedIndicatorApprovedSum = null,
	
	 ApprovedIndicatorReceiptedCount = null,	 
	 ApprovedIndicatorReceiptedSum = null,
	 ApprovedIndicatorApprovedCount = null,	 
	 ApprovedIndicatorApprovedSum = null,
	 
	 RR.PendingCount, RR.ApprovedCount, RR.ProviderCount, RR.ReceiptedCount,
	 RR.PendingSum, RR.ApprovedSum, RR.ProviderSum	

from ReceiptWaybillRow RR
join ReceiptWaybill RW on RR.ReceiptWaybillId = RW.Id
join [Contract] C on C.Id = RW.ProviderContractId
where RR.DeletionDate is null and RW.DeletionDate is null and RW.AcceptanceDate is not null

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update #T
set
	AcceptedIndicatorAcceptedCount = PendingCount,
	AcceptedIndicatorAcceptedSum = PendingSum,
	
	AcceptedIndicatorReceiptedCount = case
		when IsReceipted = 1 and IsDivergence = 1 then -PendingCount
		else null
	end,
	
	AcceptedIndicatorReceiptedSum = case
		when IsReceipted = 1 and IsDivergence = 1 then -PendingSum
		else null
	end,
	
	AcceptedIndicatorApprovedCount = case
		when IsApproved = 1 and IsDivergence = 1 then ApprovedCount
		else null
	end,
	
	AcceptedIndicatorApprovedSum = case
		when IsApproved = 1  and IsDivergence = 1 then ApprovedSum
		else null
	end,
	
	ApprovedIndicatorReceiptedCount = case
		when IsReceipted = 1 and IsDivergence = 0 then ApprovedCount
		else null
	end,
	
	ApprovedIndicatorReceiptedSum = case
		when IsReceipted = 1 and IsDivergence = 0 then ApprovedSum
		else null
	end,
	
	ApprovedIndicatorApprovedCount = case
		when IsApproved = 1 and IsDivergence = 1 then ApprovedCount
		else null
	end,
	
	ApprovedIndicatorApprovedSum = case
		when IsApproved = 1 and IsDivergence = 1 then ApprovedSum
		else null
	end

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

create table #IndicatorState (       
       [Date] DATETIME not null,       
       StorageId SMALLINT not null,
       UserId INT not null,
       ContractorId INT not null,
       ContractorOrganizationId INT not null,
       ContractId INT null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       AcceptedPurchaseCostSum DECIMAL(18, 6) null,
       AcceptedCount DECIMAL(18, 6) null,
       ApprovedPurchaseCostSum DECIMAL(18, 6) null,
       ApprovedCount DECIMAL(18, 6) null,
       RowNumber INT not null
    )

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into #IndicatorState
select
	[Date] = T.[AcceptanceDate], 	
	StorageId = T.ReceiptWaybillReceiptStorageId, 
	UserId = T.CuratorId, 
	ContractorId = T.ProviderId, 
	ContractorOrganizationId = T.ContractorOrganizationId,
	ContractId = T.ProviderContractId,
	AccountOrganizationId = T.AccountOrganizationId,
	ArticleId = T.ArticleId,	
	AcceptedPurchaseCostSum = AcceptedIndicatorAcceptedSum, 		
	AcceptedCount = AcceptedIndicatorAcceptedCount,
	ApprovedPurchaseCostSum = null, 		
	ApprovedCount = null,
	RowNumber = 0
from #T T
where AcceptanceDate is not null

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into #IndicatorState
select 
	T.[ReceiptDate], 	
	StorageId = T.ReceiptWaybillReceiptStorageId, 
	UserId = T.CuratorId, 
	ContractorId = T.ProviderId, 
	ContractorOrganizationId = T.ContractorOrganizationId,
	ContractId = T.ProviderContractId,
	AccountOrganizationId = T.AccountOrganizationId,
	ArticleId = T.ArticleId,	
	AcceptedPurchaseCostSum = AcceptedIndicatorReceiptedSum, 		
	AcceptedCount = AcceptedIndicatorReceiptedCount,
	ApprovedPurchaseCostSum = ApprovedIndicatorReceiptedSum, 		
	ApprovedCount = ApprovedIndicatorReceiptedCount,
	RowNumber = 0
from #T T
where ReceiptDate is not null

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into #IndicatorState
select
	T.[ApprovementDate], 	
	StorageId = T.ReceiptWaybillReceiptStorageId, 
	UserId = T.CuratorId, 
	ContractorId = T.ProviderId, 
	ContractorOrganizationId = T.ContractorOrganizationId,
	ContractId = T.ProviderContractId,
	AccountOrganizationId = T.AccountOrganizationId,
	ArticleId = T.ArticleId,	
	AcceptedPurchaseCostSum = AcceptedIndicatorApprovedSum, 		
	AcceptedCount = AcceptedIndicatorApprovedCount,
	ApprovedPurchaseCostSum = ApprovedIndicatorApprovedSum, 		
	ApprovedCount = ApprovedIndicatorApprovedCount,
	RowNumber = 0
from #T T
where ApprovementDate is not null


-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

select Id = newid(), ins.[Date], ins.StorageId, ins.UserId, ins.ContractorId, ins.ContractorOrganizationId, ins.ContractId, ins.AccountOrganizationId,
	ins.ArticleId, ins.AcceptedPurchaseCostSum, ins.AcceptedCount,
	
	-- ���-���� ��� ��������� ���������� ������� ����� � �����
	KeyHash = HashBytes('SHA1', 
		CAST(ins.StorageId as varchar(255)) + '_' +
		CAST(ins.UserId as varchar(255)) + '_' +
		CAST(ins.ContractorId as varchar(255)) + '_' +
		CAST(ins.ContractorOrganizationId as varchar(255)) + '_' +
		CAST(ins.ContractId as varchar(255)) + '_' +
		CAST(ins.AccountOrganizationId as varchar(255)) + '_' +
		CAST(ins.ArticleId as varchar(255))),
	
	RowNumber = ROW_NUMBER() OVER ( partition by ins.StorageId, ins.UserId, ins.ContractorId, ins.ContractorOrganizationId, 
	ins.ContractId, ins.AccountOrganizationId, ins.ArticleId
	 order by ins.[Date]
	)
into #AcceptedIndicatorStateOrdered
from #IndicatorState ins
where AcceptedCount is not null or AcceptedPurchaseCostSum is not null

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

select Id = newid(), ins.[Date], ins.StorageId, ins.UserId, ins.ContractorId, ins.ContractorOrganizationId, ins.ContractId, ins.AccountOrganizationId,
	ins.ArticleId, ins.ApprovedPurchaseCostSum, ins.ApprovedCount,
	
	-- ���-���� ��� ��������� ���������� ������� ����� � �����
	KeyHash = HashBytes('SHA1', 
		CAST(ins.StorageId as varchar(255)) + '_' +
		CAST(ins.UserId as varchar(255)) + '_' +
		CAST(ins.ContractorId as varchar(255)) + '_' +
		CAST(ins.ContractorOrganizationId as varchar(255)) + '_' +
		CAST(ins.ContractId as varchar(255)) + '_' +
		CAST(ins.AccountOrganizationId as varchar(255)) + '_' +
		CAST(ins.ArticleId as varchar(255))),
	
	RowNumber = ROW_NUMBER() OVER ( partition by ins.StorageId, ins.UserId, ins.ContractorId, ins.ContractorOrganizationId, 
	ins.ContractId, ins.AccountOrganizationId, ins.ArticleId
	 order by ins.[Date]
	)
into #ApprovedIndicatorStateOrdered
from #IndicatorState ins
where ApprovedCount is not null or ApprovedPurchaseCostSum is not null

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into AcceptedPurchaseIndicator (Id, StartDate, EndDate, StorageId, UserId, ContractorId, ContractorOrganizationId, ContractId, AccountOrganizationId,
   ArticleId, PreviousId, PurchaseCostSum, [Count])
select T.Id, T.[Date], 
	(select [Date] from #AcceptedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber = T.RowNumber + 1),
	T.StorageId, T.UserId, T.ContractorId, T.ContractorOrganizationId, T.ContractId,
	T.AccountOrganizationId, T.ArticleId,
	PreviousId = (select Id from #AcceptedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber = T.RowNumber - 1),
	PurchaseCostSum = (select SUM(AcceptedPurchaseCostSum) from #AcceptedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),	
	[Count] = (select SUM(AcceptedCount) from #AcceptedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber <= T.RowNumber)
from #AcceptedIndicatorStateOrdered T

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

insert into ApprovedPurchaseIndicator (Id, StartDate, EndDate, StorageId, UserId, ContractorId, ContractorOrganizationId, ContractId, AccountOrganizationId,
   ArticleId, PreviousId, PurchaseCostSum, [Count])
select T.Id, T.[Date], 
	(select [Date] from #ApprovedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber = T.RowNumber + 1),
	T.StorageId, T.UserId, T.ContractorId, T.ContractorOrganizationId, T.ContractId,
	T.AccountOrganizationId, T.ArticleId,
	PreviousId = (select Id from #ApprovedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber = T.RowNumber - 1),
	PurchaseCostSum = (select SUM(ApprovedPurchaseCostSum) from #ApprovedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber <= T.RowNumber),	
	[Count] = (select SUM(ApprovedCount) from #ApprovedIndicatorStateOrdered where KeyHash = T.KeyHash and RowNumber <= T.RowNumber)
from #ApprovedIndicatorStateOrdered T

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

  update PermissionDistribution 
  set PermissionDistribution.PermissionDistributionTypeId = P2.PermissionDistributionTypeId
  from PermissionDistribution join PermissionDistribution P2
  on PermissionDistribution.RoleId = P2.RoleId
  and PermissionDistribution.PermissionId = 2
  and 
  ((P2.PermissionId = 1301 and PermissionDistribution.PermissionDistributionTypeId > P2.PermissionDistributionTypeId) or P2.PermissionId is null)
  
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO	

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

