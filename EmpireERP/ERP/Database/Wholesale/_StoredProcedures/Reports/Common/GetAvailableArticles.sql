/*********************************************************************************************
Процедура: 	GetAvailableArticles

Описание:	Получение списка кодов товаров с учетом прав пользователя

Параметры:
	@TakeArticlesFromArticleGroup Признак того, откуда брать коды товары. Если 1 , то из параметров @ArticleGroupIdList или @AllArticleGroups.Иначе из @ArticleIdList.
	@ArticleGroupIdList Список кодов групп товаров
	@AllArticleGroups Признак выбора всех групп товаров
	@ArticleIdList Список кодов товаров
	
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAvailableArticles'
)
   DROP PROCEDURE dbo.GetAvailableArticles
GO

CREATE PROCEDURE dbo.GetAvailableArticles
(
	@TakeArticlesFromArticleGroup BIT, -- Признак того, откуда брать коды товары. Если 1 , то из параметров @ArticleGroupIdList или @AllArticleGroups.Иначе из @ArticleIdList.
	@ArticleGroupIdList VARCHAR(8000),-- Список кодов групп товаров
	@AllArticleGroups BIT,--- Признак выбора всех групп товаров
	@ArticleIdList  VARCHAR(8000)--- Список кодов товаров
)
AS
	
-- Таблица, в которой хранятся коды выбранных  товаров с учетом прав
CREATE TABLE #AvailableArticlesListTable (
	Id INT 
)

-- Таблица, в которую помещаются коды после парсинга в строковом виде
CREATE TABLE #StringIdListTable (Id int primary key)
			
IF @TakeArticlesFromArticleGroup = 1
BEGIN

	IF @AllArticleGroups = 0 
	BEGIN
		
		INSERT INTO #StringIdListTable (Id)
		SELECT Id 
		FROM dbo.SplitIntIdList(@ArticleGroupIdList)	-- Парсим список кодов
		
		-- Вставляем коды
		INSERT INTO #AvailableArticlesListTable (Id)
		SELECT a.Id
		FROM Article a 
		JOIN #StringIdListTable s ON s.Id = a.ArticleGroupId
		
	END
	ELSE
		INSERT INTO #AvailableArticlesListTable (Id)
		SELECT Id
		FROM dbo.[Article]

END
ELSE
BEGIN
	INSERT INTO #AvailableArticlesListTable (Id)
	SELECT Id 
	FROM dbo.SplitIntIdList(@ArticleIdList)
END

SELECT DISTINCT Id
FROM #AvailableArticlesListTable

DROP TABLE #StringIdListTable
DROP TABLE #AvailableArticlesListTable

GO
