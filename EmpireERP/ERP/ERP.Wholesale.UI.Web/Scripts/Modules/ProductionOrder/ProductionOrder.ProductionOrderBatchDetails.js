var ProductionOrder_ProductionOrderBatchDetails = {
    Init: function () {
        $(document).ready(function () {
            var graphData = $('#graphData').text();
            ProductionOrder_ProductionOrderBatchDetails.DrawExecutionGraph(graphData);

            // Возврат на прежнюю страницу
            $("#btnBack").click(function () {
                window.location = $("#BackUrl").val();
            });

            // Формируем ссылки
            ProductionOrder_ProductionOrderBatchDetails.UpdateCuratorLink();

            var productionOrderId = $("#ProductionOrderId").val();
            $("#ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + productionOrderId + GetBackUrl());

            var producerId = $("#ProducerId").val();
            $("#ProducerName").attr("href", "/Producer/Details?id=" + producerId + GetBackUrl());

            var receiptWaybillId = $("#ReceiptWaybillId").val();
            if (!IsDefaultOrEmpty(receiptWaybillId)) {
                $("#ReceiptWaybillName").attr("href", "/ReceiptWaybill/Details?id=" + receiptWaybillId + GetBackUrl());
            }

            $("#linkCreateReceiptWaybill").click(function () {
                var productionOrderBatchId = $('#Id').val();

                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/CheckPossibilityToCreateReceiptWaybill",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        window.location = "/ReceiptWaybill/Create?productionOrderBatchId=" + productionOrderBatchId + GetBackUrl();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#linkDeleteReceiptWaybill").click(function () {
                if (confirm("Вы уверены?")) {
                    var waybill_id = $("#ReceiptWaybillId").val();

                    StartLinkProgress($(this));

                    $.ajax({
                        type: "POST",
                        url: "/ReceiptWaybill/Delete/",
                        data: { id: waybill_id, returnProductionOrderBatchDetails: "1" },
                        success: function (result) {
                            StopLinkProgress();
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            ShowSuccessMessage("Накладная удалена.", "messageProductionOrderBatchEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnProductionOrderClose").click(function () {
                StartButtonProgress($(this));
                var productionOrderId = $("#ProductionOrderId").val();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/Close",
                    data: { productionOrderId: productionOrderId },
                    success: function (result) {
                        window.location = "/ProductionOrder/Details?id=" + productionOrderId;
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageModalCloseOrder");
                    }
                });
            });

            $("#btnSplitBatch").click(function () {
                var productionOrderBatchId = $('#Id').val();
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/CheckPossibilityToSplitBatch",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        var productionOrderBatchId = $('#Id').val();
                        window.location = "/ProductionOrder/SplitBatch?productionOrderBatchId=" + productionOrderBatchId + GetBackUrl();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnAccept").click(function () {
                var productionOrderBatchId = $('#Id').val();
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/Accept",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                        RefreshGrid("gridProductionOrderBatchRow", function () {
                            ShowSuccessMessage("Проведено.", "messageProductionOrderBatchEdit");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnCancelAcceptance").click(function () {
                if (confirm("Вы уверены?")) {
                    var productionOrderBatchId = $('#Id').val();
                    StartButtonProgress($(this));

                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/CancelAcceptance",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            RefreshGrid("gridProductionOrderBatchRow", function () {
                                ShowSuccessMessage("Проводка отменена.", "messageProductionOrderBatchEdit");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnApprove").click(function () {
                var productionOrderBatchId = $('#Id').val();
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/Approve",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                        ShowSuccessMessage("Готово.", "messageProductionOrderBatchEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnCancelApprovement").click(function () {
                if (confirm("Вы уверены?")) {
                    var productionOrderBatchId = $('#Id').val();
                    StartButtonProgress($(this));

                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/CancelApprovement",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            ShowSuccessMessage("Готовность отменена.", "messageProductionOrderBatchEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnApproveByLineManager").click(function () {
                var productionOrderBatchId = $('#Id').val();
                StartButtonProgress($(this));

                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/ApproveByLineManager",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                        ShowSuccessMessage("Утверждено.", "messageProductionOrderBatchEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnCancelApprovementByLineManager").click(function () {
                if (confirm("Вы уверены?")) {
                    var productionOrderBatchId = $('#Id').val();
                    StartButtonProgress($(this));

                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/CancelApprovementByLineManager",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            ShowSuccessMessage("Утверждение отменено.", "messageProductionOrderBatchEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnApproveByFinancialDepartment").click(function () {
                var productionOrderBatchId = $('#Id').val();
                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/ApproveByFinancialDepartment",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                        ShowSuccessMessage("Утверждено.", "messageProductionOrderBatchEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnCancelApprovementByFinancialDepartment").click(function () {
                if (confirm("Вы уверены?")) {
                    StartButtonProgress($(this));
                    var productionOrderBatchId = $('#Id').val();
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/CancelApprovementByFinancialDepartment",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            ShowSuccessMessage("Утверждение отменено.", "messageProductionOrderBatchEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnApproveBySalesDepartment").click(function () {
                var productionOrderBatchId = $('#Id').val();
                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/ApproveBySalesDepartment",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                        ShowSuccessMessage("Утверждено.", "messageProductionOrderBatchEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnCancelApprovementBySalesDepartment").click(function () {
                if (confirm("Вы уверены?")) {
                    StartButtonProgress($(this));
                    var productionOrderBatchId = $('#Id').val();
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/CancelApprovementBySalesDepartment",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            ShowSuccessMessage("Утверждение отменено.", "messageProductionOrderBatchEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnApproveByAnalyticalDepartment").click(function () {
                var productionOrderBatchId = $('#Id').val();
                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/ApproveByAnalyticalDepartment",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                        ShowSuccessMessage("Утверждено.", "messageProductionOrderBatchEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnCancelApprovementByAnalyticalDepartment").click(function () {
                if (confirm("Вы уверены?")) {
                    StartButtonProgress($(this));
                    var productionOrderBatchId = $('#Id').val();
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/CancelApprovementByAnalyticalDepartment",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            ShowSuccessMessage("Утверждение отменено.", "messageProductionOrderBatchEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnApproveByProjectManager").click(function () {
                StartButtonProgress($(this));
                var productionOrderBatchId = $('#Id').val();
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/ApproveByProjectManager",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                        ShowSuccessMessage("Утверждено.", "messageProductionOrderBatchEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnCancelApprovementByProjectManager").click(function () {
                if (confirm("Вы уверены?")) {
                    StartButtonProgress($(this));
                    var productionOrderBatchId = $('#Id').val();
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/CancelApprovementByProjectManager",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            ShowSuccessMessage("Утверждение отменено.", "messageProductionOrderBatchEdit");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnEditStages").click(function () {
                var productionOrderBatchId = $("#Id").val();
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditStages",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        $("#productionOrderBatchEditStages").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchEditStages"));
                        ShowModal("productionOrderBatchEditStages");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            $("#btnDeleteBatch").click(function () {
                var productionOrderBatchId = $('#Id').val();
                if (confirm("Вы уверены?")) {
                    $.ajax({
                        type: "GET",
                        url: "/ProductionOrder/DeleteProductionOrderBatch",
                        data: { productionOrderBatchId: productionOrderBatchId },
                        success: function () {
                            //Переходим на детали заказа
                            window.location = $(".main_details_table #ProductionOrderName").attr("href");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                        }
                    });
                }
            });

            $("#btnRenameBatch").click(function () {
                var productionOrderBatchId = $('#Id').val();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/RenameProductionOrderBatch",
                    data: { productionOrderBatchId: productionOrderBatchId },
                    success: function (result) {
                        $("#productionOrderRenameBatch").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderRenameBatch"));
                        ShowModal("productionOrderRenameBatch");
                    },

                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                    }
                });
            });

            // добавление строки
            $("#btnAddRow").live("click", function () {
                var batchId = $('#Id').val();
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/AddRow",
                    data: { batchId: batchId },
                    success: function (result) {
                        $('#productionOrderBatchRowEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchRowEdit"));
                        ShowModal("productionOrderBatchRowEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowList");
                    }
                });
            });

            $("#gridProductionOrderBatchRow .linkRowEdit").live("click", function () {
                var batchId = $('#Id').val();
                var batchRowId = $(this).parent("td").parent("tr").find(".hidden_column").text();

                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditRow",
                    data: { batchId: batchId, rowId: batchRowId },
                    success: function (result) {
                        $('#productionOrderBatchRowEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchRowEdit"));
                        ShowModal("productionOrderBatchRowEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowList");
                    }
                });
            });

            $("#gridProductionOrderBatchRow .linkRowDelete").live("click", function () {
                if (confirm('Вы уверены?')) {
                    var batchId = $('#Id').val();
                    var batchRowId = $(this).parent("td").parent("tr").find(".hidden_column").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/DeleteRow",
                        data: { batchId: batchId, rowId: batchRowId },
                        success: function (result) {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            RefreshGrid("gridProductionOrderBatchRow", function () { ShowSuccessMessage("Позиция удалена.", "messageProductionOrderBatchRowList"); });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowList");
                        }
                    });
                }
            });
        });

        $("#linkChangeStage").live("click", function () {
            var productionOrderBatchId = $('#Id').val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ChangeStage",
                data: { productionOrderBatchId: productionOrderBatchId, isSingleBatch: "0" },
                success: function (result) {
                    $('#productionOrderBatchChangeStage').hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderBatchChangeStage"));
                    ShowModal("productionOrderBatchChangeStage");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchEdit");
                }
            });
        });

        $("#productionOrderBatchChangeStage #btnMoveToNextStage").live("click", function () {
            var productionOrderBatchId = $('#Id').val();
            StartButtonProgress($(this));
            var currentStageId = $("#productionOrderBatchChangeStage #CurrentStageId").val();
            $.ajax({
                type: "POST",
                url: "/ProductionOrder/MoveToNextStage",
                data: { productionOrderBatchId: productionOrderBatchId, currentStageId: currentStageId, isSingleBatch: "0", isReturnBatchDetails: "1" },
                success: function (result) {
                    HideModal(function () {
                        ProductionOrder_ProductionOrderBatchDetails.RefreshExecutionGraph(function () {
                            ShowSuccessMessage("Текущий этап изменен.", "messageProductionOrderBatchEdit");
                            // RefreshMainDetails может вывести сообщение, забивающее первое, и должно идти вторым
                            ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result.mainDetails);
                            if (result.allowToCloseProductionOrder)
                                ProductionOrder_ProductionOrderBatchDetails.ShowConfirmCloseOrder();
                        });
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchStageChange");
                }
            });
        });

        $("#productionOrderBatchChangeStage #btnMoveToPreviousStage").live("click", function () {
            if (confirm('Вы уверены?')) {
                StartButtonProgress($(this));
                var productionOrderBatchId = $('#Id').val();
                var currentStageId = $("#productionOrderBatchChangeStage #CurrentStageId").val();
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/MoveToPreviousStage",
                    data: { productionOrderBatchId: productionOrderBatchId, currentStageId: currentStageId, isSingleBatch: "0", isReturnBatchDetails: "1" },
                    success: function (result) {
                        HideModal(function () {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshExecutionGraph(function () {
                                ShowSuccessMessage("Текущий этап изменен.", "messageProductionOrderBatchEdit");
                                ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result);
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchStageChange");
                    }
                });
            }
        });

        $("#productionOrderBatchChangeStage #btnMoveToUnsuccessfulClosingStage").live("click", function () {
            if (confirm('Вы уверены?')) {
                StartButtonProgress($(this));
                var productionOrderBatchId = $('#Id').val();
                var currentStageId = $("#productionOrderBatchChangeStage #CurrentStageId").val();
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/MoveToUnsuccessfulClosingStage",
                    data: { productionOrderBatchId: productionOrderBatchId, currentStageId: currentStageId, isSingleBatch: "0", isReturnBatchDetails: "1" },
                    success: function (result) {
                        HideModal(function () {
                            ProductionOrder_ProductionOrderBatchDetails.RefreshExecutionGraph(function () {
                                ShowSuccessMessage("Текущий этап изменен.", "messageProductionOrderBatchEdit");
                                // RefreshMainDetails может вывести сообщение, забивающее первое, и должно идти вторым
                                ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(result.mainDetails);
                                if (result.allowToCloseProductionOrder)
                                    ProductionOrder_ProductionOrderBatchDetails.ShowConfirmCloseOrder();
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchStageChange");
                    }
                });
            }
        });

    },

    ShowConfirmCloseOrder: function () {
        ShowConfirm("Закрыть заказ?", "При закрытии заказа будет рассчитана себестоимость товара.", "Закрыть заказ",
                "Отмена",
                function () {
                    var productionOrderId = $("#ProductionOrderId").val();
                    $.ajax({
                        type: "GET",
                        url: "/ProductionOrder/Close",
                        data: { productionOrderId: productionOrderId },
                        success: function (result) {
                            window.location = "/ProductionOrder/Details?id=" + productionOrderId;
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageConfirmError");
                        }
                    });
                });
    },


    UpdateCuratorLink: function () {
        var curatorId = $("#CuratorId").val();
        if (IsTrue($("#AllowToViewCuratorDetails").val())) {
            $("#CuratorName").attr("href", "/User/Details?id=" + curatorId + GetBackUrl());
        }
        else {
            $("#CuratorName").addClass("disabled");
        }
    },

    UpdateReceiptWaybillLink: function () {
        var receiptWaybillId = $("#ReceiptWaybillId").val();
        if (IsTrue($("#AllowToViewReceiptWaybillDetails").val())) {
            $("#ReceiptWaybillName").attr("href", "/ReceiptWaybill/Details?id=" + receiptWaybillId + GetBackUrl());
        }
        else {
            $("#ReceiptWaybillName").addClass("disabled");
        }
    },

    OnSuccessProductionOrderBatchRowEdit: function (ajaxContext) {
        if ($('#productionOrderBatchRowEdit #Id').val() != "00000000-0000-0000-0000-000000000000") {
            HideModal();
            ShowSuccessMessage("Сохранено.", "messageProductionOrderBatchRowList");
        }
        else {
            $("#productionOrderBatchRowEdit #ArticleName").text("Выберите товар");
            $("#productionOrderBatchRowEdit #ArticleId").val("");
            $("#productionOrderBatchRowEdit #ManufacturerName").text("Выберите фабрику-изготовителя");
            $("#productionOrderBatchRowEdit #ManufacturerId").val("");
            $("#productionOrderBatchRowEdit #ProductionCountryId").val(0);
            $('#productionOrderBatchRowEdit #PackHeight').val("0");
            $('#productionOrderBatchRowEdit #PackLength').val("0");
            $('#productionOrderBatchRowEdit #PackWidth').val("0");

            $('#productionOrderBatchRowEdit #PackSize').text("---");
            $('#productionOrderBatchRowEdit #PackWeight').val("");
            $('#productionOrderBatchRowEdit #PackVolume').val("");
            $('#productionOrderBatchRowEdit #Count').val("");
            $('#productionOrderBatchRowEdit #PackCount').val("");
            $('#productionOrderBatchRowEdit #ProductionCost').val("");
            $('#productionOrderBatchRowEdit #TotalCost').val("");

            $('#productionOrderBatchRowEdit #TotalWeight').text("---");
            $('#productionOrderBatchRowEdit #TotalVolume').text("---");

            $('#productionOrderBatchRowEdit #MeasureUnitName').text("");

            SetFieldScale("#Count", 12, 0, "#productionOrderBatchRowEdit", true);

            $('#productionOrderBatchRowEdit input[type!="button"][type!="submit"][type!="hidden"]').attr("disabled", "disabled");

            ShowSuccessMessage("Сохранено.", "messageProductionOrderBatchRowEdit");
        }

        ProductionOrder_ProductionOrderBatchDetails.RefreshMainDetails(ajaxContext);
        RefreshGrid("gridProductionOrderBatchRow");
    },

    RefreshMainDetails: function (details) {
        $("#ProductionOrderName").text(details.ProductionOrderName);
        $("#StateName").text(details.StateName);
        $("#ProducerName").text(details.ProducerName);
        $("#CurrentStageName").text(details.CurrentStageName);
        $("#CurrentStageActualStartDate").text(details.CurrentStageActualStartDate);
        $("#CurrentStageDaysPassed").text(details.CurrentStageDaysPassed);
        $("#CurrentStageExpectedEndDate").text(details.CurrentStageExpectedEndDate);
        $("#CurrentStageDaysLeft").text(details.CurrentStageDaysLeft);
        $("#ContainerPlacement").text(details.ContainerPlacement);
        $("#ContainerPlacementFreeVolume").text(details.ContainerPlacementFreeVolume);
        $("#CurrencyLiteralCode").text(details.CurrencyLiteralCode);
        $("#CurrencyRateName").text(details.CurrencyRateName);
        $("#CurrencyRate").text(details.CurrencyRate);
        $("#Date").text(details.Date);
        $("#ProducingPendingDate").text(details.ProducingPendingDate);
        $("#DeliveryPendingDate").text(details.DeliveryPendingDate);
        $("#DivergenceFromPlan").text(details.DivergenceFromPlan);
        $("#Weight").text(details.Weight);
        $("#Volume").text(details.Volume);
        $("#ProductionCostSumInCurrency").text(details.ProductionCostSumInCurrency);
        $("#ProductionCostSumInBaseCurrency").text(details.ProductionCostSumInBaseCurrency);
        $("#AccountingPriceSum").text(details.AccountingPriceSum);

        $("#AllowToViewStageList").val(details.AllowToViewStageList);
        $("#AllowToViewCuratorDetails").val(details.AllowToViewCuratorDetails);
        $("#AllowToViewReceiptWaybillDetails").val(details.AllowToViewReceiptWaybillDetails);

        $("#CuratorId").val(details.CuratorId);
        if (!IsDefaultOrEmpty(details.CuratorId)) {
            $("#CuratorName").html('<a id="CuratorLink">' + details.CuratorName + '</a>');
            ProductionOrder_ProductionOrderBatchDetails.UpdateCuratorLink();
        } else {
            $("#CuratorName").html(details.CuratorName);
        }

        $("#ReceiptWaybillId").val(details.ReceiptWaybillId);
        if (!IsDefaultOrEmpty(details.ReceiptWaybillId)) {
            $("#receiptWaybillLink").html('<a id="ReceiptWaybillName">' + details.ReceiptWaybillName + '</a>');
            ProductionOrder_ProductionOrderBatchDetails.UpdateReceiptWaybillLink();
        } else {
            $("#receiptWaybillLink").html(details.ReceiptWaybillName);
        }

        ProductionOrder_ProductionOrderBatchDetails.UpdateOptionColor("IsApprovedByLineManager", details.IsApprovedByLineManager);
        ProductionOrder_ProductionOrderBatchDetails.UpdateOptionColor("IsApprovedByFinancialDepartment", details.IsApprovedByFinancialDepartment);
        ProductionOrder_ProductionOrderBatchDetails.UpdateOptionColor("IsApprovedBySalesDepartment", details.IsApprovedBySalesDepartment);
        ProductionOrder_ProductionOrderBatchDetails.UpdateOptionColor("IsApprovedByAnalyticalDepartment", details.IsApprovedByAnalyticalDepartment);
        ProductionOrder_ProductionOrderBatchDetails.UpdateOptionColor("IsApprovedByProjectManager", details.IsApprovedByProjectManager);
        UpdateElementVisibility("ApprovementState", details.IsApprovementState);

        UpdateButtonAvailability("btnSplitBatch", details.AllowToSplitBatch);
        UpdateElementVisibility("btnSplitBatch", details.AllowToSplitBatch);

        UpdateButtonAvailability("btnAddRow", details.AllowToEditRows);
        UpdateElementVisibility("btnAddRow", details.AllowToEditRows);

        UpdateButtonAvailability("btnRenameBatch", details.AllowToRename);
        UpdateElementVisibility("btnRenameBatch", details.AllowToRename);

        UpdateButtonAvailability("btnDeleteBatch", details.AllowToDeleteBatch);
        UpdateElementVisibility("btnDeleteBatch", details.AllowToDeleteBatch);

        UpdateButtonAvailability("btnEditStages", details.AllowToEditStages);
        UpdateElementVisibility("btnEditStages", details.AllowToEditStages);

        UpdateButtonAvailability("btnAccept", details.AllowToAccept);
        UpdateElementVisibility("btnAccept", details.AllowToAccept);
        UpdateButtonAvailability("btnCancelAcceptance", details.AllowToCancelAcceptance);
        UpdateElementVisibility("btnCancelAcceptance", details.AllowToCancelAcceptance);
        UpdateButtonAvailability("btnApprove", details.AllowToApprove);
        UpdateElementVisibility("btnApprove", details.AllowToApprove);
        UpdateButtonAvailability("btnCancelApprovement", details.AllowToCancelApprovement);
        UpdateElementVisibility("btnCancelApprovement", details.AllowToCancelApprovement);

        UpdateButtonAvailability("btnApproveByLineManager", details.AllowToApproveByLineManager);
        UpdateElementVisibility("btnApproveByLineManager", details.AllowToApproveByLineManager);
        UpdateButtonAvailability("btnCancelApprovementByLineManager", details.AllowToCancelApprovementByLineManager);
        UpdateElementVisibility("btnCancelApprovementByLineManager", details.AllowToCancelApprovementByLineManager);

        UpdateButtonAvailability("btnApproveByFinancialDepartment", details.AllowToApproveByFinancialDepartment);
        UpdateElementVisibility("btnApproveByFinancialDepartment", details.AllowToApproveByFinancialDepartment);
        UpdateButtonAvailability("btnCancelApprovementByFinancialDepartment", details.AllowToCancelApprovementByFinancialDepartment);
        UpdateElementVisibility("btnCancelApprovementByFinancialDepartment", details.AllowToCancelApprovementByFinancialDepartment);

        UpdateButtonAvailability("btnApproveBySalesDepartment", details.AllowToApproveBySalesDepartment);
        UpdateElementVisibility("btnApproveBySalesDepartment", details.AllowToApproveBySalesDepartment);
        UpdateButtonAvailability("btnCancelApprovementBySalesDepartment", details.AllowToCancelApprovementBySalesDepartment);
        UpdateElementVisibility("btnCancelApprovementBySalesDepartment", details.AllowToCancelApprovementBySalesDepartment);

        UpdateButtonAvailability("btnApproveByAnalyticalDepartment", details.AllowToApproveByAnalyticalDepartment);
        UpdateElementVisibility("btnApproveByAnalyticalDepartment", details.AllowToApproveByAnalyticalDepartment);
        UpdateButtonAvailability("btnCancelApprovementByAnalyticalDepartment", details.AllowToCancelApprovementByAnalyticalDepartment);
        UpdateElementVisibility("btnCancelApprovementByAnalyticalDepartment", details.AllowToCancelApprovementByAnalyticalDepartment);

        UpdateButtonAvailability("btnApproveByProjectManager", details.AllowToApproveByProjectManager);
        UpdateElementVisibility("btnApproveByProjectManager", details.AllowToApproveByProjectManager);
        UpdateButtonAvailability("btnCancelApprovementByProjectManager", details.AllowToCancelApprovementByProjectManager);
        UpdateElementVisibility("btnCancelApprovementByProjectManager", details.AllowToCancelApprovementByProjectManager);

        UpdateElementVisibility("linkChangeStage", details.AllowToChangeStage);
        UpdateElementVisibility("linkCreateReceiptWaybill", details.AllowToCreateReceiptWaybill);
        UpdateElementVisibility("linkDeleteReceiptWaybill", details.AllowToDeleteReceiptWaybill);

        var showExecutionGraph = IsFalse($("#IsSingleBatch").val()) && IsTrue($("#AllowToViewStageList").val());
        UpdateElementVisibility("executionGraph", showExecutionGraph);
    },

    UpdateOptionColor: function (id, value) {
        if (IsTrue(value)) {
            $("#" + id).removeClass("grey_option").addClass("green_option");
        } else {
            $("#" + id).removeClass("green_option").addClass("grey_option");
        }
    },

    RefreshExecutionGraph: function (onSuccessFunction) {
        var showExecutionGraph = IsFalse($("#IsSingleBatch").val()) && IsTrue($("#AllowToViewStageList").val());
        if (showExecutionGraph) {
            var productionOrderBatchId = $('#Id').val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ShowOrderExecutionGraph/",
                data: { id: productionOrderBatchId },
                success: function (result) {
                    ProductionOrder_ProductionOrderBatchDetails.DrawExecutionGraph(result);
                    if (onSuccessFunction != undefined)
                    // Вызываем переданный метод
                        onSuccessFunction();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExecutionGraph");
                }
            });
        }
        else {
            if (onSuccessFunction != undefined)
            // Вызываем переданный метод
                onSuccessFunction();
        }
    },

    DrawExecutionGraph: function (graphData) {
        var showExecutionGraph = IsTrue($("#AllowToViewStageList").val());
        if (showExecutionGraph) {
            drawExecutionGraph("graph", graphData, false);

            //Получаем дату начала и конца заказа
            if (typeof graphData == "string") {
                var data = $.parseJSON(graphData);
            }
            else {
                var data = graphData;
            }
            var startDate = new Date(parseInt(data.StartDate.slice(6, 19)));
            var endDate = new Date(parseInt(data.EndDate.slice(6, 19)));

            var startDateString = dateToString(startDate, 2) + ' - Старт';
            var endDateString = 'Завершение - ' + dateToString(endDate, 2);

            //если отрисовываем граф не в первый раз, то просто меняем даты
            if ($("#executionGraph .grid").children().is("#startDate")) {
                $("#startDate").text(startDateString);
                $("#endDate").text(endDateString);
            }
            else{//создаем div для дат и выводим их
                var startDateDiv = $('<div id="startDate" style="margin:5px;position:absolute;left:0;bottom:3px">' + startDateString + '</div>');
                var endDateDiv = $('<div id="endDate" style="margin:5px;position:absolute;right:0;bottom:3px">' + endDateString + '</div>');
                $("#executionGraph .grid").append(startDateDiv).append(endDateDiv);
            }

        }
    }
};
