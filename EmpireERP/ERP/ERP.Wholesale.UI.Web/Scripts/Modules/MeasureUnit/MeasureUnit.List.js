var MeasureUnit_List = {
    OnSuccessMeasureUnitSave: function () {
        HideModal();
        RefreshGrid("gridMeasureUnits", function () {
            ShowSuccessMessage("Единица измерения сохранена.", "messageMeasureUnitList");
        });
    }
};
