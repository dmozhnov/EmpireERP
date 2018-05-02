var DealPaymentDocument_DealPayment_DealPaymentGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridDealPayment table.grid_table tr").each(function () {
                var clientOrganizationId = $(this).find(".ClientOrganizationId").text();
                $(this).find("a.ClientOrganizationName").attr("href", "/ClientOrganization/Details?id=" + clientOrganizationId + GetBackUrl());

                var clientId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + clientId + GetBackUrl());

                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + GetBackUrl());
            });

            $("#gridDealPayment #btnCreateClientOrganizationPaymentFromClient").click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/DealPayment/CreateClientOrganizationPaymentFromClient",
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

            $("#gridDealPayment #btnCreateDealPaymentFromClient").click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/DealPayment/CreateDealPaymentFromClient",
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
                StartButtonProgress($(this));
                
                $.ajax({
                    type: "GET",
                    url: "/DealPayment/CreateDealPaymentToClient",
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
                var paymentId = $(this).parent("td").parent("tr").find(".Id").text();
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

            $("#gridDealPayment .linkPaymentFromClientDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var paymentId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/DealPayment/DeleteDealPaymentFromClient",
                        data: { paymentId: paymentId },
                        success: function (result) {
                            RefreshGrid("gridDealPayment", function () {
                                ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                        }
                    });
                }
            });

            $("#gridDealPayment .linkPaymentToClientDetails").click(function () {
                var paymentId = $(this).parent("td").parent("tr").find(".Id").text();
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

            $("#gridDealPayment .linkPaymentToClientDelete").click(function () {
                if (confirm("Вы уверены?")) {
                    var paymentId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/DealPayment/DeleteDealPaymentToClient",
                        data: { paymentId: paymentId },
                        success: function (result) {
                            RefreshGrid("gridDealPayment", function () {
                                ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentList");
                        }
                    });
                }
            });

            $("#gridDealPayment .linkPaymentFromClientEdit").click(function () {
                var paymentId = $(this).findCell(".Id").text();
                $.ajax({
                    type: "POST",
                    url: "/DealPayment/SelectDestinationDocumentsForDealPaymentFromClientRedistribution",
                    data: { dealPaymentFromClientId: paymentId,
                        destinationDocumentSelectorControllerName: "DealPayment",
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