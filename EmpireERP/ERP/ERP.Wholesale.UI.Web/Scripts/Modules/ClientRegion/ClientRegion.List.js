var ClientRegion_List = {
    OnSuccessClientRegionSave: function () {
        HideModal();
        RefreshGrid("gridClientRegion", function () {
            ShowSuccessMessage("Сохранено.", "messageClientRegionList");
        });
    }
};
