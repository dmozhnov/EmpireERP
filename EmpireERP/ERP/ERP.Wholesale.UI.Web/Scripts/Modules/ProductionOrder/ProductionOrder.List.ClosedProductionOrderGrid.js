var ProductionOrder_List_ClosedProductionOrderGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridClosedProductionOrder table.grid_table tr").each(function () {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/ProductionOrder/Details?id=" + id + GetBackUrl());
                var producerId = $(this).find(".ProducerId").text();
                $(this).find("a.ProducerName").attr("href", "/Producer/Details?id=" + producerId + GetBackUrl());
            });
        });
    }
};