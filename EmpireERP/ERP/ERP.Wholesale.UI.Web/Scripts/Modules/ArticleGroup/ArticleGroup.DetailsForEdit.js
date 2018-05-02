var ArticleGroup_DetailsForEdit = {
    Init: function() {
        $(document).ready(function () {
            $("#Name").bind('blur', function () {
                if ($("#NameFor1C").val() == '') {
                    var name = $("#Name").val();
                    $("#NameFor1C").val(name);
                 }
            });
        });
    },
    OnFailArticleGroupEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageArticleGroupEdit");
    }
};
