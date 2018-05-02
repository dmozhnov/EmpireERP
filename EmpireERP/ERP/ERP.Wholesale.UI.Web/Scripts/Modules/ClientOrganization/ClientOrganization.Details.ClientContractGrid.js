var ClientOrganization_Details_ClientContractGrid = {
    Init: function () {
        $(document).ready(function () {

            $("#gridClientContract table.grid_table tr").each(function (i, el) {
                var accountOrganizationId = $(this).find(".AccountOrganizationId").text();
                $(this).find("a.AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + accountOrganizationId + GetBackUrl());
            });

            $("#gridClientContract .linkClientContractEdit").click(function () {    
                var contractId = $(this).findCell(".Id").text();            
                $.ajax({
                    type: "GET",
                    url: "/ClientContract/EditContract",
                    data: { contractId: contractId },
                    success: function (result) {
                        $("#clientContractEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#clientContractEdit"));
                        ShowModal("clientContractEdit");
                        $("#clientContractEdit #Number").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageClientContractList");
                    }
                });
            });
            
        });
    }
}; 