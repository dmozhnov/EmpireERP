<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate.ProductionOrderBatchLifeCycleTemplateListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Шаблоны жизненного цикла заказа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ProductionOrderBatchLifeCycleTemplate_List.Init();

        function OnSuccessProductionOrderBatchLifeCycleTemplateEdit(ajaxContext) {
            ProductionOrderBatchLifeCycleTemplate_List.OnSuccessProductionOrderBatchLifeCycleTemplateEdit(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.PageTitle("ProductionOrder", "Шаблоны жизненного цикла заказа", "", "/Help/GetHelp_ProductionOrderBatchLifeCycleTemplate_List")%>
    
    <div id="messageProductionOrderBatchLifeCycleTemplateList"></div>

    <% Html.RenderPartial("ProductionOrderBatchLifeCycleTemplateGrid", Model.ProductionOrderBatchLifeCycleTemplateGrid); %>

    <div id="productionOrderBatchLifeCycleTemplateEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
