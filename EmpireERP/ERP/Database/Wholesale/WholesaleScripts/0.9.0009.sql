/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.9

  ��� ������:
	* �������� ����� �� ����� ������, ���� ����� ������, ���������� ���� ������
	������ ���� �������� ���:
		�������� ������ ������ (������� �� �����(������ � ������))
		���� � �������������� ������ ������ (������� �� �������� ������ ������)
		������� �� ��������� ���� ������ (������� �� �������� ������ ������)
		������� �� ���������� ���� ������ (������� �� �������� ������ ������)
		������� �� ���� ������ ����������� �������� (������� �� �������� ������ ������)

		�������� ����� ����� (������� �� �����(������ � ������))
		�������� ������ � ���� (������� �� �������� ����� �����)
		�������������� ������� � ����� (������� �� �������� ����� �����)
		�������� ������� � ����� (������� �� �������� ����� �����)

		�������� ���. ����� (������� �� �����(������ � ������))
		���� � �������������� ���. ����� (������� �� �������� ���. �����)
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

SET @PreviousVersion = '0.9.8' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.9'			-- ����� ����� ������

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

	-- ��������� ������� �������� ��� ����������� (�����) ����
	DECLARE roleCursor CURSOR
		LOCAL FAST_FORWARD READ_ONLY
		FOR SELECT Id
		FROM [Role];
	DECLARE @roleId smallint
	OPEN roleCursor
	FETCH roleCursor
		INTO @roleId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 20901, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21002, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21003, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21004, @roleId;

		insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
		select 0, 21005, @roleId;

		FETCH NEXT FROM roleCursor
			INTO @roleId
	END
	CLOSE roleCursor
	DEALLOCATE roleCursor

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 2 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ �������������� (���� 1) ��������������� "���" �� ��� ����� �����
UPDATE P
	SET P.[PermissionDistributionTypeId] = 3
	FROM dbo.[PermissionDistribution] P
	WHERE [RoleId] = 1 AND (
		[PermissionId] = 20901 OR
		[PermissionId] = 21002 OR
		[PermissionId] = 21003 OR
		[PermissionId] = 21004 OR
		[PermissionId] = 21005);
