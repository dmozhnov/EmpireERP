var PrintingForm_TORG12PrintingForm = {
    Init: function () {
        $(document).ready(function () {
            //Запрос данных формы
            $.ajax({
                type: "GET",
                url: $("#RowsContentURL").val(),
                data: { WaybillId: $("#WaybillId").val(), PriceTypeId: $("#PriceTypeId").val(), ConsiderReturns: $("#ConsiderReturns").val() },
                success: function (result) {
                    ShowContent(result);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    alert(XMLHttpRequest.responseText);
                }
            });
        });

        //Построение всей печатной формы
        function ShowContent(content) {
            //настраиваем форму
            var mainDiv = $("#mainContentPrintingForm");

            var tmp = CreateFirstPage(content.Rows);    //Создаем первую страницу
            currentIndex = tmp.currentIndex;
            var pageIndex = tmp.pageIndex + 1;

            //Цикл пока есть данные для формирования страниц
            while (currentIndex < content.Rows.length) {
                tmp = CreatePage(pageIndex, currentIndex, content.Rows);
                currentIndex = tmp.currentIndex;
                pageIndex = tmp.pageIndex + 1;
            }
            $("#maxPageNumber").val(pageIndex);
            $("#RowsCountString").text(content.RowsCountString);
            $("#TotalSalePriceString").text(content.TotalSalePriceString);
            $("#WeightBruttoString").text(content.WeightBruttoString);
        }

        //Создание новой страницы
        function CreateNewPage(pageIndex, pageBreak) {
            //Создаем страницу
            var currentPage = $(document.createElement("div"));
            if (pageBreak) {
                $("#page_" + (pageIndex - 1)).addClass("pageBreak");
            }
            if ($.browser.opera) {
                currentPage.addClass("page_Opera");
            }
            else {
                currentPage.addClass("page");
            }
            currentPage.attr("id", "page_" + pageIndex);

            var topDiv = $(document.createElement("div"));
            topDiv.addClass("topDiv");
            topDiv.appendTo(currentPage);

            var mainDiv = $("#mainContentPrintingForm");
            mainDiv.append(currentPage); //Добавляем ее на форму

            return currentPage;
        }

        //Создание первой страницы
        function CreateFirstPage(content) {
            var currentPage = CreateNewPage(0, false);   //Создаем страницу

            //Создаем заголовок печатной формы
            var headerDiv = $(document.createElement("div"));
            headerDiv.attr("id", "headerPrintingForm");
            headerDiv.appendTo(currentPage.find(".topDiv"));

            //Взять header из View!
            $("#PageHeader").appendTo("#headerPrintingForm");

            var tableHeight = currentPage.attr("offsetHeight") - headerDiv.attr("offsetHeight");    //Вычислям высоту таблицы

            var pageIndex = 0;
            var currentIndex = CreateTable(currentPage, tableHeight, content, 0);   //Выводим на первую страницу таблицу
            currentPage.find(".PageNumber").html("Страница 1");

            return { currentIndex: currentIndex, pageIndex: pageIndex };
        }

        //Создание последующих страниц (вторая и т.д.)
        function CreatePage(pageIndex, index, content) {
            var currentPage = CreateNewPage(pageIndex, true);   //Создаем страницу

            var tableHeight = currentPage.attr("offsetHeight"); //Вычисляем высоты таблицы

            var currentIndex = CreateTable(currentPage, tableHeight, content, index);   //Выводим данные на первую страницу таблицы
            currentPage.find(".PageNumber").html("Страница " + (pageIndex + 1));

            return { currentIndex: currentIndex, pageIndex: pageIndex };
        }

        //Создание ячейки таблицы
        function CreateTableCell(tr, value, className, colspan) {
            var td = $(document.createElement("td"));

            td.html(value);
            if (className != undefined && className != null) {
                td.addClass(className);
            }
            if (colspan != undefined && colspan != null) {
                td.attr("colspan", colspan);
            }
            td.appendTo(tr);
        }

        //Генерируем заголовок таблицы
        function CreateTableHeader(table, content) {
            // TODO Взять шапку из View!
            var c = $("#MainTable").find("table").html();
            //var thead = $(document.createElement("thead"));
            //thead.appendTo(table);
            $(c).appendTo(table);
        }

        //Заполняем строку данными
        function FullingTableRow(index, tr, obj, content) {
            //TODO Переделать
            CreateTableCell(tr, "", "R22C0");
            CreateTableCell(tr, obj.RowNumber, "R22C1");
            CreateTableCell(tr, obj.ArticleName, "R22C2");
            CreateTableCell(tr, obj.Id, "R22C3");
            CreateTableCell(tr, obj.MeasureUnit, "R22C4");
            CreateTableCell(tr, obj.MeasureUnitOKEI, "R22C5");
            CreateTableCell(tr, obj.PackType, "R22C6");
            CreateTableCell(tr, obj.PackVolume, "R22C7");
            CreateTableCell(tr, obj.PackCount, "col7 R22C7");
            CreateTableCell(tr, obj.WeightBrutto, "col8 R22C7");
            CreateTableCell(tr, obj.Count, "col9 R22C10");
            CreateTableCell(tr, obj.WithoutVatPrice, "R22C10");
            CreateTableCell(tr, obj.SumWithoutValueAddedTax, "col11 R22C12");
            CreateTableCell(tr, obj.ValueAddedTaxRate, "R22C13");
            CreateTableCell(tr, obj.ValueAddedTax, "col13 R22C14");
            CreateTableCell(tr, obj.WithVatPriceSum, "col14 R22C15");
        }

        //Генерируем таблицу на страницу
        function CreateTable(currentPage, maxHeight, content, index) {

            //Создаем таблицу
            var table = $(document.createElement("table"));
            table.addClass("mainTable");
            //table.attr("style", "width: 0px; height: 0px;");
            table.appendTo(currentPage);

            CreateTableHeader(table, content);  //Создаем заголовок таблицы
            //var footer = CreateFooterTable(currentPage);    //Добавляем "подвал"

            //Цикл заполнения таблицы
            var i;
            var summaryRow = null;
            var footer = null;
            var summaryForForm = null;

            for (i = index; i < content.length; i++) {
                if (summaryRow != null) {
                    summaryRow.remove();
                }

                //Создаем строку таблицы
                var tr = $(document.createElement("tr"));
                tr.addClass("R18");
                tr.appendTo(table);

                FullingTableRow(i, tr, content[i], content);   //Заполняем строку данными
                summaryRow = CreateSummaryForPage(table);  // Вставить расчет и вставку "Итого" по странице!


                var currentHeight = table.attr("offsetHeight");   //Получает текущую высоту таблицы

                if (i == content.length - 1) {

                    if (currentPage.attr("id") == "page_0") {
                        table.find(".SummaryForPage").remove();
                    }

                    summaryForForm = CreateSummaryForForm(table, content);
                    currentHeight += summaryForForm.attr("offsetHeight");

                    footer = CreateFooterTable(currentPage);    //Добавляем "подвал"
                    currentHeight += footer.attr("offsetHeight");
                }
                //Проверяем условие окончания страницы
                if (currentHeight >= maxHeight) {
                    //Страница закончилась, удаляем последнюю добавленную строку
                    summaryRow.remove();
                    if (footer != null) {
                        footer.remove();
                    }
                    if (summaryForForm != null) {
                        summaryForForm.remove();
                    }
                    tr.remove();
                    CreateSummaryForPage(table);  // Вставить расчет и вставку "Итого" по странице!
                    break;
                }
            }

            return i;
        }

        function CreateSummaryForPage(table) {
            var sum7 = 0;
            var sum8 = 0;
            var sum9 = 0;
            var sum11 = 0;
            var sum13 = 0;
            var sum14 = 0;
            $.each(table.find("tr"), function (index, row) {
                sum7 += parseStrToFloat($(row).find(".col7").html());
                sum8 += parseStrToFloat($(row).find(".col8").html());
                sum9 += parseStrToFloat($(row).find(".col9").html());
                sum11 += parseStrToFloat($(row).find(".col11").html());
                sum13 += parseStrToFloat($(row).find(".col13").html());
                sum14 += parseStrToFloat($(row).find(".col14").html());
            });

            var tr = $(document.createElement("tr"));
            tr.addClass("SummaryForPage R18");
            tr.appendTo(table);

            CreateTableCell(tr, "", "R26C0");
            CreateTableCell(tr, "", "R26C1");
            CreateTableCell(tr, "", "R26C2");
            CreateTableCell(tr, "", "R26C3");
            CreateTableCell(tr, "", "R26C0");
            CreateTableCell(tr, "", "R26C7");
            CreateTableCell(tr, "Итого", "R26C7", 2);
            CreateTableCell(tr, ForDisplay(sum7, true), "R26C8");
            CreateTableCell(tr, ValueForDisplay(sum8, 3, false), "R26C8");
            CreateTableCell(tr, ForDisplay(sum9, true), "R26C8");
            CreateTableCell(tr, "X", "R26C11");
            CreateTableCell(tr, ForDisplay(sum11, false), "R26C8");
            CreateTableCell(tr, "X", "R26C13");
            CreateTableCell(tr, ForDisplay(sum13, false), "R26C14");
            CreateTableCell(tr, ForDisplay(sum14, false), "R26C15");

            return tr;
        }

        // removeTrailingZeroes - удалять ли последние нули в копейках
        function ForDisplay(value, removeTrailingZeroes) {
            var result = "";
            if (!(value == undefined || isNaN(value) || value == null)) {
                result = ValueForDisplay(value, 2, removeTrailingZeroes);
            }

            return result;
        }

        function parseStrToFloat(value) {
            // Проверка одновременно на null и на undefined
            if (value == null)
                return 0;

            result = isString(value) ?
                TryGetDecimal(value.replaceAll(" ", "")) :
                TryGetDecimal(value);

            return !isNaN(result) ? result : 0;
        }

        //Расчет суммы по всей форме
        function CreateSummaryForForm(table, content) {
            var sum8 = 0;
            var sum9 = 0;
            var sum11 = 0;
            var sum13 = 0;
            var sum14 = 0;
            for (var i = 0; i < content.length; i++) {
                var row = content[i];
                sum8 += row.WeightBruttoValue;
                sum9 += parseStrToFloat(row.Count);
                sum11 += parseStrToFloat(row.SumWithoutValueAddedTax);
                sum13 += parseStrToFloat(row.ValueAddedTax);
                sum14 += parseStrToFloat(row.WithVatPriceSum);
            }

            var tr = $(document.createElement("tr"));
            tr.addClass("R18");
            tr.appendTo(table);

            if (table.parent().attr("id") != "page_0") {
                CreateTableCell(tr, "", "R26C0");
                CreateTableCell(tr, "Всего по накладной", "R33C7", 7);
                CreateTableCell(tr, "", "R33C8");
                CreateTableCell(tr, ValueForDisplay(sum8, 3, false), "R33C8");
                CreateTableCell(tr, ForDisplay(sum9, true), "R33C8");
                CreateTableCell(tr, "X", "R33C11");
                CreateTableCell(tr, ForDisplay(sum11, false), "R22C10");
                CreateTableCell(tr, "X", "R33C11");
                CreateTableCell(tr, ForDisplay(sum13, false), "R22C10");
                CreateTableCell(tr, ForDisplay(sum14, false), "R22C10");
            }
            else {
                CreateTableCell(tr, "", "R26C0");
                CreateTableCell(tr, "", "R26C1");
                CreateTableCell(tr, "", "R26C2");
                CreateTableCell(tr, "", "R26C3");
                CreateTableCell(tr, "", "R26C0");
                //CreateTableCell(tr, "", "R26C7");
                CreateTableCell(tr, "Всего по накладной", "R26C7", 3);
                CreateTableCell(tr, "", "R26C8");
                CreateTableCell(tr, ValueForDisplay(sum8, 3, false), "R26C8");
                CreateTableCell(tr, ForDisplay(sum9, true), "R26C8");
                CreateTableCell(tr, "X", "R26C11");
                CreateTableCell(tr, ForDisplay(sum11, false), "R26C15");
                CreateTableCell(tr, "X", "R26C13");
                CreateTableCell(tr, ForDisplay(sum13, false), "R26C15");
                CreateTableCell(tr, ForDisplay(sum14, false), "R26C15");
            }

            return tr;
        }

        //Создаем таблицу подписей
        function CreateFooterTable(currentPage) {
            //Взять footer из View
            var footer = $(document.createElement("div"));
            footer.appendTo(currentPage);
            currentPage.find(".mainTable").after($("#PageFooter"));

            return footer;
        }
    }
};