var CurrencyRate_Selector = {
    Init: function () {
        $(document).ready(function () {
            $("#currencyRateSelector .selectCurrentRate").click(function () {
                var currencyId = $("#CurrencyId").val();
                window[$("#SelectFunctionName").val()](currencyId, "", "", "", "");
            });
        });

        // Действия после выбора курса валюты из грида (ссылка "Выбрать")
        $("#currencyRateSelector .selectCurrencyRate").die("click");
        $("#currencyRateSelector .selectCurrencyRate").live("click", function () {
            var currencyId = $("#CurrencyId").val();
            var currencyRateId = $(this).parent("td").parent("tr").find(".Id").text();
            var currencyRate = $(this).parent("td").parent("tr").find(".Rate").text();
            var currencyRateForEdit = $(this).parent("td").parent("tr").find(".RateForEdit").text();
            var currencyRateStartDate = $(this).parent("td").parent("tr").find(".StartDate").text();
            window[$("#SelectFunctionName").val()](currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate);
        });

    }
};