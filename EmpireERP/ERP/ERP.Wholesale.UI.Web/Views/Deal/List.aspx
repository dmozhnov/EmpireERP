<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Deal.DealListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Сделки
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.PageTitle("Deal", "Сделки", "", "/Help/GetHelp_Deal_List")%>

    <%=Html.GridFilterHelper("filterDeal", Model.Filter, new List<string>() { "gridActiveDeal", "gridClosedDeal" })%>
    
    <% Html.RenderPartial("ActiveDealGrid", Model.ActiveDealGrid); %>

    <% Html.RenderPartial("ClosedDealGrid", Model.ClosedDealGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
