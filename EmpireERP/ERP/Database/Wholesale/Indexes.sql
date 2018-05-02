-- ***************************************************************************************************
-- AcceptedPurchaseIndicator *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_AcceptedPurchaseIndicator_StorageId_UserId_ContractorId')
DROP INDEX IX_AcceptedPurchaseIndicator_StorageId_UserId_ContractorId ON [dbo].[AcceptedPurchaseIndicator]

CREATE INDEX [IX_AcceptedPurchaseIndicator_StorageId_UserId_ContractorId] ON [AcceptedPurchaseIndicator] ([StorageId], [UserId], [ContractorId], [ContractorOrganizationId], [ContractId], [AccountOrganizationId]) 
INCLUDE ([Id], [StartDate], [EndDate], [ArticleId], [PreviousId], [PurchaseCostSum], [Count]);

-- ***************************************************************************************************
-- AcceptedSaleIndicator *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId')
DROP INDEX IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId ON [dbo].[AcceptedSaleIndicator]

CREATE INDEX IX_AcceptedSaleIndicator_StorageId_UserId_TeamId_DealId ON [AcceptedSaleIndicator] ([StorageId], [UserId], [TeamId], [DealId])

-- ***************************************************************************************************
-- AccountingPriceListWaybillTaking *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_AccountingPriceListWaybillTaking_WaybillRowId_StorageId_AccountOrganizationId')
DROP INDEX IX_AccountingPriceListWaybillTaking_WaybillRowId_StorageId_AccountOrganizationId ON [dbo].AccountingPriceListWaybillTaking

CREATE INDEX [IX_AccountingPriceListWaybillTaking_WaybillRowId_StorageId_AccountOrganizationId] 
ON [AccountingPriceListWaybillTaking] ([WaybillRowId], [StorageId], [AccountOrganizationId]) 
INCLUDE ([Id], [TakingDate], [IsWaybillRowIncoming], [ArticleAccountingPriceId], [WaybillTypeId], [ArticleId], [AccountingPrice], [IsOnAccountingPriceListStart], [Count], [RevaluationDate]);

-- ***************************************************************************************************
-- ApprovedPurchaseIndicator *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ApprovedPurchaseIndicator_StorageId_UserId_ContractorId')
DROP INDEX IX_ApprovedPurchaseIndicator_StorageId_UserId_ContractorId ON [dbo].[ApprovedPurchaseIndicator]

CREATE INDEX [IX_ApprovedPurchaseIndicator_StorageId_UserId_ContractorId] ON [ApprovedPurchaseIndicator] ([StorageId], [UserId], [ContractorId], [ContractorOrganizationId], [ContractId], [AccountOrganizationId]) 
INCLUDE ([Id], [StartDate], [EndDate], [ArticleId], [PreviousId], [PurchaseCostSum], [Count]);

-- ***************************************************************************************************
-- Article *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Article_IsObsolete')
DROP INDEX [IX_Article_IsObsolete] ON [dbo].[Article]

CREATE INDEX [IX_Article_IsObsolete] ON [Article] ([IsObsolete], [FullName])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Article_ArticleGroup')
DROP INDEX [IX_Article_ArticleGroup] ON [dbo].[Article]

