var Task_List = {
    Init: function () {
        $("#CreatedBy").live('click', function () {
            $.ajax({
                type: "GET",
                url: "/User/SelectUserForTask/",
                data: { isExecutedBy: false },
                success: function (result) {
                    $('#createdByFilterSelector').hide().html(result);
                    ShowModal("createdByFilterSelector");
                    $('#createdByFilterSelector .attention').hide();
                    $("#createdByFilterSelector").css("top", "50px");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskList");
                }
            });
        });

        $("#createdByFilterSelector .select_user").live('click', function () {
            var userId = $(this).parent("td").parent("tr").find(".Id").text();
            var userName = $(this).parent("td").parent("tr").find(".DisplayName").text();
            $("#CreatedBy").attr("selected_id", userId);
            $("#CreatedBy").text(userName);

            HideModal();
        });

        $("#ExecutedBy").live('click', function () {
            $.ajax({
                type: "GET",
                url: "/User/SelectUserForTask/",
                data: { isExecutedBy: true },
                success: function (result) {
                    $('#executedByFilterSelector').hide().html(result);
                    ShowModal("executedByFilterSelector");
                    $('#executedByFilterSelector .attention').hide();
                    $("#executedByFilterSelector").css("top", "50px");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskList");
                }
            });
        });

        $("#executedByFilterSelector .select_user").live('click', function () {
            var userId = $(this).parent("td").parent("tr").find(".Id").text();
            var userName = $(this).parent("td").parent("tr").find(".DisplayName").text();
            $("#ExecutedBy").attr("selected_id", userId);
            $("#ExecutedBy").text(userName);

            HideModal();
        });

        $("#Contractor").live('click', function () {
            $.ajax({
                type: "GET",
                url: "/Contractor/SelectContractor/",
                success: function (result) {
                    $('#contractorFilterSelector').hide().html(result);
                    ShowModal("contractorFilterSelector");
                    $('#contractorFilterSelector .attention').hide();
                    $("#contractorFilterSelector").css("top", "50px");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskList");
                }
            });
        });

        $("#contractorFilterSelector .select_contractor").live('click', function () {
            var userId = $(this).parent("td").parent("tr").find(".Id").text();
            var userName = $(this).parent("td").parent("tr").find(".Name").text();
            $("#Contractor").attr("selected_id", userId);
            $("#Contractor").text(userName);

            HideModal();
        });

        $("#Deal").live('click', function () {
            $.ajax({
                type: "GET",
                url: "/Deal/SelectDeal/",
                data: { activeOnly: false },
                success: function (result) {
                    $('#dealFilterSelector').hide().html(result);
                    ShowModal("dealFilterSelector");
                    $('#dealFilterSelector .attention').hide();
                    $("#dealFilterSelector").css("top", "50px");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskList");
                }
            });
        });

        $("#dealFilterSelector .select_deal").live('click', function () {
            var userId = $(this).parent("td").parent("tr").find(".Id").text();
            var userName = $(this).parent("td").parent("tr").find(".Name").text();
            $("#Deal").attr("selected_id", userId);
            $("#Deal").text(userName);

            HideModal();
        });

        $("#ProductionOrder").live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/SelectProductionOrder/",
                data: { activeOnly: false },
                success: function (result) {
                    $('#productionOrderFilterSelector').hide().html(result);
                    ShowModal("productionOrderFilterSelector");
                    $('#productionOrderFilterSelector .attention').hide();
                    $("#productionOrderFilterSelector").css("top", "50px");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskList");
                }
            });
        });

        $("#productionOrderFilterSelector .select").live('click', function () {
            var userId = $(this).parent("td").parent("tr").find(".Id").text();
            var userName = $(this).parent("td").parent("tr").find(".Name").text();
            $("#ProductionOrder").attr("selected_id", userId);
            $("#ProductionOrder").text(userName);

            HideModal();
        });

        // Подгрузка списка статусов в фильтре
        $("#Type").live("change", function () {
            StartComboBoxProgress($("#ExecutionState"));
            var taskTypeId = $(this).attr("value");

            if (taskTypeId != "") {
                $.ajax({
                    type: "GET",
                    url: "/Task/GetStates/",
                    data: { taskTypeId: taskTypeId },
                    success: function (result) {
                        $("#ExecutionState").fillSelect(result);
                        StopComboBoxProgress($("#ExecutionState"));
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskList");
                    }
                });
            }
            else {
                $("#ExecutionState").clearSelect();
                StopComboBoxProgress($("#ExecutionState"));
            }
        });
    }
};