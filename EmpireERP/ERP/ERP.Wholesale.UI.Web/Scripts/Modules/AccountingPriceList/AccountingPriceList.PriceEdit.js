var AccountingPriceList_PriceEdit = {
    Init: function () {
        $(document).ready(function () {
            $('#tbAccountingPrice').change(function () {
                $('#lblDefaultRuleError').removeClass('field-validation-error').addClass('field-validation-valid');
            });
        });           // document ready

        $("span#ArticleName.select_link").bind('click', function () {
            $.ajax({
                type: "GET",
                url: "/Article/SelectArticle/",
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    AccountingPriceList_PriceEdit.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleAccountingPriceEdit");
                }
            });
        });
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die("click");
        $("#gridSelectArticle .article_select_link").live('click', function () {
            // Установка полей с информацией о выбранном товаре
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            $("#ArticleId").val($(this).parent("td").parent("tr").find(".articleId").text());
            $("#ArticleIdForDisplay").text($(this).parent("td").parent("tr").find(".articleId").text());
            $("#ArticleNumber").text($(this).parent("td").parent("tr").find(".articleNumber").text());

            var accountingPriceListId = $('#AccountingPriceListId').val();
            var articleId = $('#ArticleId').val();
            $.ajax({
                type: "GET",
                url: "/AccountingPriceList/GetTipsForArticle/",
                data: { accountingPriceListId: accountingPriceListId, articleId: articleId },
                success: function (result) {
                    $('#AveragePurchaseCost').text(result.AveragePurchaseCost);
                    $('#MinPurchaseCost').text(result.MinPurchaseCost);
                    $('#MaxPurchaseCost').text(result.MaxPurchaseCost);
                    $('#LastPurchaseCost').text(result.LastPurchaseCost);
                    $('#AverageAccountingPrice').text(result.AverageAccountingPrice);
                    $('#MinAccountingPrice').text(result.MinAccountingPrice);
                    $('#MaxAccountingPrice').text(result.MaxAccountingPrice);
                    $('#DefaultMarkupPercent').text(result.DefaultMarkupPercent);
                    $("#AccountingPriceRule").text(result.AccountingPriceRule);

                    if (result.CalculatedAccountingPrice != "") {
                        var accPrice = result.CalculatedAccountingPrice.replaceAll(" ", "");
                        $('input#AccountingPrice').val(ValueForEdit(accPrice)).prev("span").text(ValueForDisplay(accPrice));
                    }
                    else {
                        $('input#AccountingPrice').val(0).prev("span").text(0);
                    }

                    $('#CalculatedAccountingPrice').prev("span").text(result.CalculatedAccountingPrice);
                    $('#CalculatedAccountingPrice').val(result.CalculatedAccountingPrice);

                    $('#AccountingPrice').focus();


                    var defaultRule = result.UsedDefaultRule;
                    if (defaultRule != 0) $('#lblDefaultRuleError').removeClass('field-validation-valid').addClass('field-validation-error')
                    else $('#lblDefaultRuleError').removeClass('field-validation-error').addClass('field-validation-valid');
                    HideModal();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleSelectList");
                }
            });
        });
    },


    // при неудачной попытке добавления/редактирования товара в реестре цен
    OnFailArticlePriceEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageArticleAccountingPriceEdit");
    },

    OnSuccessArticlePriceEdit: function (ajaxContext) {
        RefreshGrid("gridAccountingPriceArticles", function () {
            AccountingPriceList_Shared.RefreshMainDetails(ajaxContext);

            // после редактирования мы закрываем модальную форму
            if ($('#articleSelectList #Id').val() != "00000000-0000-0000-0000-000000000000") {
                HideModal(function () {
                    ShowSuccessMessage("Сохранено.", "messageAccountingPriceListDetailsArticleList");
                });
            }
            // а после добавления мы оставляем модальную форму, но очищаем ее, чтобы она была готова к новому добавлению
            else {
                $("#articleSelectList #ArticleId").val("0");
                $("#articleSelectList #ArticleIdForDisplay").text("");
                $("#ArticleName").text("Выберите товар");
                $("#articleSelectList #ArticleNumber").text("");
                $("#articleSelectList input#AccountingPrice").val("0").prev("span").text("---");
                $("#articleSelectList input#CalculatedAccountingPrice").val("0").prev("span").text("---");

                $('#AveragePurchaseCost').text("---");
                $('#MinPurchaseCost').text("---");
                $('#MaxPurchaseCost').text("---");
                $('#LastPurchaseCost').text("---");
                $('#AverageAccountingPrice').text("---");
                $('#MinAccountingPrice').text("---");
                $('#MaxAccountingPrice').text("---");
                $('#DefaultMarkupPercent').text("---");
                $("#AccountingPriceRule").text("---");

                ShowSuccessMessage("Товар добавлен.", "messageArticleAccountingPriceEdit");
            }
        });
    }
};