BEGIN TRY
	BEGIN TRAN		
		
	PRINT '���������� ��������� �������'

	update a 
	set a.CreationDate = DateAdd(second, b.RowNumber, a.CreationDate),
		a.StartDate = DateAdd(second, b.RowNumber, a.StartDate)
	FROM DealQuota a INNER JOIN
	(select *,  
	RowNumber = ROW_NUMBER() over (order by Id)
	FROM DealQuota) AS b 
	ON a.Id = b.Id

COMMIT TRAN          
 --ROLLBACK TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN	
	
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH
