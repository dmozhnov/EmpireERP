<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Товары", "gridApprovementDetails") %>
<%= Html.GridContent(Model, "ShowApprovementDetailsReceiptWaybillGrid", false) %>