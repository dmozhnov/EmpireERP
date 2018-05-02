var MovementWaybill_Details = {
    Init: function () {
        $(document).ready(function () {

            $('#btnAddRowsByList').live("click", function () {
                var id = $('#Id').val();
                window.location = "/MovementWaybill/AddRowsByList?id=" + id + GetBackUrl();
            });

            $('#printingFormSenderCost').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/ShowPrintingFormSettings/",
                    data: { PrintSenderPrice: true,
                        PrintRecepientPrice: false,
                        PrintMarkup: false
                    },
                    success: function (result) {
                        $('#movementWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#movementWaybillPrintingForm"));
                        ShowModal("movementWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            $('#printingFormReceiptCost').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/ShowPrintingFormSettings/",
                    data: { PrintSenderPrice: false,
                        PrintRecepientPrice: true,
                        PrintMarkup: false
                    },
                    success: function (result) {
                        $('#movementWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#movementWaybillPrintingForm"));
                        ShowModal("movementWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            $('#printingFormReceiptAllCost').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/ShowPrintingFormSettings/",
                    data: { PrintSenderPrice: true,
                        PrintRecepientPrice: true,
                        PrintMarkup: true
                    },
                    success: function (result) {
                        $('#movementWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#movementWaybillPrintingForm"));
                        ShowModal("movementWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            $('#printingFormTORG12').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/ShowTORG12PrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#movementWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#movementWaybillPrintingForm"));
                        ShowModal("movementWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            // T-1 (TTH)
            $('#printingFormT1').live('click', function () {
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/GetT1PrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#movementWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#movementWaybillPrintingForm"));
                        ShowModal("movementWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            $('#cashMemoPrintingForm').click('click', function () {
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/ShowCashMemoPrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#movementWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#movementWaybillPrintingForm"));
                        ShowModal("movementWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            $('#invoicePrintingForm').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/ShowInvoicePrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#movementWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#movementWaybillPrintingForm"));
                        ShowModal("movementWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            // Подготовить накладную к проводке
            $("#btnPrepareToAcceptMovementWaybill").click(function () {
                var movementWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/MovementWaybill/PrepareToAccept/",
                    data: { id: movementWaybillId },
                    success: function (result) {
                        RefreshGrid("gridMovementWaybillRows", function () {
                            MovementWaybill_Details.RefreshMainDetails(result);
                            ShowSuccessMessage("Накладная подготовлена к проводке.", "messageMovementWaybillDetails");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            // Отменить готовность к проводке
            $('#btnCancelReadinessToAcceptMovementWaybill').click(function () {
                if (confirm('Вы действительно хотите отменить готовность накладной к проводке?')) {
                    var movementWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/MovementWaybill/CancelReadinessToAccept/",
                        data: { id: movementWaybillId },
                        success: function (result) {
                            RefreshGrid("gridMovementWaybillRows", function () {
                                MovementWaybill_Details.RefreshMainDetails(result);
                                ShowSuccessMessage("Готовность накладной к проводке отменена.", "messageMovementWaybillDetails");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                        }
                    });
                }
            });

            // Отгрузить
            $('#btnShipMovementWaybill').click(function () {
                var movementWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/MovementWaybill/Ship/",
                    data: { id: movementWaybillId },
                    success: function (result) {
                        RefreshGrid("gridMovementWaybillRows", function () {
                            MovementWaybill_Details.RefreshMainDetails(result);
                            ShowSuccessMessage("Отгрузка произведена.", "messageMovementWaybillDetails");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            // Отменить отгрузку
            $('#btnCancelShippingMovementWaybill').click(function () {
                if (confirm('Вы действительно хотите отменить отгрузку накладной?')) {
                    var movementWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/MovementWaybill/CancelShipping/",
                        data: { id: movementWaybillId },
                        success: function (result) {
                            RefreshGrid("gridMovementWaybillRows", function () {
                                MovementWaybill_Details.RefreshMainDetails(result);
                                ShowSuccessMessage("Отмена отгрузки произведена.", "messageMovementWaybillDetails");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                        }
                    });
                }
            });

            // Принять
            $('#btnReceiptMovementWaybill').click(function () {
                var movementWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/MovementWaybill/Receipt/",
                    data: { id: movementWaybillId },
                    success: function (result) {
                        RefreshGrid("gridMovementWaybillRows", function () {
                            MovementWaybill_Details.RefreshMainDetails(result);
                            ShowSuccessMessage("Приемка произведена.", "messageMovementWaybillDetails");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            // Отменить приемку
            $('#btnCancelReceiptMovementWaybill').click(function () {
                if (confirm('Вы действительно хотите отменить приемку накладной?')) {
                    var movementWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/MovementWaybill/CancelReceipt/",
                        data: { id: movementWaybillId },
                        success: function (result) {
                            RefreshGrid("gridMovementWaybillRows", function () {
                                MovementWaybill_Details.RefreshMainDetails(result);
                                ShowSuccessMessage("Отмена приемки произведена.", "messageMovementWaybillDetails");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                        }
                    });
                }
            });

            // Провести
            $('#btnAcceptMovementWaybill').click(function () {
                var movementWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/MovementWaybill/Accept/",
                    data: { id: movementWaybillId },
                    success: function (result) {
                        RefreshGrid("gridMovementWaybillRows", function () {
                            MovementWaybill_Details.RefreshMainDetails(result);
                            ShowSuccessMessage("Проводка произведена.", "messageMovementWaybillDetails");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            });

            // Отменить проводку
            $('#btnCancelAcceptanceMovementWaybill').click(function () {
                if (confirm('Вы действительно хотите отменить проводку накладной?')) {
                    var movementWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/MovementWaybill/CancelAcceptance/",
                        data: { id: movementWaybillId },
                        success: function (result) {
                            RefreshGrid("gridMovementWaybillRows", function () {
                                MovementWaybill_Details.RefreshMainDetails(result);
                                ShowSuccessMessage("Отмена проводки произведена.", "messageMovementWaybillDetails");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                        }
                    });
                }
            });

            // Изменить куратора
            $("#linkChangeCurator").click(function () {
                var storageIds = $("#SenderStorageId").val() + "_" + $("#RecipientStorageId").val();
                Waybill_Edit.ShowCuratorSelectorForm(2/*waybillTypeId*/, storageIds, "", null, "messageMovementWaybillDetails");
            });

            // обработка выбора куратора
            $(".select_user").live("click", function () {
                Waybill_Details.HandlerForSelectCurator(2/*waybillTypeId*/, $(this));
            });

        });         // document ready

        $("#btnBackTo").live('click', function () {
            window.location = $('#BackURL').val();
        });

        // Редактировать
        $('#btnEditMovementWaybill').live("click", function () {
            var movementWaybillId = $('#Id').val();
            window.location = "/MovementWaybill/Edit?id=" + movementWaybillId + GetBackUrl();
        });

        // Удалить
        $('#btnDeleteMovementWaybill').live('click', function () {
            if (confirm('Вы уверены?')) {
                var movementWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/MovementWaybill/Delete/",
                    data: { id: movementWaybillId },
                    success: function () {
                        window.location = "/MovementWaybill/List";
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillDetails");
                    }
                });
            }
        });
    },

    OnSuccessMovementWaybillRowEdit: function (ajaxContext) {
        if ($('#movementWaybillRowEdit #Id').val() != "00000000-0000-0000-0000-000000000000") {
            // грид для формы добавления товаров списком
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridArticleGroups", function () {
                    RefreshGrid("gridMovementWaybillRows", function () {
                        HideModal(function () {
                            ShowSuccessMessage("Сохранено.", "messageMovementWaybillRowList");
                        });
                    });
                });

            });
        }
        else {
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridMovementWaybillRows", function () {
                    RefreshGrid("gridArticleGroups", function () {
                        MovementWaybill_Shared.ClearForm();
                        ShowSuccessMessage("Сохранено.", "messageMovementWaybillRowEdit");
                    });
                });
            });
        }
        MovementWaybill_Details.RefreshMainDetails(ajaxContext);
    },

    // обновление основной информации о накладной и состояний кнопок
    RefreshMainDetails: function (details) {
        $("#StateName").text(details.MainDetails.StateName);
        $("#PurchaseCostSum").text(details.MainDetails.PurchaseCostSum);
        $("#SenderAccountingPriceSum").text(details.MainDetails.SenderAccountingPriceSum);
        $("#RecipientAccountingPriceSum").text(details.MainDetails.RecipientAccountingPriceSum);
        $("#MovementMarkupPercent").text(details.MainDetails.MovementMarkupPercent);
        $("#MovementMarkupSum").text(details.MainDetails.MovementMarkupSum);
        $("#RowCount").text(details.MainDetails.RowCount);
        $("#ShippingPercent").text(details.MainDetails.ShippingPercent);
        $("#RecipientStorageName").text(details.MainDetails.RecipientStorageName);
        $("#RecipientName").text(details.MainDetails.RecipientName);
        $("#SenderValueAddedTaxString").text(details.MainDetails.SenderValueAddedTaxString);
        $("#RecipientValueAddedTaxString").text(details.MainDetails.RecipientValueAddedTaxString);
        $("#TotalWeight").text(details.MainDetails.TotalWeight);
        $("#TotalVolume").text(details.MainDetails.TotalVolume);

        $("#AcceptedByName").text(details.MainDetails.AcceptedByName);
        $("#AcceptedById").val(details.MainDetails.AcceptedById);
        $("#AcceptanceDate").text(details.MainDetails.AcceptanceDate);
        $("#ShippedByName").text(details.MainDetails.ShippedByName);
        $("#ShippedById").val(details.MainDetails.ShippedById);
        $("#ShippingDate").text(details.MainDetails.ShippingDate);
        $("#ReceiptedByName").text(details.MainDetails.ReceiptedByName);
        $("#ReceiptedById").val(details.MainDetails.ReceiptedById);
        $("#ReceiptDate").text(details.MainDetails.ReceiptDate);

        $("#AllowToViewAcceptedByDetails").val(details.MainDetails.AllowToViewAcceptedByDetails);
        $("#AllowToViewShippedByDetails").val(details.MainDetails.AllowToViewShippedByDetails);
        $("#AllowToViewReceiptedByDetails").val(details.MainDetails.AllowToViewReceiptedByDetails);

        $("#AcceptedByContainer").css("display", details.MainDetails.AcceptedById != "" ? "inline" : "none");
        $("#ShippedByContainer").css("display", details.MainDetails.ShippedById != "" ? "inline" : "none");
        $("#ReceiptedByContainer").css("display", details.MainDetails.ReceiptedById != "" ? "inline" : "none");

        SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');
        SetEntityDetailsLink('AllowToViewShippedByDetails', 'ShippedByName', 'User', 'ShippedById');
        SetEntityDetailsLink('AllowToViewReceiptedByDetails', 'ReceiptedByName', 'User', 'ReceiptedById');

        UpdateButtonAvailability("btnPrepareToAcceptMovementWaybill", details.IsPossibilityToPrepareToAccept);
        UpdateButtonAvailability("btnCancelReadinessToAcceptMovementWaybill", details.AllowToCancelReadinessToAccept);
        UpdateButtonAvailability("btnEditMovementWaybill", details.AllowToEdit);
        UpdateButtonAvailability("btnAddMovementWaybillRow", details.AllowToAddRow);
        UpdateButtonAvailability("btnDeleteMovementWaybill", details.AllowToDelete);
        UpdateButtonAvailability("btnAcceptMovementWaybill", details.IsPossibilityToAccept);
        UpdateButtonAvailability("btnCancelAcceptanceMovementWaybill", details.AllowToCancelAcceptance);
        UpdateButtonAvailability("btnShipMovementWaybill", details.IsPossibilityToShip);
        UpdateButtonAvailability("btnCancelShippingMovementWaybill", details.AllowToCancelShipping);
        UpdateButtonAvailability("btnReceiptMovementWaybill", details.AllowToReceipt);
        UpdateButtonAvailability("btnCancelReceiptMovementWaybill", details.AllowToCancelReceipt);
        UpdateButtonAvailability("btnAddRowsByList", details.AllowToEdit);

        UpdateElementVisibility("btnPrepareToAcceptMovementWaybill", details.AllowToPrepareToAccept);
        UpdateElementVisibility("btnCancelReadinessToAcceptMovementWaybill", details.AllowToCancelReadinessToAccept);
        UpdateElementVisibility("btnEditMovementWaybill", details.AllowToEdit);
        UpdateElementVisibility("btnAddMovementWaybillRow", details.AllowToAddRow);
        UpdateElementVisibility("btnDeleteMovementWaybill", details.AllowToDelete);
        UpdateElementVisibility("btnAcceptMovementWaybill", details.AllowToAccept);
        UpdateElementVisibility("btnCancelAcceptanceMovementWaybill", details.AllowToCancelAcceptance);
        UpdateElementVisibility("btnShipMovementWaybill", details.AllowToShip);
        UpdateElementVisibility("btnCancelShippingMovementWaybill", details.AllowToCancelShipping);
        UpdateElementVisibility("btnReceiptMovementWaybill", details.AllowToReceipt);
        UpdateElementVisibility("btnCancelReceiptMovementWaybill", details.AllowToCancelReceipt);
        UpdateElementVisibility("btnAddRowsByList", details.AllowToEdit);

        UpdateElementVisibility("feature_menu_box", details.AllowToPrintForms);

        UpdateElementVisibility("cashMemoPrintingForm", details.AllowToPrintCashMemoForm);
        UpdateElementVisibility("invoicePrintingForm", details.AllowToPrintInvoiceForm);
        UpdateElementVisibility("printingFormSenderCost", details.AllowToPrintWaybillFormInSenderPrices);
        UpdateElementVisibility("printingFormReceiptCost", details.AllowToPrintWaybillFormInRecipientPrices);
        UpdateElementVisibility("printingFormReceiptAllCost", details.AllowToPrintWaybillFormInBothPrices);
        UpdateElementVisibility("printingFormTORG12", details.AllowToPrintTORG12Form);

        UpdateElementVisibility("linkChangeCurator", details.AllowToChangeCurator);
    }
};