-- �������� ������� �������� �������
GO

alter table dbo.Setting add DataBaseVersion varchar(15) not null 
GO

 --������������� ������ �� �� ���������
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

-- �������� ������� ������������ ������� (��������������)
GO
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Employee_CreatedBy]') AND parent_object_id = OBJECT_ID('[Employee]'))
alter table dbo.[Employee]  drop constraint FK_Employee_CreatedBy

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_UserToEmployee]') AND parent_object_id = OBJECT_ID('[User]'))
alter table dbo.[User]  drop constraint FK_UserToEmployee
	
INSERT INTO [EmployeePost] (Name)
SELECT '�������������'

INSERT INTO [Employee]([LastName],[FirstName],[Patronymic],[CreationDate],[EmployeePostId],[CreatedById])
SELECT '�������', '�����', '���������', GETDATE(), 1, 1

INSERT INTO [User]([Id],[DisplayName],[Login],[PasswordHash],[CreationDate],[BlockingDate],[CreatedById],[BlockerId],[DisplayNameTemplate])
SELECT 1, '������� �.�.', 'admin', 'c6daac1f5f46a0b7bc0a9322d0909b5f387c5f1192e202e7bc50ba84a96894b91c89d5d4835f954ddbbb0ca0435f9f2e2fde777e17afc4bb7b0184ce8ff9ca98', GETDATE(), NULL, 1, NULL, 'Lfp'

alter table dbo.[Employee] add constraint FK_Employee_CreatedBy foreign key (CreatedById) references dbo.[User]
alter table dbo.[User] add constraint FK_UserToEmployee foreign key (Id) references dbo.[Employee]

INSERT INTO [Role]([Name],[CreationDate],[Comment], [IsSystemAdmin])
SELECT '�������������', GETDATE(), '', 1



INSERT INTO [UserRole]([RoleId],[UserId])
SELECT 1, 1

INSERT INTO [Team]([Name],[CreationDate],[CreatedById],[Comment])
SELECT '�������� �������', GETDATE(), 1, ''

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
SELECT 1, '��������', 1 UNION
SELECT 2, '��������', 20 UNION
SELECT 3, '���������� ��������', 20
GO

SET IDENTITY_INSERT [dbo].[DefaultProductionOrderBatchStage] OFF
GO


INSERT INTO ValueAddedTax (Name, Value, IsDefault)
SELECT '18%', 18, 1 UNION
SELECT '10%', 10, 0 UNION
SELECT '��� ���', 0, 0
GO

SET IDENTITY_INSERT [Currency] ON
INSERT INTO [Currency](Id, [Name],[NumericCode],[LiteralCode])
SELECT 1, '���������� �����', 643, 'RUB' UNION
SELECT 2, '������ ���', 840, 'USD' UNION
SELECT 3, '����', 978, 'EUR'
GO

SET IDENTITY_INSERT [Currency] OFF
GO

SET IDENTITY_INSERT [dbo].[MeasureUnit] ON
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (1, N'�����', N'��.', '', 0, N'796')
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (2, N'����', N'�', '', 3, N'006')
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (3, N'��������', N'��.', '', 0, N'778')
INSERT [dbo].[MeasureUnit] ([Id], [FullName], [ShortName], [Comment], [Scale], [NumericCode]) VALUES (4, N'����', N'�', '', 2, N'112')
GO

SET IDENTITY_INSERT [dbo].[MeasureUnit] OFF
GO

SET IDENTITY_INSERT [dbo].[LegalForm] ON
INSERT [dbo].[LegalForm] ([Id], [Name], [EconomicAgentTypeId]) 
SELECT 1, N'���', 1 UNION 
SELECT 2, N'���', 1 UNION 
SELECT 3, N'��', 2 UNION 
SELECT 4, N'���', 1 UNION 
SELECT 5, N'���', 1 UNION 
SELECT 6, N'����', 1

SET IDENTITY_INSERT [dbo].[LegalForm] OFF
GO

