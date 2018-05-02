var Trademark_List = {
    OnSuccessTrademarkSave: function () {
        HideModal();
        RefreshGrid("gridTrademark", function () {
            ShowSuccessMessage("Сохранено.", "messageTrademarkList");
        });
    }
};
