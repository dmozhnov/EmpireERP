var ContractorOrganization_Grid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridContractorOrganization table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                var typeId = $(this).find(".TypeId").text();
                var controllerName = "";

                if (typeId == "2") {
                    controllerName = "ProviderOrganization";
                }
                if (typeId == "3") {
                    controllerName = "ClientOrganization";
                }

                $(this).find("a.ShortName").attr("href", "/" + controllerName + "/Details?id=" + id + GetBackUrl());
            });
        });
    }
};