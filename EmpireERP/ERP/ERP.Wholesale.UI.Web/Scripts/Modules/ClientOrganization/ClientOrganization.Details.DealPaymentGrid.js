var ClientOrganization_Details_DealPaymentGrid = {
    Init: function () {
        $(document).ready(function () {

            $("#gridDealPayment table.grid_table tr").each(function () {
                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + GetBackUrl());

                var clientId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + clientId + GetBackUrl());
            });

            $("#btnCreateClientOrganizationPaymentFromClient").click(function () {
                var clientOrganizationId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ClientOrganization/CreateClientOrganizationPaymentFromClient",
                    data: { clientOrganizationId: clientOrganizationId },
                    success: function (result) {
                        $("#clientOrganizationPaymentFromClientEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#clientOrganizationPaymentFromClientEdit"));
                        ShowModal("clientOrganizationPaymentFromClientEdit");
                        $("#clientOrganizationPaymentFromClientEdit #PaymentDocumentNumber").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                    }
                });
            });

            $("#btnCreateDealPaymentToClient").click(function () {
                var clientOrganizationId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ClientOrganization/CreateDealPaymentToClient",
                    data: { clientOrganizationId: clientOrganizationId },
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
                        url: "/ClientOrganization/DeleteDealPaymentFromClient",
                        data: { paymentId: paymentId },
                        success: function (result) {
                            RefreshGrid("gridDealPayment", function () {
                                RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                    RefreshGrid("gridSaleWaybill", function () {
                                        ClientOrganization_Details.RefreshMainDetails(function () {
                                            ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                        });
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
                        url: "/ClientOrganization/DeleteDealPaymentToClient",
                        data: { paymentId: paymentId },
                        success: function (ajaxContext) {
                            RefreshGrid("gridDealPayment", function () {
                                RefreshGrid("gridDealInitialBalanceCorrection", function () {
                                    RefreshGrid("gridSaleWaybill", function () {
                                        ClientOrganization_Details.RefreshMainDetails(function () {
                                            ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                        });
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
                        destinationDocumentSelectorControllerName: "ClientOrganization",
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