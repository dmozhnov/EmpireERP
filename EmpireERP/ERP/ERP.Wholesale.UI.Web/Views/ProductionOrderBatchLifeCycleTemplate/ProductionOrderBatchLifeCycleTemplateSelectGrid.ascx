<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrderBatchLifeCycleTemplate_SelectGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridProductionOrderBatchLifeCycleTemplateSelect") %>
<%= Html.GridContent(Model, "/ProductionOrderBatchLifeCycleTemplate/ShowProductionOrderBatchLifeCycleTemplateSelectGrid/")%>
