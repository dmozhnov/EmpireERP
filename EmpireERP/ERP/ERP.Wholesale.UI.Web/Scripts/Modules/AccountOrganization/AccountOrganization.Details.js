var AccountOrganization_Details = {
    Init: function () {
        $(document).ready(function () {
            $('#btnBack').click(function () {
                window.location = $('#BackURL').val();
            });

            var currentUrl = $("#currentUrl").val();
            $("#gridStorage table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $('#btnAddRussianBankAccount').live("click", function () {
                StartButtonProgress($("#btnAddRussianBankAccount"));

                $.ajax({
                    type: "GET",
                    url: "/AccountOrganization/AddRussianBankAccount",
                    data: { accountOrganizationId: $('#AccountOrganizationId').val() },
                    success: function (result) {
                        $('#accountOrganizationBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#accountOrganizationBankAccountDetailsForEdit"));
                        ShowModal("accountOrganizationBankAccountDetailsForEdit");
                        $('#accountOrganizationBankAccountDetailsForEdit #BankAccountNumber').focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            $('#btnAddForeignBankAccount').live("click", function () {
                StartButtonProgress($("#btnAddForeignBankAccount"));

                $.ajax({
                    type: "GET",
                    url: "/AccountOrganization/AddForeignBankAccount",
                    data: { accountOrganizationId: $('#AccountOrganizationId').val() },
                    success: function (result) {
                        $('#accountOrganizationForeignBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#accountOrganizationForeignBankAccountDetailsForEdit"));
                        ShowModal("accountOrganizationForeignBankAccountDetailsForEdit");
                        $('#accountOrganizationForeignBankAccountDetailsForEdit #BankAccountNumber').focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            // Редактирование счета
            $('#gridRussianBankAccounts .edit_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();
                var accountOrganizationId = $('#AccountOrganizationId').val();
                $.ajax({
                    type: "GET",
                    url: "/AccountOrganization/EditRussianBankAccount",
                    data: { accountOrganizationId: accountOrganizationId, bankAccountId: accountId },
                    success: function (result) {
                        $('#accountOrganizationBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#accountOrganizationBankAccountDetailsForEdit"));
                        ShowModal("accountOrganizationBankAccountDetailsForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBankAccountList");
                    }
                });
            });

            // Редактирование счета в иностранном банке
            $('#gridForeignBankAccounts .edit_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();
                var accountOrganizationId = $('#AccountOrganizationId').val();
                $.ajax({
                    type: "GET",
                    url: "/AccountOrganization/EditForeignBankAccount",
                    data: { accountOrganizationId: accountOrganizationId, bankAccountId: accountId },
                    success: function (result) {
                        $('#accountOrganizationForeignBankAccountDetailsForEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#accountOrganizationForeignBankAccountDetailsForEdit"));
                        ShowModal("accountOrganizationForeignBankAccountDetailsForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBankAccountList");
                    }
                });
            });

            // Удаление счета
            $('#gridRussianBankAccounts .delete_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();
                var accountOrganizationId = $('#AccountOrganizationId').val();

                if (confirm('Вы уверены?')) {
                    StartGridProgress($(this).closest(".grid"));

                    $.ajax({
                        type: "POST",
                        url: "/AccountOrganization/RemoveRussianBankAccount",
                        data: { accountOrganizationId: accountOrganizationId, bankAccountId: accountId },
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
            // Удаление счета в иностранном банке
            $('#gridForeignBankAccounts .delete_link').live('click', function () {
                var accountId = $(this).parent('td').parent('tr').find('.BankAccountId').text();
                var accountOrganizationId = $('#AccountOrganizationId').val();

                if (confirm('Вы уверены?')) {
                    StartGridProgress($(this).closest(".grid"));

                    $.ajax({
                        type: "POST",
                        url: "/AccountOrganization/RemoveForeignBankAccount",
                        data: { accountOrganizationId: accountOrganizationId, bankAccountId: accountId },
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


            $('#btnAddLinkedStorage').live('click', function () {
                StartButtonProgress($("#btnAddLinkedStorage"));

                $.ajax({
                    type: "GET",
                    url: "/AccountOrganization/StoragesList",
                    data: { orgId: $('#AccountOrganizationId').val() },
                    success: function (result) {
                        $('#storageSelectList').hide().html(result);
                        $.validator.unobtrusive.parse($("#storageSelectList"));
                        ShowModal("storageSelectList");
                    }
                });
            });

            // редактирование организации
            $('#btnEditAccountOrganization').click(function () {
                $.ajax({
                    type: "GET",
                    url: "/AccountOrganization/Edit",
                    data: { accountOrganizationId: $('#AccountOrganizationId').val() },
                    success: function (result) {
                        $('#organizationEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#organizationEdit"));
                        ShowModal("organizationEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountOrganizationEdit");
                    }
                });
            });

            $('#btnDeleteAccountOrganization').click(function () {
                if (confirm("Вы уверены?")) {

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/AccountOrganization/Delete",
                        data: { accountOrganizationId: $('#AccountOrganizationId').val() },
                        success: function (result) {
                            window.location = "/AccountOrganization/List";
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountOrganizationEdit");
                        }
                    });
                }
            });
        });
    },

    OnSuccessAccountOrganizationEdit: function (ajaxContext) {
        HideModal();
        $.ajax({
            type: "GET",
            url: "/AccountOrganization/ShowMainDetails",
            data: { accountOrganizationId: $('#AccountOrganizationId').val() },
            success: function (result) {
                $('#accountOrganizationMainDetails').html(result);
                $('.page_title_item_name').text($('#OrganizationName').val());
                ShowSuccessMessage("Сохранено.", "messageAccountOrganizationEdit");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountOrganizationEdit");
            }
        });
    },

    OnSuccessStorageAdd: function () {
        HideModal();
        RefreshGrid("gridStorage", function () {
            ShowSuccessMessage("Место хранения добавлено.", "messageStorageList");
        });
    },

    OnSuccessRussianBankAccountEdit: function (ajaxContext) {
        HideModal();
        RefreshGrid("gridRussianBankAccounts", function () {
            ShowSuccessMessage('Сохранено.', 'messageRussianBankAccountList');
        });
    },

    OnSuccessForeignBankAccountEdit: function (ajaxContext) {
        HideModal();
        RefreshGrid("gridForeignBankAccounts", function () {
            ShowSuccessMessage('Сохранено.', 'messageForeignBankAccountList');
        });
    }
};