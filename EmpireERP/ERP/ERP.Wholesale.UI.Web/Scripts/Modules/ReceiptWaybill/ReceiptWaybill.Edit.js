var ReceiptWaybill_Edit = {
    Init: function () {
        Waybill_Edit.Init();

        $(document).ready(function () {

            $("#rbIsAutoNumber_true").click(function () {
                $("#Number").ValidationValid();
                $("#Number").attr("disabled", "disabled").val("");
                $("#IsAutoNumber").val("1");
            });

            $("#rbIsAutoNumber_false").click(function () {
                $("#Number").removeAttr("disabled").focus();
                $("#IsAutoNumber").val("0");
            });

            // при редактировании
            if (!IsTrue($("#AllowToGenerateNumber").val())) {
                $("#rbIsAutoNumber_false").trigger("click");
                $("#rbIsAutoNumber_false").attr("checked", "checked");

                $("#rbIsAutoNumber_true_wrapper").hide();
                $("#rbIsAutoNumber_false_wrapper").hide();
            }
            // при добавлении
            else {
                $("#rbIsAutoNumber_true").attr("checked", "checked");
                $("#rbIsAutoNumber_true").trigger("click");
            }

            if (IsFalse($("#IsCreatedFromProductionOrderBatch").val())) {
                if ($("#IsNew").val() == "False") {
                    $("#ContractId").removeAttr("disabled");
                    $("#AccountOrganizationId").removeAttr("disabled");
                    $("#ReceiptStorageId").removeAttr("disabled");
                }
                ReceiptWaybill_Edit.RefreshComboboxes();
            }

            $("#btnBack").live("click", function () {
                if (IsDefaultOrEmpty($("#Id").val())) {
                    // Не удалять. Таким образом мы исправляем баг 132 из старого Excel-файла.
                    window.location = $("#BackURL").val();
                }
                else {
                    window.location = "/ReceiptWaybill/Details?id=" + $("#Id").val() + GetBackUrlFromString($("#BackURL").val());
                }
            });

            if ($("#IsCustomsDeclarationNumberFromReceiptWaybill").val() == 1) {
                $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_true").attr("checked", "checked");
                $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_true").trigger("click");
                $("#CustomsDeclarationNumber").removeAttr("disabled").focus();
            }
            else {
                $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_false").attr("checked", "checked");
                $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_false").trigger("click");
                $("#CustomsDeclarationNumber").attr("disabled", "disabled").val("");
            }

            $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_false").click(function () {
                $("#CustomsDeclarationNumber").attr("disabled", "disabled").val("");
                $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_true").attr("checked", false);
                $("#IsCustomsDeclarationNumberFromReceiptWaybill").val("0");
            });

            $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_true").click(function () {
                $("#CustomsDeclarationNumber").removeAttr("disabled").focus();
                $("#rbIsCustomsDeclarationNumberFromReceiptWaybill_false").attr("checked", false);
                $("#IsCustomsDeclarationNumberFromReceiptWaybill").val("1");
            });

            $("#ProviderId").change(function () {
                ReceiptWaybill_Edit.RefreshComboboxes();
            });

            $("#ContractId").change(function () {
                ReceiptWaybill_Edit.RefreshComboboxes();
            });

            $("#AccountOrganizationId").change(function () {
                ReceiptWaybill_Edit.RefreshComboboxes();
            });

            $("#ReceiptStorageId").change(function () {
                ReceiptWaybill_Edit.RefreshComboboxes();
            });
            var allowToViewPurchaseCosts = $("#AllowToViewPurchaseCosts").val();

            if (IsTrue(allowToViewPurchaseCosts)) {
                $("#DiscountPercent").bind("keyup change paste cut", function () {
                    var pendingSum = TryGetDecimal($("#PendingSum").val());
                    var discountPercent = TryGetDecimal($("#DiscountPercent").val());
                    if (!isNaN(discountPercent) && !isNaN(pendingSum)) {
                        var discountSum = pendingSum * discountPercent / 100;
                        $("#DiscountSum").val(ValueForEdit(discountSum, 2));
                    }
                });

                $("#DiscountSum").live("keyup change paste cut", function () {
                    var discountSum = TryGetDecimal($("#DiscountSum").val());
                    var pendingSum = TryGetDecimal($("#PendingSum").val());
                    if (!isNaN(discountSum) && !isNaN(pendingSum) && pendingSum != 0) {
                        var discountPercent = discountSum * 100 / pendingSum;
                        $("#DiscountPercent").val(ValueForEdit(discountPercent, 2));
                    }
                });

                $("#PendingSum").live("keyup change paste cut", function () {
                    var discountSum = TryGetDecimal($("#DiscountSum").val());
                    var pendingSum = TryGetDecimal($("#PendingSum").val());
                    if (!isNaN(discountSum) && !isNaN(pendingSum) && pendingSum != 0) {
                        var discountPercent = discountSum * 100 / pendingSum;
                        $("#DiscountPercent").val(ValueForEdit(discountPercent, 2));
                    }
                });
            }

            $("#CuratorName").click(function () {
                var storageId = $("#ReceiptStorageId").val();
                if (storageId != "") {
                    Waybill_Edit.ShowCuratorSelectorForm(1/*WaybillTypeId*/, storageId, "", $(this), "messageReceiptWaybillEdit");
                }
                else {
                    $("#ReceiptStorageId").ValidationError("Укажите место хранения.");
                }
            });
        });
    },

    // при успешной попытке добавления/редактирования приходной накладной
    OnSuccessReceiptWaybillSave: function (ajaxContext) {
        // TODO: если !model.IsValid, контроллер возвращает success, но с моделью вместо id созданной накл. Тогда не надо переходить.
        window.location = "/ReceiptWaybill/Details?id=" + ajaxContext + GetBackUrlFromString($("#BackURL").val());
    },

    // при неудачной попытке добавления/редактирования приходной накладной
    OnFailReceiptWaybillSave: function (ajaxContext) {
        $(".field-validation-error").text("");
        ShowErrorMessage(ajaxContext.responseText, "messageReceiptWaybillEdit");
    },

    RefreshComboboxes: function () {
        var provider = $("#ProviderId").val();
        var contract = $("#ContractId").val();
        var receiptStorage = $("#ReceiptStorageId").val();
        var accountOrganization = $("#AccountOrganizationId").val();

        if ($("#AllowToChangeStorageAndOrganization").val() == "True") {
            ReceiptWaybill_Edit.UpdateComboboxes(provider, contract, receiptStorage, accountOrganization);
        } else {
            ReceiptWaybill_Edit.UpdateContractList(provider, receiptStorage, accountOrganization);
        }
    },

    UpdateComboboxes: function (provider, contract, receiptStorage, accountOrganization) {

        if (provider != "" && contract == "" && receiptStorage == "" && accountOrganization == "") {
            // Заполняем contract, receiptStorage и accountOrganization
            $("#ContractId").removeAttr("disabled");
            ReceiptWaybill_Edit.UpdateContractList(provider, receiptStorage, accountOrganization);

            $("#ReceiptStorageId").removeAttr("disabled");
            ReceiptWaybill_Edit.UpdateReceiptStorageList(provider, contract, accountOrganization);

            $("#AccountOrganizationId").removeAttr("disabled");
            ReceiptWaybill_Edit.UpdateAccountOrganizationList(provider, contract, receiptStorage);
        } else if (provider == "") {
            // Блокируем 3 нижних
            $("#ContractId").val("");
            $("#ContractId").attr("disabled", "disabled");

            $("#ReceiptStorageId").val("");
            $("#ReceiptStorageId").attr("disabled", "disabled");

            $("#AccountOrganizationId").val("");
            $("#AccountOrganizationId").attr("disabled", "disabled");
        } else {
            ReceiptWaybill_Edit.UpdateContractList(provider, receiptStorage, accountOrganization);
            ReceiptWaybill_Edit.UpdateReceiptStorageList(provider, contract, accountOrganization);
            ReceiptWaybill_Edit.UpdateAccountOrganizationList(provider, contract, receiptStorage);
        }
    },

    UpdateContractList: function (provider, receiptStorage, accountOrganization) {
        var contractId = $("#ContractId").val();
        StartComboBoxProgress($("#ContractId"));

        $.ajax({
            type: "GET",
            url: "/ReceiptWaybill/UpdateContractList",
            data: { providerId: provider, receiptStorageId: receiptStorage, accountOrganizationId: accountOrganization },
            success: function (result) {
                $("#ContractId").fillSelect(result);
                if (contractId != "") {
                    $("#ContractId").val(contractId);
                }
                StopComboBoxProgress($("#ContractId"));
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillEdit");
                StopComboBoxProgress($("#ContractId"));
            }
        });
    },

    UpdateReceiptStorageList: function (provider, contract, accountOrganization) {
        var storageId = $("#ReceiptStorageId").val();

        StartComboBoxProgress($("#ReceiptStorageId"));

        $.ajax({
            type: "GET",
            url: "/ReceiptWaybill/UpdateReceiptStorageList",
            data: { providerId: provider, contractId: contract, accountOrganizationId: accountOrganization },
            success: function (result) {
                $("#ReceiptStorageId").fillSelect(result);
                if (storageId != "") {
                    $("#ReceiptStorageId").val(storageId);
                }
                StopComboBoxProgress($("#ReceiptStorageId"));
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillEdit");
                StopComboBoxProgress($("#ReceiptStorageId"));
            }
        });
    },

    UpdateAccountOrganizationList: function (provider, contract, receiptStorage) {
        var accountOrganizationId = $("#AccountOrganizationId").val();

        StartComboBoxProgress($("#AccountOrganizationId"));

        $.ajax({
            type: "GET",
            url: "/ReceiptWaybill/UpdateAccountOrganizationList",
            data: { providerId: provider, contractId: contract, receiptStorageId: receiptStorage },
            success: function (result) {
                $("#AccountOrganizationId").fillSelect(result);
                if (accountOrganizationId != "") {
                    $("#AccountOrganizationId").val(accountOrganizationId);
                }
                StopComboBoxProgress($("#AccountOrganizationId"));
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillEdit");
                StopComboBoxProgress($("#AccountOrganizationId"));
            }
        });
    }

};