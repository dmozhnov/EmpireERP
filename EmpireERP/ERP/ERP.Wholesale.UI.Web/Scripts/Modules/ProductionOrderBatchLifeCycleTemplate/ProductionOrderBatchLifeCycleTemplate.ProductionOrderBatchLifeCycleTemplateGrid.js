var ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridProductionOrderBatchLifeCycleTemplate table.grid_table tr").each(function () {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/ProductionOrderBatchLifeCycleTemplate/Details?id=" + id + GetBackUrl());
            });
        });
    }
};