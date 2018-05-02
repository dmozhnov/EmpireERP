var ClientType_ClientTypeGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateClientType').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                ClientType_ClientTypeGrid.ShowClientTypeDetailsForEdit(id);
            });

            $('#gridClientType .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                ClientType_ClientTypeGrid.ShowClientTypeDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "ClientType";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridClientType", function () {
                                ShowSuccessMessage("Удалено.", "messageClientTypeList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageClientTypeList");
                        }
                    });
                }
            });
        });
    },

    ShowClientTypeDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "ClientType";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#clientTypeEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#clientTypeEdit"));
                ShowModal("clientTypeEdit");
                $("#clientTypeEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientTypeList");
            }
        });
    }
};
