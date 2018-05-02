<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderBatchSplitViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Разделение партии заказа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript">
         ProductionOrder_ProductionOrderBatchSplit.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.BackUrl)%>
    <%: Html.HiddenFor(model => model.ProductionOrderBatchId) %>
    <%: Html.HiddenFor(model => model.SplitInfo)%>

    <%= Html.PageTitle("ProductionOrder", "Разделение партии заказа", Model.ProductionOrderName) %>

    <div class="button_set">
        <%= Html.Button("btnDoSplit", "Выполнить разделение", false, true) %>
        <input id="btnBack" type="button" value="Назад" />
    </div>

    <div id='messageEditSplitCount'></div>

    <% Html.RenderPartial("ProductionOrderBatchSplitRowGrid", Model.Rows); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">

</asp:Content>
