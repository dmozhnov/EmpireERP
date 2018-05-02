var User_UserTeamsGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridUserTeams table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".TeamId").text();
                $(this).find("a.TeamName").attr("href", "/Team/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#btnAddUserTeam").click(function () {
                StartButtonProgress($(this));
                $.ajax({
                    url: "/Team/SelectTeam",
                    data: { userId: $("#Id").val() },
                    success: function (result) {
                        $("#teamSelector").hide().html(result);
                        ShowModal("teamSelector");

                        BindTeamSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageUserEdit");
                    }
                });
            });

            $("#gridUserTeams .remove_team").click(function () {
                if (confirm("Вы уверены?")) {
                    var teamId = $(this).parent("td").parent("tr").find(".TeamId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/User/RemoveTeam",
                        data: { userId: $("#Id").val(), teamId: teamId },
                        success: function (result) {
                            RefreshGrid("gridUserTeams", function () {
                                User_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Пользователь исключен из команды.", "messageUserTeamList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserTeamList");
                        }
                    });
                }
            });
        });

        function BindTeamSelection() {
            $("#gridSelectTeam .select_team").die("click");
            $("#gridSelectTeam .select_team").live('click', function () {
                var teamId = $(this).parent("td").parent("tr").find(".Id").text();

                $.ajax({
                    type: "POST",
                    url: "/User/AddTeam",
                    data: { userId: $("#Id").val(), teamId: teamId },
                    success: function (result) {
                        HideModal(function () {
                            RefreshGrid("gridUserTeams", function () {
                                User_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Пользователь добавлен в команду.", "messageUserTeamList");
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        HideModal(function () {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserTeamList");
                        });
                    }
                });
            });
        }
    }
};

