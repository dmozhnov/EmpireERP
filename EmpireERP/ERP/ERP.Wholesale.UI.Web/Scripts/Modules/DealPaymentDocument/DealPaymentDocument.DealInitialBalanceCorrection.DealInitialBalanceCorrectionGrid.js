var DealPaymentDocument_DealInitialBalanceCorrection_DealInitialBalanceCorrectionGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridDealInitialBalanceCorrection table.grid_table tr").each(function () {
                var clientOrganizationId = $(this).find(".ClientOrganizationId").text();
                $(this).find("a.ClientOrganizationName").attr("href", "/ClientOrganization/Details?id=" + clientOrganizationId + GetBackUrl());

                var clientId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + clientId + GetBackUrl());

                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + GetBackUrl());
            });

            $("#gridDealInitialBalanceCorrection #btnCreateDealCreditInitialBalanceCorrection").click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/DealInitialBalanceCorrection/CreateDealCreditInitialBalanceCorrection",
                    success: function (result) {
                        $("#dealCreditInitialBalanceCorrectionEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealCreditInitialBalanceCorrectionEdit"));
                        ShowModal("dealCreditInitialBalanceCorrectionEdit");
                        $("#dealCreditInitialBalanceCorrectionEdit #CorrectionReason").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                    }
                });
            });

            $("#gridDealInitialBalanceCorrection #btnCreateDealDebitInitialBalanceCorrection").click(function () {
                StartButtonProgress($(this));
                
                $.ajax({
                    type: "GET",
                    url: "/DealInitialBalanceCorrection/CreateDealDebitInitialBalanceCorrection",
                    success: function (result) {
                        $("#dealDebitInitialBalanceCorrectionEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealDebitInitialBalanceCorrectionEdit"));
                        ShowModal("dealDebitInitialBalanceCorrectionEdit");
                        $("#dealDebitInitialBalanceCorrectionEdit #CorrectionReason").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                    }
                });
            });

            $("#gridDealInitialBalanceCorrection .linkDealCreditInitialBalanceCorrectionDetails").click(function () {
                var correctionId = $(this).findCell(".CorrectionId").text();

                $.ajax({
                    type: "GET",
                    url: "/DealInitialBalanceCorrection/DealCreditInitialBalanceCorrectionDetails",
                    data: { correctionId: correctionId },
                    success: function (result) {
                        $("#dealPaymentDocumentDetails").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealPaymentDocumentDetails"));
                        ShowModal("dealPaymentDocumentDetails");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                    }
                });
            });

            $("#gridDealInitialBalanceCorrection .linkDealDebitInitialBalanceCorrectionDetails").click(function () {
                var correctionId = $(this).findCell(".CorrectionId").text();

                $.ajax({
                    type: "GET",
                    url: "/DealInitialBalanceCorrection/DealDebitInitialBalanceCorrectionDetails",
                    data: { correctionId: correctionId },
                    success: function (result) {
                        $("#dealPaymentDocumentDetails").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealPaymentDocumentDetails"));
                        ShowModal("dealPaymentDocumentDetails");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                    }
                });
            });

            $("#gridDealInitialBalanceCorrection .linkDealCreditInitialBalanceCorrectionDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var correctionId = $(this).findCell(".CorrectionId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/DealInitialBalanceCorrection/DeleteDealCreditInitialBalanceCorrection",
                        data: { correctionId: correctionId },
                        success: function (result) {
                            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                        }
                    });
                }
            });

            $("#gridDealInitialBalanceCorrection .linkDealDebitInitialBalanceCorrectionDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var correctionId = $(this).findCell(".CorrectionId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/DealInitialBalanceCorrection/DeleteDealDebitInitialBalanceCorrection",
                        data: { correctionId: correctionId },
                        success: function (result) {
                            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                        }
                    });
                }
            });

            $("#gridDealInitialBalanceCorrection .linkDealCreditInitialBalanceCorrectionEdit").click(function () {
                var correctionId = $(this).findCell(".CorrectionId").text();

                $.ajax({
                    type: "POST",
                    url: "/DealInitialBalanceCorrection/SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionRedistribution",
                    data: {
                        dealCreditInitialBalanceCorrectionId: correctionId,
                        destinationDocumentSelectorControllerName: "DealInitialBalanceCorrection",
                        destinationDocumentSelectorActionName: "SaveDealCreditInitialBalanceCorrection"
                    },
                    success: function (result) {
                        $("#destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution").hide().html(result);
                        $.validator.unobtrusive.parse($("#destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution"));
                        ShowModal("destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                    }
                });
            });
        });
    }
};
