/*---------------------------------------------------------------------------------------
  Скрипт для обновления базы до версии 0.9.62

  Что нового:
	+ добавлены хранимые процедуры для построения отчета 0002 (по реализациям)
	
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

SET @PreviousVersion = '0.9.61' 			-- номер ПРЕДЫДУЩЕЙ версии
SET @NewVersion     = '0.9.62'			-- номер новой версии

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

	BACKUP DATABASE @DataBaseName TO DISK = @BackupTarget WITH  INIT,  NOUNLOAD,  NAME = N'wholesale', NOSKIP, DESCRIPTION = N'Обновление версии', NOFORMAT

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


IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableStorages'
)
   DROP PROCEDURE dbo.GetAvailableStorages
GO

CREATE PROCEDURE dbo.GetAvailableStorages
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllStorages BIT,	-- Признак выбора всех мест хранения
	@UserId INT	-- Код пользователя
)
AS

-- Таблица, в которой хранятся коды выбранных МХ с учетом прав
CREATE TABLE #AvailableStorageListTable(
	Id TINYINT not null,
	Name VARCHAR(200) not null,
	StorageTypeId TINYINT not null
)

CREATE TABLE #VisibleStorages (
	Id TINYINT not null,
	NAME VARCHAR(200) not null,
	StorageTypeId TINYINT not null
)
CREATE TABLE #SelectedStorages(Id TINYINT not null)

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр МХ

-- Получаем право на просмотр МХ	
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(1901, @UserId)

IF @permissionDistributionTypeId = 2
	INSERT INTO #VisibleStorages (Id, Name, StorageTypeId)
	SELECT s.Id, s.Name, s.StorageTypeId
	FROM dbo.[Storage] s
		join dbo.[TeamStorage] ts on ts.StorageId = s.Id
		join dbo.[Team] t on t.Id = ts.TeamId
		join dbo.[UserTeam] ut on ut.TeamId = t.Id
		join dbo.[User] u on u.Id = ut.UserId
	WHERE u.Id = @UserId AND s.DeletionDate is null AND t.DeletionDate is null
	GROUP BY s.Id, s.Name, s.StorageTypeId
ELSE IF @permissionDistributionTypeId = 3
	INSERT INTO #VisibleStorages (Id, Name,StorageTypeId)
	SELECT s.Id, s.Name, s.StorageTypeId FROM dbo.[Storage] s WHERE s.DeletionDate is null

-- Если указаны МХ, то выбираем их из видимых
IF @AllStorages = 0
BEGIN
	-- Таблица, в которую помещаются коды после парсинга в строковом виде
	CREATE TABLE #StringIdListTable (Id VARCHAR(50) not null)

	INSERT INTO #StringIdListTable (Id)
	SELECT Id FROM dbo.SplitIntIdList(@IdList)	-- Парсим список кодов
	
	-- Получаем коды выбранных МХ
	INSERT INTO #SelectedStorages
	SELECT CONVERT(TINYINT, Id) FROM #StringIdListTable

	INSERT INTO #AvailableStorageListTable (Id, Name, StorageTypeId)
	SELECT vs.Id, vs.Name, vs.StorageTypeId 
	FROM #VisibleStorages vs
		join #SelectedStorages ss on ss.Id = vs.Id
		
	DROP TABLE #StringIdListTable
END
ELSE	-- Иначе берем все видимые МХ
	INSERT INTO #AvailableStorageListTable (Id, Name, StorageTypeId)
	SELECT Id, Name, StorageTypeId FROM #VisibleStorages

SELECT Id, Name, StorageTypeId FROM #AvailableStorageListTable

DROP TABLE #VisibleStorages
DROP TABLE #SelectedStorages
DROP TABLE #AvailableStorageListTable

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableClients'
)
   DROP PROCEDURE dbo.GetAvailableClients
GO

CREATE PROCEDURE dbo.GetAvailableClients
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllClients BIT,	-- Признак выбора всех клиентов
	@UserId INT	-- Код пользователя
)
AS

-- Таблица, в которой хранятся коды выбранных клиентов с учетом прав
CREATE TABLE #AvailableClientListTable(
	Id INT not null,
	Name VARCHAR(200) not null
)

CREATE TABLE #SelectedClients -- список выбранных клиентов
(
	Id INT not null
)	

DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр клиентов

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(3001, @UserId)

-- Если права нет, то выборка всегда пустая
IF @permissionDistributionTypeId = 3
BEGIN
	-- Если указаны выбранные коды, то
	IF @AllClients = 0
	BEGIN
		-- Таблица, в которую помещаются коды после парсинга в строковом виде
		CREATE TABLE #StringIdListTable (Id VARCHAR(50) not null)

		INSERT INTO #StringIdListTable (Id)
		SELECT Id 
		FROM dbo.SplitIntIdList(@IdList)	-- Парсим список кодов
		
		-- Получаем выбранные коды
		INSERT INTO #SelectedClients
		SELECT CONVERT(INT, Id) 
		FROM #StringIdListTable
		
		-- Берем пересечение выбранных кодов и видимых
		INSERT INTO #AvailableClientListTable (Id, Name)
		SELECT c.Id, co.Name
		FROM #SelectedClients sc
		join dbo.[Client] c on c.Id = sc.Id
		join dbo.[Contractor] co on co.Id = c.Id
		WHERE co.DeletionDate is null
		
		DROP TABLE #StringIdListTable
	END
	ELSE
		-- Берем только видимые коды
		INSERT INTO #AvailableClientListTable (Id, Name)
		SELECT c.Id, co.Name 
		FROM dbo.[Client] c
		join dbo.[Contractor] co on co.Id = c.Id
		WHERE co.DeletionDate is null
END

SELECT Id, Name 
FROM #AvailableClientListTable

DROP TABLE #SelectedClients
DROP TABLE #AvailableClientListTable

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableArticleGroups'
)
   DROP PROCEDURE dbo.GetAvailableArticleGroups
GO

CREATE PROCEDURE dbo.GetAvailableArticleGroups
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllArticleGroups BIT	-- Признак выбора всех групп товаров
)
AS
	
-- Таблица, в которой хранятся коды выбранных групп товаров с учетом прав
CREATE TABLE #AvailableArticleGroupListTable (
	Id TINYINT not null,
	Name VARCHAR(200) not null
)

IF @AllArticleGroups = 0 
BEGIN
	-- Таблица, в которую помещаются коды после парсинга в строковом виде
	CREATE TABLE #StringIdListTable (Id VARCHAR(50) not null)

	INSERT INTO #StringIdListTable (Id)
	SELECT Id 
	FROM dbo.SplitIntIdList(@IdList)	-- Парсим список кодов
	
	-- Вставляем коды
	INSERT INTO #AvailableArticleGroupListTable (Id, Name)
	SELECT CONVERT(TINYINT, Id), '' 
	FROM #StringIdListTable
	
	-- Дабавляем названия
	UPDATE #AvailableArticleGroupListTable
	SET Name = ag.Name
	FROM dbo.ArticleGroup ag
	WHERE ag.Id = #AvailableArticleGroupListTable.Id
	
	DROP TABLE #StringIdListTable
END
ELSE
	INSERT INTO #AvailableArticleGroupListTable (Id, Name)
	SELECT Id, Name 
	FROM dbo.[ArticleGroup]

SELECT Id, Name 
FROM #AvailableArticleGroupListTable

DROP TABLE #AvailableArticleGroupListTable

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableAccountOrganizations'
)
   DROP PROCEDURE dbo.GetAvailableAccountOrganizations
GO

CREATE PROCEDURE dbo.GetAvailableAccountOrganizations
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllAccountOrganizations BIT	-- Признак выбора всех организаций аккаунта
)
AS

-- Таблица, в которой хранятся коды выбранных  организаций аккаунта с учетом прав
CREATE TABLE #AvailableAccountOrganizationListTable(
	Id INT not null,
	Name VARCHAR(100) not null
)

IF @AllAccountOrganizations = 0
BEGIN
	-- Таблица, в которую помещаются коды после парсинга в строковом виде
	CREATE TABLE #StringIdListTable (Id VARCHAR(50) not null)

	INSERT INTO #StringIdListTable (Id)
	SELECT Id FROM dbo.SplitIntIdList(@IdList)	-- Парсим список кодов
	
	-- Вставляем коды
	INSERT INTO #AvailableAccountOrganizationListTable
	SELECT CONVERT(INT, Id), '' 
	FROM #StringIdListTable
	
	-- Дабавляем названия
	UPDATE #AvailableAccountOrganizationListTable
	SET Name = o.ShortName
	FROM dbo.[Organization] o
	WHERE o.Id = #AvailableAccountOrganizationListTable.Id
		and o.DeletionDate is null
	
	DROP TABLE #StringIdListTable
END
ELSE
	INSERT INTO #AvailableAccountOrganizationListTable
	SELECT ao.Id, o.ShortName 
	FROM dbo.[AccountOrganization] ao
		join dbo.[Organization] o on o.Id = ao.Id
	WHERE o.DeletionDate is null

SELECT Id, Name 
FROM #AvailableAccountOrganizationListTable

DROP TABLE #AvailableAccountOrganizationListTable
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAccountingPriceForArticlesOnStoragesByDate'
)
   DROP PROCEDURE dbo.GetAccountingPriceForArticlesOnStoragesByDate
GO

CREATE PROCEDURE GetAccountingPriceForArticlesOnStoragesByDate(
	@Date DATETIME	-- Дата, на которую необходимо получить УЦ
) AS

-- вспомогательная таблица для формирования перечня УЦ для товаров на МХ
create table #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination(
	ArticleId INT not null,
	StorageId SMALLINT not null,
	AccountingPrice DECIMAL(18,2) not null,
	RowNumber INT not null
)

-- Получаем УЦ для товаров и сортируем их по дате вступления в действие в обратном порядке(т.е. последний вступивший в действие будет первым)
INSERT INTO #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination(ArticleId,StorageId,AccountingPrice,RowNumber)
SELECT aapi.ArticleId, aapi.StorageId, aapi.AccountingPrice, 
	ROW_NUMBER() OVER(PARTITION BY aapi.ArticleId, aapi.StorageId ORDER BY aapi.StartDate DESC )
FROM ArticleAccountingPriceIndicator aapi
	join #ArticleAccountingPriceForActiclesOnStoragesDetermination d on d.ArticleId = aapi.ArticleId AND d.StorageId = aapi.StorageId AND 
	(aapi.StartDate <= @Date AND (aapi.EndDate > @Date OR aapi.EndDate is null))

-- Заполняем входную таблицу значениями УЦ
UPDATE #ArticleAccountingPriceForActiclesOnStoragesDetermination
SET AccountingPrice = api.AccountingPrice
FROM #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination api
WHERE api.ArticleId = #ArticleAccountingPriceForActiclesOnStoragesDetermination.ArticleId AND
	api.StorageId = #ArticleAccountingPriceForActiclesOnStoragesDetermination.StorageId AND
	api.RowNumber = 1	-- берем последнюю УЦ по дате вступления в действие

drop table #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination	

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableUsers'
)
   DROP PROCEDURE dbo.GetAvailableUsers
GO

CREATE PROCEDURE dbo.GetAvailableUsers
(
	@IdList VARCHAR(8000),	-- Список выбранных кодов 
	@AllUsers BIT,	-- Признак выбора всех клиентов
	@UserId INT	-- Код пользователя
)
AS
-- Таблица, в которой хранятся коды выбранных пользователей с учетом прав
CREATE TABLE #AvailableUserListTable(
	Id INT not null,
	Name VARCHAR(100) not null
)
CREATE TABLE #SelectedUsers (Id INT not null)	-- список выбранных пользователей
CREATE TABLE #VisibleUsers (	-- список видимых пользователю пользователей
	Id INT not null,
	Name VARCHAR(100) not null
)	
DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр пользователей

SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(5001, @UserId)

IF @permissionDistributionTypeId = 2	-- командное право
	INSERT INTO #VisibleUsers (Id, Name)
	SELECT u.Id, u.DisplayName
	FROM dbo.[UserTeam] ut
		join dbo.[Team] t on t.Id = ut.TeamId
		join dbo.[UserTeam] ut2 on ut2.TeamId = ut.TeamId
		join dbo.[User] u on u.Id = ut.UserId
	WHERE ut2.UserId = @UserId
ELSE IF @permissionDistributionTypeId = 3	-- право "Все"
	INSERT INTO #VisibleUsers
	SELECT Id, DisplayName FROM dbo.[User]
-- Если указаны коды, то
IF @AllUsers = 0
BEGIN
	-- Таблица, в которую помещаются коды после парсинга в строковом виде
	CREATE TABLE #StringIdListTable (Id VARCHAR(50) not null)

	INSERT INTO #StringIdListTable (Id)
	SELECT Id FROM dbo.SplitIntIdList(@IdList)	-- Парсим список кодов
	
	-- Получаем список выбранных кодов
	INSERT INTO #SelectedUsers
	SELECT CONVERT(INT, Id) FROM #StringIdListTable
	-- Выбираем указанные коды из видимых
	INSERT INTO #AvailableUserListTable (Id, Name)
	SELECT vu.Id, vu.Name
	FROM #SelectedUsers su
		join #VisibleUsers vu on su.Id = vu.Id
		
	DROP TABLE #StringIdListTable
END
ELSE
	-- иначе берем все видимые
	INSERT INTO #AvailableUserListTable (Id, Name)
	SELECT Id, Name FROM #VisibleUsers

SELECT Id, Name FROM #AvailableUserListTable

DROP TABLE #AvailableUserListTable
-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'Report0002_GetAvailableSaleIndicatorByUser'
)
   DROP PROCEDURE dbo.Report0002_GetAvailableSaleIndicatorByUser
GO

CREATE PROCEDURE dbo.Report0002_GetAvailableSaleIndicatorByUser
(
	@IsShippedExpenditureWaybills BIT,	-- Признак выбора индикаторов по накладным реализаций в финальном статусе
	@UserId INT	-- Код пользователя
)
AS

-- Таблица, в которой хранятся коды видимых реализаций с учетом прав
CREATE TABLE #AvailableSaleIndicatorIdListTable(Id UNIQUEIDENTIFIER not null)
DECLARE @permissionDistributionTypeId TINYINT	-- разрешение права на просмотр реализаций
SET @permissionDistributionTypeId = dbo.GetPermissionDistributionType(3601, @UserId)

IF @IsShippedExpenditureWaybills = 1
	BEGIN
		IF @permissionDistributionTypeId = 3	-- "Все"
			INSERT INTO #AvailableSaleIndicatorIdListTable
			SELECT ssi.Id 
			FROM ShippedSaleIndicator ssi
		ELSE IF @permissionDistributionTypeId = 2	-- "командные"
			INSERT INTO #AvailableSaleIndicatorIdListTable
			SELECT ssi.Id 
			FROM ShippedSaleIndicator ssi
				join TeamStorage ts on ts.StorageId = ssi.StorageId
				join UserTeam ut on ut.TeamId = ts.TeamId
				join TeamDeal td on td.TeamId = ts.TeamId
			WHERE ut.UserId = @UserId
		 ELSE IF @permissionDistributionTypeId = 1	-- "только свои"
			INSERT INTO #AvailableSaleIndicatorIdListTable
			SELECT ssi.Id 
			FROM dbo.ShippedSaleIndicator ssi
				join TeamStorage ts on ts.StorageId = ssi.StorageId
				join UserTeam ut on ut.TeamId = ts.TeamId
				join TeamDeal td on td.TeamId = ts.TeamId			
			WHERE ut.UserId = @UserId AND ssi.UserId = @UserId
	END
ELSE
	BEGIN
		IF @permissionDistributionTypeId = 3	-- "Все"
			INSERT INTO #AvailableSaleIndicatorIdListTable
			SELECT asi.Id 
			FROM AcceptedSaleIndicator asi
		ELSE IF @permissionDistributionTypeId = 2	-- "командные"
			INSERT INTO #AvailableSaleIndicatorIdListTable
			SELECT asi.Id 
			FROM dbo.AcceptedSaleIndicator asi
				join TeamStorage ts on ts.StorageId = asi.StorageId
				join UserTeam ut on ut.TeamId = ts.TeamId
				join TeamDeal td on td.TeamId = ts.TeamId
			WHERE ut.UserId = @UserId
		 ELSE IF @permissionDistributionTypeId = 1	-- "только свои"
			INSERT INTO #AvailableSaleIndicatorIdListTable
			SELECT asi.Id 
			FROM dbo.AcceptedSaleIndicator asi
				join TeamStorage ts on ts.StorageId = asi.StorageId
				join UserTeam ut on ut.TeamId = ts.TeamId
				join TeamDeal td on td.TeamId = ts.TeamId			
			WHERE ut.UserId = @UserId AND asi.UserId = @UserId
	END

SELECT Id FROM #AvailableSaleIndicatorIdListTable

DROP TABLE #AvailableSaleIndicatorIdListTable

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO


IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'Report0002'
)
   DROP PROCEDURE Report0002
GO

CREATE PROCEDURE Report0002
(
	@StartDate DATETIME,	-- Дата начала диапазона
	@EndDate DATETIME,	-- Дата завершения диапазона
	@IsClosed BIT,	-- Признак выборки накладных в финальном статусе(иначе проведенные)
	@DevideByBatch BIT, -- Признак разделения партий
	@GetArticleAvailability BIT, -- Признак необходимости подсчитать наличие на МХ
	
	@ConsiderReturnFromClient BIT,	-- Признак, нужно ли учитывать возвраты
	@ConsiderReturnFromClientByDate BIT, -- Признак того, что нужно учитывать возвраты из указанного интервала дат. Иначе из указанных возвратов.

	@StorageIdList VARCHAR(8000),	-- Список кодов МХ
	@AllStorages BIT,	-- Признак выбора всех МХ
	@ArticleGroupIdList VARCHAR(8000),	--  Список кодов групп товаров
	@AllArticleGroups BIT,	-- Признак выбора всех групп товаров
	@ClientIdList VARCHAR(8000),	--  Список кодов клиентов
	@AllClients BIT,	-- Признак выбора всех клиентов
	@UserIdList VARCHAR(8000),	--  Список кодов групп пользователей
	@AllUsers BIT,	-- Признак выбора всех пользователей
	@AccountOrganizationIdList VARCHAR(8000),	--  Список кодов организаций аккаунта
	@AllAccountOrganizations BIT,	-- Признак выбора всех организаций аккаунта
	@UserId INT	-- Код пользователя, запросившего отчет
)
AS

SET NOCOUNT ON

-- Итоговая плоская таблица
CREATE TABLE #ResultFlatTable(
	-- Основные данные о позиции реализации
	ArticleId INT not null,
	ArticleNumber VARCHAR(30) not null,
	ArticleName VARCHAR(200) not null,
	BatchId UNIQUEIDENTIFIER not null,
	BatchNumber VARCHAR(25) not null,
	BatchDate DateTime not null,
	Count DECIMAL(18, 6) not null,
	PackSize DECIMAL(12, 6) null,
	CountryName VARCHAR(200) not null,
	CustomsDeclarationNumber VARCHAR(33) not null,

	-- УЦ
	AccountingPriceSum DECIMAL (18, 2) not null,
	-- ЗЦ
	PurchaseCostSum DECIMAL (18, 6) not null,
	--ОЦ
	SalePriceSum DECIMAL (18, 2) not null,
	
	-- Средняя ЗЦ
	AveragePurchaseCost DECIMAL (18, 6) null,
	-- Средняя ОЦ
	AverageSalePrice DECIMAL (18, 2) null,
	
	IsReturn BIT not null,
	-- Дополнительные данные (для группировки)
	ArticleGroupId SMALLINT not null,
	ArticleGroupName VARCHAR(200) not null,
	StorageId TINYINT not null,
	StorageTypeId TINYINT not null,
	StorageName VARCHAR(200) not null,
	AccountOrganizationId INT not null,
	AccountOrganizationName VARCHAR(100) not null,
	TeamId SMALLINT not null,
	TeamName VARCHAR(200) not null,
	UserId INT not null,
	UserName VARCHAR(100) not null,
	ClientId INT not null,
	ClientName VARCHAR(200) not null,
	ClientOrganizationId INT not null,
	ClientOrganizationName VARCHAR(100) not null,
	ProducerId INT not null,	-- Производитель/Поставщик
	ProducerName VARCHAR(200) not null,	-- Производитель/Поставщик
	ArticleAvailabilityCount DECIMAL(18,6) not null, -- остаток товара
	ArticleAvailabilityAccountingPrice DECIMAL (18, 2) null	-- УЦ остатков
)

CREATE INDEX IX_#ResultFlatTable ON #ResultFlatTable (
	ArticleId,
	BatchId,
	-- Дополнительные данные (для группировки)
	ArticleGroupId,
	StorageId,
	AccountOrganizationId,
	TeamId,
	UserId,
	ClientId,
	ClientOrganizationId,
	ProducerId
)

-- Таблица, в которой хранятся коды выбранных МХ с учетом прав
CREATE TABLE #AvailableStorageListTable(
	Id TINYINT not null,
	Name VARCHAR(200) not null,
	StorageTypeId TINYINT not null
)
-- Заполняем таблицы "параметрами" отчета
INSERT INTO #AvailableStorageListTable(Id, Name, StorageTypeId) 
EXEC GetAvailableStorages @StorageIdList, @AllStorages, @UserId

-- Таблица, в которой хранятся коды выбранных групп товаров с учетом прав
CREATE TABLE #AvailableArticleGroupListTable(
	Id TINYINT not null,
	Name VARCHAR(200) not null
)
INSERT INTO #AvailableArticleGroupListTable (Id, Name)
EXEC GetAvailableArticleGroups @ArticleGroupIdList, @AllArticleGroups

-- Таблица, в которой хранятся коды выбранных клиентов с учетом прав
CREATE TABLE #AvailableClientListTable(
	Id INT not null,
	Name VARCHAR(200) not null
)
INSERT INTO #AvailableClientListTable (Id, Name) 
EXEC GetAvailableClients @ClientIdList, @AllClients, @UserId

-- Таблица, в которой хранятся коды выбранных пользователей с учетом прав
CREATE TABLE #AvailableUserListTable(
	Id INT not null,
	Name VARCHAR(100) not null
)
INSERT INTO #AvailableUserListTable (Id, Name)  
EXEC GetAvailableUsers @UserIdList, @AllUsers, @UserId

-- Таблица, в которой хранятся коды выбранных  организаций аккаунта с учетом прав
CREATE TABLE #AvailableAccountOrganizationListTable(
	Id INT not null,
	Name VARCHAR(100) not null
)
INSERT INTO #AvailableAccountOrganizationListTable (Id, Name)  
EXEC GetAvailableAccountOrganizations @AccountOrganizationIdList, @AllAccountOrganizations

-- Таблица, в которой хранятся коды видимых индикаторов реализаций с учетом прав
CREATE TABLE #AvailableSaleIndicatorIdListTable(Id UNIQUEIDENTIFIER not null)
INSERT INTO #AvailableSaleIndicatorIdListTable (Id)  
EXEC Report0002_GetAvailableSaleIndicatorByUser @IsClosed, @UserId

-- Если необходимо взять только накладные в финальном статусе
IF @IsClosed = 1
BEGIN
	-- Получение индикаторов на конец периода
	INSERT INTO #ResultFlatTable (ArticleId, ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
		AccountingPriceSum, PurchaseCostSum, SalePriceSum,
		ArticleAvailabilityCount, IsReturn,
		-- Дополнительные данные (для группировки)
		ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
		TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
	SELECT
		a.Id,	-- ArticleId
		a.Number,	-- ArticleNumber
		a.FullName,	-- ArticleName 
		ssi.BatchId,	-- BatchId 
		rw.Number, -- BatchNumber
		rw.Date, -- BatchDate
		a.PackSize,	-- PackSize
		SUM(ssi.SoldCount) SoldCount,	-- Count 
		country.Name,	-- CountryName
		rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
		
		SUM(ssi.AccountingPriceSum) AccountingPriceSum,	--AccountingPriceSum
		SUM(ssi.PurchaseCostSum) PurchaseCostSum,	--PurchaseCostSum
		SUM(ssi.SalePriceSum) SalePriceSum,	--SalePriceSum
		
		0,  -- ArticleAvailabilityCount
		0,	-- IsReturn
		-- Дополнительные данные (для группировки)
		_agt.Id,	-- ArticleGroupId 
		_agt.Name,	-- ArticleGroupName
		_st.Id,	-- StorageId 
		_st.Name,	-- StorageName
		_st.StorageTypeId, -- StorageTypeId
		_aot.Id,	-- AccountOrganizationId 
		_aot.Name,	-- AccountOrganizationName
		t.Id,	-- TeamId 
		t.Name,	-- TeamName 
		_ut.Id,	-- UserId 
		_ut.Name,	-- UserName 
		_ct.Id,	-- ClientId 
		_ct.Name,	-- ClientName 
		o.Id,	-- ClientOrganizationId 
		o.ShortName,	-- ClientOrganizationName 
		contractor.Id,	-- ProducerId 
		contractor.Name	-- ProducerName 
	FROM 
		ShippedSaleIndicator ssi
		join ReceiptWaybillRow rwr on rwr.Id = ssi.BatchId
		join Article a on a.Id = ssi.ArticleId		
		join Deal d on d.Id = ssi.DealId
		join Contract c on c.Id = d.ClientContractId
		join Country country on country.Id = rwr.CountryId
		join Team t on t.Id = ssi.TeamId
		join Organization o on o.Id = ssi.ClientOrganizationId
		join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
		join Contractor contractor on contractor.Id = rw.ProviderId

		-- Ограничение по правам на проcмотр
		join #AvailableSaleIndicatorIdListTable _aewl on _aewl.Id = ssi.Id
		-- Ограничения по настройкам отчета
		join #AvailableStorageListTable _st on _st.Id = ssi.StorageId	-- ограничение по МХ
		join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
		join #AvailableClientListTable _ct on _ct.Id = ssi.ClientId	-- ограничение по клиентам
		join #AvailableUserListTable _ut on _ut.Id = ssi.UserId	-- ограничение по пользователям
		join #AvailableAccountOrganizationListTable _aot on _aot.Id = ssi.AccountOrganizationId	-- ограничение по организации аккаунта
	WHERE ssi.StartDate <= @EndDate AND (ssi.EndDate > @EndDate OR ssi.EndDate is null)
	GROUP BY a.Id, a.Number,a.FullName,ssi.BatchId,rw.Number,rw.Date,a.PackSize,country.Name,rwr.CustomsDeclarationNumber,		
		-- Дополнительные данные (для группировки)
		_agt.Id,_agt.Name,_st.Id,_st.Name,_st.StorageTypeId,_aot.Id,_aot.Name,t.Id,t.Name,_ut.Id,_ut.Name,_ct.Id,_ct.Name,o.Id,
		o.ShortName,contractor.Id,contractor.Name
		
	-- Вычитаем индикаторы на начало периода
	UPDATE #ResultFlatTable
	SET Count = #ResultFlatTable.Count - ind.SoldCount,
		AccountingPriceSum = #ResultFlatTable.AccountingPriceSum - ind.AccountingPriceSum,
		PurchaseCostSum = #ResultFlatTable.PurchaseCostSum - ind.PurchaseCostSum,
		SalePriceSum = #ResultFlatTable.SalePriceSum - ind.SalePriceSum
	FROM(
		SELECT ssi.UserId, ssi.StorageId, ssi.TeamId, ssi.BatchId, ssi.ClientId, ssi.ClientOrganizationId, ssi.AccountOrganizationId,
			SUM(ssi.SoldCount) SoldCount, SUM(ssi.AccountingPriceSum) AccountingPriceSum, 
			SUM(ssi.PurchaseCostSum) PurchaseCostSum,SUM(ssi.SalePriceSum) SalePriceSum
		FROM
			ShippedSaleIndicator ssi
			join Article a on a.Id = ssi.ArticleId
			-- Ограничение по правам на проcмотр реализаций
			join #AvailableSaleIndicatorIdListTable _aewl on _aewl.Id = ssi.Id
			-- Ограничения по настройкам отчета
			join #AvailableStorageListTable _st on _st.Id = ssi.StorageId	-- ограничение по МХ
			join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
			join #AvailableClientListTable _ct on _ct.Id = ssi.ClientId	-- ограничение по клиентам
			join #AvailableUserListTable _ut on _ut.Id = ssi.UserId	-- ограничение по пользователям
			join #AvailableAccountOrganizationListTable _aot on _aot.Id = ssi.AccountOrganizationId	-- ограничение по организации аккаунта
		WHERE ssi.StartDate <= @StartDate AND (ssi.EndDate > @StartDate OR ssi.EndDate is null)
		GROUP BY ssi.UserId, ssi.StorageId, ssi.TeamId, ssi.BatchId, ssi.ClientId, ssi.ClientOrganizationId, ssi.AccountOrganizationId) ind
	WHERE ind.UserId = #ResultFlatTable.UserId AND ind.StorageId = #ResultFlatTable.StorageId AND
		ind.TeamId = #ResultFlatTable.TeamId AND ind.BatchId = #ResultFlatTable.BatchId	AND ind.ClientId = #ResultFlatTable.ClientId AND
		ind.ClientOrganizationId = 	#ResultFlatTable.ClientOrganizationId AND ind.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId
END
ELSE	-- иначе берем проведенные
BEGIN
	-- Получение всех необходимых позиций реализаций
	INSERT INTO #ResultFlatTable (ArticleId, ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
		AccountingPriceSum, PurchaseCostSum, SalePriceSum,
		ArticleAvailabilityCount, IsReturn,
		-- Дополнительные данные (для группировки)
		ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
		TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
	SELECT
		a.Id,	-- ArticleId
		a.Number,	-- ArticleNumber
		a.FullName,	-- ArticleName 
		asi.BatchId,	-- BatchId 
		rw.Number, -- BatchNumber
		rw.Date, -- BatchDate
		a.PackSize,	-- PackSize
		SUM(asi.SoldCount),	-- Count 
		country.Name,	-- CountryName
		rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
		
		SUM(asi.AccountingPriceSum),	--AccountingPriceSum
		SUM(asi.PurchaseCostSum),	--PurchaseCostSum
		SUM(asi.SalePriceSum),	--SalePriceSum
		
		0,  -- ArticleAvailabilityCount
		0,	-- IsReturn
		-- Дополнительные данные (для группировки)
		_agt.Id,	-- ArticleGroupId 
		_agt.Name,	-- ArticleGroupName
		_st.Id,	-- StorageId 
		_st.Name,	-- StorageName
		_st.StorageTypeId, -- StorageTypeId
		_aot.Id,	-- AccountOrganizationId 
		_aot.Name,	-- AccountOrganizationName
		t.Id,	-- TeamId 
		t.Name,	-- TeamName 
		_ut.Id,	-- UserId 
		_ut.Name,	-- UserName 
		_ct.Id,	-- ClientId 
		_ct.Name,	-- ClientName 
		o.Id,	-- ClientOrganizationId 
		o.ShortName,	-- ClientOrganizationName 
		contractor.Id,	-- ProducerId 
		contractor.Name	-- ProducerName 
	FROM 
		AcceptedSaleIndicator asi
		join ReceiptWaybillRow rwr on rwr.Id = asi.BatchId
		join Article a on a.Id = asi.ArticleId		
		join Deal d on d.Id = asi.DealId
		join Contract c on c.Id = d.ClientContractId
		join Country country on country.Id = rwr.CountryId
		join Team t on t.Id = asi.TeamId
		join Organization o on o.Id = asi.ClientOrganizationId
		join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
		join Contractor contractor on contractor.Id = rw.ProviderId

		-- Ограничение по правам на проcмотр реализаций
		join #AvailableSaleIndicatorIdListTable _aewl on _aewl.Id = asi.Id
		-- Ограничения по настройкам отчета
		join #AvailableStorageListTable _st on _st.Id = asi.StorageId	-- ограничение по МХ
		join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
		join #AvailableClientListTable _ct on _ct.Id = asi.ClientId	-- ограничение по клиентам
		join #AvailableUserListTable _ut on _ut.Id = asi.UserId	-- ограничение по пользователям
		join #AvailableAccountOrganizationListTable _aot on _aot.Id = asi.AccountOrganizationId	-- ограничение по организации аккаунта
	WHERE asi.StartDate <= @EndDate AND (asi.EndDate > @EndDate OR asi.EndDate is null)
	GROUP BY a.Id, a.Number,a.FullName,asi.BatchId,rw.Number,rw.Date,a.PackSize,country.Name,rwr.CustomsDeclarationNumber,		
		-- Дополнительные данные (для группировки)
		_agt.Id,_agt.Name,_st.Id,_st.Name,_st.StorageTypeId,_aot.Id,_aot.Name,t.Id,t.Name,_ut.Id,_ut.Name,_ct.Id,_ct.Name,o.Id,
		o.ShortName,contractor.Id,contractor.Name
		
	-- Вычитаем индикаторы на начало периода
	UPDATE #ResultFlatTable
	SET Count = #ResultFlatTable.Count - ind.SoldCount,
		AccountingPriceSum = #ResultFlatTable.AccountingPriceSum - ind.AccountingPriceSum,
		PurchaseCostSum = #ResultFlatTable.PurchaseCostSum - ind.PurchaseCostSum,
		SalePriceSum = #ResultFlatTable.SalePriceSum - ind.SalePriceSum
	FROM(
		SELECT asi.UserId, asi.StorageId, asi.TeamId, asi.BatchId, asi.ClientId, asi.ClientOrganizationId, asi.AccountOrganizationId,
			SUM(asi.SoldCount) SoldCount, SUM(asi.AccountingPriceSum) AccountingPriceSum, 
			SUM(asi.PurchaseCostSum) PurchaseCostSum,SUM(asi.SalePriceSum) SalePriceSum
		FROM
			AcceptedSaleIndicator asi
			join Article a on a.Id = asi.ArticleId
			-- Ограничение по правам на проcмотр реализаций
			join #AvailableSaleIndicatorIdListTable _aewl on _aewl.Id = asi.Id
			-- Ограничения по настройкам отчета
			join #AvailableStorageListTable _st on _st.Id = asi.StorageId	-- ограничение по МХ
			join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
			join #AvailableClientListTable _ct on _ct.Id = asi.ClientId	-- ограничение по клиентам
			join #AvailableUserListTable _ut on _ut.Id = asi.UserId	-- ограничение по пользователям
			join #AvailableAccountOrganizationListTable _aot on _aot.Id = asi.AccountOrganizationId	-- ограничение по организации аккаунта
		WHERE asi.StartDate <= @StartDate AND (asi.EndDate > @StartDate OR asi.EndDate is null)
		GROUP BY asi.UserId, asi.StorageId, asi.TeamId, asi.BatchId, asi.ClientId, asi.ClientOrganizationId, asi.AccountOrganizationId) ind
	WHERE ind.UserId = #ResultFlatTable.UserId AND ind.StorageId = #ResultFlatTable.StorageId AND
		ind.TeamId = #ResultFlatTable.TeamId AND ind.BatchId = #ResultFlatTable.BatchId	AND ind.ClientId = #ResultFlatTable.ClientId AND
		ind.ClientOrganizationId = 	#ResultFlatTable.ClientOrganizationId AND ind.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId
END

-- Удаляем нулевые строки
DELETE FROM #ResultFlatTable WHERE Count = 0
	
IF @ConsiderReturnFromClient = 1	-- Если нужно учесть возвраты
BEGIN
	IF @ConsiderReturnFromClientByDate = 1	-- Учитывать по дате?
	BEGIN

		-- Возвраты на конец периода
		SELECT rfci.* INTO #tmp_GetReturnFromClientWaybillRowsByDate
		FROM AcceptedReturnFromClientIndicator rfci
			-- Ограничения по настройкам отчета
			join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
			join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
			join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
			join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
			join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
			join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта

		-- Вычитаем возвраты на начало периода		
		UPDATE #tmp_GetReturnFromClientWaybillRowsByDate
		SET 
			ReturnedCount = #tmp_GetReturnFromClientWaybillRowsByDate.ReturnedCount - p.ReturnedCount,
			AccountingPriceSum = #tmp_GetReturnFromClientWaybillRowsByDate.AccountingPriceSum - p.AccountingPriceSum,
			PurchaseCostSum = #tmp_GetReturnFromClientWaybillRowsByDate.PurchaseCostSum - p.PurchaseCostSum,
			SalePriceSum = #tmp_GetReturnFromClientWaybillRowsByDate.SalePriceSum - p.SalePriceSum
		FROM (
			SELECT rfci.*
			FROM AcceptedReturnFromClientIndicator rfci
				-- Ограничения по настройкам отчета
				join Article a on a.Id = rfci.ArticleId
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта
			WHERE rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null) ) p
		WHERE
			#tmp_GetReturnFromClientWaybillRowsByDate.SaleWaybillId = p.SaleWaybillId AND 
			#tmp_GetReturnFromClientWaybillRowsByDate.BatchId = p.BatchId AND 
			#tmp_GetReturnFromClientWaybillRowsByDate.ReturnFromClientWaybillCuratorId = p.ReturnFromClientWaybillCuratorId
			
		-- Удаляем нулевые строки
		DELETE FROM #tmp_GetReturnFromClientWaybillRowsByDate WHERE ReturnedCount = 0

		-- Получение всех необходимых позиций возвратов
		INSERT INTO #ResultFlatTable (ArticleId, ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
			AccountingPriceSum, PurchaseCostSum, SalePriceSum,
			ArticleAvailabilityCount, IsReturn,
			-- Дополнительные данные (для группировки)
			ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
			TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
		SELECT
			a.Id,	-- ArticleId
			a.Number,	-- ArticleNumber
			a.FullName,	-- ArticleName 
			rfci.BatchId,	-- BatchId 
			rw.Number, --BatchNumber
			rw.Date, --BatchDate
			a.PackSize,	-- PackSize
			rfci.ReturnedCount,	-- Count 
			country.Name,	-- CountryName 
			rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
			
			rfci.AccountingPriceSum,	--AccountingPriceSum
			rfci.PurchaseCostSum,	--PurchaseCostSum
			rfci.SalePriceSum,	--SalePriceSum
			
			0,  -- ArticleAvailabilityCount
			1,	-- IsReturn
			-- Дополнительные данные (для группировки)
			_agt.Id,	-- ArticleGroupId 
			_agt.Name,	-- ArticleGroupName
			_st.Id,	-- StorageId 
			_st.Name,	-- StorageName
			_st.StorageTypeId, -- StorageTypeId
			_aot.Id,	-- AccountOrganizationId 
			_aot.Name,	-- AccountOrganizationName
			t.Id,	-- TeamId 
			t.Name,	-- TeamName 
			_ut.Id,	-- UserId 
			_ut.Name,	-- UserName 
			_ct.Id,	-- ClientId 
			_ct.Name,	-- ClientName 
			o.Id,	-- ClientOrganizationId 
			o.ShortName,	-- ClientOrganizationName 
			contractor.Id,	-- ProducerId 
			contractor.Name	-- ProducerName 
		FROM #tmp_GetReturnFromClientWaybillRowsByDate rfci
			join Article a on a.Id = rfci.ArticleId
			join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
			join Country country on country.Id = rwr.CountryId
			join Team t on t.Id = rfci.TeamId
			join Deal d on d.Id = rfci.DealId
			join Contract c on c.Id = d.ClientContractId
			join Organization o on o.Id = c.ContractorOrganizationId
			join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
			join Contractor contractor on contractor.Id = rw.ProviderId
			-- Ограничения по настройкам отчета
			join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
			join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
			join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
			join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
			join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта

		drop table #tmp_GetReturnFromClientWaybillRowsByDate
		
	END
	ELSE	-- Иначе из тех же реализаций
	BEGIN

		IF 	@IsClosed = 1	-- Если нужно взять только накладные в финальном статусе	
		BEGIN
			-- Возвраты на конец периода
			SELECT rfci.* INTO #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills
			FROM ReturnFromClientBySaleShippingDateIndicator rfci
				-- Ограничения по настройкам отчета
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта

			-- Вычитаем возвраты на начало периода		
			UPDATE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills
			SET 
				ReturnedCount = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.ReturnedCount - p.ReturnedCount,
				AccountingPriceSum = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.AccountingPriceSum - p.AccountingPriceSum
			FROM (
				SELECT rfci.*
				FROM ReturnFromClientBySaleShippingDateIndicator rfci
					-- Ограничения по настройкам отчета
					join Article a on a.Id = rfci.ArticleId
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
					join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
					join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
					join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
					join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта
				WHERE rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null)) p
			WHERE
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.SaleWaybillId = p.SaleWaybillId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.BatchId = p.BatchId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills.ReturnFromClientWaybillCuratorId = p.ReturnFromClientWaybillCuratorId
				
			-- Удаляем нулевые строки
			DELETE FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills WHERE ReturnedCount <= 0
			
			-- Получение всех необходимых позиций возвратов по дате отгрузки реализации
			INSERT INTO #ResultFlatTable (ArticleId,ArticleNumber, ArticleName, BatchId, BatchNumber, BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
				AccountingPriceSum, PurchaseCostSum, SalePriceSum,
				ArticleAvailabilityCount, IsReturn,
				-- Дополнительные данные (для группировки)
				ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId,  AccountOrganizationId, AccountOrganizationName, TeamId, 
				TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName)
			SELECT
				a.Id,	-- ArticleId
				a.Number,	-- ArticleNumber
				a.FullName,	-- ArticleName 
				rfci.BatchId,	-- BatchId 
				rw.Number, -- BatchNumber
				rw.Date, -- BatchDate
				a.PackSize,	-- PackSize
				rfci.ReturnedCount,	-- Count 
				country.Name,	-- CountryName 
				rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
				
				rfci.AccountingPriceSum,	--AccountingPriceSum
				rfci.PurchaseCostSum,	--PurchaseCostSum
				rfci.SalePriceSum,	--SalePriceSum
				
				0,  -- ArticleAvailabilityCount
				1,	-- IsReturn
				
				-- Дополнительные данные (для группировки)
				_agt.Id,	-- ArticleGroupId 
				_agt.Name,	-- ArticleGroupName
				_st.Id,	-- StorageId 
				_st.Name,	-- StorageName
				_st.StorageTypeId, -- StorageTypeId
				_aot.Id,	-- AccountOrganizationId 
				_aot.Name,	-- AccountOrganizationName
				t.Id,	-- TeamId 
				t.Name,	-- TeamName 
				_ut.Id,	-- UserId 
				_ut.Name,	-- UserName 
				_ct.Id,	-- ClientId 
				_ct.Name,	-- ClientName 
				o.Id,	-- ClientOrganizationId 
				o.ShortName,	-- ClientOrganizationName 
				contractor.Id,	-- ProducerId 
				contractor.Name	-- ProducerName 
			FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills rfci
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate >= @StartDate AND rfci.StartDate <= @EndDate
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Country country on country.Id = rwr.CountryId		
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				-- Ограничения по настройкам отчета
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта
				
			DROP TABLE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills
			
		END
		ELSE
		BEGIN
			-- Возвраты на конец периода
			SELECT rfci.* INTO #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2
			FROM ReturnFromClientBySaleAcceptanceDateIndicator rfci
				-- Ограничения по настройкам отчета
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate <= @EndDate AND (rfci.EndDate > @EndDate OR rfci.EndDate is null)
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта

			-- Вычитаем возвраты на начало периода		
			UPDATE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2
			SET 
				ReturnedCount = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.ReturnedCount - p.ReturnedCount,
				AccountingPriceSum = #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.AccountingPriceSum - p.AccountingPriceSum
			FROM (
				SELECT rfci.*
				FROM ReturnFromClientBySaleAcceptanceDateIndicator rfci
					-- Ограничения по настройкам отчета
					join Article a on a.Id = rfci.ArticleId
					join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
					join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
					join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
					join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
					join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта
				WHERE rfci.StartDate <= @StartDate AND (rfci.EndDate > @StartDate OR rfci.EndDate is null) ) p
			WHERE
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.SaleWaybillId = p.SaleWaybillId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.BatchId = p.BatchId AND 
				#tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2.ReturnFromClientWaybillCuratorId = p.ReturnFromClientWaybillCuratorId
				
			-- Удаляем нулевые строки
			DELETE FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2 WHERE ReturnedCount <= 0
			
			-- Получение всех необходимых позиций возвратов по дате проводки реализации
			INSERT INTO #ResultFlatTable (ArticleId,ArticleNumber, ArticleName, BatchId, BatchNumber,BatchDate, PackSize, Count, CountryName, CustomsDeclarationNumber, 
				AccountingPriceSum, PurchaseCostSum, SalePriceSum,
				ArticleAvailabilityCount, IsReturn,
				-- Дополнительные данные (для группировки)
				ArticleGroupId, ArticleGroupName, StorageId, StorageName, StorageTypeId, AccountOrganizationId, AccountOrganizationName, TeamId, 
				TeamName, UserId, UserName, ClientId, ClientName, ClientOrganizationId, ClientOrganizationName, ProducerId, ProducerName )
			SELECT
				a.Id,	-- ArticleId
				a.Number,	-- ArticleNumber
				a.FullName,	-- ArticleName 
				rfci.BatchId,	-- BatchId 
				rw.Number, -- BatchNumber
				rw.Date, -- BatchDate
				a.PackSize,	-- PackSize
				rfci.ReturnedCount,	-- Count 
				rwr.CountryId,	-- CountryName 
				rwr.CustomsDeclarationNumber,	-- CustomsDeclarationNumber 
				
				rfci.AccountingPriceSum,	--AccountingPriceSum
				rfci.PurchaseCostSum,	--PurchaseCostSum
				rfci.SalePriceSum,	--SalePriceSum
				
				0,  --ArticleAvailabilityCount
				1,	-- IsReturn
				
				-- Дополнительные данные (для группировки)
				_agt.Id,	-- ArticleGroupId 
				_agt.Name,	-- ArticleGroupName
				_st.Id,	-- StorageId 
				_st.Name,	-- StorageName
				_st.StorageTypeId, -- StorageTypeId
				_aot.Id,	-- AccountOrganizationId 
				_aot.Name,	-- AccountOrganizationName
				t.Id,	-- TeamId 
				t.Name,	-- TeamName 
				_ut.Id,	-- UserId 
				_ut.Name,	-- UserName 
				_ct.Id,	-- ClientId 
				_ct.Name,	-- ClientName 
				o.Id,	-- ClientOrganizationId 
				o.ShortName,	-- ClientOrganizationName 
				contractor.Id,	-- ProducerId 
				contractor.Name	-- ProducerName 
			FROM #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2 rfci
				join Article a on a.Id = rfci.ArticleId AND rfci.StartDate >= @StartDate AND rfci.StartDate <= @EndDate
				join ReceiptWaybillRow rwr on rwr.Id = rfci.BatchId
				join Team t on t.Id = rfci.TeamId
				join Deal d on d.Id = rfci.DealId
				join Contract c on c.Id = d.ClientContractId
				join Organization o on o.Id = c.ContractorOrganizationId
				join ReceiptWaybill rw on rw.Id = rwr.ReceiptWaybillId
				join Contractor contractor on contractor.Id = rw.ProviderId
				-- Ограничения по настройкам отчета
				join #AvailableStorageListTable _st on _st.Id = rfci.StorageId -- ограничение по МХ
				join #AvailableArticleGroupListTable _agt on _agt.Id = a.ArticleGroupId	-- ограничение по группам товаров
				join #AvailableClientListTable _ct on _ct.Id = rfci.ClientId	-- ограничение по клиентам
				join #AvailableUserListTable _ut on _ut.Id = rfci.SaleWaybillCuratorId	-- ограничение по пользователям
				join #AvailableAccountOrganizationListTable _aot on _aot.Id = rfci.AccountOrganizationId	-- ограничение по организации аккаунта
				
			DROP TABLE #tmp_GetReturnFromClientWaybillRowsByExpenditureWaybills2
			
		END
	END
END

-- Если нет разделения по партиям, то сливаем их
IF @DevideByBatch = 0
BEGIN
	-- Итоговая плоская таблица
	CREATE TABLE #tmp_ResultFlatTable(
		-- Основные данные о позиции реализации
		ArticleId INT  null,
		ArticleNumber VARCHAR(30)  null,
		ArticleName VARCHAR(200)  null,
		BatchId UNIQUEIDENTIFIER  null,
		BatchNumber VARCHAR(25)  null,
		BatchDate DateTime  null,
		Count DECIMAL(18, 6)  null,
		PackSize DECIMAL(12, 6) null,
		CountryName VARCHAR(200)  null,
		CustomsDeclarationNumber VARCHAR(33)  null,

		-- УЦ
		AccountingPriceSum DECIMAL (18, 2) null,
		-- ЗЦ
		PurchaseCostSum DECIMAL (18, 6) null,
		--ОЦ
		SalePriceSum DECIMAL (18, 2) null,
		
		-- Средняя ЗЦ
		AveragePurchaseCost DECIMAL (18, 6) null,
		-- Средняя ОЦ
		AverageSalePrice DECIMAL (18, 2) null,
		
		IsReturn BIT  null,
		-- Дополнительные данные (для группировки)
		ArticleGroupId SMALLINT  null,
		ArticleGroupName VARCHAR(200)  null,
		StorageId TINYINT  null,
		StorageTypeId TINYINT  null,
		StorageName VARCHAR(200)  null,
		AccountOrganizationId INT  null,
		AccountOrganizationName VARCHAR(100)  null,
		TeamId SMALLINT  null,
		TeamName VARCHAR(200)  null,
		UserId INT  null,
		UserName VARCHAR(100)  null,
		ClientId INT  null,
		ClientName VARCHAR(200)  null,
		ClientOrganizationId INT  null,
		ClientOrganizationName VARCHAR(100)  null,
		ProducerId INT  null,	-- Производитель/Поставщик
		ProducerName VARCHAR(200)  null,	-- Производитель/Поставщик
		ArticleAvailabilityCount DECIMAL(18,6)  null, -- остаток товара
		ArticleAvailabilityAccountingPrice DECIMAL (18, 2) null	-- УЦ остатков
	)

	-- Втавляем агрегированную строку по партии во временную таблицу
	INSERT INTO #tmp_ResultFlatTable (ArticleId, Count, AccountingPriceSum, PurchaseCostSum, SalePriceSum,
		IsReturn,
		-- Дополнительные данные (для группировки)
		ArticleGroupId,  StorageId,  StorageTypeId,  AccountOrganizationId,  TeamId, 
		UserId, ClientId,  ClientOrganizationId,  ProducerId)
	SELECT
		rft.ArticleId, 
		SUM(rft.Count), 
		SUM(rft.AccountingPriceSum), 
		SUM(rft.PurchaseCostSum), 
		SUM(rft.SalePriceSum),
		rft.IsReturn,
		-- Дополнительные данные (для группировки)
		rft.ArticleGroupId, 
		rft.StorageId, 
		rft.StorageTypeId,  
		rft.AccountOrganizationId, 
		rft.TeamId, 
		rft.UserId, 
		rft.ClientId, 
		rft.ClientOrganizationId, 
		rft.ProducerId
	FROM #ResultFlatTable rft
	GROUP BY ArticleId, ArticleGroupId, StorageId, StorageTypeId, AccountOrganizationId, TeamId, UserId, ClientId, ClientOrganizationId, ProducerId, IsReturn

	UPDATE #tmp_ResultFlatTable
	SET
		ArticleNumber = tmp. ArticleNumber,
		ArticleName = tmp. ArticleName,
		BatchId = tmp.BatchId,
		BatchNumber = tmp. BatchNumber,
		BatchDate = tmp. BatchDate,
		PackSize = tmp. PackSize,
		CountryName = tmp. CountryName,
		CustomsDeclarationNumber = tmp. CustomsDeclarationNumber,
		ArticleAvailabilityCount = tmp. ArticleAvailabilityCount,
		ArticleAvailabilityAccountingPrice = tmp.ArticleAvailabilityAccountingPrice,
		ArticleGroupName = tmp. ArticleGroupName,
		StorageName = tmp. StorageName,
		AccountOrganizationName = tmp. AccountOrganizationName,
		TeamName = tmp. TeamName,
		UserName = tmp. UserName,
		ClientName = tmp.ClientName ,
		ClientOrganizationName = tmp. ClientOrganizationName,
		ProducerName  = tmp.ProducerName
	FROM (
		SELECT *
		FROM #ResultFlatTable) tmp
	WHERE
		tmp.ArticleId = #tmp_ResultFlatTable.ArticleId AND tmp.ArticleGroupId = #tmp_ResultFlatTable.ArticleGroupId AND tmp.StorageId = #tmp_ResultFlatTable.StorageId AND 
		tmp.AccountOrganizationId = #tmp_ResultFlatTable.AccountOrganizationId AND tmp.TeamId = #tmp_ResultFlatTable.TeamId AND tmp.UserId = #tmp_ResultFlatTable.UserId AND 
		tmp.ClientId = #tmp_ResultFlatTable.ClientId AND tmp.ClientOrganizationId = #tmp_ResultFlatTable.ClientOrganizationId AND tmp.ProducerId = #tmp_ResultFlatTable.ProducerId
		AND #tmp_ResultFlatTable.IsReturn = tmp.IsReturn
		
	-- Переливаем данные в исходную таблицу
	DELETE FROM #ResultFlatTable
	INSERT INTO #ResultFlatTable SELECT * FROM #tmp_ResultFlatTable
			
	-- Удаляем промежуточные таблицы
	DROP TABLE #tmp_ResultFlatTable
END	

-- Если нужно получить наличие, то ...
IF @GetArticleAvailability = 1
BEGIN
	-- Получаем остатки товаров
	-- Нужно ли разделять партии?
	IF @DevideByBatch = 0	
	BEGIN
		-- Нет, не учитываем партии товаров при получении остатков
		CREATE TABLE #tmp(
			ArticleId INT not null,
			StorageId TINYINT not null,
			AccountOrganizationId INT not null,
			Count DECIMAL(18,6) not null -- остаток товара
		)
		-- Получаем остатки товара без учета пратий
		INSERT INTO #tmp (ArticleId, StorageId , AccountOrganizationId, Count)
		SELECT ArticleId, StorageId, AccountOrganizationId, SUM(Count)
		FROM ExactArticleAvailabilityIndicator
		WHERE StartDate <= @EndDate AND (EndDate > @EndDate OR EndDate is null)
		GROUP BY ArticleId, StorageId, AccountOrganizationId
		-- Сохраняем остатки в выходной таблице
		UPDATE #ResultFlatTable 
		SET ArticleAvailabilityCount = eaa.Count
		FROM #tmp eaa
		WHERE
			eaa.StorageId = #ResultFlatTable.StorageId AND eaa.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId AND
			eaa.ArticleId = #ResultFlatTable.ArticleId
		
		DROP TABLE #tmp
	END
	ELSE
		-- Выставляем остатки с учетом партий
		UPDATE #ResultFlatTable 
		SET ArticleAvailabilityCount = eaa.Count
		FROM (
			SELECT *
			FROM ExactArticleAvailabilityIndicator
			WHERE
				StartDate <= @EndDate AND (EndDate > @EndDate OR EndDate is null)) eaa
		WHERE
			eaa.StorageId = #ResultFlatTable.StorageId AND eaa.AccountOrganizationId = #ResultFlatTable.AccountOrganizationId AND
			eaa.ArticleId = #ResultFlatTable.ArticleId AND eaa.BatchId = #ResultFlatTable.BatchId
	
	-- таблица входных данных для получения УЦ
	CREATE TABLE #ArticleAccountingPriceForActiclesOnStoragesDetermination(
		ArticleId INT not null,
		StorageId SMALLINT not null,
		AccountingPrice DECIMAL(18,2) null
	)
	-- заполняет таблицу товарами и МХ, для которых нужно получить УЦ
	INSERT INTO #ArticleAccountingPriceForActiclesOnStoragesDetermination(ArticleId, StorageId, AccountingPrice)
	SELECT rft.ArticleId, rft.StorageId, -1
	FROM #ResultFlatTable rft
	GROUP BY rft.ArticleId, rft.StorageId
	
	-- Получаем УЦ
	EXEC GetAccountingPriceForArticlesOnStoragesByDate @EndDate
	
	-- Выставляем УЦ
	UPDATE #ResultFlatTable
	SET ArticleAvailabilityAccountingPrice = t.AccountingPrice
	FROM #ArticleAccountingPriceForActiclesOnStoragesDetermination t
	WHERE t.ArticleId = #ResultFlatTable.ArticleId AND t.StorageId = #ResultFlatTable.StorageId
		
	DROP TABLE #ArticleAccountingPriceForActiclesOnStoragesDetermination
END

-- возвращаем сформированную плоскую таблицу данных
SELECT * FROM #ResultFlatTable 

-- Удаляем временные таблицы
DROP INDEX IX_#ResultFlatTable on #ResultFlatTable
DROP TABLE #AvailableStorageListTable
DROP TABLE #AvailableArticleGroupListTable
DROP TABLE #AvailableClientListTable
DROP TABLE #AvailableUserListTable
DROP TABLE #AvailableAccountOrganizationListTable
DROP TABLE #AvailableSaleIndicatorIdListTable
DROP TABLE #ResultFlatTable

-- ПОСЛЕ КАЖДОЙ ОПЕРАЦИИ
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0 BEGIN PRINT 'Шаг окончен неуспешно.' ROLLBACK TRAN END
GO
IF @@TRANCOUNT = 0 BEGIN PRINT 'Дальше выполнять ничего не будем' SET NOEXEC ON END
GO

-- В САМОМ КОНЦЕ
IF @@TRANCOUNT > 0 BEGIN COMMIT TRAN PRINT 'Обновление выполнено успешно' END
GO
