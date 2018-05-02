/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.51

  ��� ������:
	+ ��������� ������� CreatedBy, AcceptedBy, ShippedBy � �.�. � ������� ���������
	
	* ��������� �������� ���� ����������� ��������
	
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

SET @PreviousVersion = '0.9.50' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.51'			-- ����� ����� ������

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

--������������, ������� ������ ��������� ����������

alter table dbo.[ExpenditureWaybill] add ExpenditureWaybillShippedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ExpenditureWaybill] 
add constraint FK_ExpenditureWaybill_ShippedBy 
foreign key (ExpenditureWaybillShippedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [ExpenditureWaybill]
set ExpenditureWaybillShippedById = s.SaleWaybillCuratorId
from [ExpenditureWaybill] e
join SaleWaybill s on s.id = e.id
where ShippingDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� ����������

alter table dbo.[SaleWaybill] add SaleWaybillCreatedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
       
alter table dbo.[SaleWaybill] 
add constraint FK_SaleWaybill_CreatedBy 
foreign key (SaleWaybillCreatedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
      
update SaleWaybill
set SaleWaybillCreatedById = SaleWaybillCuratorId
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO              
       
alter table dbo.[SaleWaybill] alter column SaleWaybillCreatedById INT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� ����������

alter table dbo.[SaleWaybill] add SaleWaybillAcceptedById INT
       
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[SaleWaybill] 
add constraint FK_SaleWaybill_AcceptedBy 
foreign key (SaleWaybillAcceptedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update s
set SaleWaybillAcceptedById = s.SaleWaybillCuratorId
from [ExpenditureWaybill] e
join SaleWaybill s on s.id = e.id
where e.AcceptanceDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

---------------------------------

--������������, ������� ������ ��������� ��������

alter table dbo.[WriteoffWaybill] add WriteoffWaybillWrittenoffById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[WriteoffWaybill] 
add constraint FK_WriteoffWaybill_WrittenoffBy 
foreign key (WriteoffWaybillWrittenoffById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [WriteoffWaybill]
set WriteoffWaybillWrittenoffById = WriteoffWaybillCuratorId
from [WriteoffWaybill] 
where WriteoffDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� ��������

alter table dbo.[WriteoffWaybill] add WriteoffWaybillCreatedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
       
alter table dbo.[WriteoffWaybill] 
add constraint FK_WriteoffWaybill_CreatedBy 
foreign key (WriteoffWaybillCreatedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
      
update WriteoffWaybill
set WriteoffWaybillCreatedById = WriteoffWaybillCuratorId
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO              
       
alter table dbo.[WriteoffWaybill] alter column WriteoffWaybillCreatedById INT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� ��������

alter table dbo.[WriteoffWaybill] add WriteoffWaybillAcceptedById INT
       
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[WriteoffWaybill] 
add constraint FK_WriteoffWaybill_AcceptedBy 
foreign key (WriteoffWaybillAcceptedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [WriteoffWaybill]
set WriteoffWaybillAcceptedById = WriteoffWaybillCuratorId
from [WriteoffWaybill]
where AcceptanceDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

---------------------------------
--������������, ������� ������ ��������� ����� ������������

alter table dbo.[ChangeOwnerWaybill] add ChangeOwnerWaybillChangedOwnerById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ChangeOwnerWaybill] 
add constraint FK_ChangeOwnerWaybill_ChangedOwnerBy 
foreign key (ChangeOwnerWaybillChangedOwnerById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [ChangeOwnerWaybill]
set ChangeOwnerWaybillChangedOwnerById = ChangeOwnerWaybillCuratorId
from [ChangeOwnerWaybill] 
where ChangeOwnerDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� ����� ������������

alter table dbo.[ChangeOwnerWaybill] add ChangeOwnerWaybillCreatedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
       
alter table dbo.[ChangeOwnerWaybill] 
add constraint FK_ChangeOwnerWaybill_CreatedBy 
foreign key (ChangeOwnerWaybillCreatedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
      
update ChangeOwnerWaybill
set ChangeOwnerWaybillCreatedById = ChangeOwnerWaybillCuratorId
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO              
       
alter table dbo.[ChangeOwnerWaybill] alter column ChangeOwnerWaybillCreatedById INT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� ����� ������������

alter table dbo.[ChangeOwnerWaybill] add ChangeOwnerWaybillAcceptedById INT
       
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ChangeOwnerWaybill] 
add constraint FK_ChangeOwnerWaybill_AcceptedBy 
foreign key (ChangeOwnerWaybillAcceptedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [ChangeOwnerWaybill]
set ChangeOwnerWaybillAcceptedById = ChangeOwnerWaybillCuratorId
from [ChangeOwnerWaybill]
where AcceptanceDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

---------------------------------
--������������, ������� ������ ��������� �����������

alter table dbo.[MovementWaybill] add MovementWaybillReceiptedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[MovementWaybill] 
add constraint FK_MovementWaybill_ReceiptedBy 
foreign key (MovementWaybillReceiptedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [MovementWaybill]
set MovementWaybillReceiptedById = CuratorId
from [MovementWaybill] 
where ReceiptDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� �����������

alter table dbo.[MovementWaybill] add MovementWaybillCreatedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
       
alter table dbo.[MovementWaybill] 
add constraint FK_MovementWaybill_CreatedBy 
foreign key (MovementWaybillCreatedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
      
update MovementWaybill
set MovementWaybillCreatedById = CuratorId
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO              
       
alter table dbo.[MovementWaybill] alter column MovementWaybillCreatedById INT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� �����������

alter table dbo.[MovementWaybill] add MovementWaybillAcceptedById INT
       
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[MovementWaybill] 
add constraint FK_MovementWaybill_AcceptedBy 
foreign key (MovementWaybillAcceptedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [MovementWaybill]
set MovementWaybillAcceptedById = CuratorId
from [MovementWaybill]
where AcceptanceDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� �������� ��������� �����������

alter table dbo.[MovementWaybill] add MovementWaybillShippedById INT
       
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[MovementWaybill] 
add constraint FK_MovementWaybill_ShippedBy 
foreign key (MovementWaybillShippedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [MovementWaybill]
set MovementWaybillShippedById = CuratorId
from [MovementWaybill]
where ShippingDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

---------------------------------
--������������, ������� ������ ��������� �������

alter table dbo.[ReceiptWaybill] add ReceiptWaybillReceiptedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ReceiptWaybill] 
add constraint FK_ReceiptWaybill_ReceiptedBy 
foreign key (ReceiptWaybillReceiptedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [ReceiptWaybill]
set ReceiptWaybillReceiptedById = CuratorId
from [ReceiptWaybill] 
where ReceiptDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� �������

alter table dbo.[ReceiptWaybill] add ReceiptWaybillCreatedById INT
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
       
alter table dbo.[ReceiptWaybill] 
add constraint FK_ReceiptWaybill_CreatedBy 
foreign key (ReceiptWaybillCreatedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
      
update ReceiptWaybill
set ReceiptWaybillCreatedById = CuratorId
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO              
       
alter table dbo.[ReceiptWaybill] alter column ReceiptWaybillCreatedById INT not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ������ ��������� �������

alter table dbo.[ReceiptWaybill] add ReceiptWaybillAcceptedById INT
       
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ReceiptWaybill] 
add constraint FK_ReceiptWaybill_AcceptedBy 
foreign key (ReceiptWaybillAcceptedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [ReceiptWaybill]
set ReceiptWaybillAcceptedById = CuratorId
from [ReceiptWaybill]
where AcceptanceDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

--������������, ������� ���������� ��������� �������

alter table dbo.[ReceiptWaybill] add ReceiptWaybillApprovedById INT
       
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table dbo.[ReceiptWaybill] 
add constraint FK_ReceiptWaybill_ApprovedBy 
foreign key (ReceiptWaybillApprovedById) 
references dbo.[User]
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update [ReceiptWaybill]
set ReceiptWaybillApprovedById = CuratorId
from [ReceiptWaybill]
where ApprovementDate is not null
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

