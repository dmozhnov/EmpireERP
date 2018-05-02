var WriteoffWaybill_Shared = {
    CheckSaveButtonAvailability: function () {
        var writingoffCount = TryGetDecimal($("#writeoffWaybillRowEdit #WritingoffCount").val());
        var availableToReserveCount = TryGetDecimal($("#writeoffWaybillRowEdit #AvailableToReserveCount").text().replaceAll(' ', ''));

        var manualSourcesInfo = $("#ManualSourcesInfo").val();
        if (((manualSourcesInfo != "" && manualSourcesInfo != undefined) || (!isNaN(writingoffCount) && !isNaN(availableToReserveCount) && (writingoffCount <= availableToReserveCount))) && (writingoffCount > 0)) {
            EnableButton("btnSaveWriteoffWaybillRow");
        }
        else {
            DisableButton("btnSaveWriteoffWaybillRow");
        }
    },

    ClearForm: function () {
        $("#ArticleName").text("Выберите товар");
        $("#ArticleId").val("");
        $("#writeoffWaybillRowEdit #BatchName").text("не выбрана");
        $("#writeoffWaybillRowEdit #PurchaseCost").text("---");
        $("#writeoffWaybillRowEdit #SenderAccountingPrice").text("---");

        $("#writeoffWaybillRowEdit #AvailableToReserveFromStorageCount").text("---");
        $("#writeoffWaybillRowEdit #AvailableToReserveCount").text("---");
        $("#writeoffWaybillRowEdit #AvailableToReserveFromPendingCount").text("---");
        $("#writeoffWaybillRowEdit #MarkupPercent").text("---");
        $("#writeoffWaybillRowEdit #MarkupSum").text("---");
        $("#writeoffWaybillRowEdit #ReceiptWaybillRowId").val("00000000-0000-0000-0000-000000000000");
        $("#writeoffWaybillRowEdit #MeasureUnitName").text("");

        $("#writeoffWaybillRowEdit #WritingoffCount").val("");
        $("#writeoffWaybillRowEdit #WritingoffCount").removeClass("input-validation-error");
        $("#writeoffWaybillRowEdit #WritingoffCount_validationMessage").hide();
        $("#writeoffWaybillRowEdit #ManualSourcesInfo").val("");
        $("#writeoffWaybillRowEdit #ManualSourcesLink").hide();

        SetFieldScale("#WritingoffCount", 12, 0, "#writeoffWaybillRowEdit", true);

        DisableButton("btnSaveWriteoffWaybillRow");
        $("#BatchLink").hide();
    }
};