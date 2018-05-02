var AccountingPriceList_Details = {
    Init: function () {
        $(document).ready(function () {

            // Вызов окна параметров для печатных форм
            $('#lnkAccountingPriceListPrintingFormExpanded').click(function () {
                var accountingPriceListId = $('#MainDetails_Id').val();
                window.open("/AccountingPriceList/ShowAccountingPriceListPrintingForm?expMode=true&accountingPriceListId=" + accountingPriceListId);
            });

            $('#lnkAccountingPriceListPrintingForm').click(function () {
                var accountingPriceListId = $('#MainDetails_Id').val();
                window.open("/AccountingPriceList/ShowAccountingPriceListPrintingForm?expMode=false&accountingPriceListId=" + accountingPriceListId);
            });

            // Назад к списку
            $('#btnAccountingPriceListBack').click(function () {
                window.location = $('#BackURL').val();
            });

            // Редактировать
            $('#btnEditAccountingPriceList').click(function () {
                var priceListId = $('#MainDetails_Id').val();
                window.location = "/AccountingPriceList/Edit?accountingPriceListId=" + priceListId + GetBackUrl();
            });

            // Удалить
            $('#btnDeleteAccountingPriceList').click(function () {
                if (confirm('Вы уверены?')) {
                    var priceListId = $('#MainDetails_Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/AccountingPriceList/Delete/",
                        data: { id: priceListId },
                        success: function () {
                            window.location = "/AccountingPriceList/List";
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountingPriceListDetails");
                        }
                    });
                }
            });

            // Провести реестр цен
            $('#btnAccept').click(function () {
                var priceListId = $('#MainDetails_Id').val();
                var backURL = $('#BackURL').val();

                var rowCount = $('#priceListDetailsRowCount').text();
                if ((rowCount == "0") || (rowCount == "")) {
                    ShowErrorMessage("Невозможно провести реестр без товаров.", "messageAccountingPriceListDetails");

                    return;
                }

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/AccountingPriceList/Accept/",
                    data: { id: priceListId },
                    success: function (result) {
                        ShowSuccessMessage("Проведено. Ждите загрузки страницы.", "messageAccountingPriceListDetails");
                        window.location = "/AccountingPriceList/Details?id=" + priceListId + GetBackUrlFromString(backURL);
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountingPriceListDetails");
                    }
                });

            });

            // Отменить проводку реестра цен
            $('#btnCancelAcceptance').click(function () {
                var priceListId = $('#MainDetails_Id').val();
                var backURL = $('#BackURL').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/AccountingPriceList/CancelAcceptance/",
                    data: { id: priceListId },
                    success: function (result) {
                        ShowSuccessMessage("Проводка отменена. Ждите загрузки страницы.", "messageAccountingPriceListDetails");
                        window.location = "/AccountingPriceList/Details?id=" + priceListId + GetBackUrlFromString(backURL);
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountingPriceListDetails");
                    }
                });
            });

        });    // document ready
    }
};