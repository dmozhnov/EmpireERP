/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.6

  ��� ������:
	+ ����� ��������� ������
		
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

SET @PreviousVersion = '0.9.5' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.6'			-- ����� ����� ������

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

INSERT INTO [Rate] ([Name],[ActiveUserCountLimit],[TeamCountLimit],[StorageCountLimit],[AccountOrganizationCountLimit],[GigabyteCountLimit]
           ,[UseExtraActiveUserOption],[UseExtraTeamOption],[UseExtraStorageOption],[UseExtraAccountOrganizationOption],[UseExtraGigabyteOption]
           ,[ExtraActiveUserOptionCostPerMonth],[ExtraTeamOptionCostPerMonth],[ExtraStorageOptionCostPerMonth],[ExtraAccountOrganizationOptionCostPerMonth]
           ,[ExtraGigabyteOptionCostPerMonth],[BaseCostPerMonth])
           
SELECT '�������� ������', 
	[ActiveUserCountLimit] = 1000,
	[TeamCountLimit] = 1000,
	[StorageCountLimit] = 1000,
	[AccountOrganizationCountLimit] = 1000,
	[GigabyteCountLimit] = 10,
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

GO

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO




-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

