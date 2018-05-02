var WriteoffWaybill_RowEdit = {
    Init: function () {
        $(document).ready(function () {
            SetFieldScale("#WritingoffCount", 12, $("#MeasureUnitScale").val(), "#writeoffWaybillRowEdit", true);
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

                    WriteoffWaybill_RowEdit.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillRowEdit");
                }
            });
        });

        if ($("#writeoffWaybillRowEdit #ManualSourcesInfo").val() != "") {
            $("#writeoffWaybillRowEdit #WritingoffCount").disableInput();
        }

        $("#BatchLink.select_link").click(function () {
            var currentArticleBatchId = null;
            if ($("#writeoffWaybillRowEdit #CurrentReceiptWaybillRowId").val() != "00000000-0000-0000-0000-000000000000") {
                currentArticleBatchId = $("#writeoffWaybillRowEdit #CurrentReceiptWaybillRowId").val();
            }
            WriteoffWaybill_RowEdit.SelectArticleBatch(currentArticleBatchId);
        });

        $("#ManualSourcesLink.select_link").click(function () {
            WriteoffWaybill_RowEdit.SelectSourceWaybillRows();
        })

        $("#WritingoffCount").bind("keyup change paste cut", function () {
            WriteoffWaybill_Shared.CheckSaveButtonAvailability();
        });

    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live('click', function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            $("#ArticleId").val($(this).parent("td").parent("tr").find(".articleId").text());
            $("#MeasureUnitName").text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());

            if (IsTrue($("#ArticleSelector, #SelectSources").attr('checked'))) {
                HideModal(function () { WriteoffWaybill_RowEdit.SelectSourceWaybillRows(); });
            }
            else {
                HideModal(function () {
                    WriteoffWaybill_RowEdit.SelectArticleBatch();
                });
            }
        });
    },

    BindArticleBatchSelection: function () {
        $("#gridSelectArticleBatch .articleBatch_select_link").die("click");
        $("#gridSelectArticleBatch .articleBatch_select_link").live("click", function () {
            var availableToReserveCount = $(this).parent("td").parent("tr").find(".AvailableToReserveCount").text().replaceAll(' ', '');
            var purchaseCost = $(this).parent("td").parent("tr").find(".purchaseCost").text();
            var senderAccountingPrice = $(this).closest("#articleBatchSelector").find("#SenderAccountingPrice").text();

            $("#writeoffWaybillRowEdit #BatchName").text($(this).parent("td").parent("tr").find(".batchName").text());
            $("#writeoffWaybillRowEdit #AvailableToReserveFromStorageCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromStorageCount").text());
            $("#writeoffWaybillRowEdit #AvailableToReserveCount").text(availableToReserveCount);
            $("#writeoffWaybillRowEdit #AvailableToReserveFromPendingCount").text($(this).parent("td").parent("tr").find(".AvailableToReserveFromPendingCount").text());

            $("#writeoffWaybillRowEdit #SenderAccountingPrice").text(senderAccountingPrice);
            $("#writeoffWaybillRowEdit #ReceiptWaybillRowId").val($(this).parent("td").parent("tr").find(".ReceiptWaybillRowId").text());

            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            SetFieldScale("#WritingoffCount", 12, measureUnitScale, "#writeoffWaybillRowEdit", true);

            var senderAccountingPriceValue = parseFloat(senderAccountingPrice.replaceAll(' ', ''));
            var purchaseCostValue = parseFloat(purchaseCost.replaceAll(' ', ''));

            if (IsTrue($("#AllowToViewPurchaseCost").val())) {
                $("#writeoffWaybillRowEdit #PurchaseCost").text(purchaseCost);
                $("#writeoffWaybillRowEdit #MarkupSum").text(ValueForDisplay(senderAccountingPriceValue - purchaseCostValue, 2));
                $("#writeoffWaybillRowEdit #MarkupPercent").text(purchaseCostValue != 0 ? ValueForDisplay(((senderAccountingPriceValue - purchaseCostValue) / purchaseCostValue) * 100, 2) : "---");
            }
            else {
                $("#writeoffWaybillRowEdit #PurchaseCost").text("---");
                $("#writeoffWaybillRowEdit #MarkupSum").text("---");
                $("#writeoffWaybillRowEdit #MarkupPercent").text("---");
            }

            $("#writeoffWaybillRowEdit #ReceiptWaybillRowId").val($(this).findCell(".ReceiptWaybillRowId").text());

            $("#writeoffWaybillRowEdit #ManualSourcesInfo").val("");

            HideModal(function () {
                $("#BatchLink").show();
                $("#ManualSourcesLink").hide();

                $("#writeoffWaybillRowEdit #WritingoffCount").enableInput().val("").focus().removeAttr("disabled");
            });
        });
    },

    BindManualSourcesSelection: function () {
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").die("click");
        $("#sourceWaybillRowSelector #btnSaveSourcesSelection").live("click", function () {
            StartButtonProgress($(this));
            
            $.ajax({
                type: "GET",
                url: "/WriteoffWaybill/GetRowInfo/",
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
                    
                    $("#writeoffWaybillRowEdit #WritingoffCount").disableInput().val(ValueForEdit(movingCount));
                    $("#BatchLink").hide();
                    $("#ManualSourcesLink").show();
                    $("#writeoffWaybillRowEdit #ReceiptWaybillRowId").val($("#SelectedBatchId").val());
                    $("#writeoffWaybillRowEdit #BatchName").text($("#SelectedBatchName").val());

                    WriteoffWaybill_Shared.CheckSaveButtonAvailability();

                    $("#writeoffWaybillRowEdit #PurchaseCost").text(result.PurchaseCost);
                    $("#writeoffWaybillRowEdit #AvailableToReserveFromStorageCount").text(result.AvailableToReserveFromStorageCount);
                    $("#writeoffWaybillRowEdit #AvailableToReserveCount").text(result.AvailableToReserveCount);
                    $("#writeoffWaybillRowEdit #AvailableToReserveFromPendingCount").text(result.AvailableToReserveFromPendingCount);
                    $("#writeoffWaybillRowEdit #MarkupPercent").text(result.MarkupPercent);
                    $("#writeoffWaybillRowEdit #MarkupSum").text(result.MarkupSum);
                    $("#writeoffWaybillRowEdit #SenderAccountingPrice").text(result.SenderAccountingPrice);


                    HideModal();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageOutgoingWaybillRow");
                }
            });
        });
    },


    OnFailWriteoffWaybillRowEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageWriteoffWaybillRowEdit");
    },

    SelectArticleBatch: function (articleBatchToExcludeId) {
        $.ajax({
            type: "GET",
            url: "/Article/SelectArticleBatchByStorage/",
            data: { articleId: $("#ArticleId").val(), storageId: $("#SenderStorageId").val(), senderId: $('#SenderId').val(),
                date: $("#WriteoffWaybillDate").val(), articleBatchToExcludeId: articleBatchToExcludeId
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

                WriteoffWaybill_RowEdit.BindArticleBatchSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                if ($("#writeoffWaybillRowEdit #CurrentReceiptWaybillRowId").val() == "00000000-0000-0000-0000-000000000000") {
                    WriteoffWaybill_Shared.ClearForm();
                }
                ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillRowEdit");
            }
        });
    },

    SelectSourceWaybillRows: function () {
        $.ajax({
            type: "GET",
            url: "/OutgoingWaybillRow/GetAvailableToReserveWaybillRows/",
            data: { type: "WriteoffWaybill",
                articleId: $("#writeoffWaybillRowEdit #ArticleId").val(),
                storageId: $("#writeoffWaybillRowEdit #SenderStorageId").val(),
                organizationId: $("#writeoffWaybillRowEdit #SenderId").val(),
                selectedSourcesInfo: $("#ManualSourcesInfo").val(),
                waybillRowId: $("#writeoffWaybillRowEdit #Id").val()
            },
            success: function (result) {
                $('#sourceWaybillRowSelector').hide().html(result);

                var selectedBatchId = $("#writeoffWaybillRowEdit #ReceiptWaybillRowId").val();

                if (selectedBatchId != "00000000-0000-0000-0000-000000000000" && $("#ManualSourcesInfo").val() != "") {
                    $("#SelectedBatchId").val($("#writeoffWaybillRowEdit #ReceiptWaybillRowId").val());
                    $("#SelectedBatchName").val($("#writeoffWaybillRowEdit #BatchName").text());
                }

                var selectedBatch = $("#SelectedBatchId").val();
                OutgoingWaybillRow_IncomingWaybillRowGrid.DisableRowsWithAnotherBatches(selectedBatch);

                $.validator.unobtrusive.parse($("#sourceWaybillRowSelector"));
                ShowModal("sourceWaybillRowSelector");

                WriteoffWaybill_RowEdit.BindManualSourcesSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillRowEdit");
            }
        });
    }
}; 