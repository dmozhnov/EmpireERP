var Task_CompletedTaskGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridCompletedTask table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Topic").attr("href", "/Task/Details?id=" + id + GetBackUrl());

                var createdById = $(this).find(".CreatedById").text();
                $(this).find("a.CreatedBy").attr("href", "/User/Details?id=" + createdById + GetBackUrl());

                var executedById = $(this).find(".ExecutedById").text();
                $(this).find("a.ExecutedBy").attr("href", "/User/Details?id=" + createdById + GetBackUrl());
            });
        });
    }
};