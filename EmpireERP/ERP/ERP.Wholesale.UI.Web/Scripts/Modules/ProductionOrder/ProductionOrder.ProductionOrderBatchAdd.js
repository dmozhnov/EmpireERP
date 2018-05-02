var ProductionOrder_ProductionOrderBatchAdd = {
    OnBeginProductionOrderBatchSave: function () {
        StartButtonProgress($("#btnProductionOrderAddBatch"));
    },

    OnFailProductionOrderBatchSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageModalWindowAddBatch");
    },

    OnSuccessProductionOrderBatchSave: function (result) {
        if ($("#productionOrderAddBatch #Id").val() == "00000000-0000-0000-0000-000000000000") {
            window.location = "/ProductionOrder/ProductionOrderBatchDetails?id=" + result + GetBackUrl();
        } 
        else {
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/GetProductionOrderBatchName",
                data: { productionOrderBatchId: result },
                success: function (name) {
                    $(".page_title_item_name").text(name);
                    ShowSuccessMessage("Партия переименована.", "messageProductionOrderBatchEdit");
                    HideModal();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageModalWindowAddBatch");
                }
            });
        }
    }



};