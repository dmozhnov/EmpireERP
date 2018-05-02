var AccountingPriceList_ArticlesGrid = {
    Init: function () {
        $(document).ready(function () {
            // Добавить элемент реестра (товар). Вызов первого модального окна
            $("#btnAddArticle").click(function () {
                var priceListId = $('#MainDetails_Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/AccountingPriceList/AddArticle",
                    data: { accountingPriceListId: priceListId },
                    success: function (result) {
                        $('#articleSelectList').hide().html(result);
                        $.validator.unobtrusive.parse($("#articleSelectList"));
                        ShowModal("articleSelectList");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountingPriceListDetailsArticleList");
                    }
                });
            });

            // Удаление товара из грида по ссылке
            $('#gridAccountingPriceArticles .delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var articlePrice_id = $(this).parent("td").parent("tr").find(".articlePriceId").text();
                    var priceListId = $('#MainDetails_Id').val();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/AccountingPriceList/DeleteArticle/",
                        data: { articleAccountingPriceId: articlePrice_id, accountingPriceListId: priceListId },
                        success: function (result) {
                            RefreshGrid("gridAccountingPriceArticles");
                            AccountingPriceList_Shared.RefreshMainDetails(result);
                            ShowSuccessMessage("Товар удален.", "messageAccountingPriceListDetailsArticleList");
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountingPriceListDetailsArticleList");
                        }
                    });
                }
            });

            // Редактирование товара из грида по ссылке
            $("#gridAccountingPriceArticles .edit_link, #gridAccountingPriceArticles .details_link").click(function () {
                var articlePriceId = $(this).parent("td").parent("tr").find(".articlePriceId").text();
                var priceListId = $('#MainDetails_Id').val();

                $.ajax({
                    type: "GET",
                    url: "/AccountingPriceList/EditArticle",
                    data: { accountingPriceListId: priceListId, articleAccountingPriceId: articlePriceId },
                    success: function (result) {
                        $('#articleSelectList').hide().html(result);
                        $.validator.unobtrusive.parse($("#articleSelectList"));
                        ShowModal("articleSelectList");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountingPriceListDetailsArticleList");
                    }
                });
            });

            $('#btnAddArticleAccountingPriceSet').live("click", function () {
                var id = $('#MainDetails_Id').val();
                window.location = "/AccountingPriceList/AddArticleAccountingPriceSet?id=" + id + GetBackUrl();
            });

        });             // document ready
    }
};