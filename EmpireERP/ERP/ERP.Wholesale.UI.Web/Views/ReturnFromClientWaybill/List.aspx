<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill.ReturnFromClientWaybillListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Возвраты от клиентов
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("ReturnFromClientWaybill", "Возвраты от клиентов", "", "/Help/GetHelp_ReturnFromClientWaybill_List")%>

    <div id="messageReturnFromClientWaybillList"></div>

    <%= Html.GridFilterHelper("filterReturnFromClientWaybill", Model.FilterData, 
        new List<string>() { "gridNewAndAcceptedReturnFromClientWaybill", "gridReceiptedReturnFromClientWaybill" })%>

    <% Html.RenderPartial("NewAndAcceptedReturnFromClientWaybillGrid", Model.NewAndAcceptedReturnFromClientWaybillGrid); %>

    <% Html.RenderPartial("ReceiptedReturnFromClientWaybillGrid", Model.ReceiptedReturnFromClientWaybillGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
