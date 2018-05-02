var ReceiptWaybill_Receipt = {
    Init: function () {
        $(document).ready(function () {
            ReceiptWaybill_Receipt.SetGridFieldAttributes();

            if (IsTrue($("#AllowToViewPurchaseCosts").val())) {
                ReceiptWaybill_Receipt.ChangeReceiptedSumColor();
            }

            // Обработка изменения кол-ва принятого на склад товара
            $(".receiptToStorageTextEditor").live("change", function () {
                ReceiptWaybill_Receipt.UpdateEditableCount($(this));
            });

            // Обработка изменения кол-ва товара от поставщика
            $(".providerCountTextEditor").live("change", function () {
                ReceiptWaybill_Receipt.UpdateEditableCount($(this));
            });

            // Обработка изменения суммы по документу
            $(".ProviderSum").live("change", function () {
                ReceiptWaybill_Receipt.UpdateEditableCount($(this));
            });

            // Принятие накладной
            $("#btnDoReceipt").click(function () {
                if (ReceiptWaybill_Receipt.ValidateReceipt()) {
                    var waybillId = $("#WaybillId").val();
                    var sum = IsTrue($("#AllowToViewPurchaseCosts").val()) ? TryGetDecimal($("#TotalReceiptSum").val(), 2) : 0;

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/PerformReceiption",
                        data: { waybillId: waybillId, sum: sum },
                        success: function (result) {
                            window.location = "/ReceiptWaybill/Details?Id=" + waybillId + GetBackUrlFromString($("#BackURL").val());
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            var messageId = $("#messageReceiptWaybill").length > 0 ? "messageReceiptWaybill" : "messageEditReceiptCount";
                            ShowErrorMessage(XMLHttpRequest.responseText, messageId);
                        }
                    });
                }
            });

            //приемка задним числом
            $("#btnDoReceiptRetroactively").live('click', function () {
                if (ReceiptWaybill_Receipt.ValidateReceipt()) {
                    StartButtonProgress($(this));
                    $.ajax({
                        type: "GET",
                        url: "/ReceiptWaybill/ReceiptRetroactively",
                        success: function (result) {
                            $('#dateTimeSelector').hide().html(result);
                            $.validator.unobtrusive.parse($("#dateTimeSelector"));
                            BindRetroactivelyReceiptanceDateSelection();
                            ShowModal("dateTimeSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            var messageId = $("#messageReceiptWaybill").length > 0 ? "messageReceiptWaybill" : "messageEditReceiptCount";
                            ShowErrorMessage(XMLHttpRequest.responseText, messageId);
                        }
                    });
                }
            });

            function BindRetroactivelyReceiptanceDateSelection() {
                $('#btnSelectDateTime').click(function (e) {

                    e.preventDefault();
                    if (!$('#dateTimeSelectForm').valid()) return false;

                    var sum = IsTrue($("#AllowToViewPurchaseCosts").val()) ? TryGetDecimal($("#TotalReceiptSum").val(), 2) : 0;
                    var dateTime = $("#dateTimeSelector #Date").val() + " " + $("#dateTimeSelector #Time").val();
                    var receiptWaybillId = $('#WaybillId').val();

                    StartButtonProgress($("#btnSelectDateTime"));

                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/ReceiptRetroactively",
                        data: { receiptWaybillId: receiptWaybillId, receiptDate: dateTime, receiptanceSum: sum },
                        success: function (result) {
                            HideModal(function () {
                                window.location = "/ReceiptWaybill/Details?Id=" + receiptWaybillId + GetBackUrlFromString($("#BackURL").val());
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDateSelect");
                        }
                    });
                });
            }

            // Возврат к накладной
            $("#btnCloseReceipt").click(function () {
                var waybillId = $("#WaybillId").val();
                window.location = "/ReceiptWaybill/Details?Id=" + waybillId + GetBackUrlFromString($("#BackURL").val());
            });

            // Удаление позиции накладной
            $(".del_link").live("click", function () {
                if (confirm("Вы уверены?")) {
                    var waybillId = $("#WaybillId").val();
                    var rowId = $(this).parent("span").parent("td").parent("tr").find(".rowId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/DeleteWaybillRowFromReceipt/",
                        data: { waybillId: waybillId, rowId: rowId },
                        success: function (result) {
                            RefreshGrid("gridReceipt", function () {
                                ReceiptWaybill_Receipt.SetGridFieldAttributes();
                                ReceiptWaybill_Receipt.ValidateReceipt();
                                ShowSuccessMessage("Позиция удалена.", "messageEditReceiptCount");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageEditReceiptCount");
                        }
                    });
                }
            });

            // Обработка изменения суммы накладной
            // TODO: заменить change на live 4 события (paste cut...)
            $("#TotalReceiptSum").change(function () {
                var sumStr = TryGetDecimal($(this).attr("value"), 2);
                if (!isNaN(sumStr) && sumStr >= 0) {
                    $(this).val(sumStr);
                    $(this).removeClass("input-validation-error");
                    $('#messageReceiptWaybill').empty();
                    ReceiptWaybill_Receipt.ChangeReceiptedSumColor();
                }
                else {
                    $(this).addClass("input-validation-error");
                    ShowErrorMessage("Сумма должна быть числом.", "messageReceiptWaybill");
                }
            });
        });
    },

    SuccessAddedRowToWaybill: function () {
        RefreshGrid("gridReceipt", function () {
            ReceiptWaybill_Receipt.SetGridFieldAttributes();
            ReceiptWaybill_Receipt.ValidateReceipt();
            HideModal(ShowSuccessMessage("Товар добавлен в накладную.", "messageEditReceiptCount"));
        });
    },

    // TODO: проверить право просмотра закупочных цен
    ChangeReceiptedSumColor: function () {
        if (TryGetDecimal($("#TotalReceiptSum").val()) != TryGetDecimal($("#TotalReceiptSumByRows").text().replaceAll(" ", ""))) {
            $("#TotalReceiptSumByRowsCol").addClass("attention");
            $("#TotalReceiptSum").addClass("input-validation-error");
        } else {
            $("#TotalReceiptSum").removeClass("input-validation-error");
        }
    },

    SetGridFieldAttributes: function () {
        $("#gridReceipt .receiptToStorageTextEditor").each(function () {
            $(this).attr("maxlength", 19);
            $(this).parent("td").parent("tr").find(".providerCountTextEditor").attr("maxlength", 19);
            $(this).parent("td").parent("tr").find(".ProviderSum").attr("maxlength", 19);
        });
    },

    ValidateReceipt: function () {
        var allowToViewPurchaseCosts = IsTrue($("#AllowToViewPurchaseCosts").val());
        var receiptValidated = true;
        var allProviderSumsValid = true;
        var receiptSumByRowsCounted = 0;
        $("#gridReceipt .receiptToStorageTextEditor").each(function () {
            var measureUnitScale = TryGetDecimal($(this).parent("td").parent("tr").find(".measureUnitScale").text());
            var receiptCountValid = CheckValueScale($(this).val(), measureUnitScale, 12);
            var providerCountValid = CheckValueScale($(this).parent("td").parent("tr").find(".providerCountTextEditor").val(), measureUnitScale, 12);
            var providerSumValid = allowToViewPurchaseCosts ? CheckValueScale($(this).parent("td").parent("tr").find(".ProviderSum").val(), 2, 16) : true;
            var receiptCount = TryGetDecimal($(this).val(), measureUnitScale);
            var providerCount = TryGetDecimal($(this).parent("td").parent("tr").find(".providerCountTextEditor").val(), measureUnitScale);
            var providerSum = allowToViewPurchaseCosts ? TryGetDecimal($(this).parent("td").parent("tr").find(".ProviderSum").val(), 2) : 0;
            
            if (providerSumValid && providerSum >= 0) {
                receiptSumByRowsCounted += providerSum;
            } else {
                allProviderSumsValid = false;
            }
            if (!receiptCountValid || receiptCount < 0 || !providerCountValid || providerCount < 0 || !providerSumValid || providerSum < 0) {
                ShowErrorMessage(allowToViewPurchaseCosts ? "Суммы и количества должны быть числами с соответствующим количеством знаков после запятой." :
                    "Количества товара должны быть числами с соответствующим количеством знаков после запятой.", "messageEditReceiptCount");
                receiptValidated = false;
            }
            // Если оба признака уже установлены в false, прекращаем перебор (ничего нового уже не будет), т.е. возвращаем false
            // (При allowToViewPurchaseCosts == false игнорируем первый признак)
            if ((!allProviderSumsValid || !allowToViewPurchaseCosts) && !receiptValidated) {
                return false;
            }
        });
        var receiptSumValid = allowToViewPurchaseCosts ? CheckValueScale($("#TotalReceiptSum").val(), 2, 16) : true;
        var receiptSum = allowToViewPurchaseCosts ? TryGetDecimal($("#TotalReceiptSum").val(), 2) : 0;
        if (!receiptSumValid || isNaN(receiptSum) || receiptSum < 0) {
            ShowErrorMessage("Общая сумма накладной по документу должна быть числом в правильном формате.", "messageReceiptWaybill");
            receiptValidated = false;
        }
                
        if (allowToViewPurchaseCosts) {
            if (receiptSum != ValueForEdit(receiptSumByRowsCounted, 2)) {
                ShowErrorMessage("Общая сумма по накладной не сходится с суммой по позициям.", "messageReceiptWaybill");
                receiptValidated = false;
            }
            if (allProviderSumsValid) {
                $("#TotalReceiptSumByRows").text(ValueForDisplay(receiptSumByRowsCounted, 2));
                $("#TotalReceiptSumByRowsCol").removeClass("attention");
            } else {
                $("#TotalReceiptSumByRows").text("---");
                $("#TotalReceiptSumByRowsCol").addClass("attention");
            }
            ReceiptWaybill_Receipt.ChangeReceiptedSumColor();
        }

        return receiptValidated;
    },

    UpdateEditableCount: function (_this) {
        var allowToViewPurchaseCosts = IsTrue($("#AllowToViewPurchaseCosts").val());

        var receiptedCount = _this.parent("td").parent("tr").find(".receiptToStorageTextEditor");
        var providerCount = _this.parent("td").parent("tr").find(".providerCountTextEditor");
        var providerSum = _this.parent("td").parent("tr").find(".ProviderSum");

        var rowId = _this.parent("td").parent("tr").find(".rowId").text();
        var scale = _this.parent("td").parent("tr").find(".measureUnitScale").text();
        var waybillId = $("#WaybillId").val();

        var __this;
        if (_this.hasClass("receiptToStorageTextEditor")) {
            __this = receiptedCount;
        }
        else if (_this.hasClass("providerCountTextEditor")) {
            __this = providerCount;
        } else {
            __this = providerSum;
        }

        var receiptedCountResult = CheckValueScale(receiptedCount.val(), scale, 12);
        var providerCountResult = CheckValueScale(providerCount.val(), scale, 12);
        var providerSumResult = allowToViewPurchaseCosts ? CheckValueScale(providerSum.val(), 2, 16) : true;
        var pendingCount = _this.parent("td").parent("tr").find(".Count").text();

        // TODO: переделать. А если receiptedCount.val() == "00" и providerCount.val() == "00" ?
        if (receiptedCountResult == true && providerCountResult == true && providerSumResult == true && (receiptedCount.val() != 0 || providerCount.val() != 0 || pendingCount != "0")) {
            $.ajax({
                type: "POST",
                url: "/ReceiptWaybill/EditWaybillRowFromReceipt/",
                data: { waybillId: waybillId, rowId: rowId, receiptedCount: receiptedCount.val(), providerCount: providerCount.val(), providerSum: allowToViewPurchaseCosts ? providerSum.val() : 0 },
                success: function (result) {
                    receiptedCount.removeClass("field-validation-success");
                    providerCount.removeClass("field-validation-success");
                    providerSum.removeClass("field-validation-success");
                    __this.removeClass("field-validation-error").addClass("field-validation-success");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    __this.removeClass("field-validation-success");
                    __this.addClass("field-validation-error");
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageEditReceiptCount");
                }
            });
        }
        else {
            if (receiptedCountResult == false) {
                receiptedCount.removeClass("field-validation-success").addClass("field-validation-error");
            }
            else {
                receiptedCount.removeClass("field-validation-error").addClass("field-validation-success");
            }
            if (providerCountResult == false) {
                providerCount.removeClass("field-validation-success").addClass("field-validation-error");
            }
            else {
                providerCount.removeClass("field-validation-error").addClass("field-validation-success");
            }
            if (providerSumResult == false) {
                providerSum.removeClass("field-validation-success").addClass("field-validation-error");
            }
            else {
                providerSum.removeClass("field-validation-error").addClass("field-validation-success");
            }

            if (receiptedCount.val() == 0 && providerCount.val() == 0 && pendingCount == "0") {
                providerCount.removeClass("field-validation-success").addClass("field-validation-error");
                receiptedCount.removeClass("field-validation-success").addClass("field-validation-error");
                ShowErrorMessage("Одно из значений по позиции должно быть больше 0.", "messageEditReceiptCount");
            }
            else {
                if (providerSumResult == false) {
                    ShowErrorMessage("Укажите сумму - целое положительное число, до 2 знаков после запятой.", "messageEditReceiptCount");
                }
                else if (scale == 0)
                    ShowErrorMessage("Укажите целое положительное число.", "messageEditReceiptCount");
                else
                    ShowErrorMessage("Указано неверное количество товара. Допускается не более " + scale + " десятичных знаков.", "messageEditReceiptCount");
            }
        }
        ReceiptWaybill_Receipt.ValidateReceipt();
    }
};