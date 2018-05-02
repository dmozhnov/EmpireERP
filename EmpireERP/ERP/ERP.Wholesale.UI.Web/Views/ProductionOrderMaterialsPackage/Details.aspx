<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage.ProductionOrderMaterialsPackageDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали пакета материалов
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        ProductionOrderMaterialsPackage_Details.Init();

        function OnSuccessProductionOrderMaterialsPackageDocumentCreate(ajaxContext) {
            ProductionOrderMaterialsPackage_Details.OnSuccessProductionOrderMaterialsPackageDocumentCreate(ajaxContext);
        }

        function RefreshMainDetails(model) {
            ProductionOrderMaterialsPackage_Details.RefreshMainDetails(model);
        }

        function OnSuccessProductionOrderMaterialsPackageDocumentEdit(ajaxContext) {
            ProductionOrderMaterialsPackage_Details.OnSuccessProductionOrderMaterialsPackageDocumentEdit(ajaxContext);
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%: Html.HiddenFor(model => model.BackURL) %>
    <%: Html.HiddenFor(model => model.Id) %>
    
    <%= Html.PageTitle("ProductionOrderMaterialsPackage", Model.Title, Model.PackageName, "/Help/GetHelp_ProductionOrderMaterialsPackage_Details")%>

    <div id="messageProductionOrderMaterialsPackageDetails"></div>
    <div class="button_set">
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <%= Html.Button("btnBack", "Назад")%>
    </div>

    <% Html.RenderPartial("ProductionOrderMaterialsPackageMainDetails", Model.MainDetails); %>
    <br />

    <div id="messageProductionOrderMaterialsPackageDocument"></div>
    <% Html.RenderPartial("ProductionOrderMaterialsPackageDocumentGrid", Model.Grid); %>

    
    <div id="productionOrderMaterialsPackageDocumentEdit">
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
