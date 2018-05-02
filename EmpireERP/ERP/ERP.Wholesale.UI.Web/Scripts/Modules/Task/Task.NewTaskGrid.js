var Task_NewTaskGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridNewTask table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Topic").attr("href", "/Task/Details?id=" + id + GetBackUrl());

                var createdById = $(this).find(".CreatedById").text();
                $(this).find("a.CreatedBy").attr("href", "/User/Details?id=" + createdById + GetBackUrl());

                var executedById = $(this).find(".ExecutedById").text();
                $(this).find("a.ExecutedBy").attr("href", "/User/Details?id=" + executedById + GetBackUrl());
            });
        });
    },

    CreateNewTaskByCreatedBy: function () {
        window.location = "/Task/Create?" + GetBackUrl();
    },

    CreateNewTaskByExecutedBy: function (executedById) {
        window.location = "/Task/Create?executedById=" + executedById + "&" + GetBackUrl(true);
    },

    CreateNewTaskByContractor: function (contractorId) {
        window.location = "/Task/Create?contractorId=" + contractorId + "&" + GetBackUrl(true);
    },

    CreateNewTaskByDeal: function (dealId) {
        window.location = "/Task/Create?dealId=" + dealId + "&" + GetBackUrl(true);
    },

    CreateNewTaskByProductionOrder: function (productionOrderId) {
        window.location = "/Task/Create?productionOrderId=" + productionOrderId + "&" + GetBackUrl(true);
    }
};