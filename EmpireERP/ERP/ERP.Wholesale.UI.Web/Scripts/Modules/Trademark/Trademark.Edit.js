var Trademark_Edit = {
    OnBeginTrademarkSave: function () {
        StartButtonProgress($("#btnSaveTrademark"));
    },

    OnFailTrademarkSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageTrademarkEdit");
    }
};
