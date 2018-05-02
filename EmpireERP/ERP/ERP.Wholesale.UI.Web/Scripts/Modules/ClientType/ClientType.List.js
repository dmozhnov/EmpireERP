var ClientType_List = {
    OnSuccessClientTypeSave: function () {
        HideModal();
        RefreshGrid("gridClientType", function () {
            ShowSuccessMessage("Сохранено.", "messageClientTypeList");
        });
    }
};
