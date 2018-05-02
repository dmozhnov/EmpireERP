var ProductionOrder_BatchRowEdit = {
    Init: function () {
        $(document).ready(function () {

            SetFieldScale("#Count", 12, $("#MeasureUnitScale").val(), "#productionOrderBatchRowEdit", true);

            $("#productionOrderBatchRowEdit #ManufacturerName").click(function () {
                var producerId = $('#ProducerId').val();
                $.ajax({
                    type: "GET",
                    url: "/Manufacturer/SelectManufacturerOfProducer/",
                    data: { producerId: producerId },
                    success: function (result) {
                        $('#manufacturerSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#manufacturerSelector"));
                        ShowModal("manufacturerSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowEdit");
                    }
                });
            });

            $('#ProductionCost').focus();
            var articleId = $("#ArticleId").val();
            if (articleId == "" || articleId == 0) {
                $('#productionOrderBatchRowEdit input[type!="button"][type!="submit"][type!="hidden"]').attr("disabled", "disabled");
            }

            // открытие формы выбора товара
            $("#productionOrderBatchRowEdit #ArticleName").bind("click", function () {
                $.ajax({
                    type: "GET",
                    url: "/Article/SelectArticle",
                    success: function (result) {
                        $('#articleSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#articleSelector"));
                        ShowModal("articleSelector");

                        ProductionOrder_BatchRowEdit.BindArticleSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowEdit");
                    }
                });
            });

            //Обрабатываем выбор фабрики-изготовителя
            $("#manufacturerSelector .select").die("click");
            $("#manufacturerSelector .select").live("click", function () {

                var manufacturerId = $(this).parent("td").parent("tr").find(".Id").html();
                var manufacturerName = $(this).parent("td").parent("tr").find(".ManufacturerName").html();

                $("#ManufacturerName").html(manufacturerName);
                $("#ManufacturerId").val(manufacturerId);

                HideModal();
            });

            if ($("#PackHeight").val() > 0 && $("#PackWidth").val() > 0 && $("#PackLength").val() > 0) {
                $("#PackVolume").attr("disabled", "disabled");
            }

            $("#PackHeight, #PackLength, #PackWidth").die("change");
            $("#PackHeight, #PackLength, #PackWidth").live("change", function () {
                var height = parseFloat($("#PackHeight").val());
                var width = parseFloat($("#PackWidth").val());
                var length = parseFloat($("#PackLength").val());

                var volume = height * width * length / 1000000000; //мм3 переводим в м3

                if (volume < 0.000001) volume = 0; //если объем меньше 0.000001, то значение volume становится 1е-7 и ValueForDisplay на нем неверно срабатывает

                if ((isNaN(height) || height <= 0) || (isNaN(width) || width <= 0) || (isNaN(length) || length <= 0)) {
                    $("#PackVolume").removeAttr("disabled");
                }
                else {
                    $("#PackVolume").attr("disabled", "disabled");

                    if (height > 0 && width > 0 && length > 0) {
                        $("#PackVolume").val(ValueForEdit(volume, 4));
                    }
                }

                if (isNaN(volume)) return false;

                var packCount = TryGetDecimal($("#PackCount").val());

                if (!isNaN(packCount)) {
                    ProductionOrder_BatchRowEdit.CalculateVolumes(packCount);
                }
            });

            $("#Count").die("change");
            $("#Count").live("change", function () {
                var count = TryGetDecimal($("#Count").val(), $("#MeasureUnitScale").val());
                if (isNaN(count)) return false;

                var packCount;
                var packSize = TryGetDecimal($("#PackSize").text().replaceAll(" ", ""));
                if (!isNaN(packSize && packSize > 0)) {
                    packCount = Math.floor(count / packSize);
                    $("#PackCount").val(ValueForEdit(packCount));
                    count = packSize * packCount;
                    $("#Count").val(ValueForEdit(count));
                }

                var productionCost = BankRound($("#ProductionCost").val(), 2);
                if (!isNaN(productionCost)) {
                    var totalCost = BankRound(productionCost * count, 2);
                    $("#TotalCost").val(ValueForEdit(totalCost));
                }

                ProductionOrder_BatchRowEdit.CalculateMeasures(packCount);
            });

            $("#PackCount").die("change");
            $("#PackCount").live("change", function () {
                var packCount = TryGetDecimal($("#PackCount").val());
                if (isNaN(packCount)) return false;

                var count;
                var packSize = TryGetDecimal($("#PackSize").text().replaceAll(" ", ""));
                if (!isNaN(packSize)) {
                    count = packSize * packCount;
                    $("#Count").val(ValueForEdit(count));
                }

                var productionCost = BankRound($("#ProductionCost").val(), 2);
                if (!isNaN(productionCost) && !isNaN(count)) {
                    var totalCost = BankRound(productionCost * count, 2);
                    $("#TotalCost").val(ValueForEdit(totalCost));
                }

                ProductionOrder_BatchRowEdit.CalculateMeasures(packCount);
            });

            $("#TotalCost").die("change");
            $("#TotalCost").live("change", function () {
                var totalCost = BankRound($("#TotalCost").val(), 2);
                if (isNaN(totalCost)) return false;

                var productionCost = BankRound($("#ProductionCost").val(), 2);
                if (isNaN(productionCost) || productionCost <= 0) return false;

                var count = TryGetDecimal(totalCost / productionCost, $("#MeasureUnitScale").val());
                
                var packSize = TryGetDecimal($("#PackSize").text().replaceAll(" ", ""));
                if (!isNaN(packSize) && packSize > 0) {
                    var packCount = Math.floor(count / packSize);
                    $("#PackCount").val(ValueForEdit(packCount));

                    count = packSize * packCount;
                    $("#Count").val(ValueForEdit(count));

                    totalCost = BankRound(productionCost * count, 2);
                    $("#TotalCost").val(ValueForEdit(totalCost));

                    ProductionOrder_BatchRowEdit.CalculateMeasures(packCount);
                }
            });

            $("#ProductionCost").die("keyup paste cut");
            $("#ProductionCost").live("keyup paste cut", function () {
                var productionCost = BankRound($("#ProductionCost").val(), 2);
                if ($("#Count, #TotalCost, #PackCount").attr("disabled") && !isNaN(productionCost) && productionCost > 0) {
                    $("#Count, #TotalCost, #PackCount").removeAttr("disabled").val('0');
                }
            });

            $("#ProductionCost").die("change");
            $("#ProductionCost").live("change", function () {
                var productionCost = BankRound($("#ProductionCost").val(), 2);

                if (!isNaN(productionCost) && productionCost > 0) {
                    var countStr = $("#Count").val();
                    if (countStr != "") {
                        var count = TryGetDecimal(countStr, $("#MeasureUnitScale").val());
                        if (!isNaN(count) && count > 0) {
                            var totalCost = BankRound(productionCost * count, 2);
                            $("#TotalCost").val(ValueForEdit(totalCost));
                        }
                    }
                }
                else {
                    $("#Count, #TotalCost, #PackCount").attr("disabled", "disabled").val("");
                }
            });

            $("#PackWeight, #PackVolume").die("keyup change paste cut");
            $("#PackWeight, #PackVolume").live("keyup change paste cut", function () {
                var packCount = TryGetDecimal($("#PackCount").val());
                if (isNaN(packCount)) return false;

                ProductionOrder_BatchRowEdit.CalculateMeasures(packCount);
            });

            $("#AddCountry").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Country/Create/",
                    success: function (result) {
                        $('#countryAdd').hide().html(result);
                        $.validator.unobtrusive.parse($("#countryAdd"));
                        ShowModal("countryAdd");
                        $("#countryAdd #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowEdit");
                    }
                });
            });

        });
    },

    CalculateMeasures: function (count) {
        if ($("#ArticleId").val() != "") {
            ProductionOrder_BatchRowEdit.CalculateWeights(count);
            ProductionOrder_BatchRowEdit.CalculateVolumes(count);
        }
    },

    CalculateWeights: function (packCount) {
        var weight = Number($("#PackWeight").val().replaceAll(",", "."));
        if (isNaN(weight)) weight = 0;

        var previousTotalWeight = Number($("#TotalWeight").text().replaceAll(" ", ""));
        if (isNaN(previousTotalWeight)) previousTotalWeight = 0;

        var totalWeight = weight * packCount;
        $("#TotalWeight").text(ValueForDisplay(totalWeight));

        var previousBatchWeight = Number($("#BatchWeight").text().replaceAll(" ", ""));
        if (isNaN(previousBatchWeight)) previousBatchWeight = 0;
        $("#BatchWeight").text(ValueForDisplay(previousBatchWeight - previousTotalWeight + totalWeight));
    },

    CalculateVolumes: function (packCount) {
        var volume = Number($("#PackVolume").val().replaceAll(",", "."));

        var previousTotalVolume = Number($("#TotalVolume").text().replaceAll(" ", ""));
        if (isNaN(previousTotalVolume)) previousTotalVolume = 0;

        var totalVolume = volume * packCount;
        $("#TotalVolume").text(ValueForDisplay(totalVolume, 4));

        var previousBatchVolume = Number($("#BatchVolume").text().replaceAll(" ", ""));
        if (isNaN(previousBatchVolume)) previousBatchVolume = 0;
        $("#BatchVolume").text(ValueForDisplay(previousBatchVolume - previousTotalVolume + totalVolume, 4));
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live("click", function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            var articleId = $(this).parent("td").parent("tr").find(".articleId").text();
            $("#ArticleId").val(articleId);

            var producerId = $("#ProducerId").val();

            var measureUnitName = $(this).parent("td").parent("tr").find(".MeasureUnitShortName").text();
            $("#MeasureUnitName").text(measureUnitName);
            $("#MeasureUnitNameForPackSize").text(measureUnitName);

            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            $("#MeasureUnitScale").val(measureUnitScale);
            SetFieldScale("#Count", 12, measureUnitScale, "#productionOrderBatchRowEdit", true);

            HideModal();

            $.ajax({
                type: "GET",
                url: "/ProductionOrder/GetArticleInfo/",
                data: { articleId: articleId, producerId: producerId },
                success: function (result) {
                    $('#productionOrderBatchRowEdit #PackHeight').removeAttr("disabled").val(result.PackHeight);
                    $('#productionOrderBatchRowEdit #PackLength').removeAttr("disabled").val(result.PackLength);
                    $('#productionOrderBatchRowEdit #PackWidth').removeAttr("disabled").val(result.PackWidth);
                    $('#productionOrderBatchRowEdit #PackWeight').removeAttr("disabled").val(result.PackWeight);

                    $('#productionOrderBatchRowEdit #PackSize').text(result.PackSize);
                    $('#productionOrderBatchRowEdit #PackVolume').val(result.PackVolume);

                    $('#productionOrderBatchRowEdit #ProductionCost').val("");
                    $('#productionOrderBatchRowEdit #Count').val("");
                    $('#productionOrderBatchRowEdit #PackCount').val("");
                    $('#productionOrderBatchRowEdit #TotalCost').val("");

                    if (result.PackHeight > 0 && result.PackLength > 0 && result.PackWidth > 0) {
                        $("#PackVolume").attr("disabled", "disabled");
                    }
                    else {
                        $("#PackVolume").removeAttr("disabled");
                    }

                    if (result.ProductionCountryId != "") {
                        $('#productionOrderBatchRowEdit #ProductionCountryId').attr('value', result.ProductionCountryId);
                    }

                    $('#productionOrderBatchRowEdit #ManufacturerId').val(result.ManufacturerId);
                    $('#productionOrderBatchRowEdit #ManufacturerName').text(result.ManufacturerName);

                    if (result.ProductionCountryId == "") {
                        $('#productionOrderBatchRowEdit #ProductionCountryId').focus();
                    }
                    else {
                        if (result.ManufacturerId == "") {
                            $('#productionOrderBatchRowEdit #ProductionCountryId').focus();
                        }
                        else {
                            $("#productionOrderBatchRowEdit #ProductionCost").focus();
                        }
                    }

                    $('#ProductionCost').removeAttr("disabled");

                    var packCount = Number($("#PackCount").val());
                    if (!isNaN(packCount)) {
                        ProductionOrder_BatchRowEdit.CalculateMeasures(packCount);
                    }
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowEdit");
                }
            });
        });
    },

    OnFailProductionOrderBatchRowEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderBatchRowEdit");
    },

    OnBeginProductionOrderBatchRowEdit: function () {
        StartButtonProgress($("#btnSaveProductionOrderBatchRow"));
    },

    OnSuccessManufacturerSave: function (ajaxContext) {
        $("#ManufacturerName").html(ajaxContext.Name);
        $("#ManufacturerId").val(ajaxContext.Id);

        HideModal(function () {
            HideModal();
        });
    },

    OnSuccessCountrySave: function (ajaxContext) {
        HideModal();
        $.ajax({
            type: "GET",
            url: "/Country/GetList/",
            success: function (result) {
                $("#ProductionCountryId").fillSelect(result);
                $("#ProductionCountryId").val(ajaxContext.Id);
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchRowEdit");
            }
        });
    }
};