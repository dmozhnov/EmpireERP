<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientOrganization_Details_DealPaymentGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridDealPayment", "/Help/GetHelp_ClientOrganization_Details_ClientOrganizationPaymentGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateClientOrganizationPaymentFromClient", "Новая оплата", Model.ButtonPermissions["AllowToCreateClientOrganizationPaymentFromClient"], Model.ButtonPermissions["AllowToCreateClientOrganizationPaymentFromClient"])%>
        <%: Html.Button("btnCreateDealPaymentToClient", "Новый возврат оплаты", Model.ButtonPermissions["AllowToCreateDealPaymentToClient"], Model.ButtonPermissions["AllowToCreateDealPaymentToClient"])%>
    </div>
<%=Html.GridContent(Model, "/ClientOrganization/ShowDealPaymentGrid/")%>
