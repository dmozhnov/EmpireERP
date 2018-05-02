var MovementWaybill_Shared = {
    ClearForm: function () {
        $("#ArticleName").text("Выберите товар");
        $("#ArticleId").val("");
        $("#movementWaybillRowEdit #BatchName").text("не выбрана");
        $("#movementWaybillRowEdit #PurchaseCost").text("---");
        $("#movementWaybillRowEdit #SenderAccountingPrice").text("---");
        $("#movementWaybillRowEdit #SenderAccountingPriceValue").val("");
        $("#movementWaybillRowEdit #RecipientAccountingPrice").text("---");
        $("#movementWaybillRowEdit #RecipientAccountingPriceValue").val("");
        $("#movementWaybillRowEdit #AvailableToReserveFromStorageCount").text("---");
        $("#movementWaybillRowEdit #AvailableToReserveCount").text("---");
        $("#movementWaybillRowEdit #AvailableToReserveFromPendingCount").text("---");
        $("#movementWaybillRowEdit #ReceiptWaybillRowId").val("00000000-0000-0000-0000-000000000000");
        $("#movementWaybillRowEdit #MovementMarkupPercent").text("---");
        $("#movementWaybillRowEdit #MovementMarkupSum").text("---");
        $("#movementWaybillRowEdit #PurchaseMarkupPercent").text("---");
        $("#movementWaybillRowEdit #PurchaseMarkupSum").text("---");
        $("#movementWaybillRowEdit #MeasureUnitName").text("");

        $("#movementWaybillRowEdit #MovingCount").val("");
        $("#movementWaybillRowEdit #MovingCount").removeClass("input-validation-error");
        $("#movementWaybillRowEdit #MovingCount_validationMessage").hide();
        $("#movementWaybillRowEdit #ManualSourcesInfo").val("");
        $("#movementWaybillRowEdit #ManualSourcesLink").hide();

        SetFieldScale("#MovingCount", 12, 0, "#movementWaybillRowEdit", true);
        MovementWaybill_RowEdit.UpdateValueAddedTaxSum();

        DisableButton("btnSaveMovementWaybillRow");
        $("#BatchLink").hide();
    },

    CheckSaveButtonAvailability: function () {
        var movingCount = TryGetDecimal($("#movementWaybillRowEdit #MovingCount").val());
        var availableToReserveCount = TryGetDecimal($("#movementWaybillRowEdit #AvailableToReserveCount").text().replaceAll(' ', ''));
        var manualSourcesInfo = $("#ManualSourcesInfo").val();
        if (((manualSourcesInfo != "" && manualSourcesInfo != undefined) || (!isNaN(movingCount) && !isNaN(availableToReserveCount) && (movingCount <= availableToReserveCount))) && (movingCount > 0)) {
            EnableButton("btnSaveMovementWaybillRow");
        }
        else {
            DisableButton("btnSaveMovementWaybillRow");
        }
    }
};