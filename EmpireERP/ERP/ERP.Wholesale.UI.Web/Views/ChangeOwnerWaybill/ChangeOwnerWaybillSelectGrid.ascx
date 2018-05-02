<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Выбор смены собственника", "gridSelectChangeOwnerWaybill")%>
<%= Html.GridContent(Model, "/ChangeOwnerWaybill/ShowChangeOwnerWaybillSelectGrid/")%>   
 
