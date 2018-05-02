var ContractorOrganization_Selector = {
    Init: function () {
        $(document).ready(function () {
            $("#linkAddOrganization").click(function () {
                var contractorId = $("#ContractorId").val();
                var url = "/" + $("#contractorOrganizationSelector #controllerName").val() + "/" + $("#contractorOrganizationSelector #actionName").val();

                $.ajax({
                    type: "GET",
                    url: url,
                    data: { contractorId: contractorId },
                    success: function (result) {
                        $('#economicAgentEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#economicAgentEdit"));
                        ShowModal("economicAgentEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageOrganizationSelectList");
                    }
                });
            });
        });
    }
};