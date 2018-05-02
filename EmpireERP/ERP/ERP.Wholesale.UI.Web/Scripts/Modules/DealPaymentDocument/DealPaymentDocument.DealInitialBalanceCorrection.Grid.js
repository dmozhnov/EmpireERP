var DealPaymentDocument_DealInitialBalanceCorrection_Grid = {
    Init: function (gridName) {
        $(document).ready(function () {
            var grid = $("#" + gridName);

            grid.find("table.grid_table tr").each(function () {
                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + GetBackUrl());

                var clientId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + clientId + GetBackUrl());
            });
        });
    }
};