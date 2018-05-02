<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Документы по накладной", "gridExpenditureWaybillDocument", "/Help/GetHelp_ExpenditureWaybill_Details_ExpenditureWaybillDocumentGrid")%>    
<%= Html.GridContent(Model, "/ExpenditureWaybill/ShowDocumentGrid/")%>   