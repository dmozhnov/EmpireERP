var PrintingForm_InvoicePrintingForm = {
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

            var bottomDiv = $(document.createElement("div"));
            bottomDiv.addClass("bottomDiv");
            bottomDiv.appendTo(currentPage);

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
        function CreateTableCell(tr, value, className, attr, colspan) {
            var td = $(document.createElement("td"));

            td.html(value);

            if (className != undefined && className != null) {
                td.addClass(className);
            }

            if (attr != undefined && attr != null) {
                td.attr(attr);
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
            CreateTableCell(tr, obj.ArticleName, "col1");
            CreateTableCell(tr, obj.MeasureUnitCode, "col2", { "align": "right" });
            CreateTableCell(tr, obj.MeasureUnitName, "col2a");
            CreateTableCell(tr, obj.Count, "col3", { "align": "right" });
            CreateTableCell(tr, obj.Price, "col4", { "align": "right" });
            CreateTableCell(tr, obj.Cost, "col5", { "align": "right" });
            CreateTableCell(tr, obj.ExciseValue, "col6", obj.ExciseValue == "без акциза" ? { "align": "center"} : { "align": "right" });
            CreateTableCell(tr, obj.TaxValue, "col7", { "align": "center" });
            CreateTableCell(tr, obj.TaxSum, "col8", { "align": "right" });
            CreateTableCell(tr, obj.TaxedCost, "col9", { "align": "right" });
            CreateTableCell(tr, obj.CountryCode, "col10", obj.CountryCode == "-" ? { "align": "center"} : { "align": "right" });
            CreateTableCell(tr, obj.CountryName, "col10a", obj.CountryName == "-" ? { "align": "center"} : { "align": "left" });
            CreateTableCell(tr, obj.CustomsDeclarationNumber, "col11", obj.CustomsDeclarationNumber == "-" ? { "align": "center"} : { "align": "right" });
        }

        //Генерируем таблицу на страницу
        function CreateTable(currentPage, maxHeight, content, index) {

            //Создаем таблицу
            var table = $(document.createElement("table"));
            table.addClass("MainTable");
            //table.attr("style", "width: 0px; height: 0px;");
            currentPage.find(".topDiv").after(table);
            //table.appendTo(currentPage);

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
            var sum5 = 0;
            var sum8 = 0;
            var sum9 = 0;
            $.each(table.find("tr"), function (index, row) {
                sum5 += parseStrToFloat($(row).find(".col5").html());
                sum8 += parseStrToFloat($(row).find(".col8").html());
                sum9 += parseStrToFloat($(row).find(".col9").html());
            });

            var tr = $(document.createElement("tr"));
            tr.addClass("tableSummary");
            tr.appendTo(table);

            CreateTableCell(tr, "Всего по странице", null, null, 5);
            CreateTableCell(tr, ForDisplay(sum5), null, { "align": "right" });
            CreateTableCell(tr, "X", null, { "align": "center" }, 2);
            CreateTableCell(tr, ForDisplay(sum8), null, { "align": "right" });
            CreateTableCell(tr, ForDisplay(sum9), null, { "align": "right" });
            CreateTableCell(tr, "", "emptyCell", null, 3);

            return tr;
        }

        function ForDisplay(value) {
            var result = "";
            if (!(value == undefined || isNaN(value) || value == null)) {
                result = ValueForDisplay(value, 2, false);
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
            var sum5 = 0;
            var sum8 = 0;
            var sum9 = 0;
            for (var i = 0; i < content.length; i++) {
                var row = content[i];
                sum5 += parseStrToFloat(row.Cost);
                sum8 += parseStrToFloat(row.TaxSum);
                sum9 += parseStrToFloat(row.TaxedCost);
            }

            var tr = $(document.createElement("tr"));
            tr.addClass("tableSummary");
            tr.appendTo(table);

            CreateTableCell(tr, "Всего к оплате", null, null, 5);
            CreateTableCell(tr, ForDisplay(sum5), null, { "align": "right" });
            CreateTableCell(tr, "X", null, { "align": "center" }, 2);
            CreateTableCell(tr, ForDisplay(sum8), null, { "align": "right" });
            CreateTableCell(tr, ForDisplay(sum9), null, { "align": "right" });
            CreateTableCell(tr, "", "emptyCell", null, 3);

            return tr;
        }

        //Создаем таблицу подписей
        function CreateFooterTable(currentPage) {
            //Взять footer из View
            var footer = $(document.createElement("div"));
            footer.attr("id", "footerPrintingForm");
            footer.appendTo(currentPage.find(".bottomDiv"));

            $("#footerPrintingForm").after($("#PageFooter"));

            return footer;
        }
    }
};