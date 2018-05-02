var ProductionOrder_List_ActiveProductionOrderGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridActiveProductionOrder table.grid_table tr").each(function () {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/ProductionOrder/Details?id=" + id + GetBackUrl());
                var producerId = $(this).find(".ProducerId").text();
                $(this).find("a.ProducerName").attr("href", "/Producer/Details?id=" + producerId + GetBackUrl());
            });
        });
    }
};