BEGIN TRY
	BEGIN TRAN		
	--создадим ограничения на уникальность
	PRINT 'Для успешной работы скрипта может понадобиться удалить товар №60 "соковыжималка Sc-012"'

		ALTER TABLE ForeignBank
		ADD CONSTRAINT SWIFT_Unique  
		UNIQUE (SWIFT)

		ALTER TABLE Article
		ADD CONSTRAINT ArticleFullName_Unique  
		UNIQUE (FullName)

		ALTER TABLE Article
		ADD CONSTRAINT ArticleShortName_Unique  
		UNIQUE (ShortName)

		ALTER TABLE Article
		ADD CONSTRAINT ArticleNumber_Unique 
		UNIQUE (Number)

	PRINT 'Обновление выполнено успешно'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT 'Произошла ошибка в строке ' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

