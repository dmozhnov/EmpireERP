<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Выбор перемещения", "gridSelectMovementWaybill")%>
<%= Html.GridContent(Model, "/MovementWaybill/ShowMovementWaybillSelectGrid/")%>   
 
