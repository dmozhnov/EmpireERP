<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderPlannedPaymentsEditViewModel>" %>

<div style="width:770px;">
    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderEditPlannedPayment") %></div>
    <br />

    <div style="padding: 0px 10px 0px;">
        <%= Html.GridFilterHelper("filterProductionOrderPlannedPayment", Model.Filter,
            new List<string>() { "gridProductionOrderPlannedPayment" }, true)%>

        <div id="messageProductionOrderEditPlannedPayments"></div>

        <div style="max-height: 420px; overflow: auto;">
            <% Html.RenderPartial("ProductionOrderPlannedPaymentGrid", Model.ProductionOrderPlannedPaymentGrid);%>
        </div>
    </div>

    <div class='button_set'>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>

<div id="productionOrderPlannedPaymentEdit"></div>
