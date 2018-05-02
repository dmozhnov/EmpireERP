var WriteoffWaybill_Details = {
    Init: function () {
        Waybill_Details.Init();

        $(document).ready(function () {
            // Вызов окна параметров для печатных форм
            $('#lnkWriteoffWaybillPrintingForm').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/WriteoffWaybill/ShowWriteoffWaybillPrintingFormSettings/",
                    data: { waybillId: $('#Id').val() },
                    success: function (result) {
                        $('#writeoffWaybillPrintingForm').hide().html(result);
                        $.validator.unobtrusive.parse($("#WriteoffWaybillPrintingForm"));
                        ShowModal("writeoffWaybillPrintingForm");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                    }
                });
            });

            // Изменить куратора
            $("#linkChangeCurator").click(function () {
                var storageId = $("#SenderStorageId").val();
                Waybill_Edit.ShowCuratorSelectorForm(3/*waybillTypeId*/, storageId, "", null, "messageWriteoffWaybillEdit");
            });

            // обработка выбора куратора
            $(".select_user").live("click", function () {
                Waybill_Details.HandlerForSelectCurator(3/*waybillTypeId*/, $(this));
            });

        }); //$(document).ready

        $('#btnAddRowsByList').live("click", function () {
            var id = $('#Id').val();
            window.location = "/WriteoffWaybill/AddRowsByList?id=" + id + GetBackUrl();
        });

        $("#btnBackTo").live('click', function () {
            window.location = $('#BackURL').val();
        });

        $('#btnEditWriteoffWaybill').live("click", function () {
            var writeoffWaybillId = $('#Id').val();
            window.location = "/WriteoffWaybill/Edit?id=" + writeoffWaybillId + GetBackUrl();
        });

        $('#btnDeleteWriteoffWaybill').live('click', function () {
            if (confirm('Вы уверены?')) {
                var writeoffWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/WriteoffWaybill/Delete/",
                    data: { id: writeoffWaybillId },
                    success: function () {
                        window.location = "/WriteoffWaybill/List";
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                    }
                });
            }
        });

        $("#btnPrepareToAccept").live('click', function () {
            var writeoffWaybillId = $('#Id').val();

            StartButtonProgress($(this));
            $.ajax({
                type: "POST",
                url: "/WriteoffWaybill/PrepareToAccept",
                data: { writeoffWaybillId: writeoffWaybillId },
                success: function (result) {
                    RefreshGrid("gridWriteoffWaybillRows", function () {
                        WriteoffWaybill_Details.RefreshMainDetails(result);
                        ShowSuccessMessage("Накладная подготовлена к проводке.", "messageWriteoffWaybillEdit");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                }
            });
        });

        $("#btnCancelReadinessToAccept").live('click', function () {
            if (confirm('Вы уверены?')) {
                var writeoffWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/WriteoffWaybill/CancelReadinessToAccept",
                    data: { writeoffWaybillId: writeoffWaybillId },
                    success: function (result) {
                        RefreshGrid("gridWriteoffWaybillRows", function () {
                            WriteoffWaybill_Details.RefreshMainDetails(result);
                            ShowSuccessMessage("Готовность накладной к проводке отменена.", "messageWriteoffWaybillEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                    }
                });
            }
        });

        $("#btnAccept").live('click', function () {
            var writeoffWaybillId = $('#Id').val();

            StartButtonProgress($(this));
            $.ajax({
                type: "POST",
                url: "/WriteoffWaybill/Accept",
                data: { writeoffWaybillId: writeoffWaybillId },
                success: function (result) {
                    RefreshGrid("gridWriteoffWaybillRows", function () {
                        WriteoffWaybill_Details.RefreshMainDetails(result);
                        ShowSuccessMessage("Накладная проведена.", "messageWriteoffWaybillEdit");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                }
            });
        });

        $("#btnCancelAcceptance").live('click', function () {
            if (confirm('Вы уверены?')) {
                var writeoffWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/WriteoffWaybill/CancelAcceptance",
                    data: { writeoffWaybillId: writeoffWaybillId },
                    success: function (result) {
                        RefreshGrid("gridWriteoffWaybillRows", function () {
                            WriteoffWaybill_Details.RefreshMainDetails(result);
                            ShowSuccessMessage("Проводка накладной отменена.", "messageWriteoffWaybillEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                    }
                });
            }
        });

        $('#btnWriteoff').live("click", function () {
            var writeoffWaybillId = $('#Id').val();

            StartButtonProgress($(this));
            $.ajax({
                type: "POST",
                url: "/WriteoffWaybill/Writeoff/",
                data: { writeoffWaybillId: writeoffWaybillId },
                success: function (result) {
                    RefreshGrid("gridWriteoffWaybillRows", function () {
                        WriteoffWaybill_Details.RefreshMainDetails(result);
                        ShowSuccessMessage("Списание произведено.", "messageWriteoffWaybillEdit");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                }
            });
        });

        $('#btnCancelWriteoff').live("click", function () {
            var writeoffWaybillId = $('#Id').val();
            if (confirm('Вы уверены, что хотите отменить списание?')) {
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/WriteoffWaybill/CancelWriteoff/",
                    data: { writeoffWaybillId: writeoffWaybillId },
                    success: function (result) {
                        RefreshGrid("gridWriteoffWaybillRows", function () {
                            WriteoffWaybill_Details.RefreshMainDetails(result);
                            ShowSuccessMessage("Отмена списания произведена.", "messageWriteoffWaybillEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
                    }
                });
            }
        });
    },

    // обновление основной информации о накладной и состояний кнопок
    RefreshMainDetails: function (details) {
        var mainDetails = details.MainDetails;
        
        $("#StateName").text(mainDetails.StateName);
        $("#PurchaseCostSum").text(mainDetails.PurchaseCostSum);
        $("#SenderAccountingPriceSum").text(mainDetails.SenderAccountingPriceSum);
        $("#SenderStorageName").text(mainDetails.SenderStorageName);
        $("#WriteoffReasonName").text(mainDetails.WriteoffReasonName);
        $("#ReceivelessProfitPercent").text(mainDetails.ReceivelessProfitPercent);
        $("#ReceivelessProfitSum").text(mainDetails.ReceivelessProfitSum);
        $("#RowCount").text(mainDetails.RowCount);
        $("#Comment").html(mainDetails.Comment);
        $("#AcceptedByName").text(mainDetails.AcceptedByName);
        $("#AcceptedById").val(mainDetails.AcceptedById);
        $("#AcceptanceDate").text(mainDetails.AcceptanceDate);
        $("#WrittenoffByName").text(mainDetails.WrittenoffByName);
        $("#WrittenoffById").val(mainDetails.WrittenoffById);
        $("#WriteoffDate").text(mainDetails.WriteoffDate);
        $("#TotalWeight").text(mainDetails.TotalWeight);
        $("#TotalVolume").text(mainDetails.TotalVolume);

        $("#AllowToViewAcceptedByDetails").val(mainDetails.AllowToViewAcceptedByDetails);
        $("#AllowToViewWrittenoffByDetails").val(mainDetails.AllowToViewWrittenoffByDetails);

        $("#AcceptedByContainer").css("display", mainDetails.AcceptedById != "" ? "inline" : "none");
        $("#WrittenoffByContainer").css("display", mainDetails.WrittenoffById != "" ? "inline" : "none");

        SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');
        SetEntityDetailsLink('AllowToViewWrittenoffByDetails', 'WrittenoffByName', 'User', 'WrittenoffById');

        UpdateElementVisibility("btnEditWriteoffWaybill", details.AllowToEdit);
        UpdateButtonAvailability("btnEditWriteoffWaybill", details.AllowToEdit);

        UpdateElementVisibility("btnAddRowsByList", details.AllowToEdit);
        UpdateButtonAvailability("btnAddRowsByList", details.AllowToEdit);

        UpdateElementVisibility("btnDeleteWriteoffWaybill", details.AllowToDelete);
        UpdateButtonAvailability("btnDeleteWriteoffWaybill", details.AllowToDelete);

        UpdateButtonAvailability("btnWriteoff", details.AllowToWriteoff);
        UpdateElementVisibility("btnWriteoff", details.IsPossibilityToWriteoff);
        UpdateElementVisibility("btnCancelWriteoff", details.AllowToCancelWriteoff);
        UpdateButtonAvailability("btnCancelWriteoff", details.AllowToCancelWriteoff);

        UpdateElementVisibility("btnPrepareToAccept", details.IsPossibilityToPrepareToAccept);
        UpdateButtonAvailability("btnPrepareToAccept", details.AllowToPrepareToAccept);
        UpdateElementVisibility("btnCancelReadinessToAccept", details.AllowToCancelReadinessToAccept);
        UpdateButtonAvailability("btnCancelReadinessToAccept", details.AllowToCancelReadinessToAccept);

        UpdateElementVisibility("btnAccept", details.IsPossibilityToAccept);
        UpdateButtonAvailability("btnAccept", details.AllowToAccept);
        UpdateElementVisibility("btnCancelAcceptance", details.AllowToCancelAcceptance);
        UpdateButtonAvailability("btnCancelAcceptance", details.AllowToCancelAcceptance);

        UpdateElementVisibility("feature_menu_box", details.AllowToPrintForms);
        UpdateElementVisibility("linkChangeCurator", details.AllowToChangeCurator);
    },
    
    OnSuccessWriteoffWaybillRowEdit: function (ajaxContext) {
        if ($('#writeoffWaybillRowEdit #Id').val() != "00000000-0000-0000-0000-000000000000") {
            // грид для формы добавления товаров списком
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridWriteoffWaybillRows", function () {
                    RefreshGrid("gridArticleGroups", function () {
                        HideModal(function () {
                            ShowSuccessMessage("Сохранено.", "messageWriteoffWaybillRowList");
                        });
                    });
                });
            });
        }
        else {
            // грид для формы добавления товаров списком
            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                RefreshGrid("gridWriteoffWaybillRows", function () {
                    RefreshGrid("gridArticleGroups", function () {
                        WriteoffWaybill_Shared.ClearForm();
                        ShowSuccessMessage("Сохранено.", "messageWriteoffWaybillRowEdit");
                    });
                });
            });
        }

        WriteoffWaybill_Details.RefreshMainDetails(ajaxContext);
    }
}; 