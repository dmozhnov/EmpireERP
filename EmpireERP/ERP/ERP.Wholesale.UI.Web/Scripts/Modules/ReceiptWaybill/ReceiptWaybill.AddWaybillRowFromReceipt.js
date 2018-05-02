var ReceiptWaybill_AddWaybillRowFromReceipt = {
    Init: function () {
        $(document).ready(function () {
            SetFieldScale("#ReceiptedCount", 12, 0, "#receiptArticleAdd", false);
            SetFieldScale("#ProviderCount", 12, 0, "#receiptArticleAdd", true);
        });

        // Переход к выбору товара
        $("span#ArticleName.select_link").bind("click", function () {
            $.ajax({
                type: "GET",
                url: "/Article/SelectArticle",
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    ReceiptWaybill_AddWaybillRowFromReceipt.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowAdd");
                }
            });
        });

        $("#ManufacturerName.select_link").click(function () {
            $.ajax({
                type: "GET",
                url: "/Manufacturer/SelectManufacturer/",
                success: function (result) {
                    $('#manufacturerAdd').hide().html(result);
                    $.validator.unobtrusive.parse($("#manufacturerAdd"));
                    ShowModal("manufacturerAdd");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowAdd");
                }
            });
        });

        // Обрабатываем выбор фабрики-изготовителя
        $("#manufacturerAdd .select").live("click", function () {
            var manufacturerId = $(this).parent("td").parent("tr").find(".Id").html();
            var manufacturerName = $(this).parent("td").parent("tr").find(".ManufacturerName").html();

            $("#ManufacturerName").html(manufacturerName);
            $("#ManufacturerId").val(manufacturerId);

            HideModal();
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
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowAdd");
                }
            });
        });
    },

    // При успешной попытке добавления строки в накладную
    OnSuccessReceiptWaybillRowAdd: function () {
        ReceiptWaybill_Receipt.SuccessAddedRowToWaybill();
    },

    // При ошибке добавления строки в накладную
    OnFailReceiptWaybillRowAdd: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageReceiptWaybillRowAdd");
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
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowAdd");
            }
        });
    },

    OnSuccessManufacturerSave: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#receiptArticleAdd #ManufacturerId").val(ajaxContext.Id);
                $("#receiptArticleAdd #ManufacturerName").text(ajaxContext.Name);
            })
        });
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live("click", function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            $("#ArticleId").val($(this).parent("td").parent("tr").find(".articleId").text());
            $('#measureUnitNameOfReceiptedCount').text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());
            $('#measureUnitNameOfProviderCount').text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());
            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            SetFieldScale("#ReceiptedCount", 12, measureUnitScale, "#receiptArticleAdd", false);
            SetFieldScale("#ProviderCount", 12, measureUnitScale, "#receiptArticleAdd", true);

            $.ajax({
                type: "POST",
                url: "/ReceiptWaybill/GetArticleInfo/",
                data: { articleId: $("#ArticleId").val() },
                success: function (result) {
                    $('#ProductionCountryId').attr('value', result.ProductionCountryId);
                    $('#ManufacturerId').val(result.ManufacturerId);
                    $('#ManufacturerName').text(result.ManufacturerName);

                    if (result.ProductionCountryId == "") {
                        $('#ProductionCountryId').focus();
                    }
                    else {
                        if (result.ManufacturerId == "") {
                            $('#ProductionCountryId').focus();
                        }
                        else {
                            $("#ReceiptedCount").focus();
                        }
                    }
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowAdd");
                }
            });

            HideModal();
        });
    }
}; 