var ClientOrganization_Details = {
    Init: function () {
        $(document).ready(function () {
            $('#btnBack').click(function () {
                window.location = $('#BackURL').val();
            });

            $('#btnAddRussianBankAccount').live("click", function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/ClientOrganization/AddRussianBankAccount/",
                    data: { organizationId: $('#Id').val() },
                    success: function (result) {
                        $('#RussianBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#RussianBankAccountEdit"));
                        ShowModal("RussianBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            //Добавление расчетного счета в иностранном банке
            $('#btnAddForeignBankAccount').live("click", function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/ClientOrganization/AddForeignBankAccount/",
                    data: { organizationId: $('#Id').val() },
                    success: function (result) {
                        $('#ForeignBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#ForeignBankAccountEdit"));
                        ShowModal("ForeignBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            $(".russianBankAccountEdit").live("click", function () {
                var bankAccId = $(this).parent("td").parent("tr").find(".bankAccountId").text();
                $.ajax({
                    type: "GET",
                    url: "/ClientOrganization/EditRussianBankAccount/",
                    data: { organizationId: $('#Id').val(), bankAccountId: bankAccId },
                    success: function (result) {
                        $('#RussianBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#RussianBankAccountEdit"));
                        ShowModal("RussianBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            //редактирование счета в иностранном банке
            $(".foreignBankAccountEdit").live("click", function () {
                var bankAccId = $(this).parent("td").parent("tr").find(".bankAccountId").text();
                $.ajax({
                    type: "GET",
                    url: "/ClientOrganization/EditForeignBankAccount/",
                    data: { organizationId: $('#Id').val(), bankAccountId: bankAccId },
                    success: function (result) {
                        $('#ForeignBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#ForeignBankAccountEdit"));
                        ShowModal("ForeignBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            $(".russianBankAccountRemove").live("click", function () {
                var bankAccId = $(this).parent("td").parent("tr").find(".bankAccountId").text();

                if (confirm("Вы уверены?")) {
                    StartGridProgress($(this).closest(".grid"));

                    $.ajax({
                        type: "POST",
                        url: "/ClientOrganization/RemoveRussianBankAccount/",
                        data: { organizationId: $('#Id').val(), bankAccountId: bankAccId },
                        success: function (result) {
                            RefreshGrid("gridRussianBankAccounts", function () {
                                ShowSuccessMessage("Расчетный счет удален.", "messageRussianBankAccountList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                        }
                    });
                }
            });

            //Удаление счета в иностранном банке
            $(".foreignBankAccountRemove").live("click", function () {
                var bankAccId = $(this).parent("td").parent("tr").find(".bankAccountId").text();

                if (confirm("Вы уверены?")) {
                    StartGridProgress($(this).closest(".grid"));

                    $.ajax({
                        type: "POST",
                        url: "/ClientOrganization/RemoveForeignBankAccount/",
                        data: { organizationId: $('#Id').val(), bankAccountId: bankAccId },
                        success: function (result) {
                            RefreshGrid("gridForeignBankAccounts", function () {
                                ShowSuccessMessage("Расчетный счет удален.", "messageForeignBankAccountList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                        }
                    });
                }
            });

            // редактирование организации
            $("#btnEditClientOrganization").click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/ClientOrganization/Edit",
                    data: { organizationId: $('#Id').val() },
                    success: function (result) {
                        $('#organizationEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#organizationEdit"));
                        ShowModal("organizationEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageClientOrganizationEdit");
                    }
                });
            });

            // удаление организации
            $("#btnDeleteClientOrganization").click(function () {
                if (confirm('Вы уверены?')) {
                    StartButtonProgress($(this));

                    $.ajax({
                        type: "POST",
                        url: "/ClientOrganization/Delete/",
                        data: { clientOrganizationId: $('#Id').val() },
                        success: function () {
                            window.location = $("#BackURL").val();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageClientOrganizationEdit");
                        }
                    });
                }
            });

            // Вызов окна параметров печатной формы кнопкой
            $('#btnDivergenceActPrintingForm').click(function () {
                StartButtonProgress($(this));
                ClientOrganization_Details.OnShowDivergenceActPrintingFormSettings();
            });

            // Вызов окна параметров печатной формы ссылкой
            $('#divergenceActPrintingForm').click(function () {
                ClientOrganization_Details.OnShowDivergenceActPrintingFormSettings();
            });
        });

    },


    OnShowDivergenceActPrintingFormSettings: function () {
        var id = $('#Id').val();
        $.ajax({
            type: "GET",
            url: "/Report/Report0006PrintingFormSettings/",
            data: { clientId: "", clientOrganizationId: id },
            success: function (result) {
                $('#report0006PrintingFormSettings').hide().html(result);
                $.validator.unobtrusive.parse($("#report0006PrintingFormSettings"));
                ShowModal("report0006PrintingFormSettings");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientOrganizationEdit");
            }
        });
    },

    OnSuccessRussianBankAccountAdd: function (ajaxContext) {
        RefreshGrid("gridRussianBankAccounts", function () {
            HideModal(function () {
                ShowSuccessMessage("Расчетный счет добавлен.", "messageRussianBankAccountList");
            });
        });
    },

    OnSuccessForeignBankAccountAdd: function (ajaxContext) {
        RefreshGrid("gridForeignBankAccounts", function () {
            HideModal(function () {
                ShowSuccessMessage("Расчетный счет добавлен.", "messageForeignBankAccountList");
            });
        });
    },

    OnSuccessRussianBankAccountEdit: function (ajaxContext) {
        RefreshGrid("gridRussianBankAccounts", function () {
            HideModal(function () {
                ShowSuccessMessage("Расчетный счет сохранен.", "messageRussianBankAccountList");
            });
        });
    },

    OnSuccessForeignBankAccountEdit: function (ajaxContext) {
        RefreshGrid("gridForeignBankAccounts", function () {
            HideModal(function () {
                ShowSuccessMessage("Расчетный счет сохранен.", "messageForeignBankAccountList");
            });
        });
    },

    OnSuccessClientOrganizationPaymentFromClientSave: function () {
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridSaleWaybill", function () {
                    HideModal(function () {
                        HideModal(function () {
                            ClientOrganization_Details.RefreshMainDetails();
                            ShowSuccessMessage("Оплата сохранена.", "messageDealPaymentList");
                        });
                    });
                });
            });
        });
    },

    OnSuccessDealPaymentFromClientSave: function () {
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridSaleWaybill", function () {
                    HideModal(function () {
                        HideModal(function () {
                            ClientOrganization_Details.RefreshMainDetails();
                            ShowSuccessMessage("Оплата сохранена.", "messageDealPaymentList");
                        });
                    });
                });
            });
        });
    },

    OnSuccessDealPaymentToClientSave: function () {
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridSaleWaybill", function () {
                    HideModal(function () {
                        ClientOrganization_Details.RefreshMainDetails();
                        ShowSuccessMessage("Возврат оплаты сохранен.", "messageDealPaymentList");
                    });
                });
            });
        });
    },

    OnSuccessDealCreditInitialBalanceCorrectionSave: function () {
        RefreshGrid("gridDealInitialBalanceCorrection", function () {
            RefreshGrid("gridDealPayment", function () {
                RefreshGrid("gridSaleWaybill", function () {
                    HideModal(function () {
                        HideModal(function () {
                            ClientOrganization_Details.RefreshMainDetails();
                            ShowSuccessMessage("Кредитовая корректировка сальдо сохранена.", "messageDealInitialBalanceCorrectionList");
                        });
                    });
                });
            });
        });
    },

    OnSuccessDealDebitInitialBalanceCorrectionSave: function () {
        RefreshGrid("gridDealInitialBalanceCorrection", function () {
            RefreshGrid("gridDealPayment", function () {
                RefreshGrid("gridSaleWaybill", function () {
                    HideModal(function () {
                        ClientOrganization_Details.RefreshMainDetails();
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
                url: "/ClientOrganization/DeleteDealPaymentFromClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        RefreshGrid("gridDealInitialBalanceCorrection", function () {
                            RefreshGrid("gridSaleWaybill", function () {
                                // TODO Не так надо, а чтобы с сервера возвращались MainDetails! Сейчас же идет второй запрос. Все вызовы функции обработать
                                ClientOrganization_Details.RefreshMainDetails(function () {
                                    HideModal(function () {
                                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                    });
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
                url: "/ClientOrganization/DeleteDealPaymentToClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        RefreshGrid("gridDealInitialBalanceCorrection", function () {
                            RefreshGrid("gridSaleWaybill", function () {
                                ClientOrganization_Details.RefreshMainDetails(function () {
                                    HideModal(function () {
                                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                                    });
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
                url: "/ClientOrganization/DeleteDealDebitInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (ajaxContext) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        RefreshGrid("gridDealPayment", function () {
                            RefreshGrid("gridSaleWaybill", function () {
                                HideModal(function () {
                                    ClientOrganization_Details.RefreshMainDetails();
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
                url: "/ClientOrganization/DeleteDealCreditInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (ajaxContext) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        RefreshGrid("gridDealPayment", function () {
                            RefreshGrid("gridSaleWaybill", function () {
                                HideModal(function () {
                                    ClientOrganization_Details.RefreshMainDetails();
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

    OnSuccessContractEdit: function (ajaxContext) {
        RefreshGrid("gridClientContract", function () {
            HideModal(function () {
                ShowSuccessMessage("Договор по сделке сохранен.", "messageClientContractList");
            });
        });
    },

    RefreshMainDetails: function (onComplete) {
        $.ajax({
            type: "GET",
            url: "/ClientOrganization/ShowMainDetails",
            data: { organizationId: $('#Id').val() },
            success: function (result) {
                $('#clientOrganizationMainDetails').html(result);
                $.validator.unobtrusive.parse($("#clientOrganizationMainDetails"));
                $(".page_title_item_name").text($("#clientOrganizationMainDetails #ShortName").text());
                if (onComplete != undefined)
                    onComplete();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientOrganizationEdit");
            }
        });
    },

    OnSuccessClientOrganizationEdit: function (ajaxContext) {
        ClientOrganization_Details.RefreshMainDetails(function () {
            HideModal(function () {
                ShowSuccessMessage("Сохранено.", "messageClientOrganizationEdit");
            });
        });
    }
};