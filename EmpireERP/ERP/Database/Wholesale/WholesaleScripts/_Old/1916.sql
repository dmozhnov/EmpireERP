BEGIN TRY
    BEGIN TRAN

	EXEC sp_rename 'ReceiptWaybillRow.AcceptedCount', 'ApprovedCount';
	
	EXEC sp_rename 'ReceiptWaybill.AcceptanceDate', 'ApprovementDate';
	
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1312, 1)
	INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1313, 1)
	
    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
