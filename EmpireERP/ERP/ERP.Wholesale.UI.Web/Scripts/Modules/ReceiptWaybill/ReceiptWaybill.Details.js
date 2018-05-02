var ReceiptWaybill_Details = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            // Вызов окна параметров для печатных форм
            $('#printingForm').live('click', function () {
                var id = $('#Id').val();
                $.ajax({
                    type: "GET",
                    url: "/ReceiptWaybill/ShowPrintingFormSettings/",
                    data: { waybillId: id },
                    success: function (result) {
                        $('#receiptWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#receiptWaybillPrintingForm"));
                        ShowModal("receiptWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                    }
                });
            });

            $('#printingFormTORG12').live("click", function () {
                $.ajax({
                    type: "GET",
                    url: "/ReceiptWaybill/ShowTORG12PrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#receiptWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#receiptWaybillPrintingForm"));
                        ShowModal("receiptWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                    }
                });
            });

            $('#divergenceActPrintingForm').live('click', function () {
                var id = $('#Id').val();
                window.open("/ReceiptWaybill/ShowDivergenceActPrintingForm?" + "WaybillId=" + id);
            });


            $("#btnBackTo").live('click', function () {
                window.location = $('#BackURL').val();
            });

            // редактирование приходной накладной
            $('#btnEditReceiptWaybill').live('click', function () {
                var receiptWaybillId = $('#Id').val();
                window.location = "/ReceiptWaybill/Edit?id=" + receiptWaybillId + GetBackUrlFromString($('#BackURL').val());
            });

            // удаление накладной
            $("#btnDeleteReceiptWaybill").live('click', function () {
                if (confirm('Вы уверены?')) {
                    var waybill_id = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/Delete/",
                        data: { id: waybill_id },
                        success: function () {
                            window.location = "/ReceiptWaybill/List";
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                        }
                    });
                }
            });

            // добавление строки
            $("#btnAddReceiptWaybillRow").live("click", function () {
                var receiptWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ReceiptWaybill/AddRow",
                    data: { receiptWaybillId: receiptWaybillId },
                    success: function (result) {
                        $('#receiptWaybillRowForEdit').hide().html(result);
                        //$.validator.unobtrusive.parse($("#receiptWaybillRowForEdit"));
                        ShowModal("receiptWaybillRowForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowList");
                    }
                });
            });

            // проводка накладной
            $('#btnAcceptReceiptWaybill').live('click', function () {
                if (!$('#btnAcceptReceiptWaybill').attr("disabled")) {
                    var receiptWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/Accept/",
                        data: { Id: receiptWaybillId },
                        success: function (result) {
                            RefreshGrid("gridReceiptWaybillRows", function () {
                                ReceiptWaybill_Details.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Проводка произведена.", "messageReceiptWaybillDetailsEdit");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                        }
                    });
                }
            });

            //проводка задним числом
            $("#btnAcceptRetroactivelyReceiptWaybill").live('click', function () {
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ReceiptWaybill/AcceptRetroactively",
                    success: function (result) {
                        $('#dateTimeSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#dateTimeSelector"));
                        ShowModal("dateTimeSelector");
                        BindRetroactivelyAcceptanceDateSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                    }
                });
            });

            function BindRetroactivelyAcceptanceDateSelection() {
                $('#btnSelectDateTime').click(function (e) {
                
                    e.preventDefault();
                    if (!$('#dateTimeSelectForm').valid()) return false;

                    var dateTime = $("#dateTimeSelector #Date").val() + " " + $("#dateTimeSelector #Time").val();
                    var receiptWaybillId = $('#Id').val();

                    StartButtonProgress($("#btnSelectDateTime"));

                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/AcceptRetroactively",
                        data: { receiptWaybillId: receiptWaybillId, acceptanceDate: dateTime },
                        success: function (result) {
                            RefreshGrid("gridReceiptWaybillRows", function () {
                                ReceiptWaybill_Details.RefreshMainDetails(result.MainDetails);
                                HideModal(function () {
                                    ShowSuccessMessage("Накладная проведена.", "messageReceiptWaybillDetailsEdit");
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDateSelect");
                        }
                    });
                });
            }

            // переход к приемке накладной
            $('#btnReceiptReceiptWaybill').live('click', function () {
                if (!$('#btnReceiptReceiptWaybill').attr("disabled")) {
                    var receiptWaybillId = $('#Id').val();
                    if (IsTrue($("#IsCreatedFromProductionOrderBatch").val())) {
                        StartButtonProgress($(this));
                        $.ajax({
                            type: "POST",
                            url: "/ReceiptWaybill/PerformReceiption",
                            data: { waybillId: receiptWaybillId },
                            success: function (result) {
                                window.location = "/ReceiptWaybill/Details?Id=" + receiptWaybillId + GetBackUrlFromString($("#BackURL").val());
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                            }
                        });
                    } else {
                        window.location = "/ReceiptWaybill/Receipt?Id=" + receiptWaybillId + GetBackUrlFromString($('#BackURL').val());
                    }
                }
            });

            // окончательное согласование
            $('#btnApproveReceiptWaybill').live('click', function () {
                if (!$('#btnApproveReceiptWaybill').attr("disabled")) {
                    var receiptWaybillId = $('#Id').val();
                    window.location = "/ReceiptWaybill/Approve?Id=" + receiptWaybillId + GetBackUrlFromString($('#BackURL').val());
                }
            });

            // Отменить проводку
            $('#btnCancelAcceptanceReceiptWaybill').live('click', function () {
                if (confirm('Вы действительно хотите отменить проводку накладной?')) {
                    var receiptWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/CancelAcceptance/",
                        data: { receiptWaybillId: receiptWaybillId },
                        success: function (result) {
                            RefreshGrid("gridReceiptWaybillRows", function () {
                                ReceiptWaybill_Details.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Отмена проводки произведена.", "messageReceiptWaybillDetailsEdit");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                        }
                    });
                }
            });

            // Отмена приемки
            $('#btnCancelReceiptReceiptWaybill').live('click', function () {
                if (confirm('Вы уверены, что хотите отменить приемку?')) {
                    var receiptWaybillId = $('#Id').val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/CancelReceipt",
                        data: { receiptWaybillId: receiptWaybillId },
                        success: function (result) {
                            window.location = '/ReceiptWaybill/Details?id=' + receiptWaybillId + GetBackUrlFromString($('#BackURL').val());
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                        }
                    });
                }
            });

            // Отмена окончательного согласования
            $('#btnCancelApprovementReceiptWaybill').live('click', function () {
                var waybillId = $('#Id').val();
                if (confirm('Вы уверены, что хотите отменить приемку/окончательное согласование?')) {

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/CancelApprovement/",
                        data: { receiptWaybillId: waybillId },
                        success: function (result) {
                            window.location = '/ReceiptWaybill/Details?id=' + waybillId + GetBackUrlFromString($('#BackURL').val());
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillDetailsEdit");
                        }
                    });
                }
            });

            // Редактирование позиции накладной
            $("#gridReceiptWaybillRows .edit_link").live("click", function () {
                var receiptWaybillId = $('#Id').val();
                var receiptWaybillRowId = $(this).parent("td").parent("tr").find(".hidden_column").text();

                $.ajax({
                    type: "GET",
                    url: "/ReceiptWaybill/EditRow",
                    data: { receiptWaybillId: receiptWaybillId, receiptWaybillRowId: receiptWaybillRowId },
                    success: function (result) {
                        $('#receiptWaybillRowForEdit').hide().html(result);
                        ShowModal("receiptWaybillRowForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowList");
                    }
                });
            });

            // Удаление позиции накладной
            $("#gridReceiptWaybillRows .delete_link").live("click", function () {
                if (confirm('Вы уверены?')) {
                    var receiptWaybillId = $('#Id').val();
                    var receiptWaybillRowId = $(this).parent("td").parent("tr").find(".hidden_column").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/DeleteRow/",
                        data: { receiptWaybillId: receiptWaybillId, receiptWaybillRowId: receiptWaybillRowId },
                        success: function (result) {
                            RefreshGrid("gridReceiptWaybillRows", function () { ShowSuccessMessage("Позиция удалена.", "messageReceiptWaybillRowList"); });
                            RefreshGrid("gridReceiptWaybillArticleGroups");
                            ReceiptWaybill_Details.RefreshMainDetails(result.MainDetails);
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowList");
                        }
                    });
                }
            });

            $(".articleDetails").live("click", function () {
                var receiptWaybillId = $('#Id').val();
                var receiptWaybillRowId = $(this).parent("td").parent("tr").find(".hidden_column").text();

                $.ajax({
                    type: "GET",
                    url: "/ReceiptWaybill/EditRow",
                    data: { receiptWaybillId: receiptWaybillId, receiptWaybillRowId: receiptWaybillRowId },
                    success: function (result) {
                        $('#receiptWaybillRowForEdit').hide().html(result);
                        ShowModal("receiptWaybillRowForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowList");
                    }
                });
            });

            // Создание реестра цен
            $('#btnCreatePriceList').live("click", function () {
                var receiptWaybillId = $('#Id').val();
                window.location = "/AccountingPriceList/Create?reasonCode=1&additionalId=" + receiptWaybillId + GetBackUrl();
            });

            // Изменить куратора
            $("#linkChangeCurator").click(function () {
                var storageIds = $("#StorageId").val();
                Waybill_Edit.ShowCuratorSelectorForm(1/*waybillTypeId*/, storageIds, "", null, "messageReceiptWaybillDetailsEdit");
            });

            // обработка выбора куратора
            $(".select_user").live("click", function () {
                Waybill_Details.HandlerForSelectCurator(1/*waybillTypeId*/, $(this));
            });
        });
    },

    // обновление основной информации о накладной
    RefreshMainDetails: function (details) {
        $("#Sum").text(details.Sum + " р.");
        $("#RowCount").text(details.RowCount);
        $("#ShippingPercent").text(details.ShippingPercent);
        $("#TotalWeight").text(details.TotalWeight);
        $("#TotalVolume").text(details.TotalVolume);
        $("#StateName").text(details.StateName);
        $("#ValueAddedTaxString").text(details.ValueAddedTaxString);
        if (IsTrue(details.AreSumDivergences)) {
            $('#PendingSum').addClass("attention");
            $('#Sum').addClass("attention");
        }
        else {
            $('#PendingSum').removeClass("attention");
            $('#Sum').removeClass("attention");
        }

        $("#AcceptedByName").text(details.AcceptedByName);
        $("#AcceptedById").val(details.AcceptedById);
        $("#AcceptanceDate").text(details.AcceptanceDate);
        $("#ReceiptedByName").text(details.ReceiptedByName);
        $("#ReceiptedById").val(details.ReceiptedById);
        $("#ReceiptDate").text(details.ReceiptDate);
        $("#ApprovedByName").text(details.ApprovedByName);
        $("#ApprovedById").val(details.ApprovedById);
        $("#ApprovementDate").text(details.ApprovementDate);

        $("#AllowToViewAcceptedByDetails").val(details.AllowToViewAcceptedByDetails);
        $("#AllowToViewReceiptedByDetails").val(details.AllowToViewReceiptedByDetails);
        $("#AllowToViewApprovedByDetails").val(details.AllowToViewApprovedByDetails);

        $("#AcceptedByContainer").css("display", details.AcceptedById != "" ? "inline" : "none");
        $("#ReceiptedByContainer").css("display", details.ReceiptedById != "" ? "inline" : "none");
        $("#ApprovedByContainer").css("display", details.ApprovedById != "" ? "inline" : "none");

        SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');
        SetEntityDetailsLink('AllowToViewReceiptedByDetails', 'ReceiptedByName', 'User', 'ReceiptedById');
        SetEntityDetailsLink('AllowToViewApprovedByDetails', 'ApprovedByName', 'User', 'ApprovedById');

        UpdateButtonAvailability("btnEditReceiptWaybill", IsTrue(details.AllowToEdit) || IsTrue(details.AllowToEditProviderDocuments));
        UpdateElementVisibility("btnEditReceiptWaybill", IsTrue(details.AllowToEdit) || IsTrue(details.AllowToEditProviderDocuments));

        UpdateButtonAvailability("btnDeleteReceiptWaybill", details.AllowToDelete);
        UpdateElementVisibility("btnDeleteReceiptWaybill", details.AllowToDelete);

        UpdateButtonAvailability("btnAcceptReceiptWaybill", details.AllowToAccept);
        UpdateElementVisibility("btnAcceptReceiptWaybill", details.AllowToAccept);
        UpdateButtonAvailability("btnAcceptRetroactivelyReceiptWaybill", details.AllowToAcceptRetroactively);
        UpdateElementVisibility("btnAcceptRetroactivelyReceiptWaybill", details.AllowToAcceptRetroactively);
        UpdateButtonAvailability("btnCancelAcceptanceReceiptWaybill", details.AllowToCancelAcceptance);
        UpdateElementVisibility("btnCancelAcceptanceReceiptWaybill", details.AllowToCancelAcceptance);

        UpdateButtonAvailability("btnReceiptReceiptWaybill", details.AllowToReceipt);
        UpdateElementVisibility("btnReceiptReceiptWaybill", details.AllowToReceipt);

        UpdateElementVisibility("feature_menu_box", details.AllowToPrintForms);
        UpdateElementVisibility("printingForm", details.AllowToPrintForms);
        UpdateElementVisibility("printingFormTORG12", details.AllowToPrintForms);

        UpdateElementVisibility("linkChangeCurator", details.AllowToChangeCurator);
    },

    // обработка удачного редактирования строки
    OnSuccessReceiptWaybillRowEdit: function (ajaxContext) {
        RefreshGrid("gridReceiptWaybillArticleGroups");
        RefreshGrid("gridReceiptWaybillRows", function () {
            // после редактирования мы закрываем модальную форму           
            if ($('#receiptWaybillRowForEdit #Id').val() != "00000000-0000-0000-0000-000000000000") {
                HideModal();
                ShowSuccessMessage("Сохранено.", "messageReceiptWaybillRowList");
            }
            // а после добавления мы оставляем модальную форму, но очищаем ее, чтобы она была готова к новому добавлению
            else {
                $("#receiptWaybillRowForEdit #PurchaseCost").val("0");
                $("#receiptWaybillRowForEdit #ValueAddedTaxSum").text("0");
                $("#receiptWaybillRowForEdit #ArticleId").val("");
                $("#receiptWaybillRowForEdit #ArticleName").text("Выберите товар");
                $("#receiptWaybillRowForEdit #PendingCount").val("").ValidationValid();
                $("#receiptWaybillRowForEdit #PendingSum").val("").ValidationValid();
                $("#receiptWaybillRowForEdit #ProductionCountryId").val("").ValidationValid();
                $("#receiptWaybillRowForEdit #ManufacturerId").val("").ValidationValid();
                $("#receiptWaybillRowForEdit #ManufacturerName").text("Выберите фабрику-изготовителя");
                $("#receiptWaybillRowForEdit #MeasureUnitName").text("");
                $("#receiptWaybillRowForEdit #CustomsDeclarationNumber").val("");
                SetFieldScale("#PendingCount", 12, 0, "#receiptWaybillRowForEdit", true);

                ShowSuccessMessage("Сохранено.", "messageReceiptWaybillRowEdit");
            }

            ReceiptWaybill_Details.RefreshMainDetails(ajaxContext.MainDetails);
        });  
    }
}; 