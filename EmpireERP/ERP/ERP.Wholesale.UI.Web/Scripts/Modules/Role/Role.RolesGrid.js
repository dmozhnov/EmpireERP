var Role_RolesGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridRoles table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Role/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#btnCreateRole").click(function () {
                window.location = "/Role/Create?backURL=" + $("#currentUrl").val();
            });
        });
    }
};