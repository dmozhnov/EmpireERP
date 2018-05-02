var ContractorOrganization_SelectGrid = {
    Init: function () {
        $(document).ready(function () {
            // Действия после выбора организации из грида (ссылка "Выбрать")
            $("#contractorOrganizationSelector .linkOrganizationSelect").click(function () {
                var organizationId = $(this).parent("td").parent("tr").find(".organizationId").text();
                var organizationShortName = $(this).parent("td").parent("tr").find(".organizationShortName").text();
                OnContractorOrganizationSelectLinkClick(organizationId, organizationShortName);
            });
        });
    }
};