<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.ProductionPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessProductionPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messageProductionPermissionsEdit");
        
        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageProductionPermissionsEdit").offset().top - 10);
        }
    }

    function OnFailProductionPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageProductionPermissionsEdit").offset().top - 10);
        }
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveProductionPermissions", "Role", new AjaxOptions()
   {
       OnSuccess = "OnSuccessProductionPermissionsSave", OnFailure = "OnFailProductionPermissionsSave" })) %>
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
    
    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnProductionPermissionsSaveTop" value="Сохранить" />
    </div>

    <%} %>

    <div id="messageProductionPermissionsEdit"></div>
    
    <div class="permission_group">
        <div class="title">Производители</div>
        <table>
            <%= Html.Permission(Model.Producer_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.Producer_Create_ViewModel)%>
            <%= Html.Permission(Model.Producer_Edit_ViewModel)%>
            <%= Html.Permission(Model.Producer_BankAccount_Create_ViewModel)%>
            <%= Html.Permission(Model.Producer_BankAccount_Edit_ViewModel)%>
            <%= Html.Permission(Model.Producer_BankAccount_Delete_ViewModel)%>
            <%= Html.Permission(Model.Producer_Delete_ViewModel)%>
        </table>  
    </div>

    <div class="permission_group">
        <div class="title">Заказы на производство</div>
        <table>
            <%= Html.Permission(Model.ProductionOrder_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.ProductionOrder_Create_Edit_ViewModel) %>
            <%= Html.Permission(Model.ProductionOrder_Curator_Change_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrder_CurrencyRate_Change_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrder_Contract_Change_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrder_ArticlePrimeCostPrintingForm_View_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Операционный план</div>
        <table>
            <%= Html.Permission(Model.ProductionOrder_Stage_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.ProductionOrder_Stage_Create_Edit_ViewModel) %>
            <%= Html.Permission(Model.ProductionOrder_Stage_MoveToNext_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrder_Stage_MoveToPrevious_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrder_Stage_MoveToUnsuccessfulClosing_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Финансовый план</div>
        <table>
            <%= Html.Permission(Model.ProductionOrder_PlannedPayments_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.ProductionOrder_PlannedPayments_Create_ViewModel) %>
            <%= Html.Permission(Model.ProductionOrder_PlannedPayments_Edit_ViewModel) %>
            <%= Html.Permission(Model.ProductionOrder_PlannedPayments_Delete_ViewModel) %>
            <%= Html.Permission(Model.ProductionOrder_PlannedExpenses_List_Details_ViewModel, true) %>
            <%= Html.Permission(Model.ProductionOrder_PlannedExpenses_Create_Edit_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Партии заказа</div>
        <table>
            <%= Html.Permission(Model.ProductionOrderBatch_List_ViewModel, true)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Details_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Create_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Delete_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Row_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Row_Delete_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Accept_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Cancel_Acceptance_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Approve_By_LineManager_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Cancel_Approvement_By_LineManager_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Approve_By_FinancialDepartment_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Approve_By_SalesDepartment_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Approve_By_AnalyticalDepartment_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment_ViewModel)%> 
            <%= Html.Permission(Model.ProductionOrderBatch_Approve_By_ProjectManager_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Cancel_Approvement_By_ProjectManager_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Approve_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Cancel_Approvement_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Split_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Join_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatch_Edit_Placement_In_Containers_ViewModel)%>

        </table>
    </div>

    <div class="permission_group">
        <div class="title">Транспортные листы</div>
        <table>
            <%= Html.Permission(Model.ProductionOrderTransportSheet_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ProductionOrderTransportSheet_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderTransportSheet_Delete_ViewModel)%>
        </table>  
    </div>

    <div class="permission_group">
        <div class="title">Листы дополнительных расходов</div>
        <table>
            <%= Html.Permission(Model.ProductionOrderExtraExpensesSheet_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ProductionOrderExtraExpensesSheet_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderExtraExpensesSheet_Delete_ViewModel)%>
        </table>  
    </div>

    <div class="permission_group">
        <div class="title">Таможенные листы</div>
        <table>
            <%= Html.Permission(Model.ProductionOrderCustomsDeclaration_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ProductionOrderCustomsDeclaration_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderCustomsDeclaration_Delete_ViewModel)%>
        </table>  
    </div>

    <div class="permission_group">
        <div class="title">Оплаты в заказах</div>
        <table>
            <%= Html.Permission(Model.ProductionOrderPayment_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ProductionOrderPayment_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderPayment_Delete_ViewModel)%>
        </table>  
    </div>

    <div class="permission_group">
        <div class="title">Пакеты материалов</div>
        <table>
            <%= Html.Permission(Model.ProductionOrderMaterialsPackage_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ProductionOrderMaterialsPackage_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderMaterialsPackage_Delete_ViewModel)%>
        </table>  
    </div>

    <div class="permission_group">
        <div class="title">Шаблоны этапов</div>
        <table>
            <%= Html.Permission(Model.ProductionOrderBatchLifeCycleTemplate_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.ProductionOrderBatchLifeCycleTemplate_Create_Edit_ViewModel)%>
            <%= Html.Permission(Model.ProductionOrderBatchLifeCycleTemplate_Delete_ViewModel)%>
        </table>
    </div>

    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnProductionPermissionsSaveBottom" value="Сохранить" />
    </div>

    <%} %>

<% } %>