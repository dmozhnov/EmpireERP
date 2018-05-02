var Report0003_Settings = {
    Init: function () {

        $("#btnBack").live("click", function () {
            window.location = $('#BackURL').val();
        });

        $("#btnRestoreDefaults").live("click", function () {
            window.location = window.location;
        });

        $('#btnExportToExcel, #btnExportToExcel2').live('click', function () {
            if (ValidateReportParameters()) {
                var url = CreateActionURLParameters("Report0003ExportToExcel");
                StartButtonProgress($(this));
                $.fileDownload(url, {
                    successCallback: function (response) {
                        StopButtonProgress();
                        ShowSuccessMessage("Файл успешно сформирован.", "messageReport0003Settings");
                    },
                    failCallback: function (response) {
                        StopButtonProgress();
                        ShowErrorMessage("Произошла ошибка при выгрузке отчета: " + response, "messageReport0003Settings");
                    }
                });
            }
        });

        $('#btnRender, #btnRender2').live('click', function () {
            if (ValidateReportParameters()) {
                window.open(CreateActionURLParameters("Report0003"));
            }
        });

        function CreateActionURLParameters(actionName) {
            var Url = "/Report/" + actionName + "?" +
                $("#multipleSelectorStorages").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs") +
                "&DevideByInnerOuterMovement=" + $("#DevideByInnerOuterMovement").val() +
                "&StartDate=" + $("#StartDate").val() +
                "&EndDate=" + $("#EndDate").val();
            return Url;
        }

        function ValidateReportParameters() {

            if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
            "Выберите все места хранения, или не больше ", "messageReport0003Settings"))) {
                scroll(0, 205);
                return false;
            }

            if (!Report0003_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0003Settings")) {
                return false;
            }

            return true;
        }
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
    }
};