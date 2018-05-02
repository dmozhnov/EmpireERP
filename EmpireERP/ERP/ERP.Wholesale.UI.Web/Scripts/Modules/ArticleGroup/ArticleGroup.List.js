var ArticleGroup_List = {
    Init: function () {
        $(document).ready(function () {
            scrollBy(50, 50);
        });

        // добавление группы товаров
        $("#btnAddToFirstLevel").live('click', function () {
            StartButtonProgress($(this));

            ShowArticleGroupDetailsForCreation(null);
        });

        $("#btnCreateSubGroup").live('click', function () {
            StartButtonProgress($(this));

            var parentId = $("#Id").val();

            ShowArticleGroupDetailsForCreation(parentId);
        });

        // выбор элемента дерева групп товаров
        $("#treeArticleGroups .tree_node_title").live('click', function () {
            var artGroup_id = $(this).next("input.value").val();
            var x = $(this).offset().left + $(this).width() + 50;
            var y = $(this).offset().top;

            StartLinkProgress($(this));

            $.ajax({
                type: "GET",
                url: "/ArticleGroup/Details/",
                data: { id: artGroup_id },
                success: function (result) {
                    $('#articleGroupDetails').hide().html(result);
                    ShowPopup("articleGroupDetails", x, y);

                    // запрет добавления в 3 уровень
                    if ($("#treeArticleGroups .tree_table input[value='" + artGroup_id + "']").closest(".tree_node").attr("level") < 2) {
                        EnableButton("btnCreateSubGroup");
                    }
                    else {
                        DisableButton("btnCreateSubGroup");
                    }
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleGroupList");

                    if ($(document).scrollTop() > $("#messageArticleGroupList").offset().top) {
                        scroll(0, $("#messageArticleGroupList").offset().top - 7);
                    }
                }
            });
        });

        $("#btnEditArticleGroup").live('click', function () {
            StartButtonProgress($(this));

            var artGroup_id = $("#Id").val();

            HidePopup();
            ShowArticleGroupDetailsForEdit(artGroup_id);
        });

        // получение информации о группе товаров для редактирования
        function ShowArticleGroupDetailsForEdit(artGroup_id) {
            $.ajax({
                type: "GET",
                url: "/ArticleGroup/Edit/",
                data: { id: artGroup_id },
                success: function (result) {
                    $('#articleGroupDetailsForEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleGroupDetailsForEdit"));
                    ShowModal("articleGroupDetailsForEdit");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleGroupList");

                    if ($(document).scrollTop() > $("#messageArticleGroupList").offset().top) {
                        scroll(0, $("#messageArticleGroupList").offset().top - 7);
                    }
                }
            });
        }

        function ShowArticleGroupDetailsForCreation(parentGroupId) {
            HidePopup();

            $.ajax({
                type: "GET",
                url: "/ArticleGroup/Create/",
                data: { parentGroupId: parentGroupId },
                success: function (result, textStatus, XMLHttpRequest) {
                    $('#articleGroupDetailsForEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleGroupDetailsForEdit"));
                    ShowModal("articleGroupDetailsForEdit");
                    $('#articleGroupDetailsForEdit #Name').focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleGroupList");

                    if ($(document).scrollTop() > $("#messageArticleGroupList").offset().top) {
                        scroll(0, $("#messageArticleGroupList").offset().top - 7);
                    }
                }
            });
        }

        // удаление группы товаров
        $("#btnDeleteArticleGroup").live("click", function () {                        
            var id = $("#Id").val();

            if (confirm('Вы уверены?')) {
                StartButtonProgress($(this));
                
                $.ajax({
                    type: "POST",
                    url: "/ArticleGroup/Delete/",
                    data: { id: id },
                    success: function (result) {
                        var node = $("#treeArticleGroups .tree_table input[value='" + id + "']").closest(".tree_node");
                        var parentNode = node.parent(".tree_node_childs").prev(".tree_node");

                        node.next(".tree_node_childs").remove();
                        node.remove();

                        // если у родителя больше не осталось дочерних элементов, то удаляем экспандер
                        if (parentNode.next(".tree_node_childs").find(".tree_node").length == 0) {
                            parentNode.find(".tree_node_expander").remove();
                            parentNode.css("padding-left", ((parentNode.attr("level") - 1) * 40 + 32) + "px");
                            parentNode.addClass("selected");
                        }

                        // если в списке не осталось ни одного элемента
                        if ($("#treeArticleGroups .tree_table .tree_node").length == 0) {
                            $("#treeArticleGroups .tree_table").append("<div class='no_data_row'>нет данных</div>");
                        }

                        ShowSuccessMessage("Группа товаров удалена.", "messageArticleGroupList");
                        HidePopup();

                        if ($(document).scrollTop() > $("#messageArticleGroupList").offset().top) {
                            scroll(0, $("#messageArticleGroupList").offset().top - 7);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleGroupList");

                        if ($(document).scrollTop() > $("#messageArticleGroupList").offset().top) {
                            scroll(0, $("#messageArticleGroupList").offset().top - 7);
                        }
                    }
                });
            }
        });
    },

    OnSuccessArticleGroupEdit: function (ajaxContext) {
        HideModal();
        ShowSuccessMessage("Сохранено.", "messageArticleGroupList");

        if ($(document).scrollTop() > $("#messageArticleGroupList").offset().top) {
            scroll(0, $("#messageArticleGroupList").offset().top - 7);
        }

        if ($("#treeArticleGroups .tree_table .no_data_row").length != 0) {
            $("#treeArticleGroups .tree_table .no_data_row").remove();
        }

        // если элемент был добавлен
        if (ajaxContext.IsNewArticleGroup.toString() == 'true') {
            $("#treeArticleGroups .tree_table").find(".selected").removeClass("selected");

            // если это элемент первого уровня
            if (ajaxContext.ParentId == null) {
                $("#treeArticleGroups .tree_table").append(
                    "<div class='tree_node selected' level='1' style='padding-left: 32px'>" +
                    "<span class='tree_node_title'>" + ajaxContext.Name + "</span>" +
                    "<input type='hidden' class='value' value='" + ajaxContext.Id + "'></div>" +
                    "<div class='tree_node_childs hidden'></div>");

                scroll(0, $("#treeArticleGroups .tree_table").find(".selected").offset().top);
            }
            else {
                // поиск родительского элемента
                var parentNode = $("#treeArticleGroups .tree_table input[value='" + ajaxContext.ParentId + "']").closest(".tree_node");
                var parentLevel = parentNode.attr("level");
                var parentChilds = parentNode.next(".tree_node_childs");

                parentChilds.append(
                    "<div class='tree_node selected' level='" + (parseInt(parentLevel) + 1) + "' style='padding-left: " + (parentLevel * 40 + 32) + "px'>" +
                    "<span class='tree_node_title'>" + ajaxContext.Name + "</span>" +
                    "<input type='hidden' class='value' value='" + ajaxContext.Id + "'></div>" +
                    "<div class='tree_node_childs hidden'></div>");

                scroll(0, $("#treeArticleGroups .tree_table").find(".selected").offset().top);

                // добавление экспандера
                if (parentNode.find(".tree_node_expander").length == 0) {
                    parentNode.find(".tree_node_title").before("<span class='tree_node_expander expanded'>&#9660;</span>");
                    parentNode.css("padding-left", ((parentNode.attr("level") - 1) * 40 + 10) + "px");
                }
                // если он есть - раскрываем его
                else {
                    parentNode.find(".tree_node_expander").addClass("expanded").html("&#9660;");
                }

                parentNode.next(".tree_node_childs").removeClass("hidden");
            }
        }
        // если элемент был отредактирован
        else {
            $("#treeArticleGroups .tree_table input[value='" + ajaxContext.Id + "']").closest("div").find(".tree_node_title").text(ajaxContext.Name);
        }
    }
};