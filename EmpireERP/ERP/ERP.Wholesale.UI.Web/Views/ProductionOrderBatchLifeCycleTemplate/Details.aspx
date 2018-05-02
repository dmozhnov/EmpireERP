<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate.ProductionOrderBatchLifeCycleTemplateDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

<script type="text/javascript">
    ProductionOrderBatchLifeCycleTemplate_Details.Init();

    function OnSuccessProductionOrderBatchLifeCycleTemplateEdit(ajaxContext) {
        ProductionOrderBatchLifeCycleTemplate_Details.OnSuccessProductionOrderBatchLifeCycleTemplateEdit(ajaxContext);
    }

    function OnFailStageSave(ajaxContext) {
        ProductionOrderBatchLifeCycleTemplate_Details.OnFailStageSave(ajaxContext);
    }

    function OnSuccessStageSave(ajaxContext) {
        ProductionOrderBatchLifeCycleTemplate_Details.OnSuccessStageSave(ajaxContext);
    }

    function OnBeginStageSave() {
        ProductionOrderBatchLifeCycleTemplate_Details.OnBeginStageSave();
    }
</script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.HiddenFor(model => model.ProductionOrderBatchLifeCycleTemplateId)%>
    <%:Html.HiddenFor(model => model.BackUrl)%>

    <%=Html.PageTitle("ProductionOrder", Model.Title, Model.Name, "/Help/GetHelp_ProductionOrderBatchLifeCycleTemplate_Details")%>

    <div class="button_set">
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <%= Html.Button("btnBack", "Назад")%>
    </div>

    <div id="messageProductionOrderBatchLifeCycleTemplateEdit"></div>

    <% Html.RenderPartial("ProductionOrderBatchLifeCycleTemplateStageGrid", Model.ProductionOrderBatchLifeCycleTemplateStageList);%>

    <div id="productionOrderBatchLifeCycleTemplateEdit"></div>
    <div id="productionOrderBatchLifeCycleTemplateStageEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