CREATE INDEX [IX_Article_ArticleGroup] ON [Article] ([ArticleGroupId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Article_ManufacturerId')
DROP INDEX [IX_Article_ManufacturerId] ON [dbo].[Article]


CREATE INDEX [IX_Article_ManufacturerId] ON [Article] ([ManufacturerId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Article_FullName')
DROP INDEX [IX_Article_FullName] ON [dbo].[Article]


CREATE INDEX [IX_Article_FullName] ON [Article] ([FullName])

-- ***************************************************************************************************
-- ArticleAccountingPrice ****************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId')
DROP INDEX [IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId] ON [dbo].[ArticleAccountingPrice]

CREATE INDEX [IX_ArticleAccountingPrice_DeletionDate_AccountingPriceListId_ArticleId] ON [ArticleAccountingPrice] ([DeletionDate], [AccountingPriceListId], [ArticleId]) 
INCLUDE ([Id], [AccountingPrice], [CreationDate], [UsedDefaultAccountingPriceCalcRule], [UsedDefaultLastDigitRule], [OrdinalNumber], [IsOverlappedOnEnd])


-- ***************************************************************************************************
-- ArticleAccountingPriceIndicator *******************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ArticleAccountingPriceIndicator_StorageId_ArticleId_StartDate_EndDate')
DROP INDEX [IX_ArticleAccountingPriceIndicator_StorageId_ArticleId_StartDate_EndDate] ON [dbo].[ArticleAccountingPriceIndicator]

CREATE INDEX [IX_ArticleAccountingPriceIndicator_StorageId_ArticleId_StartDate_EndDate] ON [ArticleAccountingPriceIndicator] ([StorageId], [ArticleId], [StartDate],[EndDate]) 
INCLUDE ([Id], [AccountingPrice], [AccountingPriceListId], [ArticleAccountingPriceId])

-- ***************************************************************************************************
-- ArticleMovementFactualFinancialIndicator ****************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ArticleMovementFactualFinancialIndicator_RecipientId_RecipientStorageId_SenderId_SenderStorageId')
DROP INDEX [IX_ArticleMovementFactualFinancialIndicator_RecipientId_RecipientStorageId_SenderId_SenderStorageId] ON [dbo].[ArticleMovementFactualFinancialIndicator]

CREATE INDEX [IX_ArticleMovementFactualFinancialIndicator_RecipientId_RecipientStorageId_SenderId_SenderStorageId] ON [ArticleMovementFactualFinancialIndicator] ([RecipientId], [RecipientStorageId], [SenderId], [SenderStorageId], [ArticleMovementOperationType]) 
INCLUDE ([Id], [StartDate], [EndDate], [PreviousId], [WaybillId], [AccountingPriceSum], [PurchaseCostSum], [SalePriceSum])


-- ***************************************************************************************************
-- ArticleMovementOperationCountIndicator ****************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ArticleMovementOperationCountIndicator_ArticleMovementOperationType_StorageId')
DROP INDEX [IX_ArticleMovementOperationCountIndicator_ArticleMovementOperationType_StorageId] ON [dbo].[ArticleMovementOperationCountIndicator]

CREATE INDEX [IX_ArticleMovementOperationCountIndicator_ArticleMovementOperationType_StorageId] ON [ArticleMovementOperationCountIndicator] ([ArticleMovementOperationType], [StorageId]) 
INCLUDE ([Id], [StartDate], [EndDate], [PreviousId], [Count]);

-- ***************************************************************************************************
-- Deal ****************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Deal_ClientId_DealStageId')
DROP INDEX IX_Deal_ClientId_DealStageId ON [dbo].[Deal]

CREATE INDEX [IX_Deal_ClientId_DealStageId] ON [Deal] ([ClientId], [DealStageId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Deal_ClientContractId')
DROP INDEX IX_Deal_ClientContractId ON [dbo].[Deal]

CREATE INDEX [IX_Deal_ClientContractId] ON [dbo].[Deal] ([ClientContractId])

-- ***************************************************************************************************
-- DealPaymentDocument
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_DealPaymentDocument_DeletionDate_DealId_DealPaymentDocumentTypeId')
DROP INDEX [IX_DealPaymentDocument_DeletionDate_DealId_DealPaymentDocumentTypeId] ON [dbo].[DealPaymentDocument]

CREATE INDEX [IX_DealPaymentDocument_DeletionDate_DealId_DealPaymentDocumentTypeId] ON [DealPaymentDocument] ([DeletionDate], [DealId], [DealPaymentDocumentTypeId]) INCLUDE ([Id])

-- ***************************************************************************************************
-- DealPaymentDocumentDistribution
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_DealPaymentDocumentDistribution_SourceDealPaymentDocumentId')
DROP INDEX [IX_DealPaymentDocumentDistribution_SourceDealPaymentDocumentId] ON [dbo].[DealPaymentDocumentDistribution]

CREATE INDEX [IX_DealPaymentDocumentDistribution_SourceDealPaymentDocumentId] ON [DealPaymentDocumentDistribution] ([SourceDealPaymentDocumentId]) 
INCLUDE ([Id], [Sum], [CreationDate], [OrdinalNumber])

-- ***************************************************************************************************
-- DealPaymentDocumentDistributionToSaleWaybill
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_DealPaymentDocumentDistributionToSaleWaybill_SaleWaybillId')
DROP INDEX IX_DealPaymentDocumentDistributionToSaleWaybill_SaleWaybillId ON [dbo].DealPaymentDocumentDistributionToSaleWaybill

CREATE INDEX [IX_DealPaymentDocumentDistributionToSaleWaybill_SaleWaybillId] ON [DealPaymentDocumentDistributionToSaleWaybill] ([SaleWaybillId])

-- ***************************************************************************************************
-- DealPaymentDocumentDistributionToReturnFromClientWaybill
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_DealPaymentDocumentDistributionToReturnFromClientWaybill_SaleWaybillId')
DROP INDEX IX_DealPaymentDocumentDistributionToReturnFromClientWaybill_SaleWaybillId ON [dbo].DealPaymentDocumentDistributionToReturnFromClientWaybill

CREATE INDEX IX_DealPaymentDocumentDistributionToReturnFromClientWaybill_SaleWaybillId ON [DealPaymentDocumentDistributionToReturnFromClientWaybill] ([SaleWaybillId])

-- ***************************************************************************************************
-- ExactArticleAvailabilityIndicator *****************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId')
DROP INDEX [IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId] ON [dbo].[ExactArticleAvailabilityIndicator]

CREATE INDEX [IX_ExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_BatchId] ON [ExactArticleAvailabilityIndicator] ([StorageId], [AccountOrganizationId], [ArticleId], [BatchId], [StartDate], [EndDate], [Count]) 
INCLUDE ([Id], [PurchaseCost], [PreviousId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ExactArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count')
DROP INDEX IX_ExactArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count ON [dbo].[ExactArticleAvailabilityIndicator]

CREATE INDEX [IX_ExactArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count] ON [ExactArticleAvailabilityIndicator] ([ArticleId], [StartDate], [EndDate], [Count])
INCLUDE ([Id], [StorageId], [AccountOrganizationId], [BatchId], [PurchaseCost], [PreviousId]);

-- ***************************************************************************************************
-- IncomingAcceptedArticleAvailabilityIndicator ****************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_IncomingAcceptedArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_Count')
DROP INDEX [IX_IncomingAcceptedArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_Count] ON [dbo].[IncomingAcceptedArticleAvailabilityIndicator]

CREATE INDEX [IX_IncomingAcceptedArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_Count] ON [IncomingAcceptedArticleAvailabilityIndicator] ([StorageId], [AccountOrganizationId], [ArticleId], [StartDate], [EndDate], [Count]) 
INCLUDE ([Id], [BatchId], [PurchaseCost], [PreviousId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_IncomingAcceptedArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count')
DROP INDEX [IX_IncomingAcceptedArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count] ON [dbo].[IncomingAcceptedArticleAvailabilityIndicator]

CREATE INDEX [IX_IncomingAcceptedArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count] ON [IncomingAcceptedArticleAvailabilityIndicator] ([ArticleId], [StartDate], [EndDate], [Count])
INCLUDE ([Id], [StorageId], [AccountOrganizationId], [BatchId], [PurchaseCost], [PreviousId]);

-- ***************************************************************************************************
-- MovementWaybill ********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_MovementWaybill_DeletionDate_MovementWaybillStateId')
DROP INDEX IX_MovementWaybill_DeletionDate_MovementWaybillStateId ON [dbo].[MovementWaybill]

CREATE INDEX IX_MovementWaybill_DeletionDate_MovementWaybillStateId ON [dbo].[MovementWaybill] ([DeletionDate], [MovementWaybillStateId])

-- ***************************************************************************************************
-- MovementWaybillRow ********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_MovementWaybillRow_DeletionDate_MovementWaybillId_ReceiptWaybillRowId_IsUsingManualSource_AvailableToReserveCount')
DROP INDEX IX_MovementWaybillRow_DeletionDate_MovementWaybillId_ReceiptWaybillRowId_IsUsingManualSource_AvailableToReserveCount ON [dbo].MovementWaybillRow

CREATE INDEX [IX_MovementWaybillRow_DeletionDate_MovementWaybillId_ReceiptWaybillRowId_IsUsingManualSource_AvailableToReserveCount] ON [MovementWaybillRow] ([DeletionDate], [MovementWaybillId], [ReceiptWaybillRowId], [IsUsingManualSource], [AvailableToReserveCount]) 
INCLUDE ([Id], [MovingCount], [CreationDate], [AcceptedCount], [ShippedCount], [FinallyMovedCount], [OutgoingWaybillRowStateId], [MovementWaybillRecipientArticleAccountingPriceId], [MovementWaybillSenderArticleAccountingPriceId], [ValueAddedTaxId], [UsageAsManualSourceCount], [ArticleId]);

-- ***************************************************************************************************
-- OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator ****************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator_Storage_AccountOrganization_StartDate_EndDate_Article_Count')
DROP INDEX IX_OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator_Storage_AccountOrganization_StartDate_EndDate_Article_Count ON [dbo].OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator

CREATE INDEX IX_OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator_Storage_AccountOrganization_StartDate_EndDate_Article_Count 
ON [OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator] ([StorageId], [AccountOrganizationId], [StartDate], [EndDate], [ArticleId], [Count]) 
INCLUDE ([Id], [BatchId], [PurchaseCost], [PreviousId]);

-- ***************************************************************************************************
-- OutgoingAcceptedFromExactArticleAvailabilityIndicator ****************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_OutgoingAcceptedFromExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_Count')
DROP INDEX [IX_OutgoingAcceptedFromExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_Count] ON [dbo].OutgoingAcceptedFromExactArticleAvailabilityIndicator

CREATE INDEX [IX_OutgoingAcceptedFromExactArticleAvailabilityIndicator_StorageId_AccountOrganizationId_ArticleId_Count] ON OutgoingAcceptedFromExactArticleAvailabilityIndicator ([StorageId], [AccountOrganizationId], [ArticleId], [StartDate], [EndDate], [Count]) 
INCLUDE ([Id], [BatchId], [PurchaseCost], [PreviousId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_OutgoingAcceptedFromExactArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count')
DROP INDEX [IX_OutgoingAcceptedFromExactArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count] ON [dbo].[OutgoingAcceptedFromExactArticleAvailabilityIndicator]

CREATE INDEX [IX_OutgoingAcceptedFromExactArticleAvailabilityIndicator_ArticleId_StartDate_EndDate_Count] ON [OutgoingAcceptedFromExactArticleAvailabilityIndicator] ([ArticleId], [StartDate], [EndDate], [Count])
INCLUDE ([Id], [StorageId], [AccountOrganizationId], [BatchId], [PurchaseCost], [PreviousId]);

-- ***************************************************************************************************
-- Organization *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Organization_FullName_DeletionDate')
DROP INDEX IX_Organization_FullName_DeletionDate ON [dbo].[Organization] 

CREATE INDEX [IX_Organization_FullName_DeletionDate] ON [Organization] ([FullName], [DeletionDate])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_Organization_ShortName_DeletionDate')
DROP INDEX IX_Organization_ShortName_DeletionDate ON [dbo].[Organization] 

CREATE INDEX [IX_Organization_ShortName_DeletionDate] ON [Organization] ([ShortName], [DeletionDate])

-- ***************************************************************************************************
-- ReceiptWaybill *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReceiptWaybill_Number_Id')
DROP INDEX IX_ReceiptWaybill_Number_Id ON [dbo].ReceiptWaybill 

CREATE INDEX [IX_ReceiptWaybill_Number_Id] ON [ReceiptWaybill] ([Number], [Id])
--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReceiptWaybill_DeletionDate_ReceiptWaybillStateId_ReceiptWaybillReceiptStorageId')
DROP INDEX IX_ReceiptWaybill_DeletionDate_ReceiptWaybillStateId_ReceiptWaybillReceiptStorageId ON [dbo].ReceiptWaybill 

CREATE INDEX [IX_ReceiptWaybill_DeletionDate_ReceiptWaybillStateId_ReceiptWaybillReceiptStorageId] ON [ReceiptWaybill] ([DeletionDate], [ReceiptWaybillStateId], [ReceiptWaybillReceiptStorageId])

-- ***************************************************************************************************
-- ReceiptWaybillRow *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId')
DROP INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [dbo].[ReceiptWaybillRow] 

CREATE INDEX [IX_ReceiptWaybillRow_DeletionDate_ReceiptWaybillId] ON [ReceiptWaybillRow] ([DeletionDate], [ReceiptWaybillId], [ArticleId]) 
INCLUDE ([Id], [ArticleMeasureUnitScale], [PendingCount], [PendingSum], [ReceiptedCount], [ProviderCount], [ApprovedCount], [ApprovedSum], [PurchaseCost], [CustomsDeclarationNumber], [CreationDate], [AcceptedCount], [ShippedCount], [FinallyMovedCount], [RecipientArticleAccountingPriceId], [PendingValueAddedTaxId], [ApprovedValueAddedTaxId], [CountryId], [ManufacturerId], [InitialPurchaseCost], [ProviderSum], [ApprovedPurchaseCost], [UsageAsManualSourceCount], [AvailableToReserveCount], [OrdinalNumber], AreCountDivergencesAfterReceipt, AreSumDivergencesAfterReceipt);

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReceiptWaybillRow_DeletionDate_ArticleId')
DROP INDEX IX_ReceiptWaybillRow_DeletionDate_ArticleId ON [dbo].[ReceiptWaybillRow] 

CREATE INDEX [IX_ReceiptWaybillRow_DeletionDate_ArticleId] ON [dbo].[ReceiptWaybillRow] ([DeletionDate], [ArticleId]) 
INCLUDE ([Id], [ArticleMeasureUnitScale], [PendingCount], [PendingSum], [ReceiptedCount], [ProviderCount], [ApprovedCount], [ApprovedSum], [PurchaseCost], [CustomsDeclarationNumber], [CreationDate], [AcceptedCount], [ShippedCount], [FinallyMovedCount], [RecipientArticleAccountingPriceId], [ReceiptWaybillId], [PendingValueAddedTaxId], [ApprovedValueAddedTaxId], [CountryId], [ManufacturerId], [InitialPurchaseCost], [ProviderSum], [ApprovedPurchaseCost], [UsageAsManualSourceCount], [AvailableToReserveCount], [OrdinalNumber], [AreCountDivergencesAfterReceipt], [AreSumDivergencesAfterReceipt]);

-- ***************************************************************************************************
-- ReturnFromClientBySaleAcceptanceDateIndicator *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReturnFromClientBySaleAcceptanceDateIndicator_CuratorId_DealId_BatchId_EndDate')
DROP INDEX IX_ReturnFromClientBySaleAcceptanceDateIndicator_CuratorId_DealId_BatchId_EndDate ON [dbo].ReturnFromClientBySaleAcceptanceDateIndicator

CREATE INDEX [IX_ReturnFromClientBySaleAcceptanceDateIndicator_CuratorId_DealId_BatchId_EndDate] ON [ReturnFromClientBySaleAcceptanceDateIndicator] ([ReturnFromClientWaybillCuratorId], [DealId], [BatchId], [EndDate])

-- ***************************************************************************************************
-- ReturnFromClientBySaleShippingDateIndicator *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReturnFromClientBySaleShippingDateIndicator_CuratorId_DealId_BatchId_EndDate')
DROP INDEX IX_ReturnFromClientBySaleShippingDateIndicator_CuratorId_DealId_BatchId_EndDate ON [dbo].ReturnFromClientBySaleShippingDateIndicator

CREATE INDEX [IX_ReturnFromClientBySaleShippingDateIndicator_CuratorId_DealId_BatchId_EndDate] ON [ReturnFromClientBySaleShippingDateIndicator] ([ReturnFromClientWaybillCuratorId], [DealId], [BatchId], [EndDate]);

-- ***************************************************************************************************
-- ReturnFromClientWaybillRow *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ReturnFromClientWaybillRow_DeletionDate_ReturnFromClientWaybillId')
DROP INDEX [IX_ReturnFromClientWaybillRow_DeletionDate_ReturnFromClientWaybillId] ON [dbo].ReturnFromClientWaybillRow

CREATE INDEX [IX_ReturnFromClientWaybillRow_DeletionDate_ReturnFromClientWaybillId] ON [dbo].[ReturnFromClientWaybillRow] ([DeletionDate], [ReturnFromClientWaybillId])

-- ***************************************************************************************************
-- SaleWaybill *********************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybill_DeletionDate_DealId_Id')
DROP INDEX IX_SaleWaybill_DeletionDate_DealId_Id ON [dbo].SaleWaybill

CREATE INDEX IX_SaleWaybill_DeletionDate_DealId_Id ON [SaleWaybill] ([DeletionDate], [DealId], [Id])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybill_QuotaId')
DROP INDEX IX_SaleWaybill_QuotaId ON [dbo].SaleWaybill

CREATE INDEX [IX_SaleWaybill_QuotaId] ON [SaleWaybill] ([DeletionDate], [QuotaId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybill_Number')
DROP INDEX IX_SaleWaybill_Number ON [dbo].SaleWaybill

CREATE INDEX [IX_SaleWaybill_Number] ON [SaleWaybill] ([Number])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybill_DealId')
DROP INDEX IX_SaleWaybill_DealId ON [dbo].SaleWaybill


CREATE INDEX [IX_SaleWaybill_DealId] ON [SaleWaybill] ([DealId]) INCLUDE ([Id])

-- ***************************************************************************************************
-- SaleWaybillRow *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybillRow_DeletionDate_SaleWaybillId')
DROP INDEX IX_SaleWaybillRow_DeletionDate_SaleWaybillId ON [dbo].SaleWaybillRow

CREATE INDEX IX_SaleWaybillRow_DeletionDate_SaleWaybillId ON [SaleWaybillRow] ([DeletionDate], [SaleWaybillId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_SaleWaybillRow_SaleWaybillId')
DROP INDEX IX_SaleWaybillRow_SaleWaybillId ON [dbo].SaleWaybillRow

CREATE INDEX IX_SaleWaybillRow_SaleWaybillId ON [SaleWaybillRow] ([SaleWaybillId])

-- ***************************************************************************************************
-- ShippedSaleIndicator *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId')
DROP INDEX IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId ON [dbo].[ShippedSaleIndicator]

CREATE INDEX IX_ShippedSaleIndicator_StorageId_UserId_TeamId_DealId ON [ShippedSaleIndicator] ([StorageId], [UserId], [TeamId], [DealId])

--
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_ShippedSaleIndicator_DealId_StartDate_EndDate_SoldCount')
DROP INDEX IX_ShippedSaleIndicator_DealId_StartDate_EndDate_SoldCount ON [dbo].[ShippedSaleIndicator]

CREATE INDEX [IX_ShippedSaleIndicator_DealId_StartDate_EndDate_SoldCount] ON [ShippedSaleIndicator] ([DealId], [StartDate], [EndDate], [SoldCount])

-- ***************************************************************************************************
-- WaybillRowArticleMovement *******************************************************************************************
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_WaybillRowArticleMovement_DestinationWaybillRowId_SourceWaybillTypeId')
DROP INDEX IX_WaybillRowArticleMovement_DestinationWaybillRowId_SourceWaybillTypeId ON [dbo].WaybillRowArticleMovement

CREATE INDEX IX_WaybillRowArticleMovement_DestinationWaybillRowId_SourceWaybillTypeId ON [WaybillRowArticleMovement] ([DestinationWaybillRowId],		[SourceWaybillTypeId]) 
INCLUDE ([Id], [DestinationWaybillTypeId], [SourceWaybillRowId], [MovingCount], [IsManuallyCreated])

--

IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = 'IX_WaybillRowArticleMovement_SourceWaybillRowId')
DROP INDEX IX_WaybillRowArticleMovement_SourceWaybillRowId ON [dbo].[WaybillRowArticleMovement]

CREATE INDEX IX_WaybillRowArticleMovement_SourceWaybillRowId ON [WaybillRowArticleMovement] ([SourceWaybillRowId]) 
INCLUDE ([DestinationWaybillRowId])

/*
IF EXISTS(SELECT * FROM sys.sysindexes WHERE name = '')
DROP INDEX [название] ON [dbo].[таблица]
*/

