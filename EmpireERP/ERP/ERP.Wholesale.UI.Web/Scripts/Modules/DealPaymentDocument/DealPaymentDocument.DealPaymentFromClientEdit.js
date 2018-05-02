var DealPaymentDocument_DealPaymentFromClientEdit = {
    Init: function () {
        $(document).ready(function () {
            // Открытие формы выбора клиента
            $("#dealPaymentFromClientEdit #ClientName").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Client/SelectClient",
                    success: function (result) {
                        $("#clientSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#clientSelector"));
                        ShowModal("clientSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentFromClientEdit");
                    }
                });
            });

            // Обработка выбора клиента
            $("#clientSelector .select_client").live("click", function () {
                var clientId = $(this).parent("td").parent("tr").find(".Id").text();
                var clientName = $(this).parent("td").parent("tr").find(".Name").text();

                $("#dealPaymentFromClientEdit #ClientName").text(clientName);
                $("#dealPaymentFromClientEdit #ClientId").val(clientId);

                $("#dealPaymentFromClientEdit #ClientId").ValidationValid();

                // Сбрасываем сделку
                $("#dealPaymentFromClientEdit #DealName").text("Выберите сделку");
                $("#dealPaymentFromClientEdit #DealId").val("");

                HideModal();
            });

            // Открытие формы выбора сделки
            $("#dealPaymentFromClientEdit #DealName").click(function () {
                var clientId = $("#dealPaymentFromClientEdit #ClientId").val();

                if (!IsDefaultOrEmpty(clientId)) {
                    StartLinkProgress($(this));

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClient",
                        data: { clientId: clientId, mode: "ForPaymentFromClient" },
                        success: function (result) {
                            $("#dealSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealSelector"));
                            ShowModal("dealSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentFromClientEdit");
                        }
                    });
                }
                else {
                    $("#dealPaymentFromClientEdit #ClientId").ValidationError("Укажите клиента");
                }
            });

            // Обработка выбора сделки
            $("#dealSelector .select_deal").live("click", function () {
                var dealId = $(this).parent("td").parent("tr").find(".Id").text();
                var dealName = $(this).parent("td").parent("tr").find(".Name").text();

                $("#dealPaymentFromClientEdit #DealName").text(dealName);
                $("#dealPaymentFromClientEdit #DealId").val(dealId);

                HideModal();
            });
        });
    },

    OnBeginSelectDestinationDocumentsButtonClick: function () {
        StartButtonProgress($("#dealPaymentFromClientEdit #btnSelectDestinationDocuments"));
    },

    OnFailSelectDestinationDocumentsButtonClick: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDealPaymentFromClientEdit");
    },

    OnSuccessSelectDestinationDocumentsButtonClick: function (ajaxContext) {
        $("#destinationDocumentSelectorForDealPaymentFromClientDistribution").html(ajaxContext);
        $.validator.unobtrusive.parse($("#destinationDocumentSelectorForDealPaymentFromClientDistribution"));
        ShowModal("destinationDocumentSelectorForDealPaymentFromClientDistribution");
    }
};