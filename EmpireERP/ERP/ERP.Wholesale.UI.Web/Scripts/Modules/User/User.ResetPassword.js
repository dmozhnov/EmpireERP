var User_ResetPassword = {
    OnFailResetPassword: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageUserResetPassword");
    }
};