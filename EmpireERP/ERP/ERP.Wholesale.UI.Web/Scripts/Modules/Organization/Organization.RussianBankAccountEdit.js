var Organization_RussianBankAccountEdit = {
    Init: function () {
        $(document).ready(function () {
            $("#BankAccountNumber").focus();
        });

        $("#BIC").bind("keyup change paste cut", function () {
            var reg = /^[0-9]{9}$/;
            if (reg.test($(this).val())) {
                $.ajax({
                    type: "GET",
                    url: "/Organization/GetBankByBIC",
                    data: { bic: $(this).val() },
                    success: function (result) {
                        $('#BankName').text(result.BankName);
                        $('#CorAccount').text(result.CorAccount);
                        UpdateButtonAvailability("btnSaveRussianBankAccount", result.BankName.length > 0);
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        Organization_RussianBankAccountEdit.InputBadBIC();
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountEdit");
                    }
                });
            }
            else {
                Organization_RussianBankAccountEdit.InputBadBIC();
            }
        });

    },

    InputBadBIC: function () {
        $('#BankName').text("");
        $('#CorAccount').text("");
        DisableButton("btnSaveRussianBankAccount");  // Блокируем кнопку "сохранить"
    },

    OnFailBankAccountEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageRussianBankAccountEdit");
    }
};