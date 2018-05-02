var Report0009_Settings = {
    Init: function () {
        $(function () {
            //Зависимости между настройками

            $("#ShowDetailsTable, #ShowDetailReceiptWaybillRowsWithDivergencesTable").prev().bind('change', function () {
                if (($("#ShowDetailsTable").val() == "1") || ($("#ShowDetailReceiptWaybillRowsWithDivergencesTable").val() == "1")) {
                    EnableYesNoToggle($("#InCurrentAccountingPrice").prev());
                    EnableYesNoToggle($("#ShowBatch").prev());
                    EnableYesNoToggle($("#ShowCountArticleInPack").prev());
                }
                else {
                    ResetYesNoToggle($("#InCurrentAccountingPrice").prev());
                    DisableYesNoToggle($("#InCurrentAccountingPrice").prev());

                    ResetYesNoToggle($("#ShowBatch").prev());
                    DisableYesNoToggle($("#ShowBatch").prev());

                    ResetYesNoToggle($("#ShowCountArticleInPack").prev());
                    DisableYesNoToggle($("#ShowCountArticleInPack").prev());
                }
            });

            $("#DateTypeId").bind('change', function () {
                if ($("#DateTypeId").val() == "3" || $("#DateTypeId").val() == "4") {
                    ResetYesNoToggle($("#ShowDetailReceiptWaybillRowsWithDivergencesTable").prev());
                    DisableYesNoToggle($("#ShowDetailReceiptWaybillRowsWithDivergencesTable").prev());
                }
                else {
                    EnableYesNoToggle($("#ShowDetailReceiptWaybillRowsWithDivergencesTable").prev());
                }
            });

            $("#ShowBatch").prev().bind('change', function () {
                if (($("#ShowBatch").val() == "1")) {
                    EnableYesNoToggle($("#InPurchaseCost").prev());
                    EnableYesNoToggle($("#InRecipientWaybillAccountingPrice").prev());
                    EnableYesNoToggle($("#CalculateMarkup").prev());

                    EnableYesNoToggle($("#ShowCountryOfProduction").prev());
                    EnableYesNoToggle($("#ShowManufacturer").prev());
                    EnableYesNoToggle($("#ShowCustomsDeclarationNumber").prev());
                }
                else {
                    ResetYesNoToggle($("#InPurchaseCost").prev());
                    DisableYesNoToggle($("#InPurchaseCost").prev());

                    ResetYesNoToggle($("#InRecipientWaybillAccountingPrice").prev());
                    DisableYesNoToggle($("#InRecipientWaybillAccountingPrice").prev());

                    ResetYesNoToggle($("#CalculateMarkup").prev());
                    DisableYesNoToggle($("#CalculateMarkup").prev());

                    ResetYesNoToggle($("#ShowCountryOfProduction").prev());
                    DisableYesNoToggle($("#ShowCountryOfProduction").prev());

                    ResetYesNoToggle($("#ShowManufacturer").prev());
                    DisableYesNoToggle($("#ShowManufacturer").prev());

                    ResetYesNoToggle($("#ShowCustomsDeclarationNumber").prev());
                    DisableYesNoToggle($("#ShowCustomsDeclarationNumber").prev());
                }
            });

        });

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
                    var genLink = '<span class="link" id="btnDellGroupBy" delId="' + curId + '">Убрать</span>';
                    $("#tblGroupBy tbody").append('<tr id="' + curId + '"><td style="width:40px;">' + genLink + '</td><td>' + curVal + '</td></tr>');
                    Report0009_Settings.UpdateGroupBy();
                }
            }
        });

        $("#btnDellGroupBy").live("click", function () {
            var id = $(this).attr('delId');
            var text = $(this).parent().next().html();
            $(this).parent().parent().remove();
            $('#GroupByCollection').append('<option value="' + id + '">' + text + '</option>');
            Report0009_Settings.UpdateGroupBy();
        });

        $('#btnRender, #btnRender2').live('click', function () {
            if (ValidateReportParameters()) {
                window.open(CreateActionURLParameters("Report0009"));
            }
        });

        $("#btnExportToExcel, #btnExportToExcel2").live("click", function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0009ExportToExcel");
                StartButtonProgress($(this));
                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0009Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0009Settings");
                    }
                });
            }
        });

        function CreateActionURLParameters(actionName) {
            var Url = "/Report/" + actionName + "?" +
                            $("#multipleSelectorStorages").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs") + "&" +
                            $("#multipleSelectorArticleGroups").FormSelectedEntitiesUrlParametersString("AllArticleGroups", "ArticleGroupsIDs") + "&" +
                            $("#multipleSelectorProviders").FormSelectedEntitiesUrlParametersString("AllProviders", "ProvidersIDs") + "&" +
                            $("#multipleSelectorUser").FormSelectedEntitiesUrlParametersString("AllUsers", "UsersIDs") +

                            "&StartDate=" + $("#StartDate").val() +
                            "&EndDate=" + $("#EndDate").val() +
                            "&DateTypeId=" + $("#DateTypeId").val() +
                            "&GroupByCollectionIDs=" + $("#GroupByCollectionIDs").val() +

                            "&ShowDetailsTable=" + $("#ShowDetailsTable").val() +
                            "&ShowDetailReceiptWaybillRowsWithDivergencesTable=" + $("#ShowDetailReceiptWaybillRowsWithDivergencesTable").val() +
                            "&ShowStorageTable=" + $("#ShowStorageTable").val() +
                            "&ShowOrganizationTable=" + $("#ShowOrganizationTable").val() +
                            "&ShowArticleGroupTable=" + $("#ShowArticleGroupTable").val() +
                            "&ShowProviderTable=" + $("#ShowProviderTable").val() +
                            "&ShowProviderOrganizationTable=" + $("#ShowProviderOrganizationTable").val() +
                            "&ShowUserTable=" + $("#ShowUserTable").val() +

                            "&InPurchaseCost=" + $("#InPurchaseCost").val() +
                            "&InRecipientWaybillAccountingPrice=" + $("#InRecipientWaybillAccountingPrice").val() +
                            "&InCurrentAccountingPrice=" + $("#InCurrentAccountingPrice").val() +

                            "&ShowBatch=" + $("#ShowBatch").val() +
                            "&ShowCountArticleInPack=" + $("#ShowCountArticleInPack").val() +
                            "&ShowCountryOfProduction=" + $("#ShowCountryOfProduction").val() +
                            "&ShowManufacturer=" + $("#ShowManufacturer").val() +
                            "&ShowCustomsDeclarationNumber=" + $("#ShowCustomsDeclarationNumber").val() +
                            "&CalculateMarkup=" + $("#CalculateMarkup").val();
            return Url;
        }

        function ValidateReportParameters() {
            if (Report0009_Settings.IsHideAllTable()) {
                scroll(0, 205);
                ShowErrorMessage("Не выбрано ни одной таблицы.", "messageReport0009Settings");
                return false;
            }

            if ($("#DateTypeId").val() == "") {
                scroll(0, 205);
                ShowErrorMessage("Выберите тип даты, которая должна попадать в отчет.", "messageReport0009Settings");
                return false;
            }

            if (($("#DateTypeId").val() == "3" || $("#DateTypeId").val() == "4") && $("#ShowDetailReceiptWaybillRowsWithDivergencesTable").val() == "1") {
                scroll(0, 205);
                ShowErrorMessage("Параметр «Выводить развернутую таблицу с расхождениями» не может быть равен «Да» при выбранном типе даты.", "messageReport0009Settings");
                return false;
            }

            if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
            "Выберите все места хранения или не больше ", "messageReport0009Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (IsFalse($("#multipleSelectorArticleGroups").CheckSelectedEntitiesCount("Не выбрано ни одной группы товаров.",
            "Выберите все группы товаров или не больше ", "messageReport0009Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (IsFalse($("#multipleSelectorProviders").CheckSelectedEntitiesCount("Не выбрано ни одного поставщика.",
            "Выберите всех поставщиков или не больше ", "messageReport0009Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (IsFalse($("#multipleSelectorUser").CheckSelectedEntitiesCount("Не выбрано ни одного пользователя.",
            "Выберите всех пользователей или не больше ", "messageReport0009Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (!Report0009_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0009Settings")) {
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

        var isListEmpty = ($('#GroupByCollection').children().length == 0);
        UpdateButtonAvailability("GroupByCollection", !isListEmpty);
        UpdateElementVisibility("btnAddGroupBy", !isListEmpty);
    }
};