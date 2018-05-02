var Team_UsersGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridUsers table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".UserId").text();
                $(this).find("a.UserName").attr("href", "/User/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#btnAddUser").click(function () {
                StartButtonProgress($(this));
                $.ajax({
                    url: "/User/SelectUserByTeam",
                    data: { teamId: $("#Id").val() },
                    success: function (result) {
                        $("#userSelector").hide().html(result);
                        ShowModal("userSelector");

                        BindUserSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageUserList");
                    }
                });
            });

            $("#gridUsers .remove_user").click(function () {
                if (confirm("Вы уверены?")) {
                    var userId = $(this).parent("td").parent("tr").find(".UserId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Team/RemoveUser",
                        data: { teamId: $("#Id").val(), userId: userId },
                        success: function (result) {
                            RefreshGrid("gridUsers", function () {
                                Team_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Пользователь исключен из команды.", "messageUserList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserList");
                        }
                    });
                }
            });
        });

        function BindUserSelection() {
            $("#gridSelectUser .select_user").die("click");
            $("#gridSelectUser .select_user").live('click', function () {
                var userId = $(this).parent("td").parent("tr").find(".Id").text();

                $.ajax({
                    type: "POST",
                    url: "/Team/AddUser",
                    data: { teamId: $("#Id").val(), userId: userId },
                    success: function (result) {
                        HideModal(function () {
                            RefreshGrid("gridUsers", function () {
                                Team_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Пользователь добавлен в команду.", "messageUserList");
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        HideModal(function () {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserList");
                        });
                    }
                });
            });
        }
    }
};