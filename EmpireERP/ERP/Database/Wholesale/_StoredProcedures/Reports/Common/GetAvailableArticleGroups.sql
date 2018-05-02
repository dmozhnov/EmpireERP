/*********************************************************************************************
���������: 	GetAvailableArticleGroups

��������:	��������� ������ ����� ����� ������� � ������ ���� ������������

���������:
	@IdList	������ � ������������� ����� ����� �������
	@AllArticleGroups ������� ������ ���� ����� �������
	
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
	@IdList VARCHAR(8000),	-- ������ ��������� ����� 
	@AllArticleGroups BIT	-- ������� ������ ���� ����� �������
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
