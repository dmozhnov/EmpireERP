<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader(Model.Title, "gridSelectClient", "/Help/GetHelp_Client_Select_ClientSelectGrid")%>
<%= Html.GridContent(Model, "/Client/ShowClientSelectGrid")%>