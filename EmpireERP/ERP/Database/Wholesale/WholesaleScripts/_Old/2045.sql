BEGIN TRY
	BEGIN TRAN		
	--создадим ограничения на уникальность

		ALTER TABLE RussianBank
		ADD CONSTRAINT BIK_Unique  
		UNIQUE (BIC)

	PRINT 'Обновление выполнено успешно'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке ' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

