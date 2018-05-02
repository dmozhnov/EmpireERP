var ReturnFromClientReason_Edit = {
    OnBeginReturnFromClientReasonSave: function () {
        StartButtonProgress($("#btnSaveReturnFromClientReason"));
    },

    OnFailReturnFromClientReasonSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageReturnFromClientReasonEdit");
    }
};
