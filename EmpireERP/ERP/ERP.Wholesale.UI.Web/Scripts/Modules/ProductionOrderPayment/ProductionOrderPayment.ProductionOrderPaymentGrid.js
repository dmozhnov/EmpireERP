var ProductionOrderPayment_ProductionOrderPaymentGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridProductionOrderPayment table.grid_table tr").each(function () {
                var productionOrderId = $(this).find(".ProductionOrderId").text();
                $(this).find("a.ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + productionOrderId + GetBackUrl());

                var producerId = $(this).find(".ProducerId").text();
                $(this).find("a.ProducerName").attr("href", "/Producer/Details?id=" + producerId + GetBackUrl());
            });

            $("#gridProductionOrderPayment .linkPaymentDetails").click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrderPayment/Details",
                    data: { productionOrderPaymentId: id },
                    success: function (result) {
                        $("#productionOrderPaymentEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderPaymentEdit"));
                        ShowModal("productionOrderPaymentEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentList");
                    }
                });
            });

            $("#gridProductionOrderPayment .linkPaymentDelete").click(function () {
                if (confirm('Вы уверены?')) {
                    var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                    var paymentId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/DeleteProductionOrderPayment/",
                        data: { productionOrderId: productionOrderId, paymentId: paymentId },
                        success: function (result) {
                            RefreshGrid("gridProductionOrderPayment", function () {
                                ShowSuccessMessage("Оплата удалена.", "messageProductionOrderPaymentList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentList");
                        }
                    });
                }
            });

        });

    }
};