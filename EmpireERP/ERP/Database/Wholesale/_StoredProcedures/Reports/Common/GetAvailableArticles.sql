/*********************************************************************************************
���������: 	GetAvailableArticles

��������:	��������� ������ ����� ������� � ������ ���� ������������

���������:
	@TakeArticlesFromArticleGroup ������� ����, ������ ����� ���� ������. ���� 1 , �� �� ���������� @ArticleGroupIdList ��� @AllArticleGroups.����� �� @ArticleIdList.
	@ArticleGroupIdList ������ ����� ����� �������
	@AllArticleGroups ������� ������ ���� ����� �������
	@ArticleIdList ������ ����� �������
	
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
	@TakeArticlesFromArticleGroup BIT, -- ������� ����, ������ ����� ���� ������. ���� 1 , �� �� ���������� @ArticleGroupIdList ��� @AllArticleGroups.����� �� @ArticleIdList.
	@ArticleGroupIdList VARCHAR(8000),-- ������ ����� ����� �������
	@AllArticleGroups BIT,--- ������� ������ ���� ����� �������
	@ArticleIdList  VARCHAR(8000)--- ������ ����� �������
)
AS
	
-- �������, � ������� �������� ���� ���������  ������� � ������ ����
CREATE TABLE #AvailableArticlesListTable (
	Id INT 
)

-- �������, � ������� ���������� ���� ����� �������� � ��������� ����
CREATE TABLE #StringIdListTable (Id int primary key)
			
IF @TakeArticlesFromArticleGroup = 1
BEGIN

	IF @AllArticleGroups = 0 
	BEGIN
		
		INSERT INTO #StringIdListTable (Id)
		SELECT Id 
		FROM dbo.SplitIntIdList(@ArticleGroupIdList)	-- ������ ������ �����
		
		-- ��������� ����
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
