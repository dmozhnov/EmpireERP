<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Документы по накладной", "gridReturnFromClientWaybillDocument", "/Help/GetHelp_ReturnFromClientWaybill_Details_ReturnFromClientWaybillDocumentGrid")%>    
<%= Html.GridContent(Model, "/ReturnFromClientWaybill/ShowDocumentGrid/")%>   