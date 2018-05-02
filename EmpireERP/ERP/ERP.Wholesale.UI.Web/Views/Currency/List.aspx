<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Currency.CurrencyListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Справочник валют
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("Currency", Model.Title, "", "/Help/GetHelp_Currency_List")%>

    <div id="messageCurrencyList"></div>
    <% Html.RenderPartial("CurrencyGrid", Model.CurrencyGrid); %>

    <div id="currencyEdit"></div>
</asp:Content>



<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