SET IDENTITY_INSERT [dbo].[Country] ON
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (1, N'���������', N'036')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (2, N'�������', N'040')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (3, N'�����������', N'031')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (4, N'�������', N'008')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (5, N'�����', N'012')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (6, N'������������ �����', N'016')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (7, N'�������', N'660')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (8, N'������', N'024')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (9, N'�������', N'020')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (10, N'����������', N'010')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (11, N'������� ��������', N'028')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (12, N'���������', N'032')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (13, N'�������', N'051')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (14, N'�����', N'533')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (15, N'����������', N'004')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (16, N'������', N'044')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (17, N'���������', N'050')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (18, N'��������', N'052')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (19, N'�������', N'048')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (20, N'��������', N'112')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (21, N'�����', N'084')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (22, N'�������', N'056')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (23, N'�����', N'204')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (24, N'�������', N'060')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (25, N'��������', N'100')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (26, N'�������', N'068')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (27, N'������ ������������', N'070')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (28, N'��������', N'072')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (29, N'��������', N'076')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (30, N'������-����������', N'096')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (31, N'�������-����', N'854')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (32, N'�������', N'108')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (33, N'�����', N'064')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (34, N'�������', N'548')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (35, N'��������������', N'826')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (36, N'�������', N'348')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (37, N'���������', N'862')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (38, N'�������', N'704')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (39, N'�����', N'266')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (40, N'�����', N'332')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (41, N'������', N'328')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (42, N'������', N'270')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (43, N'����', N'288')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (44, N'���������', N'312')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (45, N'���������', N'320')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (46, N'������', N'324')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (47, N'������-�����', N'624')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (48, N'��������', N'276')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (49, N'������', N'831')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (50, N'���������', N'292')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (51, N'��������', N'340')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (52, N'�������', N'344')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (53, N'�������', N'308')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (54, N'����������', N'304')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (55, N'������', N'300')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (56, N'������', N'268')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (57, N'����', N'316')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (58, N'�����', N'208')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (59, N'������', N'832')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (60, N'�������', N'262')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (61, N'������������� ����������', N'214')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (62, N'������', N'818')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (63, N'������', N'894')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (64, N'�������� ������', N'732')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (65, N'��������', N'716')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (66, N'�������', N'376')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (67, N'�����', N'356')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (68, N'���������', N'360')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (69, N'��������', N'400')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (70, N'����', N'368')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (71, N'����', N'364')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (72, N'��������', N'372')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (73, N'��������', N'352')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (74, N'�������', N'724')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (75, N'������', N'380')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (76, N'�����', N'887')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (77, N'����-�����', N'132')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (78, N'���������', N'398')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (79, N'��������', N'116')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (80, N'�������', N'120')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (81, N'������', N'124')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (82, N'�����', N'634')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (83, N'�����', N'404')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (84, N'����', N'196')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (85, N'��������', N'417')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (86, N'��������', N'296')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (87, N'�����', N'156')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (88, N'��������� (������) �������', N'166')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (89, N'��������', N'170')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (90, N'������', N'174')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (91, N'�����', N'178')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (92, N'�����', N'410')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (94, N'�����-����', N'188')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (95, N'��� �''�����', N'384')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (96, N'����', N'192')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (97, N'������', N'414')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (98, N'����', N'418')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (99, N'������', N'428')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (100, N'������', N'426')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (101, N'�������', N'430')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (102, N'�����', N'422')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (103, N'��������� �������� ����������', N'434')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (104, N'�����', N'440')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (105, N'�����������', N'438')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (106, N'����������', N'442')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (107, N'��������', N'480')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (108, N'����������', N'478')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (109, N'����������', N'450')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (110, N'�������', N'175')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (111, N'�����', N'446')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (112, N'������', N'454')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (113, N'��������', N'458')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (114, N'����', N'466')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (115, N'��������', N'462')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (116, N'������', N'470')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (117, N'�������', N'504')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (118, N'���������', N'474')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (119, N'���������� �������', N'584')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (120, N'�������', N'484')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (121, N'��������', N'508')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (122, N'�������, ����������', N'498')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (123, N'������', N'492')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (124, N'��������', N'496')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (125, N'����������', N'500')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (126, N'������', N'104')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (127, N'�������', N'516')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (128, N'�����', N'520')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (129, N'�����', N'524')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (130, N'�����', N'562')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (131, N'�������', N'566')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (132, N'������������� ������', N'530')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (133, N'����������', N'528')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (134, N'���������', N'558')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (135, N'����', N'570')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (136, N'����� ��������', N'554')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (137, N'����� ���������', N'540')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (138, N'��������', N'578')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (139, N'������������ �������� �������', N'784')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (140, N'����', N'512')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (141, N'������ ����', N'074')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (143, N'������ ���', N'833')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (144, N'������ �������', N'574')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (145, N'������ ���������', N'162')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (147, N'������� ������', N'136')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (148, N'������� ����', N'184')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (149, N'������� ����� �������', N'796')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (150, N'��������', N'586')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (151, N'�����', N'585')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (152, N'������', N'591')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (153, N'�����-����� ������', N'598')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (154, N'��������', N'600')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (155, N'����', N'604')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (156, N'�������', N'612')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (157, N'������', N'616')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (158, N'����������', N'620')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (159, N'������-����', N'630')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (160, N'���������� ���������', N'807')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (161, N'�������', N'638')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (162, N'������', N'643')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (163, N'������', N'646')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (164, N'�������', N'642')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (165, N'�����', N'882')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (166, N'���-������', N'674')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (167, N'���-���� ���������', N'678')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (168, N'���������� ������', N'682')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (169, N'���������', N'748')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (170, N'������ �����', N'654')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (171, N'�������� �����', N'408')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (172, N'�������� ���������� �������', N'580')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (173, N'�������', N'690')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (174, N'���-���������', N'652')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (175, N'�������', N'686')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (176, N'���-���� ��������', N'666')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (177, N'����-������� ����������', N'670')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (178, N'����-���� ������', N'659')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (179, N'����-�����', N'662')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (180, N'������', N'688')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (181, N'��������', N'702')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (182, N'��������� �������� ����������', N'760')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (183, N'��������', N'703')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (184, N'��������', N'705')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (185, N'���������� �������', N'090')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (186, N'������', N'706')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (187, N'�����', N'736')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (188, N'�������', N'740')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (189, N'���', N'840')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (190, N'������-�����', N'694')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (191, N'�����������', N'762')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (192, N'�������', N'764')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (193, N'�������', N'158')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (194, N'��������', N'834')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (195, N'�����-�����', N'626')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (196, N'����', N'768')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (197, N'�������', N'772')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (198, N'�����', N'776')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (199, N'�������� �������', N'780')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (200, N'������', N'798')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (201, N'�����', N'788')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (202, N'���������', N'795')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (203, N'������', N'792')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (204, N'������', N'800')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (205, N'����������', N'860')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (206, N'�������', N'804')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (207, N'������ �������', N'876')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (208, N'�������', N'858')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (209, N'��������� �������', N'234')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (210, N'�����', N'242')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (211, N'���������', N'608')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (212, N'���������', N'246')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (213, N'������������ ������� (�����������)', N'238')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (214, N'�������', N'250')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (215, N'����������� ������', N'254')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (216, N'����������� ���������', N'258')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (217, N'����������� ����� ����������', N'260')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (218, N'��������', N'191')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (219, N'����������-����������� ����������', N'140')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (220, N'���', N'148')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (221, N'����������', N'499')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (222, N'������� ����������', N'203')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (223, N'����', N'152')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (224, N'���������', N'756')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (225, N'������', N'752')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (226, N'���������� ��������', N'744')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (227, N'���-�����', N'144')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (228, N'�������', N'218')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (229, N'�������������� ������', N'226')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (230, N'��������� �������', N'248')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (231, N'���-���������', N'222')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (232, N'�������', N'232')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (233, N'�������', N'233')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (234, N'�������', N'231')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (235, N'����� ������', N'710')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (237, N'������', N'388')
INSERT [dbo].[Country] ([Id], [Name], [NumericCode]) VALUES (238, N'������', N'392')

