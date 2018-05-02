var Task_TaskHistory = {
    Init: function () {
        $(document).ready(function () {
            $("#taskDetailsContainer .taskHistoryItem").each(function (i, el) {
                var id = $(this).find("#CreatedById").val();
                $(this).find("a#CreatedByName").attr("href", "/User/Details?id=" + id + GetBackUrl());

                id = $(this).find("#ContractorOldId").val();
                $(this).find("a#ContractorNameOld").attr("href", "/" + Task_TaskHistory.GetContractorController($(this), "ContractorTypeOld") + "/Details?id=" + id + GetBackUrl());

                id = $(this).find("#ContractorNewId").val();
                $(this).find("a#ContractorNameNew").attr("href", "/" + Task_TaskHistory.GetContractorController($(this), "ContractorTypeNew") + "/Details?id=" + id + GetBackUrl());

                id = $(this).find("#DealOldId").val();
                $(this).find("a#DealNameOld").attr("href", "/Deal/Details?id=" + id + GetBackUrl());

                id = $(this).find("#DealNewId").val();
                $(this).find("a#DealNameNew").attr("href", "/Deal/Details?id=" + id + GetBackUrl());

                id = $(this).find("#ExecutedByOldId").val();
                $(this).find("a#ExecutedByNameOld").attr("href", "/User/Details?id=" + id + GetBackUrl());

                id = $(this).find("#ExecutedByNewId").val();
                $(this).find("a#ExecutedByNameNew").attr("href", "/User/Details?id=" + id + GetBackUrl());

                id = $(this).find("#ProductionOrderOldId").val();
                $(this).find("a#ProductionOrderNameOld").attr("href", "/ProductionOrder/Details?id=" + id + GetBackUrl());

                id = $(this).find("#ProductionOrderNewId").val();
                $(this).find("a#ProductionOrderNameNew").attr("href", "/ProductionOrder/Details?id=" + id + GetBackUrl());

                id = $(this).find("#TaskExecutionCreatedById").val();
                $(this).find("a#ProductionOrderNameNew").attr("href", "/ProductionOrder/Details?id=" + id + GetBackUrl());

                id = $(this).find("#TaskExecutionCreatedById").val();
                $(this).find("a#TaskExecutionCreatedByName").attr("href", "/User/Details?id=" + id + GetBackUrl());
            });
        });
    },

    GetContractorController: function (parent, id) {
        var controller = "";
        switch (parent.find("#" + id).val()) {
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

        return controller;
    }
};