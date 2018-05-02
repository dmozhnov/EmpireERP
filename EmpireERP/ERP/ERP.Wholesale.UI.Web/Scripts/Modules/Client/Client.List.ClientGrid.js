var Client_List_ClientGrid = {
    Init: function () {
        $(document).ready(function () {

            $("#gridClient table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Client/Details?id=" + id + GetBackUrl());
            });

            $('#btnCreateClient').click(function () {
                window.location = "/Client/Create?" + GetBackUrl(true);
            });
        });
    }
};