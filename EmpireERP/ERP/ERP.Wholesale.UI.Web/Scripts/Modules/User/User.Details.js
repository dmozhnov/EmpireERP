var User_Details = {
    Init: function () {
        $('#btnEdit').live("click", function () {
            var id = $('#Id').val();
            window.location = "/User/Edit?id=" + id + GetBackUrl();
        });

        $("#btnBackTo").live('click', function () {
            window.location = $('#BackURL').val();
        });
    }
};