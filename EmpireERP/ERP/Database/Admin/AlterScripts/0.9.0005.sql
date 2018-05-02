/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.5

  ��� ������:
	+ ����� �������� �����
	
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


update Rate
set name = '����������', 
	[ActiveUserCountLimit] = 3,
	[TeamCountLimit] = 1,
	[StorageCountLimit] = 1,
	[AccountOrganizationCountLimit] = 1,
	[GigabyteCountLimit] = 1,
	[UseExtraActiveUserOption] = 0,
	[UseExtraTeamOption] = 0,
	[UseExtraStorageOption] = 0,
	[UseExtraAccountOrganizationOption] = 0,
	[UseExtraGigabyteOption] = 0,
	[ExtraActiveUserOptionCostPerMonth] = 0,
	[ExtraTeamOptionCostPerMonth] = 0,
	[ExtraStorageOptionCostPerMonth] = 0,
	[ExtraAccountOrganizationOptionCostPerMonth] = 0,
	[ExtraGigabyteOptionCostPerMonth] = 0,
	[BaseCostPerMonth] = 0
where id = 1
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


update Rate
set name = '��������', 
	[ActiveUserCountLimit] = 3,
	[TeamCountLimit] = 1,
	[StorageCountLimit] = 1,
	[AccountOrganizationCountLimit] = 1,
	[GigabyteCountLimit] = 1,
	[UseExtraActiveUserOption] = 1,
	[UseExtraTeamOption] = 1,
	[UseExtraStorageOption] = 1,
	[UseExtraAccountOrganizationOption] = 1,
	[UseExtraGigabyteOption] = 1,
	[ExtraActiveUserOptionCostPerMonth] = 570,
	[ExtraTeamOptionCostPerMonth] = 1200,
	[ExtraStorageOptionCostPerMonth] = 315,
	[ExtraAccountOrganizationOptionCostPerMonth] = 315,
	[ExtraGigabyteOptionCostPerMonth] = 300,
	[BaseCostPerMonth] = 990
where id = 2
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO


-- �������� �����
SET IDENTITY_INSERT Rate ON
GO

INSERT INTO [Rate] ([Id], [Name],[ActiveUserCountLimit],[TeamCountLimit],[StorageCountLimit],[AccountOrganizationCountLimit],[GigabyteCountLimit]
           ,[UseExtraActiveUserOption],[UseExtraTeamOption],[UseExtraStorageOption],[UseExtraAccountOrganizationOption],[UseExtraGigabyteOption]
           ,[ExtraActiveUserOptionCostPerMonth],[ExtraTeamOptionCostPerMonth],[ExtraStorageOptionCostPerMonth],[ExtraAccountOrganizationOptionCostPerMonth]
           ,[ExtraGigabyteOptionCostPerMonth],[BaseCostPerMonth])

SELECT 
    [Id] = 3,
    [Name] = '������', 
	[ActiveUserCountLimit] = 10,
	[TeamCountLimit] = 3,
	[StorageCountLimit] = 32767,
	[AccountOrganizationCountLimit] = 32767,
	[GigabyteCountLimit] = 3,
	[UseExtraActiveUserOption] = 1,
	[UseExtraTeamOption] = 1,
	[UseExtraStorageOption] = 0,
	[UseExtraAccountOrganizationOption] = 0,
	[UseExtraGigabyteOption] = 1,
	[ExtraActiveUserOptionCostPerMonth] = 570,
	[ExtraTeamOptionCostPerMonth] = 1200,
	[ExtraStorageOptionCostPerMonth] = 0,
	[ExtraAccountOrganizationOptionCostPerMonth] = 0,
	[ExtraGigabyteOptionCostPerMonth] = 300,
	[BaseCostPerMonth] = 10990

UNION
SELECT [Id] = 4,
    [Name] = '����������', 
	[ActiveUserCountLimit] = 250,
	[TeamCountLimit] = 32767,
	[StorageCountLimit] = 32767,
	[AccountOrganizationCountLimit] = 32767,
	[GigabyteCountLimit] = 50,
	[UseExtraActiveUserOption] = 1,
	[UseExtraTeamOption] = 0,
	[UseExtraStorageOption] = 0,
	[UseExtraAccountOrganizationOption] = 0,
	[UseExtraGigabyteOption] = 1,
	[ExtraActiveUserOptionCostPerMonth] = 570,
	[ExtraTeamOptionCostPerMonth] = 0,
	[ExtraStorageOptionCostPerMonth] = 0,
	[ExtraAccountOrganizationOptionCostPerMonth] = 0,
	[ExtraGigabyteOptionCostPerMonth] = 300,
	[BaseCostPerMonth] = 34990
GO

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

SET IDENTITY_INSERT Rate OFF
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

