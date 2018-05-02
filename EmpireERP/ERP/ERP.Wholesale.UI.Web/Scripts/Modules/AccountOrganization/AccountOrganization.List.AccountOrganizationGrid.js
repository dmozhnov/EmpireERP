var AccountOrganization_List_AccountOrganizationGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridAccountOrganization table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".accountOrganizationId").text();
                $(this).find("a.ShortName").attr("href", "/AccountOrganization/Details?id=" + id + "&backURL=" + currentUrl);
            });
        });
    }
}; 