<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Документы по накладной", "gridReceiptWaybillDoc") %>    
<%= Html.GridContent(Model, "/ReceiptWaybill/ShowDocumentsGrid/")%>   