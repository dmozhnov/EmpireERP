var Provider_ContractEdit = {
    Init: function () {
        $(document).ready(function () {
            // Вывод модальной формы "Добавление собственной организации"
            $("#linkAccountOrganizationSelector.select_link").click(function () {
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
            });

            // Вывод модальной формы "Добавление связанной организации"
            $("#linkProviderOrganizationSelector.select_link").click(function () {
                var providerId = $('#MainDetails_Id').val();
                $.ajax({
                    type: "GET",
                    url: "/Provider/SelectContractorOrganization",
                    data: { providerId: providerId, mode: "includeprovider" },
                    success: function (result) {
                        $("#contractorOrganizationSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#contractorOrganizationSelector"));
                        ShowModal("contractorOrganizationSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageContractEdit");
                    }
                });
            });
        });
    }
};