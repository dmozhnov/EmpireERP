-- Создание таблицы настроек системы
GO

alter table dbo.Setting add DataBaseVersion varchar(15) not null 
GO

 --устанавливаем версию БД по умолчанию
INSERT INTO dbo.Setting(
	Id
	,DataBaseVersion
	,UseReadyToAcceptStateForMovementWaybill				
	,UseReadyToAcceptStateForChangeOwnerWaybill			
	,UseReadyToAcceptStateForExpenditureWaybill			
	,UseReadyToAcceptStateForReturnFromClientWaybill	
	,UseReadyToAcceptStateForWriteOffWaybill					
	,ActiveUserCountLimit
	,TeamCountLimit
	,StorageCountLimit
	,AccountOrganizationCountLimit
	,GigabyteCountLimit
	)
VALUES(
	1	--Id
	,'0.9.1'	--DataBaseVersion
	,0	--UseReadyToAcceptStateForMovementWaybill				
	,0	--UseReadyToAcceptStateForChangeOwnerWaybill			
	,0	--UseReadyToAcceptStateForExpenditureWaybill			
	,0	--UseReadyToAcceptStateForReturanFromClientWaybill	 
	,0	--UseReadyToAcceptStateForWriteOffWaybill				
	,0  --ActiveUserCountLimit
	,0  --TeamCountLimit
	,0  --StorageCountLimit
	,0  --AccountOrganizationCountLimit
	,0  --GigabyteCountLimit
	)

-- создание первого пользователя системы (администратора)
GO
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Employee_CreatedBy]') AND parent_object_id = OBJECT_ID('[Employee]'))
alter table dbo.[Employee]  drop constraint FK_Employee_CreatedBy

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_UserToEmployee]') AND parent_object_id = OBJECT_ID('[User]'))
alter table dbo.[User]  drop constraint FK_UserToEmployee
	
INSERT INTO [EmployeePost] (Name)
SELECT 'Администратор'

INSERT INTO [Employee]([LastName],[FirstName],[Patronymic],[CreationDate],[EmployeePostId],[CreatedById])
SELECT 'Админов', 'Админ', 'Админович', GETDATE(), 1, 1

INSERT INTO [User]([Id],[DisplayName],[Login],[PasswordHash],[CreationDate],[BlockingDate],[CreatedById],[BlockerId],[DisplayNameTemplate])
SELECT 1, 'Админов А.А.', 'admin', 'c6daac1f5f46a0b7bc0a9322d0909b5f387c5f1192e202e7bc50ba84a96894b91c89d5d4835f954ddbbb0ca0435f9f2e2fde777e17afc4bb7b0184ce8ff9ca98', GETDATE(), NULL, 1, NULL, 'Lfp'

alter table dbo.[Employee] add constraint FK_Employee_CreatedBy foreign key (CreatedById) references dbo.[User]
alter table dbo.[User] add constraint FK_UserToEmployee foreign key (Id) references dbo.[Employee]

INSERT INTO [Role]([Name],[CreationDate],[Comment], [IsSystemAdmin])
SELECT 'Администратор', GETDATE(), '', 1



INSERT INTO [UserRole]([RoleId],[UserId])
SELECT 1, 1

INSERT INTO [Team]([Name],[CreationDate],[CreatedById],[Comment])
SELECT 'Основная команда', GETDATE(), 1, ''

INSERT INTO [UserTeam]([TeamId],[UserId])
SELECT 1, 1

GO

INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1004, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1005, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1006, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1007, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1104, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1105, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1106, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1107, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1108, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1109, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1203, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1301, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1302, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1305, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1306, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1307, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1308, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1309, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1310, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1311, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1312, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1313, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1314, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1315, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1316, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1317, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1318, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1401, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1402, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1404, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1405, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1406, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1407, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1408, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1409, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1410, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1411, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1501, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1502, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1503, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1504, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1505, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1506, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1507, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1508, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1601, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1602, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1603, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1701, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1702, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1703, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1704, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1705, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1706, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1707, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1708, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1709, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1710, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1801, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1802, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1803, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1804, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1805, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1806, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1901, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1902, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1903, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1904, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1905, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1906, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1907, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 1908, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2004, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2005, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2006, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 2007, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3006, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3007, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3104, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3105, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3106, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3107, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3108, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3109, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3203, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3301, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3302, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3303, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3401, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3402, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3403, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3501, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3502, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3505, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3506, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3508, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3509, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3510, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3511, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3512, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3601, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3602, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3603, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3604, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3605, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3606, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3607, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3608, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3609, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3610, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3611, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3612, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3613, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3614, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3615, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3701, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3702, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3703, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3704, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3705, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3706, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3707, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3801, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3802, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3803, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3804, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3901, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3902, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3903, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3904, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3905, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3906, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3907, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3908, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3909, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3910, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3911, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 3912, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4104, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4105, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4106, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 4202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5004, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5005, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5006, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5104, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5105, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5106, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5107, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5108, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5109, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5110, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5111, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5112, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5203, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5204, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5301, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5302, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 5303, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7203, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7204, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7301, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7302, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7303, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7401, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7402, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7403, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7501, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7502, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7503, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7601, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7602, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7603, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7701, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7702, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7703, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7801, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7802, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7803, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7901, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7902, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7903, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 7904, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 8203, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20005, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20006, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20007, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20008, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20104, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20105, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20106, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20107, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20203, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20204, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20205, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20206, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20207, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20208, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20209, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20210, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20211, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20212, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20213, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20214, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20215, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20216, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20217, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20218, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20219, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20220, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20221, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20222, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20223, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20301, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20302, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20303, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20401, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20402, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20403, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20501, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20502, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20503, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20601, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20602, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20603, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20701, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20702, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20703, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20801, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20802, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20803, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20901, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20902, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20903, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20904, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 20905, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 21001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 21002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 21003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 21004, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 21005, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 21006, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24201, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24202, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24301, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24302, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24401, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24402, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24501, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24601, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24602, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24603, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24701, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24702, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24801, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24802, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24803, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 24901, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30001, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30002, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30003, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30004, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30101, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30102, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30103, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30104, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30105, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30106, 1)
INSERT INTO [dbo].[PermissionDistribution] (PermissionDistributionTypeId, PermissionId, RoleId) VALUES (3, 30201, 1)

GO

SET IDENTITY_INSERT [dbo].[DefaultProductionOrderBatchStage] ON
INSERT INTO [DefaultProductionOrderBatchStage]([Id],[Name],[ProductionOrderBatchStageTypeId])
SELECT 1, 'Создание', 1 UNION
SELECT 2, 'Закрытие', 20 UNION
SELECT 3, 'Неуспешное закрытие', 20
GO

SET IDENTITY_INSERT [dbo].[DefaultProductionOrderBatchStage] OFF
GO


INSERT INTO ValueAddedTax (Name, Value, IsDefault)
SELECT '18%', 18, 1 UNION
SELECT '10%', 10, 0 UNION
SELECT 'Без НДС', 0, 0
GO

SET IDENTITY_INSERT [Currency] ON
INSERT INTO [Currency](Id, [Name],[NumericCode],[LiteralCode])
SELECT 1, 'Российский рубль', 643, 'RUB' UNION
SELECT 2, 'Доллар США', 840, 'USD' UNION
SELECT 3, 'Евро', 978, 'EUR'
GO

SET IDENTITY_INSERT [Currency] OFF
GO

SET IDENTITY_INSERT [dbo].[MeasureUnit] ON
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (1, N'Штука', N'шт.', '', 0, N'796')
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (2, N'Метр', N'м', '', 3, N'006')
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (3, N'Упаковка', N'уп.', '', 0, N'778')
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (4, N'Литр', N'л', '', 2, N'112')
GO

SET IDENTITY_INSERT [dbo].[MeasureUnit] OFF
GO

