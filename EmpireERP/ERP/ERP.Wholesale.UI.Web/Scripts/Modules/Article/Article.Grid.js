var Article_Grid = {
    Init: function () {
        $(document).ready(function () {
            $('#btnCreateArticle').click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/Article/Create/",
                    success: function (result) {
                        $("#articleEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#articleEdit"));
                        ShowModal("articleEdit");
                        $("#articleEdit #number").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleList");
                    }
                });
            });

            $('.copy_link').click(function () {
                var articleId = $(this).parent("td").parent("tr").find(".Id").text();
                Article_Grid.ShowArticleDetailsForCopy(articleId);
            });

            $('.edit_link').click(function () {
                var articleId = $(this).parent("td").parent("tr").find(".Id").text();
                Article_Grid.ShowArticleForEdit(articleId);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    StartGridProgress($(this).closest(".grid"));

                    var articleId = $(this).parent("td").parent("tr").find(".Id").text();
                    Article_Grid.DeleteArticle(articleId);
                }
            });
        });
    },

    ShowArticleDetailsForCopy: function (id) {
        $.ajax({
            type: "GET",
            url: "/Article/Copy/",
            data: { articleId: id },
            success: function (result) {
                $("#articleEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#articleEdit"));
                ShowModal("articleEdit");
                $("#articleEdit #number").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleList");
            }
        });
    },

    ShowArticleForEdit: function (id) {
        $.ajax({
            type: "GET",
            url: "/Article/Edit/",
            data: { id: id },
            success: function (result) {
                $("#articleEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#articleEdit"));
                ShowModal("articleEdit");
                $("#articleEdit #number").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleList");
            }
        });
    },

    DeleteArticle: function (id) {
        $.ajax({
            type: "POST",
            url: "/Article/Delete/",
            data: { id: id },
            success: function (result) {
                RefreshGrid("gridArticles", function () {
                    ShowSuccessMessage("Товар удален.", "messageArticleList");
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleList");
            }
        });
    }
};