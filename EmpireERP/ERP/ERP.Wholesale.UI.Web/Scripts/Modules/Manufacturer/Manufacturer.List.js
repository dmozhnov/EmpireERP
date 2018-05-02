var Manufacturer_List = {
    OnSuccessManufacturerSave: function () {
        HideModal();
        RefreshGrid("gridManufacturer", function () {
            ShowSuccessMessage("Сохранено.", "messageManufacturerList");
        });
    }
};
