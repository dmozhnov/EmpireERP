var Organization_ForeignBankAccountEdit = {
    Init: function () {
        $("#foreignBankAccountEdit #BankAccountNumber").focus();

        $("#SWIFT").bind("keyup change paste cut", function () {
            var swift = $(this).val();

            if (swift.length == 8 || swift.length == 11) {
                $.ajax({
                    type: "GET",
                    url: "/Organization/GetForeignBankBySWIFT",
                    data: { swift: swift },
                    success: function (result) {
                        $("#BankName").html(result.BankName);
                        $("#BankAddress").html(result.Address);
                        $("#ClearingCode").html(result.ClearingCode);
                        $("#ClearingCodeType").html(result.ClearingCodeType);
                        UpdateButtonAvailability("btnSaveForeignBankAccount", result.BankName.length > 0);
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        Organization_ForeignBankAccountEdit.InputBadBIC();
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountEdit");
                    }
                });
            }
            else {
                Organization_ForeignBankAccountEdit.InputBadBIC();
            }
        });

        $("#addCurrency").click(function () {
            //вызвать окно добавления валюты
        });
    },

    InputBadBIC: function () {
        $("#BankName").text("");
        $("#BankAddress").text("");
        $("#ClearingCode").text("");
        $("#ClearingCodeType").text("");
        DisableButton("btnSaveForeignBankAccount");  // Блокируем кнопку "сохранить"
    },

    OnFailForeignBankAccountEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageForeignBankAccountEdit");
    }
};