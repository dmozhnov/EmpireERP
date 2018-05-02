var ClientServiceProgram_ClientServiceProgramGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateClientServiceProgram').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                ClientServiceProgram_ClientServiceProgramGrid.ShowClientServiceProgramDetailsForEdit(id);
            });

            $('#gridClientServiceProgram .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                ClientServiceProgram_ClientServiceProgramGrid.ShowClientServiceProgramDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "ClientServiceProgram";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridClientServiceProgram", function () {
                                ShowSuccessMessage("Удалено.", "messageClientServiceProgramList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageClientServiceProgramList");
                        }
                    });
                }
            });
        });
    },

    ShowClientServiceProgramDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "ClientServiceProgram";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#clientServiceProgramEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#clientServiceProgramEdit"));
                ShowModal("clientServiceProgramEdit");
                $("#clientServiceProgramEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientServiceProgramList");
            }
        });
    }
};
