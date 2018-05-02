var ProductionOrderPayment_List = {
    Init: function () {
        $("#productionOrderPaymentEdit #linkChangePaymentCurrencyRate").live("click", function () {
            var currencyId = $("#productionOrderPaymentEdit #PaymentCurrencyId").val();
            if (!IsDefaultOrEmpty(currencyId)) {
                $.ajax({
                    type: "GET",
                    url: "/Currency/SelectCurrencyRate",
                    data: { currencyId: currencyId, selectFunctionName: "OnProductionOrderPaymentEditCurrencyRateSelectLinkClick" },
                    success: function (result) {
                        $("#currencyRateSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyRateSelector"));
                        ShowModal("currencyRateSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentEdit");
                    }
                });
            }
        });
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора курсов валюты из формы редактирования оплаты
    OnProductionOrderPaymentEditCurrencyRateSelectLinkClick: function (currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        var productionOrderPaymentId = $("#productionOrderPaymentEdit #ProductionOrderPaymentId").val();
        $.ajax({
            type: "POST",
            url: "/ProductionOrderPayment/ChangeProductionOrderPaymentCurrencyRate",
            data: { productionOrderPaymentId: productionOrderPaymentId, currencyRateId: currencyRateId },
            success: function (result) {
                // Обновление модальной формы
                $("#productionOrderPaymentEdit #PaymentCurrencyRateId").val(productionOrderPaymentId);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateName").text(result.PaymentCurrencyRateName);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateString").text(result.PaymentCurrencyRateString);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val(result.PaymentCurrencyRateValue);
                ProductionOrder_Details.RecalculateProductionOrderPaymentSumInBaseCurrency();

                RefreshGrid("gridProductionOrderPayment", function () {

                    var productionOrderPlannedPaymentId = $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentId").val();
                    // Если плановый платеж указан, то ...
                    if (productionOrderPaymentId != "00000000-0000-0000-0000-000000000000") {
                        // ... запрашиваем детали плановой оплаты
                        $.ajax({
                            type: "POST",
                            url: "/ProductionOrder/GetPlannedPaymentInfo",
                            data: { productionOrderPlannedPaymentId: productionOrderPlannedPaymentId },
                            success: function (result) {
                                // Обновление полей
                                $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentSumInCurrency").text(result.PlannedPaymentSumInCurrency);
                                $("#productionOrderPaymentEdit .ProductionOrderPlannedPaymentCurrencyLiteralCode").text(result.PlannedPaymentCurrencyLiteralCode);
                                $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentPaidSumInBaseCurrency").text(result.PaymentSumInBaseCurrency);
                                HideModal(function () {
                                    ShowSuccessMessage("Курс оплаты сохранен.", "messageProductionOrderPaymentEdit");
                                });
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyEdit");
                            }
                        });
                    } else {    // иначе закрываем МФ
                        HideModal(function () {
                            ShowSuccessMessage("Курс оплаты сохранен.", "messageProductionOrderPaymentEdit");
                        });
                    }

                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyEdit");
            }
        });
    }
};