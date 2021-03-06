﻿var Article_AddRowsByList = {
    Init: function () {
        $(document).ready(function () {
            $("#ArticleGroup").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ArticleGroup/SelectArticleGroup/",
                    success: function (result) {
                        $('#articleGroupFilterSelector').hide().html(result);
                        ShowModal("articleGroupFilterSelector");
                        $('#articleGroupFilterSelector .attention').hide();
                        $("#articleGroupFilterSelector").css("top", "50px");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleList");
                    }
                });
            });

            $("#articleGroupFilterSelector .tree_node_title").live('click', function () {
                var articleGroupId = $(this).next("input.value").val();
                $("#ArticleGroup").attr("selected_id", articleGroupId);
                $("#ArticleGroup").text($(this).text());
                HideModal();
            });

            $("#btnBack").live('click', function () {
                window.location = $('#BackURL').val();
            });
        });
    }
};
