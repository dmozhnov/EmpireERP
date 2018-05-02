var Report0001_Settings = {
    Init: function () {
        $(function () {

            $("#articleGroupNameSelector").hide();

            $('#DevideByStorages').prev('span').click(function () {
                var valObj = $('#DevideByStorages');
                if (valObj.prev('span').hasClass('yes_no_toggle')) {
                    if (valObj.val() == 1) {
                        Report0001BlockToogleId(true, 'StoragesInRows', '1', 'Да');
                        Report0001BlockToogleId(true, 'ShowAccountingPrices', '0', 'Нет');
                    }
                    else {
                        Report0001BlockToogleId(false, 'StoragesInRows');
                        Report0001BlockToogleId(false, 'ShowAccountingPrices');
                    }
                }
            });

            $('#StoragesInRows').prev('span').click(function () {
                var valObj = $('#StoragesInRows');
                if (valObj.prev('span').hasClass('yes_no_toggle')) {
                    if (valObj.val() == 1) {
                        Report0001BlockToogleId(true, 'DevideByAccountOrganizations', '0', 'Нет');
                        Report0001BlockToogleId(true, 'DevideByStorages', '1', 'Да');
                        Report0001BlockToogleId(false, 'ShowAccountingPrices');
                    }
                    else {
                        Report0001BlockToogleId(false, 'DevideByAccountOrganizations');
                        Report0001BlockToogleId(false, 'DevideByStorages');
                    }
                }
            });

            $('#ShowDetailsTable').prev('span').click(function () {
                var valObj = $('#ShowDetailsTable');
                if (valObj.prev('span').hasClass('yes_no_toggle')) {
                    if (valObj.val() == 0) {
                        Report0001BlockToogleId(true, 'ShowShortDetailsTable', '0', 'Нет');
                    }
                    else {
                        Report0001BlockToogleId(false, 'ShowShortDetailsTable');
                    }
                }
            });

            $('#ShowShortDetailsTable').prev('span').click(function () {
                var valObj = $('#ShowShortDetailsTable');
                if (valObj.prev('span').hasClass('yes_no_toggle')) {
                    if (valObj.val() == 0) {
                        Report0001BlockToogleId(true, 'ShowDetailsTable', '0', 'Нет');
                    }
                    else {
                        Report0001BlockToogleId(false, 'ShowDetailsTable');
                    }
                }
            });

            $(".yes_no_toggle").live("click", function () {
                if ($(this).next("input").attr("name") == "CreateByArticleGroup") {
                    if ($(this).next("input").val() == "1") {
                        $("#articleGroupsSelector").show();
                        $("#articleGroupNameSelector").hide();
                        $("#articlesSelector").hide();
                    } else {
                        $("#articlesSelector").show();
                        $("#articleGroupNameSelector").show();
                        $("#articleGroupsSelector").hide();
                    }
                }
            });

            $("span#ArticleGroupName").live("click", function () {
                $.ajax({
                    type: "GET",
                    url: "/ArticleGroup/SelectArticleGroup/",
                    success: function (result) {
                        $('#articleGroupSelector').hide().html(result);
                        ShowModal("articleGroupSelector");
                        $('#articleGroupSelector .attention').hide(); 
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0001Settings");
                    }
                });
            });

            $("#multipleSelectorArticles .multiple_selector_add_button").live("click", function () {
                CheckSelectedArticleCount();
            });

            $("#multipleSelectorArticles .multiple_selector_item").live("click", function () {
                CheckSelectedArticleCount();
            });

            $("#articleGroupSelector .tree_node_title").live("click", function () {
                var articleGroupId = $(this).next("input.value").val();
                $("#ArticleGroupName").attr("selected_id", articleGroupId);
                $("#ArticleGroupName").text($(this).text());
                $.ajax({
                    type: "GET",
                    url: "/Article/GetArticleFromArticleGroup/",
                    data: { id: articleGroupId },
                    success: function (result) {
                        $('#multipleSelectorArticles_available').find('div.multiple_selector_item').remove();
                        for (keyVar in result) {
                            $('#multipleSelectorArticles_available').append("<div class=\"multiple_selector_item link\" value=\"" + keyVar + "\">" + result[keyVar] + "</div>");
                        }
                        HideModal();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0001Settings");
                    }
                });
            });
        });

        $("#btnBack, #btnBack2").live("click", function () {
            window.location = $('#BackURL').val();
        });

        $("#btnRestoreDefaults, #btnRestoreDefaults2").live("click", function () {
            window.location = window.location;
        });

        $('#btnRender, #btnRender2').live('click', function () {
            if (ValidateReportParameters()) {
                window.open(CreateActionURLParameters("Report0001"));
            }
        });

        $('#btnExportToExcel, #btnExportToExcel2').live('click', function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0001ExportToExcel");
                StartButtonProgress($(this));
                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0001Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0001Settings");
                    }
                });
            }
        });

        function CreateActionURLParameters(actionName) {
            var url = "/Report/" + actionName + "?Date=" + $("#Date").val() + "&DevideByAccountOrganizations=" + $("#DevideByAccountOrganizations").val() + "&" +
                $("#multipleSelectorStorages").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs") +
                "&DevideByStorages=" + $("#DevideByStorages").val() +
                "&StoragesInRows=" + $("#StoragesInRows").val() + "&ShowPurchaseCosts=" + $("#ShowPurchaseCosts").val() +
                "&ShowAveragePurchaseCost=" + $("#ShowAveragePurchaseCost").val() + "&ShowAccountingPrices=" + $("#ShowAccountingPrices").val() +
                "&ShowAverageAccountingPrice=" + $("#ShowAverageAccountingPrice").val() + "&ShowExtendedAvailability=" + $("#ShowExtendedAvailability").val() +
				"&SortTypeId=" + $("#SortTypeId").val() + "&ShowDetailsTable=" + $("#ShowDetailsTable").val() + "&ShowStorageTable=" + $("#ShowStorageTable").val() +
                "&ShowShortDetailsTable=" + $("#ShowShortDetailsTable").val() + "&ShowArticleGroupTable=" + $("#ShowArticleGroupTable").val() + "&ShowAccountOrganizationTable=" +
				$("#ShowAccountOrganizationTable").val() + "&CreateByArticleGroup=" + $("#CreateByArticleGroup").val();
            if ($("#CreateByArticleGroup").val() == "1") {
                url = url + "&" + $("#multipleSelectorArticleGroups").FormSelectedEntitiesUrlParametersString("AllArticleGroups", "ArticleGroupsIDs");
            } else {
                url = url + "&" + $("#multipleSelectorArticles").FormSelectedEntityIDsUrlParametersString("ArticlesIDs");
            }

            return url;
        }

        function ValidateReportParameters() {
            if (Report0001_Settings.IsHideAllTable()) {
                scroll(0, 205);
                ShowErrorMessage("Не выбрано ни одной таблицы.", "messageReport0001Settings");
                return false;
            }

            if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
            "Выберите все места хранения или не больше ", "messageReport0001Settings"))) {
                scroll(0, 205);
                return false;
            }

            if ($("#CreateByArticleGroup").val() == "1") {
                if (IsFalse($("#multipleSelectorArticleGroups").CheckSelectedEntitiesCount("Не выбрано ни одной группы товаров.",
                "Выберите все группы товаров или не больше ", "messageReport0001Settings"))) {
                    scroll(0, 205);
                    return false;
                }
            }

            if ($("#CreateByArticleGroup").val() == "0") {
                if ($("#multipleSelectorArticles_selected_values").val() == "") {
                    ShowErrorMessage("Не выбрано ни одного товара.", "messageReport0001Settings");
                    scroll(0, 205);
                    return false;
                }
                var selectedCountField = $("#multipleSelectorArticles_selectedCount");
                var selectedCount = TryGetDecimal(selectedCountField.text());
                var maxSelectedCount = TryGetDecimal($("#multipleSelectorArticles").attr("data-max-selected-count"));
                if (selectedCount > maxSelectedCount) {
                    ShowErrorMessage("Выберите не больше " + maxSelectedCount + " товаров.", "messageReport0001Settings");
                    scroll(0, 205);
                    return false;
                }
            }

            if (!Report0001_Settings.ValidateDate($("#Date").val(), "messageReport0001Settings")) {
                return false;
            }

            if ($('#StoragesInRows').val() == 0 && $('#ShowDetailsTable').val() == 0 && $('#ShowShortDetailsTable').val() == 0) {
                scroll(0, 205);
                ShowErrorMessage("Необходимо выбрать вывод либо развернутой, либо сокращенной информации по товарам.", "messageReport0001Settings");
                return false;
            }

            return true;
        }

        function Report0001BlockToogleId(isBlock, id, val, html) {
            var Current = $('#' + id);
            var parentCur = Current.prev('span');
            if (val != undefined) {
                Current.val(val);
            }
            if (isBlock) {
                parentCur.removeClass('link yes_no_toggle');
            }
            else {
                if (!parentCur.hasClass('yes_no_toggle')) {
                    parentCur.addClass('link yes_no_toggle');
                }
            }
            if (html != undefined) {
                parentCur.html(html);
            }
        }

        function CheckSelectedArticleCount() {
            var selectedCountField = $("#multipleSelectorArticles_selectedCount");
            var selectedCount = TryGetDecimal(selectedCountField.text());
            var maxSelectedCount = TryGetDecimal($("#multipleSelectorArticles").attr("data-max-selected-count"));
            if (selectedCount > maxSelectedCount) {
                selectedCountField.parent("font").attr("color", "red");
            }
            else {
                selectedCountField.parent("font").attr("color", "grey");
            }
        }
    },

    IsHideAllTable: function () {
        var result = true;
        $(".table_show [type='hidden'][id]").each(function () {
            if ($(this).val() == '1')
                result = false;
        });
        return result;
    },

    ValidateDate: function (startDate, idMessage) {
        // проверка дат
        var starDateObj = stringToDate(startDate);

        if (!isValidDate(starDateObj)) {
            scroll(0, 205);
            ShowErrorMessage("Неверная дата отчета.", idMessage);

            return false;
        }

        var today = new Date();
        if (starDateObj > today) {
            scroll(0, 205);
            ShowErrorMessage("Дата составления отчета должна быть меньше или равна текущей дате.", idMessage);

            return false;
        }

        return true;
    }
};