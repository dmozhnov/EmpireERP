<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderTransportSheet.ProductionOrderTransportSheetListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Транспортные листы
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("ProductionOrderTransportSheet", Model.Title, "", "/Help/GetHelp_ProductionOrderTransportSheet_List")%>

    <%= Html.GridFilterHelper("filterProductionOrderTransportSheetGrid", Model.FilterData, new List<string>() { "gridProductionOrderTransportSheet" })%>
    
    <div id="messageProductionOrderTransportSheetList"></div>
    <% Html.RenderPartial("ProductionOrderTransportSheetGrid", Model.TransportSheetGrid); %>

    <div id="productionOrderTransportSheetEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
