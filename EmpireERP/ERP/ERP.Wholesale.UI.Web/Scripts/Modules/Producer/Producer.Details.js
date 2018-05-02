var Producer_Details = {
    Init: function () {
        $(document).ready(function () {

            $("#btnCreateProductionOrder").live("click", function () {
                window.location = "/ProductionOrder/Create?producerId=" + $("#Id").val() + GetBackUrl();
            });

            $('#btnAddRussianBankAccount').live("click", function () {
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Producer/AddRussianBankAccount",
                    data: { producerId: $('#Id').val() },
                    success: function (result) {
                        $('#producerBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#producerBankAccountDetailsForEdit"));
                        ShowModal("producerBankAccountDetailsForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            $('#btnAddForeignBankAccount').live("click", function () {
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Producer/AddForeignBankAccount",
                    data: { producerId: $('#Id').val() },
                    success: function (result) {
                        $('#producerForeignBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#producerForeignBankAccountDetailsForEdit"));
                        ShowModal("producerForeignBankAccountDetailsForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            $('#gridRussianBankAccounts .edit_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();

                $.ajax({
                    type: "GET",
                    url: "/Producer/EditRussianBankAccount",
                    data: { producerId: $('#Id').val(), bankAccountId: accountId },
                    success: function (result) {
                        $('#producerBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#producerBankAccountDetailsForEdit"));
                        ShowModal("producerBankAccountDetailsForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            $('#gridForeignBankAccounts .edit_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();

                $.ajax({
                    type: "GET",
                    url: "/Producer/EditForeignBankAccount",
                    data: { producerId: $('#Id').val(), bankAccountId: accountId },
                    success: function (result) {
                        $('#producerForeignBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#producerForeignBankAccountDetailsForEdit"));
                        ShowModal("producerForeignBankAccountDetailsForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            $('#gridRussianBankAccounts .delete_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();

                if (confirm('Вы уверены?')) {
                    StartGridProgress($(this).closest(".grid"));

                    $.ajax({
                        type: "POST",
                        url: "/Producer/RemoveRussianBankAccount",
                        data: { producerId: $('#Id').val(), bankAccountId: accountId },
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

            $('#gridForeignBankAccounts .delete_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();

                if (confirm('Вы уверены?')) {
                    StartGridProgress($(this).closest(".grid"));

                    $.ajax({
                        type: "POST",
                        url: "/Producer/RemoveForeignBankAccount",
                        data: { producerId: $('#Id').val(), bankAccountId: accountId },
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


            $("#btnBackTo").live('click', function () {
                window.location = $('#BackURL').val();
            });

            $('#btnEdit').live("click", function () {
                var id = $('#Id').val();
                window.location = "/Producer/Edit?id=" + id + GetBackUrl();
            });

            $('#btnDelete').live("click", function () {
                if (confirm('Вы уверены?')) {
                    var id = $('#Id').val();

                    $.ajax({
                        type: "POST",
                        url: "/Producer/Delete/",
                        data: { producerId: id },
                        success: function () {
                            window.location = "/Producer/List";
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProducerEdit");
                        }
                    });
                }
            });
        });

        $("#productionOrderPaymentEdit #linkChangePaymentCurrencyRate").live("click", function () {
            var currencyId = $("#productionOrderPaymentEdit #PaymentCurrencyId").val();
            if (!IsDefaultOrEmpty(currencyId)) {
                $.ajax({
                    type: "GET",
                    url: "/Currency/SelectCurrencyRate",
                    data: { currencyId: currencyId, selectFunctionName: "OnProductionOrderPaymentEditCurrencyRateSelectLinkClick" },
                    success: function (result) {
                        $("#currencyRateSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyRateSelector"));
                        ShowModal("currencyRateSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentEdit");
                    }
                });
            }
        });
    },

    RefreshMainDetails: function (obj) {
        $("#OrderSum").text(obj.OrderSum);
        $("#OpenOrderSum").text(obj.OpenOrderSum);
        $("#ProductionSum").text(obj.ProductionSum);
        $("#PaymentSum").text(obj.PaymentSum);
    },

    OnSuccessRussianBankAccountEdit: function (ajaxContext) {
        HideModal(function () {
            RefreshGrid("gridRussianBankAccounts", function () {
                ShowSuccessMessage('Сохранено.', 'messageRussianBankAccountList');
            });
        });
    },

    OnSuccessForeignBankAccountEdit: function (ajaxContext) {
        HideModal(function () {
            RefreshGrid("gridForeignBankAccounts", function () {
                ShowSuccessMessage('Сохранено.', 'messageForeignBankAccountList');
            });
        });
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора курсов валюты из формы редактирования оплаты
    OnProductionOrderPaymentEditCurrencyRateSelectLinkClick: function (currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        var productionOrderPaymentId = $("#productionOrderPaymentEdit #ProductionOrderPaymentId").val();
        $.ajax({
            type: "POST",
            url: "/ProductionOrderPayment/ChangeProductionOrderPaymentCurrencyRate",
            data: { productionOrderPaymentId: productionOrderPaymentId, currencyRateId: currencyRateId },
            success: function (result) {
                // Обновление модальной формы
                $("#productionOrderPaymentEdit #PaymentCurrencyRateId").val(productionOrderPaymentId);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateName").text(result.PaymentCurrencyRateName);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateString").text(result.PaymentCurrencyRateString);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val(result.PaymentCurrencyRateValue);
                ProductionOrder_Details.RecalculateProductionOrderPaymentSumInBaseCurrency();

                RefreshGrid("gridProducerPayments", function () {
                    var productionOrderPlannedPaymentId = $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentId").val();
                    var producerId = $('#Id').val();
                    $.ajax({
                        type: "GET",
                        url: "/Producer/GetMainChangeableIndicators",
                        data: { producerId: producerId },
                        success: function (result) {
                            Producer_Details.RefreshMainDetails(result);
                            // Если плановый платеж указан, то ...
                            if (productionOrderPaymentId != "00000000-0000-0000-0000-000000000000") {
                                // ... запрашиваем детали плановой оплаты
                                $.ajax({
                                    type: "POST",
                                    url: "/ProductionOrder/GetPlannedPaymentInfo",
                                    data: { productionOrderPlannedPaymentId: productionOrderPlannedPaymentId },
                                    success: function (result) {
                                        // Обновление полей
                                        $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentSumInCurrency").text(result.PlannedPaymentSumInCurrency);
                                        $("#productionOrderPaymentEdit .ProductionOrderPlannedPaymentCurrencyLiteralCode").text(result.PlannedPaymentCurrencyLiteralCode);
                                        $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentPaidSumInBaseCurrency").text(result.PaymentSumInBaseCurrency);

                                        HideModal(function () {
                                            ShowSuccessMessage("Курс оплаты сохранен.", "messageProductionOrderPaymentEdit");
                                        });
                                    },
                                    error: function (XMLHttpRequest, textStatus, thrownError) {
                                        ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyEdit");
                                    }
                                });
                            } else {    // иначе закрываем МФ
                                HideModal(function () {
                                    ShowSuccessMessage("Курс оплаты сохранен.", "messageProductionOrderPaymentEdit");
                                });
                            }

                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyEdit");
                        }
                    }); // end AJAX

                }); // end RefreshGrid
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyEdit");
            }
        });
    }
};