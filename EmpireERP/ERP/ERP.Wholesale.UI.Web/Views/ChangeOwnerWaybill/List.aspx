<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybillListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Накладные смены собственника
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%=Html.PageTitle("ChangeOwnerWaybill", Model.Title, "")%>

    <%= Html.GridFilterHelper("changeOwnerWaybillFilter", Model.FilterData, new List<string>() {"gridChangeOwnerWaybillNewWaybill", "gridChangeOwnerWaybillAcceptedWaybill" })%>

    <div id="gridChangeOwnerWaybillNewWaybillContainer">
        <% Html.RenderPartial("ChangeOwnerWaybillNewGrid", Model.ChangeOwnerWaybillNewGrid); %>
    </div>

    <div id="gridChangeOwnerWaybillAcceptedWaybillContainer">
        <% Html.RenderPartial("ChangeOwnerWaybillAcceptedGrid", Model.ChangeOwnerWaybillAcceptedGrid); %>
    </div>
    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
