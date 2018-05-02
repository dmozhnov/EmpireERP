var MovementWaybill_RowEdit = {
    Init: function () {
        $(document).ready(function () {
            SetFieldScale("#MovingCount", 12, $("#MeasureUnitScale").val(), "#movementWaybillRowEdit", true);
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

                    MovementWaybill_RowEdit.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillRowEdit");
                }
            });
        });

        if ($("#movementWaybillRowEdit #ManualSourcesInfo").val() != "") {
            $("#movementWaybillRowEdit #MovingCount").disableInput();
        }

        $("#BatchLink.select_link").click(function () {
            var currentArticleBatchId = null;
            if ($("#movementWaybillRowEdit #CurrentReceiptWaybillRowId").val() != "00000000-0000-0000-0000-000000000000") {
                currentArticleBatchId = $("#movementWaybillRowEdit #CurrentReceiptWaybillRowId").val();
            }
            MovementWaybill_RowEdit.SelectArticleBatch(currentArticleBatchId);
        });

        $("#ManualSourcesLink.select_link").click(function () {
            MovementWaybill_RowEdit.SelectSourceWaybillRows();
        });

        $("#MovingCount").bind("keyup change paste cut", function () {
            MovementWaybill_RowEdit.UpdateValueAddedTaxSum();
            MovementWaybill_Shared.CheckSaveButtonAvailability();
        });

        // При изменении ставки НДС
        $("#movementWaybillRowEdit #ValueAddedTaxId").live("change", function () {
            MovementWaybill_RowEdit.UpdateValueAddedTaxSum();
        });
    },

    OnFailMovementWaybillRowEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageMovementWaybillRowEdit");
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live('click', function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            $("#ArticleId").val($(this).parent("td").parent("tr").find(".articleId").text());
            $("#MeasureUnitName").text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());

            if (IsTrue($("#ArticleSelector, #SelectSources").attr('checked'))) {
                HideModal(function () { MovementWaybill_RowEdit.SelectSourceWaybillRows(); });
            }
            else {
                HideModal(function () {
                    var currentArticleBatchId = null;
                    if ($("#movementWaybillRowEdit #CurrentReceiptWaybillRowId").val() != "00000000-0000-0000-0000-000000000000") {
                        currentArticleBatchId = $("#movementWaybillRowEdit #CurrentReceiptWaybillRowId").val();
                    }

                    MovementWaybill_RowEdit.SelectArticleBatch();
                });
            }

        });
    },

    BindArticleBatchSelection: function () {
        $("#gridSelectArticleBatch .articleBatch_select_link").die("click");
        $("#gridSelectArticleBatch .articleBatch_select_link").live("click", function () {
            var availableToReserveCount = $(this).parent("td").parent("tr").find(".AvailableToReserveCount").text().replaceAll(" ", "");

            $("#movementWaybillRowEdit #BatchName").text($(this).parent("td").parent("tr").find(".batchName").text());
            $("#movementWaybillRowEdit #AvailableToReserveFromStorageCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromStorageCount").text());
            $("#movementWaybillRowEdit #AvailableToReserveCount").text(ValueForDisplay(availableToReserveCount));
            $("#movementWaybillRowEdit #AvailableToReserveFromPendingCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromPendingCount").text());

            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            SetFieldScale("#MovingCount", 12, measureUnitScale, "#movementWaybillRowEdit", true);

            var purchaseCost = $(this).parent("td").parent("tr").find(".purchaseCost").text();

            if (IsTrue($("#AllowToViewPurchaseCost").val()) && purchaseCost != "---") {
                $("#movementWaybillRowEdit #PurchaseCost").text(purchaseCost);
            }
            else {
                $("#movementWaybillRowEdit #PurchaseCost").text("---");
            }
            purchaseCost = purchaseCost.replaceAll(" ", "");

            $("#movementWaybillRowEdit #SenderAccountingPrice").text($(this).closest("#articleBatchSelector").find("#SenderAccountingPrice").text());
            var senderAccountingPrice = $(this).closest("#articleBatchSelector").find("#SenderAccountingPriceValue").val();
            $("#movementWaybillRowEdit #SenderAccountingPriceValue").val(senderAccountingPrice);

            $("#movementWaybillRowEdit #RecipientAccountingPrice").text($(this).closest("#articleBatchSelector").find("#RecipientAccountingPrice").text());
            var recipientAccountingPrice = $(this).closest("#articleBatchSelector").find("#RecipientAccountingPriceValue").val();
            $("#movementWaybillRowEdit #RecipientAccountingPriceValue").val(recipientAccountingPrice);

            var movementMarkupPercent =
                (ValueForEdit(senderAccountingPrice) != "" && ValueForEdit(recipientAccountingPrice) != "" && ValueForEdit(senderAccountingPrice) != "0") ?
                (recipientAccountingPrice - senderAccountingPrice) / senderAccountingPrice * 100 : "---";

            $("#movementWaybillRowEdit #MovementMarkupPercent").text(
                ValueForEdit(movementMarkupPercent) != "" ? ValueForDisplay(movementMarkupPercent, 2) : movementMarkupPercent);

            var movementMarkupSumValue;

            if (recipientAccountingPrice == "" || senderAccountingPrice == "") {
                movementMarkupSumValue = "---"
            }
            else {
                movementMarkupSumValue = ValueForDisplay(recipientAccountingPrice - senderAccountingPrice, 2);
            }

            $("#movementWaybillRowEdit #MovementMarkupSum").text(movementMarkupSumValue);

            if (IsTrue($("#AllowToViewPurchaseCost").val()) && recipientAccountingPrice != "" && purchaseCost != "---") {
                var purchaseMarkupPercent =
                (ValueForEdit(purchaseCost) != "" && ValueForEdit(recipientAccountingPrice) != "" && ValueForEdit(purchaseCost) != "0") ?
                (recipientAccountingPrice - purchaseCost) / purchaseCost * 100 : "---";

                $("#movementWaybillRowEdit #PurchaseMarkupPercent").text(
                ValueForEdit(purchaseMarkupPercent) != "" ? ValueForDisplay(purchaseMarkupPercent, 2) : purchaseMarkupPercent);
                $("#movementWaybillRowEdit #PurchaseMarkupSum").text(ValueForDisplay(recipientAccountingPrice - purchaseCost, 2));
            }
            else {
                $("#movementWaybillRowEdit #PurchaseMarkupPercent").text("---");
                $("#movementWaybillRowEdit #PurchaseMarkupSum").text("---");
            }

            $("#movementWaybillRowEdit #ReceiptWaybillRowId").val($(this).parent("td").parent("tr").find(".ReceiptWaybillRowId").text());

            $("#movementWaybillRowEdit #ManualSourcesInfo").val("");

            HideModal(function () {
                $("#BatchLink").show();
                $("#ManualSourcesLink").hide();

                $("#movementWaybillRowEdit #MovingCount").enableInput().val("").focus().removeAttr("disabled");
                MovementWaybill_RowEdit.UpdateValueAddedTaxSum();
            });
        });
    },

    BindManualSourcesSelection: function () {
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").die("click");
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").live("click", function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "GET",
                url: "/MovementWaybill/GetRowInfo/",
                data: { waybillId: $("#Id").val(),
                    batchId: $("#sourceWaybillRowSelector #SelectedBatchId").val()
                },
                success: function (result) {
                    $("#ManualSourcesInfo").val($("#sourceWaybillRowSelector #SelectedSources").val());

                    var movingCount = 0;
                    var selectedSourcesInfo = $("#ManualSourcesInfo").val().split(";");
                    $.each(selectedSourcesInfo, function (i, val) {
                        var fields = val.split("_");
                        var count = TryGetDecimal(fields[1]);

                        if (!isNaN(count)) {
                            movingCount += count;
                        }
                    });

                    $("#movementWaybillRowEdit #MovingCount").disableInput().val(ValueForEdit(movingCount));
                    $("#BatchLink").hide();
                    $("#ManualSourcesLink").show();
                    $("#movementWaybillRowEdit #ReceiptWaybillRowId").val($("#SelectedBatchId").val());
                    $("#movementWaybillRowEdit #BatchName").text($("#SelectedBatchName").val());

                    MovementWaybill_Shared.CheckSaveButtonAvailability();

                    $("#movementWaybillRowEdit #PurchaseCost").text(result.PurchaseCost);
                    $("#movementWaybillRowEdit #MovementMarkupSum").text(result.MovementMarkupSum);
                    $("#movementWaybillRowEdit #MovementMarkupPercent").text(result.MovementMarkupPercent);
                    $("#movementWaybillRowEdit #PurchaseMarkupSum").text(result.PurchaseMarkupSum);
                    $("#movementWaybillRowEdit #PurchaseMarkupPercent").text(result.PurchaseMarkupPercent);
                    $("#movementWaybillRowEdit #SenderAccountingPrice").text(result.SenderAccountingPrice);
                    $("#movementWaybillRowEdit #RecipientAccountingPrice").text(result.RecipientAccountingPrice);
                    $("#movementWaybillRowEdit #AvailableToReserveFromStorageCount").text(result.AvailableToReserveFromStorageCount);
                    $("#movementWaybillRowEdit #AvailableToReserveCount").text(result.AvailableToReserveCount);
                    $("#movementWaybillRowEdit #AvailableToReserveFromPendingCount").text(result.AvailableToReserveFromPendingCount);

                    HideModal();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageOutgoingWaybillRow");
                }
            });
        });
    },


    SelectArticleBatch: function (articleBatchToExcludeId) {
        $.ajax({
            type: "GET",
            url: "/Article/SelectArticleBatch/",
            data: { articleId: $("#ArticleId").val(), senderStorageId: $("#SenderStorageId").val(),
                recipientStorageId: $("#RecipientStorageId").val(), senderId: $("#SenderId").val(),
                date: $("#MovementWaybillDate").val(), articleBatchToExcludeId: articleBatchToExcludeId
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

                MovementWaybill_RowEdit.BindArticleBatchSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                if (IsDefaultOrEmpty($("#movementWaybillRowEdit #CurrentReceiptWaybillRowId").val())) {
                    MovementWaybill_Shared.ClearForm();
                }
                ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillRowEdit");
            }
        });
    },

    SelectSourceWaybillRows: function () {
        $.ajax({
            type: "GET",
            url: "/OutgoingWaybillRow/GetAvailableToReserveWaybillRows/",
            data: { type: "MovementWaybill",
                articleId: $("#movementWaybillRowEdit #ArticleId").val(),
                storageId: $("#movementWaybillRowEdit #SenderStorageId").val(),
                organizationId: $("#movementWaybillRowEdit #SenderId").val(),
                selectedSourcesInfo: $("#ManualSourcesInfo").val(),
                waybillRowId: $("#movementWaybillRowEdit #Id").val()

            },
            success: function (result) {
                $('#sourceWaybillRowSelector').hide().html(result);

                var selectedBatchId = $("#movementWaybillRowEdit #ReceiptWaybillRowId").val();

                if (selectedBatchId != "00000000-0000-0000-0000-000000000000" && $("#ManualSourcesInfo").val() != "") {
                    $("#SelectedBatchId").val($("#movementWaybillRowEdit #ReceiptWaybillRowId").val());
                    $("#SelectedBatchName").val($("#movementWaybillRowEdit #BatchName").text());
                }

                var selectedBatch = $("#SelectedBatchId").val();
                OutgoingWaybillRow_IncomingWaybillRowGrid.DisableRowsWithAnotherBatches(selectedBatch);

                $.validator.unobtrusive.parse($("#sourceWaybillRowSelector"));
                ShowModal("sourceWaybillRowSelector");

                MovementWaybill_RowEdit.BindManualSourcesSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillRowEdit");
            }
        });
    },

    // Перерасчет суммы НДС по позиции
    UpdateValueAddedTaxSum: function () {
        var senderAccountingPrice = TryGetDecimal($("#movementWaybillRowEdit #SenderAccountingPriceValue").val());
        var recipientAccountingPrice = TryGetDecimal($("#movementWaybillRowEdit #RecipientAccountingPriceValue").val());

        var count = TryGetDecimal($("#movementWaybillRowEdit #MovingCount").val());
        var vatPercent = TryGetDecimal($("#movementWaybillRowEdit #ValueAddedTaxId option:selected").attr("param"));
        var senderVatSum = CalculateVatSum(senderAccountingPrice * count, vatPercent);
        var recipientVatSum = CalculateVatSum(recipientAccountingPrice * count, vatPercent);

        if (!isNaN(senderVatSum)) {
            $("#movementWaybillRowEdit #SenderValueAddedTaxSum").text(ValueForDisplay(senderVatSum, 2));
        }
        else {
            $("#movementWaybillRowEdit #SenderValueAddedTaxSum").text("---");
        }

        if (!isNaN(recipientVatSum)) {
            $("#movementWaybillRowEdit #RecipientValueAddedTaxSum").text(ValueForDisplay(recipientVatSum, 2));
        }
        else {
            $("#movementWaybillRowEdit #RecipientValueAddedTaxSum").text("---");
        }
    }

};