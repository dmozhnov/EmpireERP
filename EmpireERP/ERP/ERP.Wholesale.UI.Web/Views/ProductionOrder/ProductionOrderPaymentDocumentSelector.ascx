<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderPaymentDocumentSelectorViewModel>" %>

<%:Html.HiddenFor(model => model.ProductionOrderPaymentTypeId)%>

<div style="width: 800px; padding: 0 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ProductionOrder_Select_ProductionOrderPaymentDocument") %></div>
    <div class="h_delim"></div>
    <br />

    <div id="messageProductionOrderPaymentDocumentSelectList"></div>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ProductionOrderPaymentDocumentGrid", Model.ProductionOrderPaymentDocumentGrid); %>
    </div>

    <div class="button_set">
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>
