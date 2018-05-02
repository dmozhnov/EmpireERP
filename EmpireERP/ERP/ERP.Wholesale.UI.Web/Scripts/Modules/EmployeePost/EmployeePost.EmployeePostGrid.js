var EmployeePost_EmployeePostGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateEmployeePost').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                EmployeePost_EmployeePostGrid.ShowEmployeePostDetailsForEdit(id);
            });

            $('#gridEmployeePost .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                EmployeePost_EmployeePostGrid.ShowEmployeePostDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "EmployeePost";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridEmployeePost", function () {
                                ShowSuccessMessage("Удалено.", "messageEmployeePostList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageEmployeePostList");
                        }
                    });
                }
            });
        });
    },

    ShowEmployeePostDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "EmployeePost";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#employeePostEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#employeePostEdit"));
                ShowModal("employeePostEdit");
                $("#employeePostEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageEmployeePostList");
            }
        });
    }
};
