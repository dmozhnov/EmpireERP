BEGIN TRY
    BEGIN TRAN

	/* Удаляем все разнесения, которые помечены как удаленные */
	/* Из разнесений на платежи */
	DELETE  p1 FROM dbo.[PaymentDistributionToPayment] as p1
	LEFT JOIN dbo.[PaymentDistribution] as p
	ON (p1.Id = p.Id) 
	WHERE NOT(p.DeletionDate is NULL);
	
	/* Из разнесений на продажи */
	DELETE p2 FROM dbo.[PaymentDistributionToSale]  as p2
	LEFT JOIN dbo.[PaymentDistribution] as p
	ON (p2.Id = p.Id)	
	WHERE NOT(p.DeletionDate is NULL);
	
	/* Из разнесений на возвраты */
	DELETE p3 FROM dbo.[PaymentDistributionToSalesReturn] as p3
	LEFT JOIN dbo.[PaymentDistribution]  as p 
	ON (p3.Id = p.Id) 
	WHERE NOT(p.DeletionDate is NULL);
	
	/* Удаляем разнесения */
	DELETE FROM dbo.[PaymentDistribution] WHERE NOT(DeletionDate is NULL)
	
	/* Удаляем столбец */
    ALTER TABLE dbo.[PaymentDistribution]
        DROP COLUMN [DeletionDate];

    PRINT 'Обновление выполнено успешно'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT 'Произошла ошибка!!!'
    RETURN
END CATCH
