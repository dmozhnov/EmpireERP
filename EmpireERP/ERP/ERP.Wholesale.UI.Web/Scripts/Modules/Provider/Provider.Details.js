var Provider_Details = {
    Init: function () {
        $(document).ready(function () {
            $("#btnBack").click(function () {
                window.location = $("#BackURL").val();
            });

            // Редактировать
            $("#btnEditProvider").click(function () {
                var providerId = $("#MainDetails_Id").val();
                window.location = "/Provider/Edit?id=" + providerId + GetBackUrl();
            });

            // Удалить поставщика
            $("#btnDeleteProvider").click(function () {
                if (confirm('Вы уверены?')) {
                    var providerId = $("#MainDetails_Id").val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/Provider/Delete/",
                        data: { providerId: providerId },
                        success: function () {
                            window.location = $("#BackURL").val();
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderDetails");
                        }
                    });
                }
            });

        });  // document ready
    },
    // обновление изменяющихся деталей шапки
    RefreshMainDetails: function (details) {
        $("#ProviderOrganizationCount").text(details.ProviderOrganizationCount);
        $("#ContractCount").text(details.ContractCount);
    },

    // после успешного создания новой организации и добавления ее поставщику
    OnSuccessOrganizationEdit: function (result) {
        RefreshGrid("gridProviderOrganization", function () {
            HideModal();
            HideModal();
            Provider_Details.RefreshMainDetails(result);
            // Хак, потом переделать
            if ($("div.modal").length == 0) { // если выбрана организация для добавления поставщику
                ShowSuccessMessage("Организация создана и добавлена в список организаций поставщика.", "messageProviderOrganizationList");
            }
            else { // если выбрана организация для создания по ней договора
                $("#providerContractEdit #ProviderOrganizationId").val(result.ContractorOrganizationId);
                $("#providerContractEdit #ProviderOrganizationId").ValidationValid();
                $("#providerContractEdit #ProviderOrganizationName").text(result.ContractorOrganizationShortName);
                ShowSuccessMessage("Организация создана и добавлена в список организаций поставщика.", "messageProviderOrganizationList");
            }
        });
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора собственных организаций (при создании договора)
    OnAccountOrganizationSelectLinkClick: function (accountOrganizationId, accountOrganizationShortName) {
        $("#providerContractEdit #AccountOrganizationId").val(accountOrganizationId);
        $("#providerContractEdit #AccountOrganizationId").ValidationValid();
        $("#providerContractEdit #AccountOrganizationName").text(accountOrganizationShortName);
        HideModal();
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора организаций контрагента
    // Грид может быть вызван для добавления организации поставщику (как 1 уровень) и для выбора организации в договор (как 2 уровень)
    OnContractorOrganizationSelectLinkClick: function (organizationId, organizationShortName) {
        // Хак, потом переделать
        if ($("div.modal2").length == 0) { // если выбрана организация для добавления поставщику
            Provider_Details.AddContractorOrganizationToProvider(organizationId);
        }
        else { // если выбрана организация для создания по ней договора
            $("#providerContractEdit #ProviderOrganizationId").val(organizationId);
            $("#providerContractEdit #ProviderOrganizationId").ValidationValid();
            $("#providerContractEdit #ProviderOrganizationName").text(organizationShortName);
            HideModal();
        }
    },

    AddContractorOrganizationToProvider: function (organizationId) {
        var providerId = $("#MainDetails_Id").val();
        $.ajax({
            type: "POST",
            url: "/Provider/AddContractorOrganization/",
            data: { providerId: providerId, organizationId: organizationId },
            success: function (result) {
                RefreshGrid("gridProviderOrganization", function () {
                    Provider_Details.RefreshMainDetails(result);
                    ShowSuccessMessage("Организация добавлена.", "messageProviderOrganizationList");
                    HideModal();
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageOrganizationSelectList");
            }
        });
    },

    OnSuccessContractEdit: function (result) {
        RefreshGrid("gridProviderOrganization", function () {
            RefreshGrid("gridProviderContract", function () {
                Provider_Details.RefreshMainDetails(result);
                if ($("#providerContractEdit #Id").val() != "0")
                    ShowSuccessMessage("Сохранено.", "messageContractList");
                else
                    ShowSuccessMessage("Договор создан.", "messageContractList");
                HideModal();
            });
        });
    },

    OnFailContractEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageContractEdit");
    },

    OnSuccessEconomicAgentTypeSelect: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#economicAgentEdit").html(ajaxContext);
                $.validator.unobtrusive.parse($("#economicAgentEdit"));
                ShowModal("economicAgentEdit");
            });
        });
    }
};