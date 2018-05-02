var RegisterEasy = {
    Init: function () {
        $(function () {
            $('#AdminLastName').focus();
        });
    },

    OnBeginUserRegistration: function () {
        $('#messageUserRegistration').text('').hide();
        StartButtonProgress($('#btnRegister'));
    },

    OnSuccessUserRegistration: function (accountNumber) {
        // сразу пытаемся залогиниться        
        $("#loginForm #AccountNumber").val(accountNumber);
        $("#loginForm #Login").val($("#free_registration_form #AdminLogin").val());
        $("#loginForm #Password").val($("#free_registration_form #AdminPassword").val());
        $("#loginForm").trigger("submit");
    },

    OnFailUserRegistration: function (ajaxContext) {
        $('#messageUserRegistration').text(ajaxContext.responseText).show();
        StopButtonProgress();
    }
};