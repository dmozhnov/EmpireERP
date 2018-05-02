var User_ActiveUsersGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridActiveUsers table.grid_table tr").each(function (i, el) {
                var userId = $(this).find(".Id").text();
                $(this).find("a.DisplayName").attr("href", "/User/Details?id=" + userId + "&backURL=" + currentUrl);

                var teamId = $(this).find(".TeamId").text();
                $(this).find("a.TeamNames").attr("href", "/Team/Details?id=" + teamId + "&backURL=" + currentUrl);

                var roleId = $(this).find(".RoleId").text();
                $(this).find("a.RoleNames").attr("href", "/Role/Details?id=" + roleId + "&backURL=" + currentUrl);
            });

            $("#btnCreateUser").click(function () {
                window.location = "/User/Create?backURL=" + $("#currentUrl").val();
            });
        });
    }
};

