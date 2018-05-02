
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class ArticleDistributionPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }

        #region Поставщики

        public byte Provider_List_Details { get; set; }
        public PermissionViewModel Provider_List_Details_ViewModel { get; set; }

        public byte Provider_Create { get; set; }
        public PermissionViewModel Provider_Create_ViewModel { get; set; }

        public byte Provider_Edit { get; set; }
        public PermissionViewModel Provider_Edit_ViewModel { get; set; }

        public byte Provider_ProviderContract_Create { get; set; }
        public PermissionViewModel Provider_ProviderContract_Create_ViewModel { get; set; }

        public byte Provider_ProviderContract_Edit { get; set; }
        public PermissionViewModel Provider_ProviderContract_Edit_ViewModel { get; set; }

        public byte Provider_ProviderContract_Delete { get; set; }
        public PermissionViewModel Provider_ProviderContract_Delete_ViewModel { get; set; }
        
        public byte Provider_Delete { get; set; }
        public PermissionViewModel Provider_Delete_ViewModel { get; set; }

        #endregion

        #region Организации поставщиков

        public byte ProviderOrganization_List_Details { get; set; }
        public PermissionViewModel ProviderOrganization_List_Details_ViewModel { get; set; }

        public byte Provider_ProviderOrganization_Add { get; set; }
        public PermissionViewModel Provider_ProviderOrganization_Add_ViewModel { get; set; }

        public byte Provider_ProviderOrganization_Remove { get; set; }
        public PermissionViewModel Provider_ProviderOrganization_Remove_ViewModel { get; set; }

        public byte ProviderOrganization_Create { get; set; }
        public PermissionViewModel ProviderOrganization_Create_ViewModel { get; set; }

        public byte ProviderOrganization_Edit { get; set; }
        public PermissionViewModel ProviderOrganization_Edit_ViewModel { get; set; }

        public byte ProviderOrganization_BankAccount_Create { get; set; }
        public PermissionViewModel ProviderOrganization_BankAccount_Create_ViewModel { get; set; }

        public byte ProviderOrganization_BankAccount_Edit { get; set; }
        public PermissionViewModel ProviderOrganization_BankAccount_Edit_ViewModel { get; set; }

        public byte ProviderOrganization_BankAccount_Delete { get; set; }
        public PermissionViewModel ProviderOrganization_BankAccount_Delete_ViewModel { get; set; }
        
        public byte ProviderOrganization_Delete { get; set; }
        public PermissionViewModel ProviderOrganization_Delete_ViewModel { get; set; }

        #endregion

        #region Типы поставщиков

        public byte ProviderType_Create { get; set; }
        public PermissionViewModel ProviderType_Create_ViewModel { get; set; }

        public byte ProviderType_Edit { get; set; }
        public PermissionViewModel ProviderType_Edit_ViewModel { get; set; }

        public byte ProviderType_Delete { get; set; }
        public PermissionViewModel ProviderType_Delete_ViewModel { get; set; }

        #endregion
        
        #region Собственные организации

        public byte AccountOrganization_Create { get; set; }
        public PermissionViewModel AccountOrganization_Create_ViewModel { get; set; }

        public byte AccountOrganization_Edit { get; set; }
        public PermissionViewModel AccountOrganization_Edit_ViewModel { get; set; }

        public byte AccountOrganization_BankAccount_Create { get; set; }
        public PermissionViewModel AccountOrganization_BankAccount_Create_ViewModel { get; set; }

        public byte AccountOrganization_BankAccount_Edit { get; set; }
        public PermissionViewModel AccountOrganization_BankAccount_Edit_ViewModel { get; set; }

        public byte AccountOrganization_BankAccount_Delete { get; set; }
        public PermissionViewModel AccountOrganization_BankAccount_Delete_ViewModel { get; set; }

        public byte AccountOrganization_Delete { get; set; }
        public PermissionViewModel AccountOrganization_Delete_ViewModel { get; set; } 
        #endregion
                
        #region Места хранения
        
        public byte Storage_List_Details { get; set; }
        public PermissionViewModel Storage_List_Details_ViewModel { get; set; }

        public byte Storage_Create { get; set; }
        public PermissionViewModel Storage_Create_ViewModel { get; set; }

        public byte Storage_Edit { get; set; }
        public PermissionViewModel Storage_Edit_ViewModel { get; set; }

        public byte Storage_AccountOrganization_Add { get; set; }
        public PermissionViewModel Storage_AccountOrganization_Add_ViewModel { get; set; }

        public byte Storage_AccountOrganization_Remove { get; set; }
        public PermissionViewModel Storage_AccountOrganization_Remove_ViewModel { get; set; }

        public byte Storage_Section_Create_Edit { get; set; }
        public PermissionViewModel Storage_Section_Create_Edit_ViewModel { get; set; }

        public byte Storage_Section_Delete { get; set; }
        public PermissionViewModel Storage_Section_Delete_ViewModel { get; set; }

        public byte Storage_Delete { get; set; }
        public PermissionViewModel Storage_Delete_ViewModel { get; set; } 
        #endregion

        #region Приходные накладные

        public byte ReceiptWaybill_List_Details { get; set; }
        public PermissionViewModel ReceiptWaybill_List_Details_ViewModel { get; set; }

        public byte ReceiptWaybill_Create_Edit { get; set; }
        public PermissionViewModel ReceiptWaybill_Create_Edit_ViewModel { get; set; }

        /*public byte ReceiptWaybill_Provider_Storage_Change { get; set; }
        public PermissionViewModel ReceiptWaybill_Provider_Storage_Change_ViewModel { get; set; }  */      

        public byte ReceiptWaybill_Curator_Change { get; set; }
        public PermissionViewModel ReceiptWaybill_Curator_Change_ViewModel { get; set; }

        public byte ReceiptWaybill_Delete_Row_Delete { get; set; }
        public PermissionViewModel ReceiptWaybill_Delete_Row_Delete_ViewModel { get; set; }

        public byte ReceiptWaybill_ProviderDocuments_Edit { get; set; }
        public PermissionViewModel ReceiptWaybill_ProviderDocuments_Edit_ViewModel { get; set; }

        public byte ReceiptWaybill_Accept { get; set; }
        public PermissionViewModel ReceiptWaybill_Accept_ViewModel { get; set; }

        public byte ReceiptWaybill_Acceptance_Cancel { get; set; }
        public PermissionViewModel ReceiptWaybill_Acceptance_Cancel_ViewModel { get; set; } 

        public byte ReceiptWaybill_Receipt { get; set; }
        public PermissionViewModel ReceiptWaybill_Receipt_ViewModel { get; set; }

        public byte ReceiptWaybill_Receipt_Cancel { get; set; }
        public PermissionViewModel ReceiptWaybill_Receipt_Cancel_ViewModel { get; set; }

        public byte ReceiptWaybill_Approve { get; set; }
        public PermissionViewModel ReceiptWaybill_Approve_ViewModel { get; set; }

        public byte ReceiptWaybill_Approvement_Cancel { get; set; }
        public PermissionViewModel ReceiptWaybill_Approvement_Cancel_ViewModel { get; set; }

        public byte ReceiptWaybill_CreateFromProductionOrderBatch { get; set; }
        public PermissionViewModel ReceiptWaybill_CreateFromProductionOrderBatch_ViewModel { get; set; }

        public byte ReceiptWaybill_Date_Change { get; set; }
        public PermissionViewModel ReceiptWaybill_Date_Change_ViewModel { get; set; }

        public byte ReceiptWaybill_Accept_Retroactively { get; set; }
        public PermissionViewModel ReceiptWaybill_Accept_Retroactively_ViewModel { get; set; }

        public byte ReceiptWaybill_Receipt_Retroactively { get; set; }
        public PermissionViewModel ReceiptWaybill_Receipt_Retroactively_ViewModel { get; set; }

        public byte ReceiptWaybill_Approve_Retroactively { get; set; }
        public PermissionViewModel ReceiptWaybill_Approve_Retroactively_ViewModel { get; set; }

        #endregion

        #region Накладные перемещения
        
        public byte MovementWaybill_List_Details { get; set; }
        public PermissionViewModel MovementWaybill_List_Details_ViewModel { get; set; }

        public byte MovementWaybill_Create_Edit { get; set; }
        public PermissionViewModel MovementWaybill_Create_Edit_ViewModel { get; set; }

        public byte MovementWaybill_Curator_Change { get; set; }
        public PermissionViewModel MovementWaybill_Curator_Change_ViewModel { get; set; }

        public byte MovementWaybill_Delete_Row_Delete { get; set; }
        public PermissionViewModel MovementWaybill_Delete_Row_Delete_ViewModel { get; set; }

        public byte MovementWaybill_Accept { get; set; }
        public PermissionViewModel MovementWaybill_Accept_ViewModel { get; set; }

        public byte MovementWaybill_Acceptance_Cancel { get; set; }
        public PermissionViewModel MovementWaybill_Acceptance_Cancel_ViewModel { get; set; }

        public byte MovementWaybill_Ship { get; set; }
        public PermissionViewModel MovementWaybill_Ship_ViewModel { get; set; }

        public byte MovementWaybill_Shipping_Cancel { get; set; }
        public PermissionViewModel MovementWaybill_Shipping_Cancel_ViewModel { get; set; }

        public byte MovementWaybill_Receipt { get; set; }
        public PermissionViewModel MovementWaybill_Receipt_ViewModel { get; set; }

        public byte MovementWaybill_Receipt_Cancel { get; set; }
        public PermissionViewModel MovementWaybill_Receipt_Cancel_ViewModel { get; set; } 
        
        #endregion

        #region Накладные смены собственника

        public byte ChangeOwnerWaybill_List_Details { get; set; }
        public PermissionViewModel ChangeOwnerWaybill_List_Details_ViewModel { get; set; }

        public byte ChangeOwnerWaybill_Create_Edit { get; set; }
        public PermissionViewModel ChangeOwnerWaybill_Create_Edit_ViewModel { get; set; }

        public byte ChangeOwnerWaybill_Recipient_Change { get; set; }
        public PermissionViewModel ChangeOwnerWaybill_Recipient_Change_ViewModel { get; set; }

        public byte ChangeOwnerWaybill_Curator_Change { get; set; }
        public PermissionViewModel ChangeOwnerWaybill_Curator_Change_ViewModel { get; set; }

        public byte ChangeOwnerWaybill_Delete_Row_Delete { get; set; }
        public PermissionViewModel ChangeOwnerWaybill_Delete_Row_Delete_ViewModel { get; set; }

        public byte ChangeOwnerWaybill_Accept { get; set; }
        public PermissionViewModel ChangeOwnerWaybill_Accept_ViewModel { get; set; }

        public byte ChangeOwnerWaybill_Acceptance_Cancel { get; set; }
        public PermissionViewModel ChangeOwnerWaybill_Acceptance_Cancel_ViewModel { get; set; }

        #endregion

        #region Списание товаров

        public byte WriteoffWaybill_List_Details { get; set; }
        public PermissionViewModel WriteoffWaybill_List_Details_ViewModel { get; set; }

        public byte WriteoffWaybill_Create_Edit { get; set; }
        public PermissionViewModel WriteoffWaybill_Create_Edit_ViewModel { get; set; }

        public byte WriteoffWaybill_Curator_Change { get; set; }
        public PermissionViewModel WriteoffWaybill_Curator_Change_ViewModel { get; set; }

        public byte WriteoffWaybill_Delete_Row_Delete { get; set; }
        public PermissionViewModel WriteoffWaybill_Delete_Row_Delete_ViewModel { get; set; }

        public byte WriteoffWaybill_Accept { get; set; }
        public PermissionViewModel WriteoffWaybill_Accept_ViewModel { get; set; }

        public byte WriteoffWaybill_Acceptance_Cancel { get; set; }
        public PermissionViewModel WriteoffWaybill_Acceptance_Cancel_ViewModel { get; set; }

        public byte WriteoffWaybill_Writeoff { get; set; }
        public PermissionViewModel WriteoffWaybill_Writeoff_ViewModel { get; set; }

        public byte WriteoffWaybill_Writeoff_Cancel { get; set; }
        public PermissionViewModel WriteoffWaybill_Writeoff_Cancel_ViewModel { get; set; }

        #endregion

        #region Основания для списания

        public byte WriteoffReason_Create { get; set; }
        public PermissionViewModel WriteoffReason_Create_ViewModel { get; set; }

        public byte WriteoffReason_Edit { get; set; }
        public PermissionViewModel WriteoffReason_Edit_ViewModel { get; set; }

        public byte WriteoffReason_Delete { get; set; }
        public PermissionViewModel WriteoffReason_Delete_ViewModel { get; set; }

        #endregion

        #region Реестры цен

        public byte AccountingPriceList_List_Details { get; set; }
        public PermissionViewModel AccountingPriceList_List_Details_ViewModel { get; set; }

        public byte AccountingPriceList_Create { get; set; }
        public PermissionViewModel AccountingPriceList_Create_ViewModel { get; set; }

        public byte AccountingPriceList_Edit { get; set; }
        public PermissionViewModel AccountingPriceList_Edit_ViewModel { get; set; }

        public byte AccountingPriceList_ArticleAccountingPrice_Create_Edit { get; set; }
        public PermissionViewModel AccountingPriceList_ArticleAccountingPrice_Create_Edit_ViewModel { get; set; }

        public byte AccountingPriceList_DefaultAccountingPrice_Edit { get; set; }
        public PermissionViewModel AccountingPriceList_DefaultAccountingPrice_Edit_ViewModel { get; set; }

        public byte AccountingPriceList_Storage_Add { get; set; }
        public PermissionViewModel AccountingPriceList_Storage_Add_ViewModel { get; set; }

        public byte AccountingPriceList_Storage_Remove { get; set; }
        public PermissionViewModel AccountingPriceList_Storage_Remove_ViewModel { get; set; }

        public byte AccountingPriceList_Delete { get; set; }
        public PermissionViewModel AccountingPriceList_Delete_ViewModel { get; set; }

        public byte AccountingPriceList_Accept { get; set; }
        public PermissionViewModel AccountingPriceList_Accept_ViewModel { get; set; }

        public byte AccountingPriceList_Acceptance_Cancel { get; set; }
        public PermissionViewModel AccountingPriceList_Acceptance_Cancel_ViewModel { get; set; }

        #endregion
    }
}
