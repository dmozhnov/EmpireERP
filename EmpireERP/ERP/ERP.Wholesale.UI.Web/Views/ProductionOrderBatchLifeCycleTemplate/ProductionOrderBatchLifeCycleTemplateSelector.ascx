<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate.ProductionOrderBatchLifeCycleTemplateSelectViewModel>" %>

<div style="width: 800px; padding: 0 10px 0;">
    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>
    <br />

    <div id="messageProductionOrderBatchLifeCycleTemplateSelectList"></div>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ProductionOrderBatchLifeCycleTemplateSelectGrid", Model.GridData); %>
    </div>

    <div class="button_set">
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>
