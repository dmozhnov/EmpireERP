var Team_TeamsGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridTeams table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Team/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#btnCreateTeam").click(function () {
                window.location = "/Team/Create?backURL=" + $("#currentUrl").val();
            });
        });
    }
};