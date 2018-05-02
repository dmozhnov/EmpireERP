var ProductionOrder_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#Name").focus();

            $("#btnSave").click(function () {
                if (!ProductionOrder_Edit.IsWorkDay("MondayIsWorkDay") &&
                    !ProductionOrder_Edit.IsWorkDay("TuesdayIsWorkDay") &&
                    !ProductionOrder_Edit.IsWorkDay("WednesdayIsWorkDay") &&
                    !ProductionOrder_Edit.IsWorkDay("ThursdayIsWorkDay") &&
                    !ProductionOrder_Edit.IsWorkDay("FridayIsWorkDay") &&
                    !ProductionOrder_Edit.IsWorkDay("SaturdayIsWorkDay") &&
                    !ProductionOrder_Edit.IsWorkDay("SundayIsWorkDay")) {
                    ShowErrorMessage("График рабочих дней должен содержать хотя бы один рабочий день.", "messageProductionOrderEdit");
                    return false;
                }
            });
        });

        $("#btnBack").live("click", function () {
            window.location = $('#BackUrl').val();
        });

        $("#SystemStagePlannedDuration").live("keyup change paste cut", function () {
            var duration = Number($("#SystemStagePlannedDuration").val());
            if (isNaN(+duration)) return false;

            var startDate = stringToDate($("#Date").val());
            if (!isValidDate(startDate)) return false;

            startDate.setDate(startDate.getDate() + duration);

            $("#SystemStagePlannedEndDate").val(dateToString(startDate));
        });

        $("#SystemStagePlannedEndDate").live("keyup change paste cut", function () {

            var startDate = stringToDate($("#Date").val());
            var endDate = stringToDate($("#SystemStagePlannedEndDate").val());
            if (!isValidDate(endDate)) return false;

            $("#SystemStagePlannedDuration").val(Math.ceil((endDate - startDate) / (1000 * 60 * 60 * 24)));
        });

        $("#ProducerName").live("click", function () {
            if (IsTrue($("#AllowToChangeProducer").val())) {
                $.ajax({
                    type: "GET",
                    url: "/Producer/SelectProducer",
                    success: function (result) {
                        $("#producerSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#producerSelector"));
                        ShowModal("producerSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderEdit");
                    }
                });
            }
        });
    },

    OnSuccessProductionOrderEdit: function (ajaxContext) {
        if ($("#Id").val() == "00000000-0000-0000-0000-000000000000" || $("#BackUrl").val() == "") {
            window.location = "/ProductionOrder/Details?id=" + ajaxContext + GetBackUrlFromString($('#BackUrl').val());
        } else {
            window.location = $("#BackUrl").val();
        }
    },

    OnBeginProductionOrderSave: function () {
        StartButtonProgress($("#btnSave"));
    },

    OnFailProductionOrderEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderEdit");
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора производителя
    OnProducerSelectLinkClick: function (producerId, producerName) {
        $("#ProducerId").val(producerId);
        $("#ProducerId").ValidationValid();
        $("#ProducerName").text(producerName);
        HideModal();
    },

    // Читаем значение, является ли день рабочим днем, из CheckBox или Hidden (смотря какой контрол имеет нужный id)
    // Если CheckBox disabled, id достается Hidden, а CheckBox без Id. Если CheckBox enabled, id есть у CheckBox, а hidden MVC делает без Id
    IsWorkDay: function (controlId) {
        if ($('#' + controlId + '[type=checkbox]').length != 0) {
            return $('#' + controlId + '[type=checkbox]').attr("checked");
        } else if ($('#' + controlId + '[type=hidden]').length != 0) {
            return IsTrue($('#' + controlId + '[type=hidden]').val())
        } else {
            return false;
        }
    }
};