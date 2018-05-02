var Report0004_Settings = {
    Init: function () {

        $("#btnBack, #btnBack2").live("click", function () {
            window.location = $('#BackURL').val();
        });

        $("#btnRestoreDefaults, #btnRestoreDefaults2").live("click", function () {
            window.location = window.location;
        });

        $('#btnRender, #btnRender2').live('click', function () {
            if (ValidateReportParameters()) {
                window.open(CreateActionURLParameters("Report0004"));
            }
        });

        $('#btnExportToExcel, #btnExportToExcel2').live('click', function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0004ExportToExcel");
                StartButtonProgress($(this));
                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0004Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0004Settings");
                    }
                });
            }
        });

        function CreateActionURLParameters(actionName) {
            var Url = "/Report/" + actionName + "?" +
                            $("#multipleSelectorStorages").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs") +
                            "&ArticleId=" + $("#ArticleId").val() +
                            "&ShowStartQuantityByStorage=" + $("#ShowStartQuantityByStorage").val() +
                            "&ShowStartQuantityByOrganization=" + $("#ShowStartQuantityByOrganization").val() +
                            "&ShowEndQuantityByStorage=" + $("#ShowEndQuantityByStorage").val() +
                            "&ShowEndQuantityByOrganization=" + $("#ShowEndQuantityByOrganization").val() +
                            "&ShowBatches=" + $("#ShowBatches").val() +
                            "&ShowPurchaseCosts=" + $("#ShowPurchaseCosts").val() +
                            "&ShowRecipientAccountingPrices=" + $("#ShowRecipientAccountingPrices").val() +
                            "&ShowSenderAccountingPrices=" + $("#ShowSenderAccountingPrices").val() +
                            "&ShowOnlyExactAvailability=" + $("#ShowOnlyExactAvailability").val() +
                            "&StartDate=" + $("#StartDate").val() +
                            "&EndDate=" + $("#EndDate").val();
            return Url;
        }

        function ValidateReportParameters() {
            if ($('#ArticleId').val() == '0') {
                scroll(0, 205);
                $('#ArticleId').ValidationError("Не выбран товар");
                return false;
            }

            if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
            "Выберите все места хранения или не больше ", "messageReport0004Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (!Report0004_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0004Settings")) {
                return false;
            }
            return true;
        }

        // открытие формы выбора товара
        $("span#ArticleName").live("click", function () {
            $.ajax({
                type: "GET",
                url: "/Article/SelectArticle",
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    Report0004_Settings.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0004Settings");
                }
            });
        });
    },

    ValidateDate: function (startDate, endDate, idMessage) {
        // проверка дат
        var starDateObj = stringToDate(startDate);
        var endDateObj = stringToDate(endDate);

        if (!isValidDate(starDateObj)) {
            scroll(0, 205);
            ShowErrorMessage("Неверная дата начала.", idMessage);

            return false;
        }

        if (!isValidDate(endDateObj)) {
            scroll(0, 205);
            ShowErrorMessage("Неверная дата конца.", idMessage);

            return false;
        }

        if (starDateObj > endDateObj) {
            scroll(0, 205);
            ShowErrorMessage("Дата начала периода для отчета должна быть меньше даты конца.", idMessage);

            return false;
        }

        var today = new Date();
        if (endDateObj > today) {
            scroll(0, 205);
            ShowErrorMessage("Дата окончания периода для отчета должна быть меньше или равна текущей дате.", idMessage);

            return false;
        }

        return true;
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live("click", function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            var articleId = $(this).parent("td").parent("tr").find(".articleId").text();
            $("#ArticleId").val(articleId);
            HideModal();
        });
    }
};