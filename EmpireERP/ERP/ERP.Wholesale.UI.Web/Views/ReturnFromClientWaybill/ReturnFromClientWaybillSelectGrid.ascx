<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Выбор возврата", "gridSelectReturnFromClientWaybill")%>
<%= Html.GridContent(Model, "/ReturnFromClientWaybill/ShowReturnFromClientWaybillSelectGrid/")%>   
 
