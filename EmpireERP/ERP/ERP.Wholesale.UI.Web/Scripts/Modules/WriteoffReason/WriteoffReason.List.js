var WriteoffReason_List = {
    OnSuccessWriteoffReasonSave: function () {
        HideModal();
        RefreshGrid("gridWriteoffReason", function () {
            ShowSuccessMessage("Сохранено.", "messageWriteoffReasonList");
        });
    }
};
