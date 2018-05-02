var ChangeOwnerWaybill_RowForEdit = {
    Init: function () {
        $(document).ready(function () {
            SetFieldScale("#MovingCount", 12, $("#MeasureUnitScale").val(), "#changeOwnerWaybillRowForEdit", true);
        });

        $("span#ArticleName.select_link").bind('click', function () {
            var storageId = $('#StorageId').val();
            var senderId = $('#SenderId').val();

            $.ajax({
                type: "GET",
                url: "/Article/SelectArticleFromStorage/",
                data: { storageId: storageId, senderId: senderId },
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    ChangeOwnerWaybill_RowForEdit.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillRowEdit");
                }
            });
        });

        if ($("#changeOwnerWaybillRowForEdit #ManualSourcesInfo").val() != "") {
            $("#changeOwnerWaybillRowForEdit #MovingCount").disableInput();
        }

        $("#BatchLink.select_link").click(function () {
            var currentArticleBatchId = null;
            if ($("#changeOwnerWaybillRowForEdit #CurrentReceiptWaybillRowId").val() != "00000000-0000-0000-0000-000000000000") {
                currentArticleBatchId = $("#changeOwnerWaybillRowForEdit #CurrentReceiptWaybillRowId").val();
            }
            ChangeOwnerWaybill_RowForEdit.SelectArticleBatch(currentArticleBatchId);
        });

        $("#ManualSourcesLink.select_link").click(function () {
            ChangeOwnerWaybill_RowForEdit.SelectSourceWaybillRows();
        });

        $("#MovingCount").bind("keyup change paste cut", function () {
            ChangeOwnerWaybill_RowForEdit.UpdateValueAddedTaxSum();
            ChangeOwnerWaybill_Shared.CheckSaveButtonAvailability();
        });

        // При изменении ставки НДС
        $("#changeOwnerWaybillRowForEdit #ValueAddedTaxId").live("change", function () {
            ChangeOwnerWaybill_RowForEdit.UpdateValueAddedTaxSum();
        });
    },

    OnFailChangeOwnerWaybillRowEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageChangeOwnerWaybillRowEdit");
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live('click', function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            $("#ArticleId").val($(this).parent("td").parent("tr").find(".articleId").text());
            $("#MeasureUnitName").text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());

            if (IsTrue($("#ArticleSelector, #SelectSources").attr('checked'))) {
                HideModal(function () { ChangeOwnerWaybill_RowForEdit.SelectSourceWaybillRows(); });
            }
            else {
                HideModal(function () {                    
                    ChangeOwnerWaybill_RowForEdit.SelectArticleBatch();
                });
            }
        });
    },

    BindArticleBatchSelection: function () {
        $("#gridSelectArticleBatch .articleBatch_select_link").die("click");
        $("#gridSelectArticleBatch .articleBatch_select_link").live("click", function () {
            var availableToReserveCount = $(this).parent("td").parent("tr").find(".AvailableToReserveCount").text().replaceAll(' ', '');

            $("#changeOwnerWaybillRowForEdit #BatchName").text($(this).parent("td").parent("tr").find(".batchName").text());

            var purchaseCostCell = $(this).parent("td").parent("tr").find(".purchaseCost");

            $("#changeOwnerWaybillRowForEdit #PurchaseCost").text(purchaseCostCell.length ? purchaseCostCell.text() : "---");
            $("#changeOwnerWaybillRowForEdit #AvailableToReserveFromStorageCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromStorageCount").text());
            $("#changeOwnerWaybillRowForEdit #AvailableToReserveCount").text(availableToReserveCount);
            $("#changeOwnerWaybillRowForEdit #AvailableToReserveFromPendingCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromPendingCount").text());

            $("#changeOwnerWaybillRowForEdit #AccountingPrice").text($(this).closest("#articleBatchSelector").find("#RecipientAccountingPrice").text());
            $("#changeOwnerWaybillRowForEdit #AccountingPriceValue").val($(this).closest("#articleBatchSelector").find("#RecipientAccountingPriceValue").val());

            $("#changeOwnerWaybillRowForEdit #ReceiptWaybillRowId").val($(this).parent("td").parent("tr").find(".ReceiptWaybillRowId").text());

            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            SetFieldScale("#MovingCount", 12, measureUnitScale, "#changeOwnerWaybillRowForEdit", true);

            $("#changeOwnerWaybillRowForEdit #ReceiptWaybillRowId").val($(this).findCell(".ReceiptWaybillRowId").text());

            $("#changeOwnerWaybillRowForEdit #ManualSourcesInfo").val("");

            HideModal(function () {
                $("#BatchLink").show();
                $("#ManualSourcesLink").hide();

                $("#changeOwnerWaybillRowForEdit #MovingCount").enableInput().val("").focus().removeAttr("disabled");
                ChangeOwnerWaybill_RowForEdit.UpdateValueAddedTaxSum();
            });
        });
    },

    BindManualSourcesSelection: function () {
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").die("click");
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").live("click", function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "GET",
                url: "/ChangeOwnerWaybill/GetRowInfo/",
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
                    
                    $("#changeOwnerWaybillRowForEdit #MovingCount").disableInput().val(ValueForEdit(movingCount));
                    $("#BatchLink").hide();
                    $("#ManualSourcesLink").show();
                    $("#changeOwnerWaybillRowForEdit #ReceiptWaybillRowId").val($("#SelectedBatchId").val());
                    $("#changeOwnerWaybillRowForEdit #BatchName").text($("#SelectedBatchName").val());

                    ChangeOwnerWaybill_Shared.CheckSaveButtonAvailability();

                    $("#changeOwnerWaybillRowForEdit #PurchaseCost").text(result.PurchaseCost);
                    $("#changeOwnerWaybillRowForEdit #AvailableToReserveFromStorageCount").text(result.AvailableToReserveFromStorageCount);
                    $("#changeOwnerWaybillRowForEdit #AvailableToReserveCount").text(result.AvailableToReserveCount);
                    $("#changeOwnerWaybillRowForEdit #AvailableToReserveFromPendingCount").text(result.AvailableToReserveFromPendingCount);

                    $("#changeOwnerWaybillRowForEdit #AccountingPrice").text(result.AccountingPriceString);
                    $("#changeOwnerWaybillRowForEdit #AccountingPriceValue").val(result.AccountingPriceValue);

                    HideModal();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageOutgoingWaybillRow");
                }
            });
        });
    },

    SelectArticleBatch: function (articleBatchToExcludeId) {
        var storageId = $("#StorageId").val();
        var senderId = $('#SenderId').val();

        $.ajax({
            type: "GET",
            url: "/Article/SelectArticleBatch/",
            data: { articleId: $("#ArticleId").val(), senderStorageId: storageId, senderId: senderId,
                recipientStorageId: storageId, date: $("#ChangeOwnerWaybillDate").val(), articleBatchToExcludeId: articleBatchToExcludeId
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

                ChangeOwnerWaybill_RowForEdit.BindArticleBatchSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                if (IsDefaultOrEmpty($("#changeOwnerWaybillRowForEdit #CurrentReceiptWaybillRowId").val())) {
                    ChangeOwnerWaybill_Shared.ClearForm();
                }
                ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillRowEdit");
            }
        });
    },

    SelectSourceWaybillRows: function () {
        $.ajax({
            type: "GET",
            url: "/OutgoingWaybillRow/GetAvailableToReserveWaybillRows/",
            data: { type: "ChangeOwnerWaybill",
                articleId: $("#changeOwnerWaybillRowForEdit #ArticleId").val(),
                storageId: $("#changeOwnerWaybillRowForEdit #StorageId").val(),
                organizationId: $("#changeOwnerWaybillRowForEdit #SenderId").val(),
                selectedSourcesInfo: $("#ManualSourcesInfo").val(),
                waybillRowId: $("#changeOwnerWaybillRowForEdit #ChangeOwnerWaybillRowId").val()
            },
            success: function (result) {
                $('#sourceWaybillRowSelector').hide().html(result);

                var selectedBatchId = $("#changeOwnerWaybillRowForEdit #ReceiptWaybillRowId").val();

                if (selectedBatchId != "00000000-0000-0000-0000-000000000000" && $("#ManualSourcesInfo").val() != "") {
                    $("#SelectedBatchId").val($("#changeOwnerWaybillRowForEdit #ReceiptWaybillRowId").val());
                    $("#SelectedBatchName").val($("#changeOwnerWaybillRowForEdit #BatchName").text());
                }

                var selectedBatch = $("#SelectedBatchId").val();
                OutgoingWaybillRow_IncomingWaybillRowGrid.DisableRowsWithAnotherBatches(selectedBatch);

                $.validator.unobtrusive.parse($("#sourceWaybillRowSelector"));
                ShowModal("sourceWaybillRowSelector");
                                
                ChangeOwnerWaybill_RowForEdit.BindManualSourcesSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillRowEdit");
            }
        });
    },

    // Перерасчет суммы НДС по позиции
    UpdateValueAddedTaxSum: function () {
        var sum = TryGetDecimal($("#changeOwnerWaybillRowForEdit #AccountingPriceValue").val());
        var count = TryGetDecimal($("#changeOwnerWaybillRowForEdit #MovingCount").val());
        var vatPercent = TryGetDecimal($("#changeOwnerWaybillRowForEdit #ValueAddedTaxId option:selected").attr("param"));
        var vatSum = CalculateVatSum(sum * count, vatPercent);

        if (!isNaN(vatSum)) {
            $("#changeOwnerWaybillRowForEdit #ValueAddedTaxSum").text(ValueForDisplay(vatSum, 2));
        } else {
            $("#changeOwnerWaybillRowForEdit #ValueAddedTaxSum").text("0");
        }
    }

};