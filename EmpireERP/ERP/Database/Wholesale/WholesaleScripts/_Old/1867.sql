BEGIN TRY
	BEGIN TRAN		
	
	UPDATE R
	SET [ArticleMeasureUnitScale] = M.Scale
	FROM [dbo].[ReceiptWaybillRow] R
	JOIN [Article] A on A.id = R.ArticleId
	JOIN [MeasureUnit] M on M.Id = A.MeasureUnitId
	--WHERE T.OrdinalNumber = 1

	PRINT '���������� ��������� �������'	
			
	COMMIT TRAN	
	
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT '��������� ������!!!'
	RETURN
END CATCH


