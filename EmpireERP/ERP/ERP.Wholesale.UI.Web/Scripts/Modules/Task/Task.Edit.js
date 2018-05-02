var Task_Edit = {
    Init: function () {
        $(document).ready(function () {
            Task_Edit.ApplyContractorType($("ContractorType").val());
        });

        $("#btnBack").live('click', function () {
            window.location = $("#BackURL").val();
        });

        $("#TaskTypeId").live('change', function () {
            var _this = $(this);
            var taskTypeId = $(this).attr("value");
            StartComboBoxProgress($("#TaskExecutionStateId"));

            $.ajax({
                type: "GET",
                url: "/Task/GetStates/",
                data: { taskTypeId: taskTypeId },
                success: function (result) {
                    $("#TaskExecutionStateId").fillSelect(result);
                    StopComboBoxProgress($("#TaskExecutionStateId"));
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskEdit");
                }
            });
        });

        $("#ExecutedBy").live('click', function () {
            $.ajax({
                type: "GET",
                url: "/User/SelectExecutedByForTask/",
                success: function (result) {
                    $('#executedBySelector').hide().html(result);
                    ShowModal("executedBySelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskEdit");
                }
            });
        });

        $("#executedBySelector .select_user").live('click', function () {
            var userId = $(this).parent("td").parent("tr").find(".Id").text();
            var userName = $(this).parent("td").parent("tr").find(".DisplayName").text();
            $("#ExecutedById").val(userId);
            $("#ExecutedBy").text(userName);

            HideModal();
        });

        $("#Contractor").live('click', function () {
            $.ajax({
                type: "GET",
                url: "/Contractor/SelectContractor/",
                success: function (result) {
                    $('#contractorSelector').hide().html(result);
                    ShowModal("contractorSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskEdit");
                }
            });
        });

        $("#contractorSelector .select_contractor").live('click', function () {
            var contractorId = $(this).parent("td").parent("tr").find(".Id").text();
            var contractorName = $(this).parent("td").parent("tr").find(".Name").text();
            var contractorType = $(this).parent("td").parent("tr").find(".TypeId").text();

            if ($("#ContractorId").val() != contractorId) {
                Task_Edit.OnClearDeal();
                Task_Edit.OnClearProductionOrder();
            }

            $("#ContractorId").val(contractorId);
            $("#Contractor").text(contractorName);
            $("#ClearContractor").show();
            Task_Edit.ApplyContractorType(contractorType);

            HideModal();
        });

        $("#Deal.select_link").live('click', function () {
            var contractorId = $("#ContractorId").val();
            $.ajax({
                type: "GET",
                url: contractorId == "" ? "/Deal/SelectDeal/" : "/Deal/SelectDealByClient/",
                data: contractorId == "" ? { activeOnly: true} : { clientId: contractorId, mode: "ForTask" },
                success: function (result) {
                    $('#dealSelector').hide().html(result);
                    ShowModal("dealSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskEdit");
                }
            });
        });

        $("#dealSelector .select_deal").live('click', function () {
            var dealId = $(this).parent("td").parent("tr").find(".Id").text();
            var dealName = $(this).parent("td").parent("tr").find(".Name").text();
            $("#DealId").val(dealId);
            $("#Deal").text(dealName);
            $("#ClearDeal").show();
            Task_Edit.OnDisableProductionOrder();

            $.ajax({
                type: "GET",
                url: "/Task/GetClientByDeal/",
                data: { dealId: dealId },
                success: function (result) {
                    $("#Contractor").text(result.ClientName);
                    $("#ContractorId").val(result.ClientId);
                    $("#ClearContractor").show();
                    HideModal();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskEdit");
                    HideModal();
                }
            });
        });

        $("#ProductionOrder.select_link").live('click', function () {
            var contractorId = $("#ContractorId").val();
            $.ajax({
                type: "GET",
                url: contractorId == "" ? "/ProductionOrder/SelectProductionOrder/" : "/ProductionOrder/SelectProductionOrderByProducer/",
                data: contractorId == "" ? { activeOnly: true} : { producerId: contractorId },
                success: function (result) {
                    $('#productionOrderSelector').hide().html(result);
                    ShowModal("productionOrderSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTaskEdit");
                }
            });
        });

        $("#productionOrderSelector .select").live('click', function () {
            var userId = $(this).parent("td").parent("tr").find(".Id").text();
            var userName = $(this).parent("td").parent("tr").find(".Name").text();
            var contractor = $(this).parent("td").parent("tr").find(".ProducerName").text();
            var contractorId = $(this).parent("td").parent("tr").find(".ProducerId").text();

            $("#ProductionOrderId").val(userId);
            $("#ProductionOrder").text(userName);
            $("#ClearProductionOrder").show();

            $("#Contractor").text(contractor);
            $("#ContractorId").val(contractorId);
            $("#ClearContractor").show();

            Task_Edit.OnDisableDeal();

            HideModal();
        });

        $("#ClearDeal").live("click", function () {
            StopLinkProgress($(this));
            Task_Edit.OnClearDeal();
        });

        $("#ClearProductionOrder").live("click", function () {
            StopLinkProgress($(this));
            Task_Edit.OnClearProductionOrder();
        });

        $("#ClearContractor").live("click", function () {
            StopLinkProgress($(this));
            Task_Edit.OnClearLinking();
        });
    },

    ApplyContractorType: function (contractorType) {
        switch (contractorType) {
            case "1":   // Поставщик
                Task_Edit.OnDisableProductionOrder();
                Task_Edit.OnDisableDeal();
                break;
            case "2":   // Клиент
                Task_Edit.OnDisableProductionOrder();
                break;
            case "3":   // Производитель
                Task_Edit.OnDisableDeal();
                break;
        }
    },

    OnClearLinking: function () {
        Task_Edit.OnClearDeal();
        Task_Edit.OnClearProductionOrder();
        Task_Edit.OnClearContractor();
    },

    OnClearContractor: function () {
        $("#Contractor").addClass("select_link").text("Выберите контрагента");
        $("#ContractorId").val("");
        $("#ClearContractor").hide();
    },

    OnClearDeal: function () {
        $("#Deal").addClass("select_link").text("Выберите сделку");
        $("#DealId").val("");
        $("#ClearDeal").hide();
    },


    OnDisableDeal: function () {
        $("#Deal").removeClass("select_link").text("---");
        $("#DealId").val("");
        $("#ClearDeal").hide();
    },

    OnClearProductionOrder: function () {
        $("#ProductionOrder").addClass("select_link").text("Выберите заказ на производство");
        $("#ProductionOrderId").val("");
        $("#ClearProductionOrder").hide();
    },

    OnDisableProductionOrder: function () {
        $("#ProductionOrder").removeClass("select_link").text("---");
        $("#ProductionOrderId").val("");
        $("#ClearProductionOrder").hide();
    },

    OnFailTaskSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageTaskEdit");
    },

    OnSuccessTaskSave: function (ajaxContext) {
        // TODO: если !model.IsValid, контроллер возвращает success, но с моделью вместо id созданной накл. Тогда не надо переходить.
        if ($("#Id").val() == 0) {
            window.location = "/Task/Details?id=" + ajaxContext + GetBackUrlFromString($("#BackURL").val());
        }
        else {
            window.location = $("#BackURL").val();
        }
    }
};