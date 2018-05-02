var ReturnFromClientWaybill_Shared = {
    CheckSaveButtonAvailability: function () {
        var returningCount = TryGetDecimal($("#returnFromClientWaybillRowEdit #ReturningCount").val());
        var availableToReturnCount = TryGetDecimal($("#returnFromClientWaybillRowEdit #AvailableToReturnCount").text().replaceAll(' ', ''));
        if (!isNaN(returningCount) && !isNaN(availableToReturnCount) && (returningCount <= availableToReturnCount) && (returningCount > 0)) {
            EnableButton("btnSaveReturnFromClientWaybillRow");
        }
        else {
            DisableButton("btnSaveReturnFromClientWaybillRow");
        }
    }
};