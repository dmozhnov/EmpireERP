var Article_Edit = {
    Init: function () {
        $(document).ready(function () {
            $('#IsSalaryPercentFromGroup').bind("modify", function () {
                UpdateButtonAvailability("SalaryPercent", $("#IsSalaryPercentFromGroup").val() != "1");
                if ($("#IsSalaryPercentFromGroup").val() == "1") {
                    $("#SalaryPercent").val($("#SalaryPercentFromGroup").val());
                } else {
                    $("#SalaryPercent").val("");
                    $("#SalaryPercent").focus();
                }
            });

            if ($("#PackHeight").val() > 0 || $("#PackWidth").val() > 0 || $("#PackLength").val() > 0) {
                $("#PackVolume").attr("disabled", "disabled");
            }

            SetFieldScale("#PackSize", 6, $("#MeasureUnitScale").val(), "#articleEdit", true);

            $('#IsSalaryPercentFromGroup').bind('click', function () { Article_Edit.CheckIfSalaryPercentCorrect(); });

            $('#SalaryPercent').live('change', function () { Article_Edit.CheckIfSalaryPercentCorrect(); });

            $('#FullArticleName').change(function () {
                var fullName = $('#FullArticleName').val();
                var articleId = $("#articleEdit #Id").val();
                if ($("#ShortName").val().length == 0) {
                    $("#ShortName").val($(this).val());
                }
            });

            $("#IsCurrentArticleGroupLevelCorrect").val(1);

            // Очистка сертификата товара
            $("#clearCertificate").click(function () {
                if (confirm('Вы уверены?')) {
                    $("#selectCertificate").text("Выбрать сертификат товара");
                    $("#CertificateId").val(0);
                    UpdateElementVisibility("clearCertificate", false);
                }
            });

            $("#AddCountry").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Country/Create/",
                    success: function (result) {
                        $("#countryAdd").hide().html(result);
                        $.validator.unobtrusive.parse($("#countryAdd"));
                        ShowModal("countryAdd");
                        $("#countryAdd #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleEdit");
                    }
                });
            });
        });

        // Обработка выбора торговой марки
        $("#trademarkSelector .select_link").live("click", function () {
            var trademarkName = $(this).parent("td").parent("tr").find(".Name").text();
            var trademarkId = $(this).parent("td").parent("tr").find(".Id").text();

            $("#selectTrademark").text(trademarkName);
            $("#TrademarkId").val(trademarkId);

            HideModal();
        });

        // Обработка выбора единицы измерения
        $("#gridMeasureUnit .measureUnit_select_link").live("click", function () {
            var measureUnitName = $(this).parent("td").parent("tr").find(".FullName").text();
            var measureUnitShortName = $(this).parent("td").parent("tr").find(".ShortName").text();
            var measureUnitId = $(this).parent("td").parent("tr").find(".Id").text();

            var measureUnitScale = $(this).parent("td").parent("tr").find(".Scale").text();
            SetFieldScale("#PackSize", 6, measureUnitScale, "#articleEdit", true);

            $("#MeasureUnitShortName").text(measureUnitShortName);
            $("#selectMeasureUnit").text(measureUnitName);
            $("#MeasureUnitId").val(measureUnitId);

            HideModal();
        });

        // Обработка выбора сертификата товара
        $("#gridArticleCertificate .articleCertificate_select_link").live("click", function () {
            var articleCertificateName = $(this).parent("td").parent("tr").find(".Name").text();
            var articleCertificateId = $(this).parent("td").parent("tr").find(".Id").text();

            $("#selectCertificate").text(articleCertificateName);
            $("#CertificateId").val(articleCertificateId);
            UpdateElementVisibility("clearCertificate", true);

            HideModal();
        });

        $("#ArticleGroupName").click(function () {
            $.ajax({
                type: "GET",
                url: "/ArticleGroup/SelectArticleGroup/",
                success: function (result) {
                    $('#articleGroupSelector').hide().html(result);
                    ShowModal("articleGroupSelector");
                    $("#articleGroupSelector").css("top", "50px");

                    Article_Edit.BindArticleGroupSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleEdit");
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
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReceiptWaybillRowEdit");
                }
            });
        });

        // Открытие окна выбора торговой марки
        $("#selectTrademark.select_link").click(function () {
            $.ajax({
                type: "GET",
                url: "/Trademark/SelectTrademark/",
                success: function (result) {
                    $('#trademarkSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#trademarkSelector"));
                    ShowModal("trademarkSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleEdit");
                }
            });
        });

        // Открытие окна выбора единицы измерения
        $("#selectMeasureUnit.select_link").click(function () {
            $.ajax({
                type: "GET",
                url: "/MeasureUnit/SelectMeasureUnit/",
                success: function (result) {
                    $('#measureUnitSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#measureUnitSelector"));
                    ShowModal("measureUnitSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleEdit");
                }
            });
        });

        // Открытие окна выбора сертификата товара
        $("#selectCertificate.select_link").click(function () {
            $.ajax({
                type: "GET",
                url: "/ArticleCertificate/SelectArticleCertificate/",
                success: function (result) {
                    $('#articleCertificateSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleCertificateSelector"));
                    ShowModal("articleCertificateSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleEdit");
                }
            });
        });

        $("#PackHeight, #PackLength, #PackWidth").live("change", function () {
            var height = parseFloat($("#PackHeight").val());
            var width = parseFloat($("#PackWidth").val());
            var length = parseFloat($("#PackLength").val());

            if ((isNaN(height) || height == 0) || (isNaN(width) || width == 0) || (isNaN(length) || length == 0)) {
                $("#PackVolume").removeAttr("disabled");
            }
            else {
                $("#PackVolume").attr("disabled", "disabled");
                var volume = height * width * length / 1000000000;

                if (volume < 0.000001) volume = 0; //если объем меньше 0.000001, то значение volume становится 1е-7 и ValueForDisplay на нем неверно срабатывает

                if (!isNaN(volume)) {
                    $("#PackVolume").val(ValueForEdit(volume));
                }
            }
        });

        //Обрабатываем выбор фабрики-изготовителя
        $("#manufacturerAdd .select").live("click", function () {
            var manufacturerId = $(this).parent("td").parent("tr").find(".Id").text();
            var manufactureName = $(this).parent("td").parent("tr").find(".ManufacturerName").text();

            $("#ManufacturerName").text(manufactureName);
            $("#ManufacturerId").val(manufacturerId);

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
                ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleEdit");
            }
        });
    },

    // обработка создания торговой марки
    OnSuccessTrademarkSave: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#selectTrademark").text(ajaxContext.Name);
                $("#TrademarkId").val(ajaxContext.Id);
            });
        });
    },

    // обработка создания единицы измерения
    OnSuccessMeasureUnitSave: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#selectMeasureUnit").text(ajaxContext.Name);

                SetFieldScale("#PackSize", 12, ajaxContext.Scale, "#articleEdit", true);

                $("#MeasureUnitId").val(ajaxContext.Id);
            });
        });
    },

    // обработка создания сертификата товара
    OnSuccessArticleCertificateSave: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#selectCertificate").text(ajaxContext.Name);
                $("#CertificateId").val(ajaxContext.Id);
                UpdateElementVisibility("clearCertificate", true);
            });
        });
    },

    OnFailArticleSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageArticleEdit");
    },

    CheckIfSalaryPercentCorrect: function () {
        var regexp = /^[0-9]{1,2}([,.][0-9]{1,2})?$/;

        if ($("#IsSalaryPercentFromGroup").val() != "1") {
            var correctValue = (regexp.test($('#SalaryPercent').val()));
            if (!correctValue) { $('#isSalaryPercentCorrect').val(1); }
            else { $('#isSalaryPercentCorrect').val(0); }
        }
        else { $('#isSalaryPercentCorrect').val(0); }
    },

    BindArticleGroupSelection: function () {
        $(".tree_node_title").bind('click', function () {
            var articleGroupId = $(this).next("input.value").val();
            var level = $(this).closest(".tree_node").attr("level");

            // товар можно добавить только в группы 2 уровня
            if (level > 1) {
                $("#ArticleGroupId").val(articleGroupId);
                $("#ArticleGroupName").text($(this).text());
                HideModal();

                $.ajax({
                    type: "GET",
                    url: "/ArticleGroup/GetArticleGroupInfo/",
                    data: { id: articleGroupId },
                    success: function (result) {
                        $("#SalaryPercentFromGroup").val(result.SalaryPercent);
                        if ($("#IsSalaryPercentFromGroup").val() == "1") {
                            $("#SalaryPercent").val(result.SalaryPercent);
                        }
                        if ($("#articleEdit #Id").val() == "0") {
                            $("#MarkupPercent").val(result.MarkupPercent);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleEdit");
                    }
                });
            }
        });
    }
};