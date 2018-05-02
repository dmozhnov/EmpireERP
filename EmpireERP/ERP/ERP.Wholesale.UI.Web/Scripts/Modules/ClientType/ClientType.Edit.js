var ClientType_Edit = {
    OnBeginClientTypeSave: function () {
        StartButtonProgress($("#btnSaveClientType"));
    },

    OnFailClientTypeSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageClientTypeEdit");
    }
};
