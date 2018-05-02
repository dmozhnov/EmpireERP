var DealPaymentDocument_DestinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution = {
    Init: function () {
        $(document).ready(function () {
            $("#TeamId").change(function () {
                StartGridProgress($("#SaleWaybillSelectGridContainer").find(".grid"));
                StartGridProgress($("#DealDebitInitialBalanceCorrectionSelectGridContainer").find(".grid"));
                var teamId = $(this).val();

                $.ajax({
                    type: "GET",
                    url: "/DealInitialBalanceCorrection/ShowDestinationSaleGridForDealCreditInitialBalanceCorrectionDistribution/",
                    data: { dealId: $("#DealId").val(), teamId: teamId },
                    success: function (result1) {
                        $.ajax({
                            type: "GET",
                            url: "/DealInitialBalanceCorrection/ShowDestinationDocumentGridForDealCreditInitialBalanceCorrectionDistribution/",
                            data: { dealId: $("#DealId").val(), teamId: teamId },
                            success: function (result2) {
                                $("#SaleWaybillSelectGridContainer").html(result1);
                                $("#DealDebitInitialBalanceCorrectionSelectGridContainer").html(result2);
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageDestinationDocumentForDealCreditInitialBalanceCorrectionDistributionSelectList");
                            }
                        });

                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDestinationDocumentForDealCreditInitialBalanceCorrectionDistributionSelectList");
                    }
                });
            });
        });
    },

    OnBeginDealCreditInitialBalanceCorrectionSave: function (ajaxContext) {
        StartButtonProgress($("#btnDistribute"));
    },

    OnFailDealCreditInitialBalanceCorrectionSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDestinationDocumentForDealCreditInitialBalanceCorrectionDistributionSelectList");
    }
};
