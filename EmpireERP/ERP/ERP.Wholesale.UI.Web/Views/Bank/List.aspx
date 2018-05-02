<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Bank.BankListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Банки
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("Bank", "Банки", "", "/Help/GetHelp_Bank_List")%>

    <%= Html.GridFilterHelper("mainFilter", Model.Filter, new List<string>() { "gridRussianBank", "gridForeignBank" })  %>

    <div id="messageRussianBank"></div>
    <% Html.RenderPartial("RussianBankGrid", Model.RussianBankGrid); %>

    <div id="messageForeignBank"></div>
    <% Html.RenderPartial("ForeignBankGrid", Model.ForeignBankGrid); %>

    <div id="bankRussianBankEdit"></div>
    <div id="bankForeignBankEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
