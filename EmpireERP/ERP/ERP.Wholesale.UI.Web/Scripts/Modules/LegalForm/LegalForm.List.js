var LegalForm_List = {
    OnSuccessLegalFormSave: function () {
        HideModal();
        RefreshGrid("gridLegalForm", function () {
            ShowSuccessMessage("Сохранено.", "messageLegalFormList");
        });
    }
};
