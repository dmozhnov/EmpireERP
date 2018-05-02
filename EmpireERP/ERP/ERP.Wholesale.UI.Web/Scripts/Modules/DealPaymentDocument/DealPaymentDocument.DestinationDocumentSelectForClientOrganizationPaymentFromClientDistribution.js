var DealPaymentDocument_DestinationDocumentSelectForClientOrganizationPaymentFromClientDistribution = {
    Init: function(){
        // связывание списков команд и пользователей
        $('#TeamId').FillChildComboBox('TakenById', "/User/GetListByTeamForDealPayment", 'teamId', "messageDestinationDocumentForClientOrganizationPaymentFromClientDistributionSelectList");
        
        $("#TeamId").change(function () {
                StartGridProgress($("#SaleWaybillSelectGridContainer").find(".grid"));
                StartGridProgress($("#DealDebitInitialBalanceCorrectionSelectGridContainer").find(".grid"));
                var teamId = $(this).val();
                var clientOrganizationId = $("#ClientOrganizationId").val();

                $.ajax({
                    type: "GET",
                    url: "/DealPayment/ShowDestinationSaleGridForClientOrganizationPaymentFromClientDistribution/",
                    data: { clientOrganizationId: clientOrganizationId, teamId: teamId },
                    success: function (result1) {
                        $.ajax({
                            type: "GET",
                            url: "/DealPayment/ShowDestinationPaymentDocumentGridForClientOrganizationPaymentFromClientDistribution/",
                            data: { clientOrganizationId: clientOrganizationId, teamId: teamId },
                            success: function (result2) {
                                $("#DealDebitInitialBalanceCorrectionSelectGridContainer").html(result2);
                                $("#SaleWaybillSelectGridContainer").html(result1);
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageDestinationDocumentForClientOrganizationPaymentFromClientDistributionSelectList");
                            }
                        });
                        
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDestinationDocumentForClientOrganizationPaymentFromClientDistributionSelectList");
                    }
                });
            });
    }
};