var Manufacturer_Edit = {
    OnBeginManufacturerSave: function () {
        StartButtonProgress($("#btnSaveManufacturer"));
    },

    OnFailManufacturerSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageManufacturerEdit");
    }
};
