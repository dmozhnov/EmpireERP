var ChangeOwnerWaybill_Edit = {
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

            $("#btnEdit").click(function () {
                window.location = "/ChangeOwnerWaybill/Edit?id=" + $("#Id").val() + "&backURL=" + $('#currentUrl').val();
            });

            //Обработчик выбора места хранения
            $("#StorageId").change(function () {
                var storageId = $(this).val();
                if (storageId != "") {
                    StartComboBoxProgress($("#SenderId"));
                    StartComboBoxProgress($("#RecipientId"));

                    $.ajax({
                        type: "POST",
                        url: "/ChangeOwnerWaybill/GetOrganizationList",
                        data: { storageId: storageId },
                        success: function (result) {
                            $("#SenderId").removeAttr("disabled");
                            $("#RecipientId").removeAttr("disabled");

                            $("#SenderId").fillSelect(result);
                            $("#RecipientId").fillSelect(result);

                            StopComboBoxProgress($("#SenderId"));
                            StopComboBoxProgress($("#RecipientId"));
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillEdit");
                            StopComboBoxProgress($("#SenderId"));
                            StopComboBoxProgress($("#RecipientId"));
                        }
                    });
                }
                else {
                    $("#SenderId").attr("disabled", "disabled");
                    $("#RecipientId").attr("disabled", "disabled");
                }
            });

            $("#CuratorName").click(function () {
                var storageId = $("#StorageId").val();
                if (storageId != "") {
                    Waybill_Edit.ShowCuratorSelectorForm(5/*WaybillTypeId*/, storageId, "", $(this), "messageChangeOwnerWaybillEdit");
                }
                else {
                    $("#StorageId").ValidationError("Укажите место хранения.");
                }
            });
        });

        $("#btnBack").live("click", function () {
            window.location = $('#BackURL').val();
        });
    },

    OnSuccessChangeOwnerWaybillEdit: function (ajaxContext) {
        window.location = "/ChangeOwnerWaybill/Details?id=" + ajaxContext + "&backURL=" + $("#BackURL").val();
    },

    OnFailChangeOwnerWaybillEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageChangeOwnerWaybillEdit");
    }
};