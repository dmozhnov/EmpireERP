using System.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Коллекция деталей прав
    /// </summary>
    public static class PermissionDetailsSet
    {                
        public static IEnumerable<PermissionDetails> PermissionDetails
        {
            get { return permissionDetails; }
        }
        private static List<PermissionDetails> permissionDetails;


        static PermissionDetailsSet()
        {
            permissionDetails = new List<PermissionDetails>();
            
            var pd_NonePersonalTeamsAll = new List<PermissionDistributionType>() { PermissionDistributionType.None, PermissionDistributionType.Personal, PermissionDistributionType.Teams, PermissionDistributionType.All };
            var pd_NoneTeamsAll = new List<PermissionDistributionType>() { PermissionDistributionType.None, PermissionDistributionType.Teams, PermissionDistributionType.All };
            var pd_NoneAll = new List<PermissionDistributionType>() { PermissionDistributionType.None, PermissionDistributionType.All };

            #region Общие права доступа

            // Просмотр закупочных цен товаров
            var PurchaseCost_View_ForEverywhere = new PermissionDetails(Permission.PurchaseCost_View_ForEverywhere, pd_NoneAll);
            var PurchaseCost_View_ForReceipt = new PermissionDetails(Permission.PurchaseCost_View_ForReceipt, pd_NonePersonalTeamsAll);
            var AccountingPrice_NotCommandStorage_View = new PermissionDetails(Permission.AccountingPrice_NotCommandStorage_View, pd_NoneAll);

            permissionDetails.Add(PurchaseCost_View_ForEverywhere);
            permissionDetails.Add(PurchaseCost_View_ForReceipt);
            permissionDetails.Add(AccountingPrice_NotCommandStorage_View);

            #endregion

            #region Права доступа к операциям товародвижения

            #region Поставщики

            var Provider_List_Details = new PermissionDetails(Permission.Provider_List_Details, pd_NoneAll);
            var Provider_Create = new PermissionDetails(Permission.Provider_Create, pd_NoneAll);
            var Provider_Edit = new PermissionDetails(Permission.Provider_Edit, pd_NoneAll);
            var Provider_ProviderContract_Create = new PermissionDetails(Permission.Provider_ProviderContract_Create, pd_NoneAll);
            var Provider_ProviderContract_Edit = new PermissionDetails(Permission.Provider_ProviderContract_Edit, pd_NoneAll);
            var Provider_ProviderContract_Delete = new PermissionDetails(Permission.Provider_ProviderContract_Delete, pd_NoneAll);
            var Provider_Delete = new PermissionDetails(Permission.Provider_Delete, pd_NoneAll);

            Provider_List_Details.AddChildDirectRelation(Provider_Create);
            Provider_List_Details.AddChildDirectRelation(Provider_Edit);            
            Provider_List_Details.AddChildDirectRelation(Provider_Delete);

            permissionDetails.Add(Provider_List_Details);
            permissionDetails.Add(Provider_Create);
            permissionDetails.Add(Provider_Edit);
            permissionDetails.Add(Provider_ProviderContract_Create);
            permissionDetails.Add(Provider_ProviderContract_Edit);
            permissionDetails.Add(Provider_ProviderContract_Delete);
            permissionDetails.Add(Provider_Delete);

            #endregion

            #region Организации поставщиков
            
            var ProviderOrganization_List_Details = new PermissionDetails(Permission.ProviderOrganization_List_Details, pd_NoneAll);
            var Provider_ProviderOrganization_Add = new PermissionDetails(Permission.Provider_ProviderOrganization_Add, pd_NoneAll);
            var Provider_ProviderOrganization_Remove = new PermissionDetails(Permission.Provider_ProviderOrganization_Remove, pd_NoneAll);
            var ProviderOrganization_Create = new PermissionDetails(Permission.ProviderOrganization_Create, pd_NoneAll);
            var ProviderOrganization_Edit = new PermissionDetails(Permission.ProviderOrganization_Edit, pd_NoneAll);
            var ProviderOrganization_BankAccount_Create = new PermissionDetails(Permission.ProviderOrganization_BankAccount_Create, pd_NoneAll);
            var ProviderOrganization_BankAccount_Edit = new PermissionDetails(Permission.ProviderOrganization_BankAccount_Edit, pd_NoneAll);
            var ProviderOrganization_BankAccount_Delete = new PermissionDetails(Permission.ProviderOrganization_BankAccount_Delete, pd_NoneAll);
            var ProviderOrganization_Delete = new PermissionDetails(Permission.ProviderOrganization_Delete, pd_NoneAll);

            ProviderOrganization_List_Details.AddParentDirectRelation(Provider_List_Details);
            ProviderOrganization_List_Details.AddChildDirectRelation(Provider_ProviderOrganization_Add);
            ProviderOrganization_List_Details.AddChildDirectRelation(Provider_ProviderOrganization_Remove);
            ProviderOrganization_Create.AddParentDirectRelation(Provider_ProviderOrganization_Add);
            ProviderOrganization_Edit.AddParentDirectRelation(ProviderOrganization_List_Details);
            ProviderOrganization_Delete.AddParentDirectRelation(ProviderOrganization_List_Details);
            ProviderOrganization_List_Details.AddChildDirectRelation(Provider_ProviderContract_Create);
            ProviderOrganization_List_Details.AddChildDirectRelation(Provider_ProviderContract_Edit);
            ProviderOrganization_List_Details.AddChildDirectRelation(Provider_ProviderContract_Delete);
            ProviderOrganization_List_Details.AddChildDirectRelation(ProviderOrganization_BankAccount_Create);
            ProviderOrganization_List_Details.AddChildDirectRelation(ProviderOrganization_BankAccount_Edit);
            ProviderOrganization_List_Details.AddChildDirectRelation(ProviderOrganization_BankAccount_Delete);
            
            permissionDetails.Add(ProviderOrganization_List_Details);
            permissionDetails.Add(Provider_ProviderOrganization_Add);
            permissionDetails.Add(Provider_ProviderOrganization_Remove);
            permissionDetails.Add(ProviderOrganization_Create);
            permissionDetails.Add(ProviderOrganization_Edit);
            permissionDetails.Add(ProviderOrganization_Delete);
            permissionDetails.Add(ProviderOrganization_BankAccount_Create);
            permissionDetails.Add(ProviderOrganization_BankAccount_Edit);
            permissionDetails.Add(ProviderOrganization_BankAccount_Delete);

            #endregion

            #region Типы поставщиков

            var ProviderType_Create = new PermissionDetails(Permission.ProviderType_Create, pd_NoneAll);
            var ProviderType_Edit = new PermissionDetails(Permission.ProviderType_Edit, pd_NoneAll);
            var ProviderType_Delete = new PermissionDetails(Permission.ProviderType_Delete, pd_NoneAll);

            ProviderType_Create.AddParentDirectRelation(Provider_List_Details);
            ProviderType_Edit.AddParentDirectRelation(Provider_List_Details);
            ProviderType_Delete.AddParentDirectRelation(Provider_List_Details);

            permissionDetails.Add(ProviderType_Create);
            permissionDetails.Add(ProviderType_Edit);
            permissionDetails.Add(ProviderType_Delete);

            #endregion

            #region Собственные организации

            var AccountOrganization_Create = new PermissionDetails(Permission.AccountOrganization_Create, pd_NoneAll);
            permissionDetails.Add(AccountOrganization_Create);

            var AccountOrganization_Edit = new PermissionDetails(Permission.AccountOrganization_Edit, pd_NoneAll);
            permissionDetails.Add(AccountOrganization_Edit);

            var AccountOrganization_BankAccount_Create = new PermissionDetails(Permission.AccountOrganization_BankAccount_Create, pd_NoneAll);
            permissionDetails.Add(AccountOrganization_BankAccount_Create);

            var AccountOrganization_BankAccount_Edit = new PermissionDetails(Permission.AccountOrganization_BankAccount_Edit, pd_NoneAll);
            permissionDetails.Add(AccountOrganization_BankAccount_Edit);

            var AccountOrganization_BankAccount_Delete = new PermissionDetails(Permission.AccountOrganization_BankAccount_Delete, pd_NoneAll);
            permissionDetails.Add(AccountOrganization_BankAccount_Delete);

            var AccountOrganization_Delete = new PermissionDetails(Permission.AccountOrganization_Delete, pd_NoneAll);
            permissionDetails.Add(AccountOrganization_Delete);

            #endregion

            #region Места хранения

            var Storage_List_Details = new PermissionDetails(Permission.Storage_List_Details, pd_NoneTeamsAll);
            var Storage_Create = new PermissionDetails(Permission.Storage_Create, pd_NoneAll);
            var Storage_Edit = new PermissionDetails(Permission.Storage_Edit, pd_NoneTeamsAll);
            var Storage_AccountOrganization_Add = new PermissionDetails(Permission.Storage_AccountOrganization_Add, pd_NoneTeamsAll);
            var Storage_AccountOrganization_Remove = new PermissionDetails(Permission.Storage_AccountOrganization_Remove, pd_NoneTeamsAll);
            var Storage_Section_Create_Edit = new PermissionDetails(Permission.Storage_Section_Create_Edit, pd_NoneTeamsAll);
            var Storage_Section_Delete = new PermissionDetails(Permission.Storage_Section_Delete, pd_NoneTeamsAll);
            var Storage_Delete = new PermissionDetails(Permission.Storage_Delete, pd_NoneTeamsAll);

            Storage_List_Details.AddChildDirectRelation(Storage_Create);
            Storage_List_Details.AddChildDirectRelation(Storage_Edit);
            Storage_List_Details.AddChildDirectRelation(Storage_AccountOrganization_Add);
            Storage_List_Details.AddChildDirectRelation(Storage_AccountOrganization_Remove);
            Storage_List_Details.AddChildDirectRelation(Storage_Section_Create_Edit);
            Storage_List_Details.AddChildDirectRelation(Storage_Section_Delete);
            Storage_List_Details.AddChildDirectRelation(Storage_Delete);

            permissionDetails.Add(Storage_List_Details);
            permissionDetails.Add(Storage_Create);
            permissionDetails.Add(Storage_Edit);
            permissionDetails.Add(Storage_AccountOrganization_Add);
            permissionDetails.Add(Storage_AccountOrganization_Remove);
            permissionDetails.Add(Storage_Section_Create_Edit);
            permissionDetails.Add(Storage_Section_Delete);
            permissionDetails.Add(Storage_Delete);

            #endregion

            #region Приходные накладные

            var ReceiptWaybill_List_Details = new PermissionDetails(Permission.ReceiptWaybill_List_Details, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Create_Edit = new PermissionDetails(Permission.ReceiptWaybill_Create_Edit, pd_NonePersonalTeamsAll);            
            //var ReceiptWaybill_Provider_Storage_Change = new PermissionDetails(Permission.ReceiptWaybill_Provider_Storage_Change, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Curator_Change = new PermissionDetails(Permission.ReceiptWaybill_Curator_Change, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Delete_Row_Delete = new PermissionDetails(Permission.ReceiptWaybill_Delete_Row_Delete, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_ProviderDocuments_Edit = new PermissionDetails(Permission.ReceiptWaybill_ProviderDocuments_Edit, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Accept = new PermissionDetails(Permission.ReceiptWaybill_Accept, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Acceptance_Cancel = new PermissionDetails(Permission.ReceiptWaybill_Acceptance_Cancel, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Receipt = new PermissionDetails(Permission.ReceiptWaybill_Receipt, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Receipt_Cancel = new PermissionDetails(Permission.ReceiptWaybill_Receipt_Cancel, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Approve = new PermissionDetails(Permission.ReceiptWaybill_Approve, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Approvement_Cancel = new PermissionDetails(Permission.ReceiptWaybill_Approvement_Cancel, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_CreateFromProductionOrderBatch = new PermissionDetails(Permission.ReceiptWaybill_CreateFromProductionOrderBatch, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Date_Change = new PermissionDetails(Permission.ReceiptWaybill_Date_Change, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Accept_Retroactively = new PermissionDetails(Permission.ReceiptWaybill_Accept_Retroactively, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Receipt_Retroactively = new PermissionDetails(Permission.ReceiptWaybill_Receipt_Retroactively, pd_NonePersonalTeamsAll);
            var ReceiptWaybill_Approve_Retroactively = new PermissionDetails(Permission.ReceiptWaybill_Approve_Retroactively, pd_NonePersonalTeamsAll);

            ReceiptWaybill_List_Details.AddParentDirectRelation(Storage_List_Details);

            ReceiptWaybill_List_Details.AddChildDirectRelation(PurchaseCost_View_ForReceipt);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Create_Edit);
            //ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Provider_Storage_Change);            
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Curator_Change);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Delete_Row_Delete);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_ProviderDocuments_Edit);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Accept);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Acceptance_Cancel);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Receipt);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Receipt_Cancel);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Approve);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Approvement_Cancel);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_CreateFromProductionOrderBatch);
            ReceiptWaybill_List_Details.AddChildDirectRelation(ReceiptWaybill_Date_Change);
            ReceiptWaybill_Accept.AddChildDirectRelation(ReceiptWaybill_Accept_Retroactively);
            ReceiptWaybill_Receipt.AddChildDirectRelation(ReceiptWaybill_Receipt_Retroactively);
            ReceiptWaybill_Approve.AddChildDirectRelation(ReceiptWaybill_Approve_Retroactively);

            permissionDetails.Add(ReceiptWaybill_List_Details);
            permissionDetails.Add(ReceiptWaybill_Create_Edit);
            //permissionDetails.Add(ReceiptWaybill_Provider_Storage_Change);
            permissionDetails.Add(ReceiptWaybill_Curator_Change);
            permissionDetails.Add(ReceiptWaybill_Delete_Row_Delete);
            permissionDetails.Add(ReceiptWaybill_ProviderDocuments_Edit);
            permissionDetails.Add(ReceiptWaybill_Accept);
            permissionDetails.Add(ReceiptWaybill_Acceptance_Cancel);
            permissionDetails.Add(ReceiptWaybill_Receipt);
            permissionDetails.Add(ReceiptWaybill_Receipt_Cancel);
            permissionDetails.Add(ReceiptWaybill_Approve);
            permissionDetails.Add(ReceiptWaybill_Approvement_Cancel);
            permissionDetails.Add(ReceiptWaybill_CreateFromProductionOrderBatch);
            permissionDetails.Add(ReceiptWaybill_Date_Change);
            permissionDetails.Add(ReceiptWaybill_Accept_Retroactively);
            permissionDetails.Add(ReceiptWaybill_Receipt_Retroactively);
            permissionDetails.Add(ReceiptWaybill_Approve_Retroactively);
    
            #endregion

            #region Накладные перемещения

            var MovementWaybill_List_Details = new PermissionDetails(Permission.MovementWaybill_List_Details, pd_NonePersonalTeamsAll);
            var MovementWaybill_Create_Edit = new PermissionDetails(Permission.MovementWaybill_Create_Edit, pd_NonePersonalTeamsAll);
            var MovementWaybill_Curator_Change = new PermissionDetails(Permission.MovementWaybill_Curator_Change, pd_NonePersonalTeamsAll);
            var MovementWaybill_Delete_Row_Delete = new PermissionDetails(Permission.MovementWaybill_Delete_Row_Delete, pd_NonePersonalTeamsAll);
            var MovementWaybill_Accept = new PermissionDetails(Permission.MovementWaybill_Accept, pd_NonePersonalTeamsAll);
            var MovementWaybill_Acceptance_Cancel = new PermissionDetails(Permission.MovementWaybill_Acceptance_Cancel, pd_NonePersonalTeamsAll);
            var MovementWaybill_Ship = new PermissionDetails(Permission.MovementWaybill_Ship, pd_NonePersonalTeamsAll);
            var MovementWaybill_Shipping_Cancel = new PermissionDetails(Permission.MovementWaybill_Shipping_Cancel, pd_NonePersonalTeamsAll);
            var MovementWaybill_Receipt = new PermissionDetails(Permission.MovementWaybill_Receipt, pd_NonePersonalTeamsAll);
            var MovementWaybill_Receipt_Cancel = new PermissionDetails(Permission.MovementWaybill_Receipt_Cancel, pd_NonePersonalTeamsAll);

            MovementWaybill_List_Details.AddParentDirectRelation(Storage_List_Details);

            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Create_Edit);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Curator_Change);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Delete_Row_Delete);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Accept);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Acceptance_Cancel);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Ship);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Shipping_Cancel);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Receipt);
            MovementWaybill_List_Details.AddChildDirectRelation(MovementWaybill_Receipt_Cancel);

            permissionDetails.Add(MovementWaybill_List_Details);
            permissionDetails.Add(MovementWaybill_Create_Edit);
            permissionDetails.Add(MovementWaybill_Curator_Change);
            permissionDetails.Add(MovementWaybill_Delete_Row_Delete);
            permissionDetails.Add(MovementWaybill_Accept);
            permissionDetails.Add(MovementWaybill_Acceptance_Cancel);
            permissionDetails.Add(MovementWaybill_Ship);
            permissionDetails.Add(MovementWaybill_Shipping_Cancel);
            permissionDetails.Add(MovementWaybill_Receipt);
            permissionDetails.Add(MovementWaybill_Receipt_Cancel);

            #endregion

            #region Накладные смен собственника

            var ChangeOwnerWaybill_List_Details = new PermissionDetails(Permission.ChangeOwnerWaybill_List_Details, pd_NonePersonalTeamsAll);
            var ChangeOwnerWaybill_Create_Edit = new PermissionDetails(Permission.ChangeOwnerWaybill_Create_Edit, pd_NonePersonalTeamsAll);
            var ChangeOwnerWaybill_Recipient_Change = new PermissionDetails(Permission.ChangeOwnerWaybill_Recipient_Change, pd_NonePersonalTeamsAll);
            var ChangeOwnerWaybill_Curator_Change = new PermissionDetails(Permission.ChangeOwnerWaybill_Curator_Change, pd_NonePersonalTeamsAll);
            var ChangeOwnerWaybill_Delete_Row_Delete = new PermissionDetails(Permission.ChangeOwnerWaybill_Delete_Row_Delete, pd_NonePersonalTeamsAll);
            var ChangeOwnerWaybill_Accept = new PermissionDetails(Permission.ChangeOwnerWaybill_Accept, pd_NonePersonalTeamsAll);
            var ChangeOwnerWaybill_Acceptance_Cancel = new PermissionDetails(Permission.ChangeOwnerWaybill_Acceptance_Cancel, pd_NonePersonalTeamsAll);

            ChangeOwnerWaybill_List_Details.AddParentDirectRelation(Storage_List_Details);

            ChangeOwnerWaybill_List_Details.AddChildDirectRelation(ChangeOwnerWaybill_Create_Edit);
            ChangeOwnerWaybill_List_Details.AddChildDirectRelation(ChangeOwnerWaybill_Recipient_Change);
            ChangeOwnerWaybill_List_Details.AddChildDirectRelation(ChangeOwnerWaybill_Curator_Change);
            ChangeOwnerWaybill_List_Details.AddChildDirectRelation(ChangeOwnerWaybill_Delete_Row_Delete);
            ChangeOwnerWaybill_List_Details.AddChildDirectRelation(ChangeOwnerWaybill_Accept);
            ChangeOwnerWaybill_List_Details.AddChildDirectRelation(ChangeOwnerWaybill_Acceptance_Cancel);

            permissionDetails.Add(ChangeOwnerWaybill_List_Details);
            permissionDetails.Add(ChangeOwnerWaybill_Create_Edit);
            permissionDetails.Add(ChangeOwnerWaybill_Recipient_Change);
            permissionDetails.Add(ChangeOwnerWaybill_Curator_Change);
            permissionDetails.Add(ChangeOwnerWaybill_Delete_Row_Delete);
            permissionDetails.Add(ChangeOwnerWaybill_Accept);
            permissionDetails.Add(ChangeOwnerWaybill_Acceptance_Cancel);            

            #endregion

            #region Списание товаров

            var WriteoffWaybill_List_Details = new PermissionDetails(Permission.WriteoffWaybill_List_Details, pd_NonePersonalTeamsAll);
            var WriteoffWaybill_Create_Edit = new PermissionDetails(Permission.WriteoffWaybill_Create_Edit, pd_NonePersonalTeamsAll);
            var WriteoffWaybill_Curator_Change = new PermissionDetails(Permission.WriteoffWaybill_Curator_Change, pd_NonePersonalTeamsAll);
            var WriteoffWaybill_Delete_Row_Delete = new PermissionDetails(Permission.WriteoffWaybill_Delete_Row_Delete, pd_NonePersonalTeamsAll);
            var WriteoffWaybill_Accept = new PermissionDetails(Permission.WriteoffWaybill_Accept, pd_NonePersonalTeamsAll);
            var WriteoffWaybill_Acceptance_Cancel = new PermissionDetails(Permission.WriteoffWaybill_Acceptance_Cancel, pd_NonePersonalTeamsAll);
            var WriteoffWaybill_Writeoff = new PermissionDetails(Permission.WriteoffWaybill_Writeoff, pd_NonePersonalTeamsAll);
            var WriteoffWaybill_Writeoff_Cancel = new PermissionDetails(Permission.WriteoffWaybill_Writeoff_Cancel, pd_NonePersonalTeamsAll);

            WriteoffWaybill_List_Details.AddParentDirectRelation(Storage_List_Details);

            WriteoffWaybill_List_Details.AddChildDirectRelation(WriteoffWaybill_Create_Edit);
            WriteoffWaybill_List_Details.AddChildDirectRelation(WriteoffWaybill_Curator_Change);
            WriteoffWaybill_List_Details.AddChildDirectRelation(WriteoffWaybill_Delete_Row_Delete);
            WriteoffWaybill_List_Details.AddChildDirectRelation(WriteoffWaybill_Accept);
            WriteoffWaybill_List_Details.AddChildDirectRelation(WriteoffWaybill_Acceptance_Cancel);
            WriteoffWaybill_List_Details.AddChildDirectRelation(WriteoffWaybill_Writeoff);
            WriteoffWaybill_List_Details.AddChildDirectRelation(WriteoffWaybill_Writeoff_Cancel);

            permissionDetails.Add(WriteoffWaybill_List_Details);
            permissionDetails.Add(WriteoffWaybill_Create_Edit);
            permissionDetails.Add(WriteoffWaybill_Curator_Change);
            permissionDetails.Add(WriteoffWaybill_Delete_Row_Delete);
            permissionDetails.Add(WriteoffWaybill_Accept);
            permissionDetails.Add(WriteoffWaybill_Acceptance_Cancel);
            permissionDetails.Add(WriteoffWaybill_Writeoff);
            permissionDetails.Add(WriteoffWaybill_Writeoff_Cancel);

            #endregion

            #region Основания для списания

            var WriteoffReason_Create = new PermissionDetails(Permission.WriteoffReason_Create, pd_NoneAll);
            var WriteoffReason_Edit = new PermissionDetails(Permission.WriteoffReason_Edit, pd_NoneAll);
            var WriteoffReason_Delete = new PermissionDetails(Permission.WriteoffReason_Delete, pd_NoneAll);

            WriteoffReason_Create.AddParentDirectRelation(WriteoffWaybill_List_Details);
            WriteoffReason_Edit.AddParentDirectRelation(WriteoffWaybill_List_Details);
            WriteoffReason_Delete.AddParentDirectRelation(WriteoffWaybill_List_Details);

            permissionDetails.Add(WriteoffReason_Create);
            permissionDetails.Add(WriteoffReason_Edit);
            permissionDetails.Add(WriteoffReason_Delete);

            #endregion

            #region Реестры цен

            var AccountingPriceList_List_Details = new PermissionDetails(Permission.AccountingPriceList_List_Details, pd_NonePersonalTeamsAll);
            var AccountingPriceList_Create = new PermissionDetails(Permission.AccountingPriceList_Create, pd_NoneAll);
            var AccountingPriceList_Edit = new PermissionDetails(Permission.AccountingPriceList_Edit, pd_NonePersonalTeamsAll);
            var AccountingPriceList_ArticleAccountingPrice_Create_Edit = new PermissionDetails(Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit, pd_NonePersonalTeamsAll);
            var AccountingPriceList_DefaultAccountingPrice_Edit = new PermissionDetails(Permission.AccountingPriceList_DefaultAccountingPrice_Edit, pd_NonePersonalTeamsAll);
            var AccountingPriceList_Storage_Add = new PermissionDetails(Permission.AccountingPriceList_Storage_Add, pd_NonePersonalTeamsAll);
            var AccountingPriceList_Storage_Remove = new PermissionDetails(Permission.AccountingPriceList_Storage_Remove, pd_NonePersonalTeamsAll);
            var AccountingPriceList_Delete = new PermissionDetails(Permission.AccountingPriceList_Delete, pd_NonePersonalTeamsAll);
            var AccountingPriceList_Accept = new PermissionDetails(Permission.AccountingPriceList_Accept, pd_NonePersonalTeamsAll);
            var AccountingPriceList_Acceptance_Cancel = new PermissionDetails(Permission.AccountingPriceList_Acceptance_Cancel, pd_NonePersonalTeamsAll);

            AccountingPriceList_List_Details.AddParentDirectRelation(Storage_List_Details);

            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_Create);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_Edit);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_ArticleAccountingPrice_Create_Edit);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_DefaultAccountingPrice_Edit);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_Storage_Add);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_Storage_Remove);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_Delete);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_Accept);
            AccountingPriceList_List_Details.AddChildDirectRelation(AccountingPriceList_Acceptance_Cancel);

            permissionDetails.Add(AccountingPriceList_List_Details);
            permissionDetails.Add(AccountingPriceList_Create);
            permissionDetails.Add(AccountingPriceList_Edit);
            permissionDetails.Add(AccountingPriceList_ArticleAccountingPrice_Create_Edit);
            permissionDetails.Add(AccountingPriceList_DefaultAccountingPrice_Edit);
            permissionDetails.Add(AccountingPriceList_Storage_Add);
            permissionDetails.Add(AccountingPriceList_Storage_Remove);
            permissionDetails.Add(AccountingPriceList_Delete);
            permissionDetails.Add(AccountingPriceList_Accept);
            permissionDetails.Add(AccountingPriceList_Acceptance_Cancel);

            #endregion

            #endregion

            #region Права доступа к операциям производства

            #region Производители

            var Producer_List_Details = new PermissionDetails(Permission.Producer_List_Details, pd_NoneAll);
            var Producer_Create = new PermissionDetails(Permission.Producer_Create, pd_NoneAll);
            var Producer_Edit = new PermissionDetails(Permission.Producer_Edit, pd_NoneAll);
            var Producer_BankAccount_Create = new PermissionDetails(Permission.Producer_BankAccount_Create, pd_NoneAll);
            var Producer_BankAccount_Edit = new PermissionDetails(Permission.Producer_BankAccount_Edit, pd_NoneAll);
            var Producer_BankAccount_Delete = new PermissionDetails(Permission.Producer_BankAccount_Delete, pd_NoneAll);
            var Producer_Delete = new PermissionDetails(Permission.Producer_Delete, pd_NoneAll);

            Producer_List_Details.AddChildDirectRelation(Producer_Create);
            Producer_List_Details.AddChildDirectRelation(Producer_Edit);
            Producer_List_Details.AddChildDirectRelation(Producer_BankAccount_Create);
            Producer_List_Details.AddChildDirectRelation(Producer_BankAccount_Edit);
            Producer_List_Details.AddChildDirectRelation(Producer_BankAccount_Delete);
            Producer_List_Details.AddChildDirectRelation(Producer_Delete);

            permissionDetails.Add(Producer_List_Details);
            permissionDetails.Add(Producer_Create);
            permissionDetails.Add(Producer_Edit);
            permissionDetails.Add(Producer_BankAccount_Create);
            permissionDetails.Add(Producer_BankAccount_Edit);
            permissionDetails.Add(Producer_BankAccount_Delete);
            permissionDetails.Add(Producer_Delete);

            #endregion

            #region Заказы на производство

            var ProductionOrder_List_Details = new PermissionDetails(Permission.ProductionOrder_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrder_Create_Edit = new PermissionDetails(Permission.ProductionOrder_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrder_Curator_Change = new PermissionDetails(Permission.ProductionOrder_Curator_Change, pd_NonePersonalTeamsAll);
            var ProductionOrder_CurrencyRate_Change = new PermissionDetails(Permission.ProductionOrder_CurrencyRate_Change, pd_NonePersonalTeamsAll);
            var ProductionOrder_Contract_Change = new PermissionDetails(Permission.ProductionOrder_Contract_Change, pd_NonePersonalTeamsAll);
            var ProductionOrder_ArticlePrimeCostPrintingForm_View = new PermissionDetails(Permission.ProductionOrder_ArticlePrimeCostPrintingForm_View, pd_NonePersonalTeamsAll);

            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_Create_Edit);
            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_Curator_Change);
            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_CurrencyRate_Change);
            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_Contract_Change);
            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_ArticlePrimeCostPrintingForm_View);

            permissionDetails.Add(ProductionOrder_List_Details);
            permissionDetails.Add(ProductionOrder_Create_Edit);
            permissionDetails.Add(ProductionOrder_Curator_Change);
            permissionDetails.Add(ProductionOrder_CurrencyRate_Change);
            permissionDetails.Add(ProductionOrder_Contract_Change);
            permissionDetails.Add(ProductionOrder_ArticlePrimeCostPrintingForm_View);

            #endregion

            #region План исполнения (этапы) заказа

            var ProductionOrder_Stage_List_Details = new PermissionDetails(Permission.ProductionOrder_Stage_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrder_Stage_Create_Edit = new PermissionDetails(Permission.ProductionOrder_Stage_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrder_Stage_MoveToNext = new PermissionDetails(Permission.ProductionOrder_Stage_MoveToNext, pd_NonePersonalTeamsAll);
            var ProductionOrder_Stage_MoveToPrevious = new PermissionDetails(Permission.ProductionOrder_Stage_MoveToPrevious, pd_NonePersonalTeamsAll);
            var ProductionOrder_Stage_MoveToUnsuccessfulClosing = new PermissionDetails(Permission.ProductionOrder_Stage_MoveToUnsuccessfulClosing, pd_NonePersonalTeamsAll);

            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_Stage_List_Details);

            ProductionOrder_Stage_List_Details.AddChildDirectRelation(ProductionOrder_Stage_Create_Edit);
            ProductionOrder_Stage_List_Details.AddChildDirectRelation(ProductionOrder_Stage_MoveToNext);
            ProductionOrder_Stage_List_Details.AddChildDirectRelation(ProductionOrder_Stage_MoveToPrevious);
            ProductionOrder_Stage_List_Details.AddChildDirectRelation(ProductionOrder_Stage_MoveToUnsuccessfulClosing);

            permissionDetails.Add(ProductionOrder_Stage_List_Details);
            permissionDetails.Add(ProductionOrder_Stage_Create_Edit);
            permissionDetails.Add(ProductionOrder_Stage_MoveToNext);
            permissionDetails.Add(ProductionOrder_Stage_MoveToPrevious);
            permissionDetails.Add(ProductionOrder_Stage_MoveToUnsuccessfulClosing);

            #endregion

            #region Финансовый план заказа

            #region План оплат

            var ProductionOrder_PlannedPayments_List_Details = new PermissionDetails(Permission.ProductionOrder_PlannedPayments_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrder_PlannedPayments_Create = new PermissionDetails(Permission.ProductionOrder_PlannedPayments_Create, pd_NonePersonalTeamsAll);
            var ProductionOrder_PlannedPayments_Edit = new PermissionDetails(Permission.ProductionOrder_PlannedPayments_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrder_PlannedPayments_Delete = new PermissionDetails(Permission.ProductionOrder_PlannedPayments_Delete, pd_NonePersonalTeamsAll);

            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_PlannedPayments_List_Details);

            ProductionOrder_PlannedPayments_List_Details.AddChildDirectRelation(ProductionOrder_PlannedPayments_Create);
            ProductionOrder_PlannedPayments_List_Details.AddChildDirectRelation(ProductionOrder_PlannedPayments_Edit);
            ProductionOrder_PlannedPayments_List_Details.AddChildDirectRelation(ProductionOrder_PlannedPayments_Delete);

            permissionDetails.Add(ProductionOrder_PlannedPayments_List_Details);
            permissionDetails.Add(ProductionOrder_PlannedPayments_Create);
            permissionDetails.Add(ProductionOrder_PlannedPayments_Edit);
            permissionDetails.Add(ProductionOrder_PlannedPayments_Delete);

            #endregion

            #region Финансовый план

            var ProductionOrder_PlannedExpenses_List_Details = new PermissionDetails(Permission.ProductionOrder_PlannedExpenses_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrder_PlannedExpenses_Create_Edit = new PermissionDetails(Permission.ProductionOrder_PlannedExpenses_Create_Edit, pd_NonePersonalTeamsAll);

            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrder_PlannedExpenses_List_Details);

            ProductionOrder_PlannedExpenses_List_Details.AddChildDirectRelation(ProductionOrder_PlannedExpenses_Create_Edit);

            permissionDetails.Add(ProductionOrder_PlannedExpenses_List_Details);
            permissionDetails.Add(ProductionOrder_PlannedExpenses_Create_Edit);
            
            #endregion

            #endregion

            #region Партии заказа

            var ProductionOrderBatch_List = new PermissionDetails(Permission.ProductionOrderBatch_List, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Create = new PermissionDetails(Permission.ProductionOrderBatch_Create, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Delete = new PermissionDetails(Permission.ProductionOrderBatch_Delete, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Details = new PermissionDetails(Permission.ProductionOrderBatch_Details, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Row_Create_Edit = new PermissionDetails(Permission.ProductionOrderBatch_Row_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Row_Delete = new PermissionDetails(Permission.ProductionOrderBatch_Row_Delete, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Accept = new PermissionDetails(Permission.ProductionOrderBatch_Accept, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Cancel_Acceptance = new PermissionDetails(Permission.ProductionOrderBatch_Cancel_Acceptance, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Approve_By_LineManager = new PermissionDetails(Permission.ProductionOrderBatch_Approve_By_LineManager, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Cancel_Approvement_By_LineManager = new PermissionDetails(Permission.ProductionOrderBatch_Cancel_Approvement_By_LineManager, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Approve_By_FinancialDepartment = new PermissionDetails(Permission.ProductionOrderBatch_Approve_By_FinancialDepartment, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment = new PermissionDetails(Permission.ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Approve_By_SalesDepartment = new PermissionDetails(Permission.ProductionOrderBatch_Approve_By_SalesDepartment, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment = new PermissionDetails(Permission.ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Approve_By_AnalyticalDepartment = new PermissionDetails(Permission.ProductionOrderBatch_Approve_By_AnalyticalDepartment, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment = new PermissionDetails(Permission.ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Approve_By_ProjectManager = new PermissionDetails(Permission.ProductionOrderBatch_Approve_By_ProjectManager, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Cancel_Approvement_By_ProjectManager = new PermissionDetails(Permission.ProductionOrderBatch_Cancel_Approvement_By_ProjectManager, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Approve = new PermissionDetails(Permission.ProductionOrderBatch_Approve, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Cancel_Approvement = new PermissionDetails(Permission.ProductionOrderBatch_Cancel_Approvement, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Split = new PermissionDetails(Permission.ProductionOrderBatch_Split, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Join = new PermissionDetails(Permission.ProductionOrderBatch_Join, pd_NonePersonalTeamsAll);
            var ProductionOrderBatch_Edit_Placement_In_Containers = new PermissionDetails(Permission.ProductionOrderBatch_Edit_Placement_In_Containers, pd_NonePersonalTeamsAll);

            ProductionOrder_List_Details.AddChildDirectRelation(ProductionOrderBatch_List);

            ProductionOrderBatch_List.AddChildDirectRelation(ProductionOrderBatch_Details);
          
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Create);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Delete);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Row_Create_Edit);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Row_Delete);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Accept);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Cancel_Acceptance);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Approve_By_LineManager);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Cancel_Approvement_By_LineManager);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Approve_By_FinancialDepartment);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Approve_By_SalesDepartment);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Approve_By_AnalyticalDepartment);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Approve_By_ProjectManager);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Cancel_Approvement_By_ProjectManager);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Approve);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Cancel_Approvement);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Split);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Join);
            ProductionOrderBatch_Details.AddChildDirectRelation(ProductionOrderBatch_Edit_Placement_In_Containers);

            permissionDetails.Add(ProductionOrderBatch_Details);
            permissionDetails.Add(ProductionOrderBatch_List);
            permissionDetails.Add(ProductionOrderBatch_Create);
            permissionDetails.Add(ProductionOrderBatch_Delete);
            permissionDetails.Add(ProductionOrderBatch_Row_Create_Edit);
            permissionDetails.Add(ProductionOrderBatch_Row_Delete);
            permissionDetails.Add(ProductionOrderBatch_Accept);
            permissionDetails.Add(ProductionOrderBatch_Cancel_Acceptance);
            permissionDetails.Add(ProductionOrderBatch_Approve_By_LineManager);
            permissionDetails.Add(ProductionOrderBatch_Cancel_Approvement_By_LineManager);
            permissionDetails.Add(ProductionOrderBatch_Approve_By_FinancialDepartment);
            permissionDetails.Add(ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment);
            permissionDetails.Add(ProductionOrderBatch_Approve_By_SalesDepartment);
            permissionDetails.Add(ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment);
            permissionDetails.Add(ProductionOrderBatch_Approve_By_AnalyticalDepartment);
            permissionDetails.Add(ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment); 
            permissionDetails.Add(ProductionOrderBatch_Approve_By_ProjectManager);
            permissionDetails.Add(ProductionOrderBatch_Cancel_Approvement_By_ProjectManager);
            permissionDetails.Add(ProductionOrderBatch_Approve);
            permissionDetails.Add(ProductionOrderBatch_Cancel_Approvement);
            permissionDetails.Add(ProductionOrderBatch_Split);
            permissionDetails.Add(ProductionOrderBatch_Join);
            permissionDetails.Add(ProductionOrderBatch_Edit_Placement_In_Containers);

            #endregion

            #region Транспортные листы

            var ProductionOrderTransportSheet_List_Details = new PermissionDetails(Permission.ProductionOrderTransportSheet_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrderTransportSheet_Create_Edit = new PermissionDetails(Permission.ProductionOrderTransportSheet_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrderTransportSheet_Delete = new PermissionDetails(Permission.ProductionOrderTransportSheet_Delete, pd_NonePersonalTeamsAll);

            ProductionOrderTransportSheet_List_Details.AddChildDirectRelation(ProductionOrderTransportSheet_Create_Edit);
            ProductionOrderTransportSheet_List_Details.AddChildDirectRelation(ProductionOrderTransportSheet_Delete);

            permissionDetails.Add(ProductionOrderTransportSheet_List_Details);
            permissionDetails.Add(ProductionOrderTransportSheet_Create_Edit);
            permissionDetails.Add(ProductionOrderTransportSheet_Delete);
            
            #endregion

            #region Листы дополнительных расходов

            var ProductionOrderExtraExpensesSheet_List_Details = new PermissionDetails(Permission.ProductionOrderExtraExpensesSheet_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrderExtraExpensesSheet_Create_Edit = new PermissionDetails(Permission.ProductionOrderExtraExpensesSheet_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrderExtraExpensesSheet_Delete = new PermissionDetails(Permission.ProductionOrderExtraExpensesSheet_Delete, pd_NonePersonalTeamsAll);

            ProductionOrderExtraExpensesSheet_List_Details.AddChildDirectRelation(ProductionOrderExtraExpensesSheet_Create_Edit);
            ProductionOrderExtraExpensesSheet_List_Details.AddChildDirectRelation(ProductionOrderExtraExpensesSheet_Delete);

            permissionDetails.Add(ProductionOrderExtraExpensesSheet_List_Details);
            permissionDetails.Add(ProductionOrderExtraExpensesSheet_Create_Edit);
            permissionDetails.Add(ProductionOrderExtraExpensesSheet_Delete);

            #endregion

            #region Таможенные листы

            var ProductionOrderCustomsDeclaration_List_Details = new PermissionDetails(Permission.ProductionOrderCustomsDeclaration_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrderCustomsDeclaration_Create_Edit = new PermissionDetails(Permission.ProductionOrderCustomsDeclaration_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrderCustomsDeclaration_Delete = new PermissionDetails(Permission.ProductionOrderCustomsDeclaration_Delete, pd_NonePersonalTeamsAll);

            ProductionOrderCustomsDeclaration_List_Details.AddChildDirectRelation(ProductionOrderCustomsDeclaration_Create_Edit);
            ProductionOrderCustomsDeclaration_List_Details.AddChildDirectRelation(ProductionOrderCustomsDeclaration_Delete);

            permissionDetails.Add(ProductionOrderCustomsDeclaration_List_Details);
            permissionDetails.Add(ProductionOrderCustomsDeclaration_Create_Edit);
            permissionDetails.Add(ProductionOrderCustomsDeclaration_Delete);

            #endregion

            #region Оплаты в заказах

            var ProductionOrderPayment_List_Details = new PermissionDetails(Permission.ProductionOrderPayment_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrderPayment_Create_Edit = new PermissionDetails(Permission.ProductionOrderPayment_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrderPayment_Delete = new PermissionDetails(Permission.ProductionOrderPayment_Delete, pd_NonePersonalTeamsAll);

            ProductionOrderPayment_List_Details.AddChildDirectRelation(ProductionOrderPayment_Create_Edit);
            ProductionOrderPayment_List_Details.AddChildDirectRelation(ProductionOrderPayment_Delete);

            permissionDetails.Add(ProductionOrderPayment_List_Details);
            permissionDetails.Add(ProductionOrderPayment_Create_Edit);
            permissionDetails.Add(ProductionOrderPayment_Delete);

            #endregion

            #region Пакеты материалов

            var ProductionOrderMaterialsPackage_List_Details = new PermissionDetails(Permission.ProductionOrderMaterialsPackage_List_Details, pd_NonePersonalTeamsAll);
            var ProductionOrderMaterialsPackage_Create_Edit = new PermissionDetails(Permission.ProductionOrderMaterialsPackage_Create_Edit, pd_NonePersonalTeamsAll);
            var ProductionOrderMaterialsPackage_Delete = new PermissionDetails(Permission.ProductionOrderMaterialsPackage_Delete, pd_NonePersonalTeamsAll);

            ProductionOrderMaterialsPackage_List_Details.AddChildDirectRelation(ProductionOrderMaterialsPackage_Create_Edit);
            ProductionOrderMaterialsPackage_List_Details.AddChildDirectRelation(ProductionOrderMaterialsPackage_Delete);

            permissionDetails.Add(ProductionOrderMaterialsPackage_List_Details);
            permissionDetails.Add(ProductionOrderMaterialsPackage_Create_Edit);
            permissionDetails.Add(ProductionOrderMaterialsPackage_Delete);

            #endregion

            #region Шаблоны этапов

            var ProductionOrderBatchLifeCycleTemplate_List_Details = new PermissionDetails(Permission.ProductionOrderBatchLifeCycleTemplate_List_Details, pd_NoneAll);
            var ProductionOrderBatchLifeCycleTemplate_Create_Edit = new PermissionDetails(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit, pd_NoneAll);
            var ProductionOrderBatchLifeCycleTemplate_Delete = new PermissionDetails(Permission.ProductionOrderBatchLifeCycleTemplate_Delete, pd_NoneAll);

            ProductionOrderBatchLifeCycleTemplate_List_Details.AddChildDirectRelation(ProductionOrderBatchLifeCycleTemplate_Create_Edit);
            ProductionOrderBatchLifeCycleTemplate_List_Details.AddChildDirectRelation(ProductionOrderBatchLifeCycleTemplate_Delete);

            permissionDetails.Add(ProductionOrderBatchLifeCycleTemplate_List_Details);
            permissionDetails.Add(ProductionOrderBatchLifeCycleTemplate_Create_Edit);
            permissionDetails.Add(ProductionOrderBatchLifeCycleTemplate_Delete);

            #endregion

            #endregion

            #region Права доступа к операциям выполнения продаж

            #region Клиенты

            var Client_List_Details = new PermissionDetails(Permission.Client_List_Details, pd_NoneAll);
            var Client_Create = new PermissionDetails(Permission.Client_Create, pd_NoneAll);
            var Client_Edit = new PermissionDetails(Permission.Client_Edit, pd_NoneAll);
            var Client_Delete = new PermissionDetails(Permission.Client_Delete, pd_NoneAll);
            var Client_Block = new PermissionDetails(Permission.Client_Block, pd_NoneAll);

            Client_Create.AddParentDirectRelation(Client_List_Details);
            Client_Edit.AddParentDirectRelation(Client_List_Details);
            Client_Delete.AddParentDirectRelation(Client_List_Details);
            Client_Block.AddParentDirectRelation(Client_List_Details);

            permissionDetails.Add(Client_List_Details);
            permissionDetails.Add(Client_Create);
            permissionDetails.Add(Client_Edit);
            permissionDetails.Add(Client_Delete);
            permissionDetails.Add(Client_Block);

            #endregion

            #region Организации клиентов

            var ClientOrganization_List_Details = new PermissionDetails(Permission.ClientOrganization_List_Details, pd_NoneAll);
            var Client_ClientOrganization_Add = new PermissionDetails(Permission.Client_ClientOrganization_Add, pd_NoneAll);
            var Client_ClientOrganization_Remove = new PermissionDetails(Permission.Client_ClientOrganization_Remove, pd_NoneAll);
            var ClientOrganization_Create = new PermissionDetails(Permission.ClientOrganization_Create, pd_NoneAll);
            var ClientOrganization_Edit = new PermissionDetails(Permission.ClientOrganization_Edit, pd_NoneAll);
            var ClientOrganization_BankAccount_Create = new PermissionDetails(Permission.ClientOrganization_BankAccount_Create, pd_NoneAll);
            var ClientOrganization_BankAccount_Edit = new PermissionDetails(Permission.ClientOrganization_BankAccount_Edit, pd_NoneAll);
            var ClientOrganization_BankAccount_Delete = new PermissionDetails(Permission.ClientOrganization_BankAccount_Delete, pd_NoneAll);
            var ClientOrganization_Delete = new PermissionDetails(Permission.ClientOrganization_Delete, pd_NoneAll);

            ClientOrganization_List_Details.AddParentDirectRelation(Client_List_Details);
            Client_ClientOrganization_Add.AddParentDirectRelation(ClientOrganization_List_Details);
            Client_ClientOrganization_Remove.AddParentDirectRelation(ClientOrganization_List_Details);
            ClientOrganization_Create.AddParentDirectRelation(Client_ClientOrganization_Add);
            ClientOrganization_Edit.AddParentDirectRelation(ClientOrganization_List_Details);
            ClientOrganization_BankAccount_Create.AddParentDirectRelation(ClientOrganization_List_Details);
            ClientOrganization_BankAccount_Edit.AddParentDirectRelation(ClientOrganization_List_Details);
            ClientOrganization_BankAccount_Delete.AddParentDirectRelation(ClientOrganization_List_Details);
            ClientOrganization_Delete.AddParentDirectRelation(ClientOrganization_List_Details);
            
            permissionDetails.Add(ClientOrganization_List_Details);
            permissionDetails.Add(Client_ClientOrganization_Add);
            permissionDetails.Add(Client_ClientOrganization_Remove);
            permissionDetails.Add(ClientOrganization_Create);
            permissionDetails.Add(ClientOrganization_Edit);            
            permissionDetails.Add(ClientOrganization_BankAccount_Create);
            permissionDetails.Add(ClientOrganization_BankAccount_Edit);
            permissionDetails.Add(ClientOrganization_BankAccount_Delete);
            permissionDetails.Add(ClientOrganization_Delete);

            #endregion

            #region Типы клиентов

            var ClientType_Create = new PermissionDetails(Permission.ClientType_Create, pd_NoneAll);
            var ClientType_Edit = new PermissionDetails(Permission.ClientType_Edit, pd_NoneAll);
            var ClientType_Delete = new PermissionDetails(Permission.ClientType_Delete, pd_NoneAll);

            ClientType_Create.AddParentDirectRelation(Client_List_Details);
            ClientType_Edit.AddParentDirectRelation(Client_List_Details);
            ClientType_Delete.AddParentDirectRelation(Client_List_Details);

            permissionDetails.Add(ClientType_Create);
            permissionDetails.Add(ClientType_Edit);
            permissionDetails.Add(ClientType_Delete);

            #endregion

            #region Программы обслуживания клиентов

            var ClientServiceProgram_Create = new PermissionDetails(Permission.ClientServiceProgram_Create, pd_NoneAll);
            var ClientServiceProgram_Edit = new PermissionDetails(Permission.ClientServiceProgram_Edit, pd_NoneAll);
            var ClientServiceProgram_Delete = new PermissionDetails(Permission.ClientServiceProgram_Delete, pd_NoneAll);

            ClientServiceProgram_Create.AddParentDirectRelation(Client_List_Details);
            ClientServiceProgram_Edit.AddParentDirectRelation(Client_List_Details);
            ClientServiceProgram_Delete.AddParentDirectRelation(Client_List_Details);

            permissionDetails.Add(ClientServiceProgram_Create);
            permissionDetails.Add(ClientServiceProgram_Edit);
            permissionDetails.Add(ClientServiceProgram_Delete);

            #endregion

            #region Регионы клиентов

            var ClientRegion_Create = new PermissionDetails(Permission.ClientRegion_Create, pd_NoneAll);
            var ClientRegion_Edit = new PermissionDetails(Permission.ClientRegion_Edit, pd_NoneAll);
            var ClientRegion_Delete = new PermissionDetails(Permission.ClientRegion_Delete, pd_NoneAll);

            ClientRegion_Create.AddParentDirectRelation(Client_List_Details);
            ClientRegion_Edit.AddParentDirectRelation(Client_List_Details);
            ClientRegion_Delete.AddParentDirectRelation(Client_List_Details);

            permissionDetails.Add(ClientRegion_Create);
            permissionDetails.Add(ClientRegion_Edit);
            permissionDetails.Add(ClientRegion_Delete);

            #endregion

            #region Сделки

            var Deal_List_Details = new PermissionDetails(Permission.Deal_List_Details, pd_NonePersonalTeamsAll);
            var Deal_Create_Edit = new PermissionDetails(Permission.Deal_Create_Edit, pd_NonePersonalTeamsAll);
            var Deal_Stage_Change = new PermissionDetails(Permission.Deal_Stage_Change, pd_NonePersonalTeamsAll);
            var Deal_Contract_Set = new PermissionDetails(Permission.Deal_Contract_Set, pd_NonePersonalTeamsAll);            
            var Deal_Curator_Change = new PermissionDetails(Permission.Deal_Curator_Change, pd_NonePersonalTeamsAll);
            var Deal_CreaditSum_View = new PermissionDetails(Permission.Deal_Balance_View, pd_NonePersonalTeamsAll);
            var Deal_Quota_List = new PermissionDetails(Permission.Deal_Quota_List, pd_NonePersonalTeamsAll);
            var Deal_Quota_Add = new PermissionDetails(Permission.Deal_Quota_Add, pd_NonePersonalTeamsAll);
            var Deal_Quota_Remove = new PermissionDetails(Permission.Deal_Quota_Remove, pd_NonePersonalTeamsAll);

            Deal_List_Details.AddChildDirectRelation(Deal_Create_Edit);            
            Deal_List_Details.AddChildDirectRelation(Deal_Stage_Change);
            Deal_List_Details.AddChildDirectRelation(Deal_Contract_Set);            
            Deal_List_Details.AddChildDirectRelation(Deal_Curator_Change);
            Deal_List_Details.AddChildDirectRelation(Deal_CreaditSum_View);
            Deal_List_Details.AddChildDirectRelation(Deal_Quota_List);
            Deal_List_Details.AddChildDirectRelation(Deal_Quota_Add);
            Deal_List_Details.AddChildDirectRelation(Deal_Quota_Remove);

            permissionDetails.Add(Deal_List_Details);
            permissionDetails.Add(Deal_Create_Edit);
            permissionDetails.Add(Deal_Stage_Change);
            permissionDetails.Add(Deal_Contract_Set);            
            permissionDetails.Add(Deal_Curator_Change);
            permissionDetails.Add(Deal_CreaditSum_View);
            permissionDetails.Add(Deal_Quota_List);
            permissionDetails.Add(Deal_Quota_Add);
            permissionDetails.Add(Deal_Quota_Remove);

            #endregion

            #region Договоры по сделке

            var ClientContract_Create = new PermissionDetails(Permission.ClientContract_Create, pd_NonePersonalTeamsAll);
            var ClientContract_Edit = new PermissionDetails(Permission.ClientContract_Edit, pd_NonePersonalTeamsAll);

            Deal_List_Details.AddChildDirectRelation(ClientContract_Create);
            Deal_List_Details.AddChildDirectRelation(ClientContract_Edit);

            permissionDetails.Add(ClientContract_Create);
            permissionDetails.Add(ClientContract_Edit);

            #endregion

            #region Накладные реализации товаров
            
            var ExpenditureWaybill_List_Details = new PermissionDetails(Permission.ExpenditureWaybill_List_Details, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Create_Edit = new PermissionDetails(Permission.ExpenditureWaybill_Create_Edit, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Curator_Change = new PermissionDetails(Permission.ExpenditureWaybill_Curator_Change, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Delete_Row_Delete = new PermissionDetails(Permission.ExpenditureWaybill_Delete_Row_Delete, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Accept_Deal_List = new PermissionDetails(Permission.ExpenditureWaybill_Accept_Deal_List, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Accept_Storage_List = new PermissionDetails(Permission.ExpenditureWaybill_Accept_Storage_List, pd_NoneTeamsAll);
            var ExpenditureWaybill_Cancel_Acceptance_Deal_List = new PermissionDetails(Permission.ExpenditureWaybill_Cancel_Acceptance_Deal_List, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Cancel_Acceptance_Storage_List = new PermissionDetails(Permission.ExpenditureWaybill_Cancel_Acceptance_Storage_List, pd_NoneTeamsAll);
            var ExpenditureWaybill_Ship_Deal_List = new PermissionDetails(Permission.ExpenditureWaybill_Ship_Deal_List, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Ship_Storage_List = new PermissionDetails(Permission.ExpenditureWaybill_Ship_Storage_List, pd_NoneTeamsAll);
            var ExpenditureWaybill_Cancel_Shipping_Deal_List = new PermissionDetails(Permission.ExpenditureWaybill_Cancel_Shipping_Deal_List, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Cancel_Shipping_Storage_List = new PermissionDetails(Permission.ExpenditureWaybill_Cancel_Shipping_Storage_List, pd_NoneTeamsAll);
            var ExpenditureWaybill_Date_Change = new PermissionDetails(Permission.ExpenditureWaybill_Date_Change, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Accept_Retroactively = new PermissionDetails(Permission.ExpenditureWaybill_Accept_Retroactively, pd_NonePersonalTeamsAll);
            var ExpenditureWaybill_Ship_Retroactively = new PermissionDetails(Permission.ExpenditureWaybill_Ship_Retroactively, pd_NonePersonalTeamsAll);
            
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Create_Edit);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Curator_Change);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Delete_Row_Delete);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Accept_Deal_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Accept_Storage_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Cancel_Acceptance_Deal_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Cancel_Acceptance_Storage_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Ship_Deal_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Ship_Storage_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Cancel_Shipping_Deal_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Cancel_Shipping_Storage_List);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Date_Change);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Accept_Retroactively);
            ExpenditureWaybill_List_Details.AddChildDirectRelation(ExpenditureWaybill_Ship_Retroactively);

            permissionDetails.Add(ExpenditureWaybill_List_Details);
            permissionDetails.Add(ExpenditureWaybill_Create_Edit);
            permissionDetails.Add(ExpenditureWaybill_Curator_Change);
            permissionDetails.Add(ExpenditureWaybill_Delete_Row_Delete);
            permissionDetails.Add(ExpenditureWaybill_Accept_Deal_List);
            permissionDetails.Add(ExpenditureWaybill_Accept_Storage_List);
            permissionDetails.Add(ExpenditureWaybill_Cancel_Acceptance_Deal_List);
            permissionDetails.Add(ExpenditureWaybill_Cancel_Acceptance_Storage_List);
            permissionDetails.Add(ExpenditureWaybill_Ship_Deal_List);
            permissionDetails.Add(ExpenditureWaybill_Ship_Storage_List);
            permissionDetails.Add(ExpenditureWaybill_Cancel_Shipping_Deal_List);
            permissionDetails.Add(ExpenditureWaybill_Cancel_Shipping_Storage_List);
            permissionDetails.Add(ExpenditureWaybill_Date_Change);
            permissionDetails.Add(ExpenditureWaybill_Accept_Retroactively);
            permissionDetails.Add(ExpenditureWaybill_Ship_Retroactively);

            #endregion

            #region Оплаты
            
            var DealPayment_List_Details = new PermissionDetails(Permission.DealPayment_List_Details, pd_NonePersonalTeamsAll);
            var DealPaymentFromClient_Create = new PermissionDetails(Permission.DealPaymentFromClient_Create_Edit, pd_NonePersonalTeamsAll);
            var DealPaymentToClient_Create = new PermissionDetails(Permission.DealPaymentToClient_Create, pd_NonePersonalTeamsAll);            
            var DealPaymentFromClient_Delete = new PermissionDetails(Permission.DealPaymentFromClient_Delete, pd_NonePersonalTeamsAll);
            var DealPaymentToClient_Delete = new PermissionDetails(Permission.DealPaymentToClient_Delete, pd_NonePersonalTeamsAll);
            var DealPayment_User_Change = new PermissionDetails(Permission.DealPayment_User_Change, pd_NonePersonalTeamsAll);
            var DealPayment_Date_Change = new PermissionDetails(Permission.DealPayment_Date_Change, pd_NoneAll);

            DealPayment_List_Details.AddChildDirectRelation(DealPaymentFromClient_Create);
            DealPayment_List_Details.AddChildDirectRelation(DealPaymentToClient_Create);
            DealPayment_List_Details.AddChildDirectRelation(DealPaymentFromClient_Delete);
            DealPayment_List_Details.AddChildDirectRelation(DealPaymentToClient_Delete);
            DealPayment_List_Details.AddChildDirectRelation(DealPayment_User_Change);
            DealPayment_List_Details.AddChildDirectRelation(DealPayment_Date_Change);

            permissionDetails.Add(DealPayment_List_Details);
            permissionDetails.Add(DealPaymentFromClient_Create);
            permissionDetails.Add(DealPaymentToClient_Create);
            permissionDetails.Add(DealPaymentFromClient_Delete);
            permissionDetails.Add(DealPaymentToClient_Delete);
            permissionDetails.Add(DealPayment_User_Change);
            permissionDetails.Add(DealPayment_Date_Change);

            #endregion

            #region Квоты

            var DealQuota_List_Details = new PermissionDetails(Permission.DealQuota_List_Details, pd_NoneAll);
            var DealQuota_Create = new PermissionDetails(Permission.DealQuota_Create, pd_NoneAll);
            var DealQuota_Edit = new PermissionDetails(Permission.DealQuota_Edit, pd_NoneAll);
            var DealQuota_Delete = new PermissionDetails(Permission.DealQuota_Delete, pd_NoneAll);

            DealQuota_List_Details.AddParentDirectRelation(Deal_List_Details);
            DealQuota_List_Details.AddChildDirectRelation(DealQuota_Create);
            DealQuota_List_Details.AddChildDirectRelation(DealQuota_Edit);
            DealQuota_List_Details.AddChildDirectRelation(DealQuota_Delete);
            
            permissionDetails.Add(DealQuota_List_Details);
            permissionDetails.Add(DealQuota_Create);
            permissionDetails.Add(DealQuota_Edit);
            permissionDetails.Add(DealQuota_Delete);

            #endregion

            #region Возвраты от клиентов
            
            var ReturnFromClientWaybill_List_Details = new PermissionDetails(Permission.ReturnFromClientWaybill_List_Details, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Create_Edit = new PermissionDetails(Permission.ReturnFromClientWaybill_Create_Edit, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Curator_Change = new PermissionDetails(Permission.ReturnFromClientWaybill_Curator_Change, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Delete_Row_Delete = new PermissionDetails(Permission.ReturnFromClientWaybill_Delete_Row_Delete, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Accept_Deal_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Accept_Deal_List, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Accept_Storage_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Accept_Storage_List, pd_NoneTeamsAll);
            var ReturnFromClientWaybill_Acceptance_Cancel_Deal_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Acceptance_Cancel_Deal_List, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Acceptance_Cancel_Storage_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Acceptance_Cancel_Storage_List, pd_NoneTeamsAll);
            var ReturnFromClientWaybill_Receipt_Deal_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Receipt_Deal_List, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Receipt_Storage_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Receipt_Storage_List, pd_NoneTeamsAll);
            var ReturnFromClientWaybill_Receipt_Cancel_Deal_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Receipt_Cancel_Deal_List, pd_NonePersonalTeamsAll);
            var ReturnFromClientWaybill_Receipt_Cancel_Storage_List = new PermissionDetails(Permission.ReturnFromClientWaybill_Receipt_Cancel_Storage_List, pd_NoneTeamsAll);

            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Create_Edit);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Curator_Change);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Delete_Row_Delete);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Accept_Deal_List);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Accept_Storage_List);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Acceptance_Cancel_Deal_List);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Acceptance_Cancel_Storage_List);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Receipt_Deal_List);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Receipt_Storage_List);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Receipt_Cancel_Deal_List);
            ReturnFromClientWaybill_List_Details.AddChildDirectRelation(ReturnFromClientWaybill_Receipt_Cancel_Storage_List);

            permissionDetails.Add(ReturnFromClientWaybill_List_Details);
            permissionDetails.Add(ReturnFromClientWaybill_Create_Edit);
            permissionDetails.Add(ReturnFromClientWaybill_Curator_Change);
            permissionDetails.Add(ReturnFromClientWaybill_Delete_Row_Delete);
            permissionDetails.Add(ReturnFromClientWaybill_Accept_Deal_List);
            permissionDetails.Add(ReturnFromClientWaybill_Accept_Storage_List);
            permissionDetails.Add(ReturnFromClientWaybill_Acceptance_Cancel_Deal_List);
            permissionDetails.Add(ReturnFromClientWaybill_Acceptance_Cancel_Storage_List);
            permissionDetails.Add(ReturnFromClientWaybill_Receipt_Deal_List);
            permissionDetails.Add(ReturnFromClientWaybill_Receipt_Storage_List);
            permissionDetails.Add(ReturnFromClientWaybill_Receipt_Cancel_Deal_List);
            permissionDetails.Add(ReturnFromClientWaybill_Receipt_Cancel_Storage_List);
            
            #endregion

            #region Основание для возврата

            var ReturnFromClientReason_Create = new PermissionDetails(Permission.ReturnFromClientReason_Create, pd_NoneAll);
            var ReturnFromClientReason_Edit = new PermissionDetails(Permission.ReturnFromClientReason_Edit, pd_NoneAll);
            var ReturnFromClientReason_Delete = new PermissionDetails(Permission.ReturnFromClientReason_Delete, pd_NoneAll);

            ReturnFromClientReason_Create.AddParentDirectRelation(ReturnFromClientWaybill_List_Details);
            ReturnFromClientReason_Edit.AddParentDirectRelation(ReturnFromClientWaybill_List_Details);
            ReturnFromClientReason_Delete.AddParentDirectRelation(ReturnFromClientWaybill_List_Details);

            permissionDetails.Add(ReturnFromClientReason_Create);
            permissionDetails.Add(ReturnFromClientReason_Edit);
            permissionDetails.Add(ReturnFromClientReason_Delete);

            #endregion

            #region Корректировки сальдо

            var DealInitialBalanceCorrection_List_Details = new PermissionDetails(Permission.DealInitialBalanceCorrection_List_Details, pd_NonePersonalTeamsAll);
            var DealCreditInitialBalanceCorrection_Create_Edit = new PermissionDetails(Permission.DealCreditInitialBalanceCorrection_Create_Edit, pd_NonePersonalTeamsAll);
            var DealDebitInitialBalanceCorrection_Create = new PermissionDetails(Permission.DealDebitInitialBalanceCorrection_Create, pd_NonePersonalTeamsAll);
            var DealCreditInitialBalanceCorrection_Delete = new PermissionDetails(Permission.DealCreditInitialBalanceCorrection_Delete, pd_NonePersonalTeamsAll);
            var DealDebitInitialBalanceCorrection_Delete = new PermissionDetails(Permission.DealDebitInitialBalanceCorrection_Delete, pd_NonePersonalTeamsAll);
            var DealInitialBalanceCorrection_Date_Change = new PermissionDetails(Permission.DealInitialBalanceCorrection_Date_Change, pd_NoneAll);

            DealInitialBalanceCorrection_List_Details.AddChildDirectRelation(DealCreditInitialBalanceCorrection_Create_Edit);
            DealInitialBalanceCorrection_List_Details.AddChildDirectRelation(DealDebitInitialBalanceCorrection_Create);
            DealInitialBalanceCorrection_List_Details.AddChildDirectRelation(DealCreditInitialBalanceCorrection_Delete);
            DealInitialBalanceCorrection_List_Details.AddChildDirectRelation(DealDebitInitialBalanceCorrection_Delete);
            DealInitialBalanceCorrection_List_Details.AddChildDirectRelation(DealInitialBalanceCorrection_Date_Change);

            permissionDetails.Add(DealInitialBalanceCorrection_List_Details);
            permissionDetails.Add(DealCreditInitialBalanceCorrection_Create_Edit);
            permissionDetails.Add(DealDebitInitialBalanceCorrection_Create);
            permissionDetails.Add(DealCreditInitialBalanceCorrection_Delete);
            permissionDetails.Add(DealDebitInitialBalanceCorrection_Delete);
            permissionDetails.Add(DealInitialBalanceCorrection_Date_Change);
            
            #endregion        

            #endregion

            #region Права доступа к операциям работы со справочниками

            #region Товары

            var Article_List_Details = new PermissionDetails(Permission.Article_List_Details, pd_NoneAll);
            var Article_Create = new PermissionDetails(Permission.Article_Create, pd_NoneAll);
            var Article_Edit = new PermissionDetails(Permission.Article_Edit, pd_NoneAll);
            var Article_Delete = new PermissionDetails(Permission.Article_Delete, pd_NoneAll);

            Article_List_Details.AddChildDirectRelation(Article_Create);
            Article_List_Details.AddChildDirectRelation(Article_Edit);
            Article_List_Details.AddChildDirectRelation(Article_Delete);

            permissionDetails.Add(Article_List_Details);
            permissionDetails.Add(Article_Create);
            permissionDetails.Add(Article_Edit);
            permissionDetails.Add(Article_Delete);

            #endregion

            #region Группы товаров

            var ArticleGroup_Create = new PermissionDetails(Permission.ArticleGroup_Create, pd_NoneAll);
            permissionDetails.Add(ArticleGroup_Create);

            var ArticleGroup_Edit = new PermissionDetails(Permission.ArticleGroup_Edit, pd_NoneAll);
            permissionDetails.Add(ArticleGroup_Edit);

            var ArticleGroup_Delete = new PermissionDetails(Permission.ArticleGroup_Delete, pd_NoneAll);
            permissionDetails.Add(ArticleGroup_Delete);

            #endregion

            #region Торговые марки

            var Trademark_Create = new PermissionDetails(Permission.Trademark_Create, pd_NoneAll);
            permissionDetails.Add(Trademark_Create);

            var Trademark_Edit = new PermissionDetails(Permission.Trademark_Edit, pd_NoneAll);
            permissionDetails.Add(Trademark_Edit);

            var Trademark_Delete = new PermissionDetails(Permission.Trademark_Delete, pd_NoneAll);
            permissionDetails.Add(Trademark_Delete);

            #endregion

            #region Изготовители

            var Manufacturer_Create = new PermissionDetails(Permission.Manufacturer_Create, pd_NoneAll);
            permissionDetails.Add(Manufacturer_Create);

            var Manufacturer_Edit = new PermissionDetails(Permission.Manufacturer_Edit, pd_NoneAll);
            permissionDetails.Add(Manufacturer_Edit);

            var Manufacturer_Delete = new PermissionDetails(Permission.Manufacturer_Delete, pd_NoneAll);
            permissionDetails.Add(Manufacturer_Delete);

            #endregion

            #region Страны

            var Country_Create = new PermissionDetails(Permission.Country_Create, pd_NoneAll);
            permissionDetails.Add(Country_Create);

            var Country_Edit = new PermissionDetails(Permission.Country_Edit, pd_NoneAll);
            permissionDetails.Add(Country_Edit);

            var Country_Delete = new PermissionDetails(Permission.Country_Delete, pd_NoneAll);
            permissionDetails.Add(Country_Delete);

            #endregion

            #region Единицы измерения

            var MeasureUnit_Create = new PermissionDetails(Permission.MeasureUnit_Create, pd_NoneAll);
            permissionDetails.Add(MeasureUnit_Create);

            var MeasureUnit_Edit = new PermissionDetails(Permission.MeasureUnit_Edit, pd_NoneAll);
            permissionDetails.Add(MeasureUnit_Edit);

            var MeasureUnit_Delete = new PermissionDetails(Permission.MeasureUnit_Delete, pd_NoneAll);
            permissionDetails.Add(MeasureUnit_Delete);

            #endregion

            #region Банки

            var Bank_Create = new PermissionDetails(Permission.Bank_Create, pd_NoneAll);
            permissionDetails.Add(Bank_Create);

            var Bank_Edit = new PermissionDetails(Permission.Bank_Edit, pd_NoneAll);
            permissionDetails.Add(Bank_Edit);

            var Bank_Delete = new PermissionDetails(Permission.Bank_Delete, pd_NoneAll);
            permissionDetails.Add(Bank_Delete);

            #endregion

            #region Валюты

            var Currency_Create = new PermissionDetails(Permission.Currency_Create, pd_NoneAll);
            permissionDetails.Add(Currency_Create);

            var Currency_Edit = new PermissionDetails(Permission.Currency_Edit, pd_NoneAll);
            permissionDetails.Add(Currency_Edit);

            var Currency_AddRate = new PermissionDetails(Permission.Currency_AddRate, pd_NoneAll);
            permissionDetails.Add(Currency_AddRate);

            var Currency_Delete = new PermissionDetails(Permission.Currency_Delete, pd_NoneAll);
            permissionDetails.Add(Currency_Delete);

            #endregion

            #region Сертификаты товаров

            var ArticleCertificate_Create = new PermissionDetails(Permission.ArticleCertificate_Create, pd_NoneAll);
            permissionDetails.Add(ArticleCertificate_Create);

            var ArticleCertificate_Edit = new PermissionDetails(Permission.ArticleCertificate_Edit, pd_NoneAll);
            permissionDetails.Add(ArticleCertificate_Edit);

            var ArticleCertificate_Delete = new PermissionDetails(Permission.ArticleCertificate_Delete, pd_NoneAll);
            permissionDetails.Add(ArticleCertificate_Delete);
            
            #endregion

            #region Ставки НДС

            var ValueAddedTax_Create = new PermissionDetails(Permission.ValueAddedTax_Create, pd_NoneAll);
            permissionDetails.Add(ValueAddedTax_Create);

            var ValueAddedTax_Edit = new PermissionDetails(Permission.ValueAddedTax_Edit, pd_NoneAll);
            permissionDetails.Add(ValueAddedTax_Edit);

            var ValueAddedTax_Delete = new PermissionDetails(Permission.ValueAddedTax_Delete, pd_NoneAll);
            permissionDetails.Add(ValueAddedTax_Delete);

            #endregion

            #region организационно-правовые формы

            var LegalForm_Create = new PermissionDetails(Permission.LegalForm_Create, pd_NoneAll);
            permissionDetails.Add(LegalForm_Create);

            var LegalForm_Edit = new PermissionDetails(Permission.LegalForm_Edit, pd_NoneAll);
            permissionDetails.Add(LegalForm_Edit);

            var LegalForm_Delete = new PermissionDetails(Permission.LegalForm_Delete, pd_NoneAll);
            permissionDetails.Add(LegalForm_Delete);

            #endregion

            #endregion

            #region Права доступа к операциям работы с пользователями

            #region Пользователи

            var User_List_Details = new PermissionDetails(Permission.User_List_Details, pd_NoneTeamsAll);
            var User_Create = new PermissionDetails(Permission.User_Create, pd_NoneAll);
            var User_Edit = new PermissionDetails(Permission.User_Edit, pd_NoneTeamsAll);
            var User_Role_Add = new PermissionDetails(Permission.User_Role_Add, pd_NoneTeamsAll);
            var User_Role_Remove = new PermissionDetails(Permission.User_Role_Remove, pd_NoneTeamsAll);            
            var User_Delete = new PermissionDetails(Permission.User_Delete, pd_NoneTeamsAll);

            User_List_Details.AddChildDirectRelation(User_Create);
            User_List_Details.AddChildDirectRelation(User_Edit);
            User_List_Details.AddChildDirectRelation(User_Role_Add);
            User_List_Details.AddChildDirectRelation(User_Role_Remove);
            User_List_Details.AddChildDirectRelation(User_Delete);

            permissionDetails.Add(User_List_Details);            
            permissionDetails.Add(User_Create);
            permissionDetails.Add(User_Edit);
            permissionDetails.Add(User_Role_Add);
            permissionDetails.Add(User_Role_Remove);
            permissionDetails.Add(User_Delete);

            #endregion

            #region Команды

            var Team_List_Details = new PermissionDetails(Permission.Team_List_Details, pd_NoneTeamsAll);
            var Team_Create = new PermissionDetails(Permission.Team_Create, pd_NoneAll);
            var Team_Edit = new PermissionDetails(Permission.Team_Edit, pd_NoneTeamsAll);
            var Team_Storage_Add = new PermissionDetails(Permission.Team_Storage_Add, pd_NoneTeamsAll);
            var Team_Storage_Remove = new PermissionDetails(Permission.Team_Storage_Remove, pd_NoneTeamsAll);
            var Team_ProductionOrder_Add = new PermissionDetails(Permission.Team_ProductionOrder_Add, pd_NoneTeamsAll);
            var Team_ProductionOrder_Remove = new PermissionDetails(Permission.Team_ProductionOrder_Remove, pd_NoneTeamsAll);
            var Team_Deal_Add = new PermissionDetails(Permission.Team_Deal_Add, pd_NoneTeamsAll);
            var Team_Deal_Remove = new PermissionDetails(Permission.Team_Deal_Remove, pd_NoneTeamsAll);
            var Team_User_Add = new PermissionDetails(Permission.Team_User_Add, pd_NoneTeamsAll);
            var Team_User_Remove = new PermissionDetails(Permission.Team_User_Remove, pd_NoneTeamsAll);
            var Team_Delete = new PermissionDetails(Permission.Team_Delete, pd_NoneTeamsAll);

            Team_List_Details.AddChildDirectRelation(Team_Create);
            Team_List_Details.AddChildDirectRelation(Team_Edit);
            Team_List_Details.AddChildDirectRelation(Team_Storage_Add);
            Team_List_Details.AddChildDirectRelation(Team_Storage_Remove);
            Team_List_Details.AddChildDirectRelation(Team_ProductionOrder_Add);
            Team_List_Details.AddChildDirectRelation(Team_ProductionOrder_Remove);
            Team_List_Details.AddChildDirectRelation(Team_Deal_Add);
            Team_List_Details.AddChildDirectRelation(Team_Deal_Remove);
            Team_List_Details.AddChildDirectRelation(Team_User_Add);
            Team_List_Details.AddChildDirectRelation(Team_User_Remove);
            Team_List_Details.AddChildDirectRelation(Team_Delete);

            permissionDetails.Add(Team_List_Details);
            permissionDetails.Add(Team_Create);
            permissionDetails.Add(Team_Edit);
            permissionDetails.Add(Team_Storage_Add);
            permissionDetails.Add(Team_Storage_Remove);
            permissionDetails.Add(Team_ProductionOrder_Add);
            permissionDetails.Add(Team_ProductionOrder_Remove);
            permissionDetails.Add(Team_Deal_Add);
            permissionDetails.Add(Team_Deal_Remove);
            permissionDetails.Add(Team_User_Add);
            permissionDetails.Add(Team_User_Remove);
            permissionDetails.Add(Team_Delete);

            #endregion

            #region Роль

            var Role_List_Details = new PermissionDetails(Permission.Role_List_Details, pd_NoneAll);
            var Role_Create = new PermissionDetails(Permission.Role_Create, pd_NoneAll);
            var Role_Edit = new PermissionDetails(Permission.Role_Edit, pd_NoneAll);
            var Role_Delete = new PermissionDetails(Permission.Role_Delete, pd_NoneAll);

            Role_List_Details.AddChildDirectRelation(Role_Create);
            Role_List_Details.AddChildDirectRelation(Role_Edit);
            Role_List_Details.AddChildDirectRelation(Role_Delete);

            permissionDetails.Add(Role_List_Details);
            permissionDetails.Add(Role_Create);
            permissionDetails.Add(Role_Edit);
            permissionDetails.Add(Role_Delete);

            #endregion

            #region Должности пользователя

            var EmployeePost_Create = new PermissionDetails(Permission.EmployeePost_Create, pd_NoneAll);
            permissionDetails.Add(EmployeePost_Create);

            var EmployeePost_Edit = new PermissionDetails(Permission.EmployeePost_Edit, pd_NoneAll);
            permissionDetails.Add(EmployeePost_Edit);

            var EmployeePost_Delete = new PermissionDetails(Permission.EmployeePost_Delete, pd_NoneAll);
            permissionDetails.Add(EmployeePost_Delete);

            #endregion

            #endregion

            #region Права доступа к отчетам

            #region Наличие товаров на местах хранения

            var Report0001_View = new PermissionDetails(Permission.Report0001_View, pd_NoneAll);
            var Report0001_Storage_List = new PermissionDetails(Permission.Report0001_Storage_List, pd_NoneTeamsAll);

            Report0001_View.AddChildDirectRelation(Report0001_Storage_List);

            permissionDetails.Add(Report0001_View);
            permissionDetails.Add(Report0001_Storage_List);

            #endregion

            #region Реализация товаров

            var Report0002_View = new PermissionDetails(Permission.Report0002_View, pd_NoneAll);
            var Report0002_Storage_List = new PermissionDetails(Permission.Report0002_Storage_List, pd_NoneTeamsAll);
            var Report0002_User_List = new PermissionDetails(Permission.Report0002_User_List, pd_NonePersonalTeamsAll);

            Report0002_View.AddChildDirectRelation(Report0002_Storage_List);
            Report0002_View.AddChildDirectRelation(Report0002_User_List);

            permissionDetails.Add(Report0002_View);
            permissionDetails.Add(Report0002_Storage_List);
            permissionDetails.Add(Report0002_User_List);

            #endregion

            #region Финансовый отчет

            var Report0003_View = new PermissionDetails(Permission.Report0003_View, pd_NoneAll);
            var Report0003_Storage_List = new PermissionDetails(Permission.Report0003_Storage_List, pd_NoneTeamsAll);

            Report0003_View.AddChildDirectRelation(Report0003_Storage_List);

            permissionDetails.Add(Report0003_View);
            permissionDetails.Add(Report0003_Storage_List);

            #endregion

            #region Движение товара за период

            var Report0004_View = new PermissionDetails(Permission.Report0004_View, pd_NoneAll);
            var Report0004_Storage_List = new PermissionDetails(Permission.Report0004_Storage_List, pd_NoneTeamsAll);

            Report0004_View.AddChildDirectRelation(Report0004_Storage_List);

            permissionDetails.Add(Report0004_View);
            permissionDetails.Add(Report0004_Storage_List);

            #endregion

            #region Карта движения товара

            var Report0005_View = new PermissionDetails(Permission.Report0005_View, pd_NoneAll);
            var Report0005_Storage_List = new PermissionDetails(Permission.Report0005_Storage_List, pd_NoneTeamsAll);

            Report0005_View.AddChildDirectRelation(Report0005_Storage_List);

            permissionDetails.Add(Report0005_View);
            permissionDetails.Add(Report0005_Storage_List);

            #endregion

            #region Отчет по взаиморасчетам с клиентами

            var Report0006_View = new PermissionDetails(Permission.Report0006_View, pd_NoneAll);

            permissionDetails.Add(Report0006_View);

            #endregion

            #region Отчет по взаиморасчетам по реализациям

            var Report0007_View = new PermissionDetails(Permission.Report0007_View, pd_NoneAll);
            var Report0007_Storage_List = new PermissionDetails(Permission.Report0007_Storage_List, pd_NoneTeamsAll);
            var Report0007_Date_Change = new PermissionDetails(Permission.Report0007_Date_Change, pd_NoneAll);

            Report0007_View.AddChildDirectRelation(Report0007_Storage_List);
            Report0007_View.AddChildDirectRelation(Report0007_Date_Change);
            
            permissionDetails.Add(Report0007_View);
            permissionDetails.Add(Report0007_Storage_List);
            permissionDetails.Add(Report0007_Date_Change);
            
            #endregion
            
            #region Реестр накладных

            var Report0008_View = new PermissionDetails(Permission.Report0008_View, pd_NoneAll);
            var Report0008_Storage_List = new PermissionDetails(Permission.Report0008_Storage_List, pd_NoneTeamsAll);

            Report0008_View.AddChildDirectRelation(Report0008_Storage_List);

            permissionDetails.Add(Report0008_View);
            permissionDetails.Add(Report0008_Storage_List);

            #endregion

            #region Отчет по поставкам

            var Report0009_View = new PermissionDetails(Permission.Report0009_View, pd_NoneAll);
            var Report0009_User_List = new PermissionDetails(Permission.Report0009_User_List, pd_NonePersonalTeamsAll);
            var Report0009_Storage_List = new PermissionDetails(Permission.Report0009_Storage_List, pd_NoneTeamsAll);

            Report0009_View.AddChildDirectRelation(Report0009_User_List);
            Report0009_View.AddChildDirectRelation(Report0009_Storage_List);

            permissionDetails.Add(Report0009_View);
            permissionDetails.Add(Report0009_User_List);
            permissionDetails.Add(Report0009_Storage_List);

			#endregion

            #region Принятые платежи

            var Report0010_View = new PermissionDetails(Permission.Report0010_View, pd_NoneAll);

            permissionDetails.Add(Report0010_View);
            #endregion

            #region Права доступа к выгрузки в 1С

            var ExportTo1C = new PermissionDetails(Permission.ExportTo1C, pd_NoneAll);

            permissionDetails.Add(ExportTo1C);

            #endregion

            #endregion

            #region Права доступа к задачам

            var Task_Create = new PermissionDetails(Permission.Task_Create, pd_NonePersonalTeamsAll);
            var Task_CreatedBy_List_Details = new PermissionDetails(Permission.Task_CreatedBy_List_Details, pd_NonePersonalTeamsAll);
            var Task_Delete = new PermissionDetails(Permission.Task_Delete, pd_NonePersonalTeamsAll);
            var Task_Edit = new PermissionDetails(Permission.Task_Edit, pd_NonePersonalTeamsAll);
            var Task_ExecutedBy_List_Details = new PermissionDetails(Permission.Task_ExecutedBy_List_Details, pd_NonePersonalTeamsAll);
            var Task_TaskExecutionItem_Edit_Delete = new PermissionDetails(Permission.Task_TaskExecutionItem_Edit_Delete, pd_NonePersonalTeamsAll);
            var TaskExecutionItem_Create = new PermissionDetails(Permission.TaskExecutionItem_Create, pd_NonePersonalTeamsAll);
            var TaskExecutionItem_Delete = new PermissionDetails(Permission.TaskExecutionItem_Delete, pd_NonePersonalTeamsAll);
            var TaskExecutionItem_Edit = new PermissionDetails(Permission.TaskExecutionItem_Edit, pd_NonePersonalTeamsAll);
            var TaskExecutionItem_EditExecutionDate = new PermissionDetails(Permission.TaskExecutionItem_EditExecutionDate, pd_NonePersonalTeamsAll);

            permissionDetails.Add(Task_Create);
            permissionDetails.Add(Task_CreatedBy_List_Details);
            permissionDetails.Add(Task_Delete);
            permissionDetails.Add(Task_Edit);
            permissionDetails.Add(Task_ExecutedBy_List_Details);
            permissionDetails.Add(Task_TaskExecutionItem_Edit_Delete);
            permissionDetails.Add(TaskExecutionItem_Create);
            permissionDetails.Add(TaskExecutionItem_Delete);
            permissionDetails.Add(TaskExecutionItem_Edit);
            permissionDetails.Add(TaskExecutionItem_EditExecutionDate);
            

            #endregion

    

            // var  = new PermissionDetails(Permission., pd_NonePersonalTeamsAll);
            // permissionDetails.Add();
        }
    }
}
