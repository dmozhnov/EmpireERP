var ExpenditureWaybill_ShippedGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridShippedExpenditureWaybill table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/ExpenditureWaybill/Details?id=" + id + "&backURL=" + currentUrl);

                var clientId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + clientId + "&backURL=" + currentUrl);

                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + "&backURL=" + currentUrl);

                var storageId = $(this).find(".StorageId").text();
                $(this).find("a.StorageName").attr("href", "/Storage/Details?id=" + storageId + "&backURL=" + currentUrl);
            });
        });
    }
};