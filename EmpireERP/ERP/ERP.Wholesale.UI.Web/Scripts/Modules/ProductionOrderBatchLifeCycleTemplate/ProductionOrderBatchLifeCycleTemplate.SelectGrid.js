var ProductionOrderBatchLifeCycleTemplate_SelectGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridProductionOrderBatchLifeCycleTemplateSelect table.grid_table tr").each(function () {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/ProductionOrderBatchLifeCycleTemplate/Details?id=" + id + GetBackUrl());
            });

            // Действия после выбора из грида (ссылка "Выбрать")
            $(".linkProductionOrderBatchLifeCycleTemplateSelect").click(function () {
                var productionOrderBatchLifeCycleTemplateId = $(this).parent("td").parent("tr").find(".Id").text();
                var productionOrderBatchLifeCycleTemplateName = $(this).parent("td").parent("tr").find(".Name").text();
                OnProductionOrderBatchLifeCycleTemplateSelectLinkClick(productionOrderBatchLifeCycleTemplateId, productionOrderBatchLifeCycleTemplateName);
            });
        });
    }
};