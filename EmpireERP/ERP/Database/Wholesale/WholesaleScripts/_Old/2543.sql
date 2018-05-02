BEGIN TRY
	BEGIN TRAN		
	
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24301, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24302, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24401, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24402, 1)
	
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
