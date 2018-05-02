var Article_Selector = {
    Init: function () {
        $("#ArticleGroup").click(function () {
            $.ajax({
                type: "GET",
                url: "/ArticleGroup/SelectArticleGroup/",
                success: function (result) {
                    $('#articleGroupFilterSelector').hide().html(result);
                    ShowModal("articleGroupFilterSelector");
                    $('#articleGroupFilterSelector .attention').hide();
                    $("#articleGroupFilterSelector").css("top", "50px");

                    $("#articleGroupFilterSelector .tree_node_title").bind('click', function () {
                        var articleGroupId = $(this).next("input.value").val();
                        $("#ArticleGroup").attr("selected_id", articleGroupId);
                        $("#ArticleGroup").text($(this).text());
                        HideModal();
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleSelectList");
                }
            });
        });
    }
};