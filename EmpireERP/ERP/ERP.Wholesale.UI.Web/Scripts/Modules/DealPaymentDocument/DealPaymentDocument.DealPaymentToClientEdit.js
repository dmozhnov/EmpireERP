var DealPaymentDocument_DealPaymentToClientEdit = {
    Init: function () {
        $(document).ready(function () {
            $("#ReturnedById").attr("disabled", "disabled");
            
            // связывание списков команд и пользователей
            $('#TeamId').FillChildComboBox('ReturnedById', "/User/GetListByTeamForDealPayment", 'teamId', "messageDealPaymentToClientEdit");

            if ($("#DealId").val() == "0") {
                $("#TeamId").attr("disabled", "disabled");  //Блокируем выбор команды до выбора сделки
            }

            if ($("#TeamId").val() != "0" && $("#TeamId").val() != null && $("#TeamId").val() != "") {
                $("#dealPaymentToClientEdit #TeamId").trigger("change");
            }
            
            $("#dealPaymentToClientEdit #ClientName").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Client/SelectClient",
                    success: function (result) {
                        $("#clientSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#clientSelector"));
                        ShowModal("clientSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentToClientEdit");
                    }
                });
            });

            $("#clientSelector .select_client").live("click", function () {
                var clientId = $(this).parent("td").parent("tr").find(".Id").text();
                var clientName = $(this).parent("td").parent("tr").find(".Name").text();

                $("#dealPaymentToClientEdit #ClientName").text(clientName);
                $("#dealPaymentToClientEdit #ClientId").val(clientId);

                $("#dealPaymentToClientEdit #ClientId").ValidationValid();

                // Сбрасываем значения полей
                $("#dealPaymentToClientEdit #DealName").text("Выберите сделку");
                $("#dealPaymentToClientEdit #DealId").val("");

                $("#TeamId").clearSelect();
                $("#TeamId").attr("disabled", "disabled");

                $("#ReturnedById").clearSelect();
                $("#ReturnedById").attr("disabled", "disabled");

                HideModal();
            });

            $("#dealPaymentToClientEdit #DealName").click(function () {
                var isDealSelectedByClient = $("#dealPaymentToClientEdit #IsDealSelectedByClient").val();

                if (IsTrue(isDealSelectedByClient)) {
                    var clientId = $("#dealPaymentToClientEdit #ClientId").val();

                    if (IsDefaultOrEmpty(clientId)) {
                        StopLinkProgress();
                        $("#dealPaymentToClientEdit #ClientId").ValidationError("Укажите клиента");
                        return false;
                    }

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClient",
                        data: { clientId: clientId, mode: "ForPaymentToClient" },
                        success: function (result) {
                            $("#dealSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealSelector"));
                            ShowModal("dealSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentToClientEdit");
                        }
                    });
                }
                else {
                    var clientOrganizationId = $("#dealPaymentToClientEdit #ClientOrganizationId").val();

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClientOrganization",
                        data: { clientOrganizationId: clientOrganizationId, mode: "ForPaymentToClient" },
                        success: function (result) {
                            $("#dealSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealSelector"));
                            ShowModal("dealSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentToClientEdit");
                        }
                    });
                }
            });

            $("#dealSelector .select_deal").live("click", function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                var name = $(this).parent("td").parent("tr").find(".Name").text();

                $("#dealPaymentToClientEdit #DealName").text(name);
                $("#dealPaymentToClientEdit #DealId").val(id);


                StartComboBoxProgress($("#TeamId"))
                $.ajax({
                    type: "GET",
                    url: "/Deal/GetTeamListForDealDocument",
                    data: { dealId: id },
                    success: function (result) {
                        $("#TeamId").clearSelect().fillSelect(result, true).removeAttr("disabled");
                        StopComboBoxProgress($("#TeamId"));

                        if ($("#TeamId").val() != "") {
                            $("#dealPaymentToClientEdit #TeamId").trigger("change");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentToClientEdit");
                    }
                });

                HideModal();
            });
        });
    },

    OnBeginDealPaymentToClientSave: function () {
        StartButtonProgress($("#dealPaymentToClientEdit #btnSave"));
    },

    OnFailDealPaymentToClientSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDealPaymentToClientEdit");
    }
};