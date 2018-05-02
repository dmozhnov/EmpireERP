/*********************************************************************************************
Процедура: 	GetAvailableArticleGroups

Описание:	Получение списка кодов групп товаров с учетом прав пользователя

Параметры:
	@IdList	Строка с перечислением кодов групп товаров
	@AllArticleGroups Признак выбора всех групп товаров
	
*********************************************************************************************/

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

IF @AllArticleGroups = 0 
	SELECT ag.Id, ag.Name
	FROM dbo.SplitIntIdList(@IdList) sil
	JOIN ArticleGroup ag on ag.Id = sil.Id 
ELSE
	SELECT Id, Name 
	FROM ArticleGroup
GO
