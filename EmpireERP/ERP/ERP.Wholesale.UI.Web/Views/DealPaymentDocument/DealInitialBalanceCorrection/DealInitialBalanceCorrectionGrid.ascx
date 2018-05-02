<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    DealPaymentDocument_DealInitialBalanceCorrection_DealInitialBalanceCorrectionGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridDealInitialBalanceCorrection", "/Help/GetHelp_DealInitialBalanceCorrection_List_DealInitialBalanceCorrectionGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateDealCreditInitialBalanceCorrection", "Новая кредитовая корректировка",
            Model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"], Model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"])%>
        <%= Html.Button("btnCreateDealDebitInitialBalanceCorrection", "Новая дебетовая корректировка",
            Model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"], Model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"])%>
    </div>
<%=Html.GridContent(Model, "/DealInitialBalanceCorrection/ShowDealInitialBalanceCorrectionGrid/")%>