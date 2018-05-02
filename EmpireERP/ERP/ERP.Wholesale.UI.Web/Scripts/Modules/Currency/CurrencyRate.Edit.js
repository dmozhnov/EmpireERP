var CurrencyRate_Edit = {
    OnSuccessCurrencyRateEdit: function (ajaxContext) {
        RefreshGrid("gridCurrencyRate", function () {
            HideModal(function () {
                ShowSuccessMessage("Курс сохранен.", "messageCurrentRateGrid");
            });
        });
    },

    OnFailCurrencyRateEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageCurrencyRateEdit");
    }
};