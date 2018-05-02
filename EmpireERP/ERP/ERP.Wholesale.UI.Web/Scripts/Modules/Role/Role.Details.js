var Role_Details = {
    Init: function () {
        $('#btnEdit').live("click", function () {
            var id = $('#Id').val();
            window.location = "/Role/Edit?id=" + id + GetBackUrl();
        });

        $('#btnDelete').live("click", function () {
            if (confirm('Вы уверены?')) {
                var id = $('#Id').val();

                $.ajax({
                    type: "POST",
                    url: "/Role/Delete/",
                    data: { roleId: id },
                    success: function () {
                        window.location = "/Role/List";
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRoleEdit");
                    }
                });
            }
        });

        $("#btnBackTo").live('click', function () {
            window.location = $('#BackURL').val();
        });

        $("#commonTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetCommonPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });

        $("#productionTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetProductionPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });

        $("#articleDistributionTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetArticleDistributionPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });

        $("#salesTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetSalesPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });

        $("#taskTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetTaskDistributionPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });

        $("#reportsTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetReportsPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });

        $("#directoriesTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetDirectoriesPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });

        $("#usersTab").live("click", function () {
            $("#tabPanel_menu_container div").removeClass("selected");
            $(this).addClass("selected");

            var id = $('#Id').val();

            $.ajax({
                url: "/Role/GetUsersPermissions/",
                data: { roleId: id },
                success: function (result) {
                    $("#permissionGroupContainer").html(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageRolePermissionEdit");
                }
            });
        });
    }
};