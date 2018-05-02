var DealPaymentDocument_DealDebitInitialBalanceCorrection_Edit = {
    Init: function () {
        $(document).ready(function () {
            if ($("#DealId").val() == "0") {
                $("#TeamId").attr("disabled", "disabled");  //Блокируем выбор команды до выбора сделки
            }
            DealPaymentDocument_DealInitialBalanceCorrection_Edit.Init("dealDebitInitialBalanceCorrectionEdit",
                "ForDealDebitInitialBalanceCorrection", "messageDealDebitInitialBalanceCorrectionEdit");
        });

        // Обработка выбора сделки
        $("#dealSelector .select_deal").live("click", function () {
            var dealId = $(this).findCell(".Id").text();
            var dealName = $(this).findCell(".Name").text();

            $("#dealDebitInitialBalanceCorrectionEdit #DealName").text(dealName);
            $("#dealDebitInitialBalanceCorrectionEdit #DealId").val(dealId);

            var teamComboBox = $("#TeamId");
            StartComboBoxProgress(teamComboBox)
            $.ajax({
                type: "GET",
                url: "/Deal/GetTeamListForDealDocument",
                data: { dealId: dealId },
                success: function (result) {
                    $("#TeamId").fillSelect(result, true).removeAttr("disabled");
                    StopComboBoxProgress(teamComboBox);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "errorMessageId");
                }
            });
            HideModal();
        });
    },

    OnBeginDealDebitInitialBalanceCorrectionSave: function () {
        StartButtonProgress($("#dealDebitInitialBalanceCorrectionEdit #btnSave"));
    },

    OnFailDealDebitInitialBalanceCorrectionSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDealDebitInitialBalanceCorrectionEdit");
    }
};