var ValueAddedTax_Edit = {
    OnBeginValueAddedTaxSave: function () {
        StartButtonProgress($("#btnSaveValueAddedTax"));
    },

    OnFailValueAddedTaxSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageValueAddedTaxEdit");
    }
};
