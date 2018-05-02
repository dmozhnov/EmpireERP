var ValueAddedTax_List = {
    OnSuccessValueAddedTaxSave: function () {
        HideModal();
        RefreshGrid("gridValueAddedTax", function () {
            ShowSuccessMessage("Сохранено.", "messageValueAddedTaxList");
        });
    }
};
