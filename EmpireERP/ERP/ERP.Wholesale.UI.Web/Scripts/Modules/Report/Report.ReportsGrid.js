var Report_ReportsGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridReports table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".ReportId").text();
                $(this).find("a.Name").attr("href", "/Report/" + id + "Settings" + "?backURL=" + currentUrl);
            });
        });
    }
};