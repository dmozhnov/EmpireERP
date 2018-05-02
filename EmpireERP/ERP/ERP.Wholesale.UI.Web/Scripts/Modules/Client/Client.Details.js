var Client_Details = {
    Init: function () {
        $(document).ready(function () {
            // Редактирование клиента
            $('#btnEdit').click(function () {
                window.location = "/Client/Edit?id=" + $('#Id').val() + GetBackUrl();
            });

            // Возврат на прежнюю страницу
            $('#btnBack').click(function () {
                window.location = $('#BackURL').val();
            });

            // Удалить клиента
            $("#btnDelete").click(function () {
                if (confirm('Вы уверены?')) {
                    StartButtonProgress($(this));

                    $.ajax({
                        type: "POST",
                        url: "/Client/Delete/",
                        data: { clientId: $("#Id").val() },
                        success: function () {
                            window.location = $("#BackURL").val();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
                        }
                    });
                }
            });

            $(".yes_no_toggle").live("click", function () {
                var toggle = $(this);
                if ($(this).next("input").attr("id") == "IsBlockedManually") {
                    var isBlockedManually = $(this).next("input").val();
                    $.ajax({
                        type: "POST",
                        url: "/Client/SetClientBlockingValue/",
                        data: { id: $("#Id").val(), isBlockedManually: isBlockedManually },
                        success: function (result) {
                            ShowSuccessMessage(isBlockedManually == 1 ?
                                "Клиент «" + $("#Name").text() + "» заблокирован." :
                                "Ручная блокировка клиента «" + $("#Name").text() + "» снята.",
                                "messageClientEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ChangeYesNoToggleValue(toggle);
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
                        }
                    });
                }
            });

            $(".removeClientOrganization").live("click", function () {
                if (confirm('Вы уверены?')) {
                    var clientOrganizationId = $(this).parent("td").parent("tr").find(".ClientOrganizationId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Client/RemoveClientOrganization/",
                        data: { clientId: $("#Id").val(), clientOrganizationId: clientOrganizationId },
                        success: function (result) {
                            RefreshGrid("gridClientOrganization", function () {
                                ShowSuccessMessage("Организация удалена из списка.", "messageClientOrganizationList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageClientOrganizationList");
                        }
                    });
                }
            });

            // Вызов окна параметров печатной формы кнопкой
            $('#btnDivergenceActPrintingForm').click(function () {
                StartButtonProgress($(this));
                Client_Details.OnShowDivergenceActPrintingFormSettings();
            });

            // Вызов окна параметров печатной формы ссылкой
            $('#divergenceActPrintingForm').click(function () {
                Client_Details.OnShowDivergenceActPrintingFormSettings();
            });
        });
    },

    OnShowDivergenceActPrintingFormSettings: function () {
        var id = $('#Id').val();
        $.ajax({
            type: "GET",
            url: "/Report/Report0006PrintingFormSettings/",
            data: { clientId: id, clientOrganizationId: "" },
            success: function (result) {
                $('#report0006PrintingFormSettings').hide().html(result);
                $.validator.unobtrusive.parse($("#report0006PrintingFormSettings"));
                ShowModal("report0006PrintingFormSettings");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
            }
        });
    },
    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора организаций контрагента
    OnContractorOrganizationSelectLinkClick: function (organizationId, organizationShortName) {
        var clientId = $("#Id").val();
        $.ajax({
            type: "POST",
            url: "/Client/AddClientOrganization/",
            data: { clientId: clientId, organizationId: organizationId },
            success: function (result) {
                RefreshGrid("gridClientOrganization", function () {
                    ShowSuccessMessage("Организация добавлена в список.", "messageClientOrganizationList");
                    HideModal();
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageOrganizationSelectList");
            }
        });
    },

    // после успешного создания новой организации и добавления ее клиенту
    OnSuccessOrganizationEdit: function (result) {
        RefreshGrid("gridClientOrganization", function () {
            HideModal(function () {
                HideModal();
                ShowSuccessMessage("Организация создана и добавлена в список организаций клиента.", "messageClientOrganizationList");
            });
        });
    },

    OnSuccessEconomicAgentTypeSelect: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#economicAgentEdit").html(ajaxContext);
                $.validator.unobtrusive.parse($("#economicAgentEdit"));
                ShowModal("economicAgentEdit");
            });
        });
    },

    OnSuccessDealPaymentFromClientSave: function (ajaxContext) {
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridClientSales", function () {
                    Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
                    HideModal(function () {
                        HideModal(function () {
                            ShowSuccessMessage("Оплата сохранена.", "messageDealPaymentList");
                        });
                    });
                });
            });
        });
    },

    OnSuccessDealPaymentToClientSave: function (ajaxContext) {
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridClientSales", function () {
                    Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
                    HideModal(function () {
                        ShowSuccessMessage("Возврат оплаты сохранен.", "messageDealPaymentList");
                    });
                });
            });
        });
    },

    OnSuccessDealCreditInitialBalanceCorrectionSave: function (ajaxContext) {
        RefreshGrid("gridDealInitialBalanceCorrection", function () {
            RefreshGrid("gridDealPayment", function () {
                RefreshGrid("gridClientSales", function () {
                    HideModal(function () {
                        HideModal(function () {
                            Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
                            ShowSuccessMessage("Кредитовая корректировка сальдо сохранена.", "messageDealInitialBalanceCorrectionList");
                        });
                    });
                });
            });
        });
    },

    OnSuccessDealDebitInitialBalanceCorrectionSave: function (ajaxContext) {
        RefreshGrid("gridDealInitialBalanceCorrection", function () {
            RefreshGrid("gridDealPayment", function () {
                RefreshGrid("gridClientSales", function () {
                    HideModal(function () {
                        Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
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
                url: "/Client/DeleteDealPaymentFromClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        RefreshGrid("gridDealInitialBalanceCorrection", function () {
                            RefreshGrid("gridClientSales", function () {
                                HideModal(function () {
                                    Client_Details.RefreshMainDetails(result.MainDetails);
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
                url: "/Client/DeleteDealPaymentToClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        RefreshGrid("gridDealInitialBalanceCorrection", function () {
                            RefreshGrid("gridClientSales", function () {
                                HideModal(function () {
                                    Client_Details.RefreshMainDetails(result.MainDetails);
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
                url: "/Client/DeleteDealDebitInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (ajaxContext) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        RefreshGrid("gridDealPayment", function () {
                            RefreshGrid("gridClientSales", function () {
                                HideModal(function () {
                                    Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
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
                url: "/Client/DeleteDealCreditInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (ajaxContext) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        RefreshGrid("gridDealPayment", function () {
                            RefreshGrid("gridClientSales", function () {
                                HideModal(function () {
                                    Client_Details.RefreshMainDetails(ajaxContext.MainDetails);
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

    RefreshMainDetails: function (details) {
        $("#PaymentSum").text(details.PaymentSum);
        $("#Balance").text(details.Balance);
        $("#InitialBalance").text(details.InitialBalance);
    }
};