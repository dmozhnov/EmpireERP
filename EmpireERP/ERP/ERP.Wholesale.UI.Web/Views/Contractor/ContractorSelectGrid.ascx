<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Контрагенты", "gridContractor") %>
<%= Html.GridContent(Model, "/Contractor/ShowContractorSelectGrid/")%>