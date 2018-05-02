var ProductionOrder_ProducerContractEdit = {
    Init: function () {
        $(document).ready(function () {
            // Вывод модальной формы "Добавление собственной организации"
            $("#linkAccountOrganizationSelector").click(function () {
                if (IsTrue($("#producerContractEdit #AllowToChangeAccountOrganization").val())) {
                    var storageId = $("#main_page #StorageId").val();
                    $.ajax({
                        type: "GET",
                        url: "/AccountOrganization/SelectAccountOrganizationForStorage",
                        data: { storageId: storageId },
                        success: function (result) {
                            $("#accountOrganizationSelector").hide().html(result);
                            $.validator.unobtrusive.parse($("#accountOrganizationSelector"));
                            ShowModal("accountOrganizationSelector");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProducerContractEdit");
                        }
                    });
                }
            });

        });
    },

    OnFailContractEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProducerContractEdit");
    }

};