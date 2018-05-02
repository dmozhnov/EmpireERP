var User_UserSelector = {
    Init: function () {
        $("#gridSelectUser table.grid_table tr").each(function () {
            var id = $(this).find(".Id").text();
            $(this).find("a.Name").attr("href", "/User/Details?id=" + id + GetBackUrl());
        });
    }
};