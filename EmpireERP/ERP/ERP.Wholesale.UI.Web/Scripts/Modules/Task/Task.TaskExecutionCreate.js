var Task_TaskExecutionCreate = {
    Init: function () {
        $(document).ready(function () {
            $("#btnCancel").click(function () {
                $("#taskExecutionCreate").html("");
            });

            $("#Date").datepicker();
        });
    },

    OnBeginTaskExecutionCreate: function (ajaxContext) {
        StartButtonProgress($("#btnSaveTaskExecution"));
    },

    OnSuccessTaskExecutionCreate: function (ajaxContext) {
        var taskId = $("#Id").val();
        var url = "";
        if ($("#taskExecutionTab").parent("div").hasClass("selected")) {
            url = "/Task/GetTaskExecution/";
        }
        else {
            url = "/Task/GetTaskHistoryForTaskExecution/";
        }

        $.ajax({
            type: "GET",
            url: url,
            data: { taskId: taskId, taskExecutionId: ajaxContext.TaskExecutionId },
            success: function (result) {
                Task_MainDetails.RefreshDetails("messageTaskExecutionDetailsEdit", function () {
                    var baseHtml = $("#taskDetailsContainer").html();
                    $("#taskDetailsContainer").html(baseHtml + result);
                    $("#taskExecutionCreate").html("");
                    $("#taskDetailsContainer #TaskHistoryNoDataMessage").remove();
                    $("#taskDetailsContainer #TaskExecutionNoDataMessage").remove();
                    Task_Executions.OnLoadExecutions(); // Проставляем ссылки
                    ShowSuccessMessage(ajaxContext.Message, "messageTaskExecutionDetailsEdit");
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskExecutionDetailsEdit");
            }
        });

    },

    OnFailTaskExecutionCreate: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageTaskExecutionDetailsEdit");
    }
};