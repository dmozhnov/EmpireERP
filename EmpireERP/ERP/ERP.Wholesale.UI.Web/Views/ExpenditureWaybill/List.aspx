<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ExpenditureWaybill.ExpenditureWaybillListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Реализация товаров
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% if (TempData["Message"] != null) { %>
        <script type="text/javascript">
            $(document).ready(function () {
                ShowSuccessMessage('<%: TempData["Message"].ToString() %>', "messageExpenditureWaybillList");
            });
        </script>
    <%} %>       
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("ExpenditureWaybill", "Реализация товаров", "", "/Help/GetHelp_ExpenditureWaybill_List")%>

    <div id="messageExpenditureWaybillList"></div>

    <%= Html.GridFilterHelper("filterExpenditureWaybill", Model.FilterData, 
        new List<string>() { "gridNewAndAcceptedExpenditureWaybill", "gridShippedExpenditureWaybill" })%>

    <% Html.RenderPartial("NewAndAcceptedExpenditureWaybillGrid", Model.NewAndAcceptedGrid); %>

    <% Html.RenderPartial("ShippedExpenditureWaybillGrid", Model.ShippedGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
