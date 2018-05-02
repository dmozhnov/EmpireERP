var Producer_ProducerPaymentsGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridProducerPayments table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".ProductionOrderId").text();
                $(this).find("a.ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $(".paymentDelete").click(function () {
                var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                var paymentId = $(this).parent("td").parent("tr").find(".Id").text();

                if (confirm("Вы действительно хотите удалить оплату?")) {
                    $.ajax({
                        type: "POST",
                        url: "/Producer/DeleteProducerPayment",
                        data: { productionOrderId: productionOrderId, paymentId: paymentId },
                        success: function (result) {
                            Producer_Details.RefreshMainDetails(result);
                            RefreshGrid("gridProducerPayments", function () {
                                ShowSuccessMessage("Оплата удалена", "messagePaymentsList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messagePaymentsList");
                        }
                    });
                }
            });

            $(".paymentDetails").click(function () {
                var paymentId = $(this).parent("td").parent("tr").find(".Id").text();

                $.ajax({
                    type: "GET",
                    url: "/ProductionOrderPayment/Details",
                    data: { productionOrderPaymentId: paymentId },
                    success: function (result) {
                        $("#productionOrderPaymentEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderPaymentEdit"));
                        ShowModal("productionOrderPaymentEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messagePaymentsList");
                    }
                });
            });

        });
    }
};