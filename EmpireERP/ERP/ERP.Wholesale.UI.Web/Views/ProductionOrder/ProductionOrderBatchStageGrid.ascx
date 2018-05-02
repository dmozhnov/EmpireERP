<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Операционный план", "gridProductionOrderBatchStage")%>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderBatchStageGrid/", false)%>
