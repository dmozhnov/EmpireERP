var ProductionOrderBatchLifeCycleTemplate_Details = {
    Init: function () {
        $(document).ready(function () {
            $("#btnEdit").click(function () {
                var id = $("#ProductionOrderBatchLifeCycleTemplateId").val();
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrderBatchLifeCycleTemplate/Edit",
                    data: { id: id },
                    success: function (result) {
                        $('#productionOrderBatchLifeCycleTemplateEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchLifeCycleTemplateEdit"));
                        ShowModal("productionOrderBatchLifeCycleTemplateEdit");
                        $("#productionOrderBatchLifeCycleTemplateEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateEdit");
                    }
                });
            });

            $("#btnDelete").click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $("#ProductionOrderBatchLifeCycleTemplateId").val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrderBatchLifeCycleTemplate/Delete",
                        data: { id: id },
                        success: function (result) {
                            window.location = $("#BackUrl").val();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateEdit");
                        }
                    });
                }
            });

            // Возврат на прежнюю страницу
            $("#btnBack").click(function () {
                window.location = $("#BackUrl").val();
            });
        });

        $(".linkAddStage").live("click", function () {
            var productionOrderBatchLifeCycleTemplateId = $("#ProductionOrderBatchLifeCycleTemplateId").val();
            var id = $(this).parent("td").parent("tr").find(".Id").text();
            var position = $(this).parent("td").parent("tr").find(".OrdinalNumber").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrderBatchLifeCycleTemplate/AddStage",
                data: { productionOrderBatchLifeCycleTemplateId: productionOrderBatchLifeCycleTemplateId, id: id, position: position },
                success: function (result) {
                    $("#productionOrderBatchLifeCycleTemplateStageEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderBatchLifeCycleTemplateStageEdit"));
                    ShowModal("productionOrderBatchLifeCycleTemplateStageEdit");
                    $("#productionOrderBatchLifeCycleTemplateStageEdit #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateEdit");
                }
            });
        });

        $(".linkEditStage").live("click", function () {
            var productionOrderBatchLifeCycleTemplateId = $("#ProductionOrderBatchLifeCycleTemplateId").val();
            var id = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrderBatchLifeCycleTemplate/EditStage",
                data: { productionOrderBatchLifeCycleTemplateId: productionOrderBatchLifeCycleTemplateId, id: id },
                success: function (result) {
                    $("#productionOrderBatchLifeCycleTemplateStageEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderBatchLifeCycleTemplateStageEdit"));
                    ShowModal("productionOrderBatchLifeCycleTemplateStageEdit");
                    $("#productionOrderBatchLifeCycleTemplateStageEdit #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateEdit");
                }
            });
        });

        $(".linkDeleteStage").live("click", function () {
            if (confirm('Вы уверены?')) {
                var productionOrderBatchLifeCycleTemplateId = $("#ProductionOrderBatchLifeCycleTemplateId").val();
                var id = $(this).parent("td").parent("tr").find(".Id").text();

                StartGridProgress($(this).closest(".grid"));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrderBatchLifeCycleTemplate/DeleteStage",
                    data: { productionOrderBatchLifeCycleTemplateId: productionOrderBatchLifeCycleTemplateId, id: id },
                    success: function (result) {
                        RefreshGrid("gridProductionOrderBatchLifeCycleTemplateStage", function () {
                            ShowSuccessMessage("Этап удален.", "messageProductionOrderBatchLifeCycleTemplateEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateEdit");
                    }
                });
            }
        });

        $(".linkMoveStageUp").live("click", function () {
            var productionOrderBatchLifeCycleTemplateId = $("#ProductionOrderBatchLifeCycleTemplateId").val();
            var id = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "POST",
                url: "/ProductionOrderBatchLifeCycleTemplate/MoveStageUp",
                data: { productionOrderBatchLifeCycleTemplateId: productionOrderBatchLifeCycleTemplateId, id: id },
                success: function (result) {
                    RefreshGrid("gridProductionOrderBatchLifeCycleTemplateStage");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateEdit");
                }
            });
        });

        $(".linkMoveStageDown").live("click", function () {
            var productionOrderBatchLifeCycleTemplateId = $("#ProductionOrderBatchLifeCycleTemplateId").val();
            var id = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "POST",
                url: "/ProductionOrderBatchLifeCycleTemplate/MoveStageDown",
                data: { productionOrderBatchLifeCycleTemplateId: productionOrderBatchLifeCycleTemplateId, id: id },
                success: function (result) {
                    RefreshGrid("gridProductionOrderBatchLifeCycleTemplateStage");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateEdit");
                }
            });
        });

    },

    OnSuccessProductionOrderBatchLifeCycleTemplateEdit: function (ajaxContext) {
        HideModal(function () {
            ProductionOrderBatchLifeCycleTemplate_Details.RefreshMainDetailsAndPermissions(ajaxContext);
            ShowSuccessMessage("Сохранено.", "messageProductionOrderBatchLifeCycleTemplateEdit");            
        });
    },

    OnFailStageSave: function (XMLHttpRequest, textStatus, thrownError) {
        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateStageEdit");
    },

    OnSuccessStageSave: function (ajaxContext) {
        HideModal(function () {
            RefreshGrid("gridProductionOrderBatchLifeCycleTemplateStage", function () {
                ShowSuccessMessage("Сохранено.", "messageProductionOrderBatchLifeCycleTemplateEdit");
            });
        });
    },

    OnBeginStageSave: function () {
        StartButtonProgress($("#btnStageSave"));
    },
    
    RefreshMainDetailsAndPermissions: function (result) {
        ProductionOrderBatchLifeCycleTemplate_Details.RefreshMainDetails(result.MainDetails);
        ProductionOrderBatchLifeCycleTemplate_Details.RefreshPermissions(result.Permissions);
    },

    RefreshMainDetails: function (details) {
        $(".page_title_item_name").text(details.Name);
    },

    RefreshPermissions: function (permissions) {
    }

};