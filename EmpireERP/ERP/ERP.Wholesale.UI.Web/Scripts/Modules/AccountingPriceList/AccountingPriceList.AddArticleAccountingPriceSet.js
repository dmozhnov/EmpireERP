var AccountingPriceList_AddArticleAccountingPriceSet = {
    Init: function () {
        $(function () {

            $("#OnlyAvailability").prev(".yes_no_toggle").change(function () {
                if ($("#OnlyAvailability").val() == "0") {
                    $(".OnlyAvailabilityContainer").hide();
                }
                else {
                    $(".OnlyAvailabilityContainer").show();
                }
            });

            $("#btnBack").live("click", function () {
                window.location = $("#BackURL").val();
            });

            $('#btnAddArticleAccountingPriceSet').live('click', function () {
                if (IsFalse($("#multipleSelectorArticleGroups").CheckSelectedEntitiesCount("Не выбрано ни одной группы товаров.", "Выберите все группы товаров или не больше ", "messageAddArticleAccountingPriceSet"))) {
                    scroll(0, $("#messageAddArticleAccountingPriceSet").offset().top - 10);
                    return false;
                }
                if ($("#OnlyAvailability").val() == "1") {
                    if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.", "Выберите все места хранения или не больше ", "messageAddArticleAccountingPriceSet"))) {
                        scroll(0, $("#messageAddArticleAccountingPriceSet").offset().top - 10);
                        return false;
                    }
                }
                SetMultipleSelectorSelectedValues("multipleSelectorArticleGroups", "ArticleGroupsIDs", "AllArticleGroups");
                SetMultipleSelectorSelectedValues("multipleSelectorStorages", "StorageIDs", "AllStorages");
            });
        });
    },

    OnFailAddArticleAccountingPriceSet: function (ajaxContext) {
        scroll(0, $("#messageAddArticleAccountingPriceSet").offset().top - 10);
        ShowErrorMessage(ajaxContext.responseText, "messageAddArticleAccountingPriceSet");
    },

    OnSuccessAddArticleAccountingPriceSet: function (ajaxContext) {
            window.location = $("#BackURL").val();
    }
};