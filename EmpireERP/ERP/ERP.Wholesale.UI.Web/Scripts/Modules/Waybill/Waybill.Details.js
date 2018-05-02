var Waybill_Details = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            //Работа с табами
            $(".tab_rows").click(function () {
                var _this = $(this);
                StartLinkProgress(_this.children("span"));

                _this.next().removeClass("selected");
                _this.addClass("selected");

                $(".tab_rows").parents(".tabPanel_with_grids_container").find(".gridContainer_rows").show();
                _this.parents(".tabPanel_with_grids_container").find(".gridContainer_articleGroups").hide();
                StopLinkProgress(_this.children("span"));
            });

            $(".tab_articleGroups").click(function () {
                var _this = $(this);
                StartLinkProgress(_this.children("span"));

                //если первый раз нажали на этот таб - то подгружаем таблицу
                var articleGroupTableShowed = _this.parents(".tabPanel_with_grids_container").find(".article_group_showed");

                if (IsFalse(articleGroupTableShowed.val())) {
                    articleGroupTableShowed.val('1'); //ставим пометку что таблица подгружена
                    var idGrid = _this.parents(".tabPanel_with_grids_container").find(".gridContainer_articleGroups").find(".grid").attr("id");

                    RefreshGrid(idGrid, function () {
                        ShowArticleGroupGrid(_this)
                    });
                }
                else {
                    ShowArticleGroupGrid(_this);
                }
            });
        });
    },

    // обработка выбора куратора
    HandlerForSelectCurator: function (waybillTypeId, select_user_action_link) {
        var userId = select_user_action_link.parent("td").parent("tr").find(".Id").text();
        var userName = select_user_action_link.parent("td").parent("tr").find(".Name").text();
        var waybillId = $("#Id").val();
        var url = "";

        switch (waybillTypeId) {
            case 1:
                url = "/ReceiptWaybill";
                break;
            case 2:
                url = "/MovementWaybill";
                break;
            case 3:
                url = "/WriteoffWaybill";
                break;
            case 4:
                url = "/ExpenditureWaybill";
                break;
            case 5:
                url = "/ChangeOwnerWaybill";
                break;
            case 6:
                url = "/ReturnFromClientWaybill";
                break;
        }

        $.ajax({
            type: "POST",
            url: url + "/ChangeCurator",
            data: { waybillId: waybillId, curatorId: userId },
            success: function (result) {
                var currentUrl = $("#currentUrl").val();
                $("#CuratorId").val(userId);
                $("#CuratorName").text(userName).attr("href", "/User/Details?id=" + userId + "&backURL=" + currentUrl)
                            .removeClass("disabled");

                HideModal();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageUserSelectList");
            }
        });
    }
};

function ShowArticleGroupGrid(_this) {
    _this.prev().removeClass("selected");
    _this.addClass("selected");

    _this.parents(".tabPanel_with_grids_container").find(".gridContainer_rows").hide();
    _this.parents(".tabPanel_with_grids_container").find(".gridContainer_articleGroups").show();
    StopLinkProgress(_this.children("span"));
};
