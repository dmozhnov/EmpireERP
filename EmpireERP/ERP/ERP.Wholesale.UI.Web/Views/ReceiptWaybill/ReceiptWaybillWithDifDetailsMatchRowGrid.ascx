<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Совпадающие позиции", "gridMatchRows") %>
<%= Html.GridContent(Model, "/ReceiptWaybill/ShowMatchRowInReceiptWaybillGrid")%>