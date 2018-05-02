/* Сделано две транзакции, т.к. в рамках одной UPDATE не видит добавленный столбец */

/* Добавляем столбец в таблицу [Producer] */
BEGIN TRY
	BEGIN TRAN		
	
	ALTER TABLE dbo.[Producer] ADD ManufacturerId SMALLINT NULL
	ALTER TABLE dbo.[Producer] DROP COLUMN IsManufacturer
	
	alter table dbo.[Producer] 
        add constraint FK_Producer_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.Manufacturer
   		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH

GO

/* Удаляем ненужные столбцы и конвертируем данные */
BEGIN TRY
	BEGIN TRAN		

	UPDATE P
	SET P.ManufacturerId = M.Id
	FROM dbo.[Producer] P
		JOIN [Manufacturer] M ON P.Id = M.ProducerId
	
	ALTER TABLE dbo.[Manufacturer]  DROP CONSTRAINT FK_Manufacturer_Producer
	
	ALTER TABLE dbo.[Manufacturer] DROP COLUMN ProducerId

	PRINT 'Обновление выполнено успешно'	
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Произошла ошибка!!!'
	RETURN
END CATCH