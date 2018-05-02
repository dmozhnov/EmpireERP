BEGIN TRY
	BEGIN TRAN		
	
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24001, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24002, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24101, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24102, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24103, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24201, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24202, 1)
	
	PRINT '���������� ��������� �������'	
		
	--ROLLBACK TRAN -- ����� �������� ��� �������, � COMMIT TRAN ��� ������� ������������
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
