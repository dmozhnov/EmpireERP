-- �������� ������� �������� �������
if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Setting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.Setting
CREATE TABLE dbo.Setting (
	DataBaseVersion varchar(15) not null
)
go

-- ������������� ������ �� �� ���������
INSERT INTO dbo.Setting(DataBaseVersion)
VALUES('0.9.1')

-- ���������� ��������������� ��-���������
INSERT INTO [Administrator] ([LastName],[FirstName],[Login],[PasswordHash],[CreationDate])
SELECT '�������', '�������', 'Manikhin', '8c0cc593ead64c5fa60cdae67c29d87b4ff4aa9b7d30ccad0dc8d5e45d7aa80b3b295fbf2a18ef9df300e11954be37e569fbe0417132c99b3110b5aab10c7e75', GETDATE() UNION

SELECT '��������', '�������', 'Sitchikhin', '8c0cc593ead64c5fa60cdae67c29d87b4ff4aa9b7d30ccad0dc8d5e45d7aa80b3b295fbf2a18ef9df300e11954be37e569fbe0417132c99b3110b5aab10c7e75', GETDATE()

-- �������� �����
INSERT INTO [Rate] ([Name],[ActiveUserCountLimit],[TeamCountLimit],[StorageCountLimit],[AccountOrganizationCountLimit],[GigabyteCountLimit]
           ,[UseExtraActiveUserOption],[UseExtraTeamOption],[UseExtraStorageOption],[UseExtraAccountOrganizationOption],[UseExtraGigabyteOption]
           ,[ExtraActiveUserOptionCostPerMonth],[ExtraTeamOptionCostPerMonth],[ExtraStorageOptionCostPerMonth],[ExtraAccountOrganizationOptionCostPerMonth]
           ,[ExtraGigabyteOptionCostPerMonth],[BaseCostPerMonth])

SELECT '����������', 
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

UNION
SELECT '��������', 
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

UNION
SELECT '������', 
	[ActiveUserCountLimit] = 10,
	[TeamCountLimit] = 3,
	[StorageCountLimit] = 32768,
	[AccountOrganizationCountLimit] = 32768,
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
SELECT '����������', 
	[ActiveUserCountLimit] = 250,
	[TeamCountLimit] = 32768,
	[StorageCountLimit] = 32768,
	[AccountOrganizationCountLimit] = 32768,
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

UNION
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

-- �������
set identity_insert [Region] on 

INSERT INTO [Region]([Id], [Name],[Code],[SortOrder])
select 1, '������', 1, 1000 union
select 2, '������������', 2, 1000 union
select 3, '�������', 3, 1000 union
select 4, '�����', 4, 1000 union
select 5, '��������', 5, 1000 union
select 6, '���������', 6, 1000 union
select 7, '���������-��������', 7, 1000 union
select 8, '��������', 8, 1000 union
select 9, '���������-��������', 9, 1000 union
select 10, '�������', 10, 1000 union
select 11, '����', 11, 1000 union
select 12, '����� ��', 12, 1000 union
select 13, '��������', 13, 1000 union
select 14, '���� (������)', 14, 1000 union
select 15, '�������� ������ - ������', 15, 1000 union
select 16, '���������', 16, 1000 union
select 17, '����', 17, 1000 union
select 18, '��������', 18, 1000 union
select 19, '�������', 19, 1000 union
select 20, '�����', 20, 1000 union
select 21, '�������', 21, 1000 union
select 22, '��������� ����', 22, 1000 union
select 23, '������������� ����', 23, 1000 union
select 24, '������������ ����', 24, 1000 union
select 25, '���������� ����', 25, 1000 union
select 26, '�������������� ����', 26, 1000 union
select 27, '����������� ����', 27, 1000 union
select 28, '�������� ���.', 28, 1000 union
select 29, '������������� ���.', 29, 1000 union
select 30, '������������ ���.', 30, 1000 union
select 31, '������������ ���.', 31, 1000 union
select 32, '�������� ���.', 32, 1000 union
select 33, '������������ ���.', 33, 1000 union
select 34, '������������� ���.', 34, 1000 union
select 35, '����������� ���.', 35, 1000 union
select 36, '����������� ���.', 36, 1000 union
select 37, '���������� ���.', 37, 1000 union
select 38, '��������� ���.', 38, 1000 union
select 39, '��������������� ���.', 39, 1000 union
select 40, '��������� ���.', 40, 1000 union
select 41, '���������� ����', 41, 1000 union
select 42, '����������� ���.', 42, 1000 union
select 43, '��������� ���.', 43, 1000 union
select 44, '����������� ���.', 44, 1000 union
select 45, '���������� ���.', 45, 1000 union
select 46, '������� ���.', 46, 1000 union
select 47, '������������� ���.', 47, 1000 union
select 48, '�������� ���.', 48, 1000 union
select 49, '����������� ���.', 49, 1000 union
select 50, '���������� ���.', 50, 1000 union
select 51, '���������� ���.', 51, 1000 union
select 52, '������������� ���.', 52, 1000 union
select 53, '������������ ���.', 53, 1000 union
select 54, '������������� ���.', 54, 1000 union
select 55, '������ ���.', 55, 1000 union
select 56, '������������ ���.', 56, 1000 union
select 57, '��������� ���.', 57, 1000 union
select 58, '���������� ���.', 58, 1000 union
select 59, '�������� ����', 59, 1000 union
select 60, '��������� ���.', 60, 1000 union
select 61, '���������� ���.', 61, 1000 union
select 62, '��������� ���.', 62, 1000 union
select 63, '��������� ���.', 63, 1000 union
select 64, '����������� ���.', 64, 1000 union
select 65, '����������� ���.', 65, 1000 union
select 66, '������������ ���.', 66, 1000 union
select 67, '���������� ���.', 67, 1000 union
select 68, '���������� ���.', 68, 1000 union
select 69, '�������� ���.', 69, 1000 union
select 70, '������� ���.', 70, 1000 union
select 71, '�������� ���.', 71, 1000 union
select 72, '��������� ���.', 72, 1000 union
select 73, '����������� ���.', 73, 1000 union
select 74, '����������� ���.', 74, 1000 union
select 75, '������������� ����', 75, 1000 union
select 76, '����������� ���.', 76, 1000 union
select 77, '������', 77, 1 union
select 78, '�����-���������', 78, 2 union
select 79, '��������� �� ', 79, 1000 union
select 83, '�������� ��', 83, 1000 union
select 86, '�����-���������� ��', 86, 1000 union
select 87, '��������� ��', 87, 1000 union
select 89, '�����-�������� ��', 89, 1000

