<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
   Deal_Details_DealInitialBalanceCorrectionGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridDealInitialBalanceCorrection", "/Help/GetHelp_Deal_Details_InitialBalanceCorrectionGrid")%>
    <div class="grid_buttons">        
        <%= Html.Button("btnCreateDealCreditInitialBalanceCorrection", "Новая кредитовая корректировка",
            Model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"], Model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"])%>
        <%= Html.Button("btnCreateDealDebitInitialBalanceCorrection", "Новая дебетовая корректировка",
            Model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"], Model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"])%>
    </div>
<%=Html.GridContent(Model, "/Deal/ShowDealInitialBalanceCorrectionGrid/") %>