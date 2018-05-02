var Storage_Grid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridStorage table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);
            });
        });
    }
};