-- ����� ���� �� ��� �������� �� [PermissionId], �� ���������

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 7 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ������������ �����
UPDATE P
	SET P.[PermissionId] = 20902
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20003;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 8 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ������������ �����
UPDATE P
	SET P.[PermissionId] = 20903
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20008;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 9 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ������������ �����
UPDATE P
	SET P.[PermissionId] = 20904
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20009;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 10 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ������������ �����
UPDATE P
	SET P.[PermissionId] = 20905
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20010;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 11 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ������������ �����
UPDATE P
	SET P.[PermissionId] = 21001
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20011;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 12 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- ������ ������������ �����
UPDATE P
	SET P.[PermissionId] = 21006
	FROM dbo.[PermissionDistribution] P
	WHERE [PermissionId] = 20004;

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 13 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- �������� � ����� ��� ����,
-- � ���� 21001 ����� (�������� ����� �����) ����� ��������������� 0 ��� ��� �� � ����,
-- � ���������� ����� 20601 (�������� ������ � ������� ����� �� �������), ������������ �� ��������
-- � ����� 21001.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
DECLARE @permissionDistributionTypeIdProductionOrder smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- ���������, ���� �� ������ ��� ������ ���� � ����� "�����. ����� �����" (���� ���, �� -1)
    set @permissionDistributionTypeId = -1;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 21001;

    -- ���������, ���� �� ������ ��� ������ ���� � ����� "������� ������: ������" (���� ���, �� 0)
    set @permissionDistributionTypeIdPayments = 0;
	SELECT TOP 1 @permissionDistributionTypeIdPayments = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 20601;

    -- ���������, ���� �� ������ ��� ������ ���� � ����� "�����. ������� ������" (���� ���, �� 0)
    set @permissionDistributionTypeIdProductionOrder = 0;
	SELECT TOP 1 @permissionDistributionTypeIdProductionOrder = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 20001;

	IF (@permissionDistributionTypeId <= 0 AND @permissionDistributionTypeIdPayments > 0)
	BEGIN
	    DECLARE @newDistributionTypeId smallint;
	    IF (@permissionDistributionTypeIdPayments < @permissionDistributionTypeIdProductionOrder)
			SET @newDistributionTypeId = @permissionDistributionTypeIdPayments
		ELSE
			SET @newDistributionTypeId = @permissionDistributionTypeIdProductionOrder;
		
		IF (@permissionDistributionTypeId = -1)
		BEGIN
			--print '������������ �������.';
			insert into [PermissionDistribution] ([PermissionDistributionTypeId],[PermissionId],[RoleId])
				select @newDistributionTypeId, 21001, @roleId;
		END
		ELSE
		BEGIN
			--print '������������ ������.';
			UPDATE P
				SET P.PermissionDistributionTypeId = @newDistributionTypeId
				FROM [PermissionDistribution] P
				WHERE [PermissionId] = 21001 AND [RoleId] = @roleId;
		END
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 14 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- �������� � ����� ��� ����,
-- � ���� 20001 ����� (�������� �������) ����� ��������������� ������ ��� "������",
-- ������������ �� �������� � ����� 20901 "�������� ������ ������" (��� ��� �������), ��� ������ 3 ������� 3.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- ���������, ���� �� ������ ��� ������ ���� � ����� "�������� �������" (���� ���, �� 0)
    set @permissionDistributionTypeId = 0;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 20001;

	IF (@permissionDistributionTypeId > 0)
	BEGIN
		--print '������������ ������.';
		UPDATE P
			SET P.PermissionDistributionTypeId = @permissionDistributionTypeId
			FROM [PermissionDistribution] P
			WHERE [PermissionId] = 20901 AND [RoleId] = @roleId;
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 15 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- �������� � ����� ��� ����,
-- � ���� 21006 ����� (���� � ���. ���. �����) ����� ��������������� ������ ��� "������",
-- ������������ �� �������� � ����� 21005 "�������� ���. �����" (��� ��� �������), ��� ������ 3 ������� 3.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- ���������, ���� �� ������ ��� ������ ���� � ����� "���� � ���. ���. �����" (���� ���, �� 0)
    set @permissionDistributionTypeId = 0;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 21006;

	IF (@permissionDistributionTypeId > 0)
	BEGIN
		--print '������������ ������.';
		UPDATE P
			SET P.PermissionDistributionTypeId = @permissionDistributionTypeId
			FROM [PermissionDistribution] P
			WHERE [PermissionId] = 21005 AND [RoleId] = @roleId;
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 16 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- �������� � ����� ��� ����,
-- � ���� 21001 ����� (�����. ����� �����) ����� ��������������� ������ ��� "������",
-- ������������ �� �������� � ����� 21002 21003 "����-��� ����. �����" (��� ��� �������), ��� ������ 3 ������� 3.
DECLARE roleCursor2 CURSOR
	LOCAL FAST_FORWARD READ_ONLY
	FOR SELECT Id
	FROM [Role];
DECLARE @roleId smallint
DECLARE @permissionDistributionTypeId smallint
DECLARE @permissionDistributionTypeIdPayments smallint
OPEN roleCursor2
FETCH roleCursor2
	INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    -- ���������, ���� �� ������ ��� ������ ���� � ����� "���� � ���. ���. �����" (���� ���, �� 0)
    set @permissionDistributionTypeId = 0;
	SELECT TOP 1 @permissionDistributionTypeId = [PermissionDistributionTypeId] FROM [PermissionDistribution]
		WHERE [RoleId] = @roleId AND [PermissionId] = 21001;

	IF (@permissionDistributionTypeId > 0)
	BEGIN

		UPDATE P
			SET P.PermissionDistributionTypeId = @permissionDistributionTypeId
			FROM [PermissionDistribution] P
			WHERE ([PermissionId] = 21002 OR [PermissionId] = 21003) AND [RoleId] = @roleId;
	END;

	FETCH NEXT FROM roleCursor2
		INTO @roleId
END
CLOSE roleCursor2
DEALLOCATE roleCursor2

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� 17 ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

