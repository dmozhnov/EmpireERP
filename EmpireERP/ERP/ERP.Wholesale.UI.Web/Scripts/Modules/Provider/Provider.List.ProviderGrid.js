var Provider_List_ProviderGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            
            $("#gridProvider table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Provider/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $('#btnCreateProvider').click(function () {
                window.location = "/Provider/Create?" + GetBackUrl(true);
            });
        });
    }
};