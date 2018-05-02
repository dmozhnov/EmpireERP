var ReturnFromClientReason_List = {
    OnSuccessReturnFromClientReasonSave: function () {
        HideModal();
        RefreshGrid("gridReturnFromClientReason", function () {
            ShowSuccessMessage("Сохранено.", "messageReturnFromClientReasonList");
        });
    }
};
