var ReturnFromClientWaybill_Details = {
    Init: function () {
        $(document).ready(function () {
            $("#btnBackTo").live('click', function () {
                window.location = $('#BackURL').val();
            });

            $('#btnEdit').live("click", function () {
                window.location = "/ReturnFromClientWaybill/Edit?id=" + $('#Id').val() + GetBackUrl();
            });

            $('#btnDelete').live('click', function () {
                if (confirm('Вы уверены?')) {
                    var returnFromClientWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReturnFromClientWaybill/Delete/",
                        data: { id: returnFromClientWaybillId },
                        success: function () {
                            window.location = "/ReturnFromClientWaybill/List";
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                        }
                    });
                }
            });

            $("#btnPrepareToAccept").live('click', function () {
                var returnFromClientWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ReturnFromClientWaybill/PrepareToAccept",
                    data: { returnFromClientWaybillId: returnFromClientWaybillId },
                    success: function (result) {
                        RefreshGrid("gridReturnFromClientWaybillRows", function () {
                            ReturnFromClientWaybill_Details.RefreshMainDetails(result.MainDetails);
                            ReturnFromClientWaybill_Details.RefreshPermissions(result.Permissions);
                            ShowSuccessMessage("Накладная подготовлена к проводке.", "messageReturnFromClientWaybillEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                    }
                });
            });

            $("#btnCancelReadinessToAccept").live('click', function () {
                if (confirm('Вы уверены?')) {
                    var returnFromClientWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReturnFromClientWaybill/CancelReadinessToAccept",
                        data: { returnFromClientWaybillId: returnFromClientWaybillId },
                        success: function (result) {
                            RefreshGrid("gridReturnFromClientWaybillRows", function () {
                                ReturnFromClientWaybill_Details.RefreshMainDetails(result.MainDetails);
                                ReturnFromClientWaybill_Details.RefreshPermissions(result.Permissions);
                                ShowSuccessMessage("Готовность накладной к проводке отменена.", "messageReturnFromClientWaybillEdit");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                        }
                    });
                }
            });

            $("#btnAccept").live('click', function () {
                var returnFromClientWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ReturnFromClientWaybill/Accept",
                    data: { returnFromClientWaybillId: returnFromClientWaybillId },
                    success: function (result) {
                        RefreshGrid("gridReturnFromClientWaybillRows", function () {
                            ReturnFromClientWaybill_Details.RefreshMainDetails(result.MainDetails);
                            ReturnFromClientWaybill_Details.RefreshPermissions(result.Permissions);
                            ShowSuccessMessage("Накладная проведена.", "messageReturnFromClientWaybillEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                    }
                });
            });

            $("#btnCancelAcceptance").live('click', function () {
                if (confirm('Вы уверены?')) {
                    var returnFromClientWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReturnFromClientWaybill/CancelAcceptance",
                        data: { returnFromClientWaybillId: returnFromClientWaybillId },
                        success: function (result) {
                            RefreshGrid("gridReturnFromClientWaybillRows", function () {
                                ReturnFromClientWaybill_Details.RefreshMainDetails(result.MainDetails);
                                ReturnFromClientWaybill_Details.RefreshPermissions(result.Permissions);
                                ShowSuccessMessage("Проводка накладной отменена.", "messageReturnFromClientWaybillEdit");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                        }
                    });
                }
            });

            $("#btnReceipt").live('click', function () {
                var returnFromClientWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ReturnFromClientWaybill/Receipt",
                    data: { returnFromClientWaybillId: returnFromClientWaybillId },
                    success: function (result) {
                        ReturnFromClientWaybill_Details.RefreshMainDetails(result.MainDetails);
                        ReturnFromClientWaybill_Details.RefreshPermissions(result.Permissions);
                        ShowSuccessMessage("Накладная принята.", "messageReturnFromClientWaybillEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                    }
                });
            });

            $("#btnCancelReceipt").live('click', function () {
                if (confirm('Вы уверены?')) {
                    var returnFromClientWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReturnFromClientWaybill/CancelReceipt",
                        data: { returnFromClientWaybillId: returnFromClientWaybillId },
                        success: function (result) {
                            ReturnFromClientWaybill_Details.RefreshMainDetails(result.MainDetails);
                            ReturnFromClientWaybill_Details.RefreshPermissions(result.Permissions);
                            ShowSuccessMessage("Приемка накладной отменена.", "messageReturnFromClientWaybillEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                        }
                    });
                }
            });

            // ТОРГ-12
            $('#printingFormTORG12').live('click', function () {
                $.ajax({
                    type: "GET",
                    url: "/ReturnFromClientWaybill/ShowTORG12PrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#returnFromClientWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#returnFromClientWaybillPrintingForm"));
                        ShowModal("returnFromClientWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                    }
                });
            });

            // Изменить куратора
            $("#linkChangeCurator").click(function () {
                var dealId = $("#DealId").val();
                var storageId = $("#RecipientStorageId").val();

                Waybill_Edit.ShowCuratorSelectorForm(6/*WaybillTypeId*/, storageId, dealId, null, "messageReturnFromClientWaybillEdit");
            });

            // обработка выбора куратора
            $(".select_user").live("click", function () {
                Waybill_Details.HandlerForSelectCurator(6/*waybillTypeId*/, $(this));
            });
        });
    },

    RefreshMainDetails: function (details) {
        $("#StateName").text(details.StateName);
        $("#PurchaseCostSum").text(details.PurchaseCostSum);
        $("#SalePriceSum").text(details.SalePriceSum);
        $("#AccountingPriceSum").text(details.AccountingPriceSum);
        $("#RowCount").text(details.RowCount);

        $("#CuratorName").text(details.CuratorName);
        $("#CuratorId").val(details.CuratorId);
        $("#AcceptedByName").text(details.AcceptedByName);
        $("#AcceptedById").val(details.AcceptedById);
        $("#AcceptanceDate").text(details.AcceptanceDate);
        $("#ReceiptedByName").text(details.ReceiptedByName);
        $("#ReceiptedById").val(details.ReceiptedById);
        $("#ReceiptDate").text(details.ReceiptDate);

        $("#AllowToViewAcceptedByDetails").val(details.AllowToViewAcceptedByDetails);
        $("#AllowToViewReceiptedByDetails").val(details.AllowToViewReceiptedByDetails);

        $("#AcceptedByContainer").css("display", details.AcceptedById != "" ? "inline" : "none");
        $("#ReceiptedByContainer").css("display", details.ReceiptedById != "" ? "inline" : "none");

        SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');
        SetEntityDetailsLink('AllowToViewReceiptedByDetails', 'ReceiptedByName', 'User', 'ReceiptedById');

        $("#TotalWeight").text(details.TotalWeight);
        $("#TotalVolume").text(details.TotalVolume);
    },

    RefreshPermissions: function (permissions) {
        UpdateButtonAvailability("btnPrepareToAccept", permissions.AllowToPrepareToAccept);
        UpdateElementVisibility("btnPrepareToAccept", permissions.IsPossibilityToPrepareToAccept);

        UpdateButtonAvailability("btnCancelReadinessToAccept", permissions.AllowToCancelReadinessToAccept);
        UpdateElementVisibility("btnCancelReadinessToAccept", permissions.AllowToCancelReadinessToAccept);

        UpdateButtonAvailability("btnAccept", permissions.AllowToAccept);
        UpdateElementVisibility("btnAccept", permissions.IsPossibilityToAccept);

        UpdateButtonAvailability("btnCancelAcceptance", permissions.AllowToCancelAcceptance);
        UpdateElementVisibility("btnCancelAcceptance", permissions.AllowToCancelAcceptance);

        UpdateButtonAvailability("btnCancelReceipt", permissions.AllowToCancelReceipt);
        UpdateElementVisibility("btnCancelReceipt", permissions.AllowToCancelReceipt);

        UpdateButtonAvailability("btnDelete", permissions.AllowToDelete);
        UpdateElementVisibility("btnDelete", permissions.AllowToDelete);

        UpdateButtonAvailability("btnEdit", permissions.AllowToEdit);
        UpdateElementVisibility("btnEdit", permissions.AllowToEdit);

        UpdateButtonAvailability("btnCreateRow", permissions.AllowToEdit);
        UpdateElementVisibility("btnCreateRow", permissions.AllowToEdit);

        UpdateButtonAvailability("btnReceipt", permissions.AllowToReceipt);
        UpdateElementVisibility("btnReceipt", permissions.AllowToReceipt);

        UpdateElementVisibility("linkChangeCurator", permissions.AllowToChangeCurator);

        UpdateElementVisibility("feature_menu_box", permissions.AllowToPrintForms);
    }
};