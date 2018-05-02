var Report0002_Settings = {
    Init: function () {
        $(function () {
            $('#tblGroupBy').hide();
            $("#articleGroupNameSelector").hide();

            if ($("#WithReturnFromClient").val() == "0") {
                $(".ReturnFromClientTypeSelector").hide();
            }
            else {
                $(".ReturnFromClientTypeSelector").show();
            }

            //Ищем span от YesNoToggle для развернутой таблицы
            $("#ShowDetailsTable").prev().bind('change', function () {
                if ($("#ShowDetailsTable").val() == "1") {
                    EnableYesNoToggle($("#InAvgPrice").prev());
                    EnableYesNoToggle($("#DevideByBatch").prev());
                    EnableYesNoToggle($("#ShowAdditionColumns").prev());

                    var elements = $("#tblGroupBy tbody").children();
                    //Делаем активным переключатель "Вывод развернутой информации по товарам в сокращенном виде",
                    //если есть хотя бы одна группировка 
                    //Вычитается 1 , потому что в таблице есть строка заголовка
                    if (elements.length - 1 > 0) {
                        EnableYesNoToggle($("#ShowShortDetailsTable").prev());
                    }
                }
                else {
                    ResetYesNoToggle($("#InAvgPrice").prev());
                    DisableYesNoToggle($("#InAvgPrice").prev());
                    ResetYesNoToggle($("#DevideByBatch").prev());
                    DisableYesNoToggle($("#DevideByBatch").prev());
                    ResetYesNoToggle($("#ShowAdditionColumns").prev());
                    DisableYesNoToggle($("#ShowAdditionColumns").prev());
                    ResetYesNoToggle($("#ShowShortDetailsTable").prev());
                    DisableYesNoToggle($("#ShowShortDetailsTable").prev());
                }
            });

            $("#StoragesInColumns").prev().bind('change', function () {
                if ($("#StoragesInColumns").val() == "1") {
                    ResetYesNoToggle($("#ShowStorageTable").prev());
                    DisableYesNoToggle($("#ShowStorageTable").prev());
                    ResetYesNoToggle($("#ShowAccountOrganizationTable").prev());
                    DisableYesNoToggle($("#ShowAccountOrganizationTable").prev());
                    ResetYesNoToggle($("#ShowClientTable").prev());
                    DisableYesNoToggle($("#ShowClientTable").prev());
                    ResetYesNoToggle($("#ShowClientOrganizationTable").prev());
                    DisableYesNoToggle($("#ShowClientOrganizationTable").prev());
                    ResetYesNoToggle($("#ShowArticleGroupTable").prev());
                    DisableYesNoToggle($("#ShowArticleGroupTable").prev());
                    ResetYesNoToggle($("#ShowTeamTable").prev());
                    DisableYesNoToggle($("#ShowTeamTable").prev());
                    ResetYesNoToggle($("#ShowUserTable").prev());
                    DisableYesNoToggle($("#ShowUserTable").prev());
                    ResetYesNoToggle($("#ShowProviderAndProducerTable").prev());
                    DisableYesNoToggle($("#ShowProviderAndProducerTable").prev());
                }
                else {
                    EnableYesNoToggle($("#ShowStorageTable").prev());
                    EnableYesNoToggle($("#ShowAccountOrganizationTable").prev());
                    EnableYesNoToggle($("#ShowClientTable").prev());
                    EnableYesNoToggle($("#ShowClientOrganizationTable").prev());
                    EnableYesNoToggle($("#ShowArticleGroupTable").prev());
                    EnableYesNoToggle($("#ShowTeamTable").prev());
                    EnableYesNoToggle($("#ShowUserTable").prev());
                    EnableYesNoToggle($("#ShowProviderAndProducerTable").prev());
                }
            });

            $("#WithReturnFromClient").prev(".yes_no_toggle").change(function () {
                if ($("#WithReturnFromClient").val() == "0") {
                    $(".ReturnFromClientTypeSelector").hide();
                }
                else {
                    $(".ReturnFromClientTypeSelector").show();
                }
            });

            $("#InPurchaseCost, #InSalePrice").prev().bind('change', function () {
                if ($("#InPurchaseCost").val() == "0" && $("#InSalePrice").val() == "0") {
                    ResetYesNoToggle($("#InAvgPrice").prev());
                    DisableYesNoToggle($("#InAvgPrice").prev());
                }
                else {
                    EnableYesNoToggle($("#InAvgPrice").prev());
                }
            });

            $("#ShowStorageTable, #ShowAccountOrganizationTable, #ShowClientTable, #ShowClientOrganizationTable, #ShowArticleGroupTable, #ShowTeamTable, #ShowUserTable, #ShowProviderAndProducerTable")
            .prev().bind('change', function () {
                if ($("#ShowStorageTable").val() == "0" && $("#ShowAccountOrganizationTable").val() == "0" && $("#ShowClientTable").val() == "0" &&
                    $("#ShowClientOrganizationTable").val() == "0" && $("#ShowArticleGroupTable").val() == "0" && $("#ShowProviderAndProducerTable").val() == "0" &&
                    $("#ShowTeamTable").val() == "0" && $("#ShowUserTable").val() == "0") {
                    ResetYesNoToggle($("#ShowSoldArticleCount").prev());
                    DisableYesNoToggle($("#ShowSoldArticleCount").prev());
                }
                else {
                    EnableYesNoToggle($("#ShowSoldArticleCount").prev());
                }
            });

            $("#InAccountingPrice").prev(".yes_no_toggle").change(function () {
                var _this = this;
                StartLinkProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Report/Report0002StorageSelector/",
                    data: { inAccountingPrice: $("#InAccountingPrice").val() },
                    success: function (result) {
                        $("#storageSelector").html(result);
                        StopLinkProgress($(_this));
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0002Settings");
                        StopLinkProgress($(_this));
                    }
                });
            });
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


        $("#btnBack, #btnBack2").live("click", function () {
            window.location = $('#BackURL').val();
        });

        $("#btnRestoreDefaults, #btnRestoreDefaults2").live("click", function () {
            window.location = window.location;
        });

        $("#btnAddGroupBy").live("click", function () {
            var curElem = $('#GroupByCollection  option:selected');
            var curVal = curElem.text();
            var curId = $('#GroupByCollection').val();
            if (IsNumber(curId)) {
                if (curId >= 0) {
                    curElem.remove();
                    var genLink = '<span class="link" id="btnDelGroupBy" delId="' + curId + '">Убрать</span>';
                    $("#tblGroupBy tbody").append('<tr id="' + curId + '"><td style="width:40px;">' + genLink + '</td><td>' + curVal + '</td></tr>');
                    Report0002_Settings.UpdateGroupBy();
                }
            }

            //Делаем активным переключатель "Вывод развернутой информации по товарам в сокращенном виде",
            //если переключатель "Выводить развернутую таблицу" активен
            if ($("#ShowDetailsTable").val() == "1") {
                EnableYesNoToggle($("#ShowShortDetailsTable").prev());
            }

        });

        $("#btnDelGroupBy").live("click", function () {
            var id = $(this).attr('delId');
            var text = $(this).parent().next().html();
            $(this).parent().parent().remove();
            $('#GroupByCollection').append('<option value="' + id + '">' + text + '</option>');
            Report0002_Settings.UpdateGroupBy();

            var elements = $("#tblGroupBy tbody").children();

            //Если из группировки удаляется последний элемент, то сбрасывать переключатель
            //"Вывод развернутой информации по товарам в сокращенном виде" 
            //Вычитается 1 , потому что в таблице есть строка заголовка
            if (elements.length - 1 == 0) {
                ResetYesNoToggle($("#ShowShortDetailsTable").prev());
                DisableYesNoToggle($("#ShowShortDetailsTable").prev());
            }
        });

        $('#btnRender, #btnRender2').live('click', function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0002");
                window.open(url);
            }
        });

        $('#btnExportToExcel, #btnExportToExcel2').live('click', function () {
            if (ValidateReportParameters()) {
                StartButtonProgress($(this));
                var url = CreateActionURLParameters("Report0002ExportToExcel");

                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0002Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0002Settings");
                    }
                });
            }
        });



        function CreateActionURLParameters(actionName) {
            var url = "/Report/" + actionName + "?" +
                $("#multipleSelectorStorages").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs");
            if ($("#CreateByArticleGroup").val() == "1") {
                url = url + "&" + $("#multipleSelectorArticleGroups").FormSelectedEntitiesUrlParametersString("AllArticleGroups", "ArticleGroupsIDs");
            }
            else {
                url = url + "&" + $("#multipleSelectorArticles").FormSelectedEntityIDsUrlParametersString("ArticlesIDs");
            }

            url = url + "&" + $("#multipleSelectorClient").FormSelectedEntitiesUrlParametersString("AllClients", "ClientsIDs") + "&" +
                $("#multipleSelectorUser").FormSelectedEntitiesUrlParametersString("AllUsers", "UsersIDs") + "&" +
                $("#multipleSelectorAccountOrganization").FormSelectedEntitiesUrlParametersString("AllAccountOrganizations", "AccountOrganizationsIDs") +

                "&GroupByCollectionIDs=" + $("#GroupByCollectionIDs").val() +
                "&StartDate=" + $("#StartDate").val() +
                "&EndDate=" + $("#EndDate").val() +
                "&StoragesInColumns=" + $("#StoragesInColumns").val() +
                "&DevideByBatch=" + $("#DevideByBatch").val() +
                "&CalculateMarkup=" + $("#CalculateMarkup").val() +
                "&WithReturnFromClient=" + $("#WithReturnFromClient").val() +
                "&ShowStorageTable=" + $("#ShowStorageTable").val() +
                "&ShowAccountOrganizationTable=" + $("#ShowAccountOrganizationTable").val() +
                "&ShowClientTable=" + $("#ShowClientTable").val() +
                "&ShowClientOrganizationTable=" + $("#ShowClientOrganizationTable").val() +
                "&ShowArticleGroupTable=" + $("#ShowArticleGroupTable").val() +
                "&ShowTeamTable=" + $("#ShowTeamTable").val() +
                "&ShowUserTable=" + $("#ShowUserTable").val() +
                "&ShowProviderAndProducerTable=" + $("#ShowProviderAndProducerTable").val() +
                "&ShowAdditionColumns=" + $("#ShowAdditionColumns").val() +
                "&InPurchaseCost=" + $("#InPurchaseCost").val() +
                "&InAccountingPrice=" + $("#InAccountingPrice").val() +
                "&InAvgPrice=" + $("#InAvgPrice").val() +
                "&InSalePrice=" + $("#InSalePrice").val() +
                "&WaybillStateId=" + $("input[name=WaybillStateId]:checked").val() +
                "&ShowDetailsTable=" + $("#ShowDetailsTable").val() +
                "&ReturnFromClientType=" + $("input[name=ReturnFromClientType]:checked").val() +
                "&ShowShortDetailsTable=" + $("#ShowShortDetailsTable").val() +
                "&CreateByArticleGroup=" + $("#CreateByArticleGroup").val() +
                "&ShowSoldArticleCount=" + $("#ShowSoldArticleCount").val();

            return url;
        }

        function ValidateReportParameters() {
            if (Report0002_Settings.IsHideAllTable()) {
                scroll(0, 205);
                ShowErrorMessage("Не выбрано ни одной таблицы.", "messageReport0002Settings");
                return false;
            }

            if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
            "Выберите все места хранения или не больше ", "messageReport0002Settings"))) {
                scroll(0, 205);
                return false;
            }

            if ($("#CreateByArticleGroup").val() == "1") {
                if (IsFalse($("#multipleSelectorArticleGroups").CheckSelectedEntitiesCount("Не выбрано ни одной группы товаров.",
            "Выберите все группы товаров или не больше ", "messageReport0002Settings"))) {
                    scroll(0, 205);
                    return false;
                }
            }
            if ($("#CreateByArticleGroup").val() == "0") {
                if ($("#multipleSelectorArticles_selected_values").val() == "") {
                    ShowErrorMessage("Не выбрано ни одного товара.", "messageReport0002Settings");
                    scroll(0, 205);
                    return false;
                }
                var selectedCountField = $("#multipleSelectorArticles_selectedCount");
                var selectedCount = TryGetDecimal(selectedCountField.text());
                var maxSelectedCount = TryGetDecimal($("#multipleSelectorArticles").attr("data-max-selected-count"));
                if (selectedCount > maxSelectedCount) {
                    ShowErrorMessage("Выберите не больше " + maxSelectedCount + " товаров.", "messageReport0002Settings");
                    scroll(0, 205);
                    return false;
                }
            }

            if (IsFalse($("#multipleSelectorClient").CheckSelectedEntitiesCount("Не выбрано ни одного клиента.",
            "Выберите всех клиентов или не больше ", "messageReport0002Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (IsFalse($("#multipleSelectorUser").CheckSelectedEntitiesCount("Не выбрано ни одного пользователя.",
            "Выберите всех пользователей или не больше ", "messageReport0002Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (IsFalse($("#multipleSelectorAccountOrganization").CheckSelectedEntitiesCount("Не выбрано ни одной собственной организации.",
            "Выберите все собственные организации или не больше ", "messageReport0002Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (!Report0002_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0002Settings")) {
                return false;
            }

            return true;
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

    UpdateGroupBy: function () {
        var str = "";
        var elements = $("#tblGroupBy tbody").children();
        for (var i = 1; i < elements.length; ++i) {
            str += $(elements[i]).attr("id");
            if (i < elements.length - 1) str += '_';
        }
        if (str != "")
            $('#tblGroupBy').show();
        else
            $('#tblGroupBy').hide();
        $('#GroupByCollectionIDs').val(str);
    }
};