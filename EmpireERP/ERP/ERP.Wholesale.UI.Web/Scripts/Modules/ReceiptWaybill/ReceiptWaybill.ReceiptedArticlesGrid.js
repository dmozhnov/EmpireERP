var ReceiptWaybill_ReceiptedArticlesGrid = {
    Init: function () {
        $(document).ready(function () {

            // Добавление товара в накладную
            $("#btnAddArticle").bind("click", function () {
                var waybillId = $("#WaybillId").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ReceiptWaybill/AddWaybillRowFromReceipt/",
                    data: { waybillId: waybillId },
                    success: function (result) {
                        $('#receiptArticleAdd').hide().html(result);
                        ShowModal("receiptArticleAdd");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageEditReceiptCount");
                    }
                });
            });

        });
    }
}; 