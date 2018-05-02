var ProviderOrganization_Details = {
    Init: function () {
        $(document).ready(function () {
            $('#btnBack').click(function () {
                window.location = $('#BackURL').val();
            });

            $('#btnAddRussianBankAccount').live("click", function () {
                $.ajax({
                    type: "GET",
                    url: "/ProviderOrganization/AddRussianBankAccount/",
                    data: { providerOrganizationId: $('#Id').val() },
                    success: function (result) {
                        $('#RussianBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#RussianBankAccountEdit"));
                        ShowModal("RussianBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            //Добавление счета в иностранном банке
            $('#btnAddForeignBankAccount').live("click", function () {
                $.ajax({
                    type: "GET",
                    url: "/ProviderOrganization/AddForeignBankAccount/",
                    data: { providerOrganizationId: $('#Id').val() },
                    success: function (result) {
                        $('#ForeignBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#ForeignBankAccountEdit"));
                        ShowModal("ForeignBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            $(".linkRussianBankAccountEdit").live("click", function () {
                var bankAccountId = $(this).parent("td").parent("tr").find(".BankAccountId").text();
                $.ajax({
                    type: "GET",
                    url: "/ProviderOrganization/EditRussianBankAccount/",
                    data: { providerOrganizationId: $('#Id').val(), bankAccountId: bankAccountId },
                    success: function (result) {
                        $('#RussianBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#RussianBankAccountEdit"));
                        ShowModal("RussianBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            //Редактирование счета в иностранном банке
            $(".linkForeignBankAccountEdit").live("click", function () {
                var bankAccountId = $(this).parent("td").parent("tr").find(".BankAccountId").text();
                $.ajax({
                    type: "GET",
                    url: "/ProviderOrganization/EditForeignBankAccount/",
                    data: { providerOrganizationId: $('#Id').val(), bankAccountId: bankAccountId },
                    success: function (result) {
                        $('#ForeignBankAccountEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#ForeignBankAccountEdit"));
                        ShowModal("ForeignBankAccountEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            $(".linkRussianBankAccountDelete").live("click", function () {
                var bankAccountId = $(this).parent("td").parent("tr").find(".BankAccountId").text();

                if (confirm("Вы действительно хотите удалить расчетный счет?")) {
                    StartGridProgress($(this).closest(".grid"));
                    
                    $.ajax({
                        type: "POST",
                        url: "/ProviderOrganization/RemoveRussianBankAccount/",
                        data: { providerOrganizationId: $('#Id').val(), bankAccountId: bankAccountId },
                        success: function (result) {
                            RefreshGrid("gridRussianBankAccounts", function () {
                                ShowSuccessMessage("Расчетный счет удален.", "messageRussianBankAccountList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                        }
                    });
                }
            });

            //Удаление счета в иностранном банке
            $(".linkForeignBankAccountDelete").live("click", function () {
                var bankAccountId = $(this).parent("td").parent("tr").find(".BankAccountId").text();

                if (confirm("Вы действительно хотите удалить расчетный счет?")) {
                    StartGridProgress($(this).closest(".grid"));
                    
                    $.ajax({
                        type: "POST",
                        url: "/ProviderOrganization/RemoveForeignBankAccount/",
                        data: { providerOrganizationId: $('#Id').val(), bankAccountId: bankAccountId },
                        success: function (result) {
                            RefreshGrid("gridForeignBankAccounts", function () {
                                ShowSuccessMessage("Расчетный счет удален.", "messageForeignBankAccountList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                        }
                    });
                }
            });

            // редактирование организации
            $("#btnEditProviderOrganization").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ProviderOrganization/Edit",
                    data: { providerOrganizationId: $('#Id').val() },
                    success: function (result) {
                        $('#organizationEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#organizationEdit"));
                        ShowModal("organizationEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderOrganizationEdit");
                    }
                });
            });

            // удаление организации
            $("#btnDeleteProviderOrganization").click(function () {
                if (confirm('Вы действительно хотите удалить организацию?')) {
                    StartButtonProgress($(this));

                    $.ajax({
                        type: "POST",
                        url: "/ProviderOrganization/Delete/",
                        data: { providerOrganizationId: $('#Id').val() },
                        success: function () {
                            window.location = $("#BackURL").val();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderOrganizationEdit");
                        }
                    });
                }
            });

        });    // document ready       
     },

    OnSuccessRussianBankAccountAdd: function (ajaxContext) {
        RefreshGrid("gridRussianBankAccounts", function () {
            ShowSuccessMessage("Расчетный счет добавлен.", "messageRussianBankAccountList");
            HideModal();
        });
    },

    OnSuccessForeignBankAccountAdd: function (ajaxContext) {
        RefreshGrid("gridForeignBankAccounts", function () {
            ShowSuccessMessage("Расчетный счет добавлен.", "messageForeignBankAccountList");
            HideModal();
        });
    },

    OnSuccessRussianBankAccountEdit: function (ajaxContext) {
        RefreshGrid("gridRussianBankAccounts", function () {
            ShowSuccessMessage("Расчетный счет сохранен.", "messageRussianBankAccountList");
            HideModal();
        });
    },

    OnSuccessForeignBankAccountEdit: function (ajaxContext) {
        RefreshGrid("gridForeignBankAccounts", function () {
            ShowSuccessMessage("Расчетный счет сохранен.", "messageForeignBankAccountList");
            HideModal();
        });
    },

    OnSuccessProviderOrganizationEdit:function(ajaxContext) {
        $.ajax({
            type: "GET",
            url: "/ProviderOrganization/ShowMainDetails",
            data: { providerOrganizationId: $('#Id').val() },
            success: function (result) {
                $('#providerOrganizationMainDetails').html(result);
                $.validator.unobtrusive.parse($("#providerOrganizationMainDetails"));
                $('.page_title_item_name').text($('#providerOrganizationMainDetails #ShortName').text());
                ShowSuccessMessage("Сохранено.", "messageProviderOrganizationEdit");
                HideModal();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderOrganizationEdit");
            }
        });
    }
};