<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Платежи", "gridProductionOrderPlannedPaymentSelect")%>
<%= Html.GridContent(Model, "/ProductionOrder/ShowPlannedPaymentSelectGrid/")%>