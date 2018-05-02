<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderBatchStagesEditViewModel>" %>

<script type="text/javascript">
    ProductionOrder_ProductionOrderBatchEditStages.Init();
</script>

<div style="width:800px;">
    <%:Html.HiddenFor(model => model.ProductionOrderBatchId)%>

    <div class="modal_title"><%:Model.Title%></div>

    <% if (Model.AllowToLoadFromTemplate || Model.AllowToEdit) { %>
        <div class="h_delim"></div>
        <br />
        <% if (Model.AllowToLoadFromTemplate) { %><span style="margin-left:20px"><span id="linkLoadFromTemplate" class="link">Загрузить из шаблона</span></span><%} %>
        <% if (Model.AllowToEdit) { %><span style="margin-left:20px"><span id="linkClearCustomStages" class="link">Очистить этапы</span></span><%} %>
        <br />
    <% } %>
    <br />

    <div id="messageProductionOrderBatchEditStages"></div>

    <div style="max-height: 420px; padding: 0px 10px 0px; overflow: auto;">
        <% Html.RenderPartial("ProductionOrderBatchStageGrid", Model.ProductionOrderBatchStageGrid);%>
    </div>

    <div class='button_set'>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>

    <div id="productionOrderBatchStageEdit"></div>
    <div id="productionOrderBatchLifeCycleTemplateSelector"></div>
</div>