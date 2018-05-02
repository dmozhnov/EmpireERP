var DealPaymentDocument_DealCreditInitialBalanceCorrection_Edit = {
    Init: function () {
        $(document).ready(function () {
            DealPaymentDocument_DealInitialBalanceCorrection_Edit.Init("dealCreditInitialBalanceCorrectionEdit",
                "ForDealCreditInitialBalanceCorrection", "messageDealCreditInitialBalanceCorrectionEdit");
        });

        // Обработка выбора сделки
        $("#dealSelector .select_deal").live("click", function () {
            var dealId = $(this).findCell(".Id").text();
            var dealName = $(this).findCell(".Name").text();

            $("#dealCreditInitialBalanceCorrectionEdit #DealName").text(dealName);
            $("#dealCreditInitialBalanceCorrectionEdit #DealId").val(dealId);

            HideModal();
        });
    },

    OnBeginSelectDestinationDocumentsButtonClick: function () {
        StartButtonProgress($("#dealCreditInitialBalanceCorrectionEdit #btnSelectDestinationDocuments"));
    },

    OnFailSelectDestinationDocumentsButtonClick: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDealCreditInitialBalanceCorrectionEdit");
    },

    OnSuccessSelectDestinationDocumentsButtonClick: function (ajaxContext) {
        $("#destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution").html(ajaxContext);
        $.validator.unobtrusive.parse($("#destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution"));
        ShowModal("destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution");
    }
};