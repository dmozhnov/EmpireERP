var ReceiptWaybill_List_DeliveryPendingGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridDeliveryPendingWaybill table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/ReceiptWaybill/Details?id=" + id + "&backURL=" + currentUrl);

                var providerOrProducerId = $(this).find(".ProviderOrProducerId").text();
                var isCreatedFromProductionOrderBatch = $(this).find(".IsCreatedFromProductionOrderBatch").text();
                if (isCreatedFromProductionOrderBatch == "0") {
                    $(this).find("a.ProviderOrProducerName").attr("href", "/Provider/Details?id=" + providerOrProducerId + "&backURL=" + currentUrl);
                } else {
                    $(this).find("a.ProviderOrProducerName").attr("href", "/Producer/Details?id=" + providerOrProducerId + "&backURL=" + currentUrl);
                }

                var storageId = $(this).find(".receiptStorageId").text();
                $(this).find("a.ReceiptStorageName").attr("href", "/Storage/Details?id=" + storageId + "&backURL=" + currentUrl);

                var accountOrganizationId = $(this).find(".accountOrganizationId").text();
                $(this).find("a.AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + accountOrganizationId + "&backURL=" + currentUrl);
            });
        });
    }
}; 