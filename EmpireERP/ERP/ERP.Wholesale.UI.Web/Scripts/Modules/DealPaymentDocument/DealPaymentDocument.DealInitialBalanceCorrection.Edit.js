var DealPaymentDocument_DealInitialBalanceCorrection_Edit = {
    Init: function (modalFormId, dealSelectionMode, errorMessageId) {
        $(document).ready(function () {
            // Открытие формы выбора клиента
            $("#" + modalFormId + " #ClientName").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Client/SelectClient",
                    success: function (result) {
                        $("#clientSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#clientSelector"));
                        ShowModal("clientSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, errorMessageId);
                    }
                });
            });

            // Обработка выбора клиента
            $("#clientSelector .select_client").live("click", function () {
                var clientId = $(this).findCell(".Id").text();
                var clientName = $(this).findCell(".Name").text();

                $("#" + modalFormId + " #ClientName").text(clientName);
                $("#" + modalFormId + " #ClientId").val(clientId);

                $("#" + modalFormId + " #ClientId").ValidationValid();

                // Сбрасываем сделку
                $("#" + modalFormId + " #DealName").text("Выберите сделку");
                $("#" + modalFormId + " #DealId").val("");

                HideModal();
            });

            // Открытие формы выбора сделки
            $("#" + modalFormId + " #DealName").click(function () {
                var isDealSelectedByClient = $("#" + modalFormId + " #IsDealSelectedByClient").val();

                if (IsTrue(isDealSelectedByClient)) {
                    var clientId = $("#" + modalFormId + " #ClientId").val();

                    if (IsDefaultOrEmpty(clientId)) {
                        StopLinkProgress();
                        $("#" + modalFormId + " #ClientId").ValidationError("Укажите клиента");
                        return false;
                    }

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClient",
                        data: { clientId: clientId, mode: dealSelectionMode },
                        success: function (result) {
                            $("#dealSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealSelector"));
                            ShowModal("dealSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, errorMessageId);
                        }
                    });
                }
                else {
                    var clientOrganizationId = $("#" + modalFormId + " #ClientOrganizationId").val();

                    $.ajax({
                        type: "GET",
                        url: "/Deal/SelectDealByClientOrganization",
                        data: { clientOrganizationId: clientOrganizationId, mode: dealSelectionMode },
                        success: function (result) {
                            $("#dealSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#dealSelector"));
                            ShowModal("dealSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, errorMessageId);
                        }
                    });
                }
            });
        });
    }
};