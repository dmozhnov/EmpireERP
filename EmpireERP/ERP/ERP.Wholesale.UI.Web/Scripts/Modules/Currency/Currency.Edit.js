var Currency_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#Name").focus();
            Currency_Edit.CheckEnabledCurrencyRateGrid();
        });
    },

    CheckEnabledCurrencyRateGrid: function () {
        if ($("#IsNew").val() == "True") {
            DisableButton("btnCreateCurrencyRate");
            DisableButton("btnImportRate");
            $("#currentRateGrid .page_size").attr("disabled", "disabled");
        }
        else {
            EnableButton("btnCreateCurrencyRate");
            EnableButton("btnImportRate");
            $("#currentRateGrid .page_size").removeAttr("disabled");
        }
    },

    OnSuccessCurrencyEdit: function (ajaxContext) {
        $("#gridCurrencyRate .parameters").val("CurrencyId=" + ajaxContext.Id);
        RefreshGrid("gridCurrency", function () {
            ShowSuccessMessage("Валюта сохранена.", "messageCurrencyEdit");
            $("#CurrencyId").val(ajaxContext.Id);
            $("#IsNew").val("False");
            Currency_Edit.CheckEnabledCurrencyRateGrid();
        });
    },

    OnFailCurrencyEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageCurrencyEdit");
    }
};