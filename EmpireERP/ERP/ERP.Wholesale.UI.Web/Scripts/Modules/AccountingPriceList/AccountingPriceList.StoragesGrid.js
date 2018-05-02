var AccountingPriceList_StoragesGrid = {
    Init: function () {
        $(document).ready(function () {
            $('#btnAddOne').click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/AccountingPriceList/StoragesList",
                    data: { priceListId: $('#MainDetails_Id').val() },
                    success: function (result) {
                        $('#storageSelectList').hide().html(result);
                        $.validator.unobtrusive.parse($("#storageSelectList"));
                        ShowModal("storageSelectList");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, 'messageAccountingPriceListDetailsStorageList');
                    }
                });
            });

            $('#btnAddAll').click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/AccountingPriceList/StoragesAddAll",
                    data: { priceListId: $('#MainDetails_Id').val() },
                    success: function (result) {
                        RefreshGrid("gridAccountingPriceStorages", function () {
                            AccountingPriceList_Shared.RefreshMainDetails(result);
                            ShowSuccessMessage('Места хранения добавлены.', 'messageAccountingPriceListDetailsStorageList');
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, 'messageAccountingPriceListDetailsStorageList');
                    }
                });
            });

            $('#btnAddTradePoint').click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/AccountingPriceList/StoragesAddTradePoint",
                    data: { priceListId: $('#MainDetails_Id').val() },
                    success: function (result) {
                        RefreshGrid("gridAccountingPriceStorages", function () {
                            AccountingPriceList_Shared.RefreshMainDetails(result);
                            ShowSuccessMessage('Торговые точки добавлены.', 'messageAccountingPriceListDetailsStorageList');
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, 'messageAccountingPriceListDetailsStorageList');
                    }
                });
            });

            $('#gridAccountingPriceStorages .delFromList_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var storage_id = $(this).parent("td").parent("tr").find(".storageId").text();
                    var priceListId = $('#MainDetails_Id').val();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/AccountingPriceList/DeleteStorage/",
                        data: { storageId: storage_id, accPriceListId: priceListId },
                        success: function (result) {
                            RefreshGrid("gridAccountingPriceStorages", function () {
                                AccountingPriceList_Shared.RefreshMainDetails(result);
                                RefreshGrid("gridAccountingPriceArticles");
                                ShowSuccessMessage('Место хранения удалено.', 'messageAccountingPriceListDetailsStorageList');
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, 'messageAccountingPriceListDetailsStorageList');
                        }
                    });
                }
            });
        });         // document ready
    }
};