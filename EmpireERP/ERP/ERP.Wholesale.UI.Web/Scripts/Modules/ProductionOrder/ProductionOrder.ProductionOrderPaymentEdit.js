var ProductionOrder_ProductionOrderPaymentEdit = {
    Init: function () {
        $(document).ready(function () {
            $("#main_page #ProductionOrderPaymentTypeId").val($("#productionOrderPaymentEdit #ProductionOrderPaymentTypeId").val());

            $("#linkChangePlannedPayment").click(function () {
                var productionOrderId = $("#ProductionOrderId").val();
                var productionOrderPaymentTypeId = $("#ProductionOrderPaymentTypeId").val();

                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/SelectPlannedPayment",
                    data: { productionOrderId: productionOrderId, productionOrderPaymentTypeId: productionOrderPaymentTypeId, selectFunctionName: "OnProductionOrderPaymentEditSelectLinkClick" },
                    success: function (result) {
                        $("#productionOrderPlannedPaymentSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderPlannedPaymentSelector"));
                        ShowModal("productionOrderPlannedPaymentSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentEdit");
                    }
                });
            });

            //обработка выбора планового платежа для оплаты
            $("#productionOrderPlannedPaymentSelector .selectPlannedPayment").die("click");
            $("#productionOrderPlannedPaymentSelector .selectPlannedPayment").live("click", function () {
                var productionOrderPlannedPaymentId = $(this).parent("td").parent("tr").find(".Id").text();
                var sumInCurrency = $(this).parent("td").parent("tr").find(".SumInCurrency").text();
                var currencyLiteralCode = $(this).parent("td").parent("tr").find(".CurrencyLiteralCode").text();
                var paymentSumInBaseCurrency = $(this).parent("td").parent("tr").find(".PaymentSumInBaseCurrency").text();

                $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentId").val(productionOrderPlannedPaymentId);
                $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentSumInCurrency").text(sumInCurrency);
                $("#productionOrderPaymentEdit .ProductionOrderPlannedPaymentCurrencyLiteralCode").text(currencyLiteralCode);
                $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentPaidSumInBaseCurrency").text(paymentSumInBaseCurrency);

                var productionOrderPaymentId = $("#ProductionOrderPaymentId").val();
                // Если платеж редактируется, то сразу сохраняем изменение планового платежа
                if (productionOrderPaymentId != "00000000-0000-0000-0000-000000000000") {
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrderPayment/ChangeProductionOrderPaymentPlannedPayment",
                        data: { productionOrderPaymentId: productionOrderPaymentId, productionOrderPlannedPaymentId: productionOrderPlannedPaymentId },
                        success: function (result) {
                            HideModal(function () {
                                $("#linkChangePlannedPayment").hide();
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPlannedPaymentSelectList");
                        }
                    });
                } // иначе все изменения будут сохранены при сохранении самой оплаты
                else {
                    HideModal(function () {
                    });
                }
            });
        });
    }
};