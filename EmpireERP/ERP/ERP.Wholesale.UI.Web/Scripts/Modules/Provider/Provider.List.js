var Provider_List = {    
     OnSuccessProviderSave:function(ajaxContext) {
            RefreshGrid("gridProvider", function () {
                ShowSuccessMessage("Поставщик добавлен.", "messageProviderList");
            });
            HideModal();
        }
};