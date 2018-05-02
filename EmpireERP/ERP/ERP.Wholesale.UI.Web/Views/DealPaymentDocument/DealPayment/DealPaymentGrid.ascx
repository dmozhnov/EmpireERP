<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    DealPaymentDocument_DealPayment_DealPaymentGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridDealPayment", "/Help/GetHelp_DealPayment_List_DealPaymentGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateDealPaymentFromClient", "Новая оплата по сделке", 
            Model.ButtonPermissions["AllowToCreateDealPaymentFromClient"], Model.ButtonPermissions["AllowToCreateDealPaymentFromClient"])%>
        <%: Html.Button("btnCreateClientOrganizationPaymentFromClient", "Новая оплата по организации", 
            Model.ButtonPermissions["AllowToCreateDealPaymentFromClient"], Model.ButtonPermissions["AllowToCreateDealPaymentFromClient"])%>
        <%: Html.Button("btnCreateDealPaymentToClient", "Новый возврат оплаты",
            Model.ButtonPermissions["AllowToCreateDealPaymentToClient"], Model.ButtonPermissions["AllowToCreateDealPaymentToClient"])%>
    </div>
<%=Html.GridContent(Model, "/DealPayment/ShowDealPaymentGrid/")%>