SET IDENTITY_INSERT [dbo].[LegalForm] ON
INSERT [dbo].[LegalForm] ([Id], [Name], [EconomicAgentTypeId]) 
SELECT 1, N'ООО', 1 UNION 
SELECT 2, N'ОАО', 1 UNION 
SELECT 3, N'ИП', 2 UNION 
SELECT 4, N'ЗАО', 1 UNION 
SELECT 5, N'ИКЦ', 1 UNION 
SELECT 6, N'ФГУП', 1

SET IDENTITY_INSERT [dbo].[LegalForm] OFF
GO

SET IDENTITY_INSERT [dbo].[Country] ON
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (1, N'Австралия', N'036')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (2, N'Австрия', N'040')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (3, N'Азербайджан', N'031')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (4, N'Албания', N'008')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (5, N'Алжир', N'012')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (6, N'Американское Самоа', N'016')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (7, N'Ангилья', N'660')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (8, N'Ангола', N'024')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (9, N'Андорра', N'020')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (10, N'Антарктида', N'010')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (11, N'Антигуа и Барбуда', N'028')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (12, N'Аргентина', N'032')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (13, N'Армения', N'051')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (14, N'Аруба', N'533')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (15, N'Афганистан', N'004')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (16, N'Багамы', N'044')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (17, N'Бангладеш', N'050')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (18, N'Барбадос', N'052')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (19, N'Бахрейн', N'048')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (20, N'Беларусь', N'112')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (21, N'Белиз', N'084')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (22, N'Бельгия', N'056')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (23, N'Бенин', N'204')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (24, N'Бермуды', N'060')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (25, N'Болгария', N'100')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (26, N'Боливия', N'068')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (27, N'Босния и Герцеговина', N'070')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (28, N'Ботсвана', N'072')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (29, N'Бразилия', N'076')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (30, N'Бруней-Даруссалам', N'096')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (31, N'Буркина-Фасо', N'854')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (32, N'Бурунди', N'108')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (33, N'Бутан', N'064')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (34, N'Вануату', N'548')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (35, N'Великобритания', N'826')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (36, N'Венгрия', N'348')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (37, N'Венесуэла', N'862')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (38, N'Вьетнам', N'704')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (39, N'Габон', N'266')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (40, N'Гаити', N'332')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (41, N'Гайана', N'328')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (42, N'Гамбия', N'270')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (43, N'Гана', N'288')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (44, N'Гваделупа', N'312')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (45, N'Гватемала', N'320')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (46, N'Гвинея', N'324')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (47, N'Гвинея-Бисау', N'624')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (48, N'Германия', N'276')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (49, N'Гернси', N'831')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (50, N'Гибралтар', N'292')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (51, N'Гондурас', N'340')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (52, N'Гонконг', N'344')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (53, N'Гренада', N'308')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (54, N'Гренландия', N'304')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (55, N'Греция', N'300')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (56, N'Грузия', N'268')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (57, N'Гуам', N'316')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (58, N'Дания', N'208')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (59, N'Джерси', N'832')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (60, N'Джибути', N'262')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (61, N'Доминиканская Республика', N'214')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (62, N'Египет', N'818')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (63, N'Замбия', N'894')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (64, N'Западная Сахара', N'732')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (65, N'Зимбабве', N'716')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (66, N'Израиль', N'376')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (67, N'Индия', N'356')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (68, N'Индонезия', N'360')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (69, N'Иордания', N'400')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (70, N'Ирак', N'368')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (71, N'Иран', N'364')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (72, N'Ирландия', N'372')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (73, N'Исландия', N'352')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (74, N'Испания', N'724')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (75, N'Италия', N'380')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (76, N'Йемен', N'887')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (77, N'Кабо-Верде', N'132')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (78, N'Казахстан', N'398')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (79, N'Камбоджа', N'116')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (80, N'Камерун', N'120')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (81, N'Канада', N'124')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (82, N'Катар', N'634')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (83, N'Кения', N'404')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (84, N'Кипр', N'196')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (85, N'Киргизия', N'417')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (86, N'Кирибати', N'296')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (87, N'Китай', N'156')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (88, N'Кокосовые (Килинг) острова', N'166')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (89, N'Колумбия', N'170')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (90, N'Коморы', N'174')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (91, N'Конго', N'178')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (92, N'Корея', N'410')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (94, N'Коста-Рика', N'188')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (95, N'Кот д''Ивуар', N'384')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (96, N'Куба', N'192')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (97, N'Кувейт', N'414')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (98, N'Лаос', N'418')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (99, N'Латвия', N'428')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (100, N'Лесото', N'426')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (101, N'Либерия', N'430')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (102, N'Ливан', N'422')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (103, N'Ливийская Арабская Джамахирия', N'434')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (104, N'Литва', N'440')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (105, N'Лихтенштейн', N'438')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (106, N'Люксембург', N'442')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (107, N'Маврикий', N'480')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (108, N'Мавритания', N'478')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (109, N'Мадагаскар', N'450')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (110, N'Майотта', N'175')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (111, N'Макао', N'446')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (112, N'Малави', N'454')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (113, N'Малайзия', N'458')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (114, N'Мали', N'466')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (115, N'Мальдивы', N'462')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (116, N'Мальта', N'470')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (117, N'Марокко', N'504')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (118, N'Мартиника', N'474')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (119, N'Маршалловы острова', N'584')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (120, N'Мексика', N'484')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (121, N'Мозамбик', N'508')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (122, N'Молдова, Республика', N'498')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (123, N'Монако', N'492')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (124, N'Монголия', N'496')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (125, N'Монтсеррат', N'500')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (126, N'Мьянма', N'104')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (127, N'Намибия', N'516')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (128, N'Науру', N'520')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (129, N'Непал', N'524')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (130, N'Нигер', N'562')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (131, N'Нигерия', N'566')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (132, N'Нидерландские Антилы', N'530')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (133, N'Нидерланды', N'528')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (134, N'Никарагуа', N'558')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (135, N'Ниуэ', N'570')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (136, N'Новая Зеландия', N'554')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (137, N'Новая Каледония', N'540')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (138, N'Норвегия', N'578')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (139, N'Объединенные Арабские Эмираты', N'784')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (140, N'Оман', N'512')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (141, N'Остров Буве', N'074')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (143, N'Остров Мэн', N'833')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (144, N'Остров Норфолк', N'574')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (145, N'Остров Рождества', N'162')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (147, N'Острова Кайман', N'136')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (148, N'Острова Кука', N'184')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (149, N'Острова Теркс и Кайкос', N'796')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (150, N'Пакистан', N'586')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (151, N'Палау', N'585')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (152, N'Панама', N'591')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (153, N'Папуа-Новая Гвинея', N'598')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (154, N'Парагвай', N'600')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (155, N'Перу', N'604')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (156, N'Питкерн', N'612')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (157, N'Польша', N'616')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (158, N'Португалия', N'620')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (159, N'Пуэрто-Рико', N'630')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (160, N'Республика Македония', N'807')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (161, N'Реюньон', N'638')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (162, N'Россия', N'643')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (163, N'Руанда', N'646')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (164, N'Румыния', N'642')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (165, N'Самоа', N'882')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (166, N'Сан-Марино', N'674')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (167, N'Сан-Томе и Принсипи', N'678')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (168, N'Саудовская Аравия', N'682')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (169, N'Свазиленд', N'748')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (170, N'Святая Елена', N'654')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (171, N'Северная Корея', N'408')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (172, N'Северные Марианские острова', N'580')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (173, N'Сейшелы', N'690')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (174, N'Сен-Бартельми', N'652')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (175, N'Сенегал', N'686')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (176, N'Сен-Пьер и Микелон', N'666')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (177, N'Сент-Винсент и Гренадины', N'670')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (178, N'Сент-Китс и Невис', N'659')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (179, N'Сент-Люсия', N'662')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (180, N'Сербия', N'688')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (181, N'Сингапур', N'702')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (182, N'Сирийская Арабская Республика', N'760')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (183, N'Словакия', N'703')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (184, N'Словения', N'705')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (185, N'Соломоновы острова', N'090')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (186, N'Сомали', N'706')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (187, N'Судан', N'736')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (188, N'Суринам', N'740')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (189, N'США', N'840')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (190, N'Сьерра-Леоне', N'694')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (191, N'Таджикистан', N'762')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (192, N'Таиланд', N'764')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (193, N'Тайвань', N'158')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (194, N'Танзания', N'834')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (195, N'Тимор-Лесте', N'626')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (196, N'Того', N'768')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (197, N'Токелау', N'772')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (198, N'Тонга', N'776')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (199, N'Тринидад и Тобаго', N'780')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (200, N'Тувалу', N'798')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (201, N'Тунис', N'788')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (202, N'Туркмения', N'795')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (203, N'Турция', N'792')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (204, N'Уганда', N'800')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (205, N'Узбекистан', N'860')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (206, N'Украина', N'804')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (207, N'Уоллис и Футуна', N'876')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (208, N'Уругвай', N'858')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (209, N'Фарерские острова', N'234')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (210, N'Фиджи', N'242')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (211, N'Филиппины', N'608')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (212, N'Финляндия', N'246')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (213, N'Фолклендские острова (Мальвинские)', N'238')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (214, N'Франция', N'250')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (215, N'Французская Гвиана', N'254')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (216, N'Французская Полинезия', N'258')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (217, N'Французские Южные территории', N'260')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (218, N'Хорватия', N'191')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (219, N'Центрально-Африканская Республика', N'140')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (220, N'Чад', N'148')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (221, N'Черногория', N'499')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (222, N'Чешская Республика', N'203')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (223, N'Чили', N'152')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (224, N'Швейцария', N'756')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (225, N'Швеция', N'752')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (226, N'Шпицберген и Ян Майен', N'744')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (227, N'Шри-Ланка', N'144')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (228, N'Эквадор', N'218')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (229, N'Экваториальная Гвинея', N'226')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (230, N'Эландские острова', N'248')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (231, N'Эль-Сальвадор', N'222')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (232, N'Эритрея', N'232')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (233, N'Эстония', N'233')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (234, N'Эфиопия', N'231')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (235, N'Южная Африка', N'710')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (237, N'Ямайка', N'388')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (238, N'Япония', N'392')

SET IDENTITY_INSERT [dbo].[Country] OFF
GO

SET IDENTITY_INSERT [dbo].[TaskType] ON

INSERT INTO TaskType (Name,Id) VALUES ('Задача', 1);
INSERT INTO TaskType (Name,Id) VALUES ('Звонок', 2);
INSERT INTO TaskType (Name,Id) VALUES ('Встреча', 3);
INSERT INTO TaskType (Name,Id) VALUES ('Презентация', 4);
INSERT INTO TaskType (Name,Id) VALUES ('Совещание', 5);
INSERT INTO TaskType (Name,Id) VALUES ('Поздравление', 6);
INSERT INTO TaskType (Name,Id) VALUES ('Мероприятие', 7);

GO
SET IDENTITY_INSERT [dbo].[TaskType] OFF

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 1);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 2);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 3);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 4);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 5);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 6);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Новая', 1, 1, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('В работе', 2, 2, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Выполнена', 3, 3, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Закрыта', 4, 3, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('Снята', 5, 3, 7);

SET IDENTITY_INSERT [dbo].[TaskPriority] ON

INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Немедленный', 1, 1);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Срочный', 2, 2);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Высокий', 3, 3);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Нормальный', 4, 4);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('Низкий', 5, 5);

GO
SET IDENTITY_INSERT [dbo].[TaskPriority] OFF


GO