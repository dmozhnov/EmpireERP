<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderCustomsDeclaration.ProductionOrderCustomsDeclarationListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Таможенные листы
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("ProductionOrderCustomsDeclaration", Model.Title, "", "/Help/GetHelp_ProductionOrderCustomsDeclaration_List")%>

    <%= Html.GridFilterHelper("filterProductionOrderCustomsDeclarationGrid", Model.FilterData, new List<string> {"gridProductionOrderCustomsDeclaration"}) %>

    <div id="messageProductionOrderCustomsDeclarationList"></div>
    <% Html.RenderPartial("ProductionOrderCustomsDeclarationGrid", Model.CustomsDeclarationGrid); %>

    <div id="productionOrderCustomsDeclarationEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
