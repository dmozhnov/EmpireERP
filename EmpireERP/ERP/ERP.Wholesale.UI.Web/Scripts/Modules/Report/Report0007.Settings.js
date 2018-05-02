var Report0007_Settings = {
    Init: function () {

        $(document).ready(function () {
            $('#tblGroupBy').hide();
            $('#GroupByCollectionIDs').val("");
            $('#multipleSelectorClient_selected_values').val("");
            $('#multipleSelectorStorage_selected_values').val("");
        });

        $("#btnBack, #btnBack2").live("click", function () {
            window.location = $('#BackURL').val();
        });

        $("#btnRestoreDefaults, #btnRestoreDefaults2").live("click", function () {
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
                        Report0007_Settings.UpdateGroupBy();
                    }
                }
            }
        });

        $("#btnDelGroupBy").live("click", function () {
            var id = $(this).attr('delId');
            var text = $(this).parent().next().html();
            $(this).parent().parent().remove();
            $('#GroupByCollection').append('<option value="' + id + '">' + text + '</option>');
            Report0007_Settings.UpdateGroupBy();
        });

        $("#btnRender, #btnRender2").live("click", function () {
            if (ValidateReportParameters()) {
                window.open(CreateActionURLParameters("Report0007"));
            }
        });

        $("#btnExportToExcel, #btnExportToExcel2").live("click", function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0007ExportToExcel");
                StartButtonProgress($(this));
                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0007Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0007Settings");
                    }
                });
            }
        });

        function CreateActionURLParameters(actionName) {
            var Url = "/Report/" + actionName + "?" +
                "Date=" + $("#Date").val() +
                "&ShowOnlyDelayDebt=" + $("#ShowOnlyDelayDebt").val() +
                "&GroupByCollectionIDs=" + $("#GroupByCollectionIDs").val() +
                "&ShowStorageTable=" + $("#ShowStorageTable").val() +
                "&ShowAccountOrganizationTable=" + $("#ShowAccountOrganizationTable").val() +
                "&ShowClientTable=" + $("#ShowClientTable").val() +
                "&ShowClientOrganizationTable=" + $("#ShowClientOrganizationTable").val() +
                "&ShowDealTable=" + $("#ShowDealTable").val() +
                "&ShowTeamTable=" + $("#ShowTeamTable").val() +
                "&ShowUserTable=" + $("#ShowUserTable").val() +
                "&ShowExpenditureWaybillTable=" + $("#ShowExpenditureWaybillTable").val() +
                "&" + $("#multipleSelectorClient").FormSelectedEntitiesUrlParametersString("AllClients", "ClientIDs") +
                "&" + $("#multipleSelectorStorage").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs") +
                "&" + $("#multipleSelectorAccountOrganization").FormSelectedEntitiesUrlParametersString("AllAccountOrganizations", "AccountOrganizationIDs") +
                "&" + $("#multipleSelectorUser").FormSelectedEntitiesUrlParametersString("AllUsers", "UserIDs");
            return Url;
        }

        function ValidateReportParameters() {
            var scroll_y = $("#messageReport0007Settings").offset().top;

            if (!Report0007_Settings.ValidateDate($("#Date").val(), "messageReport0007Settings", true)) {
                return false;
            }

            if ($("#ShowStorageTable").val() == "0" && $("#ShowAccountOrganizationTable").val() == "0" && $("#ShowClientTable").val() == "0" &&
                    $("#ShowClientOrganizationTable").val() == "0" && $("#ShowDealTable").val() == "0" && $("#ShowTeamTable").val() == "0" &&
                    $("#ShowUserTable").val() == "0" && $("#ShowExpenditureWaybillTable").val() == "0") {

                scroll(0, scroll_y);
                ShowErrorMessage("Необходимо выбрать хотя бы одну таблицу.", "messageReport0007Settings");

                return false;
            }

            if (IsFalse($("#multipleSelectorClient").CheckSelectedEntitiesCount("Не выбрано ни одного клиента.",
                "Выберите всех клиентов или не больше ", "messageReport0007Settings"))) {
                scroll(0, scroll_y);
                return false;
            }

            if (IsFalse($("#multipleSelectorStorage").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
                "Выберите все места хранения или не больше ", "messageReport0007Settings"))) {
                scroll(0, scroll_y);
                return false;
            }

            if (IsFalse($("#multipleSelectorAccountOrganization").CheckSelectedEntitiesCount("Не выбрана ни одна собственная организация.",
                "Выберите все организации или не больше ", "messageReport0007Settings"))) {
                scroll(0, scroll_y);
                return false;
            }

            if (IsFalse($("#multipleSelectorUser").CheckSelectedEntitiesCount("Не выбрано ни одного пользователя.",
                "Выберите всех пользователей или не больше ", "messageReport0007Settings"))) {
                scroll(0, scroll_y);
                return false;
            }
            return true;
        }
    },

    ValidateDate: function (date, messageId, performScrolling) {
        var dateObj = stringToDate(date);

        if (!isValidDate(dateObj)) {
            if (performScrolling) {
                scroll(0, 196);
            }
            ShowErrorMessage("Неверная дата отчета", messageId);

            return false;
        }

        return true;
    },

    // Обновление строки с кодами группировок, установка статуса ComboBox
    UpdateGroupBy: function () {
        var str = "";
        var maxElemetcount = parseInt($("#groupByCollectionCount").val());
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

        var isListEmpty = elements.length <= maxElemetcount;
        UpdateButtonAvailability("GroupByCollection", isListEmpty);
        UpdateElementVisibility("btnAddGroupBy", isListEmpty);
        $("#AllowToAddGrouping").val(isListEmpty);
    }
};