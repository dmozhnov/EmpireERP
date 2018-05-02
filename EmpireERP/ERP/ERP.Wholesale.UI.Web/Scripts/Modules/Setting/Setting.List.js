var Setting_List = {
    Init: function () {
        $("#btnBack").live("click", function () {
            window.location = $("#BackURL").val();
        });
    },

    OnBeginSettingSave: function (ajaxContext) {
        StartButtonProgress($("#btnSaveSetting"));
    },

    OnSuccessSettingSave: function (ajaxContext) {
        ShowSuccessMessage("Сохранено.", "messageSettingList");
    },

    OnFailSettingSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageSettingList");
    },

    GetSettingUrl: function () {
        return "/Setting?" + GetBackUrl(true);
    }
};