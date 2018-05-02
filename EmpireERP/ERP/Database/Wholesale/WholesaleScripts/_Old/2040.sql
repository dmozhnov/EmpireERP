BEGIN TRY
	BEGIN TRAN		

	PRINT '��� �������� ������ ������� �� ���� �������, ��������� � ������� �� �������, ������ ������������ �����'

	-- ���� ������: ������� Id ������, � ������� ���� StartDate ����������� ����� ���� ������� � ��������
	-- ��������� ���� CurrencyId
	update P
	set CurrencyRateId = (
		SELECT top 1 Id
		FROM [CurrencyRate]
		WHERE CurrencyId = T.CurrencyId
	    ORDER BY StartDate desc
	)
    FROM dbo.[ProductionOrderPayment] P
	JOIN [ProductionOrderTransportSheetPayment] PT on PT.Id = P.Id
	JOIN [ProductionOrderTransportSheet] T on T.Id = PT.TransportSheetId
	WHERE P.CurrencyRateId is null	

	update P
	set CurrencyRateId = (
		SELECT top 1 Id
		FROM [CurrencyRate]
		WHERE CurrencyId = T.CurrencyId
	    ORDER BY StartDate desc
	)
    FROM dbo.[ProductionOrderPayment] P
	JOIN [ProductionOrderExtraExpensesSheetPayment] PT on PT.Id = P.Id
	JOIN [ProductionOrderExtraExpensesSheet] T on T.Id = PT.ExtraExpensesSheetId
	WHERE P.CurrencyRateId is null

	update P
	set CurrencyRateId = (
		SELECT top 1 Id
		FROM [CurrencyRate]
		WHERE CurrencyId = 3
	    ORDER BY StartDate desc
	)
    FROM dbo.[ProductionOrderPayment] P
	JOIN [ProductionOrderCustomsDeclarationPayment] PT on PT.Id = P.Id
	JOIN [ProductionOrderCustomsDeclaration] T on T.Id = PT.CustomsDeclarationId
	WHERE P.CurrencyRateId is null

	update P
	set CurrencyRateId = (
		SELECT top 1 Id
		FROM [CurrencyRate]
		WHERE CurrencyId = T.CurrencyId
	    ORDER BY StartDate desc
	)
    FROM dbo.[ProductionOrderPayment] P
	JOIN [ProductionOrder] T on T.Id = P.ProductionOrderId
	WHERE P.CurrencyRateId is null AND P.ProductionOrderPaymentTypeId = 1

	ALTER TABLE dbo.[ProductionOrderPayment] ALTER COLUMN CurrencyRateId INT not null

	PRINT '���������� ��������� �������'
		
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
    PRINT '��������� ������ � ������' + STR(ERROR_LINE()) +':'
    PRINT ERROR_MESSAGE()
	RETURN
END CATCH

