var WriteoffReason_Edit = {
    OnBeginWriteoffReasonSave: function () {
        StartButtonProgress($("#btnSaveWriteoffReason"));
    },

    OnFailWriteoffReasonSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageWriteoffReasonEdit");
    }
};