set identity_insert [Region] off 

GO

-- ������
INSERT INTO [City]([Name],[SortOrder],[RegionId])

select '������', 1, 77 union
select '�����-���������', 1, 78 union

select '��������', 1000, 1 union
select '������', 1, 1 union
select '�������', 1000, 2 union
select '������', 1000, 2 union
select '�������', 1000, 2 union
select '��������', 1000, 2 union
select '�����', 1000, 2 union
select '������������', 1000, 2 union
select '�����������', 1000, 2 union
select '�������', 1000, 2 union
select '�������', 1000, 2 union
select '��������', 1000, 2 union
select '��������', 1000, 2 union
select '������', 1000, 2 union
select '����������', 1000, 2 union
select '�����������', 1000, 2 union
select '�������', 1000, 2 union
select '�����', 1000, 2 union
select '�����������', 1000, 2 union
select '�������', 1000, 2 union
select '���', 1, 2 union
select '�����', 1000, 2 union
select '�����', 1000, 2 union
select '��������', 1000, 3 union
select '������������', 1000, 3 union
select '���������', 1000, 3 union
select '�����', 1000, 3 union
select '���������������', 1000, 3 union
select '����-���', 1, 3 union
select '�����-�������', 1, 4 union
select '��������', 1000, 5 union
select '������������ ����', 1000, 5 union
select '�������', 1000, 5 union
select '��������', 1000, 5 union
select '��������', 1000, 5 union
select '��������', 1000, 5 union
select '������', 1000, 5 union
select '���������', 1, 5 union
select '��������', 1000, 5 union
select '����-���������', 1000, 5 union
select '���������', 1000, 6 union
select '�����', 1, 6 union
select '��������', 1000, 6 union
select '�������', 1000, 6 union
select '������', 1000, 7 union
select '�������', 1000, 7 union
select '�������', 1, 7 union
select '��������', 1000, 7 union
select '����������', 1000, 7 union
select '�����', 1000, 7 union
select '��������', 1000, 7 union
select '�����', 1000, 7 union
select '�������������', 1000, 8 union
select '������', 1000, 8 union
select '������', 1, 8 union
select '����������', 1000, 9 union
select '�������', 1000, 9 union
select '����-�������', 1000, 9 union
select '��������', 1, 9 union
select '���������', 1000, 10 union
select '����', 1000, 10 union
select '���������', 1000, 10 union
select '����������', 1000, 10 union
select '�����������', 1000, 10 union
select '�������������', 1000, 10 union
select '������', 1000, 10 union
select '������������', 1, 10 union
select '����������', 1000, 10 union
select '�����', 1000, 10 union
select '������', 1000, 10 union
select '���������', 1000, 10 union
select '�������', 1000, 10 union
select '�������', 1000, 11 union
select '������', 1000, 11 union
select '����', 1000, 11 union
select '����', 1000, 11 union
select '������', 1000, 11 union
select '������', 1000, 11 union
select '����������', 1000, 11 union
select '���������', 1, 11 union
select '������', 1000, 11 union
select '����', 1000, 11 union
select '������', 1000, 12 union
select '���������', 1000, 12 union
select '������-���', 1, 12 union
select '��������������', 1000, 12 union
select '�������', 1000, 13 union
select '�����', 1000, 13 union
select '���������', 1000, 13 union
select '��������������', 1000, 13 union
select '��������', 1000, 13 union
select '�������', 1, 13 union
select '��������', 1000, 13 union
select '�����', 1000, 14 union
select '���������', 1000, 14 union
select '�������', 1000, 14 union
select '�����', 1000, 14 union
select '������', 1000, 14 union
select '��������', 1000, 14 union
select '�����', 1000, 14 union
select '���������', 1000, 14 union
select '��������', 1000, 14 union
select '�������������', 1000, 14 union
select '������', 1000, 14 union
select '�������', 1000, 14 union
select '������', 1, 14 union
select '������', 1000, 15 union
select '�����', 1000, 15 union
select '������', 1000, 15 union
select '�����������', 1, 15 union
select '������', 1000, 15 union
select '������', 1000, 15 union
select '�����', 1000, 16 union
select '���������', 1000, 16 union
select '�����������', 1000, 16 union
select '����', 1000, 16 union
select '�����', 1000, 16 union
select '������', 1000, 16 union
select '��������', 1000, 16 union
select '������', 1000, 16 union
select '�������', 1000, 16 union
select '������', 1000, 16 union
select '������������', 1000, 16 union
select '������', 1, 16 union
select '�������', 1000, 16 union
select '�����������', 1000, 16 union
select '�������', 1000, 16 union
select '�����������', 1000, 16 union
select '����������', 1000, 16 union
select '���������� �����', 1000, 16 union
select '����������', 1000, 16 union
select '������', 1000, 16 union
select '������', 1000, 16 union
select '���������', 1000, 16 union
select '��-�������', 1000, 17 union
select '�����', 1, 17 union
select '�����', 1000, 17 union
select '�����', 1000, 17 union
select '�������', 1000, 17 union
select '��������', 1000, 18 union
select '������', 1000, 18 union
select '������', 1, 18 union
select '��������', 1000, 18 union
select '�����', 1000, 18 union
select '�������', 1000, 18 union
select '�����', 1000, 19 union
select '������', 1, 19 union
select '����������', 1000, 19 union
select '�����', 1000, 19 union
select '����������', 1000, 19 union
select '�����', 1000, 20 union
select '�������', 1, 20 union
select '��������', 1000, 20 union
select '����-������', 1000, 20 union
select '����', 1000, 20 union
select '�������', 1000, 21 union
select '�����', 1000, 21 union
select '��������', 1000, 21 union
select '���������� �����', 1000, 21 union
select '��������������', 1000, 21 union
select '��������', 1000, 21 union
select '���������', 1, 21 union
select '�������', 1000, 21 union
select '�����', 1000, 21 union
select '������', 1000, 22 union
select '�������', 1, 22 union
select '����������', 1000, 22 union
select '�����', 1000, 22 union
select '������', 1000, 22 union
select '�������', 1000, 22 union
select '�����������', 1000, 22 union
select '������-��-���', 1000, 22 union
select '�����������', 1000, 22 union
select '��������', 1000, 22 union
select '���������', 1000, 22 union
select '������', 1000, 22 union
select '������', 1000, 23 union
select '�����', 1000, 23 union
select '���������', 1000, 23 union
select '�������', 1000, 23 union
select '�����������', 1000, 23 union
select '���������', 1000, 23 union
select '������� ����', 1000, 23 union
select '����������', 1000, 23 union
select '����', 1000, 23 union
select '���������', 1000, 23 union
select '���������', 1, 23 union
select '���������', 1000, 23 union
select '������', 1000, 23 union
select '����������', 1000, 23 union
select '�������', 1000, 23 union
select '�����������', 1000, 23 union
select '������������', 1000, 23 union
select '���������-�������', 1000, 23 union
select '��������-��-������', 1000, 23 union
select '����', 1000, 23 union
select '������', 1000, 23 union
select '���������', 1000, 23 union
select '��������', 1000, 23 union
select '������', 1000, 23 union
select '����-�������', 1000, 23 union
select '���������', 1000, 23 union
select '���������', 1000, 24 union
select '������', 1000, 24 union
select '�������', 1000, 24 union
select '��������', 1000, 24 union
select '����������', 1000, 24 union
select '�������', 1000, 24 union
select '��������', 1000, 24 union
select '������������', 1000, 24 union
select '���������', 1000, 24 union
select '�����������', 1000, 24 union
select '������', 1000, 24 union
select '��������', 1000, 24 union
select '��������', 1000, 24 union
select '�����', 1000, 24 union
select '�������', 1000, 24 union
select '����������', 1, 24 union
select '����������-26', 1000, 24 union
select '����������-45', 1000, 24 union
select '�����������', 1000, 24 union
select '���������', 1000, 24 union
select '��������', 1000, 24 union
select '��������', 1000, 24 union
select '������������', 1000, 24 union
select '������', 1000, 24 union
select '����', 1000, 24 union
select '���', 1000, 24 union
select '��������', 1000, 24 union
select '��������', 1000, 25 union
select '�����', 1000, 25 union
select '������� ������', 1000, 25 union
select '�����������', 1, 25 union
select '�����������', 1000, 25 union
select '�������������', 1000, 25 union
select '�����������', 1000, 25 union
select '�������', 1000, 25 union
select '����������', 1000, 25 union
select '������-�������', 1000, 25 union
select '���������', 1000, 25 union
select '������', 1000, 25 union
select '�����������', 1000, 26 union
select '����������', 1000, 26 union
select '����������', 1000, 26 union
select '���������', 1000, 26 union
select '������������', 1000, 26 union
select '�����������', 1000, 26 union
select '����������', 1000, 26 union
select '�������', 1000, 26 union
select '����������', 1000, 26 union
select '���������', 1000, 26 union
select '����������� ����', 1000, 26 union
select '����������', 1000, 26 union
select '������������', 1000, 26 union
select '����������', 1000, 26 union
select '�����������������', 1000, 26 union
select '������������', 1000, 26 union
select '���������', 1000, 26 union
select '����������', 1000, 26 union
select '����������', 1, 26 union
select '������', 1000, 27 union
select '�����', 1000, 27 union
select '���������', 1000, 27 union
select '�����������-��-�����', 1000, 27 union
select '����������-��-�����', 1000, 27 union
select '��������� ������', 1000, 27 union
select '���������', 1, 27 union
select '���������-47', 1000, 27 union
select '���������', 1000, 28 union
select '������������', 1, 28 union
select '���������', 1000, 28 union
select '���', 1000, 28 union
select '����������', 1000, 28 union
select '���������', 1000, 28 union
select '�����������', 1000, 28 union
select '�����', 1000, 28 union
select '���������', 1000, 28 union
select '�����������', 1, 29 union
select '������', 1000, 29 union
select '���������', 1000, 29 union
select '�������', 1000, 29 union
select '������', 1000, 29 union
select '������', 1000, 29 union
select '������', 1000, 29 union
select '����������', 1000, 29 union
select '�������', 1000, 29 union
select '�����', 1000, 29 union
select '������������', 1000, 29 union
select '�������������', 1000, 29 union
select '��������', 1000, 29 union
select '���������', 1, 30 union
select '���������', 1000, 30 union
select '��������', 1000, 30 union
select '�������', 1000, 30 union
select '���������', 1000, 30 union
select '��������', 1000, 30 union
select '����������', 1000, 31 union
select '��������', 1, 31 union
select '�����', 1000, 31 union
select '�������', 1000, 31 union
select '���������', 1000, 31 union
select '������', 1000, 31 union
select '������', 1000, 31 union
select '����� �����', 1000, 31 union
select '������ �����', 1000, 31 union
select '���������', 1000, 31 union
select '��������', 1000, 31 union
select '������', 1, 32 union
select '��������', 1000, 32 union
select '�������', 1000, 32 union
select '������', 1000, 32 union
select '�������', 1000, 32 union
select '������', 1000, 32 union
select '�����', 1000, 32 union
select '����������', 1000, 32 union
select '�����', 1000, 32 union
select '�����', 1000, 32 union
select '������', 1000, 32 union
select '��������', 1000, 32 union
select '�����', 1000, 32 union
select '���������', 1000, 32 union
select '�����', 1000, 32 union
select '������', 1000, 32 union
select '�����������', 1000, 33 union
select '��������', 1, 33 union
select '�������', 1000, 33 union
select '���������', 1000, 33 union
select '����-�����������', 1000, 33 union
select '���������', 1000, 33 union
select '����������', 1000, 33 union
select '������', 1000, 33 union
select '������', 1000, 33 union
select '����������', 1000, 33 union
select '���������', 1000, 33 union
select '�������', 1000, 33 union
select '�������', 1000, 33 union
select '�������', 1000, 33 union
select '�����', 1000, 33 union
select '�������', 1000, 33 union
select '������', 1000, 33 union
select '��������', 1000, 33 union
select '�������', 1000, 33 union
select '��������', 1000, 33 union
select '�������', 1000, 33 union
select '�������', 1000, 33 union
select '�����-��������', 1000, 33 union
select '���������', 1, 34 union
select '��������', 1000, 34 union
select '�������', 1000, 34 union
select '��������', 1000, 34 union
select '�����-��-����', 1000, 34 union
select '�������', 1000, 34 union
select '������������', 1000, 34 union
select '������', 1000, 34 union
select '��������������', 1000, 34 union
select '�������', 1000, 34 union
select '����������', 1000, 34 union
select '����������', 1000, 34 union
select '�������������', 1000, 34 union
select '����������', 1000, 34 union
select '������ ���', 1000, 34 union
select '�����������', 1000, 34 union
select '����������', 1000, 34 union
select '��������', 1000, 34 union
select '�������', 1000, 34 union
select '�������', 1000, 35 union
select '���������', 1000, 35 union
select '������� �����', 1000, 35 union
select '�������', 1, 35 union
select '�������', 1000, 35 union
select '��������', 1000, 35 union
select '��������', 1000, 35 union
select '��������', 1000, 35 union
select '���������', 1000, 35 union
select '��������', 1000, 35 union
select '�����', 1000, 35 union
select '������', 1000, 35 union
select '�������', 1000, 35 union
select '�������', 1000, 35 union
select '���������', 1000, 35 union
select '������', 1000, 36 union
select '�������', 1000, 36 union
select '������������', 1000, 36 union
select '������������', 1000, 36 union
select '�������', 1, 36 union
select '�����', 1000, 36 union
select '�����', 1000, 36 union
select '�����������', 1000, 36 union
select '�����������', 1000, 36 union
select '����������', 1000, 36 union
select '��������', 1000, 36 union
select '��������', 1000, 36 union
select '�������', 1000, 36 union
select '��������', 1000, 36 union
select '������', 1000, 36 union
select '������', 1000, 37 union
select '�������� �����', 1000, 37 union
select '��������', 1000, 37 union
select '�������', 1, 37 union
select '�������', 1000, 37 union
select '�����������', 1000, 37 union
select '�����', 1000, 37 union
select '��������', 1000, 37 union
select '����', 1000, 37 union
select '���������', 1000, 37 union
select '�����', 1000, 37 union
select '�������', 1000, 37 union
select '�������', 1000, 37 union
select '��������', 1000, 37 union
select '���', 1000, 37 union
select '���', 1000, 37 union
select '�������', 1000, 37 union
select '�������', 1000, 38 union
select '�������', 1000, 38 union
select '���������', 1000, 38 union
select '���������', 1000, 38 union
select '�������', 1000, 38 union
select '������', 1000, 38 union
select '���������', 1000, 38 union
select '������������-��������', 1000, 38 union
select '����', 1000, 38 union
select '�������', 1, 38 union
select '�������', 1000, 38 union
select '�����������', 1000, 38 union
select '������', 1000, 38 union
select '������', 1000, 38 union
select '��������', 1000, 38 union
select '������', 1000, 38 union
select '�����', 1000, 38 union
select '������-���������', 1000, 38 union
select '����-������', 1000, 38 union
select '����-���', 1000, 38 union
select '���������', 1000, 38 union
select '�������', 1000, 38 union
select '�������������', 1000, 39 union
select '��������', 1000, 39 union
select '���������', 1000, 39 union
select '��������', 1000, 39 union
select '�����', 1000, 39 union
select '������������', 1000, 39 union
select '�����������', 1, 39 union
select '��������������', 1000, 39 union
select '��������', 1000, 39 union
select '��������', 1000, 39 union
select '�����', 1000, 39 union
select '��������', 1000, 39 union
select '������', 1000, 39 union
select '����������', 1000, 39 union
select '�������', 1000, 39 union
select '���������', 1000, 39 union
select '�����������', 1000, 39 union
select '�������', 1000, 39 union
select '������', 1000, 39 union
select '�������', 1000, 39 union
select '����������', 1000, 39 union
select '����������', 1000, 40 union
select '���������', 1000, 40 union
select '�������', 1000, 40 union
select '��������', 1000, 40 union
select '������', 1000, 40 union
select '�����', 1000, 40 union
select '������', 1, 40 union
select '�����', 1000, 40 union
select '��������', 1000, 40 union
select '��������', 1000, 40 union
select '��������', 1000, 40 union
select '��������', 1000, 40 union
select '�������������', 1000, 40 union
select '������', 1000, 40 union
select '�������', 1000, 40 union
select '��������', 1000, 40 union
select '�������', 1000, 40 union
select '���������', 1000, 40 union
select '����-�������', 1000, 40 union
select '��������', 1000, 40 union
select '������', 1000, 40 union
select '�����', 1000, 40 union
select '�����-1', 1000, 40 union
select '�����-2', 1000, 40 union
select '���������', 1000, 41 union
select '�������', 1000, 41 union
select '�������������-����������', 1, 41 union
select '������-��������', 1000, 42 union
select '������', 1000, 42 union
select '�����������', 1000, 42 union
select '��������', 1000, 42 union
select '������', 1000, 42 union
select '��������', 1, 42 union
select '���������', 1000, 42 union
select '�������-���������', 1000, 42 union
select '��������', 1000, 42 union
select '������������', 1000, 42 union
select '�����', 1000, 42 union
select '�����������', 1000, 42 union
select '��������', 1000, 42 union
select '���������', 1000, 42 union
select '�����������', 1000, 42 union
select '������', 1000, 42 union
select '�����', 1000, 42 union
select '��������', 1000, 42 union
select '�����', 1000, 42 union
select '����', 1000, 42 union
select '����� ��������', 1000, 43 union
select '������� ������', 1000, 43 union
select '������', 1000, 43 union
select '�����', 1, 43 union
select '������-������', 1000, 43 union
select '����', 1000, 43 union
select '���������', 1000, 43 union
select '����', 1000, 43 union
select '������', 1000, 43 union
select '������', 1000, 43 union
select '�������', 1000, 43 union
select '���������', 1000, 43 union
select '�����', 1000, 43 union
select '����������', 1000, 43 union
select '�������', 1000, 43 union
select '��������', 1000, 43 union
select '�����', 1000, 43 union
select '��������', 1000, 43 union
select '������', 1000, 43 union
select '���', 1000, 44 union
select '������������', 1000, 44 union
select '�����', 1000, 44 union
select '��������', 1000, 44 union
select '��������', 1, 44 union
select '��������', 1000, 44 union
select '���������', 1000, 44 union
select '�������', 1000, 44 union
select '���', 1000, 44 union
select '���������', 1000, 44 union
select '�������', 1000, 44 union
select '�����', 1000, 44 union
select '���������', 1000, 45 union
select '�������', 1000, 45 union
select '������', 1, 45 union
select '��������', 1000, 45 union
select '��������', 1000, 45 union
select '��������', 1000, 45 union
select '��������', 1000, 45 union
select '������', 1000, 45 union
select '�����', 1000, 45 union
select '��������-���������', 1000, 46 union
select '������������', 1000, 46 union
select '�����', 1, 46 union
select '��������', 1000, 46 union
select '�����', 1000, 46 union
select '������', 1000, 46 union
select '������', 1000, 46 union
select '�����', 1000, 46 union
select '���������', 1000, 46 union
select '�����', 1000, 46 union
select '�����', 1000, 46 union
select '������������', 1000, 47 union
select '��������', 1000, 47 union
select '������', 1000, 47 union
select '����������', 1000, 47 union
select '������', 1000, 47 union
select '������', 1000, 47 union
select '�������', 1000, 47 union
select '�����������', 1000, 47 union
select '���������', 1000, 47 union
select '������������', 1000, 47 union
select '���������', 1000, 47 union
select '������', 1000, 47 union
select '�������', 1000, 47 union
select '�������', 1000, 47 union
select '��������', 1000, 47 union
select '������� ����', 1000, 47 union
select '���������', 1000, 47 union
select '�������� ����', 1000, 47 union
select '���������', 1000, 47 union
select '����', 1000, 47 union
select '������', 1000, 47 union
select '����������', 1000, 47 union
select '����� ������', 1000, 47 union
select '��������', 1000, 47 union
select '��������', 1000, 47 union
select '��������', 1000, 47 union
select '�����������', 1000, 47 union
select '��������', 1000, 47 union
select '����������', 1000, 47 union
select '��������', 1000, 47 union
select '���������', 1000, 47 union
select '������', 1000, 47 union
select '�������� ������', 1000, 47 union
select '����������', 1000, 47 union
select '���������', 1000, 47 union
select '����������', 1000, 47 union
select '������', 1000, 47 union
select '�������� ���', 1000, 47 union
select '���������', 1000, 47 union
select '������', 1000, 47 union
select '�����', 1000, 47 union
select '��������� ������', 1000, 47 union
select '������������', 1000, 47 union
select '���� ������', 1000, 47 union
select '�����', 1000, 48 union
select '������', 1000, 48 union
select '����', 1000, 48 union
select '�������', 1000, 48 union
select '��������', 1000, 48 union
select '������', 1, 48 union
select '������', 1000, 48 union
select '��������', 1000, 48 union
select '�������', 1, 49 union
select '�������', 1000, 49 union
select '���������', 1000, 50 union
select '��������', 1000, 50 union
select '��������', 1000, 50 union
select '�����', 1000, 50 union
select '������', 1000, 50 union
select '�����������', 1000, 50 union
select '�����������', 1000, 50 union
select '���������', 1000, 50 union
select '��������', 1000, 50 union
select '�������', 1000, 50 union
select '�����������', 1000, 50 union
select '�������', 1000, 50 union
select '������������', 1000, 50 union
select '����������', 1000, 50 union
select '������', 1000, 50 union
select '�����', 1000, 50 union
select '���������', 1000, 50 union
select '���������������', 1000, 50 union
select '���������', 1000, 50 union
select '�������', 1000, 50 union
select '����������', 1000, 50 union
select '����������', 1000, 50 union
select '�����', 1000, 50 union
select '�����������', 1000, 50 union
select '������', 1000, 50 union
select '��������', 1000, 50 union
select '����', 1000, 50 union
select '�������', 1000, 50 union
select '�������', 1000, 50 union
select '����������', 1000, 50 union
select '�������������', 1000, 50 union
select '�����������', 1000, 50 union
select '�������������', 1000, 50 union
select '��������������', 1000, 50 union
select '�������', 1000, 50 union
select '���������', 1000, 50 union
select '������-������', 1000, 50 union
select '�����', 1000, 50 union
select '������-����������', 1000, 50 union
select '��������', 1000, 50 union
select '���������', 1000, 50 union
select '�������', 1000, 50 union
select '�������', 1000, 50 union
select '����������', 1000, 50 union
select '������', 1000, 50 union
select '����-�������', 1000, 50 union
select '�������', 1000, 50 union
select '��������', 1000, 50 union
select '��������-10', 1000, 50 union
select '��������', 1000, 50 union
select '�����', 1000, 50 union
select '�������-�����', 1000, 50 union
select '���������� �����', 1000, 50 union
select '��������', 1000, 50 union
select '��������', 1000, 50 union
select '��������', 1000, 50 union
select '�������', 1000, 50 union
select '������', 1000, 50 union
select '���������', 1000, 50 union
select '������', 1000, 50 union
select '������', 1000, 50 union
select '����', 1000, 50 union
select '������� �����', 1000, 50 union
select '��������', 1000, 50 union
select '�������������', 1000, 50 union
select '�������������-2', 1000, 50 union
select '�������������-25', 1000, 50 union
select '�������������-30', 1000, 50 union
select '�������������-7', 1000, 50 union
select '������ �������', 1000, 50 union
select '�������', 1000, 50 union
select '������', 1000, 50 union
select '������', 1000, 50 union
select '������', 1000, 50 union
select '�������', 1000, 50 union
select '�����', 1000, 50 union
select '��������', 1000, 50 union
select '������������', 1000, 50 union
select '�����', 1000, 50 union
select '�����-1', 1000, 50 union
select '�����-2', 1000, 50 union
select '�����-3', 1000, 50 union
select '�����-4', 1000, 50 union
select '�����-5', 1000, 50 union
select '�����-6', 1000, 50 union
select '�����-7', 1000, 50 union
select '�����-8', 1000, 50 union
select '������', 1000, 50 union
select '�������', 1000, 50 union
select '��������', 1000, 50 union
select '������������', 1000, 50 union
select '������������', 1000, 50 union
select '�����������', 1000, 50 union
select '���������', 1000, 50 union
select '������', 1000, 50 union
select '�������', 1000, 51 union
select '��������', 1000, 51 union
select '��������', 1000, 51 union
select '����������', 1000, 51 union
select '����������', 1000, 51 union
select '�������', 1000, 51 union
select '������', 1000, 51 union
select '����', 1000, 51 union
select '����������', 1000, 51 union
select '��������', 1, 51 union
select '����������', 1000, 51 union
select '����������-1', 1000, 51 union
select '����������-2', 1000, 51 union
select '����������-4', 1000, 51 union
select '���������', 1000, 51 union
select '�������� ����', 1000, 51 union
select '��������', 1000, 51 union
select '�����������', 1000, 51 union
select '�����������', 1000, 51 union
select '�������', 1000, 52 union
select '�������', 1000, 52 union
select '���������', 1000, 52 union
select '���', 1000, 52 union
select '�������', 1000, 52 union
select '���������', 1000, 52 union
select '������', 1000, 52 union
select '�����', 1000, 52 union
select '��������', 1000, 52 union
select '�������', 1000, 52 union
select '�������', 1000, 52 union
select '���������', 1000, 52 union
select '��������', 1000, 52 union
select '���������', 1000, 52 union
select '������', 1000, 52 union
select '��������', 1000, 52 union
select '��������', 1000, 52 union
select '�������', 1000, 52 union
select '��������', 1000, 52 union
select '������ ��������', 1, 52 union
select '�������', 1000, 52 union
select '����������', 1000, 52 union
select '�������', 1000, 52 union
select '�����', 1000, 52 union
select '�������', 1000, 52 union
select '������', 1000, 52 union
select '�����', 1000, 52 union
select '��������', 1000, 52 union
select '�������', 1000, 52 union
select '��������', 1000, 53 union
select '������', 1000, 53 union
select '������� ��������', 1, 53 union
select '����� ������', 1000, 53 union
select '��������', 1000, 53 union
select '�������', 1000, 53 union
select '������', 1000, 53 union
select '������ 2', 1000, 53 union
select '������ �����', 1000, 53 union
select '����', 1000, 53 union
select '������', 1000, 53 union
select '���������', 1000, 54 union
select '������', 1000, 54 union
select '��������', 1000, 54 union
select '�������', 1000, 54 union
select '�������', 1000, 54 union
select '������', 1000, 54 union
select '��������', 1000, 54 union
select '������', 1000, 54 union
select '�����������', 1, 54 union
select '���', 1000, 54 union
select '�������', 1000, 54 union
select '�������', 1000, 54 union
select '����������', 1000, 54 union
select '�����', 1000, 54 union
select '�����-3', 1000, 54 union
select '���������', 1000, 55 union
select '���������', 1000, 55 union
select '����������', 1000, 55 union
select '����', 1, 55 union
select '����', 1000, 55 union
select '���������', 1000, 55 union
select '��������', 1000, 56 union
select '����������', 1000, 56 union
select '�������', 1000, 56 union
select '���', 1000, 56 union
select '��������', 1000, 56 union
select '����������', 1000, 56 union
select '����������', 1000, 56 union
select '��������', 1, 56 union
select '����', 1000, 56 union
select '����-�����', 1000, 56 union
select '���������', 1000, 56 union
select '�����', 1000, 56 union
select '������', 1000, 57 union
select '���������', 1000, 57 union
select '�����', 1000, 57 union
select '���������������', 1000, 57 union
select '������', 1000, 57 union
select '��������', 1000, 57 union
select '����', 1, 57 union
select '���������������', 1000, 58 union
select '���������', 1000, 58 union
select '��������', 1000, 58 union
select '��������', 1000, 58 union
select '�������', 1000, 58 union
select '�������', 1000, 58 union
select '�������-12', 1000, 58 union
select '�������-8', 1000, 58 union
select '������ �����', 1000, 58 union
select '��������', 1000, 58 union
select '�����', 1, 58 union
select '��������', 1000, 58 union
select '������', 1000, 58 union
select '�����', 1000, 58 union
select '�������������', 1000, 59 union
select '���������', 1000, 59 union
select '����������', 1000, 59 union
select '������������', 1000, 59 union
select '����������', 1000, 59 union
select '������', 1000, 59 union
select '��������', 1000, 59 union
select '�����', 1000, 59 union
select '�������������', 1000, 59 union
select '�����������', 1000, 59 union
select '��������', 1000, 59 union
select '������', 1000, 59 union
select '������', 1000, 59 union
select '�����', 1000, 59 union
select '���', 1000, 59 union
select '������', 1000, 59 union
select '����', 1000, 59 union
select '�����', 1, 59 union
select '���������', 1000, 59 union
select '������', 1000, 59 union
select '����������', 1000, 59 union
select '�������', 1000, 59 union
select '������', 1000, 59 union
select '��������', 1000, 59 union
select '�������', 1000, 59 union
select '������� ����', 1000, 60 union
select '����', 1000, 60 union
select '���', 1000, 60 union
select '������', 1000, 60 union
select '��������', 1000, 60 union
select '��������������', 1000, 60 union
select '������', 1000, 60 union
select '������', 1000, 60 union
select '������', 1000, 60 union
select '������', 1000, 60 union
select '�����', 1, 60 union
select '��������', 1000, 60 union
select '��������', 1000, 60 union
select '�����', 1000, 60 union
select '����', 1000, 61 union
select '�����', 1000, 61 union
select '�������', 1000, 61 union
select '����� �������', 1000, 61 union
select '����������', 1000, 61 union
select '������', 1000, 61 union
select '������', 1000, 61 union
select '�������', 1000, 61 union
select '���������', 1000, 61 union
select '�������-����������', 1000, 61 union
select '��������������', 1000, 61 union
select '������� �����', 1000, 61 union
select '���������', 1000, 61 union
select '���������', 1000, 61 union
select '������������', 1000, 61 union
select '������������', 1000, 61 union
select '����������', 1000, 61 union
select '������-��-����', 1, 61 union
select '������', 1000, 61 union
select '�������������', 1000, 61 union
select '��������', 1000, 61 union
select '��������', 1000, 61 union
select '�����', 1000, 61 union
select '�������', 1000, 62 union
select '���������', 1000, 62 union
select '��������', 1000, 62 union
select '�������������', 1000, 62 union
select '������', 1000, 62 union
select '�����', 1000, 62 union
select '������', 1, 62 union
select '������', 1000, 62 union
select '������', 1000, 62 union
select '����-�������', 1000, 62 union
select '������-���������', 1000, 62 union
select '����', 1000, 62 union
select '���������', 1000, 63 union
select '������', 1000, 63 union
select '����������', 1000, 63 union
select '��������������', 1000, 63 union
select '���������', 1000, 63 union
select '��������', 1000, 63 union
select '�����������', 1000, 63 union
select '������', 1, 63 union
select '�������', 1000, 63 union
select '��������', 1000, 63 union
select '��������', 1000, 63 union
select '�������', 1000, 64 union
select '�������', 1000, 64 union
select '��������', 1000, 64 union
select '�������', 1000, 64 union
select '������', 1000, 64 union
select '�����', 1000, 64 union
select '���������', 1000, 64 union
select '�������������', 1000, 64 union
select '������� ���', 1000, 64 union
select '�����', 1000, 64 union
select '����������', 1000, 64 union
select '��������', 1000, 64 union
select '�������', 1000, 64 union
select '�������', 1000, 64 union
select '�������', 1, 64 union
select '��������', 1000, 64 union
select '������', 1000, 64 union
select '�������', 1000, 64 union
select '�������������-�����������', 1000, 65 union
select '�����', 1000, 65 union
select '������������', 1000, 65 union
select '�������', 1000, 65 union
select '��������', 1000, 65 union
select '�����������', 1000, 65 union
select '��������', 1000, 65 union
select '�������', 1000, 65 union
select '��������', 1000, 65 union
select '���', 1000, 65 union
select '���������', 1000, 65 union
select '������-��������', 1000, 65 union
select '������', 1000, 65 union
select '���������', 1000, 65 union
select '������', 1000, 65 union
select '�����', 1000, 65 union
select '��������', 1000, 65 union
select '����-���������', 1, 65 union
select '���������', 1000, 66 union
select '�������', 1000, 66 union
select '�����������', 1000, 66 union
select '������', 1000, 66 union
select '�����������', 1000, 66 union
select '����������', 1000, 66 union
select '������� �����', 1000, 66 union
select '������� �����', 1000, 66 union
select '������� �����', 1000, 66 union
select '������� ����', 1000, 66 union
select '����������', 1000, 66 union
select '��������', 1000, 66 union
select '��������', 1000, 66 union
select '������������', 1, 66 union
select '��������', 1000, 66 union
select '������', 1000, 66 union
select '�����', 1000, 66 union
select '�������-���������', 1000, 66 union
select '��������', 1000, 66 union
select '��������', 1000, 66 union
select '��������', 1000, 66 union
select '���������', 1000, 66 union
select '��������������', 1000, 66 union
select '�������������', 1000, 66 union
select '������������', 1000, 66 union
select '�����', 1000, 66 union
select '������', 1000, 66 union
select '����������', 1000, 66 union
select '��������', 1000, 66 union
select '������ �����', 1000, 66 union
select '������ �����', 1000, 66 union
select '������ �����', 1000, 66 union
select '������ ����', 1000, 66 union
select '����� ����', 1000, 66 union
select '�����������', 1000, 66 union
select '������������', 1000, 66 union
select '���������', 1000, 66 union
select '�����', 1000, 66 union
select '���', 1000, 66 union
select '����������', 1000, 66 union
select '����������-44', 1000, 66 union
select '����������-45', 1000, 66 union
select '�������������', 1000, 66 union
select '�����', 1000, 66 union
select '�������������', 1000, 66 union
select '����� ���', 1000, 66 union
select '�������', 1000, 66 union
select '�����', 1000, 66 union
select '������', 1000, 66 union
select '�������', 1000, 66 union
select '�����', 1000, 67 union
select '������', 1000, 67 union
select '�������', 1000, 67 union
select '�������', 1000, 67 union
select '����������', 1000, 67 union
select '���������', 1000, 67 union
select '���������', 1000, 67 union
select '�����', 1000, 67 union
select '�������', 1000, 67 union
select '��������', 1000, 67 union
select '�����', 1000, 67 union
select '��������', 1000, 67 union
select '��������', 1, 67 union
select '�������', 1000, 67 union
select '������', 1000, 67 union
select '��������', 1000, 68 union
select '��������', 1000, 68 union
select '�������', 1000, 68 union
select '���������', 1000, 68 union
select '��������', 1000, 68 union
select '����������', 1000, 68 union
select '������', 1, 68 union
select '�������', 1000, 68 union
select '����������', 1000, 69 union
select '������', 1000, 69 union
select '�����', 1000, 69 union
select '�������', 1000, 69 union
select '����������', 1000, 69 union
select '������ �������', 1000, 69 union
select '�������� �����', 1000, 69 union
select '������', 1000, 69 union
select '�������', 1000, 69 union
select '�������', 1000, 69 union
select '�����', 1000, 69 union
select '�����', 1000, 69 union
select '��������', 1000, 69 union
select '������� ����', 1000, 69 union
select '���������', 1000, 69 union
select '����������', 1000, 69 union
select '��������', 1000, 69 union
select '��������', 1000, 69 union
select '����', 1000, 69 union
select '�������', 1000, 69 union
select '�����', 1, 69 union
select '������', 1000, 69 union
select '�������', 1000, 69 union
select '������', 1000, 69 union
select '�����', 1000, 70 union
select '��������', 1000, 70 union
select '���������', 1000, 70 union
select '�������', 1000, 70 union
select '���������', 1000, 70 union
select '�����', 1, 70 union
select '�������', 1000, 71 union
select '�����', 1000, 71 union
select '����������', 1000, 71 union
select '��������', 1000, 71 union
select '�����', 1000, 71 union
select '�������', 1000, 71 union
select '�������', 1000, 71 union
select '�������', 1000, 71 union
select '��������', 1000, 71 union
select '�����', 1000, 71 union
select '������������', 1000, 71 union
select '������', 1000, 71 union
select '�������', 1000, 71 union
select '����������', 1000, 71 union
select '�������', 1000, 71 union
select '����', 1, 71 union
select '�������', 1000, 71 union
select '�������', 1000, 71 union
select '������', 1000, 71 union
select '���������', 1000, 71 union
select '������������', 1000, 72 union
select '����', 1000, 72 union
select '��������', 1000, 72 union
select '������', 1, 72 union
select '����������', 1000, 72 union
select '�����', 1000, 73 union
select '������������', 1000, 73 union
select '����', 1000, 73 union
select '�������������', 1000, 73 union
select '��������', 1000, 73 union
select '���������', 1, 73 union
select '���', 1000, 74 union
select '�����', 1000, 74 union
select '�������������', 1000, 74 union
select '������� ������', 1000, 74 union
select '�����������', 1000, 74 union
select '��������', 1000, 74 union
select '�������', 1000, 74 union
select '�������', 1000, 74 union
select '�����', 1000, 74 union
select '�����-��������', 1000, 74 union
select '�������', 1000, 74 union
select '�������', 1000, 74 union
select '����', 1000, 74 union
select '������', 1000, 74 union
select '������������', 1000, 74 union
select '�����', 1000, 74 union
select '������', 1000, 74 union
select '������������', 1000, 74 union
select '������', 1000, 74 union
select '�����', 1000, 74 union
select '�����', 1000, 74 union
select '���', 1000, 74 union
select '��������', 1000, 74 union
select '����������', 1000, 74 union
select '����������-1', 1000, 74 union
select '������', 1000, 74 union
select '����-�����', 1000, 74 union
select '���������', 1000, 74 union
select '���������', 1, 74 union
select '�����������', 1000, 74 union
select '�������', 1000, 74 union
select '�����', 1000, 75 union
select '�����', 1000, 75 union
select '�������������', 1000, 75 union
select '������', 1000, 75 union
select '��������', 1000, 75 union
select '��������-�������������', 1000, 75 union
select '��������', 1000, 75 union
select '�����', 1000, 75 union
select '����', 1, 75 union
select '����-46', 1000, 75 union
select '�����', 1000, 75 union
select '��������-��', 1000, 76 union
select '�������', 1000, 76 union
select '�����', 1000, 76 union
select '������', 1000, 76 union
select '����������-���������', 1000, 76 union
select '���������', 1000, 76 union
select '������', 1000, 76 union
select '�������', 1000, 76 union
select '������', 1000, 76 union
select '�����', 1000, 76 union
select '���������', 1, 76 union
select '����������', 1, 79 union
select '�������', 1000, 79 union
select '������-���', 1, 83 union
select '����������', 1000, 86 union
select '�������', 1000, 86 union
select '��������', 1000, 86 union
select '������', 1000, 86 union
select '������', 1000, 86 union
select '�����������', 1000, 86 union
select '�������������', 1000, 86 union
select '������', 1000, 86 union
select '������', 1000, 86 union
select '����-��', 1000, 86 union
select '��������', 1000, 86 union
select '���������', 1000, 86 union
select '������', 1000, 86 union
select '����', 1000, 86 union
select '�����-��������', 1, 86 union
select '������', 1000, 86 union
select '�������', 1, 87 union
select '��������', 1000, 87 union
select '�����', 1000, 87 union
select '����������', 1000, 89 union
select '����������', 1000, 89 union
select '����������', 1000, 89 union
select '�����', 1000, 89 union
select '����� �������', 1000, 89 union
select '��������', 1000, 89 union
select '��������', 1, 89 union
select '�����-����', 1000, 89 

GO

