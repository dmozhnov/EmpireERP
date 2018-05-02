var EmployeePost_Edit = {
    OnBeginEmployeePostSave: function () {
        StartButtonProgress($("#btnSaveEmployeePost"));
    },

    OnFailEmployeePostSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageEmployeePostEdit");
    }
};
