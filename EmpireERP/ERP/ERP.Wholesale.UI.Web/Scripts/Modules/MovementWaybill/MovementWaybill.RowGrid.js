var MovementWaybill_RowGrid = {
    Init: function () {
        $(document).ready(function () {

            // добавление позиции
            $("#btnAddMovementWaybillRow").click(function () {
                var movementWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/AddRow",
                    data: { movementWaybillId: movementWaybillId },
                    success: function (result) {
                        $('#movementWaybillRowEdit').hide().html(result);
                        ShowModal("movementWaybillRowEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillRowList");
                    }
                });
            });

            // редактирование / просмотр позиции
            $("#gridMovementWaybillRows .edit_link, #gridMovementWaybillRows .details_link").click(function () {
                var movementWaybillId = $('#Id').val();
                var movementWaybillRowId = $(this).parent("td").parent("tr").find(".movementWaybillRowId").text();

                $.ajax({
                    type: "GET",
                    url: "/MovementWaybill/EditRow",
                    data: { movementWaybillId: movementWaybillId, movementWaybillRowId: movementWaybillRowId },
                    success: function (result) {
                        $('#movementWaybillRowEdit').hide().html(result);
                        ShowModal("movementWaybillRowEdit");

                        if ($("#movementWaybillRowEdit #ManualSourcesInfo").val() == "") {
                            $("#BatchLink").show();
                        }
                        else {
                            $("#ManualSourcesLink").show();
                        }
                        $("#movementWaybillRowEdit #MovingCount").focus();
                        MovementWaybill_Shared.CheckSaveButtonAvailability();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillRowList");
                    }
                });
            });

            //просмотр источников позиции накладной
            $("#gridMovementWaybillRows .source_link").click(function () {
                var movementWaybillRowId = $(this).parent("td").parent("tr").find(".movementWaybillRowId").text();
                var articleName = $(this).parent("td").parent("tr").find(".ArticleName").text();
                var batchName = $(this).parent("td").parent("tr").find(".Batch").text();

                $.ajax({
                    type: "POST",
                    url: "/OutgoingWaybillRow/GetSourceWaybill/",
                    data: { type: "MovementWaybill", id: movementWaybillRowId, articleName: articleName, batchName: batchName },
                    success: function (result) {
                        $("#movementWaybillSourceLink").hide().html(result);
                        ShowModal("movementWaybillSourceLink");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillRowList");
                    }
                });
            });

            // удаление позиции накладной
            $("#gridMovementWaybillRows .delete_link").click(function () {
                if (confirm('Вы уверены?')) {
                    var movementWaybillId = $('#Id').val();
                    var movementWaybillRowId = $(this).parent("td").parent("tr").find(".hidden_column").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/MovementWaybill/DeleteRow/",
                        data: { movementWaybillId: movementWaybillId, movementWaybillRowId: movementWaybillRowId },
                        success: function (result) {
                            // грид для формы добавления товаров списком
                            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                                RefreshGrid("gridArticleGroups", function () {
                                    RefreshGrid("gridMovementWaybillRows", function () {
                                        MovementWaybill_Details.RefreshMainDetails(result);
                                        ShowSuccessMessage("Позиция удалена.", "messageMovementWaybillRowList");
                                    });
                                });   
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageMovementWaybillRowList");
                        }
                    });
                }
            });
        });
    }
};