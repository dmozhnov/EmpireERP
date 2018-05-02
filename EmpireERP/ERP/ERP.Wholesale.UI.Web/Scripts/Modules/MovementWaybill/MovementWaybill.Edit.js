var MovementWaybill_Edit = {
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

            $("#btnBack").live('click', function () {
                window.location = $('#BackURL').val();
            });

            $('#Number').change(function () {
                var num = $('#Number').val();
                var id = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/IsNumberUnique",
                    data: { number: num, id: id },
                    success: function (result) {
                        if (result == "False") {
                            $('#Number').addClass('input-validation-error');
                            $('#NumberIsUnique_validationMessage').removeClass('field-validation-valid').addClass('field-validation-error').text('Данный номер уже используется. Укажите другой номер.');
                            $('#NumberIsUnique').val(0);
                        }
                        else {
                            $('#NumberIsUnique_validationMessage').addClass('field-validation-valid').removeClass('field-validation-error').text('');
                            $('#NumberIsUnique').val(1);
                        }
                    }
                });
            });

            // связывание списков отправитель-организация отправителя
            $('#SenderStorageId').FillChildComboBox('SenderId', "/MovementWaybill/GetAccountOrganizationsForSenderStorage", 'storageId', "messageMovementWaybillEdit");

            // связывание списков получатель-организация получателя
            $('#RecipientStorageId').FillChildComboBox('RecipientId', "/MovementWaybill/GetAccountOrganizationsForRecipientStorage", 'storageId', "messageMovementWaybillEdit");

            MovementWaybill_Edit.UpdateValueAddedTaxListState($("#AllowToChangeValueAddedTax").val());

            $("#SenderStorageId").change(function () {
                MovementWaybill_Edit.UpdateValueAddedTaxListState(true);
            });

            $("#RecipientStorageId").change(function () {
                MovementWaybill_Edit.UpdateValueAddedTaxListState(true);
            });

            $("#SenderId").change(function () {
                MovementWaybill_Edit.RecalculateValueAddedTaxListState();
            });

            $("#RecipientId").change(function () {
                MovementWaybill_Edit.RecalculateValueAddedTaxListState();
            });

            $("#btnSave").live("click", function () {
                MovementWaybill_Edit.UpdateValueAddedTaxListState(true);
            });

            $("#CuratorName").click(function () {
                var storageId = $("#SenderStorageId").val() + "_" + $("#RecipientStorageId").val();
                if ($("#SenderStorageId").val() != "" && $("#RecipientStorageId").val() != "") {
                    Waybill_Edit.ShowCuratorSelectorForm(2/*WaybillTypeId*/, storageId, "", $(this), "messageMovementWaybillEdit");
                }
                else {
                    if ($("#SenderStorageId").val() == "") {
                        $("#SenderStorageId").ValidationError("Укажите отправителя.");
                    }
                    if ($("#RecipientStorageId").val() == "") {
                        $("#RecipientStorageId").ValidationError("Укажите получателя.");
                    }
                }
            });
        });
    },

    OnSuccessMovementWaybillEdit: function (ajaxContext) {
        window.location = "/MovementWaybill/Details?id=" + ajaxContext + "&backURL=/MovementWaybill/List";
    },

    OnFailMovementWaybillEdit: function (ajaxContext) {
        MovementWaybill_Edit.RecalculateValueAddedTaxListState();
        ShowErrorMessage(ajaxContext.responseText, "messageMovementWaybillEdit");
    },

    RecalculateValueAddedTaxListState: function () {
        var senderId = $("#SenderId").val();
        var recipientId = $("#RecipientId").val();
        MovementWaybill_Edit.UpdateValueAddedTaxListState(IsDefaultOrEmpty(senderId) || IsDefaultOrEmpty(recipientId) || senderId != recipientId);
    },

    UpdateValueAddedTaxListState: function (value) {
        if (IsFalse(value)) {
            // Делаем выбранным элемент с 0 значением НДС (атрибут param)
            $("#ValueAddedTaxId option[param='0']").attr("selected", "selected");
        }
        UpdateButtonAvailability("ValueAddedTaxId", value);
    }
};