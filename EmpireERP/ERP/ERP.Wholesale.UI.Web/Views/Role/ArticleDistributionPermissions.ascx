<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.ArticleDistributionPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessArticleDistributionPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messageArticleDistributionPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageArticleDistributionPermissionsEdit").offset().top - 10);
        }
    }

    function OnFailArticleDistributionPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageArticleDistributionPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageArticleDistributionPermissionsEdit").offset().top - 10);
        }
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveArticleDistributionPermissions", "Role", new AjaxOptions() {
       OnSuccess = "OnSuccessArticleDistributionPermissionsSave", OnFailure = "OnFailArticleDistributionPermissionsSave" })) %> 
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
    
    <%if (Model.AllowToEdit) { %>
        <div class="button_set">
            <input type="submit" id="btnArticleDistributionPermissionsSaveTop" value="Сохранить" />
        </div>
    <%} %>

    <div id="messageArticleDistributionPermissionsEdit"></div>

    <div class="permission_group">
        <div class="title">Поставщики</div>
        <table>
            <%= Html.Permission(Model.Provider_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.Provider_Create_ViewModel) %>
            <%= Html.Permission(Model.Provider_Edit_ViewModel) %>            
            <%= Html.Permission(Model.Provider_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Организации поставщиков</div>
        <table>
            <%= Html.Permission(Model.ProviderOrganization_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.Provider_ProviderOrganization_Add_ViewModel) %>
            <%= Html.Permission(Model.Provider_ProviderOrganization_Remove_ViewModel) %>
            <%= Html.Permission(Model.ProviderOrganization_Create_ViewModel) %>
            <%= Html.Permission(Model.ProviderOrganization_Edit_ViewModel) %>
            <%= Html.Permission(Model.ProviderOrganization_BankAccount_Create_ViewModel) %>
            <%= Html.Permission(Model.ProviderOrganization_BankAccount_Edit_ViewModel) %>
            <%= Html.Permission(Model.ProviderOrganization_BankAccount_Delete_ViewModel) %>
            <%= Html.Permission(Model.ProviderOrganization_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Договоры с поставщиками</div>
        <table>            
            <%= Html.Permission(Model.Provider_ProviderContract_Create_ViewModel)%>
            <%= Html.Permission(Model.Provider_ProviderContract_Edit_ViewModel)%>
            <%= Html.Permission(Model.Provider_ProviderContract_Delete_ViewModel)%>            
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Типы поставщиков</div>
        <table>
            <%= Html.Permission(Model.ProviderType_Create_ViewModel) %>
            <%= Html.Permission(Model.ProviderType_Edit_ViewModel) %>
            <%= Html.Permission(Model.ProviderType_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Собственные организации</div>
        <table>
            <%= Html.Permission(Model.AccountOrganization_Create_ViewModel) %>
            <%= Html.Permission(Model.AccountOrganization_Edit_ViewModel) %>
            <%= Html.Permission(Model.AccountOrganization_BankAccount_Create_ViewModel) %>
            <%= Html.Permission(Model.AccountOrganization_BankAccount_Edit_ViewModel)%>
            <%= Html.Permission(Model.AccountOrganization_BankAccount_Delete_ViewModel)%>
            <%= Html.Permission(Model.AccountOrganization_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Места хранения</div>
        <table>
            <%= Html.Permission(Model.Storage_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.Storage_Create_ViewModel) %>
            <%= Html.Permission(Model.Storage_Edit_ViewModel) %>
            <%= Html.Permission(Model.Storage_AccountOrganization_Add_ViewModel) %>
            <%= Html.Permission(Model.Storage_AccountOrganization_Remove_ViewModel) %>
            <%= Html.Permission(Model.Storage_Section_Create_Edit_ViewModel) %>
            <%= Html.Permission(Model.Storage_Section_Delete_ViewModel) %>
            <%= Html.Permission(Model.Storage_Delete_ViewModel) %>
        </table>        
    </div>

    <div class="permission_group">
        <div class="title">Приход товаров - приходные накладные</div>
        <table>
            <%= Html.Permission(Model.ReceiptWaybill_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ReceiptWaybill_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_CreateFromProductionOrderBatch_ViewModel)%>
            <%--<%= Html.Permission(Model.ReceiptWaybill_Provider_Storage_Change_ViewModel)%> --%>           
            <%= Html.Permission(Model.ReceiptWaybill_Curator_Change_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Delete_Row_Delete_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_ProviderDocuments_Edit_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Accept_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Acceptance_Cancel_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Receipt_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Receipt_Cancel_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Approve_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Approvement_Cancel_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Date_Change_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Accept_Retroactively_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Receipt_Retroactively_ViewModel)%>
            <%= Html.Permission(Model.ReceiptWaybill_Approve_Retroactively_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Перемещение товаров - накладные перемещения</div>
        <table>
            <%= Html.Permission(Model.MovementWaybill_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.MovementWaybill_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Curator_Change_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Delete_Row_Delete_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Accept_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Acceptance_Cancel_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Ship_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Shipping_Cancel_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Receipt_ViewModel)%>
            <%= Html.Permission(Model.MovementWaybill_Receipt_Cancel_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Смена собственника - накладные смены собственника</div>
        <table>
            <%= Html.Permission(Model.ChangeOwnerWaybill_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ChangeOwnerWaybill_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ChangeOwnerWaybill_Recipient_Change_ViewModel)%>
            <%= Html.Permission(Model.ChangeOwnerWaybill_Curator_Change_ViewModel)%>
            <%= Html.Permission(Model.ChangeOwnerWaybill_Delete_Row_Delete_ViewModel)%>
            <%= Html.Permission(Model.ChangeOwnerWaybill_Accept_ViewModel)%>
            <%= Html.Permission(Model.ChangeOwnerWaybill_Acceptance_Cancel_ViewModel)%>            
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Списание товаров - накладные списания</div>
        <table>
            <%= Html.Permission(Model.WriteoffWaybill_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.WriteoffWaybill_Create_Edit_ViewModel) %>
            <%= Html.Permission(Model.WriteoffWaybill_Curator_Change_ViewModel) %>
            <%= Html.Permission(Model.WriteoffWaybill_Delete_Row_Delete_ViewModel) %>
            <%= Html.Permission(Model.WriteoffWaybill_Accept_ViewModel) %>
            <%= Html.Permission(Model.WriteoffWaybill_Acceptance_Cancel_ViewModel) %>
            <%= Html.Permission(Model.WriteoffWaybill_Writeoff_ViewModel) %>
            <%= Html.Permission(Model.WriteoffWaybill_Writeoff_Cancel_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Основания для списания</div>
        <table>
            <%= Html.Permission(Model.WriteoffReason_Create_ViewModel) %>
            <%= Html.Permission(Model.WriteoffReason_Edit_ViewModel) %>
            <%= Html.Permission(Model.WriteoffReason_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Реестры цен</div>
        <table>
            <%= Html.Permission(Model.AccountingPriceList_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.AccountingPriceList_Create_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_Edit_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_ArticleAccountingPrice_Create_Edit_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_DefaultAccountingPrice_Edit_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_Storage_Add_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_Storage_Remove_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_Delete_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_Accept_ViewModel) %>
            <%= Html.Permission(Model.AccountingPriceList_Acceptance_Cancel_ViewModel) %>
        </table>
    </div>

    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnArticleDistributionPermissionsSave" value="Сохранить" />
    </div>

    <%} %>

<% } %>