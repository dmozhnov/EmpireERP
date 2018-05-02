BEGIN TRY
	BEGIN TRAN		

    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Article_Certificate]') AND parent_object_id = OBJECT_ID('[Article]'))
	alter table dbo.[Article]  drop constraint FK_Article_Certificate
	
    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ArticleCertificate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
    drop table dbo.[ArticleCertificate]
	
    create table dbo.[ArticleCertificate] (
        Id INT IDENTITY NOT NULL,
       Name VARCHAR(250) not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       primary key (Id)
    )
	
	IF EXISTS
	(
		SELECT [name]
		FROM syscolumns
		WHERE id = object_id(N'[dbo].[Article]') 
		AND OBJECTPROPERTY(id, N'IsUserTable') = 1
		AND [name] = N'CertificateId'
	)
	BEGIN
		ALTER TABLE dbo.[Article] DROP COLUMN CertificateId;
		print 'Столбец найден'
	END

	ALTER TABLE dbo.[Article] ADD CertificateId INT null;

    alter table dbo.[Article] 
        add constraint FK_Article_Certificate 
        foreign key (CertificateId) 
        references dbo.[ArticleCertificate]
	
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8003, 1)
	
	PRINT 'Обновление выполнено успешно'	
		
	--ROLLBACK TRAN -- Очень помогает при отладке, а COMMIT TRAN при отладке комментируем
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
