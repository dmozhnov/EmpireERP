
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class SalesPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }

        #region Клиенты

        public byte Client_List_Details { get; set; }
        public PermissionViewModel Client_List_Details_ViewModel { get; set; }

        public byte Client_Create { get; set; }
        public PermissionViewModel Client_Create_ViewModel { get; set; }

        public byte Client_Edit { get; set; }
        public PermissionViewModel Client_Edit_ViewModel { get; set; }

        public byte Client_Delete { get; set; }
        public PermissionViewModel Client_Delete_ViewModel { get; set; }

        public byte Client_Block { get; set; }
        public PermissionViewModel Client_Block_ViewModel { get; set; }

        #endregion

        #region Организации клиентов

        public byte ClientOrganization_List_Details { get; set; }
        public PermissionViewModel ClientOrganization_List_Details_ViewModel { get; set; }

        public byte Client_ClientOrganization_Add { get; set; }
        public PermissionViewModel Client_ClientOrganization_Add_ViewModel { get; set; }

        public byte Client_ClientOrganization_Remove { get; set; }
        public PermissionViewModel Client_ClientOrganization_Remove_ViewModel { get; set; }

        public byte ClientOrganization_Create { get; set; }
        public PermissionViewModel ClientOrganization_Create_ViewModel { get; set; }

        public byte ClientOrganization_Edit { get; set; }
        public PermissionViewModel ClientOrganization_Edit_ViewModel { get; set; }

        public byte ClientOrganization_BankAccount_Create { get; set; }
        public PermissionViewModel ClientOrganization_BankAccount_Create_ViewModel { get; set; }

        public byte ClientOrganization_BankAccount_Edit { get; set; }
        public PermissionViewModel ClientOrganization_BankAccount_Edit_ViewModel { get; set; }

        public byte ClientOrganization_BankAccount_Delete { get; set; }
        public PermissionViewModel ClientOrganization_BankAccount_Delete_ViewModel { get; set; }
        
        public byte ClientOrganization_Delete { get; set; }
        public PermissionViewModel ClientOrganization_Delete_ViewModel { get; set; }

        #endregion

        #region Типы клиентов

        public byte ClientType_Create { get; set; }
        public PermissionViewModel ClientType_Create_ViewModel { get; set; }

        public byte ClientType_Edit { get; set; }
        public PermissionViewModel ClientType_Edit_ViewModel { get; set; }

        public byte ClientType_Delete { get; set; }
        public PermissionViewModel ClientType_Delete_ViewModel { get; set; }
        
        #endregion

        #region Программы обслуживания клиентов

        public byte ClientServiceProgram_Create { get; set; }
        public PermissionViewModel ClientServiceProgram_Create_ViewModel { get; set; }

        public byte ClientServiceProgram_Edit { get; set; }
        public PermissionViewModel ClientServiceProgram_Edit_ViewModel { get; set; }

        public byte ClientServiceProgram_Delete { get; set; }
        public PermissionViewModel ClientServiceProgram_Delete_ViewModel { get; set; }
        
        #endregion

        #region Регионы клиентов

        public byte ClientRegion_Create { get; set; }
        public PermissionViewModel ClientRegion_Create_ViewModel { get; set; }

        public byte ClientRegion_Edit { get; set; }
        public PermissionViewModel ClientRegion_Edit_ViewModel { get; set; }

        public byte ClientRegion_Delete { get; set; }
        public PermissionViewModel ClientRegion_Delete_ViewModel { get; set; }
        
        #endregion

        #region Сделки

        public byte Deal_List_Details { get; set; }
        public PermissionViewModel Deal_List_Details_ViewModel { get; set; }

        public byte Deal_Create_Edit { get; set; }
        public PermissionViewModel Deal_Create_Edit_ViewModel { get; set; }

        public byte Deal_Stage_Change { get; set; }
        public PermissionViewModel Deal_Stage_Change_ViewModel { get; set; }

        public byte Deal_Contract_Set { get; set; }
        public PermissionViewModel Deal_Contract_Set_ViewModel { get; set; }
                
        public byte Deal_Curator_Change { get; set; }
        public PermissionViewModel Deal_Curator_Change_ViewModel { get; set; }

        public byte Deal_Balance_View { get; set; }
        public PermissionViewModel Deal_Balance_View_ViewModel { get; set; }

        public byte Deal_Quota_List { get; set; }
        public PermissionViewModel Deal_Quota_List_ViewModel { get; set; }

        public byte Deal_Quota_Add { get; set; }
        public PermissionViewModel Deal_Quota_Add_ViewModel { get; set; }

        public byte Deal_Quota_Remove { get; set; }
        public PermissionViewModel Deal_Quota_Remove_ViewModel { get; set; }

        #endregion

        #region Договоры по сделке

        public byte ClientContract_Create { get; set; }
        public PermissionViewModel ClientContract_Create_ViewModel { get; set; }

        public byte ClientContract_Edit { get; set; }
        public PermissionViewModel ClientContract_Edit_ViewModel { get; set; }

        #endregion

        #region Накладные реализации товаров

        public byte ExpenditureWaybill_List_Details { get; set; }
        public PermissionViewModel ExpenditureWaybill_List_Details_ViewModel { get; set; }

        public byte ExpenditureWaybill_Create_Edit { get; set; }
        public PermissionViewModel ExpenditureWaybill_Create_Edit_ViewModel { get; set; }

        public byte ExpenditureWaybill_Curator_Change { get; set; }
        public PermissionViewModel ExpenditureWaybill_Curator_Change_ViewModel { get; set; }

        public byte ExpenditureWaybill_Delete_Row_Delete { get; set; }
        public PermissionViewModel ExpenditureWaybill_Delete_Row_Delete_ViewModel { get; set; }

        public byte ExpenditureWaybill_Accept_Deal_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Accept_Deal_List_ViewModel { get; set; }

        public byte ExpenditureWaybill_Accept_Storage_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Accept_Storage_List_ViewModel { get; set; }

        public byte ExpenditureWaybill_Cancel_Acceptance_Deal_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Cancel_Acceptance_Deal_List_ViewModel { get; set; }

        public byte ExpenditureWaybill_Cancel_Acceptance_Storage_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Cancel_Acceptance_Storage_List_ViewModel { get; set; }

        public byte ExpenditureWaybill_Ship_Deal_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Ship_Deal_List_ViewModel { get; set; }

        public byte ExpenditureWaybill_Ship_Storage_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Ship_Storage_List_ViewModel { get; set; }

        public byte ExpenditureWaybill_Cancel_Shipping_Deal_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Cancel_Shipping_Deal_List_ViewModel { get; set; }

        public byte ExpenditureWaybill_Cancel_Shipping_Storage_List { get; set; }
        public PermissionViewModel ExpenditureWaybill_Cancel_Shipping_Storage_List_ViewModel { get; set; }
        
        public byte ExpenditureWaybill_Date_Change { get; set; }
        public PermissionViewModel ExpenditureWaybill_Date_Change_ViewModel { get; set; }

        public byte ExpenditureWaybill_Accept_Retroactively { get; set; }
        public PermissionViewModel ExpenditureWaybill_Accept_Retroactively_ViewModel { get; set; }

        public byte ExpenditureWaybill_Ship_Retroactively { get; set; }
        public PermissionViewModel ExpenditureWaybill_Ship_Retroactively_ViewModel { get; set; }
        
        #endregion

        #region Оплаты

        public byte DealPayment_List_Details { get; set; }
        public PermissionViewModel DealPayment_List_Details_ViewModel { get; set; }

        public byte DealPaymentFromClient_Create_Edit { get; set; }
        public PermissionViewModel DealPaymentFromClient_Create_Edit_ViewModel { get; set; }

        public byte DealPaymentToClient_Create { get; set; }
        public PermissionViewModel DealPaymentToClient_Create_ViewModel { get; set; }

        public byte DealPaymentFromClient_Delete { get; set; }
        public PermissionViewModel DealPaymentFromClient_Delete_ViewModel { get; set; }

        public byte DealPaymentToClient_Delete { get; set; }
        public PermissionViewModel DealPaymentToClient_Delete_ViewModel { get; set; }

        public byte DealPayment_User_Change { get; set; }
        public PermissionViewModel DealPayment_User_Change_ViewModel { get; set; }

        public byte DealPayment_Date_Change { get; set; }
        public PermissionViewModel DealPayment_Date_Change_ViewModel { get; set; }

        #endregion

        #region Квоты

        public byte DealQuota_List_Details { get; set; }
        public PermissionViewModel DealQuota_List_Details_ViewModel { get; set; }

        public byte DealQuota_Create { get; set; }
        public PermissionViewModel DealQuota_Create_ViewModel { get; set; }

        public byte DealQuota_Edit { get; set; }
        public PermissionViewModel DealQuota_Edit_ViewModel { get; set; }

        public byte DealQuota_Delete { get; set; }
        public PermissionViewModel DealQuota_Delete_ViewModel { get; set; }

        #endregion

        #region Возвраты от клиентов
        
        public byte ReturnFromClientWaybill_List_Details { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_List_Details_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Create_Edit { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Create_Edit_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Curator_Change { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Curator_Change_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Delete_Row_Delete { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Delete_Row_Delete_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Accept_Deal_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Accept_Deal_List_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Accept_Storage_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Accept_Storage_List_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Acceptance_Cancel_Deal_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Acceptance_Cancel_Deal_List_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Acceptance_Cancel_Storage_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Acceptance_Cancel_Storage_List_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Receipt_Deal_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Receipt_Deal_List_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Receipt_Storage_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Receipt_Storage_List_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Receipt_Cancel_Deal_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Receipt_Cancel_Deal_List_ViewModel { get; set; }

        public byte ReturnFromClientWaybill_Receipt_Cancel_Storage_List { get; set; }
        public PermissionViewModel ReturnFromClientWaybill_Receipt_Cancel_Storage_List_ViewModel { get; set; }

        #endregion

        #region Основание для возврата
        
        public byte ReturnFromClientReason_Create { get; set; }
        public PermissionViewModel ReturnFromClientReason_Create_ViewModel { get; set; }

        public byte ReturnFromClientReason_Edit { get; set; }
        public PermissionViewModel ReturnFromClientReason_Edit_ViewModel { get; set; }

        public byte ReturnFromClientReason_Delete { get; set; }
        public PermissionViewModel ReturnFromClientReason_Delete_ViewModel { get; set; }

        #endregion

        #region Корректировки сальдо по сделке

        public byte DealInitialBalanceCorrection_List_Details { get; set; }
        public PermissionViewModel DealInitialBalanceCorrection_List_Details_ViewModel { get; set; }

        public byte DealCreditInitialBalanceCorrection_Create_Edit { get; set; }
        public PermissionViewModel DealCreditInitialBalanceCorrection_Create_Edit_ViewModel { get; set; }

        public byte DealDebitInitialBalanceCorrection_Create { get; set; }
        public PermissionViewModel DealDebitInitialBalanceCorrection_Create_ViewModel { get; set; }

        public byte DealCreditInitialBalanceCorrection_Delete { get; set; }
        public PermissionViewModel DealCreditInitialBalanceCorrection_Delete_ViewModel { get; set; }

        public byte DealDebitInitialBalanceCorrection_Delete { get; set; }
        public PermissionViewModel DealDebitInitialBalanceCorrection_Delete_ViewModel { get; set; }

        public byte DealInitialBalanceCorrection_Date_Change { get; set; }
        public PermissionViewModel DealInitialBalanceCorrection_Date_Change_ViewModel { get; set; }

        #endregion        

        /*
        public byte  { get; set; }
        public PermissionViewModel _ViewModel { get; set; }
        */
    }
}
