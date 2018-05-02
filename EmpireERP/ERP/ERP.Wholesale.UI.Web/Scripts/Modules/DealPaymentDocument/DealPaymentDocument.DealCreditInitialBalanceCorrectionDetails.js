var DealPaymentDocument_DealCreditInitialBalanceCorrectionDetails = {
    Init: function () {
        $(function () {
            var currentUrl = $("#currentUrl").val();

            if (IsTrue($("#AllowToViewDealDetails").val())) {
                var dealId = $("#dealPaymentDocumentDetails #DealId").val();
                $("#DealName").attr("href", "/Deal/Details?id=" + dealId + "&backURL=" + currentUrl);
            }
            else {
                $("#DealName").addClass("disabled");
            }
            if (IsTrue($("#AllowToViewTeamDetails").val())) {
                var teamId = $("#dealPaymentDocumentDetails #TeamId").val();
                $("#TeamName").attr("href", "/Team/Details?id=" + teamId + "&backURL=" + currentUrl);
            }
            else {
                $("#TeamName").addClass("disabled");
            }

            $("#dealPaymentDocumentDetails #btnDeleteDealCreditInitialBalanceCorrection").bind("click", function () {
                var correctionId = $("#dealPaymentDocumentDetails #DealCreditInitialBalanceCorrectionId").val();
                OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId);
            });
        });
    }
};