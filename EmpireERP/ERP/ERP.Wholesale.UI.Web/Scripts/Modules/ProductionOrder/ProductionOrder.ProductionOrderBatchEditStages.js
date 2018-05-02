var ProductionOrder_ProductionOrderBatchEditStages = {
    Init: function () {
        $(document).ready(function () {

            $("#linkLoadFromTemplate").click(function () {
                StartLinkProgress($(this));
                var id = $("#ProductionOrderBatchLifeCycleTemplateId").val();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrderBatchLifeCycleTemplate/SelectProductionOrderBatchLifeCycleTemplate",
                    success: function (result) {
                        $("#productionOrderBatchLifeCycleTemplateSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchLifeCycleTemplateSelector"));
                        ShowModal("productionOrderBatchLifeCycleTemplateSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEditStages");
                    }
                });
            });

            $("#gridProductionOrderBatchStage .linkAddStage").live("click", function () {
                var productionOrderBatchId = $("#Id").val();
                var idPreviousStage = $(this).parent("td").parent("tr").find(".Id").text();
                var position = $(this).parent("td").parent("tr").find(".OrdinalNumber").text();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/AddStage",
                    data: { productionOrderBatchId: productionOrderBatchId, idPreviousStage: idPreviousStage, position: position },
                    success: function (result) {
                        $("#productionOrderBatchStageEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchStageEdit"));
                        ShowModal("productionOrderBatchStageEdit");
                        $("#productionOrderBatchStageEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEditStages");
                    }
                });
            });

            $("#gridProductionOrderBatchStage .linkEditStage").live("click", function () {
                var productionOrderBatchId = $("#Id").val();
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditStage",
                    data: { productionOrderBatchId: productionOrderBatchId, id: id },
                    success: function (result) {
                        $("#productionOrderBatchStageEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchStageEdit"));
                        ShowModal("productionOrderBatchStageEdit");
                        $("#productionOrderBatchStageEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEditStages");
                    }
                });
            });

            $("#gridProductionOrderBatchStage .linkDeleteStage").live("click", function () {
                if (confirm('Вы уверены?')) {
                    var productionOrderBatchId = $("#Id").val();
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/DeleteStage",
                        data: { productionOrderBatchId: productionOrderBatchId, id: id },
                        success: function (result) {
                            RefreshOrderBatchAfterModifiedStages(result, function () {
                                ShowSuccessMessage("Этап удален.", "messageProductionOrderBatchEditStages");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEditStages");
                        }
                    });
                }
            });

            $("#gridProductionOrderBatchStage .linkMoveStageUp").live("click", function () {
                var productionOrderBatchId = $("#Id").val();
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/MoveStageUp",
                    data: { productionOrderBatchId: productionOrderBatchId, id: id },
                    success: function (result) {
                        $(".main_details_table #ProducingPendingDate").text(result.producingPendingDate);
                        RefreshGrid("gridProductionOrderBatchStage", function () {
                            RefreshExecutionGraph();
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEditStages");
                    }
                });
            });

            $("#gridProductionOrderBatchStage .linkMoveStageDown").live("click", function () {
                var productionOrderBatchId = $("#Id").val();
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/MoveStageDown",
                    data: { productionOrderBatchId: productionOrderBatchId, id: id },
                    success: function (result) {
                        $(".main_details_table #ProducingPendingDate").text(result.producingPendingDate);
                        RefreshGrid("gridProductionOrderBatchStage", function () {
                            RefreshExecutionGraph();
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEditStages");
                    }
                });
            });

            $("#linkClearCustomStages").click(function () {
                if (confirm('Вы уверены? Данная операция уничтожит все пользовательские этапы.')) {
                    var productionOrderBatchId = $("#Id").val();
                    StartLinkProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/ClearCustomStages",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            RefreshOrderBatchAfterModifiedStages(result, function () {
                                ShowSuccessMessage("Этапы удалены.", "messageProductionOrderBatchEditStages");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEditStages");
                        }
                    });
                }
            });



        });
    },

    // --------- Форма редактирования стадии ---------

    OnFailStageSave: function (XMLHttpRequest, textStatus, thrownError) {
        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchStageEdit");
    },

    OnBeginStageSave: function () {
        StartButtonProgress($("#btnStageSave"));
    },

    OnSuccessStageSave: function (result) {
        RefreshOrderBatchAfterModifiedStages(result, function () {
            HideModal(function () {
                ShowSuccessMessage("Сохранено.", "messageProductionOrderBatchEditStages");
            });
        });
    }

    // ---------------------------------------------------

};

function RefreshExecutionGraph(onSuccessFunction) {
    var productionOrderBatchId = $("#Id").val();

    $.ajax({
        type: "GET",
        url: "/ProductionOrder/ShowOrderExecutionGraph/",
        data: { id: productionOrderBatchId },
        success: function (result) {
            drawExecutionGraph("graph", result);
            if (onSuccessFunction != undefined)
            // Вызываем переданный метод
                onSuccessFunction();
        },
        error: function (XMLHttpRequest, textStatus, thrownError) {
            ShowErrorMessage(XMLHttpRequest.responseText, "messageExecutionGraph");
        }
    });
};

// Пользователь щелкнул на ссылку "Выбрать" в гриде выбора шаблона заказа
function OnProductionOrderBatchLifeCycleTemplateSelectLinkClick (productionOrderBatchLifeCycleTemplateId, productionOrderBatchLifeCycleTemplateName) {
    var productionOrderBatchId = $("#Id").val();
    $.ajax({
        type: "POST",
        url: "/ProductionOrder/LoadStagesFromTemplate/",
        data: { productionOrderBatchId: productionOrderBatchId, productionOrderBatchLifeCycleTemplateId: productionOrderBatchLifeCycleTemplateId },
        success: function (result) {
            RefreshOrderBatchAfterModifiedStages(result, function () {
                HideModal(function () {
                    ShowSuccessMessage("Этапы загружены из шаблона.", "messageProductionOrderBatchEditStages");
                });
            });
        },
        error: function (XMLHttpRequest, textStatus, thrownError) {
            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateSelectList");
        }
    });
};

function RefreshOrderBatchAfterModifiedStages(result, onSuccessFunction) {
    $(".main_details_table #ProducingPendingDate").text(result.producingPendingDate);
    $(".main_details_table #DeliveryPendingDate").text(result.deliveryPendingDate);
    RefreshGrid("gridProductionOrderBatchStage", function () {
        RefreshExecutionGraph(function () {
            if (onSuccessFunction != undefined)
                onSuccessFunction();
        });
    });
};

