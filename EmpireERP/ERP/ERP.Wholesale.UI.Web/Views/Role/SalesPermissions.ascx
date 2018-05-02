<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.SalesPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessSalesPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messageSalesPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageSalesPermissionsEdit").offset().top - 10);
        }
    }

    function OnFailSalesPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageSalesPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageSalesPermissionsEdit").offset().top - 10);
        }
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveSalesPermissions", "Role", new AjaxOptions()
   {
       OnSuccess = "OnSuccessSalesPermissionsSave", OnFailure = "OnFailSalesPermissionsSave" })) %>
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
    
    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnSalesPermissionsSaveTop" value="Сохранить" />
    </div>

    <%} %>

    <div id="messageSalesPermissionsEdit"></div>

    <div class="permission_group">
        <div class="title">Клиенты</div>
        <table>
            <%= Html.Permission(Model.Client_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.Client_Create_ViewModel)%>
            <%= Html.Permission(Model.Client_Edit_ViewModel)%>
            <%= Html.Permission(Model.Client_Delete_ViewModel)%>
            <%= Html.Permission(Model.Client_Block_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Организации клиентов</div>
        <table>
            <%= Html.Permission(Model.ClientOrganization_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.Client_ClientOrganization_Add_ViewModel)%>
            <%= Html.Permission(Model.Client_ClientOrganization_Remove_ViewModel)%>
            <%= Html.Permission(Model.ClientOrganization_Create_ViewModel)%>
            <%= Html.Permission(Model.ClientOrganization_Edit_ViewModel)%>
            <%= Html.Permission(Model.ClientOrganization_BankAccount_Create_ViewModel)%>
            <%= Html.Permission(Model.ClientOrganization_BankAccount_Edit_ViewModel)%>
            <%= Html.Permission(Model.ClientOrganization_BankAccount_Delete_ViewModel)%>
            <%= Html.Permission(Model.ClientOrganization_Delete_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Типы клиентов</div>
        <table>
            <%= Html.Permission(Model.ClientType_Create_ViewModel)%>
            <%= Html.Permission(Model.ClientType_Edit_ViewModel)%>
            <%= Html.Permission(Model.ClientType_Delete_ViewModel)%>            
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Программы обслуживания клиентов</div>
        <table>
            <%= Html.Permission(Model.ClientServiceProgram_Create_ViewModel)%>
            <%= Html.Permission(Model.ClientServiceProgram_Edit_ViewModel)%>
            <%= Html.Permission(Model.ClientServiceProgram_Delete_ViewModel)%>            
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Регионы клиентов</div>
        <table>
            <%= Html.Permission(Model.ClientRegion_Create_ViewModel)%>
            <%= Html.Permission(Model.ClientRegion_Edit_ViewModel)%>
            <%= Html.Permission(Model.ClientRegion_Delete_ViewModel)%>            
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Сделки</div>
        <table>
            <%= Html.Permission(Model.Deal_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.Deal_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.Deal_Stage_Change_ViewModel)%>
            <%= Html.Permission(Model.Deal_Contract_Set_ViewModel)%>            
            <%= Html.Permission(Model.Deal_Curator_Change_ViewModel)%>
            <%= Html.Permission(Model.Deal_Balance_View_ViewModel)%>
            <%= Html.Permission(Model.Deal_Quota_List_ViewModel)%>
            <%= Html.Permission(Model.Deal_Quota_Add_ViewModel)%>
            <%= Html.Permission(Model.Deal_Quota_Remove_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Договоры по сделке</div>
        <table>
            <%= Html.Permission(Model.ClientContract_Create_ViewModel)%>
            <%= Html.Permission(Model.ClientContract_Edit_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Квоты</div>
        <table>
            <%= Html.Permission(Model.DealQuota_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.DealQuota_Create_ViewModel)%>
            <%= Html.Permission(Model.DealQuota_Edit_ViewModel)%>
            <%= Html.Permission(Model.DealQuota_Delete_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Реализация товаров</div>
        <table>
            <%= Html.Permission(Model.ExpenditureWaybill_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Curator_Change_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Delete_Row_Delete_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Accept_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Accept_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Cancel_Acceptance_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Cancel_Acceptance_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Ship_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Ship_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Cancel_Shipping_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Cancel_Shipping_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Date_Change_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Accept_Retroactively_ViewModel)%>
            <%= Html.Permission(Model.ExpenditureWaybill_Ship_Retroactively_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Оплаты</div>
        <table>
            <%= Html.Permission(Model.DealPayment_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.DealPaymentFromClient_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.DealPaymentToClient_Create_ViewModel)%>
            <%= Html.Permission(Model.DealPaymentFromClient_Delete_ViewModel)%>
            <%= Html.Permission(Model.DealPaymentToClient_Delete_ViewModel)%>
            <%= Html.Permission(Model.DealPayment_User_Change_ViewModel)%>
            <%= Html.Permission(Model.DealPayment_Date_Change_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Корректировки сальдо</div>
        <table>
            <%= Html.Permission(Model.DealInitialBalanceCorrection_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.DealCreditInitialBalanceCorrection_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.DealDebitInitialBalanceCorrection_Create_ViewModel)%>
            <%= Html.Permission(Model.DealCreditInitialBalanceCorrection_Delete_ViewModel)%>
            <%= Html.Permission(Model.DealDebitInitialBalanceCorrection_Delete_ViewModel)%>
            <%= Html.Permission(Model.DealInitialBalanceCorrection_Date_Change_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Возвраты от клиентов</div>
        <table>
            <%= Html.Permission(Model.ReturnFromClientWaybill_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Curator_Change_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Delete_Row_Delete_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Accept_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Accept_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Acceptance_Cancel_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Acceptance_Cancel_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Receipt_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Receipt_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Receipt_Cancel_Deal_List_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientWaybill_Receipt_Cancel_Storage_List_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Основания для возврата</div>
        <table>
            <%= Html.Permission(Model.ReturnFromClientReason_Create_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientReason_Edit_ViewModel)%>
            <%= Html.Permission(Model.ReturnFromClientReason_Delete_ViewModel)%>
        </table>
    </div>

    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnSalesPermissionsSaveBottom" value="Сохранить" />
    </div>

    <%} %>

<% } %>