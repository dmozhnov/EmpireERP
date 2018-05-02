var DealPaymentDocument_DestinationDocumentSelector_SelectGrid = {
    // Общие методы для всех гридов выбора документов при разнесении платежных документов
    Init: function (gridId) {
        $("#" + gridId + " .IsPaid").change(function (elem) {
            var eps = 0.0000005;

            var modalForm = $("#" + gridId).closest("form");

            var undistributedSum = TryGetDecimal(modalForm.find("#UndistributedSumValue").val(), 2);

            if ($(this).attr("checked")) {
                var debtRemainderValue = TryGetDecimal($(this).parent("td").parent("tr").find(".DebtRemainderValue").text(), 2);

                var saleWaybillPaymentSum;
                if (debtRemainderValue < undistributedSum)
                    saleWaybillPaymentSum = debtRemainderValue;
                else
                    saleWaybillPaymentSum = undistributedSum;

                $(this).parent("td").parent("tr").find(".CurrentPaymentSumValue").text(ValueForEdit(saleWaybillPaymentSum, 2));
                $(this).parent("td").parent("tr").find(".CurrentPaymentSumString").text(ValueForDisplay(saleWaybillPaymentSum, 2));

                var newUndistributedSum = undistributedSum - saleWaybillPaymentSum;

                modalForm.find("#UndistributedSumValue").val(ValueForEdit(newUndistributedSum, 2));
                modalForm.find("#UndistributedSumString").text(ValueForDisplay(newUndistributedSum, 2));

                if (newUndistributedSum < eps) {
                    modalForm.find(".IsPaid").each(function () {
                        if (!$(this).attr("checked")) {
                            $(this).attr("disabled", "disabled");
                        }
                    });
                }

                var currentOrdinalNumber = TryGetDecimal(modalForm.find("#CurrentOrdinalNumber").val(), 0);
                $(this).parent("td").parent("tr").find(".OrdinalNumber").text(ValueForEdit(currentOrdinalNumber, 0));
                modalForm.find("#CurrentOrdinalNumber").val(ValueForEdit(currentOrdinalNumber + 1, 0));
                DealPaymentDocument_DestinationDocumentSelector_SelectGrid.RefreshDistributionInfo(modalForm);
            }
            else {
                var newUndistributedSum = undistributedSum + TryGetDecimal($(this).parent("td").parent("tr").find(".CurrentPaymentSumValue").text(), 2);

                modalForm.find("#UndistributedSumValue").val(ValueForEdit(newUndistributedSum, 2));
                modalForm.find("#UndistributedSumString").text(ValueForDisplay(newUndistributedSum, 2));
                $(this).parent("td").parent("tr").find(".CurrentPaymentSumValue").text("0");
                $(this).parent("td").parent("tr").find(".CurrentPaymentSumString").text("0");

                modalForm.find(".IsPaid").each(function () {
                    $(this).removeAttr("disabled");
                });

                $(this).parent("td").parent("tr").find(".OrdinalNumber").text("0");
                DealPaymentDocument_DestinationDocumentSelector_SelectGrid.RefreshDistributionInfo(modalForm);
            };
        });
    },

    RefreshDistributionInfo: function (modalForm) {
        var distributionInfo = "";
        var eps = 0.0000005;

        // Грид накладных реализации содержит записи с типом 1 - «накладная реализации товаров» (в конце _1)
        modalForm.find("#gridSaleWaybillSelect .IsPaid").each(function () {
            if ($(this).attr("checked")) {
                var currentPaymentSumValue = TryGetDecimal($(this).parent("td").parent("tr").find(".CurrentPaymentSumValue").text(), 2);
                if (!isNaN(currentPaymentSumValue)) {
                    // Вставляем в строку только ненулевые значения
                    if (currentPaymentSumValue > eps) {
                        distributionInfo += ($(this).parent("td").parent("tr").find(".Id").text() + "=" +
                            $(this).parent("td").parent("tr").find(".OrdinalNumber").text() + "_" +
                            $(this).parent("td").parent("tr").find(".CurrentPaymentSumValue").text() + "_1;");
                    }
                }
            }
        });

        // Грид дебетовых корректировок сальдо содержит записи с типом 2 - «дебетовая корректировка сальдо» (в конце _2)
        modalForm.find("#gridDealDebitInitialBalanceCorrectionSelect .IsPaid").each(function () {
            if ($(this).attr("checked")) {
                var currentPaymentSumValue = TryGetDecimal($(this).parent("td").parent("tr").find(".CurrentPaymentSumValue").text(), 2);
                if (!isNaN(currentPaymentSumValue)) {
                    // Вставляем в строку только ненулевые значения
                    if (currentPaymentSumValue > eps) {
                        distributionInfo += ($(this).parent("td").parent("tr").find(".Id").text() + "=" +
                            $(this).parent("td").parent("tr").find(".OrdinalNumber").text() + "_" +
                            $(this).parent("td").parent("tr").find(".CurrentPaymentSumValue").text() + "_2;");
                    }
                }
            }
        });

        modalForm.find("#DistributionInfo").val(distributionInfo);
    }
};