var Report0010_Settings = {
    Init: function () {
        $(function () {
            $('#multipleSelectorClient_selected_values').val("");
            $('#multipleSelectorTeam_selected_values').val("");
            $('#multipleSelectorUser_selected_values').val("");

            // управляем зависимостями от параметра «Развернутая информация с документами оплат»
            $("#ShowDetailsTable").prev().bind('change', function () {
                if (IsTrue($("#ShowDetailsTable").val())) {
                    EnableYesNoToggle($("#SeparateByDealPaymentForm").prev());
                    EnableYesNoToggle($("#ShowDistributedAndUndistributedSums").prev());
                    EnableYesNoToggle($("#ShowDistributionDetails").prev());
                }
                else {
                    ResetYesNoToggle($("#SeparateByDealPaymentForm").prev());
                    DisableYesNoToggle($("#SeparateByDealPaymentForm").prev());
                    ResetYesNoToggle($("#ShowDistributedAndUndistributedSums").prev());
                    DisableYesNoToggle($("#ShowDistributedAndUndistributedSums").prev());
                    ResetYesNoToggle($("#ShowDistributionDetails").prev());
                    DisableYesNoToggle($("#ShowDistributionDetails").prev());
                }

                $("#ShowDistributedAndUndistributedSums").prev().trigger("change");
            });

            // управляем зависимостями от параметра «Выводить столбцы «Разнесено в сумме» и «Неразнесенный остаток»»
            $("#ShowDistributedAndUndistributedSums").prev().bind('change', function () {
                if (IsTrue($("#ShowDistributedAndUndistributedSums").val())) {
                    EnableYesNoToggle($("#ShowDistributionDetails").prev());
                }
                else {
                    ResetYesNoToggle($("#ShowDistributionDetails").prev());
                    DisableYesNoToggle($("#ShowDistributionDetails").prev());
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
            var curElem = $('#GroupByCollection option:selected');
            var curVal = curElem.text();
            var curId = $('#GroupByCollection').val();
            if (IsNumber(curId)) {
                if (curId >= 0) {
                    curElem.remove();
                    var genLink = '<span class="link" id="btnDelGroupBy" delId="' + curId + '">Убрать</span>';
                    $("#tblGroupBy tbody").append('<tr id="' + curId + '"><td>' + genLink + '</td><td>' + curVal + '</td></tr>');
                    UpdateGroupBy();
                }
            }
        });

        $("#btnDelGroupBy").live("click", function () {
            var id = $(this).attr('delId');
            var text = $(this).parent().next().html();
            $(this).parent().parent().remove();
            $('#GroupByCollection').append('<option value="' + id + '">' + text + '</option>');
            UpdateGroupBy();
        });

        // Обновление строки с кодами группировок, установка статуса ComboBox (выключен, если договор уже добавлен)
        function UpdateGroupBy() {
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

        $('#btnRender, #btnRender2').live('click', function () {
            if (ValidateReportParameters()) {
                window.open(CreateActionURLParameters("Report0010"));
            }
        });

        $("#btnExportToExcel, #btnExportToExcel2").live("click", function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0010ExportToExcel");
                StartButtonProgress($(this));
                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0010Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0010Settings");
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
                "&showAccountOrganizationSummary=" + $("#ShowAccountOrganizationSummary").val() +
                "&showClientContractSummary=" + $("#ShowClientContractSummary").val() +
                "&showTeamSummary=" + $("#ShowTeamSummary").val() +
                "&showUserSummary=" + $("#ShowUserSummary").val() +
                "&showDetailsTable=" + $("#ShowDetailsTable").val() +
                "&separateByDealPaymentForm=" + $("#SeparateByDealPaymentForm").val() +
                "&showDistributedAndUndistributedSums=" + $("#ShowDistributedAndUndistributedSums").val() +
                "&showDistributionDetails=" + $("#ShowDistributionDetails").val() +
                "&" + $("#multipleSelectorClient").FormSelectedEntitiesUrlParametersString("AllClients", "ClientIDs") +
                "&" + $("#multipleSelectorAccountOrganization").FormSelectedEntitiesUrlParametersString("AllAccountOrganizations", "AccountOrganizationIDs") +
                "&" + $("#multipleSelectorTeam").FormSelectedEntitiesUrlParametersString("AllTeams", "TeamIDs") +
                "&" + $("#multipleSelectorUser").FormSelectedEntitiesUrlParametersString("AllUsers", "UserIDs");
            return Url;
        }

        function ValidateReportParameters() {
            var scroll_y = $("#messageReport0010Settings").offset().top - 10;

            if (!ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0010Settings", true, scroll_y)) {
                return false;
            }

            if ($("#ShowClientSummary").val() == "0" && $("#ShowClientOrganizationSummary").val() == "0" && $("#ShowClientContractSummary").val() == "0" &&
                    $("#ShowTeamSummary").val() == "0" && $("#ShowUserSummary").val() == "0" && $("#ShowDetailsTable").val() == "0") {

                scroll(0, scroll_y);
                ShowErrorMessage("Необходимо выбрать хотя бы одну таблицу.", "messageReport0010Settings");

                return false;
            }

            if (IsFalse($("#multipleSelectorClient").CheckSelectedEntitiesCount("Не выбрано ни одного клиента.",
                "Выберите всех клиентов или не больше ", "messageReport0010Settings"))) {
                scroll(0, scroll_y);
                return false;
            }

            if (IsFalse($("#multipleSelectorAccountOrganization").CheckSelectedEntitiesCount("Не выбрана ни одна собственная организация.",
                "Выберите все организации или не больше ", "messageReport0010Settings"))) {
                scroll(0, scroll_y);
                return false;
            }

            if (IsFalse($("#multipleSelectorTeam").CheckSelectedEntitiesCount("Не выбрано ни одной команды.",
                    "Выберите все команды или не больше ", "messageReport0010Settings"))) {
                scroll(0, scroll_y);
                return false;
            }

            if (IsFalse($("#multipleSelectorUser").CheckSelectedEntitiesCount("Не выбрано ни одного пользователя.",
                "Выберите всех пользователей или не больше ", "messageReport0010Settings"))) {
                scroll(0, scroll_y);
                return false;
            }

            return true;
        }

        function ValidateDate(startDate, endDate, messageId, performScrolling, scroll_y) {
            var starDateObj = stringToDate(startDate);
            var endDateObj = stringToDate(endDate);

            if (!isValidDate(starDateObj)) {
                if (performScrolling) {
                    scroll(0, scroll_y);
                }
                ShowErrorMessage("Неверная дата начала периода.", messageId);

                return false;
            }

            if (!isValidDate(endDateObj)) {
                if (performScrolling) {
                    scroll(0, scroll_y);
                }
                ShowErrorMessage("Неверная дата окончания периода.", messageId);

                return false;
            }

            if (starDateObj > endDateObj) {
                if (performScrolling) {
                    scroll(0, scroll_y);
                }
                ShowErrorMessage("Дата начала периода должна быть меньше даты окончания периода.", messageId);

                return false;
            }

            var today = new Date();
            if (endDateObj > today) {
                if (performScrolling) {
                    scroll(0, scroll_y);
                }
                ShowErrorMessage("Дата окончания периода должна быть меньше или равна текущей дате.", messageId);

                return false;
            }

            return true;
        }
    }
};
