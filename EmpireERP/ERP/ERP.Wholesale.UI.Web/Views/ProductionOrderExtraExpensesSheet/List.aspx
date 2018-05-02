<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderExtraExpensesSheet.ProductionOrderExtraExpensesSheetListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Листы дополнительных расходов
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("ProductionOrderExtraExpensesSheet", Model.Title, "", "/Help/GetHelp_ProductionOrderExtraExpensesSheet_List")%>

    <%= Html.GridFilterHelper("filterProductionOrderExtraExpensesSheetGrid", Model.FilterData, new List<string>() { "gridProductionOrderExtraExpensesSheet" })%>

    <div id="messageProductionOrderExtraExpensesSheetList"></div>
    <% Html.RenderPartial("ProductionOrderExtraExpensesSheetGrid", Model.ExtraExpensesSheetGrid); %>

    <div id="productionOrderExtraExpensesSheetEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
