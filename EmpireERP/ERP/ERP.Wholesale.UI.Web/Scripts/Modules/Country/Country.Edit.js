var Country_Edit = {
    OnBeginCountrySave: function () {
        StartButtonProgress($("#btnSaveCountry"));
    },

    OnFailCountrySave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageCountryEdit");
    }
};
