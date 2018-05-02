var ProviderType_List = {
    OnSuccessProviderTypeSave: function () {
        HideModal();
        RefreshGrid("gridProviderType", function () {
            ShowSuccessMessage("Сохранено.", "messageProviderTypeList");
        });
    }
};
