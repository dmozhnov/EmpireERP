var AccountingPriceList_List_NewGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridNewAccountingPriceList table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/AccountingPriceList/Details?id=" + id + GetBackUrl());

                var receiptId = $(this).find(".ReceiptWaybillId").text();
                $(this).find("a.Reason").attr("href", "/ReceiptWaybill/Details?id=" + receiptId + GetBackUrl());
            });

            $('#btnCreateAccountingPriceListRevaluation').click(function () {
                window.location = "/AccountingPriceList/Create?reasonCode=2" + GetBackUrl();
            });
        });
    }
};