-- Создание таблицы настроек системы
if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Setting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.Setting
CREATE TABLE dbo.Setting (
	DataBaseVersion varchar(15) not null
)
go

-- устанавливаем версию БД по умолчанию
INSERT INTO dbo.Setting(DataBaseVersion)
VALUES('0.9.1')

-- Добавление администраторов по-умолчанию
INSERT INTO [Administrator] ([LastName],[FirstName],[Login],[PasswordHash],[CreationDate])
SELECT 'Манихин', 'Виталий', 'Manikhin', '8c0cc593ead64c5fa60cdae67c29d87b4ff4aa9b7d30ccad0dc8d5e45d7aa80b3b295fbf2a18ef9df300e11954be37e569fbe0417132c99b3110b5aab10c7e75', GETDATE() UNION

SELECT 'Ситчихин', 'Евгений', 'Sitchikhin', '8c0cc593ead64c5fa60cdae67c29d87b4ff4aa9b7d30ccad0dc8d5e45d7aa80b3b295fbf2a18ef9df300e11954be37e569fbe0417132c99b3110b5aab10c7e75', GETDATE()

-- тарифные планы
INSERT INTO [Rate] ([Name],[ActiveUserCountLimit],[TeamCountLimit],[StorageCountLimit],[AccountOrganizationCountLimit],[GigabyteCountLimit]
           ,[UseExtraActiveUserOption],[UseExtraTeamOption],[UseExtraStorageOption],[UseExtraAccountOrganizationOption],[UseExtraGigabyteOption]
           ,[ExtraActiveUserOptionCostPerMonth],[ExtraTeamOptionCostPerMonth],[ExtraStorageOptionCostPerMonth],[ExtraAccountOrganizationOptionCostPerMonth]
           ,[ExtraGigabyteOptionCostPerMonth],[BaseCostPerMonth])

SELECT 'Бесплатный', 
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
SELECT 'Стандарт', 
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
SELECT 'Бизнес', 
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
SELECT 'Корпорация', 
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
SELECT 'Тестовый период', 
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

-- регионы
set identity_insert [Region] on 

INSERT INTO [Region]([Id], [Name],[Code],[SortOrder])
select 1, 'Адыгея', 1, 1000 union
select 2, 'Башкортостан', 2, 1000 union
select 3, 'Бурятия', 3, 1000 union
select 4, 'Алтай', 4, 1000 union
select 5, 'Дагестан', 5, 1000 union
select 6, 'Ингушетия', 6, 1000 union
select 7, 'Кабардино-Балкария', 7, 1000 union
select 8, 'Калмыкия', 8, 1000 union
select 9, 'Карачаево-Черкесия', 9, 1000 union
select 10, 'Карелия', 10, 1000 union
select 11, 'Коми', 11, 1000 union
select 12, 'Марий Эл', 12, 1000 union
select 13, 'Мордовия', 13, 1000 union
select 14, 'Саха (Якутия)', 14, 1000 union
select 15, 'Северная Осетия - Алания', 15, 1000 union
select 16, 'Татарстан', 16, 1000 union
select 17, 'Тыва', 17, 1000 union
select 18, 'Удмуртия', 18, 1000 union
select 19, 'Хакасия', 19, 1000 union
select 20, 'Чечня', 20, 1000 union
select 21, 'Чувашия', 21, 1000 union
select 22, 'Алтайский край', 22, 1000 union
select 23, 'Краснодарский край', 23, 1000 union
select 24, 'Красноярский край', 24, 1000 union
select 25, 'Приморский край', 25, 1000 union
select 26, 'Ставропольский край', 26, 1000 union
select 27, 'Хабаровский край', 27, 1000 union
select 28, 'Амурская обл.', 28, 1000 union
select 29, 'Архангельская обл.', 29, 1000 union
select 30, 'Астраханская обл.', 30, 1000 union
select 31, 'Белгородская обл.', 31, 1000 union
select 32, 'Брянская обл.', 32, 1000 union
select 33, 'Владимирская обл.', 33, 1000 union
select 34, 'Волгоградская обл.', 34, 1000 union
select 35, 'Вологодская обл.', 35, 1000 union
select 36, 'Воронежская обл.', 36, 1000 union
select 37, 'Ивановская обл.', 37, 1000 union
select 38, 'Иркутская обл.', 38, 1000 union
select 39, 'Калининградская обл.', 39, 1000 union
select 40, 'Калужская обл.', 40, 1000 union
select 41, 'Камчатский край', 41, 1000 union
select 42, 'Кемеровская обл.', 42, 1000 union
select 43, 'Кировская обл.', 43, 1000 union
select 44, 'Костромская обл.', 44, 1000 union
select 45, 'Курганская обл.', 45, 1000 union
select 46, 'Курская обл.', 46, 1000 union
select 47, 'Ленинградская обл.', 47, 1000 union
select 48, 'Липецкая обл.', 48, 1000 union
select 49, 'Магаданская обл.', 49, 1000 union
select 50, 'Московская обл.', 50, 1000 union
select 51, 'Мурманская обл.', 51, 1000 union
select 52, 'Нижегородская обл.', 52, 1000 union
select 53, 'Новгородская обл.', 53, 1000 union
select 54, 'Новосибирская обл.', 54, 1000 union
select 55, 'Омская обл.', 55, 1000 union
select 56, 'Оренбургская обл.', 56, 1000 union
select 57, 'Орловская обл.', 57, 1000 union
select 58, 'Пензенская обл.', 58, 1000 union
select 59, 'Пермский край', 59, 1000 union
select 60, 'Псковская обл.', 60, 1000 union
select 61, 'Ростовская обл.', 61, 1000 union
select 62, 'Рязанская обл.', 62, 1000 union
select 63, 'Самарская обл.', 63, 1000 union
select 64, 'Саратовская обл.', 64, 1000 union
select 65, 'Сахалинская обл.', 65, 1000 union
select 66, 'Свердловская обл.', 66, 1000 union
select 67, 'Смоленская обл.', 67, 1000 union
select 68, 'Тамбовская обл.', 68, 1000 union
select 69, 'Тверская обл.', 69, 1000 union
select 70, 'Томская обл.', 70, 1000 union
select 71, 'Тульская обл.', 71, 1000 union
select 72, 'Тюменская обл.', 72, 1000 union
select 73, 'Ульяновская обл.', 73, 1000 union
select 74, 'Челябинская обл.', 74, 1000 union
select 75, 'Забайкальский край', 75, 1000 union
select 76, 'Ярославская обл.', 76, 1000 union
select 77, 'Москва', 77, 1 union
select 78, 'Санкт-Петербург', 78, 2 union
select 79, 'Еврейская АО ', 79, 1000 union
select 83, 'Ненецкий АО', 83, 1000 union
select 86, 'Ханты-Мансийский АО', 86, 1000 union
select 87, 'Чукотский АО', 87, 1000 union
select 89, 'Ямало-Ненецкий АО', 89, 1000

set identity_insert [Region] off 

GO

-- города
INSERT INTO [City]([Name],[SortOrder],[RegionId])

select 'Москва', 1, 77 union
select 'Санкт-Петербург', 1, 78 union

select 'Адыгейск', 1000, 1 union
select 'Майкоп', 1, 1 union
select 'Агидель', 1000, 2 union
select 'Баймак', 1000, 2 union
select 'Белебей', 1000, 2 union
select 'Белорецк', 1000, 2 union
select 'Бирск', 1000, 2 union
select 'Благовещенск', 1000, 2 union
select 'Давлеканово', 1000, 2 union
select 'Дюртюли', 1000, 2 union
select 'Ишимбай', 1000, 2 union
select 'Кумертау', 1000, 2 union
select 'Межгорье', 1000, 2 union
select 'Мелеуз', 1000, 2 union
select 'Нефтекамск', 1000, 2 union
select 'Октябрьский', 1000, 2 union
select 'Салават', 1000, 2 union
select 'Сибай', 1000, 2 union
select 'Стерлитамак', 1000, 2 union
select 'Туймазы', 1000, 2 union
select 'Уфа', 1, 2 union
select 'Учалы', 1000, 2 union
select 'Янаул', 1000, 2 union
select 'Бабушкин', 1000, 3 union
select 'Гусиноозерск', 1000, 3 union
select 'Закаменск', 1000, 3 union
select 'Кяхта', 1000, 3 union
select 'Северобайкальск', 1000, 3 union
select 'Улан-Удэ', 1, 3 union
select 'Горно-Алтайск', 1, 4 union
select 'Буйнакск', 1000, 5 union
select 'Дагестанские Огни', 1000, 5 union
select 'Дербент', 1000, 5 union
select 'Избербаш', 1000, 5 union
select 'Каспийск', 1000, 5 union
select 'Кизилюрт', 1000, 5 union
select 'Кизляр', 1000, 5 union
select 'Махачкала', 1, 5 union
select 'Хасавюрт', 1000, 5 union
select 'Южно-Сухокумск', 1000, 5 union
select 'Карабулак', 1000, 6 union
select 'Магас', 1, 6 union
select 'Малгобек', 1000, 6 union
select 'Назрань', 1000, 6 union
select 'Баксан', 1000, 7 union
select 'Майский', 1000, 7 union
select 'Нальчик', 1, 7 union
select 'Нарткала', 1000, 7 union
select 'Прохладный', 1000, 7 union
select 'Терек', 1000, 7 union
select 'Тырныауз', 1000, 7 union
select 'Чегем', 1000, 7 union
select 'Городовиковск', 1000, 8 union
select 'Лагань', 1000, 8 union
select 'Элиста', 1, 8 union
select 'Карачаевск', 1000, 9 union
select 'Теберда', 1000, 9 union
select 'Усть-Джегута', 1000, 9 union
select 'Черкесск', 1, 9 union
select 'Беломорск', 1000, 10 union
select 'Кемь', 1000, 10 union
select 'Кондопога', 1000, 10 union
select 'Костомукша', 1000, 10 union
select 'Лахденпохья', 1000, 10 union
select 'Медвежьегорск', 1000, 10 union
select 'Олонец', 1000, 10 union
select 'Петрозаводск', 1, 10 union
select 'Питкяранта', 1000, 10 union
select 'Пудож', 1000, 10 union
select 'Сегежа', 1000, 10 union
select 'Сортавала', 1000, 10 union
select 'Суоярви', 1000, 10 union
select 'Воркута', 1000, 11 union
select 'Вуктыл', 1000, 11 union
select 'Емва', 1000, 11 union
select 'Инта', 1000, 11 union
select 'Микунь', 1000, 11 union
select 'Печора', 1000, 11 union
select 'Сосногорск', 1000, 11 union
select 'Сыктывкар', 1, 11 union
select 'Усинск', 1000, 11 union
select 'Ухта', 1000, 11 union
select 'Волжск', 1000, 12 union
select 'Звенигово', 1000, 12 union
select 'Йошкар-Ола', 1, 12 union
select 'Козьмодемьянск', 1000, 12 union
select 'Ардатов', 1000, 13 union
select 'Инсар', 1000, 13 union
select 'Ковылкино', 1000, 13 union
select 'Краснослободск', 1000, 13 union
select 'Рузаевка', 1000, 13 union
select 'Саранск', 1, 13 union
select 'Темников', 1000, 13 union
select 'Алдан', 1000, 14 union
select 'Верхоянск', 1000, 14 union
select 'Вилюйск', 1000, 14 union
select 'Ленск', 1000, 14 union
select 'Мирный', 1000, 14 union
select 'Нерюнгри', 1000, 14 union
select 'Нюрба', 1000, 14 union
select 'Олекминск', 1000, 14 union
select 'Покровск', 1000, 14 union
select 'Среднеколымск', 1000, 14 union
select 'Томмот', 1000, 14 union
select 'Удачный', 1000, 14 union
select 'Якутск', 1, 14 union
select 'Алагир', 1000, 15 union
select 'Ардон', 1000, 15 union
select 'Беслан', 1000, 15 union
select 'Владикавказ', 1, 15 union
select 'Дигора', 1000, 15 union
select 'Моздок', 1000, 15 union
select 'Агрыз', 1000, 16 union
select 'Азнакаево', 1000, 16 union
select 'Альметьевск', 1000, 16 union
select 'Арск', 1000, 16 union
select 'Бавлы', 1000, 16 union
select 'Болгар', 1000, 16 union
select 'Бугульма', 1000, 16 union
select 'Буинск', 1000, 16 union
select 'Елабуга', 1000, 16 union
select 'Заинск', 1000, 16 union
select 'Зеленодольск', 1000, 16 union
select 'Казань', 1, 16 union
select 'Лаишево', 1000, 16 union
select 'Лениногорск', 1000, 16 union
select 'Мамадыш', 1000, 16 union
select 'Менделеевск', 1000, 16 union
select 'Мензелинск', 1000, 16 union
select 'Набережные Челны', 1000, 16 union
select 'Нижнекамск', 1000, 16 union
select 'Нурлат', 1000, 16 union
select 'Тетюши', 1000, 16 union
select 'Чистополь', 1000, 16 union
select 'Ак-Довурак', 1000, 17 union
select 'Кызыл', 1, 17 union
select 'Туран', 1000, 17 union
select 'Чадан', 1000, 17 union
select 'Шагонар', 1000, 17 union
select 'Воткинск', 1000, 18 union
select 'Глазов', 1000, 18 union
select 'Ижевск', 1, 18 union
select 'Камбарка', 1000, 18 union
select 'Можга', 1000, 18 union
select 'Сарапул', 1000, 18 union
select 'Абаза', 1000, 19 union
select 'Абакан', 1, 19 union
select 'Саяногорск', 1000, 19 union
select 'Сорск', 1000, 19 union
select 'Черногорск', 1000, 19 union
select 'Аргун', 1000, 20 union
select 'Грозный', 1, 20 union
select 'Гудермес', 1000, 20 union
select 'Урус-Мартан', 1000, 20 union
select 'Шали', 1000, 20 union
select 'Алатырь', 1000, 21 union
select 'Канаш', 1000, 21 union
select 'Козловка', 1000, 21 union
select 'Мариинский Посад', 1000, 21 union
select 'Новочебоксарск', 1000, 21 union
select 'Цивильск', 1000, 21 union
select 'Чебоксары', 1, 21 union
select 'Шумерля', 1000, 21 union
select 'Ядрин', 1000, 21 union
select 'Алейск', 1000, 22 union
select 'Барнаул', 1, 22 union
select 'Белокуриха', 1000, 22 union
select 'Бийск', 1000, 22 union
select 'Горняк', 1000, 22 union
select 'Заринск', 1000, 22 union
select 'Змеиногорск', 1000, 22 union
select 'Камень-на-Оби', 1000, 22 union
select 'Новоалтайск', 1000, 22 union
select 'Рубцовск', 1000, 22 union
select 'Славгород', 1000, 22 union
select 'Яровое', 1000, 22 union
select 'Абинск', 1000, 23 union
select 'Анапа', 1000, 23 union
select 'Апшеронск', 1000, 23 union
select 'Армавир', 1000, 23 union
select 'Белореченск', 1000, 23 union
select 'Геленджик', 1000, 23 union
select 'Горячий Ключ', 1000, 23 union
select 'Гулькевичи', 1000, 23 union
select 'Ейск', 1000, 23 union
select 'Кореновск', 1000, 23 union
select 'Краснодар', 1, 23 union
select 'Кропоткин', 1000, 23 union
select 'Крымск', 1000, 23 union
select 'Курганинск', 1000, 23 union
select 'Лабинск', 1000, 23 union
select 'Новокубанск', 1000, 23 union
select 'Новороссийск', 1000, 23 union
select 'Приморско-Ахтарск', 1000, 23 union
select 'Славянск-на-Кубани', 1000, 23 union
select 'Сочи', 1000, 23 union
select 'Темрюк', 1000, 23 union
select 'Тимашевск', 1000, 23 union
select 'Тихорецк', 1000, 23 union
select 'Туапсе', 1000, 23 union
select 'Усть-Лабинск', 1000, 23 union
select 'Хадыженск', 1000, 23 union
select 'Артемовск', 1000, 24 union
select 'Ачинск', 1000, 24 union
select 'Боготол', 1000, 24 union
select 'Бородино', 1000, 24 union
select 'Дивногорск', 1000, 24 union
select 'Дудинка', 1000, 24 union
select 'Енисейск', 1000, 24 union
select 'Железногорск', 1000, 24 union
select 'Заозерный', 1000, 24 union
select 'Зеленогорск', 1000, 24 union
select 'Игарка', 1000, 24 union
select 'Иланский', 1000, 24 union
select 'Кайеркан', 1000, 24 union
select 'Канск', 1000, 24 union
select 'Кодинск', 1000, 24 union
select 'Красноярск', 1, 24 union
select 'Красноярск-26', 1000, 24 union
select 'Красноярск-45', 1000, 24 union
select 'Лесосибирск', 1000, 24 union
select 'Минусинск', 1000, 24 union
select 'Назарово', 1000, 24 union
select 'Норильск', 1000, 24 union
select 'Сосновоборск', 1000, 24 union
select 'Талнах', 1000, 24 union
select 'Ужур', 1000, 24 union
select 'Уяр', 1000, 24 union
select 'Шарыпово', 1000, 24 union
select 'Арсеньев', 1000, 25 union
select 'Артем', 1000, 25 union
select 'Большой Камень', 1000, 25 union
select 'Владивосток', 1, 25 union
select 'Дальнегорск', 1000, 25 union
select 'Дальнереченск', 1000, 25 union
select 'Лесозаводск', 1000, 25 union
select 'Находка', 1000, 25 union
select 'Партизанск', 1000, 25 union
select 'Спасск-Дальний', 1000, 25 union
select 'Уссурийск', 1000, 25 union
select 'Фокино', 1000, 25 union
select 'Благодарный', 1000, 26 union
select 'Буденновск', 1000, 26 union
select 'Георгиевск', 1000, 26 union
select 'Ессентуки', 1000, 26 union
select 'Железноводск', 1000, 26 union
select 'Зеленокумск', 1000, 26 union
select 'Изобильный', 1000, 26 union
select 'Ипатово', 1000, 26 union
select 'Кисловодск', 1000, 26 union
select 'Лермонтов', 1000, 26 union
select 'Минеральные Воды', 1000, 26 union
select 'Михайловск', 1000, 26 union
select 'Невинномысск', 1000, 26 union
select 'Нефтекумск', 1000, 26 union
select 'Новоалександровск', 1000, 26 union
select 'Новопавловск', 1000, 26 union
select 'Пятигорск', 1000, 26 union
select 'Светлоград', 1000, 26 union
select 'Ставрополь', 1, 26 union
select 'Амурск', 1000, 27 union
select 'Бикин', 1000, 27 union
select 'Вяземский', 1000, 27 union
select 'Комсомольск-на-Амуре', 1000, 27 union
select 'Николаевск-на-Амуре', 1000, 27 union
select 'Советская Гавань', 1000, 27 union
select 'Хабаровск', 1, 27 union
select 'Хабаровск-47', 1000, 27 union
select 'Белогорск', 1000, 28 union
select 'Благовещенск', 1, 28 union
select 'Завитинск', 1000, 28 union
select 'Зея', 1000, 28 union
select 'Райчихинск', 1000, 28 union
select 'Свободный', 1000, 28 union
select 'Сковородино', 1000, 28 union
select 'Тында', 1000, 28 union
select 'Шимановск', 1000, 28 union
select 'Архангельск', 1, 29 union
select 'Вельск', 1000, 29 union
select 'Каргополь', 1000, 29 union
select 'Коряжма', 1000, 29 union
select 'Котлас', 1000, 29 union
select 'Мезень', 1000, 29 union
select 'Мирный', 1000, 29 union
select 'Новодвинск', 1000, 29 union
select 'Няндома', 1000, 29 union
select 'Онега', 1000, 29 union
select 'Северодвинск', 1000, 29 union
select 'Сольвычегодск', 1000, 29 union
select 'Шенкурск', 1000, 29 union
select 'Астрахань', 1, 30 union
select 'Ахтубинск', 1000, 30 union
select 'Знаменск', 1000, 30 union
select 'Камызяк', 1000, 30 union
select 'Нариманов', 1000, 30 union
select 'Харабали', 1000, 30 union
select 'Алексеевка', 1000, 31 union
select 'Белгород', 1, 31 union
select 'Бирюч', 1000, 31 union
select 'Валуйки', 1000, 31 union
select 'Грайворон', 1000, 31 union
select 'Губкин', 1000, 31 union
select 'Короча', 1000, 31 union
select 'Новый Оскол', 1000, 31 union
select 'Старый Оскол', 1000, 31 union
select 'Строитель', 1000, 31 union
select 'Шебекино', 1000, 31 union
select 'Брянск', 1, 32 union
select 'Дятьково', 1000, 32 union
select 'Жуковка', 1000, 32 union
select 'Злынка', 1000, 32 union
select 'Карачев', 1000, 32 union
select 'Клинцы', 1000, 32 union
select 'Мглин', 1000, 32 union
select 'Новозыбков', 1000, 32 union
select 'Почеп', 1000, 32 union
select 'Севск', 1000, 32 union
select 'Сельцо', 1000, 32 union
select 'Стародуб', 1000, 32 union
select 'Сураж', 1000, 32 union
select 'Трубчевск', 1000, 32 union
select 'Унеча', 1000, 32 union
select 'Фокино', 1000, 32 union
select 'Александров', 1000, 33 union
select 'Владимир', 1, 33 union
select 'Вязники', 1000, 33 union
select 'Гороховец', 1000, 33 union
select 'Гусь-Хрустальный', 1000, 33 union
select 'Камешково', 1000, 33 union
select 'Карабаново', 1000, 33 union
select 'Киржач', 1000, 33 union
select 'Ковров', 1000, 33 union
select 'Кольчугино', 1000, 33 union
select 'Костерево', 1000, 33 union
select 'Курлово', 1000, 33 union
select 'Лакинск', 1000, 33 union
select 'Меленки', 1000, 33 union
select 'Муром', 1000, 33 union
select 'Петушки', 1000, 33 union
select 'Покров', 1000, 33 union
select 'Радужный', 1000, 33 union
select 'Собинка', 1000, 33 union
select 'Струнино', 1000, 33 union
select 'Судогда', 1000, 33 union
select 'Суздаль', 1000, 33 union
select 'Юрьев-Польский', 1000, 33 union
select 'Волгоград', 1, 34 union
select 'Волжский', 1000, 34 union
select 'Дубовка', 1000, 34 union
select 'Жирновск', 1000, 34 union
select 'Калач-на-Дону', 1000, 34 union
select 'Камышин', 1000, 34 union
select 'Котельниково', 1000, 34 union
select 'Котово', 1000, 34 union
select 'Краснослободск', 1000, 34 union
select 'Ленинск', 1000, 34 union
select 'Михайловка', 1000, 34 union
select 'Николаевск', 1000, 34 union
select 'Новоаннинский', 1000, 34 union
select 'Палласовка', 1000, 34 union
select 'Петров Вал', 1000, 34 union
select 'Серафимович', 1000, 34 union
select 'Суровикино', 1000, 34 union
select 'Урюпинск', 1000, 34 union
select 'Фролово', 1000, 34 union
select 'Бабаево', 1000, 35 union
select 'Белозерск', 1000, 35 union
select 'Великий Устюг', 1000, 35 union
select 'Вологда', 1, 35 union
select 'Вытегра', 1000, 35 union
select 'Грязовец', 1000, 35 union
select 'Кадников', 1000, 35 union
select 'Кириллов', 1000, 35 union
select 'Красавино', 1000, 35 union
select 'Никольск', 1000, 35 union
select 'Сокол', 1000, 35 union
select 'Тотьма', 1000, 35 union
select 'Устюжна', 1000, 35 union
select 'Харовск', 1000, 35 union
select 'Череповец', 1000, 35 union
select 'Бобров', 1000, 36 union
select 'Богучар', 1000, 36 union
select 'Борисоглебск', 1000, 36 union
select 'Бутурлиновка', 1000, 36 union
select 'Воронеж', 1, 36 union
select 'Калач', 1000, 36 union
select 'Лиски', 1000, 36 union
select 'Нововоронеж', 1000, 36 union
select 'Новохоперск', 1000, 36 union
select 'Острогожск', 1000, 36 union
select 'Павловск', 1000, 36 union
select 'Поворино', 1000, 36 union
select 'Россошь', 1000, 36 union
select 'Семилуки', 1000, 36 union
select 'Эртиль', 1000, 36 union
select 'Вичуга', 1000, 37 union
select 'Гаврилов Посад', 1000, 37 union
select 'Заволжск', 1000, 37 union
select 'Иваново', 1, 37 union
select 'Кинешма', 1000, 37 union
select 'Комсомольск', 1000, 37 union
select 'Кохма', 1000, 37 union
select 'Наволоки', 1000, 37 union
select 'Плес', 1000, 37 union
select 'Приволжск', 1000, 37 union
select 'Пучеж', 1000, 37 union
select 'Родники', 1000, 37 union
select 'Тейково', 1000, 37 union
select 'Фурманов', 1000, 37 union
select 'Шуя', 1000, 37 union
select 'Южа', 1000, 37 union
select 'Юрьевец', 1000, 37 union
select 'Алзамай', 1000, 38 union
select 'Ангарск', 1000, 38 union
select 'Байкальск', 1000, 38 union
select 'Бирюсинск', 1000, 38 union
select 'Бодайбо', 1000, 38 union
select 'Братск', 1000, 38 union
select 'Вихоревка', 1000, 38 union
select 'Железногорск-Илимский', 1000, 38 union
select 'Зима', 1000, 38 union
select 'Иркутск', 1, 38 union
select 'Киренск', 1000, 38 union
select 'Нижнеудинск', 1000, 38 union
select 'Саянск', 1000, 38 union
select 'Свирск', 1000, 38 union
select 'Слюдянка', 1000, 38 union
select 'Тайшет', 1000, 38 union
select 'Тулун', 1000, 38 union
select 'Усолье-Сибирское', 1000, 38 union
select 'Усть-Илимск', 1000, 38 union
select 'Усть-Кут', 1000, 38 union
select 'Черемхово', 1000, 38 union
select 'Шелехов', 1000, 38 union
select 'Багратионовск', 1000, 39 union
select 'Балтийск', 1000, 39 union
select 'Гвардейск', 1000, 39 union
select 'Гурьевск', 1000, 39 union
select 'Гусев', 1000, 39 union
select 'Зеленоградск', 1000, 39 union
select 'Калининград', 1, 39 union
select 'Краснознаменск', 1000, 39 union
select 'Ладушкин', 1000, 39 union
select 'Мамоново', 1000, 39 union
select 'Неман', 1000, 39 union
select 'Нестеров', 1000, 39 union
select 'Озерск', 1000, 39 union
select 'Пионерский', 1000, 39 union
select 'Полесск', 1000, 39 union
select 'Правдинск', 1000, 39 union
select 'Светлогорск', 1000, 39 union
select 'Светлый', 1000, 39 union
select 'Славск', 1000, 39 union
select 'Советск', 1000, 39 union
select 'Черняховск', 1000, 39 union
select 'Балабаново', 1000, 40 union
select 'Белоусово', 1000, 40 union
select 'Боровск', 1000, 40 union
select 'Ермолино', 1000, 40 union
select 'Жиздра', 1000, 40 union
select 'Жуков', 1000, 40 union
select 'Калуга', 1, 40 union
select 'Киров', 1000, 40 union
select 'Козельск', 1000, 40 union
select 'Кондрово', 1000, 40 union
select 'Кременки', 1000, 40 union
select 'Людиново', 1000, 40 union
select 'Малоярославец', 1000, 40 union
select 'Медынь', 1000, 40 union
select 'Мещовск', 1000, 40 union
select 'Мосальск', 1000, 40 union
select 'Обнинск', 1000, 40 union
select 'Сосенский', 1000, 40 union
select 'Спас-Деменск', 1000, 40 union
select 'Сухиничи', 1000, 40 union
select 'Таруса', 1000, 40 union
select 'Юхнов', 1000, 40 union
select 'Юхнов-1', 1000, 40 union
select 'Юхнов-2', 1000, 40 union
select 'Вилючинск', 1000, 41 union
select 'Елизово', 1000, 41 union
select 'Петропавловск-Камчатский', 1, 41 union
select 'Анжеро-Судженск', 1000, 42 union
select 'Белово', 1000, 42 union
select 'Березовский', 1000, 42 union
select 'Гурьевск', 1000, 42 union
select 'Калтан', 1000, 42 union
select 'Кемерово', 1, 42 union
select 'Киселевск', 1000, 42 union
select 'Ленинск-Кузнецкий', 1000, 42 union
select 'Мариинск', 1000, 42 union
select 'Междуреченск', 1000, 42 union
select 'Мыски', 1000, 42 union
select 'Новокузнецк', 1000, 42 union
select 'Осинники', 1000, 42 union
select 'Полысаево', 1000, 42 union
select 'Прокопьевск', 1000, 42 union
select 'Салаир', 1000, 42 union
select 'Тайга', 1000, 42 union
select 'Таштагол', 1000, 42 union
select 'Топки', 1000, 42 union
select 'Юрга', 1000, 42 union
select 'Белая Холуница', 1000, 43 union
select 'Вятские Поляны', 1000, 43 union
select 'Зуевка', 1000, 43 union
select 'Киров', 1, 43 union
select 'Кирово-Чепецк', 1000, 43 union
select 'Кирс', 1000, 43 union
select 'Котельнич', 1000, 43 union
select 'Луза', 1000, 43 union
select 'Малмыж', 1000, 43 union
select 'Мураши', 1000, 43 union
select 'Нолинск', 1000, 43 union
select 'Омутнинск', 1000, 43 union
select 'Орлов', 1000, 43 union
select 'Слободской', 1000, 43 union
select 'Советск', 1000, 43 union
select 'Сосновка', 1000, 43 union
select 'Уржум', 1000, 43 union
select 'Халтурин', 1000, 43 union
select 'Яранск', 1000, 43 union
select 'Буй', 1000, 44 union
select 'Волгореченск', 1000, 44 union
select 'Галич', 1000, 44 union
select 'Кологрив', 1000, 44 union
select 'Кострома', 1, 44 union
select 'Макарьев', 1000, 44 union
select 'Мантурово', 1000, 44 union
select 'Нерехта', 1000, 44 union
select 'Нея', 1000, 44 union
select 'Солигалич', 1000, 44 union
select 'Чухлома', 1000, 44 union
select 'Шарья', 1000, 44 union
select 'Далматово', 1000, 45 union
select 'Катайск', 1000, 45 union
select 'Курган', 1, 45 union
select 'Куртамыш', 1000, 45 union
select 'Макушино', 1000, 45 union
select 'Петухово', 1000, 45 union
select 'Шадринск', 1000, 45 union
select 'Шумиха', 1000, 45 union
select 'Щучье', 1000, 45 union
select 'Дмитриев-Льговский', 1000, 46 union
select 'Железногорск', 1000, 46 union
select 'Курск', 1, 46 union
select 'Курчатов', 1000, 46 union
select 'Льгов', 1000, 46 union
select 'Обоянь', 1000, 46 union
select 'Рыльск', 1000, 46 union
select 'Суджа', 1000, 46 union
select 'Усланский', 1000, 46 union
select 'Фатеж', 1000, 46 union
select 'Щигры', 1000, 46 union
select 'Бокситогорск', 1000, 47 union
select 'Волосово', 1000, 47 union
select 'Волхов', 1000, 47 union
select 'Всеволожск', 1000, 47 union
select 'Выборг', 1000, 47 union
select 'Высоцк', 1000, 47 union
select 'Гатчина', 1000, 47 union
select 'Зеленогорск', 1000, 47 union
select 'Ивангород', 1000, 47 union
select 'Каменногорск', 1000, 47 union
select 'Кингисепп', 1000, 47 union
select 'Кириши', 1000, 47 union
select 'Кировск', 1000, 47 union
select 'Колпино', 1000, 47 union
select 'Коммунар', 1000, 47 union
select 'Красное Село', 1000, 47 union
select 'Кронштадт', 1000, 47 union
select 'Лодейное Поле', 1000, 47 union
select 'Ломоносов', 1000, 47 union
select 'Луга', 1000, 47 union
select 'Любань', 1000, 47 union
select 'Никольское', 1000, 47 union
select 'Новая Ладога', 1000, 47 union
select 'Отрадное', 1000, 47 union
select 'Павловск', 1000, 47 union
select 'Петергоф', 1000, 47 union
select 'Петродворец', 1000, 47 union
select 'Пикалево', 1000, 47 union
select 'Подпорожье', 1000, 47 union
select 'Приморск', 1000, 47 union
select 'Приозерск', 1000, 47 union
select 'Пушкин', 1000, 47 union
select 'Сарженка массив', 1000, 47 union
select 'Светогорск', 1000, 47 union
select 'Сертолово', 1000, 47 union
select 'Сестрорецк', 1000, 47 union
select 'Сланцы', 1000, 47 union
select 'Сосновый Бор', 1000, 47 union
select 'Сясьстрой', 1000, 47 union
select 'Тихвин', 1000, 47 union
select 'Тосно', 1000, 47 union
select 'Хиттолово массив', 1000, 47 union
select 'Шлиссельбург', 1000, 47 union
select 'Юкки массив', 1000, 47 union
select 'Грязи', 1000, 48 union
select 'Данков', 1000, 48 union
select 'Елец', 1000, 48 union
select 'Задонск', 1000, 48 union
select 'Лебедянь', 1000, 48 union
select 'Липецк', 1, 48 union
select 'Усмань', 1000, 48 union
select 'Чаплыгин', 1000, 48 union
select 'Магадан', 1, 49 union
select 'Сусуман', 1000, 49 union
select 'Апрелевка', 1000, 50 union
select 'Балашиха', 1000, 50 union
select 'Бронницы', 1000, 50 union
select 'Верея', 1000, 50 union
select 'Видное', 1000, 50 union
select 'Волоколамск', 1000, 50 union
select 'Воскресенск', 1000, 50 union
select 'Высоковск', 1000, 50 union
select 'Голицыно', 1000, 50 union
select 'Дедовск', 1000, 50 union
select 'Дзержинский', 1000, 50 union
select 'Дмитров', 1000, 50 union
select 'Долгопрудный', 1000, 50 union
select 'Домодедово', 1000, 50 union
select 'Дрезна', 1000, 50 union
select 'Дубна', 1000, 50 union
select 'Егорьевск', 1000, 50 union
select 'Железнодорожный', 1000, 50 union
select 'Жуковский', 1000, 50 union
select 'Зарайск', 1000, 50 union
select 'Звенигород', 1000, 50 union
select 'Ивантеевка', 1000, 50 union
select 'Истра', 1000, 50 union
select 'Калининград', 1000, 50 union
select 'Кашира', 1000, 50 union
select 'Климовск', 1000, 50 union
select 'Клин', 1000, 50 union
select 'Коломна', 1000, 50 union
select 'Королев', 1000, 50 union
select 'Котельники', 1000, 50 union
select 'Красноармейск', 1000, 50 union
select 'Красногорск', 1000, 50 union
select 'Краснозаводск', 1000, 50 union
select 'Краснознаменск', 1000, 50 union
select 'Кубинка', 1000, 50 union
select 'Куровское', 1000, 50 union
select 'Ликино-Дулево', 1000, 50 union
select 'Лобня', 1000, 50 union
select 'Лосино-Петровский', 1000, 50 union
select 'Луховицы', 1000, 50 union
select 'Лыткарино', 1000, 50 union
select 'Люберцы', 1000, 50 union
select 'Можайск', 1000, 50 union
select 'Московский', 1000, 50 union
select 'Мытищи', 1000, 50 union
select 'Наро-Фоминск', 1000, 50 union
select 'Ногинск', 1000, 50 union
select 'Одинцово', 1000, 50 union
select 'Одинцово-10', 1000, 50 union
select 'Ожерелье', 1000, 50 union
select 'Озеры', 1000, 50 union
select 'Орехово-Зуево', 1000, 50 union
select 'Павловский Посад', 1000, 50 union
select 'Пересвет', 1000, 50 union
select 'Подольск', 1000, 50 union
select 'Протвино', 1000, 50 union
select 'Пушкино', 1000, 50 union
select 'Пущино', 1000, 50 union
select 'Раменское', 1000, 50 union
select 'Реутов', 1000, 50 union
select 'Рошаль', 1000, 50 union
select 'Руза', 1000, 50 union
select 'Сергиев Посад', 1000, 50 union
select 'Серпухов', 1000, 50 union
select 'Солнечногорск', 1000, 50 union
select 'Солнечногорск-2', 1000, 50 union
select 'Солнечногорск-25', 1000, 50 union
select 'Солнечногорск-30', 1000, 50 union
select 'Солнечногорск-7', 1000, 50 union
select 'Старая Купавна', 1000, 50 union
select 'Ступино', 1000, 50 union
select 'Сходня', 1000, 50 union
select 'Талдом', 1000, 50 union
select 'Троицк', 1000, 50 union
select 'Фрязино', 1000, 50 union
select 'Химки', 1000, 50 union
select 'Хотьково', 1000, 50 union
select 'Черноголовка', 1000, 50 union
select 'Чехов', 1000, 50 union
select 'Чехов-1', 1000, 50 union
select 'Чехов-2', 1000, 50 union
select 'Чехов-3', 1000, 50 union
select 'Чехов-4', 1000, 50 union
select 'Чехов-5', 1000, 50 union
select 'Чехов-6', 1000, 50 union
select 'Чехов-7', 1000, 50 union
select 'Чехов-8', 1000, 50 union
select 'Шатура', 1000, 50 union
select 'Щелково', 1000, 50 union
select 'Щербинка', 1000, 50 union
select 'Электрогорск', 1000, 50 union
select 'Электросталь', 1000, 50 union
select 'Электроугли', 1000, 50 union
select 'Юбилейный', 1000, 50 union
select 'Яхрома', 1000, 50 union
select 'Апатиты', 1000, 51 union
select 'Гаджиево', 1000, 51 union
select 'Заозерск', 1000, 51 union
select 'Заполярный', 1000, 51 union
select 'Кандалакша', 1000, 51 union
select 'Кировск', 1000, 51 union
select 'Ковдор', 1000, 51 union
select 'Кола', 1000, 51 union
select 'Мончегорск', 1000, 51 union
select 'Мурманск', 1, 51 union
select 'Оленегорск', 1000, 51 union
select 'Оленегорск-1', 1000, 51 union
select 'Оленегорск-2', 1000, 51 union
select 'Оленегорск-4', 1000, 51 union
select 'Островной', 1000, 51 union
select 'Полярные Зори', 1000, 51 union
select 'Полярный', 1000, 51 union
select 'Североморск', 1000, 51 union
select 'Снежногорск', 1000, 51 union
select 'Арзамас', 1000, 52 union
select 'Балахна', 1000, 52 union
select 'Богородск', 1000, 52 union
select 'Бор', 1000, 52 union
select 'Ветлуга', 1000, 52 union
select 'Володарск', 1000, 52 union
select 'Ворсма', 1000, 52 union
select 'Выкса', 1000, 52 union
select 'Горбатов', 1000, 52 union
select 'Городец', 1000, 52 union
select 'Горький', 1000, 52 union
select 'Дзержинск', 1000, 52 union
select 'Заволжье', 1000, 52 union
select 'Княгинино', 1000, 52 union
select 'Кстово', 1000, 52 union
select 'Кулебаки', 1000, 52 union
select 'Лукоянов', 1000, 52 union
select 'Лысково', 1000, 52 union
select 'Навашино', 1000, 52 union
select 'Нижний Новгород', 1, 52 union
select 'Павлово', 1000, 52 union
select 'Первомайск', 1000, 52 union
select 'Перевоз', 1000, 52 union
select 'Саров', 1000, 52 union
select 'Семенов', 1000, 52 union
select 'Сергач', 1000, 52 union
select 'Урень', 1000, 52 union
select 'Чкаловск', 1000, 52 union
select 'Шахунья', 1000, 52 union
select 'Боровичи', 1000, 53 union
select 'Валдай', 1000, 53 union
select 'Великий Новгород', 1, 53 union
select 'Малая Вишера', 1000, 53 union
select 'Окуловка', 1000, 53 union
select 'Пестово', 1000, 53 union
select 'Сольцы', 1000, 53 union
select 'Сольцы 2', 1000, 53 union
select 'Старая Русса', 1000, 53 union
select 'Холм', 1000, 53 union
select 'Чудово', 1000, 53 union
select 'Барабинск', 1000, 54 union
select 'Бердск', 1000, 54 union
select 'Болотное', 1000, 54 union
select 'Искитим', 1000, 54 union
select 'Карасук', 1000, 54 union
select 'Каргат', 1000, 54 union
select 'Куйбышев', 1000, 54 union
select 'Купино', 1000, 54 union
select 'Новосибирск', 1, 54 union
select 'Обь', 1000, 54 union
select 'Татарск', 1000, 54 union
select 'Тогучин', 1000, 54 union
select 'Черепаново', 1000, 54 union
select 'Чулым', 1000, 54 union
select 'Чулым-3', 1000, 54 union
select 'Исилькуль', 1000, 55 union
select 'Калачинск', 1000, 55 union
select 'Называевск', 1000, 55 union
select 'Омск', 1, 55 union
select 'Тара', 1000, 55 union
select 'Тюкалинск', 1000, 55 union
select 'Абдулино', 1000, 56 union
select 'Бугуруслан', 1000, 56 union
select 'Бузулук', 1000, 56 union
select 'Гай', 1000, 56 union
select 'Кувандык', 1000, 56 union
select 'Медногорск', 1000, 56 union
select 'Новотроицк', 1000, 56 union
select 'Оренбург', 1, 56 union
select 'Орск', 1000, 56 union
select 'Соль-Илецк', 1000, 56 union
select 'Сорочинск', 1000, 56 union
select 'Ясный', 1000, 56 union
select 'Болхов', 1000, 57 union
select 'Дмитровск', 1000, 57 union
select 'Ливны', 1000, 57 union
select 'Малоархангельск', 1000, 57 union
select 'Мценск', 1000, 57 union
select 'Новосиль', 1000, 57 union
select 'Орел', 1, 57 union
select 'Беднодемьяновск', 1000, 58 union
select 'Белинский', 1000, 58 union
select 'Городище', 1000, 58 union
select 'Заречный', 1000, 58 union
select 'Каменка', 1000, 58 union
select 'Кузнецк', 1000, 58 union
select 'Кузнецк-12', 1000, 58 union
select 'Кузнецк-8', 1000, 58 union
select 'Нижний Ломов', 1000, 58 union
select 'Никольск', 1000, 58 union
select 'Пенза', 1, 58 union
select 'Сердобск', 1000, 58 union
select 'Спасск', 1000, 58 union
select 'Сурск', 1000, 58 union
select 'Александровск', 1000, 59 union
select 'Березники', 1000, 59 union
select 'Верещагино', 1000, 59 union
select 'Горнозаводск', 1000, 59 union
select 'Гремячинск', 1000, 59 union
select 'Губаха', 1000, 59 union
select 'Добрянка', 1000, 59 union
select 'Кизел', 1000, 59 union
select 'Красновишерск', 1000, 59 union
select 'Краснокамск', 1000, 59 union
select 'Кудымкар', 1000, 59 union
select 'Кунгур', 1000, 59 union
select 'Лысьва', 1000, 59 union
select 'Нытва', 1000, 59 union
select 'Оса', 1000, 59 union
select 'Оханск', 1000, 59 union
select 'Очер', 1000, 59 union
select 'Пермь', 1, 59 union
select 'Соликамск', 1000, 59 union
select 'Усолье', 1000, 59 union
select 'Чайковский', 1000, 59 union
select 'Чердынь', 1000, 59 union
select 'Чермоз', 1000, 59 union
select 'Чернушка', 1000, 59 union
select 'Чусовой', 1000, 59 union
select 'Великие Луки', 1000, 60 union
select 'Гдов', 1000, 60 union
select 'Дно', 1000, 60 union
select 'Невель', 1000, 60 union
select 'Новоржев', 1000, 60 union
select 'Новосокольники', 1000, 60 union
select 'Опочка', 1000, 60 union
select 'Остров', 1000, 60 union
select 'Печоры', 1000, 60 union
select 'Порхов', 1000, 60 union
select 'Псков', 1, 60 union
select 'Пустошка', 1000, 60 union
select 'Пыталово', 1000, 60 union
select 'Себеж', 1000, 60 union
select 'Азов', 1000, 61 union
select 'Аксай', 1000, 61 union
select 'Батайск', 1000, 61 union
select 'Белая Калитва', 1000, 61 union
select 'Волгодонск', 1000, 61 union
select 'Гуково', 1000, 61 union
select 'Донецк', 1000, 61 union
select 'Зверево', 1000, 61 union
select 'Зерноград', 1000, 61 union
select 'Каменск-Шахтинский', 1000, 61 union
select 'Константиновск', 1000, 61 union
select 'Красный Сулин', 1000, 61 union
select 'Миллерово', 1000, 61 union
select 'Морозовск', 1000, 61 union
select 'Новочеркасск', 1000, 61 union
select 'Новошахтинск', 1000, 61 union
select 'Пролетарск', 1000, 61 union
select 'Ростов-на-Дону', 1, 61 union
select 'Сальск', 1000, 61 union
select 'Семикаракорск', 1000, 61 union
select 'Таганрог', 1000, 61 union
select 'Цимлянск', 1000, 61 union
select 'Шахты', 1000, 61 union
select 'Касимов', 1000, 62 union
select 'Кораблино', 1000, 62 union
select 'Михайлов', 1000, 62 union
select 'Новомичуринск', 1000, 62 union
select 'Рыбное', 1000, 62 union
select 'Ряжск', 1000, 62 union
select 'Рязань', 1, 62 union
select 'Сасово', 1000, 62 union
select 'Скопин', 1000, 62 union
select 'Спас-Клепики', 1000, 62 union
select 'Спасск-Рязанский', 1000, 62 union
select 'Шацк', 1000, 62 union
select 'Жигулевск', 1000, 63 union
select 'Кинель', 1000, 63 union
select 'Нефтегорск', 1000, 63 union
select 'Новокуйбышевск', 1000, 63 union
select 'Октябрьск', 1000, 63 union
select 'Отрадный', 1000, 63 union
select 'Похвистнево', 1000, 63 union
select 'Самара', 1, 63 union
select 'Сызрань', 1000, 63 union
select 'Тольятти', 1000, 63 union
select 'Чапаевск', 1000, 63 union
select 'Аркадак', 1000, 64 union
select 'Аткарск', 1000, 64 union
select 'Балаково', 1000, 64 union
select 'Балашов', 1000, 64 union
select 'Вольск', 1000, 64 union
select 'Ершов', 1000, 64 union
select 'Калининск', 1000, 64 union
select 'Красноармейск', 1000, 64 union
select 'Красный Кут', 1000, 64 union
select 'Маркс', 1000, 64 union
select 'Новоузенск', 1000, 64 union
select 'Петровск', 1000, 64 union
select 'Пугачев', 1000, 64 union
select 'Ртищево', 1000, 64 union
select 'Саратов', 1, 64 union
select 'Хвалынск', 1000, 64 union
select 'Шиханы', 1000, 64 union
select 'Энгельс', 1000, 64 union
select 'Александровск-Сахалинский', 1000, 65 union
select 'Анива', 1000, 65 union
select 'Горнозаводск', 1000, 65 union
select 'Долинск', 1000, 65 union
select 'Корсаков', 1000, 65 union
select 'Красногорск', 1000, 65 union
select 'Курильск', 1000, 65 union
select 'Макаров', 1000, 65 union
select 'Невельск', 1000, 65 union
select 'Оха', 1000, 65 union
select 'Поронайск', 1000, 65 union
select 'Северо-Курильск', 1000, 65 union
select 'Томари', 1000, 65 union
select 'Углегорск', 1000, 65 union
select 'Холмск', 1000, 65 union
select 'Чехов', 1000, 65 union
select 'Шахтерск', 1000, 65 union
select 'Южно-Сахалинск', 1, 65 union
select 'Алапаевск', 1000, 66 union
select 'Арамиль', 1000, 66 union
select 'Артемовский', 1000, 66 union
select 'Асбест', 1000, 66 union
select 'Березовский', 1000, 66 union
select 'Богданович', 1000, 66 union
select 'Верхний Тагил', 1000, 66 union
select 'Верхняя Пышма', 1000, 66 union
select 'Верхняя Салда', 1000, 66 union
select 'Верхняя Тура', 1000, 66 union
select 'Верхотурье', 1000, 66 union
select 'Волчанск', 1000, 66 union
select 'Дегтярск', 1000, 66 union
select 'Екатеринбург', 1, 66 union
select 'Заречный', 1000, 66 union
select 'Ивдель', 1000, 66 union
select 'Ирбит', 1000, 66 union
select 'Каменск-Уральский', 1000, 66 union
select 'Камышлов', 1000, 66 union
select 'Карпинск', 1000, 66 union
select 'Качканар', 1000, 66 union
select 'Кировград', 1000, 66 union
select 'Краснотурьинск', 1000, 66 union
select 'Красноуральск', 1000, 66 union
select 'Красноуфимск', 1000, 66 union
select 'Кушва', 1000, 66 union
select 'Лесной', 1000, 66 union
select 'Михайловск', 1000, 66 union
select 'Невьянск', 1000, 66 union
select 'Нижние Серги', 1000, 66 union
select 'Нижний Тагил', 1000, 66 union
select 'Нижняя Салда', 1000, 66 union
select 'Нижняя Тура', 1000, 66 union
select 'Новая Ляля', 1000, 66 union
select 'Новоуральск', 1000, 66 union
select 'Первоуральск', 1000, 66 union
select 'Полевской', 1000, 66 union
select 'Ревда', 1000, 66 union
select 'Реж', 1000, 66 union
select 'Свердловск', 1000, 66 union
select 'Свердловск-44', 1000, 66 union
select 'Свердловск-45', 1000, 66 union
select 'Североуральск', 1000, 66 union
select 'Серов', 1000, 66 union
select 'Среднеуральск', 1000, 66 union
select 'Сухой Лог', 1000, 66 union
select 'Сысерть', 1000, 66 union
select 'Тавда', 1000, 66 union
select 'Талица', 1000, 66 union
select 'Туринск', 1000, 66 union
select 'Велиж', 1000, 67 union
select 'Вязьма', 1000, 67 union
select 'Гагарин', 1000, 67 union
select 'Демидов', 1000, 67 union
select 'Десногорск', 1000, 67 union
select 'Дорогобуж', 1000, 67 union
select 'Духовщина', 1000, 67 union
select 'Ельня', 1000, 67 union
select 'Починок', 1000, 67 union
select 'Рославль', 1000, 67 union
select 'Рудня', 1000, 67 union
select 'Сафоново', 1000, 67 union
select 'Смоленск', 1, 67 union
select 'Сычевка', 1000, 67 union
select 'Ярцево', 1000, 67 union
select 'Жердевка', 1000, 68 union
select 'Кирсанов', 1000, 68 union
select 'Котовск', 1000, 68 union
select 'Мичуринск', 1000, 68 union
select 'Моршанск', 1000, 68 union
select 'Рассказово', 1000, 68 union
select 'Тамбов', 1, 68 union
select 'Уварово', 1000, 68 union
select 'Андреаполь', 1000, 69 union
select 'Бежецк', 1000, 69 union
select 'Белый', 1000, 69 union
select 'Бологое', 1000, 69 union
select 'Весьегонск', 1000, 69 union
select 'Вышний Волочек', 1000, 69 union
select 'Западная Двина', 1000, 69 union
select 'Зубцов', 1000, 69 union
select 'Калинин', 1000, 69 union
select 'Калязин', 1000, 69 union
select 'Кашин', 1000, 69 union
select 'Кимры', 1000, 69 union
select 'Конаково', 1000, 69 union
select 'Красный Холм', 1000, 69 union
select 'Кувшиново', 1000, 69 union
select 'Лихославль', 1000, 69 union
select 'Нелидово', 1000, 69 union
select 'Осташков', 1000, 69 union
select 'Ржев', 1000, 69 union
select 'Старица', 1000, 69 union
select 'Тверь', 1, 69 union
select 'Торжок', 1000, 69 union
select 'Торопец', 1000, 69 union
select 'Удомля', 1000, 69 union
select 'Асино', 1000, 70 union
select 'Кедровый', 1000, 70 union
select 'Колпашево', 1000, 70 union
select 'Северск', 1000, 70 union
select 'Стрежевой', 1000, 70 union
select 'Томск', 1, 70 union
select 'Алексин', 1000, 71 union
select 'Белев', 1000, 71 union
select 'Богородицк', 1000, 71 union
select 'Болохово', 1000, 71 union
select 'Венев', 1000, 71 union
select 'Донской', 1000, 71 union
select 'Ефремов', 1000, 71 union
select 'Кимовск', 1000, 71 union
select 'Киреевск', 1000, 71 union
select 'Липки', 1000, 71 union
select 'Новомосковск', 1000, 71 union
select 'Плавск', 1000, 71 union
select 'Советск', 1000, 71 union
select 'Сокольники', 1000, 71 union
select 'Суворов', 1000, 71 union
select 'Тула', 1, 71 union
select 'Узловая', 1000, 71 union
select 'Чекалин', 1000, 71 union
select 'Щекино', 1000, 71 union
select 'Ясногорск', 1000, 71 union
select 'Заводоуковск', 1000, 72 union
select 'Ишим', 1000, 72 union
select 'Тобольск', 1000, 72 union
select 'Тюмень', 1, 72 union
select 'Ялуторовск', 1000, 72 union
select 'Барыш', 1000, 73 union
select 'Димитровград', 1000, 73 union
select 'Инза', 1000, 73 union
select 'Новоульяновск', 1000, 73 union
select 'Сенгилей', 1000, 73 union
select 'Ульяновск', 1, 73 union
select 'Аша', 1000, 74 union
select 'Бакал', 1000, 74 union
select 'Верхнеуральск', 1000, 74 union
select 'Верхний Уфалей', 1000, 74 union
select 'Еманжелинск', 1000, 74 union
select 'Златоуст', 1000, 74 union
select 'Карабаш', 1000, 74 union
select 'Карталы', 1000, 74 union
select 'Касли', 1000, 74 union
select 'Катав-Ивановск', 1000, 74 union
select 'Копейск', 1000, 74 union
select 'Коркино', 1000, 74 union
select 'Куса', 1000, 74 union
select 'Кыштым', 1000, 74 union
select 'Магнитогорск', 1000, 74 union
select 'Миасс', 1000, 74 union
select 'Миньяр', 1000, 74 union
select 'Нязепетровск', 1000, 74 union
select 'Озерск', 1000, 74 union
select 'Пласт', 1000, 74 union
select 'Сатка', 1000, 74 union
select 'Сим', 1000, 74 union
select 'Снежинск', 1000, 74 union
select 'Трехгорный', 1000, 74 union
select 'Трехгорный-1', 1000, 74 union
select 'Троицк', 1000, 74 union
select 'Усть-Катав', 1000, 74 union
select 'Чебаркуль', 1000, 74 union
select 'Челябинск', 1, 74 union
select 'Южноуральск', 1000, 74 union
select 'Юрюзань', 1000, 74 union
select 'Балей', 1000, 75 union
select 'Борзя', 1000, 75 union
select 'Краснокаменск', 1000, 75 union
select 'Могоча', 1000, 75 union
select 'Нерчинск', 1000, 75 union
select 'Петровск-Забайкальский', 1000, 75 union
select 'Сретенск', 1000, 75 union
select 'Хилок', 1000, 75 union
select 'Чита', 1, 75 union
select 'Чита-46', 1000, 75 union
select 'Шилка', 1000, 75 union
select 'Гаврилов-Ям', 1000, 76 union
select 'Данилов', 1000, 76 union
select 'Любим', 1000, 76 union
select 'Мышкин', 1000, 76 union
select 'Переславль-Залесский', 1000, 76 union
select 'Пошехонье', 1000, 76 union
select 'Ростов', 1000, 76 union
select 'Рыбинск', 1000, 76 union
select 'Тутаев', 1000, 76 union
select 'Углич', 1000, 76 union
select 'Ярославль', 1, 76 union
select 'Биробиджан', 1, 79 union
select 'Облучье', 1000, 79 union
select 'Нарьян-Мар', 1, 83 union
select 'Белоярский', 1000, 86 union
select 'Когалым', 1000, 86 union
select 'Лангепас', 1000, 86 union
select 'Лянтор', 1000, 86 union
select 'Мегион', 1000, 86 union
select 'Нефтеюганск', 1000, 86 union
select 'Нижневартовск', 1000, 86 union
select 'Нягань', 1000, 86 union
select 'Покачи', 1000, 86 union
select 'Пыть-Ях', 1000, 86 union
select 'Радужный', 1000, 86 union
select 'Советский', 1000, 86 union
select 'Сургут', 1000, 86 union
select 'Урай', 1000, 86 union
select 'Ханты-Мансийск', 1, 86 union
select 'Югорск', 1000, 86 union
select 'Анадырь', 1, 87 union
select 'Билибино', 1000, 87 union
select 'Певек', 1000, 87 union
select 'Губкинский', 1000, 89 union
select 'Лабытнанги', 1000, 89 union
select 'Муравленко', 1000, 89 union
select 'Надым', 1000, 89 union
select 'Новый Уренгой', 1000, 89 union
select 'Ноябрьск', 1000, 89 union
select 'Салехард', 1, 89 union
select 'Тарко-Сале', 1000, 89 

GO