SET IDENTITY_INSERT [dbo].[Country] OFF
GO

SET IDENTITY_INSERT [dbo].[TaskType] ON

INSERT INTO TaskType (Name,Id) VALUES ('������', 1);
INSERT INTO TaskType (Name,Id) VALUES ('������', 2);
INSERT INTO TaskType (Name,Id) VALUES ('�������', 3);
INSERT INTO TaskType (Name,Id) VALUES ('�����������', 4);
INSERT INTO TaskType (Name,Id) VALUES ('���������', 5);
INSERT INTO TaskType (Name,Id) VALUES ('������������', 6);
INSERT INTO TaskType (Name,Id) VALUES ('�����������', 7);

GO
SET IDENTITY_INSERT [dbo].[TaskType] OFF

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 1, 1, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('� ������', 2, 2, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('���������', 3, 3, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�������', 4, 3, 1);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 5, 3, 1);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 1, 1, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('� ������', 2, 2, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('���������', 3, 3, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�������', 4, 3, 2);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 5, 3, 2);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 1, 1, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('� ������', 2, 2, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('���������', 3, 3, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�������', 4, 3, 3);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 5, 3, 3);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 1, 1, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('� ������', 2, 2, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('���������', 3, 3, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�������', 4, 3, 4);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 5, 3, 4);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 1, 1, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('� ������', 2, 2, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('���������', 3, 3, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�������', 4, 3, 5);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 5, 3, 5);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 1, 1, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('� ������', 2, 2, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('���������', 3, 3, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�������', 4, 3, 6);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 5, 3, 6);

INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 1, 1, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('� ������', 2, 2, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('���������', 3, 3, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�������', 4, 3, 7);
INSERT INTO TaskExecutionState (Name,OrdinalNumber,ExecutionStateTypeId,TaskTypeId) VALUES('�����', 5, 3, 7);

SET IDENTITY_INSERT [dbo].[TaskPriority] ON

INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('�����������', 1, 1);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('�������', 2, 2);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('�������', 3, 3);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('����������', 4, 4);
INSERT INTO TaskPriority (Name, OrdinalNumber, Id) VALUES('������', 5, 5);

GO
SET IDENTITY_INSERT [dbo].[TaskPriority] OFF


GO