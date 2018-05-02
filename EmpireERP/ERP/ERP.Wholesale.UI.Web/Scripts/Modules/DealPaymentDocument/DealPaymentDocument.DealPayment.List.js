var DealPaymentDocument_DealPayment_List = {

    OnDealPaymentFromClientDeleteButtonClick: function (paymentId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealPaymentFromClient"));
            
            $.ajax({
                type: "POST",
                url: "/DealPayment/DeleteDealPaymentFromClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                        HideModal();
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentFromClientDetails");
                }
            });
        }
    },

    OnDealPaymentToClientDeleteButtonClick: function (paymentId) {
        if (confirm("Вы уверены?")) {
            StartButtonProgress($("#dealPaymentDocumentDetails #btnDeleteDealPaymentToClient"));
            
            $.ajax({
                type: "POST",
                url: "/DealPayment/DeleteDealPaymentToClient",
                data: { paymentId: paymentId },
                success: function (result) {
                    RefreshGrid("gridDealPayment", function () {
                        ShowSuccessMessage("Оплата удалена.", "messageDealPaymentList");
                        HideModal();
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentToClientDetails");
                }
            });
        }
    },

    OnSuccessClientOrganizationPaymentFromClientSave: function () {
        HideModal(function () {
            HideModal(function () {
                RefreshGrid("gridDealPayment", function () {
                    ShowSuccessMessage("Оплата сохранена.", "messageDealPaymentList");
                });
            });
        });
    },

    OnSuccessDealPaymentFromClientSave: function () {
        HideModal(function () {
            HideModal(function () {
                RefreshGrid("gridDealPayment", function () {
                    ShowSuccessMessage("Оплата сохранена.", "messageDealPaymentList");
                });
            });
        });
    },

    OnSuccessDealPaymentToClientSave: function () {
        HideModal(function () {
            RefreshGrid("gridDealPayment", function () {
                ShowSuccessMessage("Оплата сохранена.", "messageDealPaymentList");
            });
        });
    }
};