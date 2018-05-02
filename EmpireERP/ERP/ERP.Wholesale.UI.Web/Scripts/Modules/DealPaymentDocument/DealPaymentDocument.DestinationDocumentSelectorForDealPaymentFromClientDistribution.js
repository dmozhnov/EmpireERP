var DealPaymentDocument_DestinationDocumentSelectorForDealPaymentFromClientDistribution = {
    Init: function () {
        $(document).ready(function () {
            // связывание списков команд и пользователей
            $('#TeamId').FillChildComboBox('TakenById', "/User/GetListByTeamForDealPayment", 'teamId', "messageDestinationDocumentForDealPaymentFromClientDistributionSelectList");

            $("#TeamId").change(function () {
                StartGridProgress($("#SaleWaybillSelectGrid").find(".grid"));
                StartGridProgress($("#DealDebitInitialBalanceCorrectionSelectGrid").find(".grid"));
                var teamId = $(this).val();

                $.ajax({
                    type: "GET",
                    url: "/DealPayment/ShowDestinationSaleGridForDealPaymentFromClientDistribution/",
                    data: { dealId: $("#DealId").val(), teamId: teamId },
                    success: function (result1) {
                        $.ajax({
                            type: "GET",
                            url: "/DealPayment/ShowDestinationPaymentDocumentGridForDealPaymentFromClientDistribution/",
                            data: { dealId: $("#DealId").val(), teamId: teamId },
                            success: function (result2) {
                                $("#DealDebitInitialBalanceCorrectionSelectGrid").html(result2);
                                $("#SaleWaybillSelectGrid").html(result1);
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageDestinationDocumentForDealPaymentFromClientDistributionSelectList");
                            }
                        });

                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDestinationDocumentForDealPaymentFromClientDistributionSelectList");
                    }
                });
            });
        });
    },

    OnFailDealPaymentFromClientSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDestinationDocumentForDealPaymentFromClientDistributionSelectList");
    },

    OnBeginDealPaymentFromClientSave: function (ajaxContext) {
        StartButtonProgress($("#btnDistribute"));
    }
};