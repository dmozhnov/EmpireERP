var LegalForm_Edit = {
    OnBeginLegalFormSave: function () {
        StartButtonProgress($("#btnSaveLegalForm"));
    },

    OnFailLegalFormSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageLegalFormEdit");
    }
};
