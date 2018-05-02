var ProviderType_ProviderTypeGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateProviderType').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                ProviderType_ProviderTypeGrid.ShowProviderTypeDetailsForEdit(id);
            });

            $('#gridProviderType .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                ProviderType_ProviderTypeGrid.ShowProviderTypeDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "ProviderType";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridProviderType", function () {
                                ShowSuccessMessage("Удалено.", "messageProviderTypeList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderTypeList");
                        }
                    });
                }
            });
        });
    },

    ShowProviderTypeDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "ProviderType";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#providerTypeEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#providerTypeEdit"));
                ShowModal("providerTypeEdit");
                $("#providerTypeEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderTypeList");
            }
        });
    }
};
