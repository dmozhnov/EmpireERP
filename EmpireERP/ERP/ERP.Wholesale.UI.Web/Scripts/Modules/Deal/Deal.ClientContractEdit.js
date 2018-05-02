var Deal_ClientContractEdit = {
    Init: function () {
        $(document).ready(function () {
            // Вывод модальной формы "Добавление собственной организации"
            $("#linkAccountOrganizationSelector").click(function () {
                if (IsTrue($("#clientContractEdit #AllowToEditOrganization").val())) {
                    $.ajax({
                        type: "GET",
                        url: "/AccountOrganization/SelectAccountOrganization",
                        success: function (result) {
                            $("#accountOrganizationSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#accountOrganizationSelector"));
                            ShowModal("accountOrganizationSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageContractEdit");
                        }
                    });
                }
            });

            // Вывод модальной формы "Добавление связанной организации"
            $("#linkClientOrganizationSelector").click(function () {
                var clientId = $('#ClientId').val();
                if (IsTrue($("#clientContractEdit #AllowToEditOrganization").val())) {
                    $.ajax({
                        type: "GET",
                        url: "/Client/SelectClientOrganization",
                        data: { clientId: clientId, mode: "includeclient" },
                        success: function (result) {
                            $("#contractorOrganizationSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#contractorOrganizationSelector"));
                            ShowModal("contractorOrganizationSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageContractEdit");
                        }
                    });
                }
            });
        });
    },
    
    OnFailContractEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageContractEdit");
    },

    OnSuccessOrganizationEdit: function (result) {
        $('#clientContractEdit #ClientOrganizationId').val(result.organizationId).ValidationValid();
        $('#clientContractEdit #ClientOrganizationName').text(result.organizationShortName);
        HideModal(function () {
            ShowSuccessMessage("Организация создана и добавлена в список организаций клиента.", "messageClientOrganizationList");
        });
    },

    OnSuccessEconomicAgentTypeSelect: function (ajaxContext) {
        HideModal(function () {
            HideModal(function () {
                $("#economicAgentEdit").html(ajaxContext);
                $.validator.unobtrusive.parse($("#economicAgentEdit"));
                ShowModal("economicAgentEdit");
            });
        });
    },
    
    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора собственных организаций  (при создании договора)
    OnAccountOrganizationSelectLinkClick: function (accountOrganizationId, accountOrganizationShortName) {
        $("#clientContractEdit #AccountOrganizationId").val(accountOrganizationId).ValidationValid();
        $("#clientContractEdit #AccountOrganizationName").text(accountOrganizationShortName);
        HideModal();
    },

    // Пользователь щелкнул на ссылку "Выбрать" в гриде выбора организаций контрагента
    // Грид может быть вызван для добавления организации поставщику (как 1 уровень) и для выбора организации в договор (как 2 уровень)
    OnContractorOrganizationSelectLinkClick: function (organizationId, organizationShortName) {
        $("#clientContractEdit #ClientOrganizationId").val(organizationId).ValidationValid();
        $("#clientContractEdit #ClientOrganizationName").text(organizationShortName);
        HideModal();
    }
};