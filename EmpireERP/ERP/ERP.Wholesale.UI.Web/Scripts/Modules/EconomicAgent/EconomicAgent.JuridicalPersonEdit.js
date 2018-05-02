var EconomicAgent_JuridicalPersonEdit = {
    Init: function () {
        $("#ShortName").focus();
        $('#juridicalPersonEditContactInfo').hide();       

        //Переходим на вкладку основной информации
        $('#linkMainInfo').click(function () {
            $(this).parent("div").find(".selected").removeClass("selected");
            $(this).addClass("selected");
            $('#juridicalPersonEditMainInfo').show();
            $('#juridicalPersonEditContactInfo').hide();

            $("#organizationEdit #ShortName").focus();
        });

        //Переходим на вкладку контакных лиц
        $('#linkContacts').click(function () {
            $(this).parent("div").find(".selected").removeClass("selected");
            $(this).addClass("selected");
            $('#juridicalPersonEditMainInfo').hide();
            $('#juridicalPersonEditContactInfo').show();

            $("#organizationEdit #DirectorName").focus();
        });
     },

     OnFailJuridicalPersonEdit:function(ajaxContext) {
         ShowErrorMessage(ajaxContext.responseText, "messageJuridicalPersonEdit");
     }
};