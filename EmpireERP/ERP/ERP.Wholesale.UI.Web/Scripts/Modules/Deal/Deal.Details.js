var Deal_Details = {
    Init: function () {
        $(document).ready(function () {
            $("#btnEdit").click(function () {
                window.location = "/Deal/Edit?id=" + $("#Id").val() + GetBackUrl();
            });

            // Возврат на прежнюю страницу
            $("#btnBack").click(function () {
                window.location = $("#BackURL").val();
            });
        });

        $("#gridDealQuotaSelect .dealQuota_select_link").live('click', function () {
            var dealQuotaId = $(this).parent("td").parent("tr").find(".quotaId").text();
            Deal_Details.OnDealQuotaSelectLinkClick(dealQuotaId);
        });        
    },

    OnSuccessContractEdit: function (result) {
        Deal_Details.RefreshClientContract(result.ClientContractName, result.ClientContractId, result.AccountOrganizationName, result.AccountOrganizationId, result.ClientOrganizationName, result.ClientOrganizationId);

        HideModal(function () {
            HideModal(function () {
                ShowSuccessMessage("Договор добавлен.", "messageDealEdit");
            });
        });
    },

    OnSuccessDealPaymentFromClientSave: function (ajaxContext) {
        Deal_Details.RefreshMainDetailsAndPermissions(ajaxContext);
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridDealSales", function () {
                    HideModal(function () {
                        HideModal(function () {
                            ShowSuccessMessage("Оплата добавлена.", "messageDealPaymentList");
                        });
                    });
                });
            });
        });
    },

    OnSuccessDealPaymentToClientSave: function (ajaxContext) {
        Deal_Details.RefreshMainDetailsAndPermissions(ajaxContext);
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridDealSales", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Оплата добавлена.", "messageDealPaymentList");
                    });
                });
            });
        });
    },

    OnSuccessDealCreditInitialBalanceCorrectionSave: function (ajaxContext) {
        Deal_Details.RefreshMainDetailsAndPermissions(ajaxContext);
        RefreshGrid("gridDealInitialBalanceCorrection", function () {
            RefreshGrid("gridDealPayment", function () {
                RefreshGrid("gridDealSales", function () {
                    HideModal(function () {
                        HideModal(function () {
                            ShowSuccessMessage("Кредитовая корректировка сальдо сохранена.", "messageDealInitialBalanceCorrectionList");
                        });
                    });
                });
            });
        });
    },

    OnSuccessDealDebitInitialBalanceCorrectionSave: function (ajaxContext) {
        Deal_Details.RefreshMainDetailsAndPermissions(ajaxContext);
        RefreshGrid("gridDealInitialBalanceCorrection", function () {
            RefreshGrid("gridDealPayment", function () {
                RefreshGrid("gridDealSales", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Дебетовая корректировка сальдо сохранена.", "messageDealInitialBalanceCorrectionList");
                    });
                });
            });
        });
    },

    OnDealPaymentFromClientDeleteButtonClick: function (paymentId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealPaymentFromClient"));

            $.ajax({
                type: "POST",
                url: "/Deal/DeleteDealPaymentFromClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        RefreshGrid("gridDealInitialBalanceCorrection", function () {
                            RefreshGrid("gridDealSales", function () {
                                HideModal(function () {
                                    Deal_Details.RefreshMainDetailsAndPermissions(result);
                                    ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                });
                            });
                        });
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentFromClientDetails");
                }
            });
        }
    },

    OnDealPaymentToClientDeleteButtonClick: function (paymentId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealPaymentToClient"));

            $.ajax({
                type: "POST",
                url: "/Deal/DeleteDealPaymentToClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        RefreshGrid("gridDealInitialBalanceCorrection", function () {
                            RefreshGrid("gridDealSales", function () {
                                HideModal(function () {
                                    Deal_Details.RefreshMainDetailsAndPermissions(result);
                                    ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                });
                            });
                        });
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentToClientDetails");
                }
            });
        }
    },

    OnDealDebitInitialBalanceCorrectionDeleteButtonClick: function (correctionId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealDebitInitialBalanceCorrection"));

            $.ajax({
                type: "POST",
                url: "/Deal/DeleteDealDebitInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (ajaxContext) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        RefreshGrid("gridDealPayment", function () {
                            RefreshGrid("gridDealSales", function () {
                                HideModal(function () {
                                    Deal_Details.RefreshMainDetails(ajaxContext.MainDetails);
                                    ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                                });
                            });
                        });
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealDebitInitialBalanceCorrectionDetails");
                }
            });
        }
    },

    OnDealCreditInitialBalanceCorrectionDeleteButtonClick: function (correctionId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealCreditInitialBalanceCorrection"));

            $.ajax({
                type: "POST",
                url: "/Deal/DeleteDealCreditInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (ajaxContext) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        RefreshGrid("gridDealPayment", function () {
                            RefreshGrid("gridDealSales", function () {
                                HideModal(function () {
                                    Deal_Details.RefreshMainDetails(ajaxContext.MainDetails);
                                    ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                                });
                            });
                        });
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealCreditInitialBalanceCorrectionDetails");
                }
            });
        }
    },

    RefreshMainDetailsAndPermissions: function (result) {
        Deal_Details.RefreshMainDetails(result.MainDetails);
        Deal_Details.RefreshPermissions(result.Permissions);
    },

    RefreshMainDetails: function (details) {
        $("#Name").text(details.Name);
        $("#SaleSum").text(details.SaleSum);
        $("#ShippingPendingSaleSum").text(details.ShippingPendingSaleSum);
        $("#StageName").text(details.StageName);
        $("#PaymentSum").text(details.PaymentSum);
        $("#Balance").text(details.Balance);
        $("#StageStartDate").text(details.StageStartDate);
        $("#StageDuration").text(details.StageDuration);
        $("#MaxPaymentDelayDuration").text(details.MaxPaymentDelayDuration);
        $("#PaymentDelaySum").text(details.PaymentDelaySum);
        $("#TotalReturnedSum").text(details.TotalReturnedSum);
        $("#TotalReservedByReturnSum").text(details.TotalReservedByReturnSum);
        $("#InitialBalance").text(details.InitialBalance);

        Deal_Details.RefreshClientContract(details.ClientContractName, details.ClientContractId, details.AccountOrganizationName, details.AccountOrganizationId, details.ClientOrganizationName, details.ClientOrganizationId);

        $("#Comment").html(details.Comment);

        UpdateElementVisibility("linkAddContract", details.AllowToAddContract);
        UpdateElementVisibility("linkChangeContract", details.AllowToChangeContract);
        UpdateElementVisibility("AllowToChangeContract", details.AllowToChangeContract);
        UpdateElementVisibility("linkChangeStage", details.AllowToChangeStage);
    },

    RefreshPermissions: function (permissions) {
        UpdateButtonAvailability("btnCreateDealPaymentFromClient", permissions.AllowToCreateDealPaymentFromClient);
        UpdateElementVisibility("btnCreateDealPaymentFromClient", permissions.AllowToCreateDealPaymentFromClient);
        UpdateButtonAvailability("btnCreateDealPaymentToClient", permissions.AllowToCreateDealPaymentToClient);
        UpdateElementVisibility("btnCreateDealPaymentToClient", permissions.AllowToCreateDealPaymentToClient);
        UpdateButtonAvailability("btnCreateDealCreditInitialBalanceCorrection", permissions.AllowToCreateDealCreditInitialBalanceCorrection);
        UpdateElementVisibility("btnCreateDealCreditInitialBalanceCorrection", permissions.AllowToCreateDealCreditInitialBalanceCorrection);
        UpdateButtonAvailability("btnCreateDealDebitInitialBalanceCorrection", permissions.AllowToCreateDealDebitInitialBalanceCorrection);
        UpdateElementVisibility("btnCreateDealDebitInitialBalanceCorrection", permissions.AllowToCreateDealDebitInitialBalanceCorrection);
        UpdateButtonAvailability("btnEdit", permissions.AllowToEdit);
        UpdateElementVisibility("btnEdit", permissions.AllowToEdit);
        UpdateButtonAvailability("btnCreateExpenditureWaybill", permissions.AllowToCreateExpenditureWaybill);
        UpdateElementVisibility("btnCreateExpenditureWaybill", permissions.IsPossibilityToCreateExpenditureWaybill);
        UpdateButtonAvailability("btnCreateReturnFromClientWaybill", permissions.AllowToCreateReturnFromClientWaybill);
        UpdateElementVisibility("btnCreateReturnFromClientWaybill", permissions.IsPossibilityToCreateReturnFromClientWaybill);
        UpdateElementVisibility("btnAddQuota", permissions.AllowToAddQuota);
        UpdateElementVisibility("btnAddAllQuotas", permissions.AllowToAddQuota);
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора собственных организаций (при создании договора)
    OnAccountOrganizationSelectLinkClick: function (accountOrganizationId, accountOrganizationShortName) {
        $("#clientContractEdit #AccountOrganizationId").val(accountOrganizationId).ValidationValid();
        $("#clientContractEdit #AccountOrganizationName").text(accountOrganizationShortName);
        HideModal();
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора организаций контрагента
    // Грид может быть вызван для добавления организации поставщику (как 1 уровень) и для выбора организации в договор (как 2 уровень)
    OnContractorOrganizationSelectLinkClick: function (organizationId, organizationShortName) {
        $("#clientContractEdit #ClientOrganizationId").val(organizationId).ValidationValid();
        $("#clientContractEdit #ClientOrganizationName").text(organizationShortName);
        HideModal();
    },

    RefreshClientContract: function (contractName, contractId, accountOrganizationName, accountOrganizationId, clientOrganizationName, clientOrganizationId) {
        $("#ClientContractName").text(contractName);
        $("#ClientContractId").val(contractId);

        var clientOrganizationLink;
        if (clientOrganizationId != "" && clientOrganizationId != "0") {
            clientOrganizationLink = '<a href="/ClientOrganization/Details?id=' + clientOrganizationId
                + GetBackUrl() + '">' + clientOrganizationName + '</a>';
        }
        else {
            clientOrganizationLink = clientOrganizationName;
        }
        $("#ClientOrganizationLink").html(clientOrganizationLink);

        var accountOrganizationLink;
        if (accountOrganizationId != "" && accountOrganizationId != "0") {
            accountOrganizationLink = '<a href="/AccountOrganization/Details?id=' + accountOrganizationId
                + GetBackUrl() + '">' + accountOrganizationName + '</a>'
        }
        else {
            accountOrganizationLink = accountOrganizationName;
        }
        $("#AccountOrganizationLink").html(accountOrganizationLink);

        UpdateElementVisibility("linkAddContract", false);
        UpdateElementVisibility("linkChangeContract", true);
    },

    OnClientContractSelectLinkClick: function (contractName, contractId, accountOrganizationName, accountOrganizationId, clientOrganizationName, clientOrganizationId) {
        var dealId = $("#Id").val();
        $.ajax({
            type: "POST",
            url: "/Deal/SetContract",
            data: { dealId: dealId, clientContractId: contractId },
            success: function (result) {
                Deal_Details.RefreshClientContract(contractName, contractId, accountOrganizationName, accountOrganizationId, clientOrganizationName, clientOrganizationId);
                HideModal(function () {
                    ShowSuccessMessage("Договор добавлен.", "messageDealEdit");
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientContractSelectList");
            }
        });
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора квот
    OnDealQuotaSelectLinkClick: function (dealQuotaId) {
        var dealId = $("#Id").val();
        $.ajax({
            type: "POST",
            url: "/Deal/AddQuota",
            data: { dealId: dealId, dealQuotaId: dealQuotaId },
            success: function (result) {
                RefreshGrid("gridDealQuota", function () {
                    Deal_Details.RefreshPermissions(result.Permissions);
                    HideModal(function () {
                        ShowSuccessMessage("Квота добавлена.", "messageDealQuotaList");
                    });
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageDealQuotaSelectorListGrid");
            }
        });
    }
};