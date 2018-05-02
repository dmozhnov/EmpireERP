var User_ChangePassword = {
    OnFailChangePassword: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageUserChangePassword");
    }
};

