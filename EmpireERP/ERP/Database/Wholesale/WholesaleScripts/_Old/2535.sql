BEGIN TRY
	BEGIN TRAN		
	
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3609, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3610, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3611, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3612, 1)

	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3512, 1)
	
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
