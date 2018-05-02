var ReturnFromClientWaybill_RowEdit = {
    Init: function () {
        $(document).ready(function () {
            SetFieldScale("#ReturningCount", 12, $("#MeasureUnitScale").val(), "#returnFromClientWaybillRowEdit", true);
        });

        $("span#ArticleName.select_link").bind('click', function () {
            var dealId = $('#DealId').val();
            var teamId = $("#TeamId").val();
            var recipientId = $('#RecipientId').val();

            $.ajax({
                type: "GET",
                url: "/Article/SelectArticleToReturn/",
                data: { dealId: dealId, teamId: teamId, recipientId: recipientId, date: $("#ReturnFromClientWaybillDate").val() },
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    ReturnFromClientWaybill_RowEdit.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillRowEdit");
                }
            });
        });

        $("#SaleLink.select_link").click(function () {
            var currentArticleSaleId = null;
            if ($("#returnFromClientWaybillRowEdit #CurrentSaleWaybillRowId").val() != "00000000-0000-0000-0000-000000000000") {
                currentArticleSaleId = $("#returnFromClientWaybillRowEdit #CurrentSaleWaybillRowId").val();
            }
            ReturnFromClientWaybill_RowEdit.SelectArticleSale(currentArticleSaleId);
        });

        $("#ReturningCount").bind("keyup change paste cut", function () {
            ReturnFromClientWaybill_Shared.CheckSaveButtonAvailability();
        });
    },

    OnFailReturnFromClientWaybillRowEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageReturnFromClientWaybillRowEdit");
    },

    OnSuccessReturnFromClientWaybillRowEdit: function (ajaxContext) {
        if ($('#returnFromClientWaybillRowEdit #Id').val() != "00000000-0000-0000-0000-000000000000") {
            RefreshGrid("gridReturnFromClientWaybillRows", function () {
                RefreshGrid("gridArticleGroups", function () {
                    ReturnFromClientWaybill_Details.RefreshMainDetails(ajaxContext.MainDetails);
                    ReturnFromClientWaybill_Details.RefreshPermissions(ajaxContext.Permissions);
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageReturnFromClientWaybillRowList");
                    });
                });
            });
        }
        else {
            RefreshGrid("gridReturnFromClientWaybillRows", function () {
                RefreshGrid("gridArticleGroups", function () {
                    ReturnFromClientWaybill_RowEdit.ClearForm();
                    ReturnFromClientWaybill_Details.RefreshMainDetails(ajaxContext.MainDetails);
                    ReturnFromClientWaybill_Details.RefreshPermissions(ajaxContext.Permissions);
                    ShowSuccessMessage("Сохранено.", "messageReturnFromClientWaybillRowEdit");
                });
            });
        }
    },

    ClearForm: function () {
        $("#ArticleName").text("Выберите товар");
        $("#ArticleId").val("");
        $("#returnFromClientWaybillRowEdit #MeasureUnitName").text("");

        $("#returnFromClientWaybillRowEdit #SaleWaybillName").text("не выбран");
        $("#returnFromClientWaybillRowEdit #PurchaseCost").text("---");

        $("#returnFromClientWaybillRowEdit #TotalSoldCount").text("---");
        $("#returnFromClientWaybillRowEdit #AvailableToReturnCount").text("---");
        $("#returnFromClientWaybillRowEdit #ReturnedCount").text("---");

        $("#returnFromClientWaybillRowEdit #PurchaseCost").text("---");
        $("#returnFromClientWaybillRowEdit #SaleWaybillRowId").val("00000000-0000-0000-0000-000000000000");

        $("#returnFromClientWaybillRowEdit #AccountingPrice").text("---");
        $("#returnFromClientWaybillRowEdit #SalePrice").text("---");

        $("#returnFromClientWaybillRowEdit #ReturningCount").val("");
        $("#returnFromClientWaybillRowEdit #ReturningCount").removeClass("input-validation-error");
        $("#returnFromClientWaybillRowEdit #ReturningCount_validationMessage").hide();

        SetFieldScale("#ReturningCount", 12, 0, "#returnFromClientWaybillRowEdit", true);

        DisableButton("btnSaveReturnFromClientWaybillRow");
        $("#SaleLink").hide();
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        $("#gridSelectArticle .article_select_link").die();
        $("#gridSelectArticle .article_select_link").live("click", function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            $("#ArticleId").val($(this).parent("td").parent("tr").find(".articleId").text());
            $("#MeasureUnitName").text($(this).parent("td").parent("tr").find(".MeasureUnitShortName").text());

            $("#SaleLink").show();

            HideModal(function () {
                var currentArticleSaleId = null;
                if ($("#returnFromClientWaybillRowEdit #CurrentSaleWaybillRowId").val() != "00000000-0000-0000-0000-000000000000") {
                    currentArticleSaleId = $("#returnFromClientWaybillRowEdit #CurrentSaleWaybillRowId").val();
                }

                ReturnFromClientWaybill_RowEdit.SelectArticleSale(currentArticleSaleId);
            });
        });
    },

    BindArticleSaleSelection: function () {
        $("#gridSelectArticleSale .articleSale_select_link").die();
        $("#gridSelectArticleSale .articleSale_select_link").live("click", function () {
            var availableToReturnCount = $(this).parent("td").parent("tr").find(".AvailableToReturnCount").text().replaceAll(' ', '');
            var purchaseCost = $(this).parent("td").parent("tr").find(".PurchaseCost").text();
            var accountingPrice = $(this).parent("td").parent("tr").find(".AccountingPrice").text();

            $("#returnFromClientWaybillRowEdit #SaleWaybillName").text($(this).parent("td").parent("tr").find(".SaleWaybillName").text());
            $("#returnFromClientWaybillRowEdit #TotalSoldCount").text($(this).parent("td").parent("tr").find(".SoldCount").text());
            $("#returnFromClientWaybillRowEdit #AvailableToReturnCount").text(availableToReturnCount);
            $("#returnFromClientWaybillRowEdit #ReturnedCount").text($(this).parent("td").parent("tr").find(".ReturnedCount").text());
            $("#returnFromClientWaybillRowEdit #SaleWaybillRowId").val($(this).parent("td").parent("tr").find(".SaleWaybillRowId").text());
            $("#returnFromClientWaybillRowEdit #SalePrice").text($(this).parent("td").parent("tr").find(".SalePrice").text());

            var measureUnitScale = $(this).parent("td").parent("tr").find(".MeasureUnitScale").text();
            SetFieldScale("#ReturningCount", 12, measureUnitScale, "#returnFromClientWaybillRowEdit", true);

            if (IsTrue($("#AllowToViewPurchaseCost").val())) {
                $("#returnFromClientWaybillRowEdit #PurchaseCost").text(purchaseCost);
            }
            else {
                $("#returnFromClientWaybillRowEdit #PurchaseCost").text("---");
            }

            if (IsTrue($("#AllowToViewAccountingPrice").val())) {
                $("#returnFromClientWaybillRowEdit #AccountingPrice").text(accountingPrice);
            }
            else {
                $("#returnFromClientWaybillRowEdit #AccountingPrice").text("---");
            }

            HideModal(function () {
                $("#SaleLink").show();

                $("#returnFromClientWaybillRowEdit #ReturningCount").val("");
                $("#returnFromClientWaybillRowEdit #ReturningCount").focus();
            });
        });
    },

    SelectArticleSale: function (articleSaleToExcludeId) {
        $.ajax({
            type: "GET",
            url: "/Article/SelectArticleSale/",
            data: { articleId: $("#ArticleId").val(), dealId: $("#DealId").val(), teamId: $("#TeamId").val(), recipientId: $("#RecipientId").val(),
                date: $("#ReturnFromClientWaybillDate").val(), articleSaleToExcludeId: articleSaleToExcludeId, storageId: $("#RecipientStorageId").val()
            },
            success: function (result) {
                $('#articleSaleSelector').hide().html(result);
                $.validator.unobtrusive.parse($("#articleSaleSelector"));
                ShowModal("articleSaleSelector");
                ReturnFromClientWaybill_RowEdit.BindArticleSaleSelection();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                if ($("#returnFromClientWaybillRowEdit #CurrentSaleWaybillRowId").val() == "00000000-0000-0000-0000-000000000000") {
                    ReturnFromClientWaybill_RowEdit.ClearForm();
                }
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillRowEdit");
            }
        });
    }
};
