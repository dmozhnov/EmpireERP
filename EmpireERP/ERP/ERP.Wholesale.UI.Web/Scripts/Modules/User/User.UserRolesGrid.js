var User_UserRolesGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridUserRoles table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".RoleId").text();
                $(this).find("a.RoleName").attr("href", "/Role/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#btnAddUserRole").click(function () {
                StartButtonProgress($(this));
                $.ajax({
                    url: "/Role/SelectRole",
                    data: { userId: $("#Id").val() },
                    success: function (result) {
                        $("#roleSelector").hide().html(result);
                        ShowModal("roleSelector");

                        BindRoleSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageUserEdit");
                    }
                });
            });

            $("#gridUserRoles .remove_role").click(function () {
                if (confirm("Вы уверены?")) {
                    var roleId = $(this).parent("td").parent("tr").find(".RoleId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/User/RemoveRole",
                        data: { userId: $("#Id").val(), roleId: roleId },
                        success: function (result) {
                            RefreshGrid("gridUserRoles", function () {
                                User_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Роль удалена.", "messageUserRoleList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserRoleList");
                        }
                    });
                }
            });
        });

        function BindRoleSelection() {
            $("#gridSelectRole .select_role").die("click");
            $("#gridSelectRole .select_role").live('click', function () {
                var roleId = $(this).parent("td").parent("tr").find(".Id").text();

                $.ajax({
                    type: "POST",
                    url: "/User/AddRole",
                    data: { userId: $("#Id").val(), roleId: roleId },
                    success: function (result) {
                        HideModal(function () {
                            RefreshGrid("gridUserRoles", function () {
                                User_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Роль добавлена.", "messageUserRoleList");
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        HideModal(function () {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserRoleList");
                        });
                    }
                });
            });
        } 
    }
};

