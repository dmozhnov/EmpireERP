var Producer_ProducersGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridProducers table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Producer/Details?id=" + id + GetBackUrl());
            });

            $("#btnCreateProducer").click(function () {
                window.location = "/Producer/Create?backURL=" + $("#currentUrl").val();
            });
        });               
    }
};