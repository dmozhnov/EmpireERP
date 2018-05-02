var Waybill_ForWaybillRowsAdditionByListGrid = {

    actionName: "",
    gridId: "",
    messageId: "",

    BindAddingRowsByList: function () {
        $(document).ready(function () {
            $("#gridArticlesForWaybillRowsAdditionByList input.MovingCount").bind('blur', function () {
                if ($(this).val() != "") {

                    var movingCount = TryGetDecimal($(this).val());
                    var availableToReserveCount = TryGetDecimal($(this).parent("td").parent("tr").find(".AvailableToReserveCount").text().replaceAll(' ', ''));
                    var measureUnitScale = $(this).findCell(".MeasureUnitScale").text();

                    if (isNaN(movingCount) || movingCount > availableToReserveCount || movingCount <= 0 || !CheckValueScale(movingCount, measureUnitScale, 12)) {
                        $(this).addClass("field-validation-error");
                        
                        ShowErrorMessage("Введите " + (measureUnitScale == 0 ? "целое " : "") + "кол-во товара, не большее " + availableToReserveCount.toString() + 
                        (measureUnitScale > 0 ? ", с количеством знаков после запятой не более " + measureUnitScale : "")+".", "messageArticlesForWaybillRowsAdditionByList");
                        return false;
                    }
                    else { $(this).removeClass("field-validation-error"); }

                    var articleId = $(this).parent("td").parent("tr").find(".ArticleId").text();
                    var waybillId = $("#Id").val();

                    StartGridProgress($("#" + Waybill_ForWaybillRowsAdditionByListGrid.gridId));
                    StartGridProgress($("#gridArticlesForWaybillRowsAdditionByList"));

                    var thisInput = $(this);

                    $.ajax({
                        type: "POST",
                        url: Waybill_ForWaybillRowsAdditionByListGrid.actionName,
                        data: { waybillId: waybillId, articleId: articleId, count: movingCount },
                        success: function (result) {
                            RefreshGrid(Waybill_ForWaybillRowsAdditionByListGrid.gridId, function () {
                                thisInput.disableInput();
                                StopGridProgress($("#" + Waybill_ForWaybillRowsAdditionByListGrid.gridId));
                                StopGridProgress($("#gridArticlesForWaybillRowsAdditionByList"));
                                ShowSuccessMessage("Позиция добавлена.", Waybill_ForWaybillRowsAdditionByListGrid.messageId);
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            StopGridProgress($("#" + Waybill_ForWaybillRowsAdditionByListGrid.gridId));
                            StopGridProgress($("#gridArticlesForWaybillRowsAdditionByList"));
                            ShowErrorMessage(XMLHttpRequest.responseText, Waybill_ForWaybillRowsAdditionByListGrid.messageId);
                        }
                    });
                }
            });
        });
    }
};
