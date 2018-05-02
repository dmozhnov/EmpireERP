<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%=Html.GridHeader("Выбор торговой марки", "gridTrademark", "/Help/GetHelp_Trademark_Select_TrademarkGrid")%>
<%=Html.GridContent(Model, "/Trademark/ShowSelectTrademarkGrid") %>
