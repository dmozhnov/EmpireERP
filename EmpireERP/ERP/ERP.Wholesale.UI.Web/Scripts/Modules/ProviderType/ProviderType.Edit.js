var ProviderType_Edit = {
    OnBeginProviderTypeSave: function () {
        StartButtonProgress($("#btnSaveProviderType"));
    },

    OnFailProviderTypeSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProviderTypeEdit");
    }
};
