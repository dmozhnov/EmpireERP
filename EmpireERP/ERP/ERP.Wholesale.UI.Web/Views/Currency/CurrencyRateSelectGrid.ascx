<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Курсы валюты", "gridCurrencyRateSelector", "/Help/GetHelp_Currency_Select_CurrencyRateSelectGrid")%>
<%= Html.GridContent(Model, "/Currency/ShowCurrencyRateSelectGrid")%>