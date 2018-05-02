var Report0006_Settings = {
    Init: function () {
        $(function () {
            $('#tblGroupBy').hide();
            $('#GroupByCollectionIDs').val("");
            $('#multipleSelectorClient_selected_values').val("");
            $('#multipleSelectorClientOrganization_selected_values').val("");
        });

        $("#btnBack").live("click", function () {
            window.location = $('#BackURL').val();
        });

        $("#btnRestoreDefaults").live("click", function () {
            window.location = window.location;
        });

        $("#btnAddGroupBy").live("click", function () {
            if (IsTrue($("#AllowToAddGrouping").val())) {
                var curElem = $('#GroupByCollection option:selected');
                var curVal = curElem.text();
                var curId = $('#GroupByCollection').val();
                if (IsNumber(curId)) {
                    if (curId >= 0) {
                        curElem.remove();
                        var genLink = '<span class="link" id="btnDelGroupBy" delId="' + curId + '">Убрать</span>';
                        $("#tblGroupBy tbody").append('<tr id="' + curId + '"><td>' + genLink + '</td><td>' + curVal + '</td></tr>');
                        Report0006_Settings.UpdateGroupBy();
                    }
                }
            }
        });

        $("#btnDelGroupBy").live("click", function () {
            var id = $(this).attr('delId');
            var text = $(this).parent().next().html();
            $(this).parent().parent().remove();
            $('#GroupByCollection').append('<option value="' + id + '">' + text + '</option>');
            Report0006_Settings.UpdateGroupBy();
        });

        $(".yes_no_toggle").live("click", function () {
            if ($(this).next("input").attr("name") == "CreateByClient") {
                if ($(this).next("input").val() == "1") {
                    $("#clientSelector").show();
                    $("#clientOrganizationSelector").hide();
                } else {
                    $("#clientOrganizationSelector").show();
                    $("#clientSelector").hide();
                }
            }
        });

        $("#btnRender, #btnRender2").live("click", function () {
            if (ValidateReportParameters()) {
                window.open(CreateActionURLParameters("Report0006"));
            }
        });

        $("#btnExportToExcel, #btnExportToExcel2").live("click", function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0006ExportToExcel");
                StartButtonProgress($(this));
                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0006Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0006Settings");
                    }
                });
            }
        });

        function CreateActionURLParameters(actionName) {
            var Url = "/Report/" + actionName + "?" +
                "startDate=" + $("#StartDate").val() +
                "&endDate=" + $("#EndDate").val() +
                "&groupByCollectionIDs=" + $("#GroupByCollectionIDs").val() +
                "&showClientSummary=" + $("#ShowClientSummary").val() +
                "&showClientOrganizationSummary=" + $("#ShowClientOrganizationSummary").val() +
                "&showClientContractSummary=" + $("#ShowClientContractSummary").val() +
                "&showBalanceDocumentSummary=" + $("#ShowBalanceDocumentSummary").val() +
                "&showBalanceDocumentFullInfo=" + $("#ShowBalanceDocumentFullInfo").val() +
                "&includeExpenditureWaybillsAndReturnFromClientWaybills=" + $("#IncludeExpenditureWaybillsAndReturnFromClientWaybills").val() +
                "&includeDealPayments=" + $("#IncludeDealPayments").val() +
                "&includeDealInitialBalanceCorrections=" + $("#IncludeDealInitialBalanceCorrections").val() +
                "&createByClient=" + $("#CreateByClient").val() +
                "&" + $("#multipleSelectorTeam").FormSelectedEntitiesUrlParametersString("AllTeams", "TeamIDs");

            // В зависимости от режима создания отчета добавляем Id клиентов либо организаций клиентов
            if ($("#CreateByClient").val() == "1") {
                Url = Url + "&" + $("#multipleSelectorClient").FormSelectedEntitiesUrlParametersString("AllClients", "ClientIDs");
            } else {
                Url = Url + "&" + $("#multipleSelectorClientOrganization").FormSelectedEntitiesUrlParametersString("AllClientOrganizations", "ClientOrganizationIDs");
            }
            return Url;
        }

        function ValidateReportParameters() {
            var scroll_y = $("#messageReport0006Settings").offset().top;

            if (!Report0006_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0006Settings", true)) {
                return false;
            }

            if ($("#ShowClientSummary").val() == "0" && $("#ShowClientOrganizationSummary").val() == "0" && $("#ShowClientContractSummary").val() == "0" &&
                    $("#ShowBalanceDocumentSummary").val() == "0" && $("#ShowBalanceDocumentFullInfo").val() == "0") {

                scroll(0, scroll_y);
                ShowErrorMessage("Необходимо выбрать хотя бы одну таблицу.", "messageReport0006Settings");

                return false;
            }

            if ($("#IncludeExpenditureWaybillsAndReturnFromClientWaybills").val() == "0" && $("#IncludeDealPayments").val() == "0" &&
                    $("#IncludeDealInitialBalanceCorrections").val() == "0") {
                scroll(0, scroll_y);
                ShowErrorMessage("Необходимо учитывать хотя бы один вид документов.", "messageReport0006Settings");

                return false;
            }

            if ($("#CreateByClient").val() == "1") {
                if (IsFalse($("#multipleSelectorClient").CheckSelectedEntitiesCount("Не выбрано ни одного клиента.",
                    "Выберите всех клиентов или не больше ", "messageReport0006Settings"))) {
                    scroll(0, scroll_y);
                    return false;
                }
            }

            if ($("#CreateByClient").val() == "0") {
                if (IsFalse($("#multipleSelectorClientOrganization").CheckSelectedEntitiesCount("Не выбрано ни одной организации клиента.",
                    "Выберите все организации клиентов или не больше ", "messageReport0006Settings"))) {
                    scroll(0, scroll_y);
                    return false;
                }
            }

            if (IsFalse($("#multipleSelectorTeam").CheckSelectedEntitiesCount("Не выбрано ни одной команды.",
                    "Выберите все команды или не больше ", "messageReport0006Settings"))) {
                scroll(0, scroll_y);
                return false;
            }
            return true;
        }
    },

    ValidateDate: function (startDate, endDate, messageId, performScrolling) {
        var starDateObj = stringToDate(startDate);
        var endDateObj = stringToDate(endDate);

        if (!isValidDate(starDateObj)) {
            if (performScrolling) {
                scroll(0, 196);
            }
            ShowErrorMessage("Неверная дата начала периода.", messageId);

            return false;
        }

        if (!isValidDate(endDateObj)) {
            if (performScrolling) {
                scroll(0, 196);
            }
            ShowErrorMessage("Неверная дата окончания периода.", messageId);

            return false;
        }

        if (starDateObj > endDateObj) {
            if (performScrolling) {
                scroll(0, 196);
            }
            ShowErrorMessage("Дата начала периода должна быть меньше даты окончания периода.", messageId);

            return false;
        }

        var today = new Date();
        if (endDateObj > today) {
            if (performScrolling) {
                scroll(0, 196);
            }
            ShowErrorMessage("Дата окончания периода должна быть меньше или равна текущей дате.", messageId);

            return false;
        }

        return true;
    },

    // Обновление строки с кодами группировок, установка статуса ComboBox (выключен, если договор уже добавлен)
    UpdateGroupBy: function () {
        var clientContractGroupingId = "3";
        var clientContractGroupingAdded = false;
        var str = "";
        var elements = $("#tblGroupBy tbody").children();
        for (var i = 1; i < elements.length; ++i) {
            str += $(elements[i]).attr("id");
            if ($(elements[i]).attr("id") == clientContractGroupingId) clientContractGroupingAdded = true;
            if (i < elements.length - 1) str += '_';
        }
        if (str != "")
            $('#tblGroupBy').show();
        else
            $('#tblGroupBy').hide();
        $('#GroupByCollectionIDs').val(str);
        UpdateButtonAvailability("GroupByCollection", !clientContractGroupingAdded);
        $("#AllowToAddGrouping").val(!clientContractGroupingAdded);
    }
};
