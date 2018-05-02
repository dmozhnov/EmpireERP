var ClientRegion_ClientRegionGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateClientRegion').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                ClientRegion_ClientRegionGrid.ShowClientRegionDetailsForEdit(id);
            });

            $('#gridClientRegion .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                ClientRegion_ClientRegionGrid.ShowClientRegionDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "ClientRegion";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridClientRegion", function () {
                                ShowSuccessMessage("Удалено.", "messageClientRegionList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageClientRegionList");
                        }
                    });
                }
            });
        });
    },

    ShowClientRegionDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "ClientRegion";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#clientRegionEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#clientRegionEdit"));
                ShowModal("clientRegionEdit");
                $("#clientRegionEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientRegionList");
            }
        });
    }
};
