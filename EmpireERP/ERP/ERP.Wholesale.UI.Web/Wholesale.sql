
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_AccountingPriceList_Curator]') AND parent_object_id = OBJECT_ID('dbo.[AccountingPriceList]'))
alter table dbo.[AccountingPriceList]  drop constraint FK_AccountingPriceList_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_AccountingPriceDeterminationRule_Storage]') AND parent_object_id = OBJECT_ID('dbo.[AccountingPriceList]'))
alter table dbo.[AccountingPriceList]  drop constraint FK_AccountingPriceDeterminationRule_Storage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_LastDigitCalcRule_Storage]') AND parent_object_id = OBJECT_ID('dbo.[AccountingPriceList]'))
alter table dbo.[AccountingPriceList]  drop constraint FK_LastDigitCalcRule_Storage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_AccountingPriceList_Storage]') AND parent_object_id = OBJECT_ID('dbo.AccountingPriceListStorage'))
alter table dbo.AccountingPriceListStorage  drop constraint PFK_AccountingPriceList_Storage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Storage_AccountingPriceList]') AND parent_object_id = OBJECT_ID('dbo.AccountingPriceListStorage'))
alter table dbo.AccountingPriceListStorage  drop constraint PFK_Storage_AccountingPriceList


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_AccountOrganization_AccountOrganizationDocumentNumbers_AccountOrganizationId]') AND parent_object_id = OBJECT_ID('dbo.[AccountOrganizationDocumentNumbers]'))
alter table dbo.[AccountOrganizationDocumentNumbers]  drop constraint FK_AccountOrganization_AccountOrganizationDocumentNumbers_AccountOrganizationId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_AccountingPriceList_ArticleAccountingPrice_AccountingPriceListId]') AND parent_object_id = OBJECT_ID('dbo.[ArticleAccountingPrice]'))
alter table dbo.[ArticleAccountingPrice]  drop constraint FK_AccountingPriceList_ArticleAccountingPrice_AccountingPriceListId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ArticleAccountingPrice_Article]') AND parent_object_id = OBJECT_ID('dbo.[ArticleAccountingPrice]'))
alter table dbo.[ArticleAccountingPrice]  drop constraint FK_ArticleAccountingPrice_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ArticleGroup_ArticleGroup_ParentId]') AND parent_object_id = OBJECT_ID('dbo.[ArticleGroup]'))
alter table dbo.[ArticleGroup]  drop constraint FK_ArticleGroup_ArticleGroup_ParentId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Article_ArticleGroup]') AND parent_object_id = OBJECT_ID('dbo.[Article]'))
alter table dbo.[Article]  drop constraint FK_Article_ArticleGroup


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Article_Trademark]') AND parent_object_id = OBJECT_ID('dbo.[Article]'))
alter table dbo.[Article]  drop constraint FK_Article_Trademark


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Article_Manufacturer]') AND parent_object_id = OBJECT_ID('dbo.[Article]'))
alter table dbo.[Article]  drop constraint FK_Article_Manufacturer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Article_ProductionCountry]') AND parent_object_id = OBJECT_ID('dbo.[Article]'))
alter table dbo.[Article]  drop constraint FK_Article_ProductionCountry


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Article_MeasureUnit]') AND parent_object_id = OBJECT_ID('dbo.[Article]'))
alter table dbo.[Article]  drop constraint FK_Article_MeasureUnit


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Article_Certificate]') AND parent_object_id = OBJECT_ID('dbo.[Article]'))
alter table dbo.[Article]  drop constraint FK_Article_Certificate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ForeignBank]') AND parent_object_id = OBJECT_ID('dbo.[ForeignBank]'))
alter table dbo.[ForeignBank]  drop constraint PFK_ForeignBank


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_RussianBank]') AND parent_object_id = OBJECT_ID('dbo.[RussianBank]'))
alter table dbo.[RussianBank]  drop constraint PFK_RussianBank


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BankAccount_Currency]') AND parent_object_id = OBJECT_ID('dbo.[BankAccount]'))
alter table dbo.[BankAccount]  drop constraint FK_BankAccount_Currency


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BankAccount_Bank]') AND parent_object_id = OBJECT_ID('dbo.[BankAccount]'))
alter table dbo.[BankAccount]  drop constraint FK_BankAccount_Bank


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_RussianBankAccount]') AND parent_object_id = OBJECT_ID('dbo.[RussianBankAccount]'))
alter table dbo.[RussianBankAccount]  drop constraint PFK_RussianBankAccount


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Organization_RussianBankAccount_OrganizationId]') AND parent_object_id = OBJECT_ID('dbo.[RussianBankAccount]'))
alter table dbo.[RussianBankAccount]  drop constraint FK_Organization_RussianBankAccount_OrganizationId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ForeignBankAccount]') AND parent_object_id = OBJECT_ID('dbo.[ForeignBankAccount]'))
alter table dbo.[ForeignBankAccount]  drop constraint PFK_ForeignBankAccount


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Organization_ForeignBankAccount_OrganizationId]') AND parent_object_id = OBJECT_ID('dbo.[ForeignBankAccount]'))
alter table dbo.[ForeignBankAccount]  drop constraint FK_Organization_ForeignBankAccount_OrganizationId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BaseTaskHistoryItem_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[BaseTaskHistoryItem]'))
alter table dbo.[BaseTaskHistoryItem]  drop constraint FK_BaseTaskHistoryItem_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_TaskExecutionHistoryItem]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint PFK_TaskExecutionHistoryItem


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionItem_TaskExecutionHistoryItem_TaskExecutionItemId]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint FK_TaskExecutionItem_TaskExecutionHistoryItem_TaskExecutionItemId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionHistoryItem_Task]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint FK_TaskExecutionHistoryItem_Task


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionHistoryItem_TaskExecutionState]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint FK_TaskExecutionHistoryItem_TaskExecutionState


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionHistoryItem_TaskType]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionHistoryItem]'))
alter table dbo.[TaskExecutionHistoryItem]  drop constraint FK_TaskExecutionHistoryItem_TaskType


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_TaskHistoryItem]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint PFK_TaskHistoryItem


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_TaskHistoryItem_TaskId]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_Task_TaskHistoryItem_TaskId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_Contractor]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_Contractor


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_Deal]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_Deal


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_ExecutedBy]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_ExecutedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskPriority]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskPriority


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_ProductionOrder]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_ProductionOrder


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskType]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskType


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskExecutionState]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskExecutionState


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskHistoryItem_TaskExecutionItem]') AND parent_object_id = OBJECT_ID('dbo.[TaskHistoryItem]'))
alter table dbo.[TaskHistoryItem]  drop constraint FK_TaskHistoryItem_TaskExecutionItem


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_Curator]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_Storage]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_Storage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_Sender]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_Sender


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_Recipient]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_Recipient


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_ValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_ValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_AcceptedBy]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_AcceptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_ChangedOwnerBy]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybill]'))
alter table dbo.[ChangeOwnerWaybill]  drop constraint FK_ChangeOwnerWaybill_ChangedOwnerBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_AccountOrganization_Contract_AccountOrganizationId]') AND parent_object_id = OBJECT_ID('dbo.[Contract]'))
alter table dbo.[Contract]  drop constraint FK_AccountOrganization_Contract_AccountOrganizationId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ContractorOrganization_Contract_ContractorOrganizationId]') AND parent_object_id = OBJECT_ID('dbo.[Contract]'))
alter table dbo.[Contract]  drop constraint FK_ContractorOrganization_Contract_ContractorOrganizationId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Contract_Contractor]') AND parent_object_id = OBJECT_ID('dbo.ContractorContract'))
alter table dbo.ContractorContract  drop constraint PFK_Contract_Contractor


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Contractor_Contract]') AND parent_object_id = OBJECT_ID('dbo.ContractorContract'))
alter table dbo.ContractorContract  drop constraint PFK_Contractor_Contract


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ClientContract]') AND parent_object_id = OBJECT_ID('dbo.[ClientContract]'))
alter table dbo.[ClientContract]  drop constraint PFK_ClientContract


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProducerContract]') AND parent_object_id = OBJECT_ID('dbo.[ProducerContract]'))
alter table dbo.[ProducerContract]  drop constraint PFK_ProducerContract


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProviderContract]') AND parent_object_id = OBJECT_ID('dbo.[ProviderContract]'))
alter table dbo.[ProviderContract]  drop constraint PFK_ProviderContract


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Contractor_ContractorOrganization]') AND parent_object_id = OBJECT_ID('dbo.ContractorOrganizationContractor'))
alter table dbo.ContractorOrganizationContractor  drop constraint PFK_Contractor_ContractorOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ContractorOrganization_Contractor]') AND parent_object_id = OBJECT_ID('dbo.ContractorOrganizationContractor'))
alter table dbo.ContractorOrganizationContractor  drop constraint PFK_ContractorOrganization_Contractor


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Client]') AND parent_object_id = OBJECT_ID('dbo.[Client]'))
alter table dbo.[Client]  drop constraint PFK_Client


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Client_Type]') AND parent_object_id = OBJECT_ID('dbo.[Client]'))
alter table dbo.[Client]  drop constraint FK_Client_Type


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Client_Region]') AND parent_object_id = OBJECT_ID('dbo.[Client]'))
alter table dbo.[Client]  drop constraint FK_Client_Region


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Client_ServiceProgram]') AND parent_object_id = OBJECT_ID('dbo.[Client]'))
alter table dbo.[Client]  drop constraint FK_Client_ServiceProgram


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Client_ManualBlocker]') AND parent_object_id = OBJECT_ID('dbo.[Client]'))
alter table dbo.[Client]  drop constraint FK_Client_ManualBlocker


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Producer]') AND parent_object_id = OBJECT_ID('dbo.[Producer]'))
alter table dbo.[Producer]  drop constraint PFK_Producer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Producer_Curator]') AND parent_object_id = OBJECT_ID('dbo.[Producer]'))
alter table dbo.[Producer]  drop constraint FK_Producer_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Producer_Manufacturer]') AND parent_object_id = OBJECT_ID('dbo.ProducerManufacturer'))
alter table dbo.ProducerManufacturer  drop constraint PFK_Producer_Manufacturer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Manufacturer_Producer]') AND parent_object_id = OBJECT_ID('dbo.ProducerManufacturer'))
alter table dbo.ProducerManufacturer  drop constraint PFK_Manufacturer_Producer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Provider]') AND parent_object_id = OBJECT_ID('dbo.[Provider]'))
alter table dbo.[Provider]  drop constraint PFK_Provider


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Provider_Type]') AND parent_object_id = OBJECT_ID('dbo.[Provider]'))
alter table dbo.[Provider]  drop constraint FK_Provider_Type


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_CurrencyRate_PreviousCurrencyRate]') AND parent_object_id = OBJECT_ID('dbo.[CurrencyRate]'))
alter table dbo.[CurrencyRate]  drop constraint FK_CurrencyRate_PreviousCurrencyRate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Currency_CurrencyRate_CurrencyId]') AND parent_object_id = OBJECT_ID('dbo.[CurrencyRate]'))
alter table dbo.[CurrencyRate]  drop constraint FK_Currency_CurrencyRate_CurrencyId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_CurrencyRate_BaseCurrency]') AND parent_object_id = OBJECT_ID('dbo.[CurrencyRate]'))
alter table dbo.[CurrencyRate]  drop constraint FK_CurrencyRate_BaseCurrency


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Client_Deal_ClientId]') AND parent_object_id = OBJECT_ID('dbo.[Deal]'))
alter table dbo.[Deal]  drop constraint FK_Client_Deal_ClientId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Deal_Contract]') AND parent_object_id = OBJECT_ID('dbo.[Deal]'))
alter table dbo.[Deal]  drop constraint FK_Deal_Contract


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Deal_Curator]') AND parent_object_id = OBJECT_ID('dbo.[Deal]'))
alter table dbo.[Deal]  drop constraint FK_Deal_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Deal_DealQuota]') AND parent_object_id = OBJECT_ID('dbo.DealDealQuota'))
alter table dbo.DealDealQuota  drop constraint PFK_Deal_DealQuota


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealQuota_Deal]') AND parent_object_id = OBJECT_ID('dbo.DealDealQuota'))
alter table dbo.DealDealQuota  drop constraint PFK_DealQuota_Deal


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentFromClient_DealPaymentDocumentDistribution_SourceDealPaymentDocumentId]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistribution]'))
alter table dbo.[DealPaymentDocumentDistribution]  drop constraint FK_DealPaymentFromClient_DealPaymentDocumentDistribution_SourceDealPaymentDocumentId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealPaymentDocumentDistributionToDealPaymentDocument]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]'))
alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]  drop constraint PFK_DealPaymentDocumentDistributionToDealPaymentDocument


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentToClient_DealPaymentDocumentDistributionToDealPaymentDocument_DestinationDealPaymentDocumentId]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]'))
alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]  drop constraint FK_DealPaymentToClient_DealPaymentDocumentDistributionToDealPaymentDocument_DestinationDealPaymentDocumentId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentDocumentDistributionToDealPaymentDocument_SourceDistributionToReturnFromClientWaybill]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]'))
alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]  drop constraint FK_DealPaymentDocumentDistributionToDealPaymentDocument_SourceDistributionToReturnFromClientWaybill


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealPaymentDocumentDistributionToReturnFromClientWaybill]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]'))
alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]  drop constraint PFK_DealPaymentDocumentDistributionToReturnFromClientWaybill


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentDocumentDistributionToReturnFromClientWaybill_SaleWaybill]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]'))
alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]  drop constraint FK_DealPaymentDocumentDistributionToReturnFromClientWaybill_SaleWaybill


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_DealPaymentDocumentDistributionToReturnFromClientWaybill_ReturnFromClientWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]'))
alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_DealPaymentDocumentDistributionToReturnFromClientWaybill_ReturnFromClientWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealPaymentDocumentDistributionToSaleWaybill]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToSaleWaybill]'))
alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill]  drop constraint PFK_DealPaymentDocumentDistributionToSaleWaybill


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_DealPaymentDocumentDistributionToSaleWaybill_SaleWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToSaleWaybill]'))
alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill]  drop constraint FK_SaleWaybill_DealPaymentDocumentDistributionToSaleWaybill_SaleWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentDocumentDistributionToSaleWaybill_SourceDistributionToReturnFromClientWaybill]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocumentDistributionToSaleWaybill]'))
alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill]  drop constraint FK_DealPaymentDocumentDistributionToSaleWaybill_SourceDistributionToReturnFromClientWaybill


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Deal_DealPaymentDocument_DealId]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocument]'))
alter table dbo.[DealPaymentDocument]  drop constraint FK_Deal_DealPaymentDocument_DealId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentDocument_Team]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocument]'))
alter table dbo.[DealPaymentDocument]  drop constraint FK_DealPaymentDocument_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_DealPaymentDocument_User]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentDocument]'))
alter table dbo.[DealPaymentDocument]  drop constraint FK_DealPaymentDocument_User


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealInitialBalanceCorrection]') AND parent_object_id = OBJECT_ID('dbo.[DealInitialBalanceCorrection]'))
alter table dbo.[DealInitialBalanceCorrection]  drop constraint PFK_DealInitialBalanceCorrection


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealCreditInitialBalanceCorrection]') AND parent_object_id = OBJECT_ID('dbo.[DealCreditInitialBalanceCorrection]'))
alter table dbo.[DealCreditInitialBalanceCorrection]  drop constraint PFK_DealCreditInitialBalanceCorrection


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealDebitInitialBalanceCorrection]') AND parent_object_id = OBJECT_ID('dbo.[DealDebitInitialBalanceCorrection]'))
alter table dbo.[DealDebitInitialBalanceCorrection]  drop constraint PFK_DealDebitInitialBalanceCorrection


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealPayment]') AND parent_object_id = OBJECT_ID('dbo.[DealPayment]'))
alter table dbo.[DealPayment]  drop constraint PFK_DealPayment


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealPaymentFromClient]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentFromClient]'))
alter table dbo.[DealPaymentFromClient]  drop constraint PFK_DealPaymentFromClient


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_DealPaymentToClient]') AND parent_object_id = OBJECT_ID('dbo.[DealPaymentToClient]'))
alter table dbo.[DealPaymentToClient]  drop constraint PFK_DealPaymentToClient


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Deal_DealStageHistory_DealId]') AND parent_object_id = OBJECT_ID('dbo.[DealStageHistory]'))
alter table dbo.[DealStageHistory]  drop constraint FK_Deal_DealStageHistory_DealId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_EconomicAgent_LegalForm]') AND parent_object_id = OBJECT_ID('dbo.[EconomicAgent]'))
alter table dbo.[EconomicAgent]  drop constraint FK_EconomicAgent_LegalForm


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_JuridicalPerson]') AND parent_object_id = OBJECT_ID('dbo.[JuridicalPerson]'))
alter table dbo.[JuridicalPerson]  drop constraint PFK_JuridicalPerson


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_PhysicalPerson]') AND parent_object_id = OBJECT_ID('dbo.[PhysicalPerson]'))
alter table dbo.[PhysicalPerson]  drop constraint PFK_PhysicalPerson


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_Curator]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_SenderStorage]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_SenderStorage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_Sender]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_Sender


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_RecipientStorage]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_RecipientStorage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_Recipient]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_Recipient


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_ValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_ValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_AcceptedBy]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_AcceptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_ShippedBy]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_ShippedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_ReceiptedBy]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybill]'))
alter table dbo.[MovementWaybill]  drop constraint FK_MovementWaybill_ReceiptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybillRow_RecipientArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybillRow]'))
alter table dbo.[MovementWaybillRow]  drop constraint FK_MovementWaybillRow_RecipientArticleAccountingPrice


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybillRow_SenderArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybillRow]'))
alter table dbo.[MovementWaybillRow]  drop constraint FK_MovementWaybillRow_SenderArticleAccountingPrice


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybill_MovementWaybillRow_MovementWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybillRow]'))
alter table dbo.[MovementWaybillRow]  drop constraint FK_MovementWaybill_MovementWaybillRow_MovementWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybillRow_ReceiptWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybillRow]'))
alter table dbo.[MovementWaybillRow]  drop constraint FK_MovementWaybillRow_ReceiptWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybillRow_ValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybillRow]'))
alter table dbo.[MovementWaybillRow]  drop constraint FK_MovementWaybillRow_ValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_MovementWaybillRow_Article]') AND parent_object_id = OBJECT_ID('dbo.[MovementWaybillRow]'))
alter table dbo.[MovementWaybillRow]  drop constraint FK_MovementWaybillRow_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Organization_EconomicAgent]') AND parent_object_id = OBJECT_ID('dbo.[Organization]'))
alter table dbo.[Organization]  drop constraint FK_Organization_EconomicAgent


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_AccountOrganization]') AND parent_object_id = OBJECT_ID('dbo.[AccountOrganization]'))
alter table dbo.[AccountOrganization]  drop constraint PFK_AccountOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_AccountOrganization_Storage]') AND parent_object_id = OBJECT_ID('dbo.AccountOrganizationStorage'))
alter table dbo.AccountOrganizationStorage  drop constraint PFK_AccountOrganization_Storage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Storage_AccountOrganization]') AND parent_object_id = OBJECT_ID('dbo.AccountOrganizationStorage'))
alter table dbo.AccountOrganizationStorage  drop constraint PFK_Storage_AccountOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ContractorOrganization]') AND parent_object_id = OBJECT_ID('dbo.[ContractorOrganization]'))
alter table dbo.[ContractorOrganization]  drop constraint PFK_ContractorOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ClientOrganization]') AND parent_object_id = OBJECT_ID('dbo.[ClientOrganization]'))
alter table dbo.[ClientOrganization]  drop constraint PFK_ClientOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProducerOrganization]') AND parent_object_id = OBJECT_ID('dbo.[ProducerOrganization]'))
alter table dbo.[ProducerOrganization]  drop constraint PFK_ProducerOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProducerOrganization_Manufacturer]') AND parent_object_id = OBJECT_ID('dbo.[ProducerOrganization]'))
alter table dbo.[ProducerOrganization]  drop constraint FK_ProducerOrganization_Manufacturer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProviderOrganization]') AND parent_object_id = OBJECT_ID('dbo.[ProviderOrganization]'))
alter table dbo.[ProviderOrganization]  drop constraint PFK_ProviderOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage_TemplateId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchLifeCycleTemplateStage]'))
alter table dbo.[ProductionOrderBatchLifeCycleTemplateStage]  drop constraint FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage_TemplateId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_ProductionOrderBatch_ProductionOrderId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrder_ProductionOrderBatch_ProductionOrderId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_ReceiptWaybill]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_ReceiptWaybill


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_Curator]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_CurrentStage]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_CurrentStage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_MovedToApprovementStateBy]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_MovedToApprovementStateBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_MovedToApprovedStateBy]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_MovedToApprovedStateBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_ApprovedLineManager]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_ApprovedLineManager


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_FinancialDepartmentApprover]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_FinancialDepartmentApprover


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_SalesDepartmentApprover]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_SalesDepartmentApprover


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_AnalyticalDepartmentApprover]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_AnalyticalDepartmentApprover


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_ApprovedProjectManager]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatch]'))
alter table dbo.[ProductionOrderBatch]  drop constraint FK_ProductionOrderBatch_ApprovedProjectManager


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatchRow_Article]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatchRow_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_ProductionOrderBatchRow_BatchId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatch_ProductionOrderBatchRow_BatchId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatchRow_Currency]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatchRow_Currency


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatchRow_Manufacturer]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatchRow_Manufacturer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatchRow_ProductionCountry]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatchRow_ProductionCountry


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatchRow_ReceiptWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchRow]'))
alter table dbo.[ProductionOrderBatchRow]  drop constraint FK_ProductionOrderBatchRow_ReceiptWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderBatch_ProductionOrderBatchStage_BatchId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderBatchStage]'))
alter table dbo.[ProductionOrderBatchStage]  drop constraint FK_ProductionOrderBatch_ProductionOrderBatchStage_BatchId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_ProductionOrderCustomsDeclaration_ProductionOrderId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderCustomsDeclaration]'))
alter table dbo.[ProductionOrderCustomsDeclaration]  drop constraint FK_ProductionOrder_ProductionOrderCustomsDeclaration_ProductionOrderId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_ProductionOrderExtraExpensesSheet_ProductionOrderId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderExtraExpensesSheet]'))
alter table dbo.[ProductionOrderExtraExpensesSheet]  drop constraint FK_ProductionOrder_ProductionOrderExtraExpensesSheet_ProductionOrderId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderExtraExpensesSheet_Currency]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderExtraExpensesSheet]'))
alter table dbo.[ProductionOrderExtraExpensesSheet]  drop constraint FK_ProductionOrderExtraExpensesSheet_Currency


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderExtraExpensesSheet_CurrencyRate]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderExtraExpensesSheet]'))
alter table dbo.[ProductionOrderExtraExpensesSheet]  drop constraint FK_ProductionOrderExtraExpensesSheet_CurrencyRate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_Storage]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrder]'))
alter table dbo.[ProductionOrder]  drop constraint FK_ProductionOrder_Storage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_Currency]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrder]'))
alter table dbo.[ProductionOrder]  drop constraint FK_ProductionOrder_Currency


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_CurrencyRate]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrder]'))
alter table dbo.[ProductionOrder]  drop constraint FK_ProductionOrder_CurrencyRate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_Producer]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrder]'))
alter table dbo.[ProductionOrder]  drop constraint FK_ProductionOrder_Producer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrder]'))
alter table dbo.[ProductionOrder]  drop constraint FK_ProductionOrder_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_Curator]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrder]'))
alter table dbo.[ProductionOrder]  drop constraint FK_ProductionOrder_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_Contract]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrder]'))
alter table dbo.[ProductionOrder]  drop constraint FK_ProductionOrder_Contract


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_ProductionOrderMaterialsPackage_ProductionOrderId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderMaterialsPackage]'))
alter table dbo.[ProductionOrderMaterialsPackage]  drop constraint FK_ProductionOrder_ProductionOrderMaterialsPackage_ProductionOrderId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_ProductionOrderPayment_ProductionOrderId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderPayment]'))
alter table dbo.[ProductionOrderPayment]  drop constraint FK_ProductionOrder_ProductionOrderPayment_ProductionOrderId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderPlannedPayment_ProductionOrderPayment_ProductionOrderPlannedPaymentId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderPayment]'))
alter table dbo.[ProductionOrderPayment]  drop constraint FK_ProductionOrderPlannedPayment_ProductionOrderPayment_ProductionOrderPlannedPaymentId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderPayment_CurrencyRate]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderPayment]'))
alter table dbo.[ProductionOrderPayment]  drop constraint FK_ProductionOrderPayment_CurrencyRate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProductionOrderCustomsDeclarationPayment]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderCustomsDeclarationPayment]'))
alter table dbo.[ProductionOrderCustomsDeclarationPayment]  drop constraint PFK_ProductionOrderCustomsDeclarationPayment


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationPayment_CustomsDeclarationId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderCustomsDeclarationPayment]'))
alter table dbo.[ProductionOrderCustomsDeclarationPayment]  drop constraint FK_ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationPayment_CustomsDeclarationId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProductionOrderExtraExpensesSheetPayment]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderExtraExpensesSheetPayment]'))
alter table dbo.[ProductionOrderExtraExpensesSheetPayment]  drop constraint PFK_ProductionOrderExtraExpensesSheetPayment


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetPayment_ExtraExpensesSheetId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderExtraExpensesSheetPayment]'))
alter table dbo.[ProductionOrderExtraExpensesSheetPayment]  drop constraint FK_ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetPayment_ExtraExpensesSheetId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProductionOrderTransportSheetPayment]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderTransportSheetPayment]'))
alter table dbo.[ProductionOrderTransportSheetPayment]  drop constraint PFK_ProductionOrderTransportSheetPayment


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderTransportSheet_ProductionOrderTransportSheetPayment_TransportSheetId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderTransportSheetPayment]'))
alter table dbo.[ProductionOrderTransportSheetPayment]  drop constraint FK_ProductionOrderTransportSheet_ProductionOrderTransportSheetPayment_TransportSheetId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_ProductionOrderPlannedPayment_ProductionOrderId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderPlannedPayment]'))
alter table dbo.[ProductionOrderPlannedPayment]  drop constraint FK_ProductionOrder_ProductionOrderPlannedPayment_ProductionOrderId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderPlannedPayment_Currency]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderPlannedPayment]'))
alter table dbo.[ProductionOrderPlannedPayment]  drop constraint FK_ProductionOrderPlannedPayment_Currency


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderPlannedPayment_CurrencyRate]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderPlannedPayment]'))
alter table dbo.[ProductionOrderPlannedPayment]  drop constraint FK_ProductionOrderPlannedPayment_CurrencyRate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrder_ProductionOrderTransportSheet_ProductionOrderId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderTransportSheet]'))
alter table dbo.[ProductionOrderTransportSheet]  drop constraint FK_ProductionOrder_ProductionOrderTransportSheet_ProductionOrderId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderTransportSheet_Currency]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderTransportSheet]'))
alter table dbo.[ProductionOrderTransportSheet]  drop constraint FK_ProductionOrderTransportSheet_Currency


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderTransportSheet_CurrencyRate]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderTransportSheet]'))
alter table dbo.[ProductionOrderTransportSheet]  drop constraint FK_ProductionOrderTransportSheet_CurrencyRate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_ProductionOrderBatch]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_ProductionOrderBatch


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_Curator]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_ReceiptStorage]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_ReceiptStorage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_AccountOrganization]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_AccountOrganization


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_Provider]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_Provider


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_ProviderContract]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_ProviderContract


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_PendingValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_PendingValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_AcceptedBy]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_AcceptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_ReceiptedBy]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_ReceiptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_ApprovedBy]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybill]'))
alter table dbo.[ReceiptWaybill]  drop constraint FK_ReceiptWaybill_ApprovedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybillRow_RecipientArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybillRow]'))
alter table dbo.[ReceiptWaybillRow]  drop constraint FK_ReceiptWaybillRow_RecipientArticleAccountingPrice


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybill_ReceiptWaybillRow_ReceiptWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybillRow]'))
alter table dbo.[ReceiptWaybillRow]  drop constraint FK_ReceiptWaybill_ReceiptWaybillRow_ReceiptWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybillRow_Article]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybillRow]'))
alter table dbo.[ReceiptWaybillRow]  drop constraint FK_ReceiptWaybillRow_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybillRow_PendingValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybillRow]'))
alter table dbo.[ReceiptWaybillRow]  drop constraint FK_ReceiptWaybillRow_PendingValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybillRow_ApprovedValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybillRow]'))
alter table dbo.[ReceiptWaybillRow]  drop constraint FK_ReceiptWaybillRow_ApprovedValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybillRow_ProductionCountry]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybillRow]'))
alter table dbo.[ReceiptWaybillRow]  drop constraint FK_ReceiptWaybillRow_ProductionCountry


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReceiptWaybillRow_Manufacturer]') AND parent_object_id = OBJECT_ID('dbo.[ReceiptWaybillRow]'))
alter table dbo.[ReceiptWaybillRow]  drop constraint FK_ReceiptWaybillRow_Manufacturer


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybillRow_ArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybillRow]'))
alter table dbo.[ReturnFromClientWaybillRow]  drop constraint FK_ReturnFromClientWaybillRow_ArticleAccountingPrice


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_ReturnFromClientWaybillRow_ReturnFromClientWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybillRow]'))
alter table dbo.[ReturnFromClientWaybillRow]  drop constraint FK_ReturnFromClientWaybill_ReturnFromClientWaybillRow_ReturnFromClientWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybillRow_SaleWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybillRow]'))
alter table dbo.[ReturnFromClientWaybillRow]  drop constraint FK_ReturnFromClientWaybillRow_SaleWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybillRow_ReceiptWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybillRow]'))
alter table dbo.[ReturnFromClientWaybillRow]  drop constraint FK_ReturnFromClientWaybillRow_ReceiptWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybillRow_Article]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybillRow]'))
alter table dbo.[ReturnFromClientWaybillRow]  drop constraint FK_ReturnFromClientWaybillRow_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_Team]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_Curator]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_AcceptedBy]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_AcceptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_ReceiptedBy]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_ReceiptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_Recipient]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_Recipient


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_Deal]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_Deal


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_RecipientStorage]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_RecipientStorage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ReturnFromClientWaybill_ReturnFromClientReason]') AND parent_object_id = OBJECT_ID('dbo.[ReturnFromClientWaybill]'))
alter table dbo.[ReturnFromClientWaybill]  drop constraint FK_ReturnFromClientWaybill_ReturnFromClientReason


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_Curator]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybill]'))
alter table dbo.[SaleWaybill]  drop constraint FK_SaleWaybill_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybill]'))
alter table dbo.[SaleWaybill]  drop constraint FK_SaleWaybill_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_AcceptedBy]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybill]'))
alter table dbo.[SaleWaybill]  drop constraint FK_SaleWaybill_AcceptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_Quota]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybill]'))
alter table dbo.[SaleWaybill]  drop constraint FK_SaleWaybill_Quota


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_Deal]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybill]'))
alter table dbo.[SaleWaybill]  drop constraint FK_SaleWaybill_Deal


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_ValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybill]'))
alter table dbo.[SaleWaybill]  drop constraint FK_SaleWaybill_ValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_Team]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybill]'))
alter table dbo.[SaleWaybill]  drop constraint FK_SaleWaybill_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ExpenditureWaybill]') AND parent_object_id = OBJECT_ID('dbo.[ExpenditureWaybill]'))
alter table dbo.[ExpenditureWaybill]  drop constraint PFK_ExpenditureWaybill


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ExpenditureWaybill_SenderStorage]') AND parent_object_id = OBJECT_ID('dbo.[ExpenditureWaybill]'))
alter table dbo.[ExpenditureWaybill]  drop constraint FK_ExpenditureWaybill_SenderStorage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ExpenditureWaybill_ShippedBy]') AND parent_object_id = OBJECT_ID('dbo.[ExpenditureWaybill]'))
alter table dbo.[ExpenditureWaybill]  drop constraint FK_ExpenditureWaybill_ShippedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Employee_Post]') AND parent_object_id = OBJECT_ID('dbo.[Employee]'))
alter table dbo.[Employee]  drop constraint FK_Employee_Post


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Employee_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[Employee]'))
alter table dbo.[Employee]  drop constraint FK_Employee_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Role_PermissionDistribution_RoleId]') AND parent_object_id = OBJECT_ID('dbo.[PermissionDistribution]'))
alter table dbo.[PermissionDistribution]  drop constraint FK_Role_PermissionDistribution_RoleId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Role_User]') AND parent_object_id = OBJECT_ID('dbo.UserRole'))
alter table dbo.UserRole  drop constraint PFK_Role_User


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_User_Role]') AND parent_object_id = OBJECT_ID('dbo.UserRole'))
alter table dbo.UserRole  drop constraint PFK_User_Role


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Team_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[Team]'))
alter table dbo.[Team]  drop constraint FK_Team_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Team_Deal]') AND parent_object_id = OBJECT_ID('dbo.TeamDeal'))
alter table dbo.TeamDeal  drop constraint PFK_Team_Deal


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Deal_Team]') AND parent_object_id = OBJECT_ID('dbo.TeamDeal'))
alter table dbo.TeamDeal  drop constraint PFK_Deal_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Team_User]') AND parent_object_id = OBJECT_ID('dbo.UserTeam'))
alter table dbo.UserTeam  drop constraint PFK_Team_User


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_User_Team]') AND parent_object_id = OBJECT_ID('dbo.UserTeam'))
alter table dbo.UserTeam  drop constraint PFK_User_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Team_Storage]') AND parent_object_id = OBJECT_ID('dbo.TeamStorage'))
alter table dbo.TeamStorage  drop constraint PFK_Team_Storage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Storage_Team]') AND parent_object_id = OBJECT_ID('dbo.TeamStorage'))
alter table dbo.TeamStorage  drop constraint PFK_Storage_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_Team_ProductionOrder]') AND parent_object_id = OBJECT_ID('dbo.TeamProductionOrder'))
alter table dbo.TeamProductionOrder  drop constraint PFK_Team_ProductionOrder


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ProductionOrder_Team]') AND parent_object_id = OBJECT_ID('dbo.TeamProductionOrder'))
alter table dbo.TeamProductionOrder  drop constraint PFK_ProductionOrder_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_User_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[User]'))
alter table dbo.[User]  drop constraint FK_User_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_User_Blocker]') AND parent_object_id = OBJECT_ID('dbo.[User]'))
alter table dbo.[User]  drop constraint FK_User_Blocker


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_UserToEmployee]') AND parent_object_id = OBJECT_ID('dbo.[User]'))
alter table dbo.[User]  drop constraint FK_UserToEmployee


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Storage_StorageSection_StorageId]') AND parent_object_id = OBJECT_ID('dbo.[StorageSection]'))
alter table dbo.[StorageSection]  drop constraint FK_Storage_StorageSection_StorageId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionItem_ExecutionState]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionItem]'))
alter table dbo.[TaskExecutionItem]  drop constraint FK_TaskExecutionItem_ExecutionState


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionItem_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionItem]'))
alter table dbo.[TaskExecutionItem]  drop constraint FK_TaskExecutionItem_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_TaskExecutionItem_TaskId]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionItem]'))
alter table dbo.[TaskExecutionItem]  drop constraint FK_Task_TaskExecutionItem_TaskId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskExecutionItem_TaskType]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionItem]'))
alter table dbo.[TaskExecutionItem]  drop constraint FK_TaskExecutionItem_TaskType


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_TaskType_TaskExecutionState_TaskTypeId]') AND parent_object_id = OBJECT_ID('dbo.[TaskExecutionState]'))
alter table dbo.[TaskExecutionState]  drop constraint FK_TaskType_TaskExecutionState_TaskTypeId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_Contractor]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_Contractor


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_Deal]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_Deal


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_ExecutedBy]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_ExecutedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_Priority]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_Priority


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_ProductionOrder]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_ProductionOrder


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_ExecutionState]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_ExecutionState


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Task_Type]') AND parent_object_id = OBJECT_ID('dbo.[Task]'))
alter table dbo.[Task]  drop constraint FK_Task_Type


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybill_ChangeOwnerWaybillRow_ChangeOwnerWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybillRow]'))
alter table dbo.[ChangeOwnerWaybillRow]  drop constraint FK_ChangeOwnerWaybill_ChangeOwnerWaybillRow_ChangeOwnerWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybillRow_ReceiptWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybillRow]'))
alter table dbo.[ChangeOwnerWaybillRow]  drop constraint FK_ChangeOwnerWaybillRow_ReceiptWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybillRow_ArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybillRow]'))
alter table dbo.[ChangeOwnerWaybillRow]  drop constraint FK_ChangeOwnerWaybillRow_ArticleAccountingPrice


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybillRow_ValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybillRow]'))
alter table dbo.[ChangeOwnerWaybillRow]  drop constraint FK_ChangeOwnerWaybillRow_ValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ChangeOwnerWaybillRow_Article]') AND parent_object_id = OBJECT_ID('dbo.[ChangeOwnerWaybillRow]'))
alter table dbo.[ChangeOwnerWaybillRow]  drop constraint FK_ChangeOwnerWaybillRow_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybill_SaleWaybillRow_SaleWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybillRow]'))
alter table dbo.[SaleWaybillRow]  drop constraint FK_SaleWaybill_SaleWaybillRow_SaleWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybillRow_ValueAddedTax]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybillRow]'))
alter table dbo.[SaleWaybillRow]  drop constraint FK_SaleWaybillRow_ValueAddedTax


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_SaleWaybillRow_Article]') AND parent_object_id = OBJECT_ID('dbo.[SaleWaybillRow]'))
alter table dbo.[SaleWaybillRow]  drop constraint FK_SaleWaybillRow_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[PFK_ExpenditureWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[ExpenditureWaybillRow]'))
alter table dbo.[ExpenditureWaybillRow]  drop constraint PFK_ExpenditureWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ExpenditureWaybillRow_ReceiptWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[ExpenditureWaybillRow]'))
alter table dbo.[ExpenditureWaybillRow]  drop constraint FK_ExpenditureWaybillRow_ReceiptWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ExpenditureWaybillRow_SenderArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('dbo.[ExpenditureWaybillRow]'))
alter table dbo.[ExpenditureWaybillRow]  drop constraint FK_ExpenditureWaybillRow_SenderArticleAccountingPrice


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_Curator]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybill]'))
alter table dbo.[WriteoffWaybill]  drop constraint FK_WriteoffWaybill_Curator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_Sender]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybill]'))
alter table dbo.[WriteoffWaybill]  drop constraint FK_WriteoffWaybill_Sender


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_SenderStorage]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybill]'))
alter table dbo.[WriteoffWaybill]  drop constraint FK_WriteoffWaybill_SenderStorage


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_WriteoffReason]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybill]'))
alter table dbo.[WriteoffWaybill]  drop constraint FK_WriteoffWaybill_WriteoffReason


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybill]'))
alter table dbo.[WriteoffWaybill]  drop constraint FK_WriteoffWaybill_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_AcceptedBy]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybill]'))
alter table dbo.[WriteoffWaybill]  drop constraint FK_WriteoffWaybill_AcceptedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_WrittenoffBy]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybill]'))
alter table dbo.[WriteoffWaybill]  drop constraint FK_WriteoffWaybill_WrittenoffBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybill_WriteoffWaybillRow_WriteoffWaybillId]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybillRow]'))
alter table dbo.[WriteoffWaybillRow]  drop constraint FK_WriteoffWaybill_WriteoffWaybillRow_WriteoffWaybillId


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybillRow_SenderArticleAccountingPrice]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybillRow]'))
alter table dbo.[WriteoffWaybillRow]  drop constraint FK_WriteoffWaybillRow_SenderArticleAccountingPrice


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybillRow_ReceiptWaybillRow]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybillRow]'))
alter table dbo.[WriteoffWaybillRow]  drop constraint FK_WriteoffWaybillRow_ReceiptWaybillRow


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_WriteoffWaybillRow_Article]') AND parent_object_id = OBJECT_ID('dbo.[WriteoffWaybillRow]'))
alter table dbo.[WriteoffWaybillRow]  drop constraint FK_WriteoffWaybillRow_Article


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderMaterialsPackageDocument_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderMaterialsPackageDocument]'))
alter table dbo.[ProductionOrderMaterialsPackageDocument]  drop constraint FK_ProductionOrderMaterialsPackageDocument_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_ProductionOrderMaterialsPackage_ProductionOrderMaterialsPackageDocument_MaterialsPackageId]') AND parent_object_id = OBJECT_ID('dbo.[ProductionOrderMaterialsPackageDocument]'))
alter table dbo.[ProductionOrderMaterialsPackageDocument]  drop constraint FK_ProductionOrderMaterialsPackage_ProductionOrderMaterialsPackageDocument_MaterialsPackageId


    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AccountingPriceList]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AccountingPriceList]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.AccountingPriceListStorage') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.AccountingPriceListStorage

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AccountingPriceListWaybillTaking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AccountingPriceListWaybillTaking]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AccountOrganizationDocumentNumbers]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AccountOrganizationDocumentNumbers]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ArticleAccountingPrice]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ArticleAccountingPrice]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ArticleCertificate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ArticleCertificate]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ArticleGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ArticleGroup]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Article]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Article]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Bank]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Bank]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ForeignBank]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ForeignBank]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[RussianBank]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[RussianBank]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[BankAccount]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[BankAccount]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[RussianBankAccount]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[RussianBankAccount]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ForeignBankAccount]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ForeignBankAccount]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[BaseTaskHistoryItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[BaseTaskHistoryItem]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[TaskExecutionHistoryItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[TaskExecutionHistoryItem]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[TaskHistoryItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[TaskHistoryItem]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ChangeOwnerWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ChangeOwnerWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ClientRegion]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ClientRegion]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ClientServiceProgram]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ClientServiceProgram]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ClientType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ClientType]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Contract]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Contract]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.ContractorContract') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.ContractorContract

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ClientContract]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ClientContract]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProducerContract]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProducerContract]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProviderContract]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProviderContract]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Contractor]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Contractor]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.ContractorOrganizationContractor') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.ContractorOrganizationContractor

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Client]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Client]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Producer]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Producer]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.ProducerManufacturer') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.ProducerManufacturer

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Provider]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Provider]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Country]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Country]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Currency]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Currency]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[CurrencyRate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[CurrencyRate]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Deal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Deal]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.DealDealQuota') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.DealDealQuota

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPaymentDocumentDistribution]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPaymentDocumentDistribution]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPaymentDocumentDistributionToSaleWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPaymentDocumentDistributionToSaleWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPaymentDocument]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPaymentDocument]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealInitialBalanceCorrection]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealInitialBalanceCorrection]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealCreditInitialBalanceCorrection]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealCreditInitialBalanceCorrection]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealDebitInitialBalanceCorrection]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealDebitInitialBalanceCorrection]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPayment]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPaymentFromClient]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPaymentFromClient]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealPaymentToClient]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealPaymentToClient]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealQuota]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealQuota]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DealStageHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DealStageHistory]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[DefaultProductionOrderBatchStage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[DefaultProductionOrderBatchStage]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[EconomicAgent]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[EconomicAgent]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[JuridicalPerson]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[JuridicalPerson]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[PhysicalPerson]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[PhysicalPerson]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AcceptedArticleRevaluationIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AcceptedArticleRevaluationIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AcceptedReturnFromClientIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AcceptedReturnFromClientIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AcceptedSaleIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AcceptedSaleIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ArticleAccountingPriceIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ArticleAccountingPriceIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ArticleMovementFactualFinancialIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ArticleMovementFactualFinancialIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ArticleMovementOperationCountIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ArticleMovementOperationCountIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ExactArticleAvailabilityIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ExactArticleAvailabilityIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ExactArticleRevaluationIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ExactArticleRevaluationIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[IncomingAcceptedArticleAvailabilityIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[IncomingAcceptedArticleAvailabilityIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[OutgoingAcceptedFromExactArticleAvailabilityIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[OutgoingAcceptedFromExactArticleAvailabilityIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AcceptedPurchaseIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AcceptedPurchaseIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ApprovedPurchaseIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ApprovedPurchaseIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReceiptedReturnFromClientIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReceiptedReturnFromClientIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReturnFromClientBySaleAcceptanceDateIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReturnFromClientBySaleAcceptanceDateIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReturnFromClientBySaleShippingDateIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReturnFromClientBySaleShippingDateIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ShippedSaleIndicator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ShippedSaleIndicator]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[LegalForm]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[LegalForm]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[LogItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[LogItem]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Manufacturer]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Manufacturer]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[MeasureUnit]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[MeasureUnit]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[MovementWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[MovementWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[MovementWaybillRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[MovementWaybillRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Organization]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Organization]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[AccountOrganization]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[AccountOrganization]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.AccountOrganizationStorage') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.AccountOrganizationStorage

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ContractorOrganization]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ContractorOrganization]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ClientOrganization]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ClientOrganization]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProducerOrganization]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProducerOrganization]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProviderOrganization]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProviderOrganization]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderBatchLifeCycleTemplate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderBatchLifeCycleTemplate]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderBatchLifeCycleTemplateStage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderBatchLifeCycleTemplateStage]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderBatch]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderBatch]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderBatchRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderBatchRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderBatchStage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderBatchStage]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderCustomsDeclaration]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderCustomsDeclaration]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderExtraExpensesSheet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderExtraExpensesSheet]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrder]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrder]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderMaterialsPackage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderMaterialsPackage]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderPayment]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderCustomsDeclarationPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderCustomsDeclarationPayment]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderExtraExpensesSheetPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderExtraExpensesSheetPayment]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderTransportSheetPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderTransportSheetPayment]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderPlannedPayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderPlannedPayment]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderTransportSheet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderTransportSheet]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProviderType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProviderType]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReceiptWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReceiptWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReceiptWaybillRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReceiptWaybillRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReturnFromClientReason]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReturnFromClientReason]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReturnFromClientWaybillRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReturnFromClientWaybillRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ReturnFromClientWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ReturnFromClientWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[SaleWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[SaleWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ExpenditureWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ExpenditureWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Employee]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Employee]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[EmployeePost]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[EmployeePost]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[PermissionDistribution]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[PermissionDistribution]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Role]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Role]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.UserRole') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.UserRole

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Setting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Setting]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Team]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Team]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.TeamDeal') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.TeamDeal

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.UserTeam') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.UserTeam

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.TeamStorage') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.TeamStorage

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.TeamProductionOrder') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.TeamProductionOrder

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[User]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Storage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Storage]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[StorageSection]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[StorageSection]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[TaskExecutionItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[TaskExecutionItem]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[TaskExecutionState]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[TaskExecutionState]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Task]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Task]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[TaskPriority]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[TaskPriority]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[TaskType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[TaskType]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[Trademark]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[Trademark]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ValueAddedTax]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ValueAddedTax]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ChangeOwnerWaybillRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ChangeOwnerWaybillRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[SaleWaybillRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[SaleWaybillRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ExpenditureWaybillRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ExpenditureWaybillRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[WaybillRowArticleMovement]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[WaybillRowArticleMovement]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[WriteoffReason]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[WriteoffReason]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[WriteoffWaybill]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[WriteoffWaybill]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[WriteoffWaybillRow]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[WriteoffWaybillRow]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[ProductionOrderMaterialsPackageDocument]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[ProductionOrderMaterialsPackageDocument]

    create table dbo.[AccountingPriceList] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(25) not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       ReasonId TINYINT not null,
       ReasonReceiptWaybillId UNIQUEIDENTIFIER null,
       ReasonReceiptWaybillNumber VARCHAR(25) null,
       ReasonReceiptWaybillDate DATETIME null,
       AccountingPriceListStateId TINYINT not null,
       CreationDate DATETIME not null,
       AcceptanceDate DATETIME null,
       IsRevaluationOnStartCalculated BIT not null,
       IsRevaluationOnEndCalculated BIT not null,
       DeletionDate DATETIME null,
       CuratorId INT not null,
       AccountingPriceCalcRuleTypeId TINYINT not null,
       AccountingPriceCalcRuleByPurchaseCostPurchaseCostDeterminationRuleTypeId INT null,
       AccountingPriceCalcRuleByPurchaseCostMarkupPercentDeterminationRuleValue DECIMAL(6, 2) null,
       AccountingPriceCalcRuleByPurchaseCostMarkupPercentDeterminationRuleTypeId INT null,
       AccountingPriceCalcRuleByCurrentAccountingPriceMarkupPercentValue DECIMAL(6, 2) null,
       AccountingPriceCalcRuleByCurrentAccountingPriceTypeId TINYINT null,
       AccountingPriceCalcRuleByCurrentAccountingPriceStorageTypeId TINYINT null,
       AccountingPriceCalcRuleByCurrentAccountingPriceStorageId SMALLINT null,
       LastDigitCalcRuleTypeId TINYINT not null,
       LastDigitCalcRuleLastDigit TINYINT null,
       LastDigitCalcRuleDecimals SMALLINT null,
       LastDigitCalcRuleStorageId SMALLINT null,
       primary key (Id)
    )

    create table dbo.AccountingPriceListStorage (
        AccountingPriceListId UNIQUEIDENTIFIER not null,
       StorageId SMALLINT not null,
       primary key (AccountingPriceListId, StorageId)
    )

    create table dbo.[AccountingPriceListWaybillTaking] (
        Id UNIQUEIDENTIFIER not null,
       TakingDate DATETIME not null,
       IsWaybillRowIncoming BIT not null,
       ArticleAccountingPriceId UNIQUEIDENTIFIER not null,
       WaybillTypeId TINYINT not null,
       WaybillRowId UNIQUEIDENTIFIER not null,
       ArticleId INT not null,
       StorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       AccountingPrice DECIMAL(18, 2) not null,
       IsOnAccountingPriceListStart BIT not null,
       Count DECIMAL(18, 6) not null,
       RevaluationDate DATETIME null,
       primary key (Id)
    )

    create table dbo.[AccountOrganizationDocumentNumbers] (
        Id INT IDENTITY NOT NULL,
       Year INT not null,
       ReceiptWaybillLastNumber DECIMAL(19,5) not null,
       ChangeOwnerWaybillLastNumber DECIMAL(19,5) not null,
       ExpenditureWaybillLastNumber DECIMAL(19,5) not null,
       MovementWaybillLastNumber DECIMAL(19,5) not null,
       ReturnFromClientWaybillLastNumber DECIMAL(19,5) not null,
       WriteoffWaybillLastNumber DECIMAL(19,5) not null,
       AccountOrganizationId INT not null,
       primary key (Id)
    )

    create table dbo.[ArticleAccountingPrice] (
        Id UNIQUEIDENTIFIER not null,
       AccountingPrice DECIMAL(18, 2) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       ErrorAccountingPriceCalculation BIT null,
       ErrorLastDigitCalculation BIT null,
       IsOverlappedOnEnd BIT not null,
       OrdinalNumber INT not null,
       AccountingPriceListId UNIQUEIDENTIFIER not null,
       ArticleId INT not null,
       primary key (Id)
    )

    create table dbo.[ArticleCertificate] (
        Id INT IDENTITY NOT NULL,
       Name VARCHAR(500) not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       primary key (Id)
    )

    create table dbo.[ArticleGroup] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       Comment VARCHAR(4000) not null,
       MarkupPercent DECIMAL(6, 2) not null,
       SalaryPercent DECIMAL(4, 2) not null,
       NameFor1C VARCHAR(200) not null,
       ParentId SMALLINT null,
       primary key (Id)
    )

    create table dbo.[Article] (
        Id INT IDENTITY NOT NULL,
       FullName VARCHAR(200) not null,
       ShortName VARCHAR(200) not null,
       Number VARCHAR(30) not null unique,
       ManufacturerNumber VARCHAR(30) not null,
       PackSize DECIMAL(12, 6) not null,
       PackWeight DECIMAL(8, 3) not null,
       PackHeight INT not null,
       PackLength INT not null,
       PackWidth INT not null,
       PackVolume DECIMAL(15, 6) not null,
       IsObsolete BIT not null,
       IsSalaryPercentFromGroup BIT not null,
       SalaryPercent DECIMAL(4, 2) not null,
       MarkupPercent DECIMAL(6, 2) not null,
       Comment VARCHAR(4000) not null,
       ArticleGroupId SMALLINT not null,
       TradeMarkId SMALLINT null,
       ManufacturerId SMALLINT null,
       ProductionCountryId SMALLINT null,
       MeasureUnitId SMALLINT not null,
       CertificateId INT null,
       primary key (Id)
    )

    create table dbo.[Bank] (
        Id INT IDENTITY NOT NULL,
       Address VARCHAR(250) not null,
       Name VARCHAR(250) not null unique,
       DeletionDate DATETIME null,
       primary key (Id)
    )

    create table dbo.[ForeignBank] (
        Id INT not null,
       ClearingCode VARCHAR(9) not null,
       ClearingCodeTypeId TINYINT null,
       SWIFT VARCHAR(11) not null unique,
       primary key (Id)
    )

    create table dbo.[RussianBank] (
        Id INT not null,
       CorAccount VARCHAR(20) not null,
       BIC VARCHAR(9) not null unique,
       primary key (Id)
    )

    create table dbo.[BankAccount] (
        Id INT IDENTITY NOT NULL,
       IsMaster BIT not null,
       Number VARCHAR(34) not null,
       CreationDate DATETIME not null,
       CurrencyId SMALLINT not null,
       BankId INT not null,
       primary key (Id)
    )

    create table dbo.[RussianBankAccount] (
        Id INT not null,
       DeletionDate DATETIME null,
       OrganizationId INT not null,
       primary key (Id)
    )

    create table dbo.[ForeignBankAccount] (
        Id INT not null,
       DeletionDate DATETIME null,
       IBAN VARCHAR(34) not null,
       OrganizationId INT not null,
       primary key (Id)
    )

    create table dbo.[BaseTaskHistoryItem] (
        Id INT IDENTITY NOT NULL,
       CreationDate DATETIME not null,
       CreatedById INT not null,
       primary key (Id)
    )

    create table dbo.[TaskExecutionHistoryItem] (
        Id INT not null,
       IsDateChanged BIT not null,
       IsDeletionDateChanged BIT not null,
       IsResultDescriptionChanged BIT not null,
       IsSpentTimeChanged BIT not null,
       IsCompletionPercentageChanged BIT not null,
       IsTaskExecutionStateChanged BIT not null,
       IsTaskTypeChanged BIT not null,
       Date DATETIME null,
       DeletionDate DATETIME null,
       ResultDescription VARCHAR(4000) not null,
       SpentTime INT null,
       CompletionPercentage TINYINT null,
       TaskExecutionItemId INT not null,
       TaskId INT not null,
       TaskExecutionStateId SMALLINT null,
       TaskTypeId SMALLINT null,
       primary key (Id)
    )

    create table dbo.[TaskHistoryItem] (
        Id INT not null,
       IsContractorChanged BIT not null,
       IsDeadLineChanged BIT not null,
       IsDealChanged BIT not null,
       IsDeletionDateChanged BIT not null,
       IsDescriptionChanged BIT not null,
       IsExecutedByChanged BIT not null,
       IsFactualCompletionDateChanged BIT not null,
       IsFactualSpentTimeChanged BIT not null,
       IsTaskPriorityChanged BIT not null,
       IsProductionOrderChanged BIT not null,
       IsStartDateChanged BIT not null,
       IsTopicChanged BIT not null,
       IsTaskTypeChanged BIT not null,
       IsTaskExecutionStateChanged BIT not null,
       Deadline DATETIME null,
       DeletionDate DATETIME null,
       Description VARCHAR(8000) not null,
       FactualCompletionDate DATETIME null,
       FactualSpentTime INT null,
       StartDate DATETIME null,
       Topic VARCHAR(200) not null,
       TaskId INT not null,
       ContractorId INT null,
       DealId INT null,
       ExecutedById INT null,
       TaskPriorityId SMALLINT null,
       ProductionOrderId UNIQUEIDENTIFIER null,
       TaskTypeId SMALLINT null,
       TaskExecutionStateId SMALLINT null,
       TaskExecutionItemId INT null,
       primary key (Id)
    )

    create table dbo.[ChangeOwnerWaybill] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(25) not null,
       Date DATETIME not null,
       Year INT not null,
       Comment VARCHAR(4000) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       AcceptanceDate DATETIME null,
       ChangeOwnerDate DATETIME null,
       ChangeOwnerWaybillStateId TINYINT not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       ChangeOwnerWaybillCuratorId INT not null,
       ChangeOwnerWaybillStorageId SMALLINT not null,
       ChangeOwnerWaybillSenderId INT not null,
       ChangeOwnerWaybillRecipientId INT not null,
       ChangeOwnerWaybillValueAddedTaxId SMALLINT not null,
       ChangeOwnerWaybillCreatedById INT not null,
       ChangeOwnerWaybillAcceptedById INT null,
       ChangeOwnerWaybillChangedOwnerById INT null,
       primary key (Id)
    )

    create table dbo.[ClientRegion] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[ClientServiceProgram] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[ClientType] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[Contract] (
        Id SMALLINT IDENTITY NOT NULL,
       Number VARCHAR(50) not null,
       Date DATETIME not null,
       Name VARCHAR(200) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       Comment VARCHAR(4000) not null,
       AccountOrganizationId INT not null,
       ContractorOrganizationId INT not null,
       primary key (Id)
    )

    create table dbo.ContractorContract (
        ContractId SMALLINT not null,
       ContractorId INT not null,
       primary key (ContractorId, ContractId)
    )

    create table dbo.[ClientContract] (
        Id SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[ProducerContract] (
        Id SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[ProviderContract] (
        Id SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[Contractor] (
        Id INT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       ContractorTypeId TINYINT not null,
       Rating TINYINT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       primary key (Id)
    )

    create table dbo.ContractorOrganizationContractor (
        ContractorId INT not null,
       ContractorOrganizationId INT not null,
       primary key (ContractorOrganizationId, ContractorId)
    )

    create table dbo.[Client] (
        Id INT not null,
       LoyaltyId TINYINT not null,
       ManualBlockingDate DATETIME null,
       FactualAddress VARCHAR(250) not null,
       ContactPhone VARCHAR(20) not null,
       ClientTypeId SMALLINT not null,
       RegionId SMALLINT not null,
       ServiceProgramId SMALLINT not null,
       ManualBlockerId INT null,
       primary key (Id)
    )

    create table dbo.[Producer] (
        Id INT not null,
       VATNo VARCHAR(255) not null,
       ManagerName VARCHAR(100) not null,
       Email VARCHAR(100) not null,
       MobilePhone VARCHAR(100) not null,
       Skype VARCHAR(100) not null,
       MSN VARCHAR(100) not null,
       CuratorId INT not null,
       primary key (Id)
    )

    create table dbo.ProducerManufacturer (
        ProducerId INT not null,
       ManufacturerId SMALLINT not null,
       primary key (ProducerId, ManufacturerId)
    )

    create table dbo.[Provider] (
        Id INT not null,
       ProviderReliabilityId TINYINT not null,
       ProviderTypeId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[Country] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       NumericCode VARCHAR(3) not null,
       primary key (Id)
    )

    create table dbo.[Currency] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(20) not null,
       NumericCode VARCHAR(3) not null,
       LiteralCode VARCHAR(3) not null,
       DeletionDate DATETIME null,
       primary key (Id)
    )

    create table dbo.[CurrencyRate] (
        Id INT IDENTITY NOT NULL,
       CreationDate DATETIME not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       Rate DECIMAL(18, 6) not null,
       PreviousCurrencyRateId INT null,
       CurrencyId SMALLINT not null,
       BaseCurrencyId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[Deal] (
        Id INT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       DealStageId TINYINT not null,
       CreationDate DATETIME not null,
       StartDate DATETIME not null,
       StageDate DATETIME not null,
       ExpectedBudget DECIMAL(18, 2) null,
       IsClosed BIT not null,
       Comment VARCHAR(4000) not null,
       ClientId INT not null,
       ClientContractId SMALLINT null,
       CuratorId INT not null,
       primary key (Id)
    )

    create table dbo.DealDealQuota (
        DealId INT not null,
       DealQuotaId INT not null,
       primary key (DealId, DealQuotaId)
    )

    create table dbo.[DealPaymentDocumentDistribution] (
        Id UNIQUEIDENTIFIER not null,
       Sum DECIMAL(18, 2) not null,
       CreationDate DATETIME not null,
       DistributionDate DATETIME not null,
       OrdinalNumber INT not null,
       SourceDealPaymentDocumentId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument] (
        Id UNIQUEIDENTIFIER not null,
       DestinationDealPaymentDocumentId UNIQUEIDENTIFIER not null,
       SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] (
        Id UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       ReturnFromClientWaybillId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[DealPaymentDocumentDistributionToSaleWaybill] (
        Id UNIQUEIDENTIFIER not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       SourceDistributionToReturnFromClientWaybillId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[DealPaymentDocument] (
        Id UNIQUEIDENTIFIER not null,
       Date DATETIME not null,
       Sum DECIMAL(18, 2) not null,
       DistributedSum DECIMAL(18, 2) not null,
       IsFullyDistributed BIT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       DealPaymentDocumentTypeId TINYINT not null,
       DealId INT not null,
       TeamId SMALLINT not null,
       UserId INT not null,
       primary key (Id)
    )

    create table dbo.[DealInitialBalanceCorrection] (
        Id UNIQUEIDENTIFIER not null,
       CorrectionReason VARCHAR(140) not null,
       primary key (Id)
    )

    create table dbo.[DealCreditInitialBalanceCorrection] (
        Id UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[DealDebitInitialBalanceCorrection] (
        Id UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[DealPayment] (
        Id UNIQUEIDENTIFIER not null,
       PaymentDocumentNumber VARCHAR(50) not null,
       DealPaymentFormId TINYINT not null,
       primary key (Id)
    )

    create table dbo.[DealPaymentFromClient] (
        Id UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[DealPaymentToClient] (
        Id UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[DealQuota] (
        Id INT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       IsPrepayment BIT not null,
       DiscountPercent DECIMAL(5, 2) not null,
       PostPaymentDays SMALLINT null,
       CreditLimitSum DECIMAL(18, 2) null,
       primary key (Id)
    )

    create table dbo.[DealStageHistory] (
        Id INT IDENTITY NOT NULL,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       DealStageId TINYINT not null,
       DealId INT not null,
       primary key (Id)
    )

    create table dbo.[DefaultProductionOrderBatchStage] (
        Id INT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       ProductionOrderBatchStageTypeId TINYINT not null,
       primary key (Id)
    )

    create table dbo.[EconomicAgent] (
        Id INT IDENTITY NOT NULL,
       EconomicAgentTypeId TINYINT not null,
       LegalFormId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[JuridicalPerson] (
        Id INT not null,
       INN VARCHAR(10) not null,
       KPP VARCHAR(9) not null,
       OGRN VARCHAR(13) not null,
       OKPO VARCHAR(10) not null,
       DirectorName VARCHAR(100) not null,
       DirectorPost VARCHAR(100) not null,
       MainBookkeeperName VARCHAR(100) not null,
       CashierName VARCHAR(100) not null,
       primary key (Id)
    )

    create table dbo.[PhysicalPerson] (
        Id INT not null,
       INN VARCHAR(12) not null,
       OGRNIP VARCHAR(15) not null,
       OwnerName VARCHAR(100) not null,
       PassportSeries VARCHAR(10) not null,
       PassportNumber VARCHAR(10) not null,
       PassportIssueDate DATETIME null,
       PassportIssuedBy VARCHAR(200) not null,
       PassportDepartmentCode VARCHAR(10) not null,
       primary key (Id)
    )

    create table dbo.[AcceptedArticleRevaluationIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       RevaluationSum DECIMAL(18, 2) not null,
       PreviousId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[AcceptedReturnFromClientIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       SaleWaybillCuratorId INT not null,
       ReturnFromClientWaybillCuratorId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT not null,
       ClientId INT not null,
       DealId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       BatchId UNIQUEIDENTIFIER not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       ReturnedCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[AcceptedSaleIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT not null,
       ClientId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       DealId INT not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       SoldCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[ArticleAccountingPriceIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       ArticleId INT not null,
       AccountingPrice DECIMAL(18, 2) not null,
       AccountingPriceListId UNIQUEIDENTIFIER not null,
       ArticleAccountingPriceId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[ArticleMovementFactualFinancialIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       RecipientId INT null,
       RecipientStorageId SMALLINT null,
       SenderId INT null,
       SenderStorageId SMALLINT null,
       PreviousId UNIQUEIDENTIFIER null,
       WaybillId UNIQUEIDENTIFIER not null,
       ArticleMovementOperationType TINYINT not null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       primary key (Id)
    )

    create table dbo.[ArticleMovementOperationCountIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       PreviousId UNIQUEIDENTIFIER null,
       ArticleMovementOperationType TINYINT not null,
       StorageId SMALLINT not null,
       Count INT not null,
       primary key (Id)
    )

    create table dbo.[ExactArticleAvailabilityIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       PurchaseCost DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       PreviousId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[ExactArticleRevaluationIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       RevaluationSum DECIMAL(18, 2) not null,
       PreviousId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[IncomingAcceptedArticleAvailabilityIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       PurchaseCost DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       PreviousId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[OutgoingAcceptedFromExactArticleAvailabilityIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       PurchaseCost DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       PreviousId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       PurchaseCost DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       PreviousId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[AcceptedPurchaseIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ContractorId INT not null,
       ContractorOrganizationId INT not null,
       ContractId SMALLINT null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[ApprovedPurchaseIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ContractorId INT not null,
       ContractorOrganizationId INT not null,
       ContractId SMALLINT null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       Count DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[ReceiptedReturnFromClientIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       SaleWaybillCuratorId INT not null,
       ReturnFromClientWaybillCuratorId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT not null,
       ClientId INT not null,
       DealId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       BatchId UNIQUEIDENTIFIER not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       ReturnedCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[ReturnFromClientBySaleAcceptanceDateIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       SaleWaybillCuratorId INT not null,
       ReturnFromClientWaybillCuratorId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT not null,
       ClientId INT not null,
       DealId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       BatchId UNIQUEIDENTIFIER not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       ReturnedCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[ReturnFromClientBySaleShippingDateIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       SaleWaybillCuratorId INT not null,
       ReturnFromClientWaybillCuratorId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT not null,
       ClientId INT not null,
       DealId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       BatchId UNIQUEIDENTIFIER not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       ReturnedCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[ShippedSaleIndicator] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       StorageId SMALLINT not null,
       UserId INT not null,
       ContractorId INT not null,
       ClientOrganizationId INT not null,
       TeamId SMALLINT not null,
       ClientId INT not null,
       AccountOrganizationId INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       DealId INT not null,
       PreviousId UNIQUEIDENTIFIER null,
       PurchaseCostSum DECIMAL(18, 6) not null,
       AccountingPriceSum DECIMAL(18, 2) not null,
       SalePriceSum DECIMAL(18, 2) not null,
       SoldCount DECIMAL(18, 6) not null,
       primary key (Id)
    )

    create table dbo.[LegalForm] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(15) not null unique,
       EconomicAgentTypeId TINYINT not null,
       primary key (Id)
    )

    create table dbo.[LogItem] (
        Id BIGINT IDENTITY NOT NULL,
       Time DATETIME not null,
       UserId INT null,
       Url VARCHAR(4000) not null,
       UserMessage VARCHAR(4000) not null,
       SystemMessage VARCHAR(4000) not null,
       primary key (Id)
    )

    create table dbo.[Manufacturer] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[MeasureUnit] (
        Id SMALLINT IDENTITY NOT NULL,
       NumericCode VARCHAR(3) not null,
       FullName VARCHAR(20) not null unique,
       ShortName VARCHAR(7) not null unique,
       Comment VARCHAR(500) not null,
       Scale TINYINT not null,
       primary key (Id)
    )

    create table dbo.[MovementWaybill] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(25) not null,
       Date DATETIME not null,
       Year INT not null,
       MovementWaybillStateId TINYINT not null,
       SenderAccountingPriceSum DECIMAL(18, 2) null,
       RecipientAccountingPriceSum DECIMAL(18, 2) null,
       Comment VARCHAR(4000) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       ShippingDate DATETIME null,
       ReceiptDate DATETIME null,
       AcceptanceDate DATETIME null,
       CuratorId INT not null,
       SenderStorageId SMALLINT not null,
       SenderId INT null,
       RecipientStorageId SMALLINT not null,
       RecipientId INT not null,
       ValueAddedTaxId SMALLINT not null,
       MovementWaybillCreatedById INT not null,
       MovementWaybillAcceptedById INT null,
       MovementWaybillShippedById INT null,
       MovementWaybillReceiptedById INT null,
       primary key (Id)
    )

    create table dbo.[MovementWaybillRow] (
        Id UNIQUEIDENTIFIER not null,
       MovingCount DECIMAL(18, 6) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       AcceptedCount DECIMAL(18, 6) not null,
       ShippedCount DECIMAL(18, 6) not null,
       FinallyMovedCount DECIMAL(18, 6) not null,
       AvailableToReserveCount DECIMAL(18, 6) not null,
       OutgoingWaybillRowStateId TINYINT not null,
       UsageAsManualSourceCount TINYINT not null,
       IsUsingManualSource BIT not null,
       MovementWaybillRecipientArticleAccountingPriceId UNIQUEIDENTIFIER null,
       MovementWaybillSenderArticleAccountingPriceId UNIQUEIDENTIFIER null,
       MovementWaybillId UNIQUEIDENTIFIER not null,
       ReceiptWaybillRowId UNIQUEIDENTIFIER not null,
       ValueAddedTaxId SMALLINT not null,
       ArticleId INT not null,
       primary key (Id)
    )

    create table dbo.[Organization] (
        Id INT IDENTITY NOT NULL,
       OrganizationTypeId TINYINT not null,
       Address VARCHAR(250) not null,
       ShortName VARCHAR(100) not null,
       FullName VARCHAR(250) not null,
       Phone VARCHAR(20) not null,
       Fax VARCHAR(20) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       EconomicAgentId INT null,
       primary key (Id)
    )

    create table dbo.[AccountOrganization] (
        Id INT not null,
       SalesOwnArticle BIT not null,
       primary key (Id)
    )

    create table dbo.AccountOrganizationStorage (
        AccountOrganizationId INT not null,
       StorageId SMALLINT not null,
       primary key (StorageId, AccountOrganizationId)
    )

    create table dbo.[ContractorOrganization] (
        Id INT not null,
       primary key (Id)
    )

    create table dbo.[ClientOrganization] (
        Id INT not null,
       primary key (Id)
    )

    create table dbo.[ProducerOrganization] (
        Id INT not null,
       DirectorName VARCHAR(100) not null,
       IsManufacturer BIT not null,
       ManufacturerId SMALLINT null,
       primary key (Id)
    )

    create table dbo.[ProviderOrganization] (
        Id INT not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderBatchLifeCycleTemplate] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderBatchLifeCycleTemplateStage] (
        Id INT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       ProductionOrderBatchStageTypeId TINYINT not null,
       OrdinalNumber SMALLINT not null,
       PlannedDuration SMALLINT null,
       InWorkDays BIT not null,
       IsDefault BIT not null,
       TemplateId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderBatch] (
        Id UNIQUEIDENTIFIER not null,
       Name VARCHAR(200) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       ProductionOrderBatchStateId TINYINT not null,
       Date DATETIME not null,
       IsClosed BIT not null,
       MovementToApprovementStateDate DATETIME null,
       MovementToApprovedStateDate DATETIME null,
       ApprovedByLineManagerDate DATETIME null,
       ApprovedByFinancialDepartmentDate DATETIME null,
       ApprovedBySalesDepartmentDate DATETIME null,
       ApprovedByAnalyticalDepartmentDate DATETIME null,
       ApprovedByProjectManagerDate DATETIME null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       ReceiptWaybillId UNIQUEIDENTIFIER null,
       CreatedById INT not null,
       CuratorId INT not null,
       CurrentStageId UNIQUEIDENTIFIER not null,
       MovedToApprovementStateById INT null,
       MovedToApprovedStateById INT null,
       ApprovedLineManagerId INT null,
       FinancialDepartmentApproverId INT null,
       SalesDepartmentApproverId INT null,
       AnalyticalDepartmentApproverId INT null,
       ApprovedProjectManagerId INT null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderBatchRow] (
        Id UNIQUEIDENTIFIER not null,
       ArticleMeasureUnitScale TINYINT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Count DECIMAL(18, 6) not null,
       ProductionOrderBatchRowCostInCurrency DECIMAL(18, 6) not null,
       PackLength DECIMAL(6, 0) not null,
       PackHeight DECIMAL(6, 0) not null,
       PackWidth DECIMAL(6, 0) not null,
       PackWeight DECIMAL(8, 3) not null,
       PackVolume DECIMAL(15, 6) not null,
       PackSize DECIMAL(12, 6) not null,
       OrdinalNumber INT not null,
       ArticleId INT not null,
       BatchId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT not null,
       ManufacturerId SMALLINT not null,
       ProductionCountryId SMALLINT not null,
       ReceiptWaybillRowId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderBatchStage] (
        Id UNIQUEIDENTIFIER not null,
       Name VARCHAR(200) not null,
       PlannedDuration SMALLINT null,
       ActualStartDate DATETIME null,
       ActualEndDate DATETIME null,
       OrdinalNumber SMALLINT not null,
       ProductionOrderBatchStageTypeId TINYINT not null,
       InWorkDays BIT not null,
       IsDefault BIT not null,
       BatchId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderCustomsDeclaration] (
        Id UNIQUEIDENTIFIER not null,
       CustomsDeclarationNumber VARCHAR(33) not null,
       Name VARCHAR(200) not null,
       Date DATETIME not null,
       ImportCustomsDutiesSum DECIMAL(18, 2) not null,
       ExportCustomsDutiesSum DECIMAL(18, 2) not null,
       ValueAddedTaxSum DECIMAL(18, 2) not null,
       ExciseSum DECIMAL(18, 2) not null,
       CustomsFeesSum DECIMAL(18, 2) not null,
       CustomsValueCorrection DECIMAL(18, 2) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderExtraExpensesSheet] (
        Id UNIQUEIDENTIFIER not null,
       ExtraExpensesContractorName VARCHAR(200) not null,
       Date DATETIME not null,
       ProductionOrderCurrencyDeterminationTypeId TINYINT not null,
       CostInCurrency DECIMAL(18, 2) not null,
       ExtraExpensesPurpose VARCHAR(200) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT null,
       CurrencyRateId INT null,
       primary key (Id)
    )

    create table dbo.[ProductionOrder] (
        Id UNIQUEIDENTIFIER not null,
       CreationDate DATETIME not null,
       Date DATETIME not null,
       Name VARCHAR(200) not null unique,
       IsClosed BIT not null,
       WorkDaysPlanScheme TINYINT not null,
       Comment VARCHAR(4000) not null,
       ProductionOrderPlannedProductionExpensesInCurrency DECIMAL(14, 2) not null,
       ProductionOrderPlannedTransportationExpensesInCurrency DECIMAL(14, 2) not null,
       ProductionOrderPlannedExtraExpensesInCurrency DECIMAL(14, 2) not null,
       ProductionOrderPlannedCustomsExpensesInCurrency DECIMAL(14, 2) not null,
       ArticleTransportingPrimeCostCalculationTypeId TINYINT not null,
       StorageId SMALLINT not null,
       CurrencyId SMALLINT not null,
       CurrencyRateId INT null,
       ProducerId INT not null,
       CreatedById INT not null,
       CuratorId INT not null,
       ProducerContractId SMALLINT null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderMaterialsPackage] (
        Id UNIQUEIDENTIFIER not null,
       Name VARCHAR(250) not null,
       Description VARCHAR(250) not null,
       Comment VARCHAR(4000) not null,
       CreationDate DATETIME not null,
       LastChangeDate DATETIME not null,
       DeletionDate DATETIME null,
       ProductionOrderMaterialsPackageSize DECIMAL(18, 6) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderPayment] (
        Id UNIQUEIDENTIFIER not null,
       ProductionOrderPaymentTypeId TINYINT not null,
       PaymentDocumentNumber VARCHAR(255) not null,
       Date DATETIME not null,
       SumInCurrency DECIMAL(18, 2) not null,
       ProductionOrderPaymentFormId TINYINT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       ProductionOrderPlannedPaymentId UNIQUEIDENTIFIER null,
       CurrencyRateId INT not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderCustomsDeclarationPayment] (
        Id UNIQUEIDENTIFIER not null,
       DeletionDate DATETIME null,
       CustomsDeclarationId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderExtraExpensesSheetPayment] (
        Id UNIQUEIDENTIFIER not null,
       DeletionDate DATETIME null,
       ExtraExpensesSheetId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderTransportSheetPayment] (
        Id UNIQUEIDENTIFIER not null,
       DeletionDate DATETIME null,
       TransportSheetId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderPlannedPayment] (
        Id UNIQUEIDENTIFIER not null,
       StartDate DATETIME not null,
       EndDate DATETIME not null,
       SumInCurrency DECIMAL(18, 2) not null,
       Purpose VARCHAR(50) not null,
       ProductionOrderPaymentTypeId TINYINT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT not null,
       CurrencyRateId INT null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderTransportSheet] (
        Id UNIQUEIDENTIFIER not null,
       ForwarderName VARCHAR(200) not null,
       RequestDate DATETIME not null,
       ShippingDate DATETIME null,
       PendingDeliveryDate DATETIME null,
       ActualDeliveryDate DATETIME null,
       ProductionOrderCurrencyDeterminationTypeId TINYINT not null,
       CostInCurrency DECIMAL(18, 2) not null,
       BillOfLadingNumber VARCHAR(100) not null,
       ShippingLine VARCHAR(100) not null,
       PortDocumentNumber VARCHAR(100) not null,
       PortDocumentDate DATETIME null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       CurrencyId SMALLINT null,
       CurrencyRateId INT null,
       primary key (Id)
    )

    create table dbo.[ProviderType] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[ReceiptWaybill] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(25) not null,
       Date DATETIME not null,
       Year INT not null,
       ProviderNumber VARCHAR(25) not null,
       ProviderDate DATETIME null,
       ProviderInvoiceNumber VARCHAR(25) not null,
       ProviderInvoiceDate DATETIME null,
       CustomsDeclarationNumber VARCHAR(33) not null,
       PendingSum DECIMAL(18, 2) not null,
       ApprovedSum DECIMAL(18, 2) null,
       PendingDiscountSum DECIMAL(18, 2) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       ReceiptWaybillStateId TINYINT not null,
       ReceiptDate DATETIME null,
       ApprovementDate DATETIME null,
       AcceptanceDate DATETIME null,
       Comment VARCHAR(4000) not null,
       IsCustomsDeclarationNumberFromReceiptWaybill BIT not null,
       ProductionOrderBatchId UNIQUEIDENTIFIER null,
       CuratorId INT not null,
       ReceiptWaybillReceiptStorageId SMALLINT not null,
       AccountOrganizationId INT not null,
       ProviderId INT null,
       ProviderContractId SMALLINT null,
       PendingValueAddedTaxId SMALLINT not null,
       ReceiptWaybillCreatedById INT not null,
       ReceiptWaybillAcceptedById INT null,
       ReceiptWaybillReceiptedById INT null,
       ReceiptWaybillApprovedById INT null,
       primary key (Id)
    )

    create table dbo.[ReceiptWaybillRow] (
        Id UNIQUEIDENTIFIER not null,
       ArticleMeasureUnitScale TINYINT not null,
       PendingCount DECIMAL(18, 6) not null,
       PendingSum DECIMAL(18, 2) not null,
       ReceiptedCount DECIMAL(18, 6) null,
       ProviderCount DECIMAL(18, 6) null,
       ProviderSum DECIMAL(18, 2) null,
       UsageAsManualSourceCount TINYINT not null,
       AreCountDivergencesAfterReceipt BIT not null,
       AreSumDivergencesAfterReceipt BIT not null,
       ApprovedCount DECIMAL(18, 6) null,
       ApprovedSum DECIMAL(18, 2) null,
       InitialPurchaseCost DECIMAL(18, 6) not null,
       PurchaseCost DECIMAL(18, 6) not null,
       ApprovedPurchaseCost DECIMAL(18, 6) null,
       CustomsDeclarationNumber VARCHAR(33) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       AcceptedCount DECIMAL(18, 6) not null,
       ShippedCount DECIMAL(18, 6) not null,
       FinallyMovedCount DECIMAL(18, 6) not null,
       AvailableToReserveCount DECIMAL(18, 6) not null,
       OrdinalNumber INT not null,
       RecipientArticleAccountingPriceId UNIQUEIDENTIFIER null,
       ReceiptWaybillId UNIQUEIDENTIFIER not null,
       ArticleId INT not null,
       PendingValueAddedTaxId SMALLINT not null,
       ApprovedValueAddedTaxId SMALLINT null,
       CountryId SMALLINT not null,
       ManufacturerId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[ReturnFromClientReason] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[ReturnFromClientWaybillRow] (
        Id UNIQUEIDENTIFIER not null,
       ReturnCount DECIMAL(18, 6) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       AcceptedCount DECIMAL(18, 6) not null,
       ShippedCount DECIMAL(18, 6) not null,
       FinallyMovedCount DECIMAL(18, 6) not null,
       AvailableToReserveCount DECIMAL(18, 6) not null,
       UsageAsManualSourceCount TINYINT not null,
       ReturnFromClientArticleAccountingPriceId UNIQUEIDENTIFIER null,
       ReturnFromClientWaybillId UNIQUEIDENTIFIER not null,
       SaleWaybillRowId UNIQUEIDENTIFIER not null,
       ReceiptWaybillRowId UNIQUEIDENTIFIER not null,
       ArticleId INT not null,
       primary key (Id)
    )

    create table dbo.[ReturnFromClientWaybill] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(25) not null,
       Date DATETIME not null,
       Year INT not null,
       ReturnFromClientWaybillStateId TINYINT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       AcceptanceDate DATETIME null,
       ReceiptDate DATETIME null,
       RecipientAccountingPriceSum DECIMAL(18, 2) null,
       SalePriceSum DECIMAL(18, 2) null,
       Comment VARCHAR(4000) not null,
       TeamId SMALLINT not null,
       ReturnFromClientWaybillCuratorId INT not null,
       ReturnFromClientWaybillCreatedById INT not null,
       ReturnFromClientWaybillAcceptedById INT null,
       ReturnFromClientWaybillReceiptedById INT null,
       ReturnFromClientWaybillRecipientId INT not null,
       ReturnFromClientWaybillDealId INT not null,
       ReturnFromClientWaybillRecipientStorageId SMALLINT not null,
       ReturnFromClientReasonId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[SaleWaybill] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(25) not null,
       Date DATETIME not null,
       Year INT not null,
       Comment VARCHAR(4000) not null,
       DeliveryAddress VARCHAR(250) not null,
       DeliveryAddressTypeId TINYINT not null,
       IsFullyPaid BIT not null,
       IsPrepayment BIT not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       SaleWaybillCuratorId INT not null,
       SaleWaybillCreatedById INT not null,
       SaleWaybillAcceptedById INT null,
       QuotaId INT not null,
       DealId INT not null,
       ValueAddedTaxId SMALLINT not null,
       TeamId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[ExpenditureWaybill] (
        Id UNIQUEIDENTIFIER not null,
       ExpenditureWaybillStateId TINYINT not null,
       AcceptanceDate DATETIME null,
       ShippingDate DATETIME null,
       SenderAccountingPriceSum DECIMAL(18, 2) null,
       SalePriceSum DECIMAL(18, 2) null,
       RoundSalePrice BIT null,
       ExpenditureWaybillSenderStorageId SMALLINT not null,
       ExpenditureWaybillShippedById INT null,
       primary key (Id)
    )

    create table dbo.[Employee] (
        Id INT IDENTITY NOT NULL,
       LastName VARCHAR(100) not null,
       FirstName VARCHAR(100) not null,
       Patronymic VARCHAR(100) not null,
       CreationDate DATETIME not null,
       EmployeePostId SMALLINT not null,
       CreatedById INT not null,
       primary key (Id)
    )

    create table dbo.[EmployeePost] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(100) not null unique,
       primary key (Id)
    )

    create table dbo.[PermissionDistribution] (
        Id INT IDENTITY NOT NULL,
       PermissionDistributionTypeId TINYINT not null,
       PermissionId SMALLINT not null,
       RoleId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[Role] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       IsSystemAdmin BIT not null,
       primary key (Id)
    )

    create table dbo.UserRole (
        RoleId SMALLINT not null,
       UserId INT not null,
       primary key (UserId, RoleId)
    )

    create table dbo.[Setting] (
        Id TINYINT not null,
       UseReadyToAcceptStateForChangeOwnerWaybill BIT not null,
       UseReadyToAcceptStateForExpenditureWaybill BIT not null,
       UseReadyToAcceptStateForMovementWaybill BIT not null,
       UseReadyToAcceptStateForReturnFromClientWaybill BIT not null,
       UseReadyToAcceptStateForWriteOffWaybill BIT not null,
       ActiveUserCountLimit SMALLINT not null,
       TeamCountLimit SMALLINT not null,
       StorageCountLimit SMALLINT not null,
       AccountOrganizationCountLimit SMALLINT not null,
       GigabyteCountLimit SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[Team] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       Comment VARCHAR(4000) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       CreatedById INT not null,
       primary key (Id)
    )

    create table dbo.TeamDeal (
        TeamId SMALLINT not null,
       DealId INT not null,
       primary key (TeamId, DealId)
    )

    create table dbo.UserTeam (
        TeamId SMALLINT not null,
       UserId INT not null,
       primary key (UserId, TeamId)
    )

    create table dbo.TeamStorage (
        TeamId SMALLINT not null,
       StorageId SMALLINT not null,
       primary key (TeamId, StorageId)
    )

    create table dbo.TeamProductionOrder (
        TeamId SMALLINT not null,
       ProductionOrderId UNIQUEIDENTIFIER not null,
       primary key (TeamId, ProductionOrderId)
    )

    create table dbo.[User] (
        Id INT not null,
       DisplayName VARCHAR(100) not null,
       DisplayNameTemplate VARCHAR(3) not null,
       Login VARCHAR(30) not null unique,
       PasswordHash VARCHAR(1024) not null,
       CreationDate DATETIME not null,
       BlockingDate DATETIME null,
       CreatedById INT not null,
       BlockerId INT null,
       primary key (Id)
    )

    create table dbo.[Storage] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       Comment VARCHAR(4000) not null,
       StorageTypeId TINYINT not null,
       primary key (Id)
    )

    create table dbo.[StorageSection] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null,
       Square DECIMAL(18, 2) not null,
       Volume DECIMAL(18, 2) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       StorageId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[TaskExecutionItem] (
        Id INT IDENTITY NOT NULL,
       CreationDate DATETIME not null,
       Date DATETIME not null,
       DeletionDate DATETIME null,
       ResultDescription VARCHAR(4000) not null,
       SpentTime INT not null,
       CompletionPercentage TINYINT not null,
       IsCreatedByUser BIT not null,
       ExecutionStateId SMALLINT not null,
       CreatedById INT not null,
       TaskId INT not null,
       TaskTypeId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[TaskExecutionState] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(100) not null,
       OrdinalNumber SMALLINT not null,
       ExecutionStateTypeId TINYINT not null,
       TaskTypeId SMALLINT null,
       primary key (Id)
    )

    create table dbo.[Task] (
        Id INT IDENTITY NOT NULL,
       CompletionPercentage TINYINT not null,
       CreationDate DATETIME not null,
       DeadLine DATETIME null,
       DeletionDate DATETIME null,
       Description VARCHAR(8000) not null,
       FactualCompletionDate DATETIME null,
       FactualSpentTime INT not null,
       StartDate DATETIME null,
       Topic VARCHAR(200) not null,
       ContractorId INT null,
       CreatedById INT not null,
       DealId INT null,
       ExecutedById INT null,
       PriorityId SMALLINT null,
       ProductionOrderId UNIQUEIDENTIFIER null,
       ExecutionStateId SMALLINT not null,
       TypeId SMALLINT not null,
       primary key (Id)
    )

    create table dbo.[TaskPriority] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(100) not null,
       OrdinalNumber SMALLINT not null unique,
       primary key (Id)
    )

    create table dbo.[TaskType] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(100) not null unique,
       primary key (Id)
    )

    create table dbo.[Trademark] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[ValueAddedTax] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(100) not null,
       Value DECIMAL(4, 2) not null,
       IsDefault BIT not null,
       primary key (Id)
    )

    create table dbo.[ChangeOwnerWaybillRow] (
        Id UNIQUEIDENTIFIER not null,
       MovingCount DECIMAL(18, 6) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       UsageAsManualSourceCount TINYINT not null,
       IsUsingManualSource BIT not null,
       AcceptedCount DECIMAL(18, 6) not null,
       ShippedCount DECIMAL(18, 6) not null,
       FinallyMovedCount DECIMAL(18, 6) not null,
       AvailableToReserveCount DECIMAL(18, 6) not null,
       OutgoingWaybillRowStateId TINYINT not null,
       ChangeOwnerWaybillId UNIQUEIDENTIFIER not null,
       ChangeOwnerWaybillRowReceiptWaybillRowId UNIQUEIDENTIFIER not null,
       ChangeOwnerWaybillRowArticleAccountingPriceId UNIQUEIDENTIFIER null,
       ChangeOwnerWaybillRowValueAddedTaxId SMALLINT not null,
       ArticleId INT not null,
       primary key (Id)
    )

    create table dbo.[SaleWaybillRow] (
        Id UNIQUEIDENTIFIER not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       SellingCount DECIMAL(18, 6) not null,
       SalePrice DECIMAL(18, 2) null,
       AcceptedReturnCount DECIMAL(18, 6) not null,
       ReceiptedReturnCount DECIMAL(18, 6) not null,
       AvailableToReturnCount DECIMAL(18, 6) not null,
       SaleWaybillId UNIQUEIDENTIFIER not null,
       ValueAddedTaxId SMALLINT not null,
       ArticleId INT not null,
       primary key (Id)
    )

    create table dbo.[ExpenditureWaybillRow] (
        Id UNIQUEIDENTIFIER not null,
       IsUsingManualSource BIT not null,
       OutgoingWaybillRowStateId TINYINT not null,
       ExpenditureWaybillRowReceiptWaybillRowId UNIQUEIDENTIFIER not null,
       ExpenditureWaybillSenderArticleAccountingPriceId UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table dbo.[WaybillRowArticleMovement] (
        Id UNIQUEIDENTIFIER not null,
       DestinationWaybillRowId UNIQUEIDENTIFIER not null,
       DestinationWaybillTypeId TINYINT not null,
       SourceWaybillRowId UNIQUEIDENTIFIER not null,
       SourceWaybillTypeId TINYINT not null,
       MovingCount DECIMAL(18, 6) not null,
       IsManuallyCreated BIT not null,
       primary key (Id)
    )

    create table dbo.[WriteoffReason] (
        Id SMALLINT IDENTITY NOT NULL,
       Name VARCHAR(200) not null unique,
       primary key (Id)
    )

    create table dbo.[WriteoffWaybill] (
        Id UNIQUEIDENTIFIER not null,
       Number VARCHAR(25) not null,
       Date DATETIME not null,
       Year INT not null,
       Comment VARCHAR(4000) not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       WriteoffDate DATETIME null,
       AcceptanceDate DATETIME null,
       SenderAccountingPriceSum DECIMAL(18, 2) null,
       WriteoffWaybillStateId TINYINT not null,
       WriteoffWaybillCuratorId INT not null,
       WriteoffWaybillSenderId INT not null,
       WriteoffWaybillSenderStorageId SMALLINT not null,
       WriteoffWaybillReasonId SMALLINT not null,
       WriteoffWaybillCreatedById INT not null,
       WriteoffWaybillAcceptedById INT null,
       WriteoffWaybillWrittenoffById INT null,
       primary key (Id)
    )

    create table dbo.[WriteoffWaybillRow] (
        Id UNIQUEIDENTIFIER not null,
       CreationDate DATETIME not null,
       DeletionDate DATETIME null,
       WriteoffOutgoingWaybillRowStateId TINYINT not null,
       WritingoffCount DECIMAL(18, 6) not null,
       IsUsingManualSource BIT not null,
       WriteoffWaybillId UNIQUEIDENTIFIER not null,
       WriteoffSenderArticleAccountingPriceId UNIQUEIDENTIFIER null,
       WriteoffReceiptWaybillRowId UNIQUEIDENTIFIER not null,
       ArticleId INT not null,
       primary key (Id)
    )

    create table dbo.[ProductionOrderMaterialsPackageDocument] (
        Id UNIQUEIDENTIFIER not null,
       FileName VARCHAR(250) not null,
       Description VARCHAR(250) not null,
       CreationDate DATETIME not null,
       LastChangeDate DATETIME not null,
       DeletionDate DATETIME null,
       Size DECIMAL(18, 6) not null,
       CreatedById INT not null,
       MaterialsPackageId UNIQUEIDENTIFIER not null,
       primary key (Id)
    )

    alter table dbo.[AccountingPriceList] 
        add constraint FK_AccountingPriceList_Curator 
        foreign key (CuratorId) 
        references dbo.[User]

    alter table dbo.[AccountingPriceList] 
        add constraint FK_AccountingPriceDeterminationRule_Storage 
        foreign key (AccountingPriceCalcRuleByCurrentAccountingPriceStorageId) 
        references dbo.[Storage]

    alter table dbo.[AccountingPriceList] 
        add constraint FK_LastDigitCalcRule_Storage 
        foreign key (LastDigitCalcRuleStorageId) 
        references dbo.[Storage]

    alter table dbo.AccountingPriceListStorage 
        add constraint PFK_AccountingPriceList_Storage 
        foreign key (StorageId) 
        references dbo.[Storage]

    alter table dbo.AccountingPriceListStorage 
        add constraint PFK_Storage_AccountingPriceList 
        foreign key (AccountingPriceListId) 
        references dbo.[AccountingPriceList]

    alter table dbo.[AccountOrganizationDocumentNumbers] 
        add constraint FK_AccountOrganization_AccountOrganizationDocumentNumbers_AccountOrganizationId 
        foreign key (AccountOrganizationId) 
        references dbo.[AccountOrganization]

    alter table dbo.[ArticleAccountingPrice] 
        add constraint FK_AccountingPriceList_ArticleAccountingPrice_AccountingPriceListId 
        foreign key (AccountingPriceListId) 
        references dbo.[AccountingPriceList]

    alter table dbo.[ArticleAccountingPrice] 
        add constraint FK_ArticleAccountingPrice_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[ArticleGroup] 
        add constraint FK_ArticleGroup_ArticleGroup_ParentId 
        foreign key (ParentId) 
        references dbo.[ArticleGroup]

    alter table dbo.[Article] 
        add constraint FK_Article_ArticleGroup 
        foreign key (ArticleGroupId) 
        references dbo.[ArticleGroup]

    alter table dbo.[Article] 
        add constraint FK_Article_Trademark 
        foreign key (TradeMarkId) 
        references dbo.[Trademark]

    alter table dbo.[Article] 
        add constraint FK_Article_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.[Manufacturer]

    alter table dbo.[Article] 
        add constraint FK_Article_ProductionCountry 
        foreign key (ProductionCountryId) 
        references dbo.[Country]

    alter table dbo.[Article] 
        add constraint FK_Article_MeasureUnit 
        foreign key (MeasureUnitId) 
        references dbo.[MeasureUnit]

    alter table dbo.[Article] 
        add constraint FK_Article_Certificate 
        foreign key (CertificateId) 
        references dbo.[ArticleCertificate]

    alter table dbo.[ForeignBank] 
        add constraint PFK_ForeignBank 
        foreign key (Id) 
        references dbo.[Bank]

    alter table dbo.[RussianBank] 
        add constraint PFK_RussianBank 
        foreign key (Id) 
        references dbo.[Bank]

    alter table dbo.[BankAccount] 
        add constraint FK_BankAccount_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[BankAccount] 
        add constraint FK_BankAccount_Bank 
        foreign key (BankId) 
        references dbo.[Bank]

    alter table dbo.[RussianBankAccount] 
        add constraint PFK_RussianBankAccount 
        foreign key (Id) 
        references dbo.[BankAccount]

    alter table dbo.[RussianBankAccount] 
        add constraint FK_Organization_RussianBankAccount_OrganizationId 
        foreign key (OrganizationId) 
        references dbo.[Organization]

    alter table dbo.[ForeignBankAccount] 
        add constraint PFK_ForeignBankAccount 
        foreign key (Id) 
        references dbo.[BankAccount]

    alter table dbo.[ForeignBankAccount] 
        add constraint FK_Organization_ForeignBankAccount_OrganizationId 
        foreign key (OrganizationId) 
        references dbo.[Organization]

    alter table dbo.[BaseTaskHistoryItem] 
        add constraint FK_BaseTaskHistoryItem_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[TaskExecutionHistoryItem] 
        add constraint PFK_TaskExecutionHistoryItem 
        foreign key (Id) 
        references dbo.[BaseTaskHistoryItem]

    alter table dbo.[TaskExecutionHistoryItem] 
        add constraint FK_TaskExecutionItem_TaskExecutionHistoryItem_TaskExecutionItemId 
        foreign key (TaskExecutionItemId) 
        references dbo.[TaskExecutionItem]

    alter table dbo.[TaskExecutionHistoryItem] 
        add constraint FK_TaskExecutionHistoryItem_Task 
        foreign key (TaskId) 
        references dbo.[Task]

    alter table dbo.[TaskExecutionHistoryItem] 
        add constraint FK_TaskExecutionHistoryItem_TaskExecutionState 
        foreign key (TaskExecutionStateId) 
        references dbo.[TaskExecutionState]

    alter table dbo.[TaskExecutionHistoryItem] 
        add constraint FK_TaskExecutionHistoryItem_TaskType 
        foreign key (TaskTypeId) 
        references dbo.[TaskType]

    alter table dbo.[TaskHistoryItem] 
        add constraint PFK_TaskHistoryItem 
        foreign key (Id) 
        references dbo.[BaseTaskHistoryItem]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_Task_TaskHistoryItem_TaskId 
        foreign key (TaskId) 
        references dbo.[Task]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_Contractor 
        foreign key (ContractorId) 
        references dbo.[Contractor]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_Deal 
        foreign key (DealId) 
        references dbo.[Deal]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_ExecutedBy 
        foreign key (ExecutedById) 
        references dbo.[User]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_TaskPriority 
        foreign key (TaskPriorityId) 
        references dbo.[TaskPriority]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_ProductionOrder 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_TaskType 
        foreign key (TaskTypeId) 
        references dbo.[TaskType]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_TaskExecutionState 
        foreign key (TaskExecutionStateId) 
        references dbo.[TaskExecutionState]

    alter table dbo.[TaskHistoryItem] 
        add constraint FK_TaskHistoryItem_TaskExecutionItem 
        foreign key (TaskExecutionItemId) 
        references dbo.[TaskExecutionItem]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_Curator 
        foreign key (ChangeOwnerWaybillCuratorId) 
        references dbo.[User]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_Storage 
        foreign key (ChangeOwnerWaybillStorageId) 
        references dbo.[Storage]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_Sender 
        foreign key (ChangeOwnerWaybillSenderId) 
        references dbo.[AccountOrganization]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_Recipient 
        foreign key (ChangeOwnerWaybillRecipientId) 
        references dbo.[AccountOrganization]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_ValueAddedTax 
        foreign key (ChangeOwnerWaybillValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_CreatedBy 
        foreign key (ChangeOwnerWaybillCreatedById) 
        references dbo.[User]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_AcceptedBy 
        foreign key (ChangeOwnerWaybillAcceptedById) 
        references dbo.[User]

    alter table dbo.[ChangeOwnerWaybill] 
        add constraint FK_ChangeOwnerWaybill_ChangedOwnerBy 
        foreign key (ChangeOwnerWaybillChangedOwnerById) 
        references dbo.[User]

    alter table dbo.[Contract] 
        add constraint FK_AccountOrganization_Contract_AccountOrganizationId 
        foreign key (AccountOrganizationId) 
        references dbo.[AccountOrganization]

    alter table dbo.[Contract] 
        add constraint FK_ContractorOrganization_Contract_ContractorOrganizationId 
        foreign key (ContractorOrganizationId) 
        references dbo.[ContractorOrganization]

    alter table dbo.ContractorContract 
        add constraint PFK_Contract_Contractor 
        foreign key (ContractorId) 
        references dbo.[Contractor]

    alter table dbo.ContractorContract 
        add constraint PFK_Contractor_Contract 
        foreign key (ContractId) 
        references dbo.[Contract]

    alter table dbo.[ClientContract] 
        add constraint PFK_ClientContract 
        foreign key (Id) 
        references dbo.[Contract]

    alter table dbo.[ProducerContract] 
        add constraint PFK_ProducerContract 
        foreign key (Id) 
        references dbo.[Contract]

    alter table dbo.[ProviderContract] 
        add constraint PFK_ProviderContract 
        foreign key (Id) 
        references dbo.[Contract]

    alter table dbo.ContractorOrganizationContractor 
        add constraint PFK_Contractor_ContractorOrganization 
        foreign key (ContractorOrganizationId) 
        references dbo.[ContractorOrganization]

    alter table dbo.ContractorOrganizationContractor 
        add constraint PFK_ContractorOrganization_Contractor 
        foreign key (ContractorId) 
        references dbo.[Contractor]

    alter table dbo.[Client] 
        add constraint PFK_Client 
        foreign key (Id) 
        references dbo.[Contractor]

    alter table dbo.[Client] 
        add constraint FK_Client_Type 
        foreign key (ClientTypeId) 
        references dbo.[ClientType]

    alter table dbo.[Client] 
        add constraint FK_Client_Region 
        foreign key (RegionId) 
        references dbo.[ClientRegion]

    alter table dbo.[Client] 
        add constraint FK_Client_ServiceProgram 
        foreign key (ServiceProgramId) 
        references dbo.[ClientServiceProgram]

    alter table dbo.[Client] 
        add constraint FK_Client_ManualBlocker 
        foreign key (ManualBlockerId) 
        references dbo.[User]

    alter table dbo.[Producer] 
        add constraint PFK_Producer 
        foreign key (Id) 
        references dbo.[Contractor]

    alter table dbo.[Producer] 
        add constraint FK_Producer_Curator 
        foreign key (CuratorId) 
        references dbo.[User]

    alter table dbo.ProducerManufacturer 
        add constraint PFK_Producer_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.[Manufacturer]

    alter table dbo.ProducerManufacturer 
        add constraint PFK_Manufacturer_Producer 
        foreign key (ProducerId) 
        references dbo.[Producer]

    alter table dbo.[Provider] 
        add constraint PFK_Provider 
        foreign key (Id) 
        references dbo.[Contractor]

    alter table dbo.[Provider] 
        add constraint FK_Provider_Type 
        foreign key (ProviderTypeId) 
        references dbo.[ProviderType]

    alter table dbo.[CurrencyRate] 
        add constraint FK_CurrencyRate_PreviousCurrencyRate 
        foreign key (PreviousCurrencyRateId) 
        references dbo.[CurrencyRate]

    alter table dbo.[CurrencyRate] 
        add constraint FK_Currency_CurrencyRate_CurrencyId 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[CurrencyRate] 
        add constraint FK_CurrencyRate_BaseCurrency 
        foreign key (BaseCurrencyId) 
        references dbo.[Currency]

    alter table dbo.[Deal] 
        add constraint FK_Client_Deal_ClientId 
        foreign key (ClientId) 
        references dbo.[Client]

    alter table dbo.[Deal] 
        add constraint FK_Deal_Contract 
        foreign key (ClientContractId) 
        references dbo.[ClientContract]

    alter table dbo.[Deal] 
        add constraint FK_Deal_Curator 
        foreign key (CuratorId) 
        references dbo.[User]

    alter table dbo.DealDealQuota 
        add constraint PFK_Deal_DealQuota 
        foreign key (DealQuotaId) 
        references dbo.[DealQuota]

    alter table dbo.DealDealQuota 
        add constraint PFK_DealQuota_Deal 
        foreign key (DealId) 
        references dbo.[Deal]

    alter table dbo.[DealPaymentDocumentDistribution] 
        add constraint FK_DealPaymentFromClient_DealPaymentDocumentDistribution_SourceDealPaymentDocumentId 
        foreign key (SourceDealPaymentDocumentId) 
        references dbo.[DealPaymentDocument]

    alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument] 
        add constraint PFK_DealPaymentDocumentDistributionToDealPaymentDocument 
        foreign key (Id) 
        references dbo.[DealPaymentDocumentDistribution]

    alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument] 
        add constraint FK_DealPaymentToClient_DealPaymentDocumentDistributionToDealPaymentDocument_DestinationDealPaymentDocumentId 
        foreign key (DestinationDealPaymentDocumentId) 
        references dbo.[DealPaymentDocument]

    alter table dbo.[DealPaymentDocumentDistributionToDealPaymentDocument] 
        add constraint FK_DealPaymentDocumentDistributionToDealPaymentDocument_SourceDistributionToReturnFromClientWaybill 
        foreign key (SourceDistributionToReturnFromClientWaybillId) 
        references dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]

    alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] 
        add constraint PFK_DealPaymentDocumentDistributionToReturnFromClientWaybill 
        foreign key (Id) 
        references dbo.[DealPaymentDocumentDistribution]

    alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] 
        add constraint FK_DealPaymentDocumentDistributionToReturnFromClientWaybill_SaleWaybill 
        foreign key (SaleWaybillId) 
        references dbo.[SaleWaybill]

    alter table dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_DealPaymentDocumentDistributionToReturnFromClientWaybill_ReturnFromClientWaybillId 
        foreign key (ReturnFromClientWaybillId) 
        references dbo.[ReturnFromClientWaybill]

    alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill] 
        add constraint PFK_DealPaymentDocumentDistributionToSaleWaybill 
        foreign key (Id) 
        references dbo.[DealPaymentDocumentDistribution]

    alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill] 
        add constraint FK_SaleWaybill_DealPaymentDocumentDistributionToSaleWaybill_SaleWaybillId 
        foreign key (SaleWaybillId) 
        references dbo.[SaleWaybill]

    alter table dbo.[DealPaymentDocumentDistributionToSaleWaybill] 
        add constraint FK_DealPaymentDocumentDistributionToSaleWaybill_SourceDistributionToReturnFromClientWaybill 
        foreign key (SourceDistributionToReturnFromClientWaybillId) 
        references dbo.[DealPaymentDocumentDistributionToReturnFromClientWaybill]

    alter table dbo.[DealPaymentDocument] 
        add constraint FK_Deal_DealPaymentDocument_DealId 
        foreign key (DealId) 
        references dbo.[Deal]

    alter table dbo.[DealPaymentDocument] 
        add constraint FK_DealPaymentDocument_Team 
        foreign key (TeamId) 
        references dbo.[Team]

    alter table dbo.[DealPaymentDocument] 
        add constraint FK_DealPaymentDocument_User 
        foreign key (UserId) 
        references dbo.[User]

    alter table dbo.[DealInitialBalanceCorrection] 
        add constraint PFK_DealInitialBalanceCorrection 
        foreign key (Id) 
        references dbo.[DealPaymentDocument]

    alter table dbo.[DealCreditInitialBalanceCorrection] 
        add constraint PFK_DealCreditInitialBalanceCorrection 
        foreign key (Id) 
        references dbo.[DealInitialBalanceCorrection]

    alter table dbo.[DealDebitInitialBalanceCorrection] 
        add constraint PFK_DealDebitInitialBalanceCorrection 
        foreign key (Id) 
        references dbo.[DealInitialBalanceCorrection]

    alter table dbo.[DealPayment] 
        add constraint PFK_DealPayment 
        foreign key (Id) 
        references dbo.[DealPaymentDocument]

    alter table dbo.[DealPaymentFromClient] 
        add constraint PFK_DealPaymentFromClient 
        foreign key (Id) 
        references dbo.[DealPayment]

    alter table dbo.[DealPaymentToClient] 
        add constraint PFK_DealPaymentToClient 
        foreign key (Id) 
        references dbo.[DealPayment]

    alter table dbo.[DealStageHistory] 
        add constraint FK_Deal_DealStageHistory_DealId 
        foreign key (DealId) 
        references dbo.[Deal]

    alter table dbo.[EconomicAgent] 
        add constraint FK_EconomicAgent_LegalForm 
        foreign key (LegalFormId) 
        references dbo.[LegalForm]

    alter table dbo.[JuridicalPerson] 
        add constraint PFK_JuridicalPerson 
        foreign key (Id) 
        references dbo.[EconomicAgent]

    alter table dbo.[PhysicalPerson] 
        add constraint PFK_PhysicalPerson 
        foreign key (Id) 
        references dbo.[EconomicAgent]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_Curator 
        foreign key (CuratorId) 
        references dbo.[User]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_SenderStorage 
        foreign key (SenderStorageId) 
        references dbo.[Storage]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_Sender 
        foreign key (SenderId) 
        references dbo.[AccountOrganization]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_RecipientStorage 
        foreign key (RecipientStorageId) 
        references dbo.[Storage]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_Recipient 
        foreign key (RecipientId) 
        references dbo.[AccountOrganization]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_ValueAddedTax 
        foreign key (ValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_CreatedBy 
        foreign key (MovementWaybillCreatedById) 
        references dbo.[User]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_AcceptedBy 
        foreign key (MovementWaybillAcceptedById) 
        references dbo.[User]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_ShippedBy 
        foreign key (MovementWaybillShippedById) 
        references dbo.[User]

    alter table dbo.[MovementWaybill] 
        add constraint FK_MovementWaybill_ReceiptedBy 
        foreign key (MovementWaybillReceiptedById) 
        references dbo.[User]

    alter table dbo.[MovementWaybillRow] 
        add constraint FK_MovementWaybillRow_RecipientArticleAccountingPrice 
        foreign key (MovementWaybillRecipientArticleAccountingPriceId) 
        references dbo.[ArticleAccountingPrice]

    alter table dbo.[MovementWaybillRow] 
        add constraint FK_MovementWaybillRow_SenderArticleAccountingPrice 
        foreign key (MovementWaybillSenderArticleAccountingPriceId) 
        references dbo.[ArticleAccountingPrice]

    alter table dbo.[MovementWaybillRow] 
        add constraint FK_MovementWaybill_MovementWaybillRow_MovementWaybillId 
        foreign key (MovementWaybillId) 
        references dbo.[MovementWaybill]

    alter table dbo.[MovementWaybillRow] 
        add constraint FK_MovementWaybillRow_ReceiptWaybillRow 
        foreign key (ReceiptWaybillRowId) 
        references dbo.[ReceiptWaybillRow]

    alter table dbo.[MovementWaybillRow] 
        add constraint FK_MovementWaybillRow_ValueAddedTax 
        foreign key (ValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[MovementWaybillRow] 
        add constraint FK_MovementWaybillRow_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[Organization] 
        add constraint FK_Organization_EconomicAgent 
        foreign key (EconomicAgentId) 
        references dbo.[EconomicAgent]

    alter table dbo.[AccountOrganization] 
        add constraint PFK_AccountOrganization 
        foreign key (Id) 
        references dbo.[Organization]

    alter table dbo.AccountOrganizationStorage 
        add constraint PFK_AccountOrganization_Storage 
        foreign key (StorageId) 
        references dbo.[Storage]

    alter table dbo.AccountOrganizationStorage 
        add constraint PFK_Storage_AccountOrganization 
        foreign key (AccountOrganizationId) 
        references dbo.[AccountOrganization]

    alter table dbo.[ContractorOrganization] 
        add constraint PFK_ContractorOrganization 
        foreign key (Id) 
        references dbo.[Organization]

    alter table dbo.[ClientOrganization] 
        add constraint PFK_ClientOrganization 
        foreign key (Id) 
        references dbo.[ContractorOrganization]

    alter table dbo.[ProducerOrganization] 
        add constraint PFK_ProducerOrganization 
        foreign key (Id) 
        references dbo.[ContractorOrganization]

    alter table dbo.[ProducerOrganization] 
        add constraint FK_ProducerOrganization_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.[Manufacturer]

    alter table dbo.[ProviderOrganization] 
        add constraint PFK_ProviderOrganization 
        foreign key (Id) 
        references dbo.[ContractorOrganization]

    alter table dbo.[ProductionOrderBatchLifeCycleTemplateStage] 
        add constraint FK_ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateStage_TemplateId 
        foreign key (TemplateId) 
        references dbo.[ProductionOrderBatchLifeCycleTemplate]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrder_ProductionOrderBatch_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_ReceiptWaybill 
        foreign key (ReceiptWaybillId) 
        references dbo.[ReceiptWaybill]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_Curator 
        foreign key (CuratorId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_CurrentStage 
        foreign key (CurrentStageId) 
        references dbo.[ProductionOrderBatchStage]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_MovedToApprovementStateBy 
        foreign key (MovedToApprovementStateById) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_MovedToApprovedStateBy 
        foreign key (MovedToApprovedStateById) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_ApprovedLineManager 
        foreign key (ApprovedLineManagerId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_FinancialDepartmentApprover 
        foreign key (FinancialDepartmentApproverId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_SalesDepartmentApprover 
        foreign key (SalesDepartmentApproverId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_AnalyticalDepartmentApprover 
        foreign key (AnalyticalDepartmentApproverId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatch] 
        add constraint FK_ProductionOrderBatch_ApprovedProjectManager 
        foreign key (ApprovedProjectManagerId) 
        references dbo.[User]

    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatchRow_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatch_ProductionOrderBatchRow_BatchId 
        foreign key (BatchId) 
        references dbo.[ProductionOrderBatch]

    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatchRow_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatchRow_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.[Manufacturer]

    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatchRow_ProductionCountry 
        foreign key (ProductionCountryId) 
        references dbo.[Country]

    alter table dbo.[ProductionOrderBatchRow] 
        add constraint FK_ProductionOrderBatchRow_ReceiptWaybillRow 
        foreign key (ReceiptWaybillRowId) 
        references dbo.[ReceiptWaybillRow]

    alter table dbo.[ProductionOrderBatchStage] 
        add constraint FK_ProductionOrderBatch_ProductionOrderBatchStage_BatchId 
        foreign key (BatchId) 
        references dbo.[ProductionOrderBatch]

    alter table dbo.[ProductionOrderCustomsDeclaration] 
        add constraint FK_ProductionOrder_ProductionOrderCustomsDeclaration_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderExtraExpensesSheet] 
        add constraint FK_ProductionOrder_ProductionOrderExtraExpensesSheet_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderExtraExpensesSheet] 
        add constraint FK_ProductionOrderExtraExpensesSheet_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[ProductionOrderExtraExpensesSheet] 
        add constraint FK_ProductionOrderExtraExpensesSheet_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_Storage 
        foreign key (StorageId) 
        references dbo.[Storage]

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_Producer 
        foreign key (ProducerId) 
        references dbo.[Producer]

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_Curator 
        foreign key (CuratorId) 
        references dbo.[User]

    alter table dbo.[ProductionOrder] 
        add constraint FK_ProductionOrder_Contract 
        foreign key (ProducerContractId) 
        references dbo.[ProducerContract]

    alter table dbo.[ProductionOrderMaterialsPackage] 
        add constraint FK_ProductionOrder_ProductionOrderMaterialsPackage_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderPayment] 
        add constraint FK_ProductionOrder_ProductionOrderPayment_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderPayment] 
        add constraint FK_ProductionOrderPlannedPayment_ProductionOrderPayment_ProductionOrderPlannedPaymentId 
        foreign key (ProductionOrderPlannedPaymentId) 
        references dbo.[ProductionOrderPlannedPayment]

    alter table dbo.[ProductionOrderPayment] 
        add constraint FK_ProductionOrderPayment_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]

    alter table dbo.[ProductionOrderCustomsDeclarationPayment] 
        add constraint PFK_ProductionOrderCustomsDeclarationPayment 
        foreign key (Id) 
        references dbo.[ProductionOrderPayment]

    alter table dbo.[ProductionOrderCustomsDeclarationPayment] 
        add constraint FK_ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationPayment_CustomsDeclarationId 
        foreign key (CustomsDeclarationId) 
        references dbo.[ProductionOrderCustomsDeclaration]

    alter table dbo.[ProductionOrderExtraExpensesSheetPayment] 
        add constraint PFK_ProductionOrderExtraExpensesSheetPayment 
        foreign key (Id) 
        references dbo.[ProductionOrderPayment]

    alter table dbo.[ProductionOrderExtraExpensesSheetPayment] 
        add constraint FK_ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetPayment_ExtraExpensesSheetId 
        foreign key (ExtraExpensesSheetId) 
        references dbo.[ProductionOrderExtraExpensesSheet]

    alter table dbo.[ProductionOrderTransportSheetPayment] 
        add constraint PFK_ProductionOrderTransportSheetPayment 
        foreign key (Id) 
        references dbo.[ProductionOrderPayment]

    alter table dbo.[ProductionOrderTransportSheetPayment] 
        add constraint FK_ProductionOrderTransportSheet_ProductionOrderTransportSheetPayment_TransportSheetId 
        foreign key (TransportSheetId) 
        references dbo.[ProductionOrderTransportSheet]

    alter table dbo.[ProductionOrderPlannedPayment] 
        add constraint FK_ProductionOrder_ProductionOrderPlannedPayment_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderPlannedPayment] 
        add constraint FK_ProductionOrderPlannedPayment_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[ProductionOrderPlannedPayment] 
        add constraint FK_ProductionOrderPlannedPayment_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]

    alter table dbo.[ProductionOrderTransportSheet] 
        add constraint FK_ProductionOrder_ProductionOrderTransportSheet_ProductionOrderId 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[ProductionOrderTransportSheet] 
        add constraint FK_ProductionOrderTransportSheet_Currency 
        foreign key (CurrencyId) 
        references dbo.[Currency]

    alter table dbo.[ProductionOrderTransportSheet] 
        add constraint FK_ProductionOrderTransportSheet_CurrencyRate 
        foreign key (CurrencyRateId) 
        references dbo.[CurrencyRate]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_ProductionOrderBatch 
        foreign key (ProductionOrderBatchId) 
        references dbo.[ProductionOrderBatch]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_Curator 
        foreign key (CuratorId) 
        references dbo.[User]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_ReceiptStorage 
        foreign key (ReceiptWaybillReceiptStorageId) 
        references dbo.[Storage]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_AccountOrganization 
        foreign key (AccountOrganizationId) 
        references dbo.[AccountOrganization]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_Provider 
        foreign key (ProviderId) 
        references dbo.[Provider]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_ProviderContract 
        foreign key (ProviderContractId) 
        references dbo.[ProviderContract]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_PendingValueAddedTax 
        foreign key (PendingValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_CreatedBy 
        foreign key (ReceiptWaybillCreatedById) 
        references dbo.[User]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_AcceptedBy 
        foreign key (ReceiptWaybillAcceptedById) 
        references dbo.[User]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_ReceiptedBy 
        foreign key (ReceiptWaybillReceiptedById) 
        references dbo.[User]

    alter table dbo.[ReceiptWaybill] 
        add constraint FK_ReceiptWaybill_ApprovedBy 
        foreign key (ReceiptWaybillApprovedById) 
        references dbo.[User]

    alter table dbo.[ReceiptWaybillRow] 
        add constraint FK_ReceiptWaybillRow_RecipientArticleAccountingPrice 
        foreign key (RecipientArticleAccountingPriceId) 
        references dbo.[ArticleAccountingPrice]

    alter table dbo.[ReceiptWaybillRow] 
        add constraint FK_ReceiptWaybill_ReceiptWaybillRow_ReceiptWaybillId 
        foreign key (ReceiptWaybillId) 
        references dbo.[ReceiptWaybill]

    alter table dbo.[ReceiptWaybillRow] 
        add constraint FK_ReceiptWaybillRow_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[ReceiptWaybillRow] 
        add constraint FK_ReceiptWaybillRow_PendingValueAddedTax 
        foreign key (PendingValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[ReceiptWaybillRow] 
        add constraint FK_ReceiptWaybillRow_ApprovedValueAddedTax 
        foreign key (ApprovedValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[ReceiptWaybillRow] 
        add constraint FK_ReceiptWaybillRow_ProductionCountry 
        foreign key (CountryId) 
        references dbo.[Country]

    alter table dbo.[ReceiptWaybillRow] 
        add constraint FK_ReceiptWaybillRow_Manufacturer 
        foreign key (ManufacturerId) 
        references dbo.[Manufacturer]

    alter table dbo.[ReturnFromClientWaybillRow] 
        add constraint FK_ReturnFromClientWaybillRow_ArticleAccountingPrice 
        foreign key (ReturnFromClientArticleAccountingPriceId) 
        references dbo.[ArticleAccountingPrice]

    alter table dbo.[ReturnFromClientWaybillRow] 
        add constraint FK_ReturnFromClientWaybill_ReturnFromClientWaybillRow_ReturnFromClientWaybillId 
        foreign key (ReturnFromClientWaybillId) 
        references dbo.[ReturnFromClientWaybill]

    alter table dbo.[ReturnFromClientWaybillRow] 
        add constraint FK_ReturnFromClientWaybillRow_SaleWaybillRow 
        foreign key (SaleWaybillRowId) 
        references dbo.[SaleWaybillRow]

    alter table dbo.[ReturnFromClientWaybillRow] 
        add constraint FK_ReturnFromClientWaybillRow_ReceiptWaybillRow 
        foreign key (ReceiptWaybillRowId) 
        references dbo.[ReceiptWaybillRow]

    alter table dbo.[ReturnFromClientWaybillRow] 
        add constraint FK_ReturnFromClientWaybillRow_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_Team 
        foreign key (TeamId) 
        references dbo.[Team]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_Curator 
        foreign key (ReturnFromClientWaybillCuratorId) 
        references dbo.[User]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_CreatedBy 
        foreign key (ReturnFromClientWaybillCreatedById) 
        references dbo.[User]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_AcceptedBy 
        foreign key (ReturnFromClientWaybillAcceptedById) 
        references dbo.[User]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_ReceiptedBy 
        foreign key (ReturnFromClientWaybillReceiptedById) 
        references dbo.[User]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_Recipient 
        foreign key (ReturnFromClientWaybillRecipientId) 
        references dbo.[AccountOrganization]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_Deal 
        foreign key (ReturnFromClientWaybillDealId) 
        references dbo.[Deal]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_RecipientStorage 
        foreign key (ReturnFromClientWaybillRecipientStorageId) 
        references dbo.[Storage]

    alter table dbo.[ReturnFromClientWaybill] 
        add constraint FK_ReturnFromClientWaybill_ReturnFromClientReason 
        foreign key (ReturnFromClientReasonId) 
        references dbo.[ReturnFromClientReason]

    alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_Curator 
        foreign key (SaleWaybillCuratorId) 
        references dbo.[User]

    alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_CreatedBy 
        foreign key (SaleWaybillCreatedById) 
        references dbo.[User]

    alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_AcceptedBy 
        foreign key (SaleWaybillAcceptedById) 
        references dbo.[User]

    alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_Quota 
        foreign key (QuotaId) 
        references dbo.[DealQuota]

    alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_Deal 
        foreign key (DealId) 
        references dbo.[Deal]

    alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_ValueAddedTax 
        foreign key (ValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[SaleWaybill] 
        add constraint FK_SaleWaybill_Team 
        foreign key (TeamId) 
        references dbo.[Team]

    alter table dbo.[ExpenditureWaybill] 
        add constraint PFK_ExpenditureWaybill 
        foreign key (Id) 
        references dbo.[SaleWaybill]

    alter table dbo.[ExpenditureWaybill] 
        add constraint FK_ExpenditureWaybill_SenderStorage 
        foreign key (ExpenditureWaybillSenderStorageId) 
        references dbo.[Storage]

    alter table dbo.[ExpenditureWaybill] 
        add constraint FK_ExpenditureWaybill_ShippedBy 
        foreign key (ExpenditureWaybillShippedById) 
        references dbo.[User]

    alter table dbo.[Employee] 
        add constraint FK_Employee_Post 
        foreign key (EmployeePostId) 
        references dbo.[EmployeePost]

    alter table dbo.[Employee] 
        add constraint FK_Employee_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[PermissionDistribution] 
        add constraint FK_Role_PermissionDistribution_RoleId 
        foreign key (RoleId) 
        references dbo.[Role]

    alter table dbo.UserRole 
        add constraint PFK_Role_User 
        foreign key (UserId) 
        references dbo.[User]

    alter table dbo.UserRole 
        add constraint PFK_User_Role 
        foreign key (RoleId) 
        references dbo.[Role]

    alter table dbo.[Team] 
        add constraint FK_Team_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.TeamDeal 
        add constraint PFK_Team_Deal 
        foreign key (DealId) 
        references dbo.[Deal]

    alter table dbo.TeamDeal 
        add constraint PFK_Deal_Team 
        foreign key (TeamId) 
        references dbo.[Team]

    alter table dbo.UserTeam 
        add constraint PFK_Team_User 
        foreign key (UserId) 
        references dbo.[User]

    alter table dbo.UserTeam 
        add constraint PFK_User_Team 
        foreign key (TeamId) 
        references dbo.[Team]

    alter table dbo.TeamStorage 
        add constraint PFK_Team_Storage 
        foreign key (StorageId) 
        references dbo.[Storage]

    alter table dbo.TeamStorage 
        add constraint PFK_Storage_Team 
        foreign key (TeamId) 
        references dbo.[Team]

    alter table dbo.TeamProductionOrder 
        add constraint PFK_Team_ProductionOrder 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.TeamProductionOrder 
        add constraint PFK_ProductionOrder_Team 
        foreign key (TeamId) 
        references dbo.[Team]

    alter table dbo.[User] 
        add constraint FK_User_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[User] 
        add constraint FK_User_Blocker 
        foreign key (BlockerId) 
        references dbo.[User]

    alter table dbo.[User] 
        add constraint FK_UserToEmployee 
        foreign key (Id) 
        references dbo.[Employee]

    alter table dbo.[StorageSection] 
        add constraint FK_Storage_StorageSection_StorageId 
        foreign key (StorageId) 
        references dbo.[Storage]

    alter table dbo.[TaskExecutionItem] 
        add constraint FK_TaskExecutionItem_ExecutionState 
        foreign key (ExecutionStateId) 
        references dbo.[TaskExecutionState]

    alter table dbo.[TaskExecutionItem] 
        add constraint FK_TaskExecutionItem_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[TaskExecutionItem] 
        add constraint FK_Task_TaskExecutionItem_TaskId 
        foreign key (TaskId) 
        references dbo.[Task]

    alter table dbo.[TaskExecutionItem] 
        add constraint FK_TaskExecutionItem_TaskType 
        foreign key (TaskTypeId) 
        references dbo.[TaskType]

    alter table dbo.[TaskExecutionState] 
        add constraint FK_TaskType_TaskExecutionState_TaskTypeId 
        foreign key (TaskTypeId) 
        references dbo.[TaskType]

    alter table dbo.[Task] 
        add constraint FK_Task_Contractor 
        foreign key (ContractorId) 
        references dbo.[Contractor]

    alter table dbo.[Task] 
        add constraint FK_Task_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[Task] 
        add constraint FK_Task_Deal 
        foreign key (DealId) 
        references dbo.[Deal]

    alter table dbo.[Task] 
        add constraint FK_Task_ExecutedBy 
        foreign key (ExecutedById) 
        references dbo.[User]

    alter table dbo.[Task] 
        add constraint FK_Task_Priority 
        foreign key (PriorityId) 
        references dbo.[TaskPriority]

    alter table dbo.[Task] 
        add constraint FK_Task_ProductionOrder 
        foreign key (ProductionOrderId) 
        references dbo.[ProductionOrder]

    alter table dbo.[Task] 
        add constraint FK_Task_ExecutionState 
        foreign key (ExecutionStateId) 
        references dbo.[TaskExecutionState]

    alter table dbo.[Task] 
        add constraint FK_Task_Type 
        foreign key (TypeId) 
        references dbo.[TaskType]

    alter table dbo.[ChangeOwnerWaybillRow] 
        add constraint FK_ChangeOwnerWaybill_ChangeOwnerWaybillRow_ChangeOwnerWaybillId 
        foreign key (ChangeOwnerWaybillId) 
        references dbo.[ChangeOwnerWaybill]

    alter table dbo.[ChangeOwnerWaybillRow] 
        add constraint FK_ChangeOwnerWaybillRow_ReceiptWaybillRow 
        foreign key (ChangeOwnerWaybillRowReceiptWaybillRowId) 
        references dbo.[ReceiptWaybillRow]

    alter table dbo.[ChangeOwnerWaybillRow] 
        add constraint FK_ChangeOwnerWaybillRow_ArticleAccountingPrice 
        foreign key (ChangeOwnerWaybillRowArticleAccountingPriceId) 
        references dbo.[ArticleAccountingPrice]

    alter table dbo.[ChangeOwnerWaybillRow] 
        add constraint FK_ChangeOwnerWaybillRow_ValueAddedTax 
        foreign key (ChangeOwnerWaybillRowValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[ChangeOwnerWaybillRow] 
        add constraint FK_ChangeOwnerWaybillRow_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[SaleWaybillRow] 
        add constraint FK_SaleWaybill_SaleWaybillRow_SaleWaybillId 
        foreign key (SaleWaybillId) 
        references dbo.[SaleWaybill]

    alter table dbo.[SaleWaybillRow] 
        add constraint FK_SaleWaybillRow_ValueAddedTax 
        foreign key (ValueAddedTaxId) 
        references dbo.[ValueAddedTax]

    alter table dbo.[SaleWaybillRow] 
        add constraint FK_SaleWaybillRow_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[ExpenditureWaybillRow] 
        add constraint PFK_ExpenditureWaybillRow 
        foreign key (Id) 
        references dbo.[SaleWaybillRow]

    alter table dbo.[ExpenditureWaybillRow] 
        add constraint FK_ExpenditureWaybillRow_ReceiptWaybillRow 
        foreign key (ExpenditureWaybillRowReceiptWaybillRowId) 
        references dbo.[ReceiptWaybillRow]

    alter table dbo.[ExpenditureWaybillRow] 
        add constraint FK_ExpenditureWaybillRow_SenderArticleAccountingPrice 
        foreign key (ExpenditureWaybillSenderArticleAccountingPriceId) 
        references dbo.[ArticleAccountingPrice]

    alter table dbo.[WriteoffWaybill] 
        add constraint FK_WriteoffWaybill_Curator 
        foreign key (WriteoffWaybillCuratorId) 
        references dbo.[User]

    alter table dbo.[WriteoffWaybill] 
        add constraint FK_WriteoffWaybill_Sender 
        foreign key (WriteoffWaybillSenderId) 
        references dbo.[AccountOrganization]

    alter table dbo.[WriteoffWaybill] 
        add constraint FK_WriteoffWaybill_SenderStorage 
        foreign key (WriteoffWaybillSenderStorageId) 
        references dbo.[Storage]

    alter table dbo.[WriteoffWaybill] 
        add constraint FK_WriteoffWaybill_WriteoffReason 
        foreign key (WriteoffWaybillReasonId) 
        references dbo.[WriteoffReason]

    alter table dbo.[WriteoffWaybill] 
        add constraint FK_WriteoffWaybill_CreatedBy 
        foreign key (WriteoffWaybillCreatedById) 
        references dbo.[User]

    alter table dbo.[WriteoffWaybill] 
        add constraint FK_WriteoffWaybill_AcceptedBy 
        foreign key (WriteoffWaybillAcceptedById) 
        references dbo.[User]

    alter table dbo.[WriteoffWaybill] 
        add constraint FK_WriteoffWaybill_WrittenoffBy 
        foreign key (WriteoffWaybillWrittenoffById) 
        references dbo.[User]

    alter table dbo.[WriteoffWaybillRow] 
        add constraint FK_WriteoffWaybill_WriteoffWaybillRow_WriteoffWaybillId 
        foreign key (WriteoffWaybillId) 
        references dbo.[WriteoffWaybill]

    alter table dbo.[WriteoffWaybillRow] 
        add constraint FK_WriteoffWaybillRow_SenderArticleAccountingPrice 
        foreign key (WriteoffSenderArticleAccountingPriceId) 
        references dbo.[ArticleAccountingPrice]

    alter table dbo.[WriteoffWaybillRow] 
        add constraint FK_WriteoffWaybillRow_ReceiptWaybillRow 
        foreign key (WriteoffReceiptWaybillRowId) 
        references dbo.[ReceiptWaybillRow]

    alter table dbo.[WriteoffWaybillRow] 
        add constraint FK_WriteoffWaybillRow_Article 
        foreign key (ArticleId) 
        references dbo.[Article]

    alter table dbo.[ProductionOrderMaterialsPackageDocument] 
        add constraint FK_ProductionOrderMaterialsPackageDocument_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.[ProductionOrderMaterialsPackageDocument] 
        add constraint FK_ProductionOrderMaterialsPackage_ProductionOrderMaterialsPackageDocument_MaterialsPackageId 
        foreign key (MaterialsPackageId) 
        references dbo.[ProductionOrderMaterialsPackage]
