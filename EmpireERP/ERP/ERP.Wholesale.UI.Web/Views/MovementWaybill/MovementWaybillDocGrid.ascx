<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Документы по накладной", "gridMovementWaybillDoc") %>    
<%= Html.GridContent(Model, "/MovementWaybill/ShowDocGrid/")%>   