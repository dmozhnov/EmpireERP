var ExpenditureWaybill_Edit = {
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

                $("#DealContractCashPaymentSumDiv").hide();
            }
            // при добавлении
            else {
                $("#rbIsAutoNumber_true").attr("checked", "checked");
                $("#rbIsAutoNumber_true").trigger("click");

                $("#DealContractCashPaymentSumDiv").show();
            }

            //перехват Submit для валидации
            $('#form0 input[type="submit"]').click(function () {
                if (ExpenditureWaybill_Edit.DeliveryAddressTypeValidation()) {
                    return true;
                }

                return false;
            });

            ExpenditureWaybill_Edit.DeliveryAddressTypeChange();

            $("#DeliveryAddressTypeId").change(function () {
                ExpenditureWaybill_Edit.DeliveryAddressTypeChange();
            })

            //Блокируем поля, зависящие от сделки
            if ($("#DealId").val() == "") {
                $("#DealContractCashPaymentSumDiv").hide();
                $("#SenderStorageId").attr("disabled", "disabled");
                $("#TeamId").attr("disabled", "disabled");
            }

            // Выбор клиента
            $("#ClientName").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Client/SelectClient",
                    success: function (result) {
                        $("#clientSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#clientSelector"));
                        ShowModal("clientSelector");

                        BindClientSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                    }
                });
            });

            // открытие формы выбора сделки
            $("#DealName").click(function () {
                var clientId = $("#ClientId").val();
                var clientOrganizationId = $("#ClientOrganizationId").val();

                // Зачем такой if? Откуда вообще в этой форме ClientOrganizationId?
                if ($("#ClientOrganizationId").length != 0 && clientOrganizationId != "") {
                    StartLinkProgress($(this));

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClientOrganization",
                        data: { clientOrganizationId: clientOrganizationId, mode: "ForSaleToClient" },
                        success: function (result) {
                            $("#dealSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealSelector"));
                            ShowModal("dealSelector");

                            BindDealSelection();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                        }
                    });
                }
                else if ($("#ClientId").length != 0 && clientId != "") {
                    StartLinkProgress($(this));

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClient",
                        data: { clientId: clientId, mode: "ForSaleToClient" },
                        success: function (result) {
                            $("#dealSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealSelector"));
                            ShowModal("dealSelector");

                            BindDealSelection();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                        }
                    });
                } else {
                    $("#ClientId").ValidationError("Укажите клиента");
                }
            });

            // открытие формы выбора квоты
            $("#DealQuotaName").click(function () {
                var dealId = $("#DealId").val();

                if (dealId != "") {
                    StartLinkProgress($(this));

                    $.ajax({
                        type: "GET",
                        url: "/DealQuota/SelectDealQuota",
                        data: { dealId: dealId, mode: "Sale" },
                        success: function (result) {
                            $("#dealQuotaSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealQuotaSelector"));
                            ShowModal("dealQuotaSelector");

                            BindDealQoutaSelection();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                        }
                    });
                } else {
                    $("#DealId").ValidationError("Укажите сделку");
                }
            });

            $("#CuratorName").click(function () {
                var dealId = $("#DealId").val();

                if (dealId != "" && dealId != null) {
                    Waybill_Edit.ShowCuratorSelectorForm(4/*WaybillTypeId*/, "", dealId, $(this), "messageExpenditureWaybillEdit");
                }
                else {
                    $("#DealId").ValidationError("Укажите сделку.");
                }
            });
        });

        // обработка выбора клиента
        function BindClientSelection() {
            $(".select_client").die("click");
            $(".select_client").live("click", function () {
                var clientId = $(this).parent("td").parent("tr").find(".Id").text();
                var clientName = $(this).parent("td").parent("tr").find(".Name").text();

                // ajax за доп. данными и затем закрытие модального окна
                $.ajax({
                    type: "GET",
                    url: "/Client/GetFactualAddress",
                    data: { clientId: clientId },
                    success: function (result) {
                        HideModal(function () {
                            $("#ClientName").text(clientName);
                            $("#ClientId").val(clientId);
                            $("#ClientId").ValidationValid();

                            // Сбрасываем сделку
                            $("#DealName").text("Выберите сделку");
                            $("#DealId").val("");
                            $("#DealContractCashPaymentSumDiv").hide();

                            //Настраиваем адреса
                            $("#OrganizationDeliveryAddress").val("");
                            $("#ClientDeliveryAddress").val(result);

                            // Блокируем зависимые от сделки поля
                            $("#SenderStorageId").clearSelect().attr("disabled", "disabled");
                            $("#TeamId").clearSelect().attr("disabled", "disabled");
                            $("#DealQuotaName").text("Выберите квоту");
                            $("#DealQuotaId").val("");
                            ExpenditureWaybill_Edit.DisablePaymentFormSelection();
                            ExpenditureWaybill_Edit.DeliveryAddressTypeChange();
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageClientSelectList");
                    }
                });
            });
        }

        // обработка выбора сделки
        function BindDealSelection() {
            $(".select_deal").die("click");
            $(".select_deal").live("click", function () {
                StartComboBoxProgress($("#TeamId"));
                StartComboBoxProgress($("#SenderStorageId"));

                var dealId = $(this).parent("td").parent("tr").find(".Id").text();
                var dealName = $(this).parent("td").parent("tr").find(".Name").text();

                // ajax за доп. данными и затем закрытие модального окна
                $.ajax({
                    type: "GET",
                    url: "/Deal/GetDealInfo",
                    data: { dealId: dealId },
                    success: function (result) {
                        HideModal(function () {
                            // сохраняем данные сделки
                            $("#DealName").text(dealName);
                            $("#DealId").val(dealId);

                            // выставляем зависимые от сделки данные
                            $("#SenderStorageId").clearSelect().fillSelect(result.StorageList).removeAttr("disabled");
                            $("#TeamId").clearSelect().fillSelect(result.TeamList, true).removeAttr("disabled");

                            $("#DealContractCashPaymentSumDiv").show();
                            $("#DealContractCashPaymentSum").text(result.DealContractCashPaymentSum);

                            //установим адреса
                            $("#OrganizationDeliveryAddress").val(result.OrganizationDeliveryAddress);
                            ExpenditureWaybill_Edit.DeliveryAddressTypeChange();

                            StopComboBoxProgress($("#TeamId"));
                            StopComboBoxProgress($("#SenderStorageId"));
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealSelectList");
                    }
                });
            });
        }

        // обработка выбора квоты
        function BindDealQoutaSelection() {
            $("#gridDealQuotaSelect .dealQuota_select_link").die("click");
            $("#gridDealQuotaSelect .dealQuota_select_link").live("click", function () {
                var dealQuotaId = $(this).parent("td").parent("tr").find(".quotaId").text();
                var dealQuotaName = $(this).parent("td").parent("tr").find(".quotaFullName").text();
                var isPrepayment = $(this).parent("td").parent("tr").find(".isPrepayment").text();

                HideModal(function () {
                    $("#DealQuotaName").text(dealQuotaName);
                    $("#DealQuotaId").val(dealQuotaId);

                    if (isPrepayment == "1") {
                        ExpenditureWaybill_Edit.DisablePaymentFormSelection();
                    } else {
                        ExpenditureWaybill_Edit.EnablePaymentFormSelection();
                    }
                });
            });
        }

        $("#btnBack").live("click", function () {
            window.location = $('#BackURL').val();
        });
    },

    DeliveryAddressTypeValidation: function () {
        var deliveryAddressTypeId = $("#DeliveryAddressTypeId").val();

        if (deliveryAddressTypeId == "3") {
            if ($("#CustomDeliveryAddress").val() == "") {
                $('#CustomDeliveryAddress').ValidationError("Введите адрес доставки");

                return false;
            }
        }
        $('#CustomDeliveryAddress').ValidationValid();

        return true;
    },

    DeliveryAddressTypeChange: function () {
        var deliveryAddressTypeId = $("#DeliveryAddressTypeId").val();
        switch (deliveryAddressTypeId) {
            case "1":
                $("#divCustomDeliveryAddress").hide();
                $("#divSelectedDeliveryAddress").html($("#ClientDeliveryAddress").val());
                break;
            case "2":
                $("#divCustomDeliveryAddress").hide();
                $("#divSelectedDeliveryAddress").html($("#OrganizationDeliveryAddress").val());
                break;
            case "3":
                $("#divSelectedDeliveryAddress").text("");
                $("#divCustomDeliveryAddress").show();
                break;
            default:
                $("#divSelectedDeliveryAddress").text("");
                $("#divCustomDeliveryAddress").hide();
        }
    },

    EnablePaymentFormSelection: function () {
        $("#IsPrepayment").attr("disabled", "disabled");
        $("#rbIsPrepayment_false").removeAttr("disabled");
        $("#rbIsPrepayment_true").removeAttr("disabled");
        $("#rbIsPrepayment_false").attr("checked", "checked");
        $("#rbIsPrepayment_true").removeAttr("checked");
    },

    DisablePaymentFormSelection: function () {
        $("#IsPrepayment").removeAttr("disabled");
        $("#IsPrepayment").val("1");
        $("#rbIsPrepayment_false").attr("disabled", "disabled");
        $("#rbIsPrepayment_true").attr("disabled", "disabled");
        $("#rbIsPrepayment_false").removeAttr("checked");
        $("#rbIsPrepayment_true").attr("checked", "checked");
    },

    OnSuccessExpenditureWaybillEdit: function (ajaxContext) {
        window.location = "/ExpenditureWaybill/Details?id=" + ajaxContext + "&backURL=" + $("#BackURL").val();
    },

    OnFailExpenditureWaybillEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageExpenditureWaybillEdit");
    }
};