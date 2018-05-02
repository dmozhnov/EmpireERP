var Task_TaskExecutionEdit = {
    Init: function () {
        $(document).ready(function () {
            $(".btnCancel").click(function () {
                var editDiv = $(this).parent("div").parent("div").parent("form").parent("div");
                var detailsDiv = editDiv.parent("div").find(".taskExecutionDetails");

                detailsDiv.show();
                editDiv.html("");
            });

            $("#Date").datepicker();
        });
    },

    OnBeginTaskExecutionEdit: function (ajaxContext) {
        StartButtonProgress($("#btnSaveTaskExecution"));
    },

    OnSuccessTaskExecutionEdit: function (ajaxContext) {
        var taskId = $("#TaskId").val();
        var container = $(".TaskExecutionId_" + ajaxContext.TaskExecutionId).parent("form").parent("div").parent("div");

        $.ajax({
            type: "GET",
            url: "/Task/GetTaskExecution",
            data: { taskId: taskId, taskExecutionId: ajaxContext.TaskExecutionId },
            success: function (result) {
                Task_MainDetails.RefreshDetails("messageTaskExecutionDetailsEdit", function () {
                    container.replaceWith(result);
                    ShowSuccessMessage(ajaxContext.Message, "messageTaskExecutionDetailsEdit");
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskExecutionDetailsEdit");
            }
        });
    },

    OnFailTaskExecutionEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageTaskExecutionDetailsEdit");
    }
};