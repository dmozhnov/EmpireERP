var Country_List = {
    OnSuccessCountrySave: function () {
        HideModal();
        RefreshGrid("gridCountry", function () {
            ShowSuccessMessage("Сохранено.", "messageCountryList");
        });
    }
};
