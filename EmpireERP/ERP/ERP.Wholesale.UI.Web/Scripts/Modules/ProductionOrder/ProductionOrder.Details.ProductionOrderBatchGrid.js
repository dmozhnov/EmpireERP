var ProductionOrder_Details_ProductionOrderBatchGrid = {
    Init: function () {
        $(document).ready(function () {

            $("#gridProductionOrderBatch table.production_order_batch tr").each(function () {
                var id = $(this).find(".Id").val();
                $(this).find("a.linkDetails").attr("href", "/ProductionOrder/ProductionOrderBatchDetails?id=" + id + GetBackUrl());

                var receiptWaybillId = $(this).find(".ReceiptWaybillId").val();
                if (receiptWaybillId != "" && receiptWaybillId != "00000000-0000-0000-0000-000000000000") {
                    $(this).find("a.ReceiptWaybillName").attr("href", "/ReceiptWaybill/Details?id=" + receiptWaybillId + GetBackUrl());
                }
            });

            //Вызвать модальное окно создания партии
            $("#btnCreateNewOrderBatch").click(function () {
                var productionOrderId = $("#Id").val();
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/AddProductionOrderBatch",
                    data: { productionOrderId: productionOrderId },

                    success: function (result) {
                        $("#productionOrderAddBatch").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderAddBatch"));
                        ShowModal("productionOrderAddBatch");
                    },

                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAddProductionOrderBatch");
                    }
                });
            });


        });

    }

};