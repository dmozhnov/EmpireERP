var Article_List = {
    Init: function () {
        $(document).ready(function () {

            $("#ArticleGroup").live("click", function () {
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

            $("#articleGroupFilterSelector .tree_node_title").live("click", function () {
                var articleGroupId = $(this).next("input.value").val();
                $("#ArticleGroup").attr("selected_id", articleGroupId);
                $("#ArticleGroup").text($(this).text());
                HideModal();
            });

            $("#btnCreateArticle").live("click", function () {
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

            $("#gridActualArticles .edit_link, #gridObsoleteArticles .edit_link").live("click", function () {
                var articleId = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/Article/Edit/",
                    data: { id : articleId },
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

            $("#gridActualArticles .copy_link, #gridObsoleteArticles .copy_link").live("click", function () {
                var articleId = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/Article/Copy/",
                    data: { articleId : articleId },
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

            $("#gridActualArticles .delete_link").live("click", function () {
                if (confirm("Вы уверены?")) {
                    var articleId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Article/Delete/",
                        data: { id : articleId },
                        success: function (result) {
                            RefreshGrid("gridActualArticles", function () { ShowSuccessMessage("Товар удален.", "messageActualArticleList"); });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageActualArticleList");
                        }
                    });
                }
            });

            $("#gridObsoleteArticles .delete_link").live("click", function () {
                if (confirm("Вы уверены?")) {
                    var articleId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Article/Delete/",
                        data: { id : articleId },
                        success: function (result) {
                            RefreshGrid("gridObsoleteArticles", function () { ShowSuccessMessage("Товар удален.", "messageObsoleteArticleList"); });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageObsoleteArticleList");
                        }
                    });
                }
            });
        });
    },

    OnSuccessSaveArticle: function (ajaxContext) {
        if (!ajaxContext.IsObsolete) {
            RefreshGrid("gridActualArticles", function () {
                RefreshGrid("gridObsoleteArticles", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageActualArticleList");
                    });
                });
            });
        }
        else {
            RefreshGrid("gridActualArticles", function () {
                RefreshGrid("gridObsoleteArticles", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageObsoleteArticleList");
                    });
                });
            });
        }
    }
}; 
