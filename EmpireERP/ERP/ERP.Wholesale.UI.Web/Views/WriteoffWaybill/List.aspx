<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.WriteoffWaybill.WriteoffWaybillListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Списания
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("WriteoffWaybill", "Списания", "")%>

    <%= Html.GridFilterHelper("filterWriteoffWaybill", Model.FilterData,
                new List<string>() { "gridWriteoffPending", "gridWrittenoff" })%>

    <% Html.RenderPartial("WriteoffPendingGrid", Model.WriteoffPendingGrid); %>

    <% Html.RenderPartial("WrittenoffWaybillGrid", Model.WrittenoffGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
