/*---------------------------------------------------------------------------------------
  ������ ��� ���������� ���� �� ������ 0.9.25

  ��� ������:
	+ � ������� MeasureUnit ����� ���� - NumericCode
	+ � ������� Country ����� ���� - NumericCode
	
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

SET @PreviousVersion = '0.9.24' 			-- ����� ���������� ������
SET @NewVersion     = '0.9.25'			-- ����� ����� ������

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

alter table MeasureUnit add
	NumericCode varchar(3)

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO
	
update MeasureUnit set NumericCode = '796'
where Id = 1

update MeasureUnit set NumericCode = '006'
where Id = 2

update MeasureUnit set NumericCode = '778'
where Id = 3

update MeasureUnit set NumericCode = '112'
where Id = 4

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table MeasureUnit alter column NumericCode varchar(3) not null

-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table Country add
	NumericCode varchar(3)
	
-- ����� ������ ��������
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

CREATE TABLE #CountryTemp(
	[Id] [smallint] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[NumericCode] [varchar](3) NULL
	)	

INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (1, N'���������', N'036')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (2, N'�������', N'040')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (3, N'�����������', N'031')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (4, N'�������', N'008')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (5, N'�����', N'012')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (6, N'������������ �����', N'016')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (7, N'�������', N'660')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (8, N'������', N'024')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (9, N'�������', N'020')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (10, N'����������', N'010')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (11, N'������� ��������', N'028')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (12, N'���������', N'032')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (13, N'�������', N'051')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (14, N'�����', N'533')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (15, N'����������', N'004')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (16, N'������', N'044')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (17, N'���������', N'050')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (18, N'��������', N'052')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (19, N'�������', N'048')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (20, N'��������', N'112')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (21, N'�����', N'084')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (22, N'�������', N'056')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (23, N'�����', N'204')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (24, N'�������', N'060')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (25, N'��������', N'100')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (26, N'�������', N'068')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (27, N'������ ������������', N'070')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (28, N'��������', N'072')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (29, N'��������', N'076')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (30, N'������-����������', N'096')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (31, N'�������-����', N'854')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (32, N'�������', N'108')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (33, N'�����', N'064')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (34, N'�������', N'548')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (35, N'��������������', N'826')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (36, N'�������', N'348')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (37, N'���������', N'862')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (38, N'�������', N'704')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (39, N'�����', N'266')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (40, N'�����', N'332')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (41, N'������', N'328')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (42, N'������', N'270')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (43, N'����', N'288')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (44, N'���������', N'312')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (45, N'���������', N'320')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (46, N'������', N'324')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (47, N'������-�����', N'624')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (48, N'��������', N'276')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (49, N'������', N'831')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (50, N'���������', N'292')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (51, N'��������', N'340')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (52, N'�������', N'344')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (53, N'�������', N'308')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (54, N'����������', N'304')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (55, N'������', N'300')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (56, N'������', N'268')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (57, N'����', N'316')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (58, N'�����', N'208')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (59, N'������', N'832')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (60, N'�������', N'262')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (61, N'������������� ����������', N'214')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (62, N'������', N'818')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (63, N'������', N'894')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (64, N'�������� ������', N'732')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (65, N'��������', N'716')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (66, N'�������', N'376')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (67, N'�����', N'356')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (68, N'���������', N'360')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (69, N'��������', N'400')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (70, N'����', N'368')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (71, N'����', N'364')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (72, N'��������', N'372')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (73, N'��������', N'352')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (74, N'�������', N'724')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (75, N'������', N'380')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (76, N'�����', N'887')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (77, N'����-�����', N'132')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (78, N'���������', N'398')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (79, N'��������', N'116')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (80, N'�������', N'120')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (81, N'������', N'124')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (82, N'�����', N'634')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (83, N'�����', N'404')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (84, N'����', N'196')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (85, N'��������', N'417')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (86, N'��������', N'296')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (87, N'�����', N'156')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (88, N'��������� (������) �������', N'166')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (89, N'��������', N'170')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (90, N'������', N'174')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (91, N'�����', N'178')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (92, N'�����', N'410')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (94, N'�����-����', N'188')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (95, N'��� �''�����', N'384')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (96, N'����', N'192')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (97, N'������', N'414')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (98, N'����', N'418')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (99, N'������', N'428')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (100, N'������', N'426')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (101, N'�������', N'430')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (102, N'�����', N'422')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (103, N'��������� �������� ����������', N'434')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (104, N'�����', N'440')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (105, N'�����������', N'438')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (106, N'����������', N'442')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (107, N'��������', N'480')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (108, N'����������', N'478')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (109, N'����������', N'450')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (110, N'�������', N'175')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (111, N'�����', N'446')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (112, N'������', N'454')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (113, N'��������', N'458')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (114, N'����', N'466')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (115, N'��������', N'462')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (116, N'������', N'470')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (117, N'�������', N'504')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (118, N'���������', N'474')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (119, N'���������� �������', N'584')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (120, N'�������', N'484')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (121, N'��������', N'508')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (122, N'�������, ����������', N'498')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (123, N'������', N'492')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (124, N'��������', N'496')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (125, N'����������', N'500')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (126, N'������', N'104')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (127, N'�������', N'516')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (128, N'�����', N'520')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (129, N'�����', N'524')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (130, N'�����', N'562')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (131, N'�������', N'566')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (132, N'������������� ������', N'530')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (133, N'����������', N'528')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (134, N'���������', N'558')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (135, N'����', N'570')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (136, N'����� ��������', N'554')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (137, N'����� ���������', N'540')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (138, N'��������', N'578')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (139, N'������������ �������� �������', N'784')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (140, N'����', N'512')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (141, N'������ ����', N'074')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (143, N'������ ���', N'833')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (144, N'������ �������', N'574')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (145, N'������ ���������', N'162')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (147, N'������� ������', N'136')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (148, N'������� ����', N'184')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (149, N'������� ����� �������', N'796')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (150, N'��������', N'586')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (151, N'�����', N'585')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (152, N'������', N'591')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (153, N'�����-����� ������', N'598')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (154, N'��������', N'600')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (155, N'����', N'604')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (156, N'�������', N'612')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (157, N'������', N'616')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (158, N'����������', N'620')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (159, N'������-����', N'630')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (160, N'���������� ���������', N'807')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (161, N'�������', N'638')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (162, N'������', N'643')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (163, N'������', N'646')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (164, N'�������', N'642')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (165, N'�����', N'882')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (166, N'���-������', N'674')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (167, N'���-���� ���������', N'678')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (168, N'���������� ������', N'682')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (169, N'���������', N'748')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (170, N'������ �����', N'654')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (171, N'�������� �����', N'408')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (172, N'�������� ���������� �������', N'580')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (173, N'�������', N'690')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (174, N'���-���������', N'652')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (175, N'�������', N'686')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (176, N'���-���� ��������', N'666')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (177, N'����-������� ����������', N'670')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (178, N'����-���� ������', N'659')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (179, N'����-�����', N'662')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (180, N'������', N'688')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (181, N'��������', N'702')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (182, N'��������� �������� ����������', N'760')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (183, N'��������', N'703')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (184, N'��������', N'705')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (185, N'���������� �������', N'090')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (186, N'������', N'706')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (187, N'�����', N'736')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (188, N'�������', N'740')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (189, N'���', N'840')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (190, N'������-�����', N'694')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (191, N'�����������', N'762')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (192, N'�������', N'764')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (193, N'�������', N'158')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (194, N'��������', N'834')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (195, N'�����-�����', N'626')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (196, N'����', N'768')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (197, N'�������', N'772')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (198, N'�����', N'776')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (199, N'�������� �������', N'780')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (200, N'������', N'798')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (201, N'�����', N'788')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (202, N'���������', N'795')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (203, N'������', N'792')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (204, N'������', N'800')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (205, N'����������', N'860')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (206, N'�������', N'804')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (207, N'������ �������', N'876')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (208, N'�������', N'858')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (209, N'��������� �������', N'234')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (210, N'�����', N'242')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (211, N'���������', N'608')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (212, N'���������', N'246')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (213, N'������������ ������� (�����������)', N'238')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (214, N'�������', N'250')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (215, N'����������� ������', N'254')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (216, N'����������� ���������', N'258')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (217, N'����������� ����� ����������', N'260')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (218, N'��������', N'191')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (219, N'����������-����������� ����������', N'140')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (220, N'���', N'148')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (221, N'����������', N'499')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (222, N'������� ����������', N'203')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (223, N'����', N'152')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (224, N'���������', N'756')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (225, N'������', N'752')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (226, N'���������� ��������', N'744')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (227, N'���-�����', N'144')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (228, N'�������', N'218')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (229, N'�������������� ������', N'226')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (230, N'��������� �������', N'248')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (231, N'���-���������', N'222')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (232, N'�������', N'232')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (233, N'�������', N'233')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (234, N'�������', N'231')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (235, N'����� ������', N'710')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (237, N'������', N'388')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (238, N'������', N'392')

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

update Country set NumericCode = (select NumericCode from [dbo].[#CountryTemp] where Country.Name = [dbo].[#CountryTemp].Name)


GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

drop table #CountryTemp

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

delete from Country where NumericCode is null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

alter table Country alter column NumericCode varchar(3) not null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT '��� ������� ���������.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT '������ ��������� ������ �� �����' SET NOEXEC ON END
GO

-- � ����� �����
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT '���������� ��������� �������' END
GO

