var ClientContract_SelectGrid = {
    Init: function () {
        $(document).ready(function () {

            $("#gridSelectClientContract table.grid_table tr").each(function (i, el) {
                var accountOrganizationId = $(this).find(".AccountOrganizationId").text();
                $(this).find("a.AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + accountOrganizationId + GetBackUrl());

                var clientOrganizationId = $(this).find(".ClientOrganizationId").text();
                $(this).find("a.ClientOrganizationName").attr("href", "/ClientOrganization/Details?id=" + clientOrganizationId + GetBackUrl());
            });

            $("#clientContractSelector .linkClientContractSelect").click(function () {
                var contractName = $(this).findCell(".Name").text();
                var contractId = $(this).findCell(".Id").text();
                var accountOrganizationId = $(this).findCell(".AccountOrganizationId").text();
                var clientOrganizationId = $(this).findCell(".ClientOrganizationId").text();
                var accountOrganizationName = $(this).findCell(".AccountOrganizationName").text();
                var clientOrganizationName = $(this).findCell(".ClientOrganizationName").text();

                OnClientContractSelectLinkClick(contractName, contractId, accountOrganizationName, accountOrganizationId, clientOrganizationName, clientOrganizationId);
            });
        });
    }
};