<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage.ProductionOrderMaterialsPackageListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Пакеты материалов
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("ProductionOrderMaterialsPackage", Model.Title, "", "/Help/GetHelp_ProductionOrderMaterialsPackage_List")%>

    <%= Html.GridFilterHelper("filterMaterialsPackage", Model.Filter, new List<string> { "gridMaterialsPackage" })%>

    <% Html.RenderPartial("ProductionOrderMaterialsPackageGrid", Model.MaterialsPackageGrid); %>

</asp:Content>



<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
