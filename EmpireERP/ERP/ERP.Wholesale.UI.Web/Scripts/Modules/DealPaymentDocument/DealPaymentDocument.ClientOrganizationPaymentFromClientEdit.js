var DealPaymentDocument_ClientOrganizationPaymentFromClientEdit = {
    Init: function () {
        $(document).ready(function () {
            // Открытие формы выбора организации
            $("#clientOrganizationPaymentFromClientEdit #ClientOrganizationName").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Client/SelectAllClientOrganization",
                    success: function (result) {
                        $("#contractorOrganizationSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#contractorOrganizationSelector"));
                        ShowModal("contractorOrganizationSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageClientOrganizationPaymentFromClientEdit");
                    }
                });
            });

            // Обработка выбора организации клиента
            $("#contractorOrganizationSelector .linkOrganizationSelect").live("click", function () {
                var clientOrganizationId = $(this).parent("td").parent("tr").find(".organizationId").text();
                var clientOrganizationName = $(this).parent("td").parent("tr").find(".organizationShortName").text();

                $("#clientOrganizationPaymentFromClientEdit #ClientOrganizationName").text(clientOrganizationName);
                $("#clientOrganizationPaymentFromClientEdit #ClientOrganizationId").val(clientOrganizationId);

                HideModal();
            });
        });
    },

    OnBeginSelectDestinationDocumentsButtonClick: function () {
        StartButtonProgress($("#clientOrganizationPaymentFromClientEdit #btnSelectDestinationDocuments"));
    },

    OnFailSelectDestinationDocumentsButtonClick: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageClientOrganizationPaymentFromClientEdit");
    },

    OnSuccessSelectDestinationDocumentsButtonClick: function (ajaxContext) {
        $("#destinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution").html(ajaxContext);
        $.validator.unobtrusive.parse($("#destinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution"));
        ShowModal("destinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution");
    }
};