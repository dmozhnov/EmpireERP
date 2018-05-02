var WriteoffWaybill_Edit = {
    Init: function () {
        Waybill_Edit.Init();

        $(document).ready(function () {

            $("#rbIsAutoNumber_true").click(function () {
                $("#Number").ValidationValid();
                $("#Number").attr("disabled", "disabled").val("");
                $("#IsAutoNumber").val("1");
            });

            $("#rbIsAutoNumber_false").click(function () {
                $("#Number").removeAttr("disabled").focus();
                $("#IsAutoNumber").val("0");
            });

            // при редактировании
            if (!IsTrue($("#AllowToGenerateNumber").val())) {
                $("#rbIsAutoNumber_false").trigger("click");
                $("#rbIsAutoNumber_false").attr("checked", "checked");

                $("#rbIsAutoNumber_true_wrapper").hide();
                $("#rbIsAutoNumber_false_wrapper").hide();
            }
            // при добавлении
            else {
                $("#rbIsAutoNumber_true").attr("checked", "checked");
                $("#rbIsAutoNumber_true").trigger("click");
            }

            // связывание списков отправитель-организация отправителя
            $('#SenderStorageId').FillChildComboBox('SenderId', "/WriteoffWaybill/GetAccountOrganizationsForStorage", 'storageId', "messageWriteoffWaybillEdit");

            $("#CuratorName").click(function () {
                var storageId = $("#SenderStorageId").val();
                if (storageId != "") {
                    Waybill_Edit.ShowCuratorSelectorForm(3/*WaybillTypeId*/, storageId, "", $(this), "messageWriteoffReasonEdit");
                }
                else {
                    $("#SenderStorageId").ValidationError("Укажите место хранения.");
                }
            });
        });

        $("#btnBack").live('click', function () {
            window.location = $('#BackURL').val();
        });

        $('#btnAddWriteoffReason').live("click", function () {
            $.ajax({
                type: "GET",
                url: "/WriteoffReason/Create",
                success: function (result) {
                    $('#writeoffReasonEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#writeoffReasonEdit"));
                    ShowModal("writeoffReasonEdit");
                    $('#writeoffReasonEdit #Name').focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffReasonEdit");
                }
            });
        });
    },

    OnSuccessWriteoffReasonEdit: function (ajaxContext) {
        $.ajax({
            type: "POST",
            url: "/WriteoffReason/GetWriteoffReasons",
            success: function (result) {
                $('#WriteoffReasonId').fillSelect(result);
                $('#WriteoffReasonId').attr('value', ajaxContext.Id);
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillEdit");
            }
        });

        HideModal(function () {
            ShowSuccessMessage("Основание для списания добавлено.", "messageWriteoffWaybillEdit");
        });

    },

    OnSuccessWriteoffWaybillEdit: function (ajaxContext) {
        window.location = "/WriteoffWaybill/Details?id=" + ajaxContext + "&backURL=/WriteoffWaybill/List";
    },

    OnFailWriteoffWaybillEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageWriteoffWaybillEdit");
    }
};