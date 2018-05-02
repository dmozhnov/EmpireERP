var DealPaymentDocument_DealInitialBalanceCorrection_List = {

    OnDealDebitInitialBalanceCorrectionDeleteButtonClick: function (correctionId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealDebitInitialBalanceCorrection"));

            $.ajax({
                type: "POST",
                url: "/DealInitialBalanceCorrection/DeleteDealDebitInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (result) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                        HideModal();
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealDebitInitialBalanceCorrectionDetails");
                }
            });
        }
    },

    OnDealCreditInitialBalanceCorrectionDeleteButtonClick: function (correctionId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealCreditInitialBalanceCorrection"));

            $.ajax({
                type: "POST",
                url: "/DealInitialBalanceCorrection/DeleteDealCreditInitialBalanceCorrection",
                data: { correctionId: correctionId },
                success: function (result) {
                    RefreshGrid("gridDealInitialBalanceCorrection", function () {
                        ShowSuccessMessage("Корректировка удалена.", "messageDealInitialBalanceCorrectionList");
                        HideModal();
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealCreditInitialBalanceCorrectionDetails");
                }
            });
        }
    },

    OnSuccessDealDebitInitialBalanceCorrectionSave: function () {
        HideModal(function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                ShowSuccessMessage("Дебетовая корректировка сальдо сохранена.", "messageDealInitialBalanceCorrectionList");
            });
        });
    },

    OnSuccessDealCreditInitialBalanceCorrectionSave: function () {
        HideModal(function () {
            HideModal(function () {
                RefreshGrid("gridDealInitialBalanceCorrection", function () {
                    ShowSuccessMessage("Кредитовая корректировка сальдо сохранена.", "messageDealInitialBalanceCorrectionList");
                });
            });
        });
    }
};