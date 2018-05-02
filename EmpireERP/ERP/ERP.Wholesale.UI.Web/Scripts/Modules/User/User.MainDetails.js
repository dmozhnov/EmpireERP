var User_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            $(".main_details_table #IsBlockedText.select_link").click(function () {

                var operation = "Block";
                var confirmMessage = "Вы действительно хотите заблокировать пользователя?";
                var successMessage = "Пользователь заблокирован.";

                if ($("#IsBlocked").val() == "1") {
                    operation = "UnBlock";
                    confirmMessage = "Вы действительно хотите разблокировать пользователя?";
                    successMessage = "Пользователь разблокирован.";
                }

                if (confirm(confirmMessage)) {
                    $.ajax({
                        type: "POST",
                        url: "/User/" + operation + "/",
                        data: { id: $("#Id").val() },
                        success: function (result) {
                            ShowSuccessMessage(successMessage, "messageUserEdit");
                            User_MainDetails.RefreshMainDetails(result.MainDetails);
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserEdit");
                        }
                    });
                }
            });

            //Открытие окна смены пароля
            $("#linkChangePassword").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/User/ChangePassword/",                    
                    success: function (result) {
                        $('#userChangePassword').hide().html(result);                        
                        $.validator.unobtrusive.parse($("#userChangePassword"));
                        ShowModal("userChangePassword");
                        $('#CurrentPassword').focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageUserEdit");
                    }
                });
            });

            //Открытие окна сброса пароля
            $("#linkResetPassword").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/User/ResetPassword/",
                    data: { userId: $("#Id").val() },
                    success: function (result) {
                        $('#userResetPassword').hide().html(result);                        
                        $.validator.unobtrusive.parse($("#userResetPassword"));
                        ShowModal("userResetPassword");
                        $('#NewPassword').focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageUserEdit");
                    }
                });
            });

        });
    },

    // обновление основной информации
    RefreshMainDetails: function (details) {
        $("#IsAdmin").val(details.IsAdmin);
        $("#IsAdminText").text(details.IsAdminText);
        $("#IsBlocked").val(details.IsBlocked);
        $("#IsBlockedText").text(details.IsBlockedText);

        $("#LastName").text(details.LastName);
        $("#CreationDate").text(details.CreationDate);
        $("#FirstName").text(details.FirstName);
        $("#Patronymic").text(details.Patronymic);
        $("#DisplayName").text(details.DisplayName);
        $("#RoleCount").text(details.RoleCount);
        $("#PostName").text(details.PostName);
        $("#TeamCount").text(details.TeamCount);
        $("#Login").text(details.Login);
        $("#PasswordHash").text(details.PasswordHash);
    },

    OnSuccessChangePassword: function () {
        HideModal();
        ShowSuccessMessage("Пароль изменен.", "messageUserEdit");
    },    

    OnBeginResetPassword: function (){
        StartButtonProgress($("#btnResetPassword"));
    },

    OnSuccessResetPassword: function () {
        HideModal();
        ShowSuccessMessage("Пароль изменен.", "messageUserEdit");
    }
};