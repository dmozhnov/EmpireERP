<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Заказы
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ProductionOrder_List.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.PageTitle("ProductionOrder", "Заказы", "", "/Help/GetHelp_ProductionOrder_List")%>
    
    <%= Html.GridFilterHelper("filterProductionOrder", Model.FilterData, new List<string> { "gridActiveProductionOrder", "gridClosedProductionOrder" })%>

    <div id="messageProductionOrderList"></div>

    <% Html.RenderPartial("ActiveProductionOrderGrid", Model.ActiveProductionOrderGrid); %>

    <% Html.RenderPartial("ClosedProductionOrderGrid", Model.ClosedProductionOrderGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
