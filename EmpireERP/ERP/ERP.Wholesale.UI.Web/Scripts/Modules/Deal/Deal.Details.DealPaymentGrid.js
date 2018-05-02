var Deal_Details_DealPaymentGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridDealPayment #btnCreateDealPaymentFromClient").click(function () {
                var dealId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Deal/CreateDealPaymentFromClient",
                    data: { dealId: dealId },
                    success: function (result) {
                        $("#dealPaymentFromClientEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealPaymentFromClientEdit"));
                        ShowModal("dealPaymentFromClientEdit");
                        $("#dealPaymentFromClientEdit #PaymentDocumentNumber").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                    }
                });
            });

            $("#gridDealPayment #btnCreateDealPaymentToClient").click(function () {
                var dealId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Deal/CreateDealPaymentToClient",
                    data: { dealId: dealId },
                    success: function (result) {
                        $("#dealPaymentToClientEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealPaymentToClientEdit"));
                        ShowModal("dealPaymentToClientEdit");
                        $("#dealPaymentToClientEdit #PaymentDocumentNumber").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                    }
                });
            });

            // Детали платежа клиента
            $("#gridDealPayment .linkPaymentFromClientDetails").click(function () {
                var paymentId = $(this).parent("td").parent("tr").find(".PaymentId").text();
                $.ajax({
                    type: "GET",
                    url: "/DealPayment/DealPaymentFromClientDetails",
                    data: { paymentId: paymentId },
                    success: function (result) {
                        $("#dealPaymentDocumentDetails").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealPaymentDocumentDetails"));
                        ShowModal("dealPaymentDocumentDetails");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                    }
                });
            });

            $("#gridDealPayment .linkPaymentToClientDetails").click(function () {
                var paymentId = $(this).parent("td").parent("tr").find(".PaymentId").text();
                $.ajax({
                    type: "GET",
                    url: "/DealPayment/DealPaymentToClientDetails",
                    data: { paymentId: paymentId },
                    success: function (result) {
                        $("#dealPaymentDocumentDetails").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealPaymentDocumentDetails"));
                        ShowModal("dealPaymentDocumentDetails");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                    }
                });
            });

            $("#gridDealPayment .linkPaymentFromClientDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var paymentId = $(this).parent("td").parent("tr").find(".PaymentId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/DeleteDealPaymentFromClient",
                        data: { paymentId: paymentId },
                        success: function (result) {
                            RefreshGrid("gridDealPayment", function () {
                                RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                    RefreshGrid("gridDealSales", function () {
                                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                        Deal_Details.RefreshMainDetailsAndPermissions(result);
                                    });
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                        }
                    });
                }
            });

            $("#gridDealPayment .linkPaymentToClientDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var paymentId = $(this).parent("td").parent("tr").find(".PaymentId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/DeleteDealPaymentToClient",
                        data: { paymentId: paymentId },
                        success: function (result) {
                            RefreshGrid("gridDealPayment", function () {
                                RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                    RefreshGrid("gridDealSales", function () {
                                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                        Deal_Details.RefreshMainDetailsAndPermissions(result);
                                    });
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                        }
                    });
                }
            });

            $("#gridDealPayment .linkPaymentFromClientEdit").click(function () {
                var paymentId = $(this).findCell(".PaymentId").text();
                $.ajax({
                    type: "POST",
                    url: "/DealPayment/SelectDestinationDocumentsForDealPaymentFromClientRedistribution",
                    data: { dealPaymentFromClientId: paymentId,
                        destinationDocumentSelectorControllerName: "Deal",
                        destinationDocumentSelectorActionName: "SaveDealPaymentFromClient"
                    },
                    success: function (result) {
                        $("#destinationDocumentSelectorForDealPaymentFromClientDistribution").hide().html(result);
                        $.validator.unobtrusive.parse($("#destinationDocumentSelectorForDealPaymentFromClientDistribution"));
                        ShowModal("destinationDocumentSelectorForDealPaymentFromClientDistribution");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                    }
                });
            });
        });
    }
}; 