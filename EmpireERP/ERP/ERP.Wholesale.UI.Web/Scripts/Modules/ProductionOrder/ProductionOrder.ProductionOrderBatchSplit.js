var ProductionOrder_ProductionOrderBatchSplit = {

    Init: function () {
        $(document).ready(function () {
            ProductionOrder_ProductionOrderBatchSplit.Initialize();

            // Возврат на прежнюю страницу
            $("#btnBack").click(function () {
                window.location = $("#BackUrl").val();
            });

            $("#btnDoSplit").click(function () {
                if (ProductionOrder_ProductionOrderBatchSplit.ValidateCounts()) {
                    if (confirm('Вы уверены?')) {
                        StartButtonProgress($(this));
                        var productionOrderBatchId = $('#ProductionOrderBatchId').val();
                        var splitInfo = $('#SplitInfo').val();
                        $.ajax({
                            type: "POST",
                            url: "/ProductionOrder/PerformBatchSplit",
                            data: { productionOrderBatchId: productionOrderBatchId, splitInfo: splitInfo },
                            success: function (ajaxContext) {
                                var url = "/ProductionOrder/ProductionOrderBatchDetails?id=" + ajaxContext + GetBackUrlFromString($("#BackUrl").val());
                                window.location = url;
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageEditSplitCount");
                            }
                        });
                    }
                }
                else {
                    ShowErrorMessage("Введено некорректное количество. Отредактируйте данные и попробуйте снова.", "messageEditSplitCount");
                }
            });

            $(".IsSplitted").change(function () {
                if ($(this).attr("checked")) {
                    var count = $(this).parent("td").parent("tr").find(".Count").text();

                    // Изменяем состояния TextEdit-ов
                    $(this).parent("td").parent("tr").find(".SplittedCount").removeAttr("disabled").removeClass("disabled");
                    $(this).parent("td").parent("tr").find(".Remainder").removeAttr("disabled").removeClass("disabled");

                    // Изменяем значения TextEdit-ов
                    $(this).parent("td").parent("tr").find(".SplittedCount").val(count);
                    $(this).parent("td").parent("tr").find(".SplittedCount").focus();
                    $(this).parent("td").parent("tr").find(".Remainder").val("0");

                    ProductionOrder_ProductionOrderBatchSplit.RefreshAll();
                }
                else {
                    // Изменяем состояния TextEdit-ов
                    $(this).parent("td").parent("tr").find(".SplittedCount").attr("disabled", "disabled").addClass("disabled");
                    $(this).parent("td").parent("tr").find(".Remainder").attr("disabled", "disabled").addClass("disabled");

                    // Изменяем значения TextEdit-ов
                    $(this).parent("td").parent("tr").find(".SplittedCount").val("0");
                    $(this).parent("td").parent("tr").find(".Remainder").val("0");

                    ProductionOrder_ProductionOrderBatchSplit.RefreshAll();
                };
            });
        });

        $("#gridProductionOrderBatchSplitRow .SplittedCount").live("keyup change paste cut", function () {
            var precision = TryGetDecimal($(this).parent("td").parent("tr").find(".Precision").text());
            var count = TryGetDecimal($(this).parent("td").parent("tr").find(".Count").text(), precision);
            var splittedCount = TryGetDecimal($(this).parent("td").parent("tr").find(".SplittedCount").val(), precision);
            if (!isNaN(splittedCount)) {
                $(this).parent("td").parent("tr").find(".Remainder").val(ValueForEdit(count - splittedCount));
            }
            ProductionOrder_ProductionOrderBatchSplit.RefreshAll();
        });

        $("#gridProductionOrderBatchSplitRow .Remainder").live("keyup change paste cut", function () {
            var precision = TryGetDecimal($(this).parent("td").parent("tr").find(".Precision").text());
            var count = TryGetDecimal($(this).parent("td").parent("tr").find(".Count").text(), precision);
            var remainder = TryGetDecimal($(this).parent("td").parent("tr").find(".Remainder").val(), precision);
            if (!isNaN(remainder)) {
                $(this).parent("td").parent("tr").find(".SplittedCount").val(ValueForEdit(count - remainder));
            }
            ProductionOrder_ProductionOrderBatchSplit.RefreshAll();
        });

    },

    Initialize: function () {
        // Делаем недоступными все органы ввода TextEdit
        $("#gridProductionOrderBatchSplitRow .SplittedCount").each(function () {
            $(this).attr("disabled", "disabled").addClass("disabled");
            // Сбрасываем значение (для FireFox)
            $(this).val("0");
        });
        $("#gridProductionOrderBatchSplitRow .Remainder").each(function () {
            $(this).attr("disabled", "disabled").addClass("disabled");
            // Сбрасываем значение (для FireFox)
            $(this).val("0");
        });

        // Сбрасываем все галочки (для FireFox)
        $("#gridProductionOrderBatchSplitRow .IsSplitted").each(function () {
            $(this).attr("checked", false);
        });

        // Делаем недоступной кнопку (для FireFox)
        UpdateButtonAvailability("btnDoSplit", false);
    },

    RefreshAll: function () {
        ProductionOrder_ProductionOrderBatchSplit.RefreshSplitInfo();
        ProductionOrder_ProductionOrderBatchSplit.RefreshButtonStates();
    },

    // Функция обновления состояний кнопок. Должна вызываться после перезаполнения поля SplitInfo
    RefreshButtonStates: function () {
        UpdateButtonAvailability("btnDoSplit", $("#SplitInfo").val() != "");
    },

    // Заполнить строку информацией о разделяемых позициях и поместить ее в поле SplitInfo
    RefreshSplitInfo: function () {
        var splitInfo = "";
        var eps = 0.0000005;

        $("#gridProductionOrderBatchSplitRow .IsSplitted").each(function () {
            if ($(this).attr("checked")) {
                var precision = TryGetDecimal($(this).parent("td").parent("tr").find(".Precision").text());
                var splittedCount = TryGetDecimal($(this).parent("td").parent("tr").find(".SplittedCount").val(), precision);
                if (!isNaN(splittedCount)) {
                    // Вставляем в строку только ненулевые значения
                    if (splittedCount > eps) {
                        splitInfo += ($(this).parent("td").parent("tr").find(".Id").text() + "=" + splittedCount + ";");
                    }
                }
            }
        });

        $("#SplitInfo").val(splitInfo);
    },

    // Проверить, все ли количества удовлетворяют условиям
    ValidateCounts: function () {
        var validateCountsResult = true;

        $("#gridProductionOrderBatchSplitRow .IsSplitted").each(function () {
            if ($(this).attr("checked")) {
                var eps = 0.0000005;
                var precision = TryGetDecimal($(this).parent("td").parent("tr").find(".Precision").text());
                var count = TryGetDecimal($(this).parent("td").parent("tr").find(".Count").text());
                var packSize = TryGetDecimal($(this).parent("td").parent("tr").find(".PackSize").text(), precision);
                var splittedCount = TryGetDecimal($(this).parent("td").parent("tr").find(".SplittedCount").val(), precision);
                var remainder = TryGetDecimal($(this).parent("td").parent("tr").find(".Remainder").val(), precision);
                var totalCount = (splittedCount + remainder);

                // Проверяем на нечисловые значения
                if (isNaN(splittedCount)) {
                    $(this).parent("td").parent("tr").find(".SplittedCount").focus();
                    validateCountsResult = false;

                    return (false);
                }
                if (isNaN(remainder)) {
                    $(this).parent("td").parent("tr").find(".Remainder").focus();
                    validateCountsResult = false;

                    return (false);
                }

                // Проверяем на отрицательные значения и на равенство суммы 2 полей общему количеству
                if (splittedCount < -eps || totalCount > count + eps || totalCount < count - eps) {
                    $(this).parent("td").parent("tr").find(".SplittedCount").focus();
                    validateCountsResult = false;

                    return (false);
                }
                if (remainder < -eps) {
                    $(this).parent("td").parent("tr").find(".Remainder").focus();
                    validateCountsResult = false;

                    return (false);
                }

                // Округляем значения до целого количества партий вниз
                var splittedCountBatchCount = Math.floor(splittedCount / packSize);
                // Если значение после округления вниз 0, а само число не 0, то делаем его 1
                if (splittedCountBatchCount < eps && splittedCount > eps)
                    splittedCountBatchCount = 1;
                splittedCount = splittedCountBatchCount * packSize;
                remainder = count - splittedCount;

                // Записываем значение назад с заданной точностью
                $(this).parent("td").parent("tr").find(".SplittedCount").val(ValueForEdit(splittedCount, precision));
                $(this).parent("td").parent("tr").find(".Remainder").val(ValueForEdit(remainder, precision));
            }
        });

        // Так как при округлении перерассчитывались значения, формируем заново строку с информацией о разделении и заодно освежаем статус кнопок
        ProductionOrder_ProductionOrderBatchSplit.RefreshAll();

        return (validateCountsResult);
    }

};