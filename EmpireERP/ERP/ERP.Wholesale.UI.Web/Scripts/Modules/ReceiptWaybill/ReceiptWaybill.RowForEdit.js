var ReceiptWaybill_RowForEdit = {
    Init: function () {
        $(document).ready(function () {
            SetFieldScale("#PendingCount", 12, $("#MeasureUnitScale").val(), "#receiptWaybillRowForEdit", false);
            SetFieldScale("#PurchaseCost", 12, 6, "#receiptWaybillRowForEdit", false);
            SetFieldScale("#PendingSum", 16, 2, "#receiptWaybillRowForEdit", true);
        });

        var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();

        //биндим события для пересчета показателей, только если есть права на просмотр цен
        if (IsTrue(allowToViewPurchaseCosts)) {

            $("#form0 input[type=submit]").submit(function () {
                $("#PendingCount").trigger("change");
            });

            // При изменении ожидаемого кол-ва
            $("#receiptWaybillRowForEdit #PendingCount").bind("keyup change paste cut", function () {
                if ($("#PendingSumIsChangedLast").val() == 1) {
                    ReceiptWaybill_RowForEdit.UpdatePurchaseCost(true);
                }
                else {
                    ReceiptWaybill_RowForEdit.UpdatePendingSum();
                }
            });

            // При изменении ожидаемой суммы
            $("#receiptWaybillRowForEdit #PendingSum").bind("keyup paste cut", function () {
                $("#PendingSumIsChangedLast").val(1);
                ReceiptWaybill_RowForEdit.UpdatePurchaseCost();
            });

            // При изменении ожидаемой суммы с уходом с нее (можно изменить значение самого поля)
            $("#receiptWaybillRowForEdit #PendingSum").bind("change", function () {
                $("#PendingSumIsChangedLast").val(1);
                ReceiptWaybill_RowForEdit.UpdatePurchaseCost(true);
            });

            // При изменении закупочной цены
            $("#receiptWaybillRowForEdit #PurchaseCost").bind("keyup change paste cut", function () {
                $("#PendingSumIsChangedLast").val(0);
                ReceiptWaybill_RowForEdit.UpdatePendingSum();
            });

            // При изменении ставки НДС
            $("#receiptWaybillRowForEdit #PendingValueAddedTaxId").live("change", function () {
                ReceiptWaybill_RowForEdit.UpdateValueAddedTaxSum();
            });
        }


        // открытие формы выбора товара
        $("span#ArticleName").bind("click", function () {
            $.ajax({
                type: "GET",
                url: "/Article/SelectArticle",
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    ReceiptWaybill_RowForEdit.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
                }
            });
        });

        $("#ManufacturerName").click(function () {
            $.ajax({
                type: "GET",
                url: "/Manufacturer/SelectManufacturer/",
                success: function (result) {
                    $('#manufacturerAdd').hide().html(result);
                    $.validator.unobtrusive.parse($("#manufacturerAdd"));
                    ShowModal("manufacturerAdd");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
                }
            });
        });

        // Обрабатываем выбор фабрики-изготовителя
        $("#manufacturerAdd .select").live("click", function () {

            var manufacturerId = $(this).parent("td").parent("tr").find(".Id").html();
            var manufacturerName = $(this).parent("td").parent("tr").find(".ManufacturerName").html();

            $("#ManufacturerName").html(manufacturerName);
            $("#ManufacturerId").val(manufacturerId);

            HideModal();
        });

        $("#AddCountry").click(function () {
            $.ajax({
                type: "GET",
                url: "/Country/Create/",
                success: function (result) {
                    $('#countryAdd').hide().html(result);
                    $.validator.unobtrusive.parse($("#countryAdd"));
                    ShowModal("countryAdd");
                    $("#countryAdd #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
                }
            });
        });
    },

    // при неудачной попытке добавления/редактирования строки приходной накладной
    OnFailReceiptWaybillRowEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageReceiptWaybillRowEdit");
    },

    OnSuccessCountrySave: function (ajaxContext) {
        HideModal();

        $.ajax({
            type: "GET",
            url: "/Country/GetList/",
            success: function (result) {
                $("#ProductionCountryId").fillSelect(result);
                $("#ProductionCountryId").val(ajaxContext.Id);
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
            }
        });
    },

    OnSuccessManufacturerSave: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#receiptWaybillRowForEdit #ManufacturerId").val(ajaxContext.Id);
                $("#receiptWaybillRowForEdit #ManufacturerName").text(ajaxContext.Name);
            })
        });
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live("click", function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            var articleId = $(this).parent("td").parent("tr").find(".articleId").text();
            $("#ArticleId").val(articleId);
            $("#MeasureUnitName").text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());
            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            SetFieldScale("#PendingCount", 12, measureUnitScale, "#receiptWaybillRowForEdit", true);

            $.ajax({
                type: "POST",
                url: "/ReceiptWaybill/GetArticleInfo/",
                data: { articleId: articleId },
                success: function (result) {
                    $("#ProductionCountryId").attr("value", result.ProductionCountryId);
                    $("#ManufacturerId").attr("value", result.ManufacturerId);
                    $("#ManufacturerName").html(result.ManufacturerName);

                    if (result.ProductionCountryId == "") {
                        $("#receiptWaybillRowForEdit #ProductionCountryId").focus();
                    }
                    else {
                        if (result.ManufacturerId == "") {
                            $("#receiptWaybillRowForEdit #ProductionCountryId").focus();
                        }
                        else {
                            $("#receiptWaybillRowForEdit #PendingSum").focus();
                        }
                    }

                    var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();
                    if (IsTrue(allowToViewPurchaseCosts)) {
                        var waybillId = $('#Id').val();
                        $.ajax({
                            type: "GET",
                            url: "/ReceiptWaybill/GetLastPurchaseCost/",
                            data: { articleId: articleId, waybillId: waybillId },
                            success: function (result) {
                                $("#receiptWaybillRowForEdit #PurchaseCost").val(result);
                                // Запрос за ГТД
                                $.ajax({
                                    type: "GET",
                                    url: "/ReceiptWaybill/GetCustomsDeclarationNumberForRow/",
                                    data: { articleId: articleId, waybillId: waybillId },
                                    success: function (result) {
                                        $("#receiptWaybillRowForEdit #CustomsDeclarationNumber").val(result);
                                        HideModal();
                                    },
                                    error: function (XMLHttpRequest, textStatus, thrownError) {
                                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
                                    }
                                });
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
                                HideModal();
                            }
                        });
                    }
                    else {
                        HideModal();
                    }

                    $("#receiptWaybillRowForEdit #PendingCount").focus();
                    $("#PendingSumIsChangedLast").val("0");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
                }
            });
        });
    },

    // перерасчет суммы по товару
    UpdatePendingSum: function () {
        var cost = TryGetDecimal($("#receiptWaybillRowForEdit #PurchaseCost").val());
        var count = TryGetDecimal($("#receiptWaybillRowForEdit #PendingCount").val());

        if (!isNaN(cost) && !isNaN(count) && count != 0) {
            $("#receiptWaybillRowForEdit #PendingSum").val(ValueForEdit(cost * count, 2));
        }
        else {
            $("#receiptWaybillRowForEdit #PendingSum").val("0");
        }
        ReceiptWaybill_RowForEdit.UpdateValueAddedTaxSum();
    },

    // перерасчет закупочной цены и, возможно, повторный расчет ожидаемой суммы
    UpdatePurchaseCost: function (calculatePendingSum) {
        var sum = TryGetDecimal($("#receiptWaybillRowForEdit #PendingSum").val());
        var count = TryGetDecimal($("#receiptWaybillRowForEdit #PendingCount").val());

        if (!isNaN(sum) && !isNaN(count) && count != 0) {
            $("#receiptWaybillRowForEdit #PurchaseCost").val(BankRound(sum / count));

            // Рассчитывать ли ожидаемую сумму? Проверка одновременно на null и на undefined
            if (!(calculatePendingSum == null || IsFalse(calculatePendingSum))) {
                var purchaseCost = TryGetDecimal($("#receiptWaybillRowForEdit #PurchaseCost").val());
                $("#receiptWaybillRowForEdit #PendingSum").val(BankRound(purchaseCost * count, 2));
            }
        }
        else {
            $("#receiptWaybillRowForEdit #PurchaseCost").val("0");
        }

        ReceiptWaybill_RowForEdit.UpdateValueAddedTaxSum();
    },

    // Перерасчет суммы НДС по позиции
    UpdateValueAddedTaxSum: function () {
        var sum = TryGetDecimal($("#receiptWaybillRowForEdit #PendingSum").val());
        var vatPercent = TryGetDecimal($("#receiptWaybillRowForEdit #PendingValueAddedTaxId option:selected").attr("param"));
        var vatSum = CalculateVatSum(sum, vatPercent);

        if (!isNaN(vatSum)) {
            $("#receiptWaybillRowForEdit #ValueAddedTaxSum").text(ValueForDisplay(vatSum, 2));
        } else {
            $("#receiptWaybillRowForEdit #ValueAddedTaxSum").text("0");
        }
    }
};
