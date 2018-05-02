<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader(Model.Title, "gridSelectDeal", "/Help/GetHelp_Deal_Select_DealSelectGrid")%>
<%= Html.GridContent(Model, Model.GridPartialViewAction) %>