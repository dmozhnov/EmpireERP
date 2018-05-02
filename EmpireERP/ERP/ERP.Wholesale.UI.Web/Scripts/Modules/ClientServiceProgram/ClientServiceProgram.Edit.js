var ClientServiceProgram_Edit = {
    OnBeginClientServiceProgramSave: function () {
        StartButtonProgress($("#btnSaveClientServiceProgram"));
    },

    OnFailClientServiceProgramSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageClientServiceProgramEdit");
    }
};
