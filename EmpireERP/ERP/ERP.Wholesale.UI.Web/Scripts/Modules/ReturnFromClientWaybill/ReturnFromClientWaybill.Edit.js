var ReturnFromClientWaybill_Edit = {
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

            //Блокируем поля, зависящие от сделки
            if ($("#DealId").val() == "0" || $("#DealId").val() == "") {
                $("#ReceiptStorageId").attr("disabled", "disabled");
                $("#TeamId").attr("disabled", "disabled");
            }

            $("#btnBack").live('click', function () {
                if ($("#Id").val() == "00000000-0000-0000-0000-000000000000") {
                    // Не удалять. Таким образом мы исправляем баг 132.
                    window.location = $('#BackURL').val();
                }
                else {
                    window.location = "/ReturnFromClientWaybill/Details?id=" + $("#Id").val() + GetBackUrlFromString($('#BackURL').val());
                }
            });

            $("span#ClientName").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Client/SelectClient",
                    success: function (result) {
                        $("#selector").hide().html(result);
                        ShowModal("selector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                    }
                });
            });

            $('#btnCreateReturnFromClientReason').live("click", function () {
                $.ajax({
                    type: "GET",
                    url: "/ReturnFromClientReason/Create",
                    success: function (result) {
                        $('#returnFromClientReasonEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#returnFromClientReasonEdit"));
                        ShowModal("returnFromClientReasonEdit");
                        $('#returnFromClientReasonEdit #Name').focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                    }
                });
            });

            $(".select_client").live('click', function () {
                var clientId = $(this).parent("td").parent("tr").find(".Id").text();
                var clientName = $(this).parent("td").parent("tr").find(".Name").text();

                HideModal(function () {
                    $('#ClientId').val(clientId);
                    $('#ClientName').text(clientName);

                    $("#ClientId").ValidationValid();

                    //Сбрасываем сделку, организацию и МХ
                    $("#DealName").text("Выберите сделку");
                    $("#DealId").val("0");

                    $("#AccountOrganizationName").text("---");
                    $("#AccountOrganizationId").val("");

                    $('#ReceiptStorageId').clearSelect();
                });
            });

            $("span#DealName").click(function () {
                var clientId = $("#ClientId").val();

                if (clientId != "0") {
                    StartLinkProgress($(this));

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClient",
                        data: { clientId: clientId, mode: "ForReturnFromClient" },
                        success: function (result) {
                            $("#selector").hide().html(result);
                            ShowModal("selector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientReasonEdit");
                        }
                    });
                }
                else {
                    $("#ClientId").ValidationError("Укажите клиента");
                }
            });

            $(".select_deal").live("click", function () {
                StartComboBoxProgress($("#ReceiptStorageId"));
                StartComboBoxProgress($("#TeamId"));

                var id = $(this).parent("td").parent("tr").find(".Id").text();
                var name = $(this).parent("td").parent("tr").find(".Name").text();

                var accountOrganizationId = $(this).parent("td").parent("tr").find(".AccountOrganizationId").text();
                var accountOrganizationName = $(this).parent("td").parent("tr").find(".AccountOrganizationName").text();

                $("#DealName").text(name);
                $("#DealId").val(id);

                $("#AccountOrganizationName").text(accountOrganizationName);
                $("#AccountOrganizationId").val(accountOrganizationId);

                if (accountOrganizationId != "0") {
                    // Запрос за МХ
                    $.ajax({
                        type: "GET",
                        url: "/ReturnFromClientWaybill/GetStorageListForAccountOrganization",
                        data: { accountOrganizationId: accountOrganizationId },
                        success: function (accountOrganizationList) {
                            // Запрос за командами
                            $.ajax({
                                type: "GET",
                                url: "/ReturnFromClientWaybill/GetTeamList",
                                data: { dealId: id },
                                success: function (teamList) {
                                    // Все запросы успешны, выводим данные
                                    $('#ReceiptStorageId').removeAttr("disabled").fillSelect(accountOrganizationList);
                                    $('#TeamId').removeAttr("disabled").fillSelect(teamList, true);

                                    StopComboBoxProgress($("#ReceiptStorageId"));
                                    StopComboBoxProgress($("#TeamId"));
                                },
                                error: function (XMLHttpRequest, textStatus, thrownError) {
                                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                                }
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
                        }
                    });
                }
                else {
                    $('#ReceiptStorageId').clearSelect();
                }

                HideModal();
            });

            $("#CuratorName").attr("href", "/User/Details?id=" + $("#CuratorId").val() + GetBackUrl());

            $("#CuratorName").click(function () {
                var dealId = $("#DealId").val();
                var storageId = $("#ReceiptStorageId").val();

                if (dealId != "" && dealId != null && storageId != "" && storageId != null) {
                    Waybill_Edit.ShowCuratorSelectorForm(6/*WaybillTypeId*/, storageId, dealId, $(this), "messageReturnFromClientWaybillEdit");
                }
                else {
                    if (dealId == "" || dealId == null) {
                        $("#DealId").ValidationError("Укажите сдеклку.");
                    }
                    if (storageId == "" || storageId == null) {
                        $("#ReceiptStorageId").ValidationError("Укажите место хранения.");
                    }
                }
            });
        });
    },

    // при успешной попытке добавления/редактирования возвратной накладной
    OnSuccessReturnFromClientWaybillEdit: function (ajaxContext) {
        window.location = "/ReturnFromClientWaybill/Details?id=" + ajaxContext + "&backURL=" + $('#BackURL').val();
    },

    // при неудачной попытке добавления/редактирования приходной накладной
    OnFailReturnFromClientWaybillEdit: function (ajaxContext) {
        $('.field-validation-error').text("");
        ShowErrorMessage(ajaxContext.responseText, "messageReturnFromClientWaybillEdit");
    },

    OnSuccessReturnFromClientReasonEdit: function (ajaxContext) {
        $.ajax({
            type: "GET",
            url: "/ReturnFromClientWaybill/GetReturnFromClientReasonList",
            success: function (result) {
                $('#ReturnFromClientReasonId').fillSelect(result);
                $('#ReturnFromClientReasonId').attr('value', ajaxContext.Id);
                ShowSuccessMessage("Основание для возврата добавлено.", "messageReturnFromClientWaybillEdit");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillEdit");
            }
        });

        HideModal();
    }
};