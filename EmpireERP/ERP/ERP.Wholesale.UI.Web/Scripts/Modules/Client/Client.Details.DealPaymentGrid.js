var Client_Details_DealPaymentGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridDealPayment table.grid_table tr").each(function () {
                var id = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + id + GetBackUrl());
            });

            // добавление оплаты
            $("#gridDealPayment #btnCreateDealPaymentFromClient").click(function () {
                var clientId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Client/CreateDealPaymentFromClient",
                    data: { clientId: clientId },
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
                var clientId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Client/CreateDealPaymentToClient",
                    data: { clientId: clientId },
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
                        url: "/Client/DeleteDealPaymentFromClient",
                        data: { paymentId: paymentId },
                        success: function (ajaxContext) {
                            RefreshGrid("gridDealPayment", function () {
                                RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                    RefreshGrid("gridClientSales", function () {
                                        Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
                                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
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
                        url: "/Client/DeleteDealPaymentToClient",
                        data: { paymentId: paymentId },
                        success: function (ajaxContext) {
                            RefreshGrid("gridDealPayment", function () {
                                RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                    RefreshGrid("gridClientSales", function () {
                                        Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
                                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
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
                        destinationDocumentSelectorControllerName: "Client",
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