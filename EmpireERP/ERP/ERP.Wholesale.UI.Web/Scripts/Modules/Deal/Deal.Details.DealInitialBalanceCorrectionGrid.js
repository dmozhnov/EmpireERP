var Deal_Details_DealInitialBalanceCorrectionGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridDealInitialBalanceCorrection .linkDealCreditInitialBalanceCorrectionEdit").click(function () {
                var correctionId = $(this).findCell(".CorrectionId").text();
                $.ajax({
                    type: "POST",
                    url: "/DealInitialBalanceCorrection/SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionRedistribution",
                    data: {
                        dealCreditInitialBalanceCorrectionId: correctionId,
                        destinationDocumentSelectorControllerName: "Deal",
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

            $("#gridDealInitialBalanceCorrection .linkDealCreditInitialBalanceCorrectionDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var correctionId = $(this).findCell(".CorrectionId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/DeleteDealCreditInitialBalanceCorrection",
                        data: { correctionId: correctionId },
                        success: function (ajaxContext) {
                            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                RefreshGrid("gridDealPayment", function () {
                                    RefreshGrid("gridDealSales", function () {
                                        Deal_Details.RefreshMainDetails(ajaxContext.MainDetails);
                                        ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                                    });
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                        }
                    });
                }
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

            $("#gridDealInitialBalanceCorrection .linkDealDebitInitialBalanceCorrectionDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var correctionId = $(this).findCell(".CorrectionId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/DeleteDealDebitInitialBalanceCorrection",
                        data: { correctionId: correctionId },
                        success: function (ajaxContext) {
                            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                RefreshGrid("gridDealPayment", function () {
                                    RefreshGrid("gridDealSales", function () {
                                        Deal_Details.RefreshMainDetails(ajaxContext.MainDetails);
                                        ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                                    });
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealInitialBalanceCorrectionList");
                        }
                    });
                }
            });

            $("#gridDealInitialBalanceCorrection #btnCreateDealCreditInitialBalanceCorrection").click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/Deal/CreateDealCreditInitialBalanceCorrection",
                    data: { dealId: $("#Id").val() },
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
                    url: "/Deal/CreateDealDebitInitialBalanceCorrection",
                    data: { dealId: $("#Id").val() },
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
        });
    }
}; 