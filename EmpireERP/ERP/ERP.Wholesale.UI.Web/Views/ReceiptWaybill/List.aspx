<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptWaybillListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Приходные накладные
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
     <% if (TempData["Message"] != null) { %>       
            $(document).ready(function () {
                ShowSuccessMessage('<%: TempData["Message"].ToString() %>', "messageReceiptWaybillList");
            });
    <%} %>

    ReceiptWaybill_List.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("ReceiptWaybill", "Приходные накладные", "")%>

    <div id="messageReceiptWaybillList"></div>
    
    <%= Html.GridFilterHelper("filterReceiptWaybill", Model.FilterData,
        new List<string>() { "gridDeliveryPendingWaybill", "gridDivergenceWaybill", "gridApprovedReceiptWaybill" })%>
    
    <% Html.RenderPartial("DeliveryPendingGrid", Model.DeliveryPendingGrid); %>

    <% Html.RenderPartial("DivergenceWaybillGrid", Model.DivergenceWaybillGrid); %>

    <% Html.RenderPartial("ApprovedWaybillGrid", Model.ApprovedWaybillGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
