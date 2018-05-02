var ProductionOrder_Details = {

    // Переходы между этапами - 3 обработчика: Next, Previous, ToUnsuccessful
    // Редактирование этапов - 6 обработчиков: SaveStage, MoveUp, MoveDown, DeleteStage, LoadStagesFromTemplate, ClearCustomStages

    Init: function () {
        $(document).ready(function () {
            var showExecutionGraph = IsTrue($("#AllowToViewStageList").val());
            if (showExecutionGraph) {

                var isScaleOfOrder = false;
                if ($(".graphData").length > 1)
                    isScaleOfOrder = true;

                //Отрисовываем графики партий
                $(".graphData").each(function (index, element) {
                    var graphData = $(element).text();
                    //получаем ид партии из ид элемента
                    var batchId = $(element).attr("id").substr(10);
                    drawExecutionGraph("graph-" + batchId, graphData, isScaleOfOrder);
                });

                //Получаем дату начала и конца заказа
                var data = $.parseJSON($(".graphData").html());
                var startDate = new Date(parseInt(data.ProductionOrderStartDate.slice(6, 19)));
                var endDate = new Date(parseInt(data.ProductionOrderEndDate.slice(6, 19)));

                //Выводим даты
                var dateDiv = $('<div style="height: 20px;"><div style="background: #fff;float: left; width: 50%; padding: 5px 0px 3px;">' + dateToString(startDate, 2) + ' - Старт</div>' +
                '<div style="background: #fff;float: left; width: 50%; padding: 5px 0px 3px; text-align: right">Завершение - ' + dateToString(endDate, 2) + '</div><div class="clear"></div></div>');

                $("#executionGraph .grid").append(dateDiv);
            }


            ProductionOrder_Details.RefreshColorOfSumIndicators();

            $("#btnPlannedExpensesSumDetails").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditPlannedExpenses",
                    data: { id: $("#Id").val() },
                    success: function (result) {
                        $("#ProductionOrderPlannedExpensesEdit").html(result);
                        $.validator.unobtrusive.parse($("#ProductionOrderPlannedExpensesEdit"));
                        ShowModal("ProductionOrderPlannedExpensesEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                    }
                });
            });

            // Возврат на прежнюю страницу
            $("#btnBack").click(function () {
                window.location = $("#BackUrl").val();
            });

            // Формирование ссылок
            if (IsTrue($("#AllowToViewProducerDetails").val())) {
                $("#ProducerName").attr("href", "/Producer/Details?id=" + $("#ProducerId").val() + GetBackUrl());
            }

            ProductionOrder_Details.UpdateCuratorLink();
            ProductionOrder_Details.UpdateAccountOrganizationLink();
            ProductionOrder_Details.UpdateStorageLink();

            $("#btnEditPlannedPayments").click(function () {
                var productionOrderId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditPlannedPayments",
                    data: { productionOrderId: productionOrderId },
                    success: function (result) {
                        $("#productionOrderEditPlannedPayments").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderEditPlannedPayments"));
                        ShowModal("productionOrderEditPlannedPayments");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                    }
                });
            });

            $("#btnEdit").click(function () {
                window.location = "/ProductionOrder/Edit?id=" + $("#Id").val() + GetBackUrl();
            });

            //btnClose - кнопка закрытия в деталях, 
            $("#btnClose").click(function () {
                StartButtonProgress($(this));
                var productionOrderId = $("#Id").val();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/Close",
                    data: { productionOrderId: productionOrderId },
                    success: function (result) {
                        window.location = window.location;
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                    }
                });
            });


            $("#btnOpen").click(function () {
                StartButtonProgress($(this));
                var productionOrderId = $("#Id").val();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/Open",
                    data: { productionOrderId: productionOrderId },
                    success: function (result) {
                        window.location = window.location;
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                    }
                });
            });

            $("#btnArticlePrimeCost").click(function () {
                StartButtonProgress($(this));
                var productionOrderId = $("#Id").val();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/ArticlePrimeCostSettingsForm",
                    data: { productionOrderId: productionOrderId },
                    success: function (result) {
                        $("#productionOrderArticlePrimeCostSettingsForm").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderArticlePrimeCostSettingsForm"));
                        ShowModal("productionOrderArticlePrimeCostSettingsForm");
                        $("#productionOrderArticlePrimeCostSettingsForm #ArticlePrimeCostCalculationType").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                    }
                });
            });
        });

        // Формирование ссылок
        if (IsTrue($("#AllowToViewProducerDetails").val())) {
            $("#ProducerName").attr("href", "/Producer/Details?id=" + $("#ProducerId").val() + GetBackUrl());
        }

        if (IsTrue($("#AllowToViewCuratorDetails").val())) {
            $("#CuratorName").attr("href", "/User/Details?id=" + $("#CuratorId").val() + GetBackUrl());
        }
        else {
            $("#CuratorName").addClass("disabled");
        }

        ProductionOrder_Details.UpdateCuratorLink();
        ProductionOrder_Details.UpdateAccountOrganizationLink();
        ProductionOrder_Details.UpdateStorageLink();


        $("#productionOrderArticlePrimeCostSettingsForm #btnCalculateArticlePrimeCost").live("click", function () {
            var isValid = true;
            var productionOrderId = $("#Id").val();
            var articlePrimeCostCalculationTypeId = $("#productionOrderArticlePrimeCostSettingsForm #ArticlePrimeCostCalculationTypeId").val();
            if (isNaN(TryGetDecimal(articlePrimeCostCalculationTypeId))) {
                $("#productionOrderArticlePrimeCostSettingsForm #ArticlePrimeCostCalculationTypeId")
                        .ValidationError("Укажите, по каким значениям считать себестоимость"); isValid = false;
            }
            var divideCustomsExpenses = $("#productionOrderArticlePrimeCostSettingsForm #DivideCustomsExpenses").val();
            var showArticleVolumeAndWeight = $("#productionOrderArticlePrimeCostSettingsForm #ShowArticleVolumeAndWeight").val();
            var articleTransportingPrimeCostCalculationTypeId = $("#productionOrderArticlePrimeCostSettingsForm #ArticleTransportingPrimeCostCalculationTypeId").val();
            if (isNaN(TryGetDecimal(articleTransportingPrimeCostCalculationTypeId))) {
                $("#productionOrderArticlePrimeCostSettingsForm #ArticleTransportingPrimeCostCalculationTypeId")
                        .ValidationError("Укажите, как считать себестоимость транспортировки"); isValid = false;
            }
            var includeUnsuccessfullyClosedBatches = $("#productionOrderArticlePrimeCostSettingsForm #IncludeUnsuccessfullyClosedBatches").val();
            var includeUnapprovedBatches = $("#productionOrderArticlePrimeCostSettingsForm #IncludeUnapprovedBatches").val();
            if (isValid) {
                var url = "/ProductionOrder/ArticlePrimeCostForm?productionOrderId=" + productionOrderId +
                "&articlePrimeCostCalculationTypeId=" + articlePrimeCostCalculationTypeId +
                "&divideCustomsExpenses=" + divideCustomsExpenses +
                "&showArticleVolumeAndWeight=" + showArticleVolumeAndWeight +
                "&articleTransportingPrimeCostCalculationTypeId=" + articleTransportingPrimeCostCalculationTypeId +
                "&includeUnsuccessfullyClosedBatches=" + includeUnsuccessfullyClosedBatches +
                "&includeUnapprovedBatches=" + includeUnapprovedBatches;
                window.open(url);
                HideModal();
            }
        });

        $("#linkChangeStage").live("click", function () {
            var productionOrderBatchId = $("#SingleProductionOrderBatchId").val();

            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ChangeStage",
                data: { productionOrderBatchId: productionOrderBatchId, isSingleBatch: "1" },
                success: function (result) {
                    $('#productionOrderBatchChangeStage').hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderBatchChangeStage"));
                    ShowModal("productionOrderBatchChangeStage");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                }
            });
        });

        $("#productionOrderBatchChangeStage #btnMoveToUnsuccessfulClosingStage").live("click", function () {
            if (confirm('Вы уверены?')) {
                StartButtonProgress($(this));
                var productionOrderBatchId = $("#SingleProductionOrderBatchId").val();
                var currentStageId = $("#productionOrderBatchChangeStage #CurrentStageId").val();
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/MoveToUnsuccessfulClosingStage",
                    data: { productionOrderBatchId: productionOrderBatchId, currentStageId: currentStageId, isSingleBatch: "1" },
                    success: function (result) {
                        HideModal(function () {
                            ProductionOrder_Details.RefreshExecutionGraph(function () {
                                ShowSuccessMessage("Текущий этап изменен.", "messageProductionOrderEdit");
                                ProductionOrder_Details.RefreshMainDetailsAndPermissions(result.mainDetails);

                                if (result.allowToCloseProductionOrder)
                                    ProductionOrder_Details.ShowConfirmCloseOrder();
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchStageChange");
                    }
                });
            }
        });

        $("#productionOrderBatchChangeStage #btnMoveToNextStage").live("click", function () {
            StartButtonProgress($(this));
            var productionOrderBatchId = $("#SingleProductionOrderBatchId").val();
            var currentStageId = $("#productionOrderBatchChangeStage #CurrentStageId").val();
            $.ajax({
                type: "POST",
                url: "/ProductionOrder/MoveToNextStage",
                data: { productionOrderBatchId: productionOrderBatchId, currentStageId: currentStageId, isSingleBatch: "1" },
                success: function (result) {
                    HideModal(function () {
                        ProductionOrder_Details.RefreshExecutionGraph(function () {
                            ShowSuccessMessage("Текущий этап изменен.", "messageProductionOrderEdit");
                            ProductionOrder_Details.RefreshMainDetailsAndPermissions(result.mainDetails);

                            if (result.allowToCloseProductionOrder)
                                ProductionOrder_Details.ShowConfirmCloseOrder();
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
                var productionOrderBatchId = $("#SingleProductionOrderBatchId").val();
                var currentStageId = $("#productionOrderBatchChangeStage #CurrentStageId").val();
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/MoveToPreviousStage",
                    data: { productionOrderBatchId: productionOrderBatchId, currentStageId: currentStageId, isSingleBatch: "1" },
                    success: function (result) {
                        HideModal(function () {
                            ProductionOrder_Details.RefreshExecutionGraph(function () {
                                ShowSuccessMessage("Текущий этап изменен.", "messageProductionOrderEdit");
                                ProductionOrder_Details.RefreshMainDetailsAndPermissions(result);
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchStageChange");
                    }
                });
            }
        });



        $("#linkCreateContract").live("click", function () {
            var productionOrderId = $("#Id").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/CreateContract",
                data: { productionOrderId: productionOrderId },
                success: function (result) {
                    $("#producerContractEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#producerContractEdit"));
                    ShowModal("producerContractEdit");
                    $("#producerContractEdit #Number").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                }
            });
        });

        $("#linkEditContract").live("click", function () {
            var productionOrderId = $("#Id").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/EditContract",
                data: { productionOrderId: productionOrderId },
                success: function (result) {
                    $("#producerContractEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#producerContractEdit"));
                    ShowModal("producerContractEdit");
                    $("#producerContractEdit #Number").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                }
            });
        });

        $("#main_page #linkChangeCurrencyRate").live("click", function () {
            var currencyId = $("#main_page #CurrencyId").val();
            $.ajax({
                type: "GET",
                url: "/Currency/SelectCurrencyRate",
                data: { currencyId: currencyId, selectFunctionName: "OnCurrencyRateSelectLinkClick" },
                success: function (result) {
                    $('#currencyRateSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#currencyRateSelector"));
                    ShowModal("currencyRateSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                }
            });
        });

        // Операции над транспортными листами

        $("#gridProductionOrderTransportSheet #btnCreateTransportSheet").live("click", function () {
            var productionOrderId = $("#Id").val();
            StartButtonProgress($(this));
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ProductionOrderCurrencyDeterminationTypeSelect",
                data: { productionOrderId: productionOrderId, productionOrderCurrencyDocumentType: "1" },
                success: function (result) {
                    $("#productionOrderCurrencyDeterminationTypeSelector").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderCurrencyDeterminationTypeSelector"));
                    ShowModal("productionOrderCurrencyDeterminationTypeSelector");
                    $("#productionOrderCurrencyDeterminationTypeSelector #ProductionOrderCurrencyDeterminationType").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTransportSheetList");
                }
            });
        });

        $("#gridProductionOrderTransportSheet .linkTransportSheetEdit").live("click", function () {
            var productionOrderId = $("#Id").val();
            var transportSheetId = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/EditProductionOrderTransportSheet",
                data: { productionOrderId: productionOrderId, transportSheetId: transportSheetId },
                success: function (result) {
                    $("#productionOrderTransportSheetEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderTransportSheetEdit"));
                    ShowModal("productionOrderTransportSheetEdit");
                    $("#productionOrderTransportSheetEdit #ForwarderName").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageTransportSheetList");
                }
            });
        });

        // Сбрасываем курс валюты при смене валюты во время редактирования транспортного листа
        $("#productionOrderTransportSheetEdit #TransportSheetCurrencyId").live("change", function () {
            $("#productionOrderTransportSheetEdit .TransportSheetCurrencyLiteralCode").text($("#productionOrderTransportSheetEdit #TransportSheetCurrencyId option:selected").text());
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateId").val("");
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateName").text("текущий");
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRate").text("---");
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateForEdit").val("");
            ProductionOrder_Details.RecalculateTransportSheetCostInBaseCurrency();
            // Шлем запрос на получение текущего курса выбранной на модальной форме валюты
            var currencyId = $("#productionOrderTransportSheetEdit #TransportSheetCurrencyId").val();
            if (currencyId != "" && currencyId != "0") {
                $.ajax({
                    type: "GET",
                    url: "/Currency/GetCurrentCurrencyRate",
                    data: { currencyId: currencyId },
                    success: function (result) {
                        $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRate").text(result.CurrencyRate);
                        $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateForEdit").val(result.CurrencyRateForEdit);
                        ProductionOrder_Details.RecalculateTransportSheetCostInBaseCurrency();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderTransportSheetEdit");
                    }
                });
            }
        });

        $("#productionOrderTransportSheetEdit #linkChangeCurrencyRate").live("click", function () {
            var currencyId = $("#productionOrderTransportSheetEdit #TransportSheetCurrencyId").val();
            if (currencyId != "" && currencyId != "0") {
                $.ajax({
                    type: "GET",
                    url: "/Currency/SelectCurrencyRate",
                    data: { currencyId: currencyId, selectFunctionName: "OnTransportSheetEditCurrencyRateSelectLinkClick" },
                    success: function (result) {
                        $('#currencyRateSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyRateSelector"));
                        ShowModal("currencyRateSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderTransportSheetEdit");
                    }
                });
            }
            else {
                StopLinkProgress();
                $("#productionOrderTransportSheetEdit #TransportSheetCurrencyId").ValidationError("Укажите валюту");
            }
        });

        $("#productionOrderTransportSheetEdit #CostInCurrency").live("keyup change paste cut", function () {
            ProductionOrder_Details.RecalculateTransportSheetCostInBaseCurrency();
        });

        $("#gridProductionOrderTransportSheet .linkTransportSheetDelete").live("click", function () {
            if (confirm('Вы уверены?')) {
                var productionOrderId = $("#Id").val();
                var transportSheetId = $(this).parent("td").parent("tr").find(".Id").text();

                StartGridProgress($(this).closest(".grid"));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/DeleteProductionOrderTransportSheet/",
                    data: { productionOrderId: productionOrderId, transportSheetId: transportSheetId },
                    success: function (result) {
                        RefreshGrid("gridProductionOrderTransportSheet", function () {
                            ProductionOrder_Details.RefreshMainDetailsAndPermissions(result);
                            ShowSuccessMessage("Транспортный лист удален.", "messageTransportSheetList");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTransportSheetList");
                    }
                });
            }
        });

        // Операции над листами дополнительных расходов

        $("#gridProductionOrderExtraExpensesSheet #btnCreateExtraExpensesSheet").live("click", function () {
            var productionOrderId = $("#Id").val();
            StartButtonProgress($(this));
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ProductionOrderCurrencyDeterminationTypeSelect",
                data: { productionOrderId: productionOrderId, productionOrderCurrencyDocumentType: "2" },
                success: function (result) {
                    $("#productionOrderCurrencyDeterminationTypeSelector").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderCurrencyDeterminationTypeSelector"));
                    ShowModal("productionOrderCurrencyDeterminationTypeSelector");
                    $("#productionOrderCurrencyDeterminationTypeSelector #ProductionOrderCurrencyDeterminationType").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExtraExpensesSheetList");
                }
            });
        });

        $("#gridProductionOrderExtraExpensesSheet .linkExtraExpensesSheetEdit").live("click", function () {
            var productionOrderId = $("#Id").val();
            var extraExpensesSheetId = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/EditProductionOrderExtraExpensesSheet",
                data: { productionOrderId: productionOrderId, extraExpensesSheetId: extraExpensesSheetId },
                success: function (result) {
                    $("#productionOrderExtraExpensesSheetEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderExtraExpensesSheetEdit"));
                    ShowModal("productionOrderExtraExpensesSheetEdit");
                    $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesContractorName").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExtraExpensesSheetList");
                }
            });
        });

        // Сбрасываем курс валюты при смене валюты во время редактирования листа дополнительных расходов
        $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyId").live("change", function () {
            $("#productionOrderExtraExpensesSheetEdit .ExtraExpensesSheetCurrencyLiteralCode").text($("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyId option:selected").text());
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateId").val("");
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateName").text("текущий");
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRate").text("---");
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateForEdit").val("");
            ProductionOrder_Details.RecalculateExtraExpensesSheetCostInBaseCurrency();
            // Шлем запрос на получение текущего курса выбранной на модальной форме валюты
            var currencyId = $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyId").val();
            if (currencyId != "" && currencyId != "0") {
                $.ajax({
                    type: "GET",
                    url: "/Currency/GetCurrentCurrencyRate",
                    data: { currencyId: currencyId },
                    success: function (result) {
                        $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRate").text(result.CurrencyRate);

                        $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateForEdit").val(result.CurrencyRateForEdit);
                        ProductionOrder_Details.RecalculateExtraExpensesSheetCostInBaseCurrency();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderExtraExpensesSheetEdit");
                    }
                });
            }
        });

        $("#productionOrderExtraExpensesSheetEdit #linkChangeCurrencyRate").live("click", function () {
            var currencyId = $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyId").val();
            if (currencyId != "" && currencyId != "0") {
                $.ajax({
                    type: "GET",
                    url: "/Currency/SelectCurrencyRate",
                    data: { currencyId: currencyId, selectFunctionName: "OnExtraExpensesSheetEditCurrencyRateSelectLinkClick" },
                    success: function (result) {
                        $('#currencyRateSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyRateSelector"));
                        ShowModal("currencyRateSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderExtraExpensesSheetEdit");
                    }
                });
            }
            else {
                StopLinkProgress();
                $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyId").ValidationError("Укажите валюту");
            }
        });

        $("#productionOrderExtraExpensesSheetEdit #CostInCurrency").live("keyup change paste cut", function () {
            ProductionOrder_Details.RecalculateExtraExpensesSheetCostInBaseCurrency();
        });

        $("#gridProductionOrderExtraExpensesSheet .linkExtraExpensesSheetDelete").live("click", function () {
            if (confirm('Вы уверены?')) {
                var productionOrderId = $("#Id").val();
                var extraExpensesSheetId = $(this).parent("td").parent("tr").find(".Id").text();

                StartGridProgress($(this).closest(".grid"));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/DeleteProductionOrderExtraExpensesSheet/",
                    data: { productionOrderId: productionOrderId, extraExpensesSheetId: extraExpensesSheetId },
                    success: function (result) {
                        RefreshGrid("gridProductionOrderExtraExpensesSheet", function () {
                            ProductionOrder_Details.RefreshMainDetailsAndPermissions(result);
                            ShowSuccessMessage("Лист дополнительных расходов удален.", "messageExtraExpensesSheetList");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExtraExpensesSheetList");
                    }
                });
            }
        });

        // Операции над таможенными листами

        $("#gridProductionOrderCustomsDeclaration #btnCreateCustomsDeclaration").live("click", function () {
            var productionOrderId = $("#Id").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/AddProductionOrderCustomsDeclaration",
                data: { productionOrderId: productionOrderId },
                success: function (result) {
                    $("#productionOrderCustomsDeclarationEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderCustomsDeclarationEdit"));
                    ShowModal("productionOrderCustomsDeclarationEdit");
                    $("#productionOrderCustomsDeclarationEdit #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageCustomsDeclarationList");
                }
            });
        });

        $("#gridProductionOrderCustomsDeclaration .linkCustomsDeclarationEdit").live("click", function () {
            var productionOrderId = $("#Id").val();
            var customsDeclarationId = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/EditProductionOrderCustomsDeclaration",
                data: { productionOrderId: productionOrderId, customsDeclarationId: customsDeclarationId },
                success: function (result) {
                    $("#productionOrderCustomsDeclarationEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderCustomsDeclarationEdit"));
                    ShowModal("productionOrderCustomsDeclarationEdit");
                    $("#productionOrderCustomsDeclarationEdit #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageCustomsDeclarationList");
                }
            });
        });

        $("#gridProductionOrderCustomsDeclaration .linkCustomsDeclarationDelete").live("click", function () {
            if (confirm('Вы уверены?')) {
                var productionOrderId = $("#Id").val();
                var customsDeclarationId = $(this).parent("td").parent("tr").find(".Id").text();

                StartGridProgress($(this).closest(".grid"));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/DeleteProductionOrderCustomsDeclaration/",
                    data: { productionOrderId: productionOrderId, customsDeclarationId: customsDeclarationId },
                    success: function (result) {
                        RefreshGrid("gridProductionOrderCustomsDeclaration", function () {
                            ProductionOrder_Details.RefreshMainDetailsAndPermissions(result);
                            ShowSuccessMessage("Таможенный лист удален.", "messageCustomsDeclarationList");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageCustomsDeclarationList");
                    }
                });
            }
        });

        // Операции над плановыми оплатами

        $("#productionOrderEditPlannedPayments #btnAddPlannedPayment").live("click", function () {
            var productionOrderId = $("#Id").val();

            StartButtonProgress($(this));
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/CreateProductionOrderPlannedPayment",
                data: { productionOrderId: productionOrderId },
                success: function (result) {
                    $("#productionOrderPlannedPaymentEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderPlannedPaymentEdit"));
                    ShowModal("productionOrderPlannedPaymentEdit");
                    $("#productionOrderPlannedPaymentEdit #SumInCurrency").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEditPlannedPayments");
                }
            });
        });

        $("#productionOrderEditPlannedPayments #gridProductionOrderPlannedPayment .linkEditPlannedPayment").live("click", function () {
            var productionOrderPlannedPaymentId = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/EditProductionOrderPlannedPayment",
                data: { productionOrderPlannedPaymentId: productionOrderPlannedPaymentId },
                success: function (result) {
                    $("#productionOrderPlannedPaymentEdit").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderPlannedPaymentEdit"));
                    ShowModal("productionOrderPlannedPaymentEdit");
                    $("#productionOrderPlannedPaymentEdit #SumInCurrency").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEditPlannedPayments");
                }
            });
        });

        $("#productionOrderEditPlannedPayments #gridProductionOrderPlannedPayment .linkDeletePlannedPayment").live("click", function () {
            if (confirm('Вы уверены?')) {
                var productionOrderPlannedPaymentId = $(this).parent("td").parent("tr").find(".Id").text();

                StartGridProgress($(this).closest(".grid"));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/DeleteProductionOrderPlannedPayment/",
                    data: { productionOrderPlannedPaymentId: productionOrderPlannedPaymentId },
                    success: function (result) {
                        RefreshGrid("gridProductionOrderPlannedPayment", function () {
                            ShowSuccessMessage("Оплата удалена.", "messageProductionOrderEditPlannedPayments");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEditPlannedPayments");
                    }
                });
            }
        });

        // Сбрасываем курс валюты при смене валюты во время редактирования плановой оплаты и меняем поле с ее названием
        $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyId").live("change", function () {
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyLiteralCode").text($("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyId option:selected").text());
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateId").val("");
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateName").text("текущий");
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateString").text("---");
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateValue").val("");
            // Шлем запрос на получение текущего курса выбранной на модальной форме валюты
            var currencyId = $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyId").val();
            if (!IsDefaultOrEmpty(currencyId)) {
                $.ajax({
                    type: "GET",
                    url: "/Currency/GetCurrentCurrencyRate",
                    data: { currencyId: currencyId },
                    success: function (result) {
                        $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateString").text(result.CurrencyRate);
                        $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateValue").val(result.CurrencyRateForEdit);
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPlannedPaymentEdit");
                    }
                });
            }
        });

        $("#productionOrderPlannedPaymentEdit #linkChangePlannedPaymentCurrencyRate").live("click", function () {
            var currencyId = $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyId").val();
            if (!IsDefaultOrEmpty(currencyId)) {
                $.ajax({
                    type: "GET",
                    url: "/Currency/SelectCurrencyRate",
                    data: { currencyId: currencyId, selectFunctionName: "OnPlannedPaymentEditCurrencyRateSelectLinkClick" },
                    success: function (result) {
                        $('#currencyRateSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyRateSelector"));
                        ShowModal("currencyRateSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPlannedPaymentEdit");
                    }
                });
            }
            else {
                StopLinkProgress();
                $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyId").ValidationError("Укажите валюту");
            }
        });

        // Оплаты

        $("#gridProductionOrderPayment #btnCreateProductionOrderPayment").live("click", function () {
            var productionOrderId = $("#Id").val();
            StartButtonProgress($(this));
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ProductionOrderPaymentTypeSelect",
                data: { productionOrderId: productionOrderId },
                success: function (result) {
                    $("#productionOrderPaymentTypeSelector").hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderPaymentTypeSelector"));
                    ShowModal("productionOrderPaymentTypeSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentList");
                }
            });
        });

        $("#productionOrderPaymentTypeSelector #linkProduction").live("click", function () {
            var productionOrderId = $("#Id").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/CreateProductionOrderPayment",
                data: { productionOrderId: productionOrderId, productionOrderPaymentTypeId: "1", productionOrderPaymentDocumentId: "00000000-0000-0000-0000-000000000000" },
                success: function (result) {
                    $("#productionOrderPaymentEdit").hide().html(result);
                    HideModal(function () {
                        $.validator.unobtrusive.parse($("#productionOrderPaymentEdit"));
                        ShowModal("productionOrderPaymentEdit");
                        $("#productionOrderPaymentEdit #PaymentDocumentNumber").focus();
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentTypeSelect");
                }
            });
        });

        $("#productionOrderPaymentTypeSelector #linkTransportation").live("click", function () {
            var productionOrderId = $("#Id").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ProductionOrderPaymentDocumentSelect",
                data: { productionOrderId: productionOrderId, productionOrderPaymentTypeId: "2" },
                success: function (result) {
                    $("#productionOrderPaymentDocumentSelector").hide().html(result);
                    HideModal(function () {
                        $.validator.unobtrusive.parse($("#productionOrderPaymentDocumentSelector"));
                        ShowModal("productionOrderPaymentDocumentSelector");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentTypeSelect");
                }
            });
        });

        $("#productionOrderPaymentTypeSelector #linkExtraExpenses").live("click", function () {
            var productionOrderId = $("#Id").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ProductionOrderPaymentDocumentSelect",
                data: { productionOrderId: productionOrderId, productionOrderPaymentTypeId: "3" },
                success: function (result) {
                    $("#productionOrderPaymentDocumentSelector").hide().html(result);
                    HideModal(function () {
                        $.validator.unobtrusive.parse($("#productionOrderPaymentDocumentSelector"));
                        ShowModal("productionOrderPaymentDocumentSelector");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentTypeSelect");
                }
            });
        });

        $("#productionOrderPaymentTypeSelector #linkCustoms").live("click", function () {
            var productionOrderId = $("#Id").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ProductionOrderPaymentDocumentSelect",
                data: { productionOrderId: productionOrderId, productionOrderPaymentTypeId: "4" },
                success: function (result) {
                    $("#productionOrderPaymentDocumentSelector").hide().html(result);
                    HideModal(function () {
                        $.validator.unobtrusive.parse($("#productionOrderPaymentDocumentSelector"));
                        ShowModal("productionOrderPaymentDocumentSelector");
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentTypeSelect");
                }
            });
        });

        $("#productionOrderPaymentDocumentSelector #gridProductionOrderPaymentDocument .linkPaymentDocumentSelect").live("click", function () {
            var productionOrderId = $("#Id").val();
            var productionOrderPaymentTypeId = $("#productionOrderPaymentDocumentSelector #ProductionOrderPaymentTypeId").val();
            var productionOrderPaymentDocumentId = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/CreateProductionOrderPayment",
                data: { productionOrderId: productionOrderId, productionOrderPaymentTypeId: productionOrderPaymentTypeId,
                    productionOrderPaymentDocumentId: productionOrderPaymentDocumentId
                },
                success: function (result) {
                    $("#productionOrderPaymentEdit").hide().html(result);
                    HideModal(function () {
                        $.validator.unobtrusive.parse($("#productionOrderPaymentEdit"));
                        ShowModal("productionOrderPaymentEdit");
                        $("#productionOrderPaymentEdit #PaymentDocumentNumber").focus();
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentDocumentSelectList");
                }
            });
        });

        $("#productionOrderPaymentEdit #SumInCurrency").live("keyup change paste cut", function () {
            var sumInCurrency = TryGetDecimal($("#productionOrderPaymentEdit #SumInCurrency").val());
            var currencyRate = TryGetDecimal($("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val());
            var sumInBaseCurrency = (sumInCurrency * currencyRate);

            if (isNaN(sumInBaseCurrency)) {
                $("#productionOrderPaymentEdit #SumInBaseCurrency").text("---");
            } else {
                $("#productionOrderPaymentEdit #SumInBaseCurrency").text(ValueForDisplay(sumInBaseCurrency, 2));
            }
        });

        $("#gridProductionOrderPayment .linkPaymentDelete").live("click", function () {
            if (confirm('Вы уверены?')) {
                var productionOrderId = $("#Id").val();
                var paymentId = $(this).parent("td").parent("tr").find(".Id").text();
                var paymentTypeId = $(this).parent("td").parent("tr").find(".PaymentTypeId").text();

                StartGridProgress($(this).closest(".grid"));
                $.ajax({
                    type: "POST",
                    url: "/ProductionOrder/DeleteProductionOrderPayment/",
                    data: { productionOrderId: productionOrderId, paymentId: paymentId },
                    success: function (result) {
                        RefreshGrid("gridProductionOrderPayment", function () {
                            ProductionOrder_Details.RefreshMainDetailsAndPermissions(result);
                            ShowSuccessMessage("Оплата удалена.", "messageProductionOrderPaymentList");
                        });

                        ProductionOrder_Details.RefreshPaymentDependentGrids(paymentTypeId, "Оплата удалена.");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentList");
                    }
                });
            }
        });

        $("#gridProductionOrderPayment .linkPaymentDetails").live("click", function () {
            var id = $(this).parent("td").parent("tr").find(".Id").text();
            $.ajax({
                type: "GET",
                url: "/ProductionOrderPayment/Details",
                data: { productionOrderPaymentId: id },
                success: function (result) {
                    $('#productionOrderPaymentEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#productionOrderPaymentEdit"));
                    ShowModal("productionOrderPaymentEdit");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentList");
                }
            });
        });

        $("#productionOrderPaymentEdit #linkChangePaymentCurrencyRate").live("click", function () {
            var currencyId = $("#productionOrderPaymentEdit #PaymentCurrencyId").val();
            if (currencyId != "" && currencyId != "0") {
                $.ajax({
                    type: "GET",
                    url: "/Currency/SelectCurrencyRate",
                    data: { currencyId: currencyId, selectFunctionName: "OnProductionOrderPaymentEditCurrencyRateSelectLinkClick" },
                    success: function (result) {
                        $('#currencyRateSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyRateSelector"));
                        ShowModal("currencyRateSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentEdit");
                    }
                });
            }
        });


    },

    // Обновить ссылку на куратора. Если параметр определен, то произошло обновление главных деталей, и html-код ссылки тоже пересоздается
    UpdateCuratorLink: function (text) {
        var curatorId = $("#CuratorId").val();
        if (text != undefined) {
            if (curatorId != "") text = '<a id="CuratorLink">' + text + '</a>';
            $("#CuratorName").html(text);
        }

        if (IsTrue($("#AllowToViewCuratorDetails").val())) {
            $("#CuratorName").attr("href", "/User/Details?id=" + curatorId + GetBackUrl());
        }
        else {
            $("#CuratorName").addClass("disabled");
        }
    },

    // Обновить ссылку на собственную организацию. Если параметр определен, то произошло обновление главных деталей, и html-код ссылки тоже пересоздается
    UpdateAccountOrganizationLink: function (text) {
        var accountOrganizationId = $("#AccountOrganizationId").val();
        if (text != undefined) {
            if (accountOrganizationId != "") text = '<a id="AccountOrganizationLink">' + text + '</a>';
            $("#AccountOrganizationName").html(text);
        }
        $("#AccountOrganizationLink").attr("href", "/AccountOrganization/Details?id=" + accountOrganizationId + GetBackUrl());
    },

    // Обновить ссылку на место хранения. Если параметр определен, то произошло обновление главных деталей, и html-код ссылки тоже пересоздается
    UpdateStorageLink: function (text) {
        if (IsTrue($("#AllowToViewStorageDetails").val())) {
            var storageId = $("#StorageId").val();
            if (text != undefined) {
                if (storageId != "") text = '<a id="StorageLink">' + text + '</a>';
                $("#StorageName").html(text);
            }
            $("#StorageLink").attr("href", "/Storage/Details?id=" + storageId + GetBackUrl());
        }
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора курсов валюты из главных деталей
    OnCurrencyRateSelectLinkClick: function (currencyId, currencyRateId) {
        var productionOrderId = $("#Id").val();
        $.ajax({
            type: "POST",
            url: "/ProductionOrder/ChangeCurrencyRate",
            data: { productionOrderId: productionOrderId, currencyId: currencyId, currencyRateId: currencyRateId },
            success: function (result) {
                ProductionOrder_Details.RefreshMainDetailsAndPermissions(result);
                RefreshGrid("gridProductionOrderPayment", function () {
                    HideModal(function () {
                        ProductionOrder_Details.RefreshBatchGrid("messageBatchList");
                    });
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyRateSelectList");
            }
        });
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора собственных организаций (при создании контракта)
    OnAccountOrganizationSelectLinkClick: function (accountOrganizationId, accountOrganizationShortName) {
        $("#producerContractEdit #AccountOrganizationId").val(accountOrganizationId).ValidationValid();
        $("#producerContractEdit #AccountOrganizationName").text(accountOrganizationShortName);
        HideModal();
    },

    OnSuccessContractEdit: function (result) {
        ProductionOrder_Details.RefreshMainDetailsAndPermissions(result);

        if ($("#producerContractEdit #Id").val() != "0") {
            ShowSuccessMessage("Сохранено.", "messageProductionOrderEdit");
        }
        else {
            ShowSuccessMessage("Контракт создан.", "messageProductionOrderEdit");
        }
        HideModal();
    },

    OnBeginProductionOrderSave: function () {
        StartButtonProgress($("#btnSave"));
    },

    OnSuccessProductionOrderCurrencyDeterminationTypeSelect: function (ajaxContext) {
        var productionOrderCurrencyDocumentType = $("#productionOrderCurrencyDeterminationTypeSelector #ProductionOrderCurrencyDocumentType").val();
        if (productionOrderCurrencyDocumentType == "1") {
            // Транспортный лист
            $("#productionOrderTransportSheetEdit").hide().html(ajaxContext);
            HideModal(function () {
                $.validator.unobtrusive.parse($("#productionOrderTransportSheetEdit"));
                ShowModal("productionOrderTransportSheetEdit");
                $("#productionOrderTransportSheetEdit #ForwarderName").focus();
            });
        } else if (productionOrderCurrencyDocumentType == "2") {
            // Лист дополнительных расходов
            $("#productionOrderExtraExpensesSheetEdit").hide().html(ajaxContext);
            HideModal(function () {
                $.validator.unobtrusive.parse($("#productionOrderExtraExpensesSheetEdit"));
                ShowModal("productionOrderExtraExpensesSheetEdit");
                $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesContractorName").focus();
            });
        }
    },

    OnFailProductionOrderCurrencyDeterminationTypeSelect: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderCurrencyDeterminationTypeSelect");
    },

    OnBeginProductionOrderCurrencyDeterminationTypeSelect: function () {
        StartButtonProgress($("#btnProductionOrderCurrencyDeterminationTypeSelect"));
    },

    // Работа с транспортными листами

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора курсов валюты из формы редактирования транспортного листа
    OnTransportSheetEditCurrencyRateSelectLinkClick: function (currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateId").val(currencyRateId);
        if (currencyRateId != "") { // конкретный курс
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateName").text("на " + currencyRateStartDate);
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRate").text(currencyRate);
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateForEdit").val(currencyRateForEdit);
            ProductionOrder_Details.RecalculateTransportSheetCostInBaseCurrency();
            HideModal();
        } else { // текущий курс
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateName").text("текущий");
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRate").text("---");
            $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateForEdit").val("");
            ProductionOrder_Details.RecalculateTransportSheetCostInBaseCurrency();
            HideModal(function () {
                // Шлем запрос на получение текущего курса выбранной на модальной форме валюты
                var currencyId = $("#productionOrderTransportSheetEdit #TransportSheetCurrencyId").val();
                $.ajax({
                    type: "GET",
                    url: "/Currency/GetCurrentCurrencyRate",
                    data: { currencyId: currencyId },
                    success: function (result) {
                        $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRate").text(result.CurrencyRate);
                        $("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateForEdit").val(result.CurrencyRateForEdit);
                        ProductionOrder_Details.RecalculateTransportSheetCostInBaseCurrency();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderTransportSheetEdit");
                    }
                });
            });
        }
    },

    OnSuccessProductionOrderTransportSheetEdit: function (ajaxContext) {
        RefreshGrid("gridProductionOrderTransportSheet", function () {
            ProductionOrder_Details.RefreshMainDetailsAndPermissions(ajaxContext);
            if ($("#productionOrderTransportSheetEdit #TransportSheetId").val() != "00000000-0000-0000-0000-000000000000") {
                RefreshGrid("gridProductionOrderPayment", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageTransportSheetList");
                    });
                });
            }
            else {
                HideModal(function () {
                    ShowSuccessMessage("Транспортный лист создан.", "messageTransportSheetList");
                });
            }
        });
    },

    OnFailProductionOrderTransportSheetEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderTransportSheetEdit");
    },

    RecalculateTransportSheetCostInBaseCurrency: function () {
        var costInCurrency = TryGetDecimal($("#productionOrderTransportSheetEdit #CostInCurrency").val());
        var currencyRate = TryGetDecimal($("#productionOrderTransportSheetEdit #TransportSheetCurrencyRateForEdit").val());
        var costInBaseCurrency = (costInCurrency * currencyRate);

        if (isNaN(costInBaseCurrency)) {
            $("#productionOrderTransportSheetEdit #CostInBaseCurrency").text("---");
        } else {
            $("#productionOrderTransportSheetEdit #CostInBaseCurrency").text(ValueForDisplay(costInBaseCurrency, 2));
        }
    },

    // Работа с листами дополнительных расходов

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора курсов валюты из формы редактирования листа дополнительных расходов
    OnExtraExpensesSheetEditCurrencyRateSelectLinkClick: function (currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateId").val(currencyRateId);
        if (currencyRateId != "") { // конкретный курс
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateName").text("на " + currencyRateStartDate);
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRate").text(currencyRate);
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateForEdit").val(currencyRateForEdit);
            ProductionOrder_Details.RecalculateExtraExpensesSheetCostInBaseCurrency();
            HideModal();
        } else { // текущий курс
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateName").text("текущий");
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRate").text("---");
            $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateForEdit").val("");
            ProductionOrder_Details.RecalculateExtraExpensesSheetCostInBaseCurrency();
            HideModal(function () {
                // Шлем запрос на получение текущего курса выбранной на модальной форме валюты
                var currencyId = $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyId").val();
                $.ajax({
                    type: "GET",
                    url: "/Currency/GetCurrentCurrencyRate",
                    data: { currencyId: currencyId },
                    success: function (result) {
                        $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRate").text(result.CurrencyRate);
                        $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateForEdit").val(result.CurrencyRateForEdit);
                        ProductionOrder_Details.RecalculateExtraExpensesSheetCostInBaseCurrency();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderExtraExpensesSheetEdit");
                    }
                });
            });
        }
    },

    OnSuccessProductionOrderExtraExpensesSheetEdit: function (ajaxContext) {
        RefreshGrid("gridProductionOrderExtraExpensesSheet", function () {
            ProductionOrder_Details.RefreshMainDetailsAndPermissions(ajaxContext);
            if ($("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetId").val() != "00000000-0000-0000-0000-000000000000") {
                RefreshGrid("gridProductionOrderPayment", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageExtraExpensesSheetList");
                    });
                });
            }
            else {
                HideModal(function () {
                    ShowSuccessMessage("Лист дополнительных расходов создан.", "messageExtraExpensesSheetList");
                });
            }
        });
    },

    OnFailProductionOrderExtraExpensesSheetEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderExtraExpensesSheetEdit");
    },

    OnBeginProductionOrderExtraExpensesSheetEdit: function () {
        StartButtonProgress($("#btnProductionOrderExtraExpensesSheetEdit"));
    },

    OnBeginProductionOrderCustomsDeclarationEdit: function () {
        StartButtonProgress($("#btnProductionOrderCustomsDeclarationSave"));
    },

    RecalculateExtraExpensesSheetCostInBaseCurrency: function () {
        var costInCurrency = TryGetDecimal($("#productionOrderExtraExpensesSheetEdit #CostInCurrency").val());
        var currencyRate = TryGetDecimal($("#productionOrderExtraExpensesSheetEdit #ExtraExpensesSheetCurrencyRateForEdit").val());
        var costInBaseCurrency = (costInCurrency * currencyRate);

        if (isNaN(costInBaseCurrency)) {
            $("#productionOrderExtraExpensesSheetEdit #CostInBaseCurrency").text("---");
        } else {
            $("#productionOrderExtraExpensesSheetEdit #CostInBaseCurrency").text(ValueForDisplay(costInBaseCurrency, 2));
        }
    },

    // Работа с таможенными листами

    OnSuccessProductionOrderCustomsDeclarationEdit: function (ajaxContext) {
        ProductionOrder_Details.RefreshMainDetailsAndPermissions(ajaxContext);
        RefreshGrid("gridProductionOrderCustomsDeclaration", function () {
            if ($("#productionOrderCustomsDeclarationEdit #CustomsDeclarationId").val() != "00000000-0000-0000-0000-000000000000") {
                RefreshGrid("gridProductionOrderPayment", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageCustomsDeclarationList");
                    });
                });
            }
            else {
                HideModal(function () {
                    ShowSuccessMessage("Таможенный лист создан.", "messageCustomsDeclarationList");
                });
            }
        });
    },

    OnFailProductionOrderCustomsDeclarationEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderCustomsDeclarationEdit");
    },

    // Плановые оплаты

    OnBeginProductionOrderPlannedPaymentEdit: function (ajaxContext) {
        StartButtonProgress($("#btnSaveProductionOrderPlannedPayment"));
    },

    OnSuccessProductionOrderPlannedPaymentEdit: function (ajaxContext) {
        RefreshGrid("gridProductionOrderPlannedPayment", function () {
            HideModal(function () {
                ShowSuccessMessage("Сохранено.", "messageProductionOrderEditPlannedPayments");
            });
        });
    },

    OnFailProductionOrderPlannedPaymentEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderPlannedPaymentEdit");
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора курсов валюты из формы редактирования плановой оплаты
    OnPlannedPaymentEditCurrencyRateSelectLinkClick: function (currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateId").val(currencyRateId);
        if (!IsDefaultOrEmpty(currencyRateId)) { // конкретный курс
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateName").text("на " + currencyRateStartDate);
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateString").text(currencyRate);
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateValue").val(currencyRateForEdit);
            HideModal();
        } else { // текущий курс
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateName").text("текущий");
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateString").text("---");
            $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateValue").val("");
            HideModal(function () {
                // Шлем запрос на получение текущего курса выбранной на модальной форме валюты
                var currencyId = $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyId").val();
                $.ajax({
                    type: "GET",
                    url: "/Currency/GetCurrentCurrencyRate",
                    data: { currencyId: currencyId },
                    success: function (result) {
                        $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateString").text(result.CurrencyRate);
                        $("#productionOrderPlannedPaymentEdit #PlannedPaymentCurrencyRateValue").val(result.CurrencyRateForEdit);
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPlannedPaymentEdit");
                    }
                });
            });
        }
    },

    // Оплаты

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора курсов валюты из формы редактирования оплаты
    OnProductionOrderPaymentEditCurrencyRateSelectLinkClick: function (currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        var paymentId = $("#productionOrderPaymentEdit #ProductionOrderPaymentId").val();
        if (IsDefaultOrEmpty(paymentId)) {
            $("#productionOrderPaymentEdit #PaymentCurrencyRateId").val(currencyRateId);
            if (currencyRateId != "") { // конкретный курс
                $("#productionOrderPaymentEdit #PaymentCurrencyRateName").text("на " + currencyRateStartDate);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateString").text(currencyRate);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val(currencyRateForEdit);
                ProductionOrder_Details.RecalculateProductionOrderPaymentSumInBaseCurrency();
                HideModal();
            } else { // текущий курс
                $("#productionOrderPaymentEdit #PaymentCurrencyRateName").text("текущий");
                $("#productionOrderPaymentEdit #PaymentCurrencyRateString").text("---");
                $("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val("");
                ProductionOrder_Details.RecalculateProductionOrderPaymentSumInBaseCurrency();
                HideModal(function () {
                    // Шлем запрос на получение текущего курса выбранной на модальной форме валюты
                    var currencyId = $("#productionOrderPaymentEdit #PaymentCurrencyId").val();
                    $.ajax({
                        type: "GET",
                        url: "/Currency/GetCurrentCurrencyRate",
                        data: { currencyId: currencyId },
                        success: function (result) {
                            $("#productionOrderPaymentEdit #PaymentCurrencyRateString").text(result.CurrencyRate);
                            $("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val(result.CurrencyRateForEdit);
                            ProductionOrder_Details.RecalculateProductionOrderPaymentSumInBaseCurrency();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderPaymentEdit");
                        }
                    });
                });
            }
        } else {
            ProductionOrder_Details.ChangeProductionOrderPaymentCurrencyRate(paymentId, currencyRateId);
        }
    },

    ChangeProductionOrderPaymentCurrencyRate: function (productionOrderPaymentId, currencyRateId) {
        $.ajax({
            type: "POST",
            url: "/ProductionOrderPayment/ChangeProductionOrderPaymentCurrencyRate",
            data: { productionOrderPaymentId: productionOrderPaymentId, currencyRateId: currencyRateId },
            success: function (result) {
                // Обновление модальной формы
                $("#productionOrderPaymentEdit #PaymentCurrencyRateId").val(productionOrderPaymentId);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateName").text(result.PaymentCurrencyRateName);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateString").text(result.PaymentCurrencyRateString);
                $("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val(result.PaymentCurrencyRateValue);
                ProductionOrder_Details.RecalculateProductionOrderPaymentSumInBaseCurrency();

                // Обновление главных деталей заказа
                $("#main_page #PaymentSumInCurrency").text(result.PaymentSumInCurrency);
                $("#main_page #PaymentSumInCurrencyValue").val(result.PaymentSumInCurrencyValue);
                $("#main_page #PaymentSumInBaseCurrency").text(result.PaymentSumInBaseCurrency);
                $("#main_page #PaymentSumInBaseCurrencyValue").val(result.PaymentSumInBaseCurrencyValue);
                $("#main_page #PaymentPercent").text(result.PaymentPercent);
                ProductionOrder_Details.RefreshColorOfSumIndicators();

                RefreshGrid("gridProductionOrderPayment", function () {

                    var productionOrderPlannedPaymentId = $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentId").val();
                    // Если плановый платеж указан, то ...
                    if (productionOrderPlannedPaymentId.length > 0) {
                        // ... запрашиваем детали плановой оплаты
                        $.ajax({
                            type: "POST",
                            url: "/ProductionOrder/GetPlannedPaymentInfo",
                            data: { productionOrderPlannedPaymentId: productionOrderPlannedPaymentId },
                            success: function (result) {
                                // Обновление полей
                                $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentSumInCurrency").text(result.PlannedPaymentSumInCurrency);
                                $("#productionOrderPaymentEdit .ProductionOrderPlannedPaymentCurrencyLiteralCode").text(result.PlannedPaymentCurrencyLiteralCode);
                                $("#productionOrderPaymentEdit #ProductionOrderPlannedPaymentPaidSumInBaseCurrency").text(result.PaymentSumInBaseCurrency);
                                HideModal(function () {
                                    ShowSuccessMessage("Курс оплаты сохранен.", "messageProductionOrderPaymentEdit");
                                });
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyEdit");
                            }
                        });
                    } else {    // иначе закрываем МФ
                        HideModal(function () {
                            ShowSuccessMessage("Курс оплаты сохранен.", "messageProductionOrderPaymentEdit");
                        });
                    }

                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyEdit");
            }
        });
    },

    RefreshPaymentDependentGrids: function (paymentTypeId, message) {
        switch (paymentTypeId) {
            case "1":
                ShowSuccessMessage(message, "messageProductionOrderPaymentList");
                break;
            case "2":
                RefreshGrid("gridProductionOrderTransportSheet", function () {
                    ShowSuccessMessage(message, "messageProductionOrderPaymentList");
                });
                break;
            case "3":
                RefreshGrid("gridProductionOrderExtraExpensesSheet", function () {
                    ShowSuccessMessage(message, "messageProductionOrderPaymentList");
                });
                break;
            case "4":
                RefreshGrid("gridProductionOrderCustomsDeclaration", function () {
                    ShowSuccessMessage(message, "messageProductionOrderPaymentList");
                });
                break;
            default:
                ShowErrorMessage("Неизвестное назначение оплаты.", "messageProductionOrderPaymentList");
        };
    },

    OnSuccessProductionOrderPaymentEdit: function (ajaxContext) {
        RefreshGrid("gridProductionOrderPayment", function () {
            ProductionOrder_Details.RefreshMainDetailsAndPermissions(ajaxContext);
            HideModal(function () {
                // Освежаем один из трех гридов, в которых может измениться процент оплаты
                var productionOrderPaymentTypeId = $("#main_page #ProductionOrderPaymentTypeId").val();
                ProductionOrder_Details.RefreshPaymentDependentGrids(productionOrderPaymentTypeId, "Оплата добавлена.");
            });
        });
    },

    OnFailProductionOrderPaymentEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderPaymentEdit");
    },

    OnBeginProductionOrderPaymentEdit: function () {
        StartButtonProgress($("#btnSaveProductionOrderPayment"));
    },

    ShowConfirmCloseOrder: function () {
        ShowConfirm("Закрыть заказ?", "При закрытии заказа будет рассчитана себестоимость товара.", "Закрыть заказ",
                "Отмена",
                function () {
                    var productionOrderId = $("#Id").val();
                    $.ajax({
                        type: "GET",
                        url: "/ProductionOrder/Close",
                        data: { productionOrderId: productionOrderId },
                        success: function (result) {
                            window.location = window.location;
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageConfirmError");
                        }
                    });
                });
    },

    // Вызывается также из деталей производителя, а также из страницы списка оплат по заказам. Можно вынести в Shared или в POPayment
    RecalculateProductionOrderPaymentSumInBaseCurrency: function () {
        var sumInCurrency = TryGetDecimal($("#productionOrderPaymentEdit #SumInCurrency").val().replaceAll(' ', ''));
        var currencyRate = TryGetDecimal($("#productionOrderPaymentEdit #PaymentCurrencyRateValue").val());
        var sumInBaseCurrency = sumInCurrency * currencyRate;

        if (isNaN(sumInBaseCurrency)) {
            $("#productionOrderPaymentEdit #SumInBaseCurrency").text("---");
        } else {
            $("#productionOrderPaymentEdit #SumInBaseCurrency").text(ValueForDisplay(sumInBaseCurrency, 2));
        }
    },

    // ===============================

    RefreshMainDetailsAndPermissions: function (result) {
        ProductionOrder_Details.RefreshMainDetails(result.MainDetails);
        ProductionOrder_Details.RefreshPermissions(result.MainDetails);
    },

    RefreshMainDetails: function (details) {
        $(".page_title_item_name").text(details.Name);
        $("#main_page #Name").text(details.Name);
        $("#main_page #ProducerName").text(details.ProducerName);
        $("#main_page #CurrentStageName").text(details.CurrentStageName);
        $("#main_page #CurrentStageActualStartDate").text(details.CurrentStageActualStartDate);
        $("#main_page #CurrentStageDaysPassed").text(details.CurrentStageDaysPassed);
        $("#main_page #CurrentStageExpectedEndDate").text(details.CurrentStageExpectedEndDate);
        $("#main_page #CurrentStageDaysLeft").text(details.CurrentStageDaysLeft);
        $("#main_page #State").text(details.State);
        $("#main_page #MinOrderBatchStageName").text(details.MinOrderBatchStageName);
        $("#main_page #MaxOrderBatchStageName").text(details.MaxOrderBatchStageName);
        $("#main_page #ContractName").text(details.ContractName);
        $("#main_page #CurrencyLiteralCode").text(details.CurrencyLiteralCode);
        $("#main_page #CurrencyRateName").text(details.CurrencyRateName);
        $("#main_page #CurrencyRate").text(details.CurrencyRate);
        $("#main_page #Date").text(details.Date);
        $("#main_page #DeliveryPendingDate").text(details.DeliveryPendingDate);
        $("#main_page #DivergenceFromPlan").text(details.DivergenceFromPlan);
        $("#main_page #PlannedExpensesSumInCurrency").text(details.PlannedExpensesSumInCurrency);
        $("#main_page #PlannedExpensesSumInCurrencyValue").val(details.PlannedExpensesSumInCurrencyValue);
        $("#main_page #PlannedExpensesSumInBaseCurrency").text(details.PlannedExpensesSumInBaseCurrency);
        $("#main_page #PlannedExpensesSumInBaseCurrencyValue").val(details.PlannedExpensesSumInBaseCurrencyValue);
        $("#main_page #ActualCostSumInCurrency").text(details.ActualCostSumInCurrency);
        $("#main_page #ActualCostSumInCurrencyValue").val(details.ActualCostSumInCurrencyValue);
        $("#main_page #ActualCostSumInBaseCurrency").text(details.ActualCostSumInBaseCurrency);
        $("#main_page #ActualCostSumInBaseCurrencyValue").val(details.ActualCostSumInBaseCurrencyValue);
        $("#main_page #PaymentSumInCurrency").text(details.PaymentSumInCurrency);
        $("#main_page #PaymentSumInCurrencyValue").val(details.PaymentSumInCurrencyValue);
        $("#main_page #PaymentSumInBaseCurrency").text(details.PaymentSumInBaseCurrency);
        $("#main_page #PaymentSumInBaseCurrencyValue").val(details.PaymentSumInBaseCurrencyValue);
        $("#main_page #PaymentPercent").text(details.PaymentPercent);
        $("#main_page #AccountingPriceSum").text(details.AccountingPriceSum);
        $("#main_page #MarkupPendingSum").text(details.MarkupPendingSum);
        $("#main_page #ArticleTransportingPrimeCostCalculationType").text(details.ArticleTransportingPrimeCostCalculationType);
        $("#main_page #Comment").html(details.Comment);

        $("#main_page #AllowToViewStageList").val(details.AllowToViewStageList);

        $("#main_page #CurrencyId").val(details.CurrencyId);
        $("#main_page #CurrencyRateId").val(details.CurrencyRateId);

        $("#main_page #CuratorId").val(details.CuratorId);
        ProductionOrder_Details.UpdateCuratorLink(details.CuratorName);
        $("#main_page #AccountOrganizationId").val(details.AccountOrganizationId);
        ProductionOrder_Details.UpdateAccountOrganizationLink(details.AccountOrganizationName);
        $("#main_page #StorageId").val(details.StorageId);
        ProductionOrder_Details.UpdateStorageLink(details.StorageName);

        ProductionOrder_Details.RefreshColorOfSumIndicators();

        var showExecutionGraph = IsTrue($("#AllowToViewStageList").val());
        UpdateElementVisibility("executionGraph", showExecutionGraph);
    },

    RefreshColorOfSumIndicators: function () {
        //Расчитываем условие окраски фактических затрат
        var actualCostSumInBaseCurrency = TryGetDecimal($("#ActualCostSumInBaseCurrencyValue").val());
        var plannedExpensesSumInBaseCurrency = TryGetDecimal($("#PlannedExpensesSumInBaseCurrencyValue").val());
        var cond1 = actualCostSumInBaseCurrency > plannedExpensesSumInBaseCurrency;

        //Расчитываем условие окраски оплат
        var actualCostSumInCurrency = TryGetDecimal($("#ActualCostSumInCurrencyValue").val());
        var paymentSumInCurrency = TryGetDecimal($("#PaymentSumInCurrencyValue").val());
        var paymentSumInBaseCurrency = TryGetDecimal($("#PaymentSumInBaseCurrencyValue").val());
        var cond2 = actualCostSumInBaseCurrency * paymentSumInCurrency / actualCostSumInCurrency < paymentSumInBaseCurrency;

        if (cond1) {
            $("#ActualCostSumInCurrency").addClass("orangetext").removeClass("greentext");
            $("#ActualCostSumInBaseCurrency").addClass("orangetext").removeClass("greentext");
        }
        else {
            $("#ActualCostSumInCurrency").addClass("greentext").removeClass("orangetext");
            $("#ActualCostSumInBaseCurrency").addClass("greentext").removeClass("orangetext");
        }

        if (cond2) {
            $("#PaymentSumInCurrency").addClass("orangetext").removeClass("greentext");
            $("#PaymentSumInBaseCurrency").addClass("orangetext").removeClass("greentext");
        }
        else {
            $("#PaymentSumInCurrency").addClass("greentext").removeClass("orangetext");
            $("#PaymentSumInBaseCurrency").addClass("greentext").removeClass("orangetext");
        }
    },

    RefreshPermissions: function (permissions) {
        UpdateButtonCaption("btnEditStages", IsTrue(permissions.AllowToEditStages) ? "Задать этапы" : "Этапы заказа");
        UpdateButtonAvailability("btnEditStages", permissions.AllowToViewStageList);
        UpdateElementVisibility("btnEditStages", permissions.AllowToViewStageList);
        UpdateButtonAvailability("btnEditPlannedPayments", permissions.AllowToViewPlannedPayments);
        UpdateElementVisibility("btnEditPlannedPayments", permissions.AllowToViewPlannedPayments);
        UpdateButtonAvailability("btnEdit", permissions.AllowToEdit);
        UpdateElementVisibility("btnEdit", permissions.AllowToEdit);

        UpdateElementVisibility("linkChangeCurator", permissions.AllowToChangeCurator);
        UpdateElementVisibility("linkChangeStage", permissions.AllowToChangeBatchStage);
        UpdateElementVisibility("linkCreateContract", permissions.AllowToCreateContract);
        UpdateElementVisibility("linkEditContract", permissions.AllowToEditContract);
        UpdateLinkCaption("linkPlannedExpensesSumDetails", IsTrue(permissions.AllowToEditPlannedExpenses) ? "[&nbsp;Ред.&nbsp;]" : "[&nbsp;Дет.&nbsp;]");
        UpdateElementVisibility("linkPlannedExpensesSumDetails", permissions.AllowToViewPlannedExpenses);
    },

    OnSuccessPlannedExpensesEdit: function (ajaxContext) {
        HideModal(function () {
            $("#PlannedExpensesSumInCurrency").text(ajaxContext.PlannedExpensesSumInCurrency);
            $("#PlannedExpensesSumInCurrencyValue").val(ajaxContext.PlannedExpensesSumInCurrencyValue);
            $("#PlannedExpensesSumInBaseCurrency").text(ajaxContext.PlannedExpensesSumInBaseCurrency);
            $("#PlannedExpensesSumInBaseCurrencyValue").val(ajaxContext.PlannedExpensesSumInBaseCurrencyValue);

            ProductionOrder_Details.RefreshColorOfSumIndicators();
        });
    },

    RefreshBatchGrid: function (messageId, onSuccessFunction) {
        var productionOrderId = $("#main_page #Id").val();
        $.ajax({
            type: "POST",
            url: "/ProductionOrder/ShowProductionOrderBatchGrid/",
            data: { id: productionOrderId },
            success: function (result) {
                $("#gridProductionOrderBatch").html(result);
                if (onSuccessFunction != undefined)
                // Вызываем переданный метод
                    onSuccessFunction();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, messageId);
            }
        });
    },

    RefreshExecutionGraph: function (onSuccessFunction) {
        var showExecutionGraph = IsTrue($("#AllowToViewStageList").val());
        if (showExecutionGraph) {
            var productionOrderBatchId = $("#SingleProductionOrderBatchId").val();
            $.ajax({
                type: "GET",
                url: "/ProductionOrder/ShowOrderExecutionGraph/",
                data: { id: productionOrderBatchId },
                success: function (result) {
                    drawExecutionGraph("graph-" + productionOrderBatchId, result);

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
    }

};