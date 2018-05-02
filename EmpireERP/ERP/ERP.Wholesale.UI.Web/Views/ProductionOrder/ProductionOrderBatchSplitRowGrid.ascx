<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Позиции разделяемой партии", "gridProductionOrderBatchSplitRow")%>
<%= Html.GridContent(Model, "/", false)%>
