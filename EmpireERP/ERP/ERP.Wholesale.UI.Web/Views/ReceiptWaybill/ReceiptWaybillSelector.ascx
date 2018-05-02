<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptWaybillSelectViewModel>" %>

<div style="width: 800px; padding: 10px 10px 0;">
    <%= Html.GridFilterHelper("filterReceiptWaybill", Model.FilterData,
        new List<string>() { "gridSelectReceiptWaybill" }, true)%>
    
    <div id="messageReceiptWaybillSelectList"></div>    
    <div id="grid_receipt_waybill_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ReceiptWaybillSelectGrid", Model.Data); %>
    </div>

    <div class="button_set">
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>