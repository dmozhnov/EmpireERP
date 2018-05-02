<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientOrganization_Details_DealInitialBalanceCorrectionGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridDealInitialBalanceCorrection", "/Help/GetHelp_ClientOrganization_Details_ClientOrganizationDealInitialBalanceCorrectionGrid")%>
    <div class="grid_buttons">        
        <%= Html.Button("btnCreateDealCreditInitialBalanceCorrection", "Новая кредитовая корректировка",
            Model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"], Model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"])%>
        <%= Html.Button("btnCreateDealDebitInitialBalanceCorrection", "Новая дебетовая корректировка",
            Model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"], Model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"])%>
    </div>
<%=Html.GridContent(Model, "/ClientOrganization/ShowDealInitialBalanceCorrectionGrid/") %>