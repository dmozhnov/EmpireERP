<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Этапы шаблона жизненного цикла заказа", "gridProductionOrderBatchLifeCycleTemplateStage", "/Help/GetHelp_ProductionOrderBatchLifeCycleTemplate_Details_ProductionOrderBatchLifeCycleTemplateStageGrid")%>
<%= Html.GridContent(Model, "/ProductionOrderBatchLifeCycleTemplate/ShowProductionOrderBatchLifeCycleTemplateStageGrid/", false)%>
