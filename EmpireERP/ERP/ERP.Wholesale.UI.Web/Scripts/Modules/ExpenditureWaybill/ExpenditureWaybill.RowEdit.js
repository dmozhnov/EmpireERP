var ExpenditureWaybill_RowEdit = {
    Init: function () {
        $(document).ready(function () {
            SetFieldScale("#SellingCount", 12, $("#MeasureUnitScale").val(), "#expenditureWaybillRowEdit", true);
        });

        $("span#ArticleName.select_link").bind('click', function () {
            var storageId = $('#SenderStorageId').val();
            var senderId = $('#SenderId').val();

            $.ajax({
                type: "GET",
                url: "/Article/SelectArticleFromStorage/",
                data: { storageId: storageId, senderId: senderId },
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    ExpenditureWaybill_RowEdit.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillRowEdit");
                }
            });
        });

        if ($("#expenditureWaybillRowEdit #ManualSourcesInfo").val() != "") {
            $("#expenditureWaybillRowEdit #SellingCount").disableInput();
        }

        $("#BatchLink.select_link").click(function () {
            var currentArticleBatchId = null;
            if ($("#expenditureWaybillRowEdit #CurrentReceiptWaybillRowId").val() != "00000000-0000-0000-0000-000000000000") {
                currentArticleBatchId = $("#expenditureWaybillRowEdit #CurrentReceiptWaybillRowId").val();
            }
            ExpenditureWaybill_RowEdit.SelectArticleBatch(currentArticleBatchId);
        });

        $("#ManualSourcesLink.select_link").click(function () {
            ExpenditureWaybill_RowEdit.SelectSourceWaybillRows();
        });

        $("#SellingCount").bind("keyup change paste cut", function () {
            ExpenditureWaybill_RowEdit.UpdateValueAddedTaxSum();
            ExpenditureWaybill_Shared.CheckSaveButtonAvailability();
        });

        // При изменении ставки НДС
        $("#expenditureWaybillRowEdit #ValueAddedTaxId").live("change", function () {
            ExpenditureWaybill_RowEdit.UpdateValueAddedTaxSum();
        });
    },

    OnFailExpenditureWaybillRowEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageExpenditureWaybillRowEdit");
    },

    OnSuccessExpenditureWaybillRowEdit: function (ajaxContext) {
        if ($('#expenditureWaybillRowEdit #Id').val() != "00000000-0000-0000-0000-000000000000") {
            // грид для формы добавления товаров списком
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridExpenditureWaybillRows", function () {
                    RefreshGrid("gridArticleGroups", function () {
                        ExpenditureWaybill_Details.RefreshMainDetails(ajaxContext.MainDetails);
                        ExpenditureWaybill_Details.RefreshPermissions(ajaxContext.Permissions);
                        HideModal(function () {
                            ShowSuccessMessage("Сохранено.", "messageExpenditureWaybillRowList");
                        });
                    });
                });
            });
        }
        else {
            // грид для формы добавления товаров списком
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridExpenditureWaybillRows", function () {
                    RefreshGrid("gridArticleGroups", function () {
                        ExpenditureWaybill_RowEdit.ClearForm();
                        ExpenditureWaybill_Details.RefreshMainDetails(ajaxContext.MainDetails);
                        ExpenditureWaybill_Details.RefreshPermissions(ajaxContext.Permissions);
                        ShowSuccessMessage("Сохранено.", "messageExpenditureWaybillRowEdit");
                    });
                });
            });
        }
    },

    ClearForm: function () {
        $("#ArticleName").text("Выберите товар");
        $("#ArticleId").val("");
        $("#expenditureWaybillRowEdit #MeasureUnitName").text("");
        $("#expenditureWaybillRowEdit #BatchName").text("не выбрана");
        $("#expenditureWaybillRowEdit #PurchaseCost").text("---");
        $("#expenditureWaybillRowEdit #SenderAccountingPrice").text("---");

        $("#expenditureWaybillRowEdit #AvailableToReserveFromStorageCount").text("---");
        $("#expenditureWaybillRowEdit #AvailableToReserveCount").text("---");
        $("#expenditureWaybillRowEdit #AvailableToReserveFromPendingCount").text("---");
        $("#expenditureWaybillRowEdit #MarkupPercent").text("---");
        $("#expenditureWaybillRowEdit #MarkupSum").text("---");
        $("#expenditureWaybillRowEdit #SalePrice").text("---");
        $("#expenditureWaybillRowEdit #SalePriceValue").val("");
        $("#expenditureWaybillRowEdit #ReceiptWaybillRowId").val("00000000-0000-0000-0000-000000000000");

        $("#expenditureWaybillRowEdit #SellingCount").val("");
        $("#expenditureWaybillRowEdit #SellingCount").removeClass("input-validation-error");
        $("#expenditureWaybillRowEdit #SellingCount_validationMessage").hide();
        $("#expenditureWaybillRowEdit #ManualSourcesInfo").val("");
        $("#expenditureWaybillRowEdit #ManualSourcesLink").hide();
        
        SetFieldScale("#SellingCount", 12, 0, "#expenditureWaybillRowEdit", true);
        ExpenditureWaybill_RowEdit.UpdateValueAddedTaxSum();

        DisableButton("btnSaveExpenditureWaybillRow");
        $("#BatchLink").hide();
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live('click', function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            $("#ArticleId").val($(this).parent("td").parent("tr").find(".articleId").text());
            $("#MeasureUnitName").text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());

            if (IsTrue($("#ArticleSelector, #SelectSources").attr('checked'))) {
                HideModal(function () { ExpenditureWaybill_RowEdit.SelectSourceWaybillRows(); });
            }
            else {
                HideModal(function () {
                    ExpenditureWaybill_RowEdit.SelectArticleBatch();
                });
            }
        });
    },

    BindArticleBatchSelection: function () {
        $("#gridSelectArticleBatch .articleBatch_select_link").die("click");
        $("#gridSelectArticleBatch .articleBatch_select_link").live("click", function () {
            var availableToReserveCount = $(this).parent("td").parent("tr").find(".AvailableToReserveCount").text();
            var purchaseCost = $(this).parent("td").parent("tr").find(".purchaseCost").text();
            var senderAccountingPrice = $(this).closest("#articleBatchSelector").find("#SenderAccountingPrice").text();

            $("#expenditureWaybillRowEdit #BatchName").text($(this).parent("td").parent("tr").find(".batchName").text());
            $("#expenditureWaybillRowEdit #AvailableToReserveFromStorageCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromStorageCount").text());
            $("#expenditureWaybillRowEdit #AvailableToReserveCount").text(availableToReserveCount);
            $("#expenditureWaybillRowEdit #AvailableToReserveFromPendingCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromPendingCount").text());

            $("#expenditureWaybillRowEdit #SenderAccountingPrice").text(senderAccountingPrice);
            $("#expenditureWaybillRowEdit #ReceiptWaybillRowId").val($(this).parent("td").parent("tr").find(".ReceiptWaybillRowId").text());

            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            SetFieldScale("#SellingCount", 12, measureUnitScale, "#expenditureWaybillRowEdit", true);

            var senderAccountingPriceValue = parseFloat(senderAccountingPrice.replaceAll(' ', ''));
            var purchaseCostValue = parseFloat(purchaseCost.replaceAll(' ', ''));
            var dealQuotaDiscountPercent = parseFloat($("#expenditureWaybillRowEdit #DealQuotaDiscountPercent").text().replaceAll(' ', ''));

            var digitsNumber;
            if (IsTrue($("#RoundSalePrice").val())) {
                digitsNumber = 0;
            }
            else {
                digitsNumber = 2;
            }

            var salePrice = ValueForEdit(senderAccountingPriceValue - (senderAccountingPriceValue * dealQuotaDiscountPercent / 100), digitsNumber);
            $("#expenditureWaybillRowEdit #SalePrice").text(ValueForDisplay(salePrice));
            $("#expenditureWaybillRowEdit #SalePriceValue").val(ValueForEdit(salePrice));

            if (IsTrue($("#AllowToViewPurchaseCost").val())) {
                $("#expenditureWaybillRowEdit #PurchaseCost").text(purchaseCost);
                $("#expenditureWaybillRowEdit #MarkupSum").text(ValueForDisplay(salePrice - purchaseCostValue, 2));
                $("#expenditureWaybillRowEdit #MarkupPercent").text(purchaseCostValue != 0 ? ValueForDisplay(((salePrice - purchaseCostValue) / purchaseCostValue) * 100, 2) : "---");
            }
            else {
                $("#expenditureWaybillRowEdit #PurchaseCost").text("---");
                $("#expenditureWaybillRowEdit #MarkupSum").text("---");
                $("#expenditureWaybillRowEdit #MarkupPercent").text("---");
            }

            $("#expenditureWaybillRowEdit #ReceiptWaybillRowId").val($(this).findCell(".ReceiptWaybillRowId").text());

            $("#expenditureWaybillRowEdit #ManualSourcesInfo").val("");

            HideModal(function () {
                $("#BatchLink").show();
                $("#ManualSourcesLink").hide();

                $("#expenditureWaybillRowEdit #SellingCount").enableInput().val("").focus().removeAttr("disabled");
                ExpenditureWaybill_RowEdit.UpdateValueAddedTaxSum();
            });
        });
    },

    SelectArticleBatch: function (articleBatchToExcludeId) {
        $.ajax({
            type: "GET",
            url: "/Article/SelectArticleBatchByStorage/",
            data: { articleId: $("#ArticleId").val(), storageId: $("#SenderStorageId").val(), senderId: $("#SenderId").val(),
                date: $("#ExpenditureWaybillDate").val(), articleBatchToExcludeId: articleBatchToExcludeId
            },
            success: function (result) {
                $('#articleBatchSelector').hide().html(result);
                $.validator.unobtrusive.parse($("#articleBatchSelector"));
                ShowModal("articleBatchSelector");

                var availableToReserveCount = 0;

                $("#articleBatchSelector .AvailableToReserveCount").each(function () {
                    availableToReserveCount += parseFloat($(this).text().replaceAll(' ', ''));
                });

                $("#articleBatchSelector #AvailableToMoveTotalCount").text(availableToReserveCount);

                ExpenditureWaybill_RowEdit.BindArticleBatchSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                if ($("#expenditureWaybillRowEdit #CurrentReceiptWaybillRowId").val() == "00000000-0000-0000-0000-000000000000") {
                    ExpenditureWaybill_RowEdit.ClearForm();
                }
                ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillRowEdit");
            }
        });
    },

    BindManualSourcesSelection: function () {
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").die("click");
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").live("click", function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/GetRowInfo/",
                data: { waybillId: $("#Id").val(),
                    batchId: $("#sourceWaybillRowSelector #SelectedBatchId").val()
                },
                success: function (result) {
                    $("#ManualSourcesInfo").val($("#sourceWaybillRowSelector #SelectedSources").val());

                    var sellingCount = 0;
                    var selectedSourcesInfo = $("#ManualSourcesInfo").val().split(";");
                    $.each(selectedSourcesInfo, function (i, val) {
                        var fields = val.split("_");
                        var count = TryGetDecimal(fields[1]);

                        if (!isNaN(count)) {
                            sellingCount += count;
                        }
                    });

                    $("#expenditureWaybillRowEdit #SellingCount").disableInput().val(ValueForEdit(sellingCount));
                    $("#BatchLink").hide();
                    $("#ManualSourcesLink").show();
                    $("#expenditureWaybillRowEdit #ReceiptWaybillRowId").val($("#SelectedBatchId").val());
                    $("#expenditureWaybillRowEdit #BatchName").text($("#SelectedBatchName").val());

                    ExpenditureWaybill_Shared.CheckSaveButtonAvailability();

                    $("#expenditureWaybillRowEdit #PurchaseCost").text(result.PurchaseCost);
                    $("#expenditureWaybillRowEdit #AvailableToReserveFromStorageCount").text(result.AvailableToReserveFromStorageCount);
                    $("#expenditureWaybillRowEdit #AvailableToReserveCount").text(result.AvailableToReserveCount);
                    $("#expenditureWaybillRowEdit #AvailableToReserveFromPendingCount").text(result.AvailableToReserveFromPendingCount);
                    $("#expenditureWaybillRowEdit #MarkupPercent").text(result.MarkupPercent);
                    $("#expenditureWaybillRowEdit #MarkupSum").text(result.MarkupSum);
                    $("#expenditureWaybillRowEdit #SenderAccountingPrice").text(result.SenderAccountingPrice);

                    $("#expenditureWaybillRowEdit #SalePrice").text(result.SalePrice);
                    $("#expenditureWaybillRowEdit #SalePriceValue").val(result.SalePriceValue);

                    HideModal();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageOutgoingWaybillRow");
                }
            });
        });
    },

    SelectSourceWaybillRows: function () {
        $.ajax({
            type: "GET",
            url: "/OutgoingWaybillRow/GetAvailableToReserveWaybillRows/",
            data: { type: "ExpenditureWaybill",
                articleId: $("#expenditureWaybillRowEdit #ArticleId").val(),
                storageId: $("#expenditureWaybillRowEdit #SenderStorageId").val(),
                organizationId: $("#expenditureWaybillRowEdit #SenderId").val(),
                selectedSourcesInfo: $("#ManualSourcesInfo").val(),
                waybillRowId: $("#expenditureWaybillRowEdit #Id").val()
            },
            success: function (result) {
                $('#sourceWaybillRowSelector').hide().html(result);

                var selectedBatchId = $("#expenditureWaybillRowEdit #ReceiptWaybillRowId").val();

                if (selectedBatchId != "00000000-0000-0000-0000-000000000000" && $("#ManualSourcesInfo").val() != "") {
                    $("#SelectedBatchId").val($("#expenditureWaybillRowEdit #ReceiptWaybillRowId").val());
                    $("#SelectedBatchName").val($("#expenditureWaybillRowEdit #BatchName").text());
                }

                var selectedBatch = $("#SelectedBatchId").val();
                OutgoingWaybillRow_IncomingWaybillRowGrid.DisableRowsWithAnotherBatches(selectedBatch);

                $.validator.unobtrusive.parse($("#sourceWaybillRowSelector"));
                ShowModal("sourceWaybillRowSelector");

                ExpenditureWaybill_RowEdit.BindManualSourcesSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillRowEdit");
            }
        });
    },

    // Перерасчет суммы НДС по позиции
    UpdateValueAddedTaxSum: function () {
        var sum = TryGetDecimal($("#expenditureWaybillRowEdit #SalePriceValue").val());
        var count = TryGetDecimal($("#expenditureWaybillRowEdit #SellingCount").val());
        var vatPercent = TryGetDecimal($("#expenditureWaybillRowEdit #ValueAddedTaxId option:selected").attr("param"));
        var vatSum = CalculateVatSum(sum * count, vatPercent);

        if (!isNaN(vatSum)) {
            $("#expenditureWaybillRowEdit #ValueAddedTaxSum").text(ValueForDisplay(vatSum, 2));
        } else {
            $("#expenditureWaybillRowEdit #ValueAddedTaxSum").text("0");
        }
    }

};
