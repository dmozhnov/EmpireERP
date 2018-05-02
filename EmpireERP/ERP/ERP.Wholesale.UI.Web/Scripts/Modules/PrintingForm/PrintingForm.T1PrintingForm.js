var PrintingForm_T1PrintingForm = {
    Init: function () {
        $(document).ready(function () {
            //Запрос данных формы
            $.ajax({
                type: "GET",
                url: $("#RowsContentURL").val(),
                data: { WaybillId: $("#WaybillId").val(), PricetypeId: $("#PriceTypeId").val() },
                success: function (result) {
                    PrintingForm_SplittingPage.ShowContent(
                        result,
                        FullingTableRow,
                        CreateSummaryForPage,
                        CreateSummaryForForm);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    alert(XMLHttpRequest.responseText);
                }
            });
        });

        //Заполняем строку данными
        function FullingTableRow(index, table, obj, content) {
            var template = $(".mainTableRow tbody").html();
            $(template).appendTo(table);

            var tr = table.find("tr:last");

            tr.find(".ItemNumber").text(obj.ItemNumber);
            tr.find(".ListPriseNumber").text(obj.ListPriseNumber);
            tr.find(".Number").text(obj.Number);
            tr.find(".Count").text(obj.Count);
            tr.find(".Price").text(obj.Price);
            tr.find(".Name").text(obj.Name);
            tr.find(".MeasureUnit").text(obj.MeasureUnit);
            tr.find(".PackType").text("");
            tr.find(".PackCount").text("");
            tr.find(".Weight").text(obj.Weight);
            tr.find(".Sum").text(obj.Sum);
            tr.find(".SerialNumber").text("");

            return tr;
        }

        // Создание и заполение строки итого по странице
        function CreateSummaryForPage(table) {
            var count = 0;
            var countScale = parseInt($("#CountScale").val());
            var sum = 0;
            var weight = 0;

            table.find("tr").each(function (index, tr) {
                count += PrintingForm_SplittingPage.ParseStrToFloat($(tr).find(".Count").html());
                sum += PrintingForm_SplittingPage.ParseStrToFloat($(tr).find(".Sum").html());
                weight += PrintingForm_SplittingPage.ParseStrToFloat($(tr).find(".Weight").html());
            })

            var template = $(".mainTablePageSummary tbody");
            template.find(".CountOnPage").html(PrintingForm_SplittingPage.ForDisplay(count, countScale, false));
            template.find(".SumOnPage").html(PrintingForm_SplittingPage.ForDisplay(sum, 2, false));
            template.find(".WeightOnPage").html(PrintingForm_SplittingPage.ForDisplay(weight, 3, false));

            $(template.html()).appendTo(table);
        }

        // создание строки итого по документу
        function CreateSummaryForForm(table, content) {
            var template = $(".mainTableEnding tbody").html();
            $(template).appendTo(table);

            if (table.find(".SummaryForPage").length) {
                table.find(".SummaryForDocumentWithoutPageSummary").remove();
            }
            else {
                table.find(".SummaryForDocumentWithPageSummary").remove();
            }
        }
    }
};