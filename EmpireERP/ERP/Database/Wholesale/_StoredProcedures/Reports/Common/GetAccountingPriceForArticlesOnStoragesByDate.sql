/*********************************************************************************************
���������: 	GetAccountingPriceForArticlesOnStoragesByDate

��������:	��������� �� ��� ������� �� ������� �� ��������� ���� 

���������:
	@Date ����, �� ������� ���������� �������� ��
	
	��� �������� ������ ������� � �� ������������ ��������� �������, � ������� � ���������� ��
	create table #ArticleAccountingPriceForActiclesOnStoragesDetermination(
		ArticleId INT not null,
		StorageId SMALLINT not null,
		AccountingPrice DECIMAL(18,2) null
	)
*********************************************************************************************/

IF EXISTS (
  SELECT *
    FROM INFORMATION_SCHEMA.ROUTINES
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'GetAccountingPriceForArticlesOnStoragesByDate'
)
   DROP PROCEDURE dbo.GetAccountingPriceForArticlesOnStoragesByDate
GO

CREATE PROCEDURE GetAccountingPriceForArticlesOnStoragesByDate(
	@Date DATETIME	-- ����, �� ������� ���������� �������� ��
) AS

-- ��������������� ������� ��� ������������ ������� �� ��� ������� �� ��
create table #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination(
	ArticleId INT not null,
	StorageId SMALLINT not null,
	AccountingPrice DECIMAL(18,2) not null,
	RowNumber INT not null
)

-- �������� �� ��� ������� � ��������� �� �� ���� ���������� � �������� � �������� �������(�.�. ��������� ���������� � �������� ����� ������)
INSERT INTO #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination(ArticleId,StorageId,AccountingPrice,RowNumber)
SELECT aapi.ArticleId, aapi.StorageId, aapi.AccountingPrice, 
	ROW_NUMBER() OVER(PARTITION BY aapi.ArticleId, aapi.StorageId ORDER BY aapi.StartDate DESC )
FROM ArticleAccountingPriceIndicator aapi
JOIN #ArticleAccountingPriceForActiclesOnStoragesDetermination d on d.ArticleId = aapi.ArticleId AND d.StorageId = aapi.StorageId AND 
	(aapi.StartDate <= @Date AND (aapi.EndDate > @Date OR aapi.EndDate is null))

-- ��������� ������� ������� ���������� ��
UPDATE #ArticleAccountingPriceForActiclesOnStoragesDetermination
SET AccountingPrice = api.AccountingPrice
FROM #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination api
WHERE api.ArticleId = #ArticleAccountingPriceForActiclesOnStoragesDetermination.ArticleId AND
	api.StorageId = #ArticleAccountingPriceForActiclesOnStoragesDetermination.StorageId AND
	api.RowNumber = 1	-- ����� ��������� �� �� ���� ���������� � ��������

drop table #tmp_ArticleAccountingPriceForActiclesOnStoragesDetermination	

GO