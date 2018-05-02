var ClientServiceProgram_List = {
    OnSuccessClientServiceProgramSave: function () {
        HideModal();
        RefreshGrid("gridClientServiceProgram", function () {
            ShowSuccessMessage("Сохранено.", "messageClientServiceProgramList");
        });
    }
};
