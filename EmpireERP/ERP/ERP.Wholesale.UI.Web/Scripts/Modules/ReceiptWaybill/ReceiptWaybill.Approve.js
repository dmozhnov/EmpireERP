ReceiptWaybill_Approve = {
    Init: function () {
        $(document).ready(function () {
            ReceiptWaybill_Approve.DisableZeroCountRows();
            ReceiptWaybill_Approve.SetGridFieldAttributes();

            $('#btnCloseApprovement').live('click', function () {
                var waybillId = $("#WaybillId").val();
                window.location = '/ReceiptWaybill/Details?id=' + waybillId + GetBackUrlFromString($('#BackURL').val());
            });

            // Обработка изменения кол-ва согласованного товара
            $("#gridApproveArticles input.approveArticleCount").live("change", function () {
                ReceiptWaybill_Approve.UpdateEditableSumParameter($(this));
            });

            var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();

            if (IsTrue(allowToViewPurchaseCosts)) {
                $("#gridApproveArticles input.ApprovedSum").live("change", function () {
                    ReceiptWaybill_Approve.UpdateEditableSumParameter($(this));
                });

                $("#gridApproveArticles input.ApprovedPurchaseCost").live("change", function () {
                    ReceiptWaybill_Approve.UpdateEditableSumParameter($(this));
                });

                $('#TotalApprovedSum').live('change', function () {
                    var approvedSumValid = CheckValueScale($(this).val(), 2, 16);
                    var approvedSum = TryGetDecimal($(this).val());

                    if (approvedSumValid && approvedSum >= 0) {
                        $('[data-valmsg-for="TotalApprovedSum"]').removeClass("field-validation-error").addClass("field-validation-valid");
                        $(this).removeClass("field-validation-error");
                    }
                    else {
                        $('[data-valmsg-for="TotalApprovedSum"]').removeClass("field-validation-valid").addClass("field-validation-error");
                        $(this).addClass("field-validation-error");
                    }
                });
            }

            // Изменение выпадающего списка с НДС
            $(".ApprovedValueAddedTax").live("change", function () {
                // Пересчитываем НДС в текущей строке
                ReceiptWaybill_Approve.CalcValueAddedTaxForArticle($(this).parent("td").parent("tr"));

                var valueAddedTaxId = TryGetDecimal($(this).find("option:selected").val());

                if (!isNaN(valueAddedTaxId)) {
                    var waybillId = $("#WaybillId").val();
                    var rowId = $(this).parent("td").parent("tr").find(".rowId").text();
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/EditWaybillRowValueAddedTaxFromApprovement/",
                        data: { waybillId: waybillId, rowId: rowId, valueAddedTaxId: valueAddedTaxId },
                        success: function (result) {
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageEditApproveCount");
                        }
                    });
                }
            });

            // выполнение согласования
            $('#btnApprove').live('click', function () {
                if (ReceiptWaybill_Approve.ValidateApprove()) {
                    var waybillId = $("#WaybillId").val();
                    var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();
                    if (IsTrue(allowToViewPurchaseCosts))
                        var approvedSum = TryGetDecimal($('#TotalApprovedSum').val(), 2);
                    else
                        var approvedSum = -1;

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/PerformApprovement",
                        data: { waybillId: waybillId, sum: approvedSum },
                        success: function (result) {
                            window.location = "/ReceiptWaybill/Details?Id=" + waybillId + GetBackUrlFromString($('#BackURL').val());
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageApproveWaybill");
                        }
                    });
                }
            });

            //Согласование задним числом
            $("#btnApproveRetroactively").live('click', function () {
                if (ReceiptWaybill_Approve.ValidateApprove()) {
                    StartButtonProgress($(this));
                    $.ajax({
                        type: "GET",
                        url: "/ReceiptWaybill/ApproveRetroactively",
                        success: function (result) {
                            $('#dateTimeSelector').hide().html(result);
                            $.validator.unobtrusive.parse($("#dateTimeSelector"));
                            BindRetroactivelyApprovementDateSelection();
                            ShowModal("dateTimeSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageApproveWaybill");
                        }
                    });
                }
            });

            function BindRetroactivelyApprovementDateSelection() {
                $('#btnSelectDateTime').click(function (e) {

                    e.preventDefault();
                    if (!$('#dateTimeSelectForm').valid()) return false;

                    var receiptWaybillId = $("#WaybillId").val();
                    var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();
                    if (IsTrue(allowToViewPurchaseCosts))
                        var approvedSum = TryGetDecimal($('#TotalApprovedSum').val(), 2);
                    else
                        var approvedSum = -1;
                    var dateTime = $("#dateTimeSelector #Date").val() + " " + $("#dateTimeSelector #Time").val();

                    StartButtonProgress($("#btnSelectDateTime"));

                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/ApproveRetroactively",
                        data: { receiptWaybillId: receiptWaybillId, approvementDate: dateTime, approvedSum: approvedSum },
                        success: function (result) {
                            HideModal(function () {
                                window.location = "/ReceiptWaybill/Details?Id=" + receiptWaybillId + GetBackUrlFromString($('#BackURL').val());
                                });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDateSelect");
                        }
                    });
                });
            }
        });
    },

    // Действия при изменении одного из показателей по позиции, влияющих на итоговую стоимость (согл. зак. цена / кол-во товара / согл. сумма по позиции)
    UpdateEditableSumParameter: function (_this) {
        var rowId = _this.parent("td").parent("tr").find(".rowId").text();
        var waybillId = $("#WaybillId").val();
        var scale = _this.parent("td").parent("tr").find(".measureUnitScale").text();

        // Признаки, какое из полей изменено (передано нам)
        var approvedCountChanged = _this.hasClass("approveArticleCount");
        var approvedPurchaseCostChanged = _this.hasClass("ApprovedPurchaseCost");
        var approvedSumChanged = _this.hasClass("ApprovedSum");

        // Поля ввода (text edit). Внимание! Функции DisableSumAndPurchaseCost и EnableSumAndPurchaseCost пересоздают эти элементы, после них надо получать заново
        var approvedCountEdit = _this.parent("td").parent("tr").find("input.approveArticleCount");
        var approvedPurchaseCostEdit = _this.parent("td").parent("tr").find("input.ApprovedPurchaseCost");
        var approvedSumEdit = _this.parent("td").parent("tr").find("input.ApprovedSum");

        var approvedSumIsChangedLastSpan = _this.parent("td").parent("tr").find(".ApprovedSumIsChangedLast");

        // Результаты проверки на количество знаков (и корректность формата)
        var approvedCountValid = CheckValueScale(approvedCountEdit.val(), scale, 12);
        var approvedPurchaseCostValid = CheckValueScale(approvedPurchaseCostEdit.val(), 6, 12);
        var approvedSumValid = CheckValueScale(approvedSumEdit.val(), 2, 16);

        // Сами значения (NaN, если формат некорректен)
        var approvedCount = TryGetDecimal(approvedCountEdit.val());
        var approvedPurchaseCost = TryGetDecimal(approvedPurchaseCostEdit.val());
        var approvedSum = TryGetDecimal(approvedSumEdit.val());

        var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();
        //Если нет прав на просмотр закупочных цен, то ничего пересчитывать не надо, просто отправляем на сервер информацию о изменениях в поле количество
        if (IsFalse(allowToViewPurchaseCosts)) {
            $.ajax({
                type: "POST",
                url: "/ReceiptWaybill/EditWaybillRowFromApprovement/",
                data: { waybillId: waybillId, rowId: rowId, approvedCount: approvedCount, purchaseCost: -1 },
                success: function (result) {
                    _this.addClass("field-validation-success");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    _this.addClass("field-validation-error");
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageEditApproveCount");
                }
            });
            return;
        }

        // Признаки, что в каком-то из полей есть ошибка (возможно, уже давно)
        var approvedCountError = !approvedCountValid || approvedCount < 0;
        var approvedPurchaseCostError = !approvedPurchaseCostValid || approvedPurchaseCost < 0;
        var approvedSumError = !approvedSumValid || approvedSum < 0;

        // Признаки, что в каком-то из полей сразу после его изменения была обнаружена ошибка
        var approvedCountRecentError = approvedCountChanged && approvedCountError;
        var approvedPurchaseCostRecentError = approvedPurchaseCostChanged && approvedPurchaseCostError;
        var approvedSumRecentError = approvedSumChanged && approvedSumError;

        // Пересчитываем показатели в зависимости от того, какой был изменен. Пересчитываемым показателям меняем значения и ставим флаг корректности
        if (approvedCountChanged && approvedCountValid) {
            if (approvedCount == 0) {
                approvedPurchaseCost = 0;
                approvedSum = 0;
                approvedPurchaseCostValid = true; approvedPurchaseCostError = approvedPurchaseCostRecentError = false;
                approvedSumValid = true; approvedSumError = approvedSumRecentError = false;
                ReceiptWaybill_Approve.DisableSumAndPurchaseCost(_this.parent("td").parent("tr"));
            } else {
                if (approvedPurchaseCostValid) {
                    ReceiptWaybill_Approve.EnableSumAndPurchaseCost(_this.parent("td").parent("tr"));
                    approvedSum = BankRound(approvedPurchaseCost * approvedCount, 2);
                    approvedSumValid = true; approvedSumError = approvedSumRecentError = false;
                    // input для approvedPurchaseCost и approvedSum могли быть пересозданы EnableSumAndPurchaseCost() и должны быть получены заново
                    _this.parent("td").parent("tr").find("input.ApprovedSum").val(approvedSum);
                }
            }
        }
        if (approvedPurchaseCostChanged && approvedPurchaseCostValid && approvedCountValid) {
            approvedSum = BankRound(approvedPurchaseCost * approvedCount, 2);
            approvedSumValid = true; approvedSumError = approvedSumRecentError = false;
            approvedSumEdit.val(approvedSum);
            approvedSumIsChangedLastSpan.text("0");
        }
        if (approvedSumChanged && approvedSumValid && approvedCountValid) {
            approvedPurchaseCost = BankRound(approvedSum / approvedCount, 6);
            approvedPurchaseCostValid = true; approvedPurchaseCostError = approvedPurchaseCostRecentError = false;
            approvedPurchaseCostEdit.val(approvedPurchaseCost);
            approvedSumIsChangedLastSpan.text("1");
        }

        // input для approvedPurchaseCost и approvedSum могли быть пересозданы EnableSumAndPurchaseCost/DisableSumAndPurchaseCost и должны быть получены заново
        approvedPurchaseCostEdit = _this.parent("td").parent("tr").find("input.ApprovedPurchaseCost");
        approvedSumEdit = _this.parent("td").parent("tr").find("input.ApprovedSum");

        approvedCountEdit.removeClass("field-validation-error").removeClass("field-validation-success");
        approvedPurchaseCostEdit.removeClass("field-validation-error").removeClass("field-validation-success");
        approvedSumEdit.removeClass("field-validation-error").removeClass("field-validation-success");

        if (approvedCountError) {
            approvedCountEdit.addClass("field-validation-error");
        }
        if (approvedPurchaseCostError) {
            approvedPurchaseCostEdit.addClass("field-validation-error");
        }
        if (approvedSumError) {
            approvedSumEdit.addClass("field-validation-error");
        }

        if (approvedCountRecentError || approvedPurchaseCostRecentError || approvedSumRecentError) {
            if (approvedCountRecentError) {
                if (scale == 0)
                    ShowErrorMessage("Укажите целое число.", "messageEditApproveCount");
                else
                    ShowErrorMessage("Неверно указано количество товара. Допускается не более " + scale + " десятичных знаков.", "messageEditApproveCount");
            }
            if (approvedPurchaseCostRecentError) {
                ShowErrorMessage("Указана неверная закупочная цена товара.", "messageEditApproveCount");
            }
            if (approvedSumRecentError) {
                ShowErrorMessage("Указана неверная сумма товара.", "messageEditApproveCount");
            }
        }

        if (!approvedCountError && !approvedPurchaseCostError && !approvedSumError) {
            var _thisGridRow = _this.parent("td").parent("tr");
            $.ajax({
                type: "POST",
                url: "/ReceiptWaybill/EditWaybillRowFromApprovement/",
                data: { waybillId: waybillId, rowId: rowId, approvedCount: approvedCount, purchaseCost: approvedPurchaseCost },
                success: function (result) {
                    _this.addClass("field-validation-success");

                    // Пересчитываем стоимость накладной
                    ReceiptWaybill_Approve.CalcRowsSum();
                    // Пересчитываем НДС в текущей строке
                    ReceiptWaybill_Approve.CalcValueAddedTaxForArticle(_thisGridRow);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    _this.addClass("field-validation-error");
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageEditApproveCount");
                }
            });
        }
    },

    // Проверяет, включены textedit-ы для закупочной цены и суммы в данной строке грида или отключены.
    IsSumAndPurchaseCostEnabled: function (gridRow) {
        return gridRow.find(".ApprovedSum[type=hidden]").length == 0;
    },

    // Включает textedit-ы для закупочной цены и суммы. Устанавливает значение в 0, если до этого они были отключены
    EnableSumAndPurchaseCost: function (gridRow) {
        if (!ReceiptWaybill_Approve.IsSumAndPurchaseCostEnabled(gridRow)) {
            gridRow.find(".ApprovedSum").parent("td").html('<input type="text" value="0" class="text_edit ApprovedSum" maxlength="19">');
            gridRow.find(".ApprovedPurchaseCost").parent("td").html('<input type="text" value="0" class="text_edit ApprovedPurchaseCost" maxlength="19">');
        }
    },

    // Отключает textedit-ы для закупочной цены и суммы. Устанавливает значение в 0, если до этого они были включены
    DisableSumAndPurchaseCost: function (gridRow) {
        if (ReceiptWaybill_Approve.IsSumAndPurchaseCostEnabled(gridRow)) {
            gridRow.find(".ApprovedSum").parent("td").html('<input type="hidden" value="0" class="ApprovedSum"><span class="ApprovedSum">0</span>');
            gridRow.find(".ApprovedPurchaseCost").parent("td").html('<input type="hidden" value="0" class="ApprovedPurchaseCost"><span class="ApprovedPurchaseCost">0</span>');
        }
    },

    // Пройти все строки таблицы и запретить редактировать закупочные цены и суммы там, где стоят нулевые количества
    DisableZeroCountRows: function () {
        $("#gridApproveArticles input.approveArticleCount").each(function () {
            if ($(this).val() == "0") {
                ReceiptWaybill_Approve.DisableSumAndPurchaseCost($(this).parent("td").parent("tr"));
            }
        });
    },

    // Получаем десятичное число из ячейки, которое может быть задано как в виде textedit с заданным классом, так и в виде label
    // TODO: перенести в common.js?
    TryGetCellDecimalValue: function (gridRow, className) {
        if (gridRow.find("input." + className).length > 0) {
            return TryGetDecimal(gridRow.find("input." + className).val());
        } else {
            return TryGetDecimal(gridRow.find("." + className).text().replaceAll(" ", ""));
        }
    },

    // Получаем число из ячейки, которое может быть задано как в виде textedit с заданным классом, так и в виде label
    // TODO: перенести в common.js?
    GetCellValue: function (gridRow, className) {
        if (gridRow.find("input." + className).length > 0) {
            return gridRow.find("input." + className).val();
        } else {
            return gridRow.find("." + className).text().replaceAll(" ", "");
        }
    },

    // Вычисляем НДС товара для указанной строки
    CalcValueAddedTaxForArticle: function (gridRow) {
        var sum = ReceiptWaybill_Approve.TryGetCellDecimalValue(gridRow, "ApprovedSum");
        var valueAddedTax = TryGetDecimal(gridRow.find(".ApprovedValueAddedTax option:selected").attr("param"));
        if (!isNaN(sum) && !isNaN(valueAddedTax)) {
            cost = ValueForDisplay(CalculateVatSum(sum, valueAddedTax), 2);
        }
        else {
            cost = 0;
        }

        var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();
        if (IsTrue(allowToViewPurchaseCosts)) {
            gridRow.find('.articleValueAddedTax').text(cost);
        }
    },

    SetGridFieldAttributes: function () {
        $("#gridApproveArticles input.approveArticleCount").each(function () {
            $(this).attr("maxlength", 19);
            $(this).parent("td").parent("tr").find('input[type="text"].ApprovedSum').attr("maxlength", 19);
            $(this).parent("td").parent("tr").find('input[type="text"].ApprovedPurchaseCost').attr("maxlength", 19);
        });
    },

    // Вычисление суммы накладной по ее позициям и пересчет сумм по позициям
    CalcRowsSum: function () {
        var rowsSum = 0;
        var totalSumValid = true;
        $("#gridApproveArticles .ApprovedPurchaseCost").each(function () {
            var purchaseCostValid = CheckValueScale(ReceiptWaybill_Approve.GetCellValue($(this).parent("td"), "ApprovedPurchaseCost"), 6, 12);
            var purchaseCost = ReceiptWaybill_Approve.TryGetCellDecimalValue($(this).parent("td"), "ApprovedPurchaseCost");
            if (!purchaseCostValid || purchaseCost < 0) {
                totalSumValid = false;

                return false;
            }

            var countValid, count;
            var measureUnitScale = TryGetDecimal($(this).parent("td").parent("tr").find(".measureUnitScale").text());

            // TODO: переделать под TryGetCellDecimalValue, предварительно сделав один класс, а не два approveArticleCount articleCount
            var isRowNotReadOnly = $(this).parent("td").parent("tr").find(".approveArticleCount").length > 0;
            if (isRowNotReadOnly) {
                countValid = CheckValueScale($(this).parent("td").parent("tr").find(".approveArticleCount").val(), measureUnitScale, 12);
                count = TryGetDecimal($(this).parent("td").parent("tr").find(".approveArticleCount").val(), measureUnitScale);
            } else {
                countValid = CheckValueScale($(this).parent("td").parent("tr").find(".articleCount").text().replaceAll(" ", ""), measureUnitScale, 12);
                count = TryGetDecimal($(this).parent("td").parent("tr").find(".articleCount").text().replaceAll(" ", ""), measureUnitScale);
            }
            if (!countValid || count < 0) {
                totalSumValid = false;

                return false;
            }

            if (count > 0) {
                rowsSum += TryGetDecimal(purchaseCost * count, 6);
                if (isRowNotReadOnly) {
                    $(this).parent("td").parent("tr").find(".ApprovedSum").val(BankRound(purchaseCost * count, 2));
                }
            }
        });
        if (totalSumValid)
            $("#ApprovedRowsSum").text(ValueForDisplay(rowsSum, 2));
        else
            $("#ApprovedRowsSum").text("---");
    },

    ValidateApprove: function () {
        var approveValidated = true;
        var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();

        $("#gridApproveArticles input.approveArticleCount").each(function () {
            var stringValid = true;

            var approveValueAddedTax = TryGetDecimal($(this).parent("td").parent("tr").find(".ApprovedValueAddedTax option:selected").attr("param"));
            if (isNaN(approveValueAddedTax))
                stringValid = false;

            // Проверка полей выполняется, только если их можно редактировать. Тогда там гарантированно textedit, и вызывать GetCellValue не нужно
            if ($(this).parent("td").parent("tr").find(".approveArticleCount").length > 0) {
                //Если нет прав на просмотр закупочных цен, то проверка не имеет смысл
                if (IsTrue(allowToViewPurchaseCosts)) {
                    var approveArticleSumValid = CheckValueScale($(this).parent("td").parent("tr").find(".ApprovedSum").val(), 2, 16);
                    var approveArticleSum = TryGetDecimal($(this).parent("td").parent("tr").find(".ApprovedSum").val());
                    if (!approveArticleSumValid || approveArticleSum < 0)
                        stringValid = false;

                    var approveArticlePurchaseCostValid = CheckValueScale($(this).parent("td").parent("tr").find(".ApprovedPurchaseCost").val(), 6, 12);
                    var approveArticlePurchaseCost = TryGetDecimal($(this).parent("td").parent("tr").find(".ApprovedPurchaseCost").val(), 6);
                    if (!approveArticlePurchaseCostValid || approveArticlePurchaseCost < 0)
                        stringValid = false;
                }

                var measureUnitScale = TryGetDecimal($(this).parent("td").parent("tr").find(".measureUnitScale").text());

                var approveArticleCountValid = CheckValueScale($(this).parent("td").parent("tr").find(".approveArticleCount").val(), measureUnitScale, 12);
                var approveArticleCount = TryGetDecimal($(this).parent("td").parent("tr").find(".approveArticleCount").val(), measureUnitScale);
                if (!approveArticleCountValid || approveArticleCount < 0)
                    stringValid = false;
            }

            if (!stringValid) {
                ShowErrorMessage("Суммы и количества должны быть числами с соответствующим количеством знаков после запятой, а ставка НДС выбрана.", "messageEditApproveCount");
                approveValidated = false;

                return false;
            }
        });

        //Если нет прав на просмотр закупочных цен, то проверка не имеет смысл
        if (IsTrue(allowToViewPurchaseCosts)) {
            var sumValid = CheckValueScale($("#TotalApprovedSum").val(), 2, 16);
            var sum = TryGetDecimal($("#TotalApprovedSum").val(), 2);
            if (!sumValid || sum < 0) {
                ShowErrorMessage("Итоговая сумма накладной должна быть числом в правильном формате.", "messageApproveWaybill");
                approveValidated = false;
            }
        }


        return approveValidated;
    }
}; 
