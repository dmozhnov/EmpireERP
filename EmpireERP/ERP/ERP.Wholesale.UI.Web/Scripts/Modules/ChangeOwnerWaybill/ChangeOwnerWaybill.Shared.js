var ChangeOwnerWaybill_Shared = {
    ClearForm: function () {

        $("#ArticleName").text("Выберите товар");
        $("#ArticleId").val("");
        $("#changeOwnerWaybillRowForEdit #BatchName").text("не выбрана");
        $("#changeOwnerWaybillRowForEdit #PurchaseCost").text("---");
        $("#changeOwnerWaybillRowForEdit #AccountingPrice").text("---");
        $("#changeOwnerWaybillRowForEdit #AccountingPriceValue").val("");
        $("#changeOwnerWaybillRowForEdit #AvailableToReserveFromStorageCount").text("---");
        $("#changeOwnerWaybillRowForEdit #AvailableToReserveCount").text("---");
        $("#changeOwnerWaybillRowForEdit #AvailableToReserveFromPendingCount").text("---");
        $("#changeOwnerWaybillRowForEdit #ReceiptWaybillRowId").val("00000000-0000-0000-0000-000000000000");
        $("#changeOwnerWaybillRowForEdit #MeasureUnitName").text("");

        $("#changeOwnerWaybillRowForEdit #MovingCount").val("");
        $("#changeOwnerWaybillRowForEdit #MovingCount").removeClass("input-validation-error");
        $("#changeOwnerWaybillRowForEdit #MovingCount_validationMessage").hide();
        $("#changeOwnerWaybillRowForEdit #ManualSourcesInfo").val("");
        $("#changeOwnerWaybillRowForEdit #ManualSourcesLink").hide();

        SetFieldScale("#MovingCount", 12, 0, "#changeOwnerWaybillRowForEdit", true);
        ChangeOwnerWaybill_RowForEdit.UpdateValueAddedTaxSum();

        DisableButton("btnSaveChangeOwnerWaybillRow");
        $("#BatchLink").hide();
    },

    CheckSaveButtonAvailability: function () {
        var movingCount = TryGetDecimal($("#changeOwnerWaybillRowForEdit #MovingCount").val());
        var availableToReserveCount = TryGetDecimal($("#changeOwnerWaybillRowForEdit #AvailableToReserveCount").text().replaceAll(' ', ''));

        var manualSourcesInfo = $("#ManualSourcesInfo").val();
        if (((manualSourcesInfo != "" && manualSourcesInfo != undefined) || (!isNaN(movingCount) && !isNaN(availableToReserveCount) && (movingCount <= availableToReserveCount))) && (movingCount > 0)) {
            EnableButton("btnSaveChangeOwnerWaybillRow");
        }
        else {
            DisableButton("btnSaveChangeOwnerWaybillRow");
        }
    }
};