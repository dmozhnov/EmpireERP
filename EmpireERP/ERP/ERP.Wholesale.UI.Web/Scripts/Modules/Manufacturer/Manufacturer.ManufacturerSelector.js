var Manufacturer_ManufacturerSelector = {
    Init: function () {
        $(document).ready(function () {
            $("#addNewManufacturerLink").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Manufacturer/Create/",
                    data: { producerId: $("#ProducerId").val() },
                    success: function (result) {
                        $("#addNewManufacturer").hide().html(result);
                        $.validator.unobtrusive.parse($("#addNewManufacturer"));
                        ShowModal("addNewManufacturer");
                        $("#addNewManufacturer #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAddNewManufacturer");
                    }
                });
            });
        });
    }
};
