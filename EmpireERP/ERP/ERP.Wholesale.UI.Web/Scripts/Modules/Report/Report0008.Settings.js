var Report0008_Settings = {
    Init: function () {
        $(function () {

            $("#btnBack, #btnBack2").live("click", function () {
                window.location = $('#BackURL').val();
            });

            $("#btnRestoreDefaults, #btnRestoreDefaults2").live("click", function () {
                window.location = window.location;
            });

            //-----------Установка зависимостей комбобоксов-----------------

            //Типом даты зависит от типа накладной
            $('#WaybillTypeId').FillChildComboBox('DateTypeId', "/Report/Report0008_GetWaybillDateTypeList/", 'waybillTypeId', "messageReport0008Settings", false,
            function () {
                $('#DateTypeId').trigger('change'); //а значит если мы изменям тип накладной, то автоматически изменяется тип даты
            });

            $('#WaybillOptionId').FillChildComboBoxWithMoreOption('SortDateTypeId', "/Report/Report0008_GetWaybillSortDateTypeList/", 'waybillOptionId', "messageReport0008Settings",
            function () {
                return "waybillTypeId=" + $("#WaybillTypeId").val();
            },
            function () {
                $('#SortDateTypeId').trigger('change');
            });

            //фильтр статусов накладных зависит от типа даты
            $('#DateTypeId').FillChildComboBoxWithMoreOption('WaybillOptionId', "/Report/Report0008_GetWaybillOptionList/", 'dateTypeId', "messageReport0008Settings",
            //для получения фильтров статусов накладных необходимо знать не только тип накладной, но и тип даты
            function () {
                return "waybillTypeId=" + $("#WaybillTypeId").val();
            },
            function () {
                $('#WaybillOptionId').trigger('change'); //если мы изменям тип даты, то автоматически изменяется фильтр статусов накладных
            });

            //группировка зависит от типа накладной 
            $('#WaybillTypeId').FillChildComboBox('GroupByCollection', "/Report/Report0008_GetWaybillGroupingTypeList/", 'waybillTypeId',
            "messageReport0008Settings", true,
            function () {
                Report0008_Settings.UpdateAfterWaybillTypeChange();
            });

            $("#WaybillOptionId").change(
                function () {
                    if ($('#DateTypeId').val() == 1 && $("#WaybillOptionId").val() != undefined && $("#WaybillOptionId").val() != 0) {
                        $('#PriorToDate').show();
                        $('#PriorToDateLabel').show();
                    }
                    else {
                        $('#PriorToDate').hide();
                        $('#PriorToDateLabel').hide();
                    }
                });

            //Настройка «Исключить расхождения» зависит от типа накладной
            $("#WaybillTypeId").change(
                function () {
                    if ($("#WaybillTypeId").val() == "1") {
                        $("#ExcludeDivergencesSetting").show();
                        EnableYesNoToggle($('#ExcludeDivergences').prev());
                        SetYesNoToggle($("#ExcludeDivergences").prev());
                    }
                    else {
                        ResetYesNoToggle($("#ExcludeDivergences").prev());
                        DisableYesNoToggle($('#ExcludeDivergences').prev());
                        $("#ExcludeDivergencesSetting").hide();
                    }
                });

            //-------------------------------

            $("#btnRender, #btnRender2").live("click", function () {
                if (ValidateReportParameters()) {
                    window.open(CreateActionURLParameters("Report0008"));
                }
            });

            $("#btnExportToExcel, #btnExportToExcel2").live("click", function () {
                if (ValidateReportParameters()) {
                    var url = CreateActionURLParameters("Report0008ExportToExcel");
                    StartButtonProgress($(this));
                    $.fileDownload(url, {
                        successCallback: function (response) {
                            StopButtonProgress();
                            ShowSuccessMessage("Файл успешно сформирован.", "messageReport0008Settings");
                        },
                        failCallback: function (response) {
                            StopButtonProgress();
                            ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0008Settings");
                        }
                    });
                }
            });

            function CreateActionURLParameters(actionName) {
                var Url = "/Report/" + actionName + "?" +
                "StartDate=" + $("#StartDate").val() +
                "&EndDate=" + $("#EndDate").val() +
                "&PriorToDate=" + $("#PriorToDate").val() +
                "&DateTypeId=" + $("#DateTypeId").val() +
                "&WaybillTypeId=" + $("#WaybillTypeId").val() +
                "&ExcludeDivergences=" + $("#ExcludeDivergences").val() +
                "&WaybillOptionId=" + $("#WaybillOptionId").val() +
                "&SortDateTypeId=" + $("#SortDateTypeId").val() +
                "&GroupByCollectionIDs=" + $("#GroupByCollectionIDs").val() +
                "&ShowAdditionInfo=" + $("#ShowAdditionInfo").val() +
                "&" + $("#multipleSelectorStorage").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs") +
                "&" + $("#multipleSelectorCurator").FormSelectedEntitiesUrlParametersString("AllCurators", "CuratorIDs");

                // Если выбрана накладная реализации или возврата, то добавляем клиентов
                if ($("#WaybillTypeId").val() == "4" || $("#WaybillTypeId").val() == "6") {
                    Url = Url + "&" + $("#multipleSelectorClient").FormSelectedEntitiesUrlParametersString("AllClients", "ClientIDs");
                }

                // Если выбрана приходная накладная, то добавляем поставщиков
                if ($("#WaybillTypeId").val() == "1") {
                    Url = Url + "&" + $("#multipleSelectorProvider").FormSelectedEntitiesUrlParametersString("AllProviders", "ProviderIDs");
                }

                return Url;
            }

            function ValidateReportParameters() {
                if (!Report0006_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0008Settings", true)) {
                    return false;
                }

                //проверяем "До даты" только если она отображается
                if ($('#DateTypeId').val() == 1 &&
                   $("#WaybillOptionId").val() != undefined &&
                   $("#WaybillOptionId").val() != 0 &&
                   !Report0008_Settings.ValidatePriorToDate($("#PriorToDate").val(), $("#EndDate").val(), $("#StartDate").val(),
                        "messageReport0008Settings", true)) {
                    return false;
                }

                if ($("#WaybillTypeId").val() == "") {
                    scroll(0, 205);
                    ShowErrorMessage("Необходимо указать тип выводимых накладных.", "messageReport0008Settings");

                    return false;
                }

                if ($("#DateTypeId").val() == "") {
                    scroll(0, 205);
                    ShowErrorMessage("Необходимо указать тип даты.", "messageReport0008Settings");

                    return false;
                }

                if ($("#WaybillOptionId").val() == "") {
                    scroll(0, 205);
                    ShowErrorMessage("Необходимо указать статусы выводимых накладных.", "messageReport0008Settings");

                    return false;
                }

                if ($("#SortDateTypeId").val() == "") {
                    scroll(0, 205);
                    ShowErrorMessage("Необходимо указать дату для сортировки выводимых накладных.", "messageReport0008Settings");

                    return false;
                }

                if (IsFalse($("#multipleSelectorStorage").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
                "Выберите все места хранения или не больше ", "messageReport0008Settings"))) {
                    scroll(0, 205);
                    return false;
                }

                if (IsFalse($("#multipleSelectorCurator").CheckSelectedEntitiesCount("Не выбрано ни одного куратора накладных.",
                "Выберите всех кураторов или не больше ", "messageReport0008Settings"))) {
                    scroll(0, 205);
                    return false;
                }

                // Если выбрана накладная реализации или возврата, то добавляем клиентов
                if ($("#WaybillTypeId").val() == "4" || $("#WaybillTypeId").val() == "6") {
                    if (IsFalse($("#multipleSelectorClient").CheckSelectedEntitiesCount("Не выбрано ни одного клиента.",
                "Выберите всех клиентов или не больше ", "messageReport0008Settings"))) {
                        scroll(0, 205);
                        return false;
                    }
                }

                // Если выбрана приходная накладная, то добавляем поставщиков
                if ($("#WaybillTypeId").val() == "1") {
                    if (IsFalse($("#multipleSelectorProvider").CheckSelectedEntitiesCount("Не выбрано ни одного поставщика.",
                "Выберите всех поставщиков или не больше ", "messageReport0008Settings"))) {
                        scroll(0, 205);
                        return false;
                    }
                }

                return true;
            }

            $("#btnAddGroupBy").live("click", function () {
                var curElem = $('#GroupByCollection option:selected');
                var curVal = curElem.text();
                var curId = $('#GroupByCollection').val();
                if (IsNumber(curId)) {

                    if (curId >= 0) {
                        curElem.remove();
                        var genLink = '<span class="link" id="btnDelGroupBy" delId="' + curId + '">Убрать</span>';
                        $("#tblGroupBy tbody").append('<tr id="' + curId + '"><td>' + genLink + '</td><td>' + curVal + '</td></tr>');
                    }
                    //Если в накладной перемещения выбираем группировку по одному из типов МХ, то другой удаляем
                    if ($("#WaybillTypeId").val() == 2) {
                        if (curId == 2) {
                            RemoveOption(3);
                        }
                        if (curId == 3) {
                            RemoveOption(2);
                        }
                    }
                    Report0008_Settings.UpdateGroupBy();

                }
            });

            $("#btnDelGroupBy").live("click", function () {
                var id = $(this).attr('delId');
                var text = $(this).parent().next().html();
                $(this).parent().parent().remove();
                $('#GroupByCollection').append('<option value="' + id + '">' + text + '</option>');
                //Если в накладной перемещения удаляем группировку по одному из типов МХ, то востанавливаем возможность выбора другого типа МХ
                if ($("#WaybillTypeId").val() == 2) {
                    if (id == 2) {
                        RestoreOption(3);
                    }
                    if (id == 3) {
                        RestoreOption(2);
                    }
                }
                Report0008_Settings.UpdateGroupBy();
            });

            //Поместить тег option в корзину
            function RemoveOption(id) {
                $('#basket').append($('#GroupByCollection [value = ' + id + ']'));
            }

            //Востановить тег option
            function RestoreOption(id) {
                $('#GroupByCollection').append($('#basket [value = ' + id + ']'));
            }

        });
    },

    UpdateAfterWaybillTypeChange: function () {
        var _this = $("#WaybillTypeId");

        Report0008_Settings.ClearGroupBy();

        //В случае выбора накладной реализации или накладной возврата загружаем список клиентов
        if (_this.val() == "4" || _this.val() == "6") {   // Накладная реализации или возврата 
            if ($("#clientSelectorContainer").html() == "") {

                StartLinkProgress($('#clientSelectorProgress'));

                // Подгружаем контент для выбора клиентов
                $.ajax({
                    type: "GET",
                    url: "/Report/Report0008_GetClientSelector/",
                    success: function (result) {
                        $('#clientSelectorWrapper').show();
                        $("#clientSelectorContainer").html(result);
                        StopLinkProgress();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0008Settings");
                    }
                });
            }
            else {
                $("#clientSelectorWrapper").show();
            }
        }
        else {
            $("#clientSelectorWrapper").hide();
        }

        //В случае выбора приходной накладной загружаем поставщиков
        if (_this.val() == "1") {   // Приходная накладная 
            if ($("#providerSelectorContainer").html() == "") {

                StartLinkProgress($('#providerSelectorProgress'));

                // Подгружаем контент для выбора поставщиков
                $.ajax({
                    type: "GET",
                    url: "/Report/Report0008_GetProviderSelector/",
                    success: function (result) {
                        $('#providerSelectorWrapper').show();
                        $("#providerSelectorContainer").html(result);
                        StopLinkProgress();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0008Settings");
                    }
                });
            }
            else {
                $("#providerSelectorWrapper").show();
            }
        }
        else {
            $("#providerSelectorWrapper").hide();
        }
    },

    //Отчищаем грппировки при смене типа накладной
    ClearGroupBy: function () {
        var elements = $("#tblGroupBy tbody").children();

        for (var i = 1; i < elements.length; ++i) {
            $(elements[i]).remove();
        }
        $('basket').html(''); //отчищаем корзину
        Report0008_Settings.UpdateGroupBy();
    },

    // Обновление строки с кодами группировок
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
    },

    ValidatePriorToDate: function (priorToDate, endDate, startDate, messageId, performScrolling) {
        var priorToDateObj = stringToDate(priorToDate);
        var endDateObj = stringToDate(endDate);
        var startDateObj = stringToDate(startDate);

        if (!isValidDate(priorToDateObj)) {
            if (performScrolling) {
                scroll(0, 205);
            }
            ShowErrorMessage("Неверная дата в поле «До даты».", messageId);

            return false;
        }

        if (priorToDateObj < endDateObj) {
            if (performScrolling) {
                scroll(0, 205);
            }
            ShowErrorMessage("Дата в поле «До даты» не может быть меньше даты окончания периода.", messageId);

            return false;
        }

        return true;
    }


};

//Функция основана на функции FillChildComboBox из Common.js. Перенесена сюда в свзязи с специфичными изменениями, которые могут порушить
//обратную совместимость 
//связывание комбобоксов
// источник - родительский комбобокс
// childId - id дочернего
// methodPath - путь к методу контроллера, который принимает выбранный элемент родительского и возвращает список значений для дочернего
// parameterName - имя параметра, передаваемого в метод контроллера
// errorMessageId - id элемента, в который писать сообщение об ошибке
// noEmptyOption - не добавлять пустой элемент
// moreOptions - дополнительные параметры строкой для GET-запроса вида paramName1=param1&paramName2=param2
$.fn.FillChildComboBoxWithMoreOption = function (childId, methodPath, parameterName, errorMessageId, moreOptionsCallback, callback) {
    var parentId = this.attr('id');
    var parentComboBox = this;
    var childComboBox = $('#' + childId);

    this.bind("keyup change", function () {
        childComboBox.attr('disabled', 'disabled');

        var moreOptions = "";
        if (moreOptionsCallback != undefined)
            moreOptions = "&" + moreOptionsCallback();

        var selectedId = parentComboBox.val();
        if (selectedId == "" || selectedId == null) {
            childComboBox.clearSelect();
            if (callback !== undefined)
                callback();
        }
        else {
            StartComboBoxProgress($("#" + childId));

            $.ajax({
                type: "GET",
                url: methodPath,
                data: parameterName + '=' + selectedId + moreOptions,
                success: function (result) {
                    if (result != 0) {

                        var noEmptyOption = false;
                        if (result.SelectedOption != "")
                            noEmptyOption = true;

                        childComboBox.fillSelect(result, noEmptyOption);
                        childComboBox.removeAttr('disabled').removeClass('input-validation-error');
                        $('#' + childId + '_validationMessage').addClass('field-validation-valid').removeClass('field-validation-error');
                        StopComboBoxProgress($("#" + childId));
                        if (callback !== undefined)
                            callback();
                    }
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, errorMessageId);
                    StopComboBoxProgress($("#" + childId));
                    if (callback !== undefined)
                        callback();
                }
            });
        }
    });
}
