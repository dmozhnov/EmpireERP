<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.AccountingPriceList.AccountingPriceListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Реестры цен
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("AccountingPriceList", "Реестры цен", "")%>
    
    <%= Html.GridFilterHelper("filterAccountingPriceList", Model.FilterData, 
        new List<string>() { "gridNewAccountingPriceList", "gridAcceptedAccountingPriceList" }) %>

    <% Html.RenderPartial("AccountingPriceListNewGrid", Model.NewList); %>

    <% Html.RenderPartial("AccountingPriceListAcceptedGrid", Model.AcceptedList); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
