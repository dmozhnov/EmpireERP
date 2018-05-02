<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Документы по накладной", "gridWriteoffWaybillDocument") %>    
<%= Html.GridContent(Model, "/WriteoffWaybill/ShowDocumentGrid/")%>   