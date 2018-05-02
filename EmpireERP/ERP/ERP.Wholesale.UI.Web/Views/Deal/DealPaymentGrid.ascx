<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Deal_Details_DealPaymentGrid.Init();
</script>

<%=Html.GridHeader("Оплаты по сделке", "gridDealPayment", "/Help/GetHelp_Deal_Details_PaymentGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateDealPaymentFromClient", "Новая оплата", Model.ButtonPermissions["AllowToCreateDealPaymentFromClient"], Model.ButtonPermissions["AllowToCreateDealPaymentFromClient"])%>
        <%: Html.Button("btnCreateDealPaymentToClient", "Новый возврат оплаты", Model.ButtonPermissions["AllowToCreateDealPaymentToClient"], Model.ButtonPermissions["AllowToCreateDealPaymentToClient"])%>
    </div>
<%=Html.GridContent(Model, "/Deal/ShowDealPaymentGrid/")%>

