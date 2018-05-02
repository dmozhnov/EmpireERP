/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.25

  Что нового:
	+ у таблицы MeasureUnit новое поле - NumericCode
	+ у таблицы Country новое поле - NumericCode
	
---------------------------------------------------------------------------------------*/
--SET NOEXEC OFF	-- выполнить данную команду в случае неуспешного обновления
SET DATEFORMAT DMY
SET NOCOUNT ON
SET ARITHABORT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

DECLARE @PreviousVersion varchar(15),	-- номер предыдущей версии
		@CurrentVersion varchar(15),	-- номер текущей версии базы данных
		@NewVersion varchar(15),		-- номер новой версии
		@DataBaseName varchar(256),		-- текущая база данных
		@CurrentDate nvarchar(10),		-- текущая дата
		@CurrentTime nvarchar(10),		-- текущее время
		@BackupTarget nvarchar(100)		-- куда делать бэкап базы данных

SET @PreviousVersion = '0.9.24' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.25'			-- номер новой версии

SELECT @CurrentVersion = DataBaseVersion FROM Setting
IF @@ERROR <> 0
BEGIN
	PRINT 'Неверная база данных'
END
ELSE
BEGIN
	-- СОЗДАЕМ БЭКАП БАЗЫ ДАННЫХ
	-- Получаем текущую дату
	SET @CurrentDate = CONVERT(nvarchar(20), GETDATE(), 104)	--	dd.mm.yyyy
	SET @CurrentTime = REPLACE(CONVERT(nvarchar(20), GETDATE(), 108) , ':', '.') --	hh:mm:ss
	SET @DataBaseName = DB_NAME()

	SET @BackupTarget = N'D:\Bizpulse\Backup\Update\' + @DataBaseName + '(' + CAST(@CurrentVersion as nvarchar(20)) + ') ' + 
		@CurrentDate + ' ' + @CurrentTime + N'.bak'

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = 
		N'Обновление версии', NOFORMAT

	IF @@ERROR <> 0
	BEGIN
		PRINT 'Ошибка создания backup''а. Продолжение выполнения невозможно.'
	END
	ELSE
		BEGIN

		IF (@CurrentVersion <> @PreviousVersion)
		BEGIN
			PRINT 'Обновить базу данных ' + @DataBaseName + ' до версии ' + @NewVersion + 
				' можно только из версии  ' + @PreviousVersion +
				'. Текущая версия: ' + @CurrentVersion
		END
		ELSE
		BEGIN
			--Начинаем транзакцию
			BEGIN TRAN

			--Обновляем версию базы данных
			UPDATE Setting 
			SET DataBaseVersion = @NewVersion		
		END
	END
END
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг установки версии окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table MeasureUnit add
	NumericCode varchar(3)

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO
	
update MeasureUnit set NumericCode = '796'
where Id = 1

update MeasureUnit set NumericCode = '006'
where Id = 2

update MeasureUnit set NumericCode = '778'
where Id = 3

update MeasureUnit set NumericCode = '112'
where Id = 4

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table MeasureUnit alter column NumericCode varchar(3) not null

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table Country add
	NumericCode varchar(3)
	
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

CREATE TABLE #CountryTemp(
	[Id] [smallint] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[NumericCode] [varchar](3) NULL
	)	

INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (1, N'Австралия', N'036')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (2, N'Австрия', N'040')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (3, N'Азербайджан', N'031')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (4, N'Албания', N'008')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (5, N'Алжир', N'012')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (6, N'Американское Самоа', N'016')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (7, N'Ангилья', N'660')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (8, N'Ангола', N'024')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (9, N'Андорра', N'020')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (10, N'Антарктида', N'010')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (11, N'Антигуа и Барбуда', N'028')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (12, N'Аргентина', N'032')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (13, N'Армения', N'051')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (14, N'Аруба', N'533')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (15, N'Афганистан', N'004')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (16, N'Багамы', N'044')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (17, N'Бангладеш', N'050')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (18, N'Барбадос', N'052')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (19, N'Бахрейн', N'048')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (20, N'Беларусь', N'112')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (21, N'Белиз', N'084')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (22, N'Бельгия', N'056')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (23, N'Бенин', N'204')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (24, N'Бермуды', N'060')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (25, N'Болгария', N'100')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (26, N'Боливия', N'068')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (27, N'Босния и Герцеговина', N'070')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (28, N'Ботсвана', N'072')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (29, N'Бразилия', N'076')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (30, N'Бруней-Даруссалам', N'096')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (31, N'Буркина-Фасо', N'854')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (32, N'Бурунди', N'108')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (33, N'Бутан', N'064')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (34, N'Вануату', N'548')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (35, N'Великобритания', N'826')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (36, N'Венгрия', N'348')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (37, N'Венесуэла', N'862')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (38, N'Вьетнам', N'704')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (39, N'Габон', N'266')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (40, N'Гаити', N'332')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (41, N'Гайана', N'328')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (42, N'Гамбия', N'270')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (43, N'Гана', N'288')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (44, N'Гваделупа', N'312')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (45, N'Гватемала', N'320')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (46, N'Гвинея', N'324')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (47, N'Гвинея-Бисау', N'624')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (48, N'Германия', N'276')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (49, N'Гернси', N'831')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (50, N'Гибралтар', N'292')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (51, N'Гондурас', N'340')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (52, N'Гонконг', N'344')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (53, N'Гренада', N'308')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (54, N'Гренландия', N'304')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (55, N'Греция', N'300')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (56, N'Грузия', N'268')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (57, N'Гуам', N'316')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (58, N'Дания', N'208')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (59, N'Джерси', N'832')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (60, N'Джибути', N'262')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (61, N'Доминиканская Республика', N'214')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (62, N'Египет', N'818')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (63, N'Замбия', N'894')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (64, N'Западная Сахара', N'732')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (65, N'Зимбабве', N'716')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (66, N'Израиль', N'376')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (67, N'Индия', N'356')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (68, N'Индонезия', N'360')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (69, N'Иордания', N'400')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (70, N'Ирак', N'368')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (71, N'Иран', N'364')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (72, N'Ирландия', N'372')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (73, N'Исландия', N'352')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (74, N'Испания', N'724')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (75, N'Италия', N'380')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (76, N'Йемен', N'887')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (77, N'Кабо-Верде', N'132')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (78, N'Казахстан', N'398')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (79, N'Камбоджа', N'116')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (80, N'Камерун', N'120')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (81, N'Канада', N'124')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (82, N'Катар', N'634')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (83, N'Кения', N'404')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (84, N'Кипр', N'196')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (85, N'Киргизия', N'417')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (86, N'Кирибати', N'296')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (87, N'Китай', N'156')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (88, N'Кокосовые (Килинг) острова', N'166')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (89, N'Колумбия', N'170')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (90, N'Коморы', N'174')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (91, N'Конго', N'178')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (92, N'Корея', N'410')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (94, N'Коста-Рика', N'188')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (95, N'Кот д''Ивуар', N'384')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (96, N'Куба', N'192')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (97, N'Кувейт', N'414')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (98, N'Лаос', N'418')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (99, N'Латвия', N'428')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (100, N'Лесото', N'426')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (101, N'Либерия', N'430')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (102, N'Ливан', N'422')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (103, N'Ливийская Арабская Джамахирия', N'434')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (104, N'Литва', N'440')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (105, N'Лихтенштейн', N'438')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (106, N'Люксембург', N'442')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (107, N'Маврикий', N'480')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (108, N'Мавритания', N'478')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (109, N'Мадагаскар', N'450')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (110, N'Майотта', N'175')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (111, N'Макао', N'446')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (112, N'Малави', N'454')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (113, N'Малайзия', N'458')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (114, N'Мали', N'466')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (115, N'Мальдивы', N'462')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (116, N'Мальта', N'470')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (117, N'Марокко', N'504')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (118, N'Мартиника', N'474')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (119, N'Маршалловы острова', N'584')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (120, N'Мексика', N'484')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (121, N'Мозамбик', N'508')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (122, N'Молдова, Республика', N'498')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (123, N'Монако', N'492')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (124, N'Монголия', N'496')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (125, N'Монтсеррат', N'500')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (126, N'Мьянма', N'104')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (127, N'Намибия', N'516')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (128, N'Науру', N'520')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (129, N'Непал', N'524')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (130, N'Нигер', N'562')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (131, N'Нигерия', N'566')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (132, N'Нидерландские Антилы', N'530')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (133, N'Нидерланды', N'528')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (134, N'Никарагуа', N'558')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (135, N'Ниуэ', N'570')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (136, N'Новая Зеландия', N'554')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (137, N'Новая Каледония', N'540')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (138, N'Норвегия', N'578')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (139, N'Объединенные Арабские Эмираты', N'784')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (140, N'Оман', N'512')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (141, N'Остров Буве', N'074')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (143, N'Остров Мэн', N'833')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (144, N'Остров Норфолк', N'574')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (145, N'Остров Рождества', N'162')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (147, N'Острова Кайман', N'136')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (148, N'Острова Кука', N'184')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (149, N'Острова Теркс и Кайкос', N'796')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (150, N'Пакистан', N'586')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (151, N'Палау', N'585')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (152, N'Панама', N'591')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (153, N'Папуа-Новая Гвинея', N'598')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (154, N'Парагвай', N'600')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (155, N'Перу', N'604')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (156, N'Питкерн', N'612')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (157, N'Польша', N'616')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (158, N'Португалия', N'620')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (159, N'Пуэрто-Рико', N'630')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (160, N'Республика Македония', N'807')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (161, N'Реюньон', N'638')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (162, N'Россия', N'643')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (163, N'Руанда', N'646')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (164, N'Румыния', N'642')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (165, N'Самоа', N'882')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (166, N'Сан-Марино', N'674')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (167, N'Сан-Томе и Принсипи', N'678')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (168, N'Саудовская Аравия', N'682')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (169, N'Свазиленд', N'748')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (170, N'Святая Елена', N'654')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (171, N'Северная Корея', N'408')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (172, N'Северные Марианские острова', N'580')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (173, N'Сейшелы', N'690')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (174, N'Сен-Бартельми', N'652')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (175, N'Сенегал', N'686')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (176, N'Сен-Пьер и Микелон', N'666')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (177, N'Сент-Винсент и Гренадины', N'670')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (178, N'Сент-Китс и Невис', N'659')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (179, N'Сент-Люсия', N'662')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (180, N'Сербия', N'688')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (181, N'Сингапур', N'702')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (182, N'Сирийская Арабская Республика', N'760')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (183, N'Словакия', N'703')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (184, N'Словения', N'705')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (185, N'Соломоновы острова', N'090')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (186, N'Сомали', N'706')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (187, N'Судан', N'736')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (188, N'Суринам', N'740')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (189, N'США', N'840')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (190, N'Сьерра-Леоне', N'694')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (191, N'Таджикистан', N'762')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (192, N'Таиланд', N'764')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (193, N'Тайвань', N'158')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (194, N'Танзания', N'834')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (195, N'Тимор-Лесте', N'626')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (196, N'Того', N'768')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (197, N'Токелау', N'772')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (198, N'Тонга', N'776')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (199, N'Тринидад и Тобаго', N'780')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (200, N'Тувалу', N'798')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (201, N'Тунис', N'788')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (202, N'Туркмения', N'795')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (203, N'Турция', N'792')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (204, N'Уганда', N'800')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (205, N'Узбекистан', N'860')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (206, N'Украина', N'804')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (207, N'Уоллис и Футуна', N'876')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (208, N'Уругвай', N'858')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (209, N'Фарерские острова', N'234')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (210, N'Фиджи', N'242')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (211, N'Филиппины', N'608')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (212, N'Финляндия', N'246')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (213, N'Фолклендские острова (Мальвинские)', N'238')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (214, N'Франция', N'250')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (215, N'Французская Гвиана', N'254')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (216, N'Французская Полинезия', N'258')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (217, N'Французские Южные территории', N'260')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (218, N'Хорватия', N'191')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (219, N'Центрально-Африканская Республика', N'140')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (220, N'Чад', N'148')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (221, N'Черногория', N'499')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (222, N'Чешская Республика', N'203')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (223, N'Чили', N'152')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (224, N'Швейцария', N'756')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (225, N'Швеция', N'752')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (226, N'Шпицберген и Ян Майен', N'744')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (227, N'Шри-Ланка', N'144')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (228, N'Эквадор', N'218')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (229, N'Экваториальная Гвинея', N'226')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (230, N'Эландские острова', N'248')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (231, N'Эль-Сальвадор', N'222')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (232, N'Эритрея', N'232')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (233, N'Эстония', N'233')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (234, N'Эфиопия', N'231')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (235, N'Южная Африка', N'710')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (237, N'Ямайка', N'388')
INSERT [dbo].[#CountryTemp] ([Id], [Name], [NumericCode]) VALUES (238, N'Япония', N'392')

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

update Country set NumericCode = (select NumericCode from [dbo].[#CountryTemp] where Country.Name = [dbo].[#CountryTemp].Name)


GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

drop table #CountryTemp

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

delete from Country where NumericCode is null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

alter table Country alter column NumericCode varchar(3) not null

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO

