var AccountingPriceList_List_AcceptedGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridAcceptedAccountingPriceList table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/AccountingPriceList/Details?id=" + id + "&backURL=" + currentUrl);

                var receiptId = $(this).find(".ReceiptWaybillId").text();
                $(this).find("a.Reason").attr("href", "/ReceiptWaybill/Details?id=" + receiptId + GetBackUrl());
            });
        });
    }
};