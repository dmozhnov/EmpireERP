var ProductionOrderMaterialsPackage_MaterialsPackageGrid = {
    Init: function () {
        $(document).ready(function () {

            var currentUrl = $("#currentUrl").val();
            $("#gridMaterialsPackage table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/ProductionOrderMaterialsPackage/Details?id=" + id + "&backURL=" + currentUrl);

                var productionOrderId = $(this).find(".ProductionOrderId").text();
                $(this).find("a.ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + productionOrderId + "&backURL=" + currentUrl);
            });

            $("#btnCreateMaterialsPackage").click(function () {
                window.location = "/ProductionOrderMaterialsPackage/Create?backURL=" + $("#currentUrl").val();
            });
        });
    }
};