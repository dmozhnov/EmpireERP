var ReturnFromClientWaybill_RowGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridReturnFromClientWaybillRows table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".SaleWaybillId").text();
                $(this).find("a.SaleWaybillName").attr("href", "/ExpenditureWaybill/Details?id=" + id + GetBackUrl());
            });

            $("#btnCreateRow").click(function () {
                var returnFromClientWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ReturnFromClientWaybill/AddRow",
                    data: { returnFromClientWaybillId: returnFromClientWaybillId },
                    success: function (result) {
                        $('#returnFromClientWaybillRowEdit').hide().html(result);
                        //$.validator.unobtrusive.parse($("#returnFromClientWaybillRowEdit"));
                        ShowModal("returnFromClientWaybillRowEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillRowList");
                    }
                });
            });

            $("#gridReturnFromClientWaybillRows .edit_link, #gridReturnFromClientWaybillRows .details_link").click(function () {
                var returnFromClientWaybillId = $('#Id').val();
                var returnFromClientWaybillRowId = $(this).parent("td").parent("tr").find(".returnFromClientWaybillRowId").text();

                $.ajax({
                    type: "GET",
                    url: "/ReturnFromClientWaybill/EditRow",
                    data: { returnFromClientWaybillId: returnFromClientWaybillId, returnFromClientWaybillRowId: returnFromClientWaybillRowId },
                    success: function (result) {
                        $('#returnFromClientWaybillRowEdit').hide().html(result);
                        //$.validator.unobtrusive.parse($("#returnFromClientWaybillRowEdit"));
                        ShowModal("returnFromClientWaybillRowEdit");
                        $("#SaleLink").show();
                        $("#returnFromClientWaybillRowEdit #ReturningCount").focus();
                        ReturnFromClientWaybill_Shared.CheckSaveButtonAvailability();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillRowList");
                    }
                });
            });

            // удаление позиции накладной
            $("#gridReturnFromClientWaybillRows .delete_link").click(function () {
                if (confirm('Вы уверены?')) {
                    var returnFromClientWaybillId = $('#Id').val();
                    var returnFromClientWaybillRowId = $(this).parent("td").parent("tr").find(".returnFromClientWaybillRowId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ReturnFromClientWaybill/DeleteRow/",
                        data: { returnFromClientWaybillId: returnFromClientWaybillId, returnFromClientWaybillRowId: returnFromClientWaybillRowId },
                        success: function (result) {
                            RefreshGrid("gridReturnFromClientWaybillRows", function () {
                                RefreshGrid("gridArticleGroups", function () {
                                    ReturnFromClientWaybill_Details.RefreshMainDetails(result.MainDetails);
                                    ReturnFromClientWaybill_Details.RefreshPermissions(result.Permissions);
                                    ShowSuccessMessage("Позиция удалена.", "messageReturnFromClientWaybillRowList");
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientWaybillRowList");
                        }
                    });
                }
            });

        });
    }
};