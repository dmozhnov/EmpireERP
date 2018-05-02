var CurrencyRate_Grid = {
    Init: function () {
        $("#btnCreateCurrencyRate").click(function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "GET",
                url: "/Currency/CreateRate",
                data: { currencyId: $("#CurrencyId").val() },
                success: function (result) {
                    $('#currencyRateEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#currencyRateEdit"));
                    ShowModal("currencyRateEdit");
                    $('#currencyRateEdit #Rate').focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrentRateGrid");
                }
            });
        });

        $("#btnImportRate").click(function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "GET",
                url: "/Currency/ImportCurrencyRate",
                data: { currencyId: $("#CurrencyId").val() },
                success: function (result) {
                    $('#currencyRateEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#currencyRateEdit"));
                    ShowModal("currencyRateEdit");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrentRateGrid");
                }
            });
        });

        // Редактирование курса валюты
        $("#gridCurrencyRate .edit").click(function () {
            var rateId = $(this).parent("td").parent("tr").find(".CurrencyRateId").text();

            $.ajax({
                type: "GET",
                url: "/Currency/EditRate",
                data: { currencyRateId: rateId },
                success: function (result) {
                    $('#currencyRateEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#currencyRateEdit"));
                    ShowModal("currencyRateEdit");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrentRateGrid");
                }
            });
        });

        // Удаление курса валюты
        $("#gridCurrencyRate .delete").click(function () {
            if (confirm("Вы уверены?")) {
                var rateId = $(this).parent("td").parent("tr").find(".CurrencyRateId").text();

                $.ajax({
                    type: "GET",
                    url: "/Currency/DeleteRate",
                    data: { currencyRateId: rateId },
                    success: function (result) {
                        RefreshGrid("gridCurrencyRate", function () {
                            ShowSuccessMessage("Курс валюты удален.", "messageCurrentRateGrid");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrentRateGrid");
                    }
                });
            }
        });
    }
};