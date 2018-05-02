BEGIN TRY
    BEGIN TRAN

	/* ������� ��� ����������, ������� �������� ��� ��������� */
	/* �� ���������� �� ������� */
	DELETE  p1 FROM dbo.[PaymentDistributionToPayment] as p1
	LEFT JOIN dbo.[PaymentDistribution] as p
	ON (p1.Id = p.Id) 
	WHERE NOT(p.DeletionDate is NULL);
	
	/* �� ���������� �� ������� */
	DELETE p2 FROM dbo.[PaymentDistributionToSale]  as p2
	LEFT JOIN dbo.[PaymentDistribution] as p
	ON (p2.Id = p.Id)	
	WHERE NOT(p.DeletionDate is NULL);
	
	/* �� ���������� �� �������� */
	DELETE p3 FROM dbo.[PaymentDistributionToSalesReturn] as p3
	LEFT JOIN dbo.[PaymentDistribution]  as p 
	ON (p3.Id = p.Id) 
	WHERE NOT(p.DeletionDate is NULL);
	
	/* ������� ���������� */
	DELETE FROM dbo.[PaymentDistribution] WHERE NOT(DeletionDate is NULL)
	
	/* ������� ������� */
    ALTER TABLE dbo.[PaymentDistribution]
        DROP COLUMN [DeletionDate];

    PRINT '���������� ��������� �������'

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    PRINT '��������� ������!!!'
    RETURN
END CATCH
