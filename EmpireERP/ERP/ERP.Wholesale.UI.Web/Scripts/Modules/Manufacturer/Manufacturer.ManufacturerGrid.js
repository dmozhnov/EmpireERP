var Manufacturer_ManufacturerGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateManufacturer').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                Manufacturer_ManufacturerGrid.ShowManufacturerDetailsForEdit(id);
            });

            $('#gridManufacturer .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                Manufacturer_ManufacturerGrid.ShowManufacturerDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "Manufacturer";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridManufacturer", function () {
                                ShowSuccessMessage("Удалено.", "messageManufacturerList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageManufacturerList");
                        }
                    });
                }
            });
        });
    },

    ShowManufacturerDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "Manufacturer";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#manufacturerEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#manufacturerEdit"));
                ShowModal("manufacturerEdit");
                $("#manufacturerEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageManufacturerList");
            }
        });
    }
};
