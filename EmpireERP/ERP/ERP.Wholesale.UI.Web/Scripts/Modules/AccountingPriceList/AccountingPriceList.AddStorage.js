var AccountingPriceList_AddStorage = {
    Init: function () {
        $(document).ready(function () {
            DisableButton("btnSaveStorage");

            $('#StorageId').change(function () {
                if ($('#StorageId').val() == "") {
                    DisableButton("btnSaveStorage");
                }
                else {
                    EnableButton("btnSaveStorage");
                }
            });
        });
    },

    OnSuccessStorageAdd: function (ajaxContext) {
        $("#StorageId").clearSelect();
        DisableButton("btnSaveStorage");

        RefreshGrid('gridAccountingPriceStorages');
        RefreshGrid("gridAccountingPriceArticles");
        AccountingPriceList_Shared.RefreshMainDetails(ajaxContext);
        var priceListId = $('#AccountingPriceListId').val();

        $.ajax({
            type: "POST",
            url: "/AccountingPriceList/GetListOfStorages/",
            data: { priceListId: priceListId },
            success: function (result) {
                $("#StorageId").fillSelect(result);
            }
        });

        ShowSuccessMessage("Место хранения добавлено.", 'messageAccountingPriceListStorageForm');
    },

    OnFailStorageAdd: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, 'messageAccountingPriceListStorageForm');
    }
};