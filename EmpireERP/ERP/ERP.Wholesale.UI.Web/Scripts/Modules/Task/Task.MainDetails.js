var Task_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            var id = $(this).find("#CreatedById").val();
            $(this).find("a#CreatedBy").attr("href", "/User/Details?id=" + id + GetBackUrl());

            id = $(this).find("#ExecutedById").val();
            $(this).find("a#ExecutedBy").attr("href", "/User/Details?id=" + id + GetBackUrl());

            id = $(this).find("#DealId").val();
            $(this).find("a#Deal").attr("href", "/Deal/Details?id=" + id + GetBackUrl());

            id = $(this).find("#ProductionOrderId").val();
            $(this).find("a#ProductionOrder").attr("href", "/ProductionOrder/Details?id=" + id + GetBackUrl());

            id = $(this).find("#ContractorId").val();
            var controller = "";
            switch ($(this).find("#ContractorType").val()) {
                case "1":
                    controller = "Provider";
                    break;
                case "2":
                    controller = "Client";
                    break;
                case "3":
                    controller = "Producer";
                    break;
            }
            $(this).find("a#Contractor").attr("href", "/" + controller + "/Details?id=" + id + GetBackUrl());
        });
    },

    RefreshDetails: function (errorMessageContainer, onComplete) {
        var taskId = $("#Id").val();
        $.ajax({
            type: "GET",
            url: "/Task/GetMainChangeableIndicators/",
            data: { taskId: taskId },
            success: function (result) {
                $("#StartDate").text(result.StartDate);
                $("#FactualCompletionDate").text(result.FactualCompletionDate);
                $("#FactualSpentTime").text(result.FactualSpentTime);
                $("#TaskExecutionStateName").text(result.ExecutionState);
                $("#CompletionPercentage").text(result.CompletionPercentage);

                onComplete();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, errorMessageContainer);
            }
        });
    }
};