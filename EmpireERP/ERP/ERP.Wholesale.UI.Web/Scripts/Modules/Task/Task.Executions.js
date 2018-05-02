var Task_Executions = {
    Init: function () {
        Task_Executions.OnLoadExecutions();

        $(".EditTaskExecution").live("click", function () {
            StartLinkProgress($(this));
            var detailsDiv = $(this).parent("td").parent("tr").parent("tbody").parent("table").parent("div");
            var editDiv = detailsDiv.parent("div").find(".taskExecutionEdit");
            var taskExecutionId = detailsDiv.find("#item_TaskExecutionId").val();

            $.ajax({
                type: "GET",
                url: "/Task/TaskExecutionEdit",
                data: { taskExecutionId: taskExecutionId },
                success: function (result) {
                    editDiv.hide().html(result);
                    $.validator.unobtrusive.parse(editDiv);
                    detailsDiv.hide();
                    editDiv.show();
                    StopLinkProgress($(this));
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskExecutionDetailsEdit");
                }
            });
        });

        $(".DeleteTaskExecution").live("click", function () {
            if (confirm("Вы действительно хотите удалить исполнение?")) {
                StartLinkProgress($(this));
                var content = $(this).parent("td").parent("tr").parent("tbody").parent("table").parent("div").parent("div");
                var taskExecutionId = content.find("#item_TaskExecutionId").val();

                $.ajax({
                    type: "GET",
                    url: "/Task/TaskExecutionDelete",
                    data: { taskExecutionId: taskExecutionId },
                    success: function (result) {
                        Task_MainDetails.RefreshDetails("messageTaskExecutionDetailsEdit", function () {
                            var needRemoveDelim = content.children(".h_delim").length == 0;
                            var container = content.parent("div");
                            content.remove();
                            if (needRemoveDelim) {
                                container.find(".h_delim:first").remove();
                            }
                            if ($("#taskDetailsContainer .taskExecutionDetails").length == 0) {
                                $("#taskDetailsContainer").append("<div id='TaskExecutionNoDataMessage' style='text-align: center'>Нет исполнений</div>");
                            }
                            ShowSuccessMessage(result, "messageTaskExecutionDetailsEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskExecutionDetailsEdit");
                    }
                });
            }
        });
    },

    OnLoadExecutions: function () {
        $(document).ready(function () {
            $("#taskDetailsContainer .taskExecutionDetails").each(function (i, el) {
                var id = $(this).find("#CreatedById").val();
                $(this).find("a#CreatedByName").attr("href", "/User/Details?id=" + id + GetBackUrl());
            });
        }); // end document.ready
    }
};