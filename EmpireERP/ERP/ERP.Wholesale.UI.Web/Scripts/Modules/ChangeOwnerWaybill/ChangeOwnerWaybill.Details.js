var ChangeOwnerWaybill_Details = {
    Init: function () {
        $(document).ready(function () {

            $('#btnAddRowsByList').live("click", function () {
                var id = $('#Id').val();
                window.location = "/ChangeOwnerWaybill/AddRowsByList?id=" + id + GetBackUrl();
            });

            $("#btnEdit").click(function () {
                window.location = "/ChangeOwnerWaybill/Edit?id=" + $("#Id").val() + "&backURL=" + $('#currentUrl').val();
            });

            $("#btnDelete").click(function () {
                if (confirm('Вы действительно хотите удалить накладную?')) {
                    StartButtonProgress($(this));

                    $.ajax({
                        type: "GET",
                        url: "/ChangeOwnerWaybill/Delete",
                        data: { id: $("#Id").val() },
                        success: function (result) {
                            window.location = "/ChangeOwnerWaybill/List";
                        },
                        error: function (XMLHttpRequest) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                        }
                    });
                }
            });

            // счет-фактура
            $('#invoicePrintingForm').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ChangeOwnerWaybill/ShowInvoicePrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#changeOwnerWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#changeOwnerWaybillPrintingForm"));
                        ShowModal("changeOwnerWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                    }
                });
            });

            $('#cashMemoPrintingForm').live('click', function () {
                var id = $('#Id').val();

                window.open("/ChangeOwnerWaybill/ShowCashMemoPrintingForm?" + "WaybillId=" + id);
            });

            // ТОРГ-12
            $('#printingFormTORG12').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ChangeOwnerWaybill/ShowTORG12PrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#changeOwnerWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#changeOwnerWaybillPrintingForm"));
                        ShowModal("changeOwnerWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                    }
                });
            });

            // T-1 (TTH)
            $('#printingFormT1').live('click', function () {
                $.ajax({
                    type: "GET",
                    url: "/ChangeOwnerWaybill/GetT1PrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#changeOwnerWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#changeOwnerWaybillPrintingForm"));
                        ShowModal("changeOwnerWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                    }
                });
            });

            $('#printingFormAllCost').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ChangeOwnerWaybill/ShowPrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#changeOwnerWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#changeOwnerWaybillPrintingForm"));
                        ShowModal("changeOwnerWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                    }
                });
            });

            // Изменить куратора
            $("#linkChangeCurator").click(function () {
                var storageId = $("#StorageId").val();
                Waybill_Edit.ShowCuratorSelectorForm(5/*waybillTypeId*/, storageId, "", null, "messageChangeOwnerWaybillDetails");
            });

            // обработка выбора куратора
            $(".select_user").live("click", function () {
                Waybill_Details.HandlerForSelectCurator(5/*waybillTypeId*/, $(this));
            });


        }); //$(document).ready

        $("#btnBackTo").live("click", function () {
            window.location = $('#BackURL').val();
        });

        //Изменение получателя
        $("#linkChangeRecipient").live("click", function () {
            $.ajax({
                type: "GET",
                url: "/ChangeOwnerWaybill/ChangeRecipient",
                data: { id: $("#Id").val() },
                success: function (result) {
                    $('#changeOwnerWaybillChangeRecipient').hide().html(result);
                    $.validator.unobtrusive.parse($("#changeOwnerWaybillChangeRecipient"));
                    ShowModal("changeOwnerWaybillChangeRecipient");
                },
                error: function (XMLHttpRequest) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                }
            });
        });

        // подготовка к проводке
        $("#btnPrepareToAccept").live("click", function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "POST",
                url: "/ChangeOwnerWaybill/PrepareToAccept",
                data: { id: $("#Id").val() },
                success: function (result) {
                    RefreshGrid("gridChangeOwnerWaybillRow", function () {
                        ChangeOwnerWaybill_Details.RefreshMainDetails(result);
                        ChangeOwnerWaybill_Details.RefreshPermissions(result);

                        ShowSuccessMessage("Накладная подготовлена к проводке.", "messageChangeOwnerWaybillDetails");
                    });
                },
                error: function (XMLHttpRequest) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                }
            });
        });

        // отмена готовности к проводке
        $("#btnCancelReadinessToAccept").live("click", function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "POST",
                url: "/ChangeOwnerWaybill/CancelReadinessToAccept",
                data: { id: $("#Id").val() },
                success: function (result) {
                    RefreshGrid("gridChangeOwnerWaybillRow", function () {
                        ChangeOwnerWaybill_Details.RefreshMainDetails(result);
                        ChangeOwnerWaybill_Details.RefreshPermissions(result);

                        ShowSuccessMessage("Готовность к проводке отменена.", "messageChangeOwnerWaybillDetails");
                    });
                },
                error: function (XMLHttpRequest) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                }
            });
        });

        // проводка
        $("#btnAccept").live("click", function () {
            StartButtonProgress($(this));

            $.ajax({
                type: "POST",
                url: "/ChangeOwnerWaybill/Accept",
                data: { id: $("#Id").val() },
                success: function (result) {
                    RefreshGrid("gridChangeOwnerWaybillRow", function () {
                        ChangeOwnerWaybill_Details.RefreshMainDetails(result);
                        ChangeOwnerWaybill_Details.RefreshPermissions(result);

                        ShowSuccessMessage("Накладная проведена.", "messageChangeOwnerWaybillDetails");
                    });
                },
                error: function (XMLHttpRequest) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                }
            });
        });


        // отмена проводки
        $("#btnCancelAcceptance").live("click", function () {
            if (confirm('Вы действительно хотите отменить проводку накладной?')) {
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/ChangeOwnerWaybill/CancelAcceptance",
                    data: { id: $("#Id").val() },
                    success: function (result) {
                        RefreshGrid("gridChangeOwnerWaybillRow", function () {
                            ChangeOwnerWaybill_Details.RefreshMainDetails(result);
                            ChangeOwnerWaybill_Details.RefreshPermissions(result);

                            ShowSuccessMessage("Отмена проводки произведена.", "messageChangeOwnerWaybillDetails");
                        });
                    },
                    error: function (XMLHttpRequest) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillDetails");
                    }
                });
            }
        });
    },

    OnSuccessChangeOwnerWaybillRowEdit: function (ajaxContext) {
        if ($('#changeOwnerWaybillRowForEdit #ChangeOwnerWaybillRowId').val() != "00000000-0000-0000-0000-000000000000") {
            // грид для формы добавления товаров списком
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridChangeOwnerWaybillRow", function () {
                    RefreshGrid("gridArticleGroups", function () {
                        HideModal(function () {
                            ShowSuccessMessage("Сохранено.", "messageChangeOwnerWaybillRowList");
                        });
                    });
                });
            });
        }
        else {
            // грид для формы добавления товаров списком
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridChangeOwnerWaybillRow", function () {
                    RefreshGrid("gridArticleGroups", function () {
                        ChangeOwnerWaybill_Shared.ClearForm();
                        ShowSuccessMessage("Сохранено.", "messageChangeOwnerWaybillRowEdit");
                    });
                });
            });
        }
        ChangeOwnerWaybill_Details.RefreshMainDetails(ajaxContext);
        ChangeOwnerWaybill_Details.RefreshPermissions(ajaxContext);
    },

    // обновление основной информации о накладной и состояний кнопок
    RefreshMainDetails: function (details) {
        var mainDetails = details.MainDetails;

        $("#StateName").text(mainDetails.StateName);
        $("#PurchaseCostSum").text(mainDetails.PurchaseCostSum);
        $("#AccountingPriceSum").text(mainDetails.AccountingPriceSum);
        $("#RowCount").text(mainDetails.RowCount);
        $("#ShippingPercent").text(mainDetails.ShippingPercent);
        $("#ShippingReceiptDateString").text(mainDetails.ShippingReceiptDateString);
        $("#StorageName").text(mainDetails.StorageName);
        $("#RecipientName").text(mainDetails.RecipientName);
        $("#ValueAddedTaxString").text(mainDetails.ValueAddedTaxString);
        $("#TotalWeight").text(mainDetails.TotalWeight);
        $("#TotalVolume").text(mainDetails.TotalVolume);

        $("#AcceptedByName").text(mainDetails.AcceptedByName);
        $("#AcceptedById").val(mainDetails.AcceptedById);
        $("#AcceptanceDate").text(mainDetails.AcceptanceDate);
        $("#ChangedOwnerByName").text(mainDetails.ChangedOwnerByName);
        $("#ChangedOwnerById").val(mainDetails.ChangedOwnerById);
        $("#ChangeOwnerDate").text(mainDetails.ChangeOwnerDate);

        $("#AllowToViewAcceptedByDetails").val(mainDetails.AllowToViewAcceptedByDetails);
        $("#AllowToViewChangedOwnerByDetails").val(mainDetails.AllowToViewChangedOwnerByDetails);

        $("#AcceptedByContainer").css("display", mainDetails.AcceptedById != "" ? "inline" : "none");
        $("#ChangedOwnerByContainer").css("display", mainDetails.ChangedOwnerById != "" ? "inline" : "none");

        SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');
        SetEntityDetailsLink('AllowToViewChangedOwnerByDetails', 'ChangedOwnerByName', 'User', 'ChangedOwnerById');
    },

    OnSuccessChangeOwnerWaybillChangeRecipient: function (ajaxContext) {
        $("#mainDetailsRecipientLink").text(ajaxContext.Name);
        $("#mainDetailsRecipientLink").attr("href", "/AccountOrganization/Details?id=" + ajaxContext.Id + "&backURL=" + $("#currentUrl").val());
        HideModal(function () {
            ShowSuccessMessage("Получатель сменен.", "messageChangeOwnerWaybillDetails");
        });
    },

    RefreshPermissions: function (details) {
        var permission = details.MainDetails;

        UpdateButtonAvailability("btnPrepareToAccept", details.AllowToPrepareToAccept);
        UpdateButtonAvailability("btnCancelReadinessToAccept", details.AllowToCancelReadinessToAccept);
        UpdateButtonAvailability("btnAccept", details.AllowToAccept);
        UpdateButtonAvailability("btnCancelAcceptance", details.AllowToCancelAcceptance);
        UpdateButtonAvailability("btnEdit", details.AllowToEdit);
        UpdateButtonAvailability("btnDelete", details.AllowToDelete);
        UpdateButtonAvailability("btnAddRowsByList", details.AllowToEdit);

        UpdateElementVisibility("btnPrepareToAccept", details.IsPossibilityToPrepareToAccept);
        UpdateElementVisibility("btnCancelReadinessToAccept", details.AllowToCancelReadinessToAccept);
        UpdateElementVisibility("btnAccept", details.IsPossibilityToAccept);
        UpdateElementVisibility("btnCancelAcceptance", details.AllowToCancelAcceptance);
        UpdateElementVisibility("btnEdit", details.AllowToEdit);
        UpdateElementVisibility("btnDelete", details.AllowToDelete);

        UpdateElementVisibility("linkChangeRecipient", permission.AllowToChangeRecipient);
        UpdateElementVisibility("feature_menu_box", details.AllowToPrintForms);
        UpdateElementVisibility("btnAddRowsByList", details.AllowToEdit);

        UpdateElementVisibility("linkChangeCurator", permission.AllowToChangeCurator);
    },

    OnFailChangeOwnerWaybillChangeRecipient: function (result) {
        ShowErrorMessage(result.responseText, "messageChangeOwnerWaybillChangeRecipient");
    }
};