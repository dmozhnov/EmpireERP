var DealPaymentDocument_DestinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution = {
    OnBeginClientOrganizationPaymentFromClientSave: function (ajaxContext) {
        StartButtonProgress($("#btnDistribute"));
    },

    OnFailClientOrganizationPaymentFromClientSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDestinationDocumentForClientOrganizationPaymentFromClientDistributionSelectList");
    }
};
