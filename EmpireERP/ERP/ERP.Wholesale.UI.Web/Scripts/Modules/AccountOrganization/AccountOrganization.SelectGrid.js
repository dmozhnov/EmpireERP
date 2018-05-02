var AccountOrganization_SelectGrid = {
    Init: function () {
        $(document).ready(function () {
            // Действия после выбора организации из грида (ссылка "Выбрать")
            $(".linkAccountOrganizationSelect").click(function () {
                var accountOrganizationId = $(this).parent("td").parent("tr").find(".accountOrganizationId").text();
                var accountOrganizationShortName = $(this).parent("td").parent("tr").find(".accountOrganizationShortName").text();
                OnAccountOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName);
            });
        });
     }
};