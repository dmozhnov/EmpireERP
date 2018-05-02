<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderPayment.ProductionOrderPaymentListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Оплаты по заказам
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ProductionOrderPayment_List.Init();

        function OnProductionOrderPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
            ProductionOrderPayment_List.OnProductionOrderPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("ProductionOrderPayment", Model.Title, "", "/Help/GetHelp_ProductionOrderPayment_List")%>

    <%= Html.GridFilterHelper("filterProductionOrderPaymentGrid", Model.Filter, new List<string>() { "gridProductionOrderPayment" }) %>

    <div id="messageProductionOrderPaymentList"></div>
    <% Html.RenderPartial("ProductionOrderPaymentGrid", Model.PaymentGrid); %>

    <div id="productionOrderPaymentEdit"></div>
    <div id="currencyRateSelector"></div>
    <div id="productionOrderPlannedPaymentSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
