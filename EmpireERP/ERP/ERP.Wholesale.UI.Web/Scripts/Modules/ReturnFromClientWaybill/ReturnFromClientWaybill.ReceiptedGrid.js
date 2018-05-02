var ReturnFromClientWaybill_ReceiptedGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridReceiptedReturnFromClientWaybill table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/ReturnFromClientWaybill/Details?id=" + id + GetBackUrl());

                var clientId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + clientId + GetBackUrl());

                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + GetBackUrl());

                var recipientStorageId = $(this).find(".RecipientStorageId").text();
                $(this).find("a.RecipientStorageName").attr("href", "/Storage/Details?id=" + recipientStorageId + GetBackUrl());

                var recipientId = $(this).find(".RecipientId").text();
                $(this).find("a.RecipientName").attr("href", "/AccountOrganization/Details?id=" + recipientId + GetBackUrl());                
            });
        });
    }
};