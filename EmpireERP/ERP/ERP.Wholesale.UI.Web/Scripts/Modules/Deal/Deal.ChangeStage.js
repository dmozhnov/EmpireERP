var Deal_ChangeStage = {
    Init: function (ajaxContext) {
        $(document).ready(function () {
            $("#btnMoveToNextStage").click(function () {
                var dealId = $("#Id").val();
                var currentStageId = $("#dealChangeStage #CurrentStageId").val();
                StartButtonProgress($("#btnMoveToNextStage"));
                $.ajax({
                    type: "POST",
                    url: "/Deal/MoveToNextStage",
                    data: { dealId: dealId, currentStageId: currentStageId },
                    success: Deal_ChangeStage.OnSuccessChangeStage,
                    error: Deal_ChangeStage.OnFailChangeStage
                });
            });

            $("#btnMoveToPreviousStage").click(function () {
                if (confirm("Вы уверены?")) {
                    var dealId = $("#Id").val();
                    var currentStageId = $("#dealChangeStage #CurrentStageId").val();

                    StartButtonProgress($("#btnMoveToPreviousStage"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/MoveToPreviousStage",
                        data: { dealId: dealId, currentStageId: currentStageId },
                        success: Deal_ChangeStage.OnSuccessChangeStage,
                        error: Deal_ChangeStage.OnFailChangeStage
                    });
                }
            });

            $("#btnMoveToUnsuccessfulClosingStage").click(function () {
                if (confirm("Вы уверены?")) {
                    var dealId = $("#Id").val();
                    var currentStageId = $("#dealChangeStage #CurrentStageId").val();

                    StartButtonProgress($("#btnMoveToUnsuccessfulClosingStage"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/MoveToUnsuccessfulClosingStage",
                        data: { dealId: dealId, currentStageId: currentStageId },
                        success: Deal_ChangeStage.OnSuccessChangeStage,
                        error: Deal_ChangeStage.OnFailChangeStage
                    });
                }
            });

            $("#btnMoveToDecisionMakerSearchStage").click(function () {
                if (confirm("Вы уверены?")) {
                    var dealId = $("#Id").val();
                    var currentStageId = $("#dealChangeStage #CurrentStageId").val();

                    StartButtonProgress($("#btnMoveToDecisionMakerSearchStage"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/MoveToDecisionMakerSearchStage",
                        data: { dealId: dealId, currentStageId: currentStageId },
                        success: Deal_ChangeStage.OnSuccessChangeStage,
                        error: Deal_ChangeStage.OnFailChangeStage
                    });
                }
            });
        });
    },

    OnSuccessChangeStage: function (ajaxContext) {
        RefreshGrid("gridDealPayment", function () {
            RefreshGrid("gridDealInitialBalanceCorrection", function () {
                RefreshGrid("gridDealQuota", function () {
                    Deal_Details.RefreshMainDetailsAndPermissions(ajaxContext);
                    HideModal(function () {
                        ShowSuccessMessage("Этап сделки изменен.", "messageDealEdit");
                    });
                });
            });
        });
    },

    OnFailChangeStage: function (XMLHttpRequest, textStatus, thrownError) {
        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealStageChange");
    }
};