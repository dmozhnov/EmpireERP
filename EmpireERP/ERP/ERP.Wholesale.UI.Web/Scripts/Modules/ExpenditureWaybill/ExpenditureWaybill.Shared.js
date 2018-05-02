var ExpenditureWaybill_Shared = {
    CheckSaveButtonAvailability: function () {
        var sellingCount = TryGetDecimal($("#expenditureWaybillRowEdit #SellingCount").val());
        var availableToReserveCount = TryGetDecimal($("#expenditureWaybillRowEdit #AvailableToReserveCount").text().replaceAll(' ', ''));

        var manualSourcesInfo = $("#ManualSourcesInfo").val();
        if (((manualSourcesInfo != "" && manualSourcesInfo != undefined) || (!isNaN(sellingCount) && !isNaN(availableToReserveCount) && (sellingCount <= availableToReserveCount))) && (sellingCount > 0)) {
            EnableButton("btnSaveExpenditureWaybillRow");
        }
        else {
            DisableButton("btnSaveExpenditureWaybillRow");
        }
    }
};