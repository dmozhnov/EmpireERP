var ExpenditureWaybill_Details = {
    Init: function () {
        $("#btnBackTo").live('click', function () {
            window.location = $('#BackURL').val();
        });

        $('#btnEdit').live("click", function () {
            var id = $('#Id').val();
            window.location = "/ExpenditureWaybill/Edit?id=" + id + GetBackUrl();
        });

        $('#btnAddRowsByList').live("click", function () {
            var id = $('#Id').val();
            window.location = "/ExpenditureWaybill/AddRowsByList?id=" + id + GetBackUrl();
        });

        $('#btnDelete').live('click', function () {
            if (confirm('Вы уверены?')) {
                var expenditureWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ExpenditureWaybill/Delete/",
                    data: { id: expenditureWaybillId },
                    success: function () {
                        window.location = "/ExpenditureWaybill/List";
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                    }
                });
            }
        });

        $("#btnPrepareToAccept").live('click', function () {
            var expenditureWaybillId = $('#Id').val();

            StartButtonProgress($(this));
            $.ajax({
                type: "POST",
                url: "/ExpenditureWaybill/PrepareToAccept",
                data: { expenditureWaybillId: expenditureWaybillId },
                success: function (result) {
                    RefreshGrid("gridExpenditureWaybillRows", function () {
                        ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                        ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                        ShowSuccessMessage("Накладная подготовлена к проводке.", "messageExpenditureWaybillEdit");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        $("#btnCancelReadinessToAccept").live('click', function () {
            var expenditureWaybillId = $('#Id').val();

            StartButtonProgress($(this));
            $.ajax({
                type: "POST",
                url: "/ExpenditureWaybill/CancelReadinessToAccept",
                data: { expenditureWaybillId: expenditureWaybillId },
                success: function (result) {
                    RefreshGrid("gridExpenditureWaybillRows", function () {
                        ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                        ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                        ShowSuccessMessage("Готовность к проводке отменена.", "messageExpenditureWaybillEdit");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        $("#btnAccept").live('click', function () {
            var expenditureWaybillId = $('#Id').val();

            StartButtonProgress($(this));
            $.ajax({
                type: "POST",
                url: "/ExpenditureWaybill/Accept",
                data: { expenditureWaybillId: expenditureWaybillId },
                success: function (result) {
                    RefreshGrid("gridExpenditureWaybillRows", function () {
                        ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                        ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                        ShowSuccessMessage("Накладная проведена.", "messageExpenditureWaybillEdit");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        $("#btnAcceptRetroactively").live('click', function () {
            StartButtonProgress($(this));
            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/AcceptRetroactively",
                success: function (result) {
                    $('#dateTimeSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#dateTimeSelector"));
                    ShowModal("dateTimeSelector");

                    BindRetroactivelyAcceptanceDateSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        function BindRetroactivelyAcceptanceDateSelection() {
            $('#btnSelectDateTime').click(function (e) {

                e.preventDefault();
                if (!$('#dateTimeSelectForm').valid()) return false;

                var dateTime = $("#dateTimeSelector #Date").val() + " " + $("#dateTimeSelector #Time").val();
                var expenditureWaybillId = $('#Id').val();

                StartButtonProgress($("#btnSelectDateTime"));

                $.ajax({
                    type: "POST",
                    url: "/ExpenditureWaybill/AcceptRetroactively",
                    data: { expenditureWaybillId: expenditureWaybillId, acceptanceDate: dateTime },
                    success: function (result) {
                        RefreshGrid("gridExpenditureWaybillRows", function () {
                            ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                            ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                            HideModal(function () {
                                ShowSuccessMessage("Накладная проведена.", "messageExpenditureWaybillEdit");
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDateSelect");
                    }
                });
            });
        }

        $("#btnCancelAcceptance").live('click', function () {
            if (confirm('Вы уверены?')) {
                var expenditureWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ExpenditureWaybill/CancelAcceptance",
                    data: { expenditureWaybillId: expenditureWaybillId },
                    success: function (result) {
                        RefreshGrid("gridExpenditureWaybillRows", function () {
                            ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                            ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                            ShowSuccessMessage("Проводка накладной отменена.", "messageExpenditureWaybillEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                    }
                });
            }
        });

        $("#btnShip").live('click', function () {
            var expenditureWaybillId = $('#Id').val();

            StartButtonProgress($(this));
            $.ajax({
                type: "POST",
                url: "/ExpenditureWaybill/Ship",
                data: { expenditureWaybillId: expenditureWaybillId },
                success: function (result) {
                    ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                    ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                    ShowSuccessMessage("Накладная отгружена.", "messageExpenditureWaybillEdit");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        $("#btnShipRetroactively").live('click', function () {
            StartButtonProgress($(this));
            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/ShipRetroactively",
                success: function (result) {
                    $('#dateTimeSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#dateTimeSelector"));
                    ShowModal("dateTimeSelector");

                    BindRetroactivelyShippingDateSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        function BindRetroactivelyShippingDateSelection() {
            $('#btnSelectDateTime').click(function (e) {

                e.preventDefault();
                if (!$('#dateTimeSelectForm').valid()) return false;

                var dateTime = $("#dateTimeSelector #Date").val() + " " + $("#dateTimeSelector #Time").val();
                var expenditureWaybillId = $('#Id').val();

                StartButtonProgress($("#btnSelectDateTime"));

                $.ajax({
                    type: "POST",
                    url: "/ExpenditureWaybill/ShipRetroactively",
                    data: { expenditureWaybillId: expenditureWaybillId, shippingDate: dateTime },
                    success: function (result) {
                        RefreshGrid("gridExpenditureWaybillRows", function () {
                            ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                            ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                            HideModal(function () {
                                ShowSuccessMessage("Накладная отгружена.", "messageExpenditureWaybillEdit");
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDateSelect");
                    }
                });
            });
        }

        $("#btnCancelShipping").live('click', function () {
            if (confirm('Вы уверены?')) {
                var expenditureWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ExpenditureWaybill/CancelShipping",
                    data: { expenditureWaybillId: expenditureWaybillId },
                    success: function (result) {
                        ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                        ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                        ShowSuccessMessage("Отгрузка накладной отменена.", "messageExpenditureWaybillEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                    }
                });
            }
        });

        $('#cashMemoPrintingForm').live('click', function () {
            var id = $('#Id').val();

            window.open("/ExpenditureWaybill/ShowCashMemoPrintingForm?waybillId=" + id);
        });

        // счет-фактура
        $('#invoicePrintingForm').live("click", function () {
            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/ShowInvoicePrintingFormSettings/",
                data: { waybillId: $('#Id').val() },
                success: function (result) {
                    $('#expenditureWaybillPrintingForm').hide().html(result);
                    $.validator.unobtrusive.parse($("#expenditureWaybillPrintingForm"));
                    ShowModal("expenditureWaybillPrintingForm");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        $('#paymentInvoicePrintingForm').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/ShowPaymentInvoicePrintingFormSettings/",
                data: { waybillId: $('#Id').val() },
                success: function (result) {
                    $('#expenditureWaybillPrintingForm').hide().html(result);
                    $.validator.unobtrusive.parse($("#expenditureWaybillPrintingForm"));
                    ShowModal("expenditureWaybillPrintingForm");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        $('#printingForm').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/ShowExpenditureWaybillPrintingFormSettings/",
                data: { waybillId: $('#Id').val() },
                success: function (result) {
                    $('#expenditureWaybillPrintingForm').hide().html(result);
                    $.validator.unobtrusive.parse($("#expenditureWaybillPrintingForm"));
                    ShowModal("expenditureWaybillPrintingForm");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        // ТОРГ-12
        $('#printingFormTORG12').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/ShowTORG12PrintingFormSettings/",
                data: { waybillId: $('#Id').val() },
                success: function (result) {
                    $('#expenditureWaybillPrintingForm').hide().html(result);
                    $.validator.unobtrusive.parse($("#expenditureWaybillPrintingForm"));
                    ShowModal("expenditureWaybillPrintingForm");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        // T-1 (TTH)
        $('#printingFormT1').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ExpenditureWaybill/GetT1PrintingFormSettings/",
                data: { waybillId: $('#Id').val() },
                success: function (result) {
                    $('#expenditureWaybillPrintingForm').hide().html(result);
                    $.validator.unobtrusive.parse($("#expenditureWaybillPrintingForm"));
                    ShowModal("expenditureWaybillPrintingForm");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                }
            });
        });

        // Изменить куратора
        $("#linkChangeCurator").live("click", function () {
            var dealId = $("#DealId").val();
            Waybill_Edit.ShowCuratorSelectorForm(4/*WaybillTypeId*/, "", dealId, null, "messageChangeOwnerWaybillDetails");
        });

        // обработка выбора куратора
        $(".select_user").live("click", function () {
            Waybill_Details.HandlerForSelectCurator(4/*waybillTypeId*/, $(this));
        });
    },

    // обновление основной информации о накладной и состоянии кнопок
    RefreshMainDetails: function (details) {
        $("#AcceptedByName").text(details.AcceptedByName);
        $("#AcceptedById").val(details.AcceptedById);
        $("#AcceptanceDate").text(details.AcceptanceDate);
        $("#ShippedByName").text(details.ShippedByName);
        $("#ShippedById").val(details.ShippedById);
        $("#ShippingDate").text(details.ShippingDate);
        $("#StateName").text(details.StateName);
        $("#PurchaseCostSum").text(details.PurchaseCostSum);
        $("#ClientName").text(details.ClientName);
        $("#SenderAccountingPriceSum").text(details.SenderAccountingPriceSum);
        $("#DealName").text(details.DealName);
        $("#SalePriceSum").text(details.SalePriceSum);
        $("#DealQuotaName").text(details.DealQuotaName);
        $("#TotalDiscountPercent").text(details.TotalDiscountPercent);
        $("#TotalDiscountSum").text(details.TotalDiscountSum);
        $("#SenderStorageName").text(details.SenderStorageName);
        $("#MarkupPercent").text(details.MarkupPercent);
        $("#MarkupSum").text(details.MarkupSum);
        $("#PaymentPercent").text(details.PaymentPercent);
        $("#PaymentSum").text(details.PaymentSum);
        $("#PaymentType").text(details.PaymentType);
        $("#RowCount").text(details.RowCount);
        $("#Comment").html(details.Comment);
        $("#ValueAddedTaxString").text(details.ValueAddedTaxString);
        $("#TotalReturnedSum").text(details.TotalReturnedSum);
        $("#TotalReservedByReturnSum").text(details.TotalReservedByReturnSum);
        $("#DealPaymentForm").text(details.DealPaymentForm);
        $("#TotalWeight").text(details.TotalWeight);
        $("#TotalVolume").text(details.TotalVolume);

        $("#AllowToViewAcceptedByDetails").val(details.AllowToViewAcceptedByDetails);
        $("#AllowToViewShippedByDetails").val(details.AllowToViewShippedByDetails);

        $("#AcceptedByContainer").css("display", details.AcceptedById != "" ? "inline" : "none");
        $("#ShippedByContainer").css("display", details.ShippedById != "" ? "inline" : "none");

        SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');
        SetEntityDetailsLink('AllowToViewShippedByDetails', 'ShippedByName', 'User', 'ShippedById');
    },

    RefreshPermissions: function (permissions) {
        UpdateButtonAvailability("btnPrepareToAccept", permissions.AllowToPrepareToAccept);
        UpdateButtonAvailability("btnCancelReadinessToAccept", permissions.AllowToCancelReadinessToAccept);
        UpdateButtonAvailability("btnAccept", permissions.AllowToAccept);
        UpdateButtonAvailability("btnAcceptRetroactively", permissions.AllowToAcceptRetroactively);
        UpdateButtonAvailability("btnCancelAcceptance", permissions.AllowToCancelAcceptance);
        UpdateButtonAvailability("btnShip", permissions.AllowToShip);
        UpdateButtonAvailability("btnShipRetroactively", permissions.AllowToShipRetroactively);
        UpdateButtonAvailability("btnCancelShipping", permissions.AllowToCancelShipping);
        UpdateButtonAvailability("btnEdit", permissions.AllowToEdit);
        UpdateButtonAvailability("btnDelete", permissions.AllowToDelete);
        UpdateButtonAvailability("btnCreateExpenditureWaybillRow", permissions.AllowToEdit);
        UpdateButtonAvailability("btnAddRowsByList", permissions.AllowToEdit);

        UpdateElementVisibility("btnPrepareToAccept", permissions.IsPossibilityToPrepareToAccept);
        UpdateElementVisibility("btnCancelReadinessToAccept", permissions.AllowToCancelReadinessToAccept);
        UpdateElementVisibility("btnAccept", permissions.IsPossibilityToAccept);
        UpdateElementVisibility("btnAcceptRetroactively", permissions.IsPossibilityToAcceptRetroactively);
        UpdateElementVisibility("btnCancelAcceptance", permissions.AllowToCancelAcceptance);
        UpdateElementVisibility("btnShip", permissions.IsPossibilityToShip);
        UpdateElementVisibility("btnShipRetroactively", permissions.IsPossibilityToShipRetroactively);
        UpdateElementVisibility("btnCancelShipping", permissions.AllowToCancelShipping);
        UpdateElementVisibility("btnEdit", permissions.AllowToEdit);
        UpdateElementVisibility("btnDelete", permissions.AllowToDelete);
        UpdateElementVisibility("btnCreateExpenditureWaybillRow", permissions.AllowToEdit);
        UpdateElementVisibility("btnAddRowsByList", permissions.AllowToEdit);
        UpdateElementVisibility("feature_menu_box", permissions.AllowToPrintForms);
        UpdateElementVisibility("linkChangeDealQuota", permissions.AllowToEdit);

        UpdateElementVisibility("linkChangeCurator", permissions.AllowToChangeCurator);
    }
};
