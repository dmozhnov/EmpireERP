var Client_DealGrid = {
    Init: function () {
        $(document).ready(function () {

            $("#gridClientDeal table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Deal/Details?id=" + id + GetBackUrl());

                var clientOrgId = $(this).find(".ClientOrganizationId").text();
                $(this).find("a.ClientOrganizationName").attr("href", "/ClientOrganization/Details?id=" + clientOrgId + GetBackUrl());

                var accountOrgId = $(this).find(".AccountOrganizationId").text();
                $(this).find("a.AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + accountOrgId + GetBackUrl());
            });

            $('#btnCreateDeal').click(function () {
                window.location = "/Deal/Create?ClientId=" + $('#Id').val() + GetBackUrl();
            });
        });
    }
};