<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Выбор прихода", "gridSelectReceiptWaybill")%>
<%= Html.GridContent(Model, "/ReceiptWaybill/ShowReceiptWaybillSelectGrid/")%>
 
