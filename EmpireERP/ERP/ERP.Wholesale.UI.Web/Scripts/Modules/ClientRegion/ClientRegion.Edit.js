var ClientRegion_Edit = {
    OnBeginClientRegionSave: function () {
        StartButtonProgress($("#btnSaveClientRegion"));
    },

    OnFailClientRegionSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageClientRegionEdit");
    }
};
