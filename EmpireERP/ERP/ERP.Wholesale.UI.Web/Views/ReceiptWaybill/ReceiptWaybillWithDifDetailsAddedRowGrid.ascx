<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Добавленные при приемке позиции", "gridAddedRows")%>
<%= Html.GridContent(Model, "/ReceiptWaybill/ShowAddedRowInReceiptWaybillGrid")%>