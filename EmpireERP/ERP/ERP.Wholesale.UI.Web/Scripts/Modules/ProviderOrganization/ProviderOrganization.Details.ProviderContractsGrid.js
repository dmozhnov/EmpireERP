var ProviderOrganization_Details_ProviderContractsGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridProviderContract table.grid_table tr").each(function () {
                var id = $(this).find(".ProviderId").text();
                $(this).find("a.ProviderName").attr("href", "/Provider/Details?id=" + id + GetBackUrl());

                id = $(this).find(".AccountOrganizationId").text();
                $(this).find("a.AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + id + GetBackUrl());
            });
        });
    }
};