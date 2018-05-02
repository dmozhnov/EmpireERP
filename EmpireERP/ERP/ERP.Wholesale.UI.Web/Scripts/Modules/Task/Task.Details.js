var Task_Details = {
    Init: function () {
        $(document).ready(function () {
            $("#taskExecutionTab").click(function () {
                var _this = $(this);
                StartLinkProgress(_this.children("span"));
                var taskId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/Task/GetTaskExecutions/",
                    data: { taskId: taskId },
                    success: function (result) {
                        $("#taskDetailsContainer").html(result);
                        Task_Executions.OnLoadExecutions(); // Проставляем ссылки
                        $("#taskHistoryTab").removeClass("selected");
                        _this.addClass("selected");
                        StopLinkProgress(_this.children("span"));
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskDetailsContainer");
                    }
                });
            });

            $("#taskHistoryTab").click(function () {
                var _this = $(this);
                StartLinkProgress(_this.children("span"));
                var taskId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/Task/GetTaskHistory/",
                    data: { taskId: taskId },
                    success: function (result) {
                        $("#taskDetailsContainer").html(result);
                        $("#taskExecutionTab").removeClass("selected");
                        _this.addClass("selected");
                        StopLinkProgress(_this.children("span"));
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskDetailsContainer");
                    }
                });
            });

            $("#btnEditTask").click(function () {
                var taskId = $("#Id").val();
                window.location = "/Task/Edit?taskId=" + taskId + GetBackUrl();
            });

            $("#btnCreateTaskExecution").click(function () {
                StartButtonProgress($(this));
                var taskId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/Task/TaskExecutionCreate/",
                    data: { taskId: taskId },
                    success: function (result) {
                        $('#taskExecutionCreate').hide().html(result);
                        $.validator.unobtrusive.parse($("#taskExecutionCreate"));
                        $('#taskExecutionCreate').show();
                        StopButtonProgress($(this));
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskExecutionCreateButton");
                    }
                });
            });

            $("#btnDeleteTask").click(function () {
                if (confirm("Вы действительно хотите удалить задачу?")) {
                    StartButtonProgress($(this));
                    var taskId = $("#Id").val();

                    $.ajax({
                        type: "GET",
                        url: "/Task/Delete/",
                        data: { taskId: taskId },
                        success: function (result) {
                            window.location = $("#BackURL").val();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskDetails");
                        }
                    });
                }
            });

            $("#btnBackTo").click(function () {
                window.location = $("#BackURL").val();
            });

            $("#btnCompleteTask").click(function () {
                StartButtonProgress($(this));
                var taskId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/Task/CompleteTask/",
                    data: { taskId: taskId },
                    success: function (result) {
                        $('#taskExecutionCreate').hide().html(result);
                        $.validator.unobtrusive.parse($("#taskExecutionCreate"));
                        $('#taskExecutionCreate').show();
                        StopButtonProgress($(this));
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskExecutionCreateButton");
                    }
                });
            });
        });
    }
};