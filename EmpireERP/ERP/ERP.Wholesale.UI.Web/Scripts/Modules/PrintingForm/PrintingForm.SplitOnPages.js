var PrintingForm_SplittingPage = {
    // Главный метод
    // function_FullingTableRow - метод заполнения строки таблицы
    // function_CreateSummaryForPage - метод создания строки "Итого по странице"
    // function_CreateSummaryForDocument - метод создания строки "Итого по документу"
    // function_PageProcessing - метод обработки созданной страницы [не обязательный, если не нужен, то можно опустить]
    ShowContent: function (content, function_FullingTableRow, function_CreateSummaryForPage, function_CreateSummaryForDocument, function_PageProcessing) {
        //Построение всей печатной формы
        //настраиваем форму
        var mainDiv = $("#mainContentPrintingForm");

        var tmp = PrintingForm_SplittingPage.CreateFirstPage(content.Rows, function_FullingTableRow,
            function_CreateSummaryForPage, function_CreateSummaryForDocument, function_PageProcessing);    //Создаем первую страницу
        currentIndex = tmp.currentIndex;
        var pageIndex = tmp.pageIndex + 1;

        //Цикл пока есть данные для формирования страниц
        while (currentIndex < content.Rows.length) {
            tmp = PrintingForm_SplittingPage.CreatePage(pageIndex, currentIndex, content.Rows, function_FullingTableRow,
                function_CreateSummaryForPage, function_CreateSummaryForDocument, function_PageProcessing);

            currentIndex = tmp.currentIndex;
            pageIndex = tmp.pageIndex + 1;
        }
    },

    //Создание новой страницы
    CreateNewPage: function (pageIndex, pageBreak) {
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
    },

    //Создание первой страницы
    CreateFirstPage: function (content, function_FullingTableRow, function_CreateSummaryForPage, function_CreateSummaryForDocument) {
        var currentPage = PrintingForm_SplittingPage.CreateNewPage(0, false);   //Создаем страницу

        //Создаем заголовок печатной формы
        var headerDiv = $(document.createElement("div"));
        headerDiv.attr("id", "headerPrintingForm");
        headerDiv.appendTo(currentPage.find(".topDiv"));

        //Взять header из View!
        $("#PageHeader").appendTo("#headerPrintingForm");

        var tableHeight = currentPage.attr("offsetHeight") - headerDiv.attr("offsetHeight");    //Вычислям высоту таблицы

        var pageIndex = 0;
        var currentIndex = PrintingForm_SplittingPage.CreateTable(currentPage, tableHeight, content, 0, function_FullingTableRow, function_CreateSummaryForPage, function_CreateSummaryForDocument);   //Выводим на первую страницу таблицу

        return { currentIndex: currentIndex, pageIndex: pageIndex };
    },

    //Создание последующих страниц (вторая и т.д.)
    CreatePage: function (pageIndex, index, content, function_FullingTableRow, function_CreateSummaryForPage, function_CreateSummaryForDocument, function_PageProcessing) {
        var currentPage = PrintingForm_SplittingPage.CreateNewPage(pageIndex, true);   //Создаем страницу

        var tableHeight = currentPage.attr("offsetHeight"); //Вычисляем высоту таблицы

        var currentIndex = PrintingForm_SplittingPage.CreateTable(currentPage, tableHeight, content, index, function_FullingTableRow,
                function_CreateSummaryForPage, function_CreateSummaryForDocument);   //Выводим данные на первую страницу таблицы

        // Обработка добавленной страницы
        if (function_PageProcessing) {  //Если обработчик указан, то
            function_PageProcessing(currentPage, pageIndex);    // вызываем его
        }

        return { currentIndex: currentIndex, pageIndex: pageIndex };
    },

    //Генерируем заголовок таблицы
    CreateTableHeader: function (currentPage, content) {
        var c = $("#MainTable .mainTableHeader").clone();
        c.appendTo(currentPage);

        return currentPage.find(".mainTableHeader");
    },

    //Генерируем таблицу на страницу
    CreateTable: function (currentPage, maxHeight, content, index, function_FullingTableRow, function_CreateSummaryForPage, function_CreateSummaryForDocument) {

        var table = PrintingForm_SplittingPage.CreateTableHeader(currentPage, content);  //Создаем таблицу с заголовком

        //Цикл заполнения таблицы
        var i;
        var footer = null;

        for (i = index; i < content.length; i++) {

            table.find(".SummaryForPage").remove();

            //Добавляем очередную строку таблицы
            var tr = function_FullingTableRow(i, table, content[i], content);   //Заполняем строку данными
            function_CreateSummaryForPage(table);  // Вставить расчет и вставку "Итого" по странице!

            var currentHeight = 0;

            if (i == content.length - 1) {

                if (currentPage.attr("id") == "page_0") {
                    table.find(".SummaryForPage").remove();
                }

                function_CreateSummaryForDocument(table, content);

                footer = PrintingForm_SplittingPage.CreateFooterTable(currentPage);    //Добавляем "подвал"
                currentHeight += footer.attr("offsetHeight");
            }

            currentHeight += table.attr("offsetHeight");   //Получает текущую высоту таблицы

            //Проверяем условие окончания страницы
            if (currentHeight >= maxHeight) {
                //Страница закончилась, удаляем последнюю добавленную строку

                table.find(".SummaryForPage").remove();
                table.find(".SummaryForDocument").remove();

                if (footer != null) {
                    footer.remove();
                    footer = null;
                }
                tr.remove();

                function_CreateSummaryForPage(table);  // Вставить расчет и вставку "Итого" по странице!
                break;
            }
        }

        return i;
    },

    //Создаем таблицу подписей
    CreateFooterTable: function (currentPage) {
        //Взять footer из View
        var footer = $(document.createElement("div"));
        footer.appendTo(currentPage);

        $("#PageFooter").clone().appendTo(footer);
        //currentPage.find(".mainTable").after($("#PageFooter"));

        return footer;
    },

    ParseStrToFloat: function (value) {
        // Проверка одновременно на null и на undefined
        if (value == null)
            return 0;

        result = isString(value) ?
                TryGetDecimal(value.replaceAll(" ", "")) :
                TryGetDecimal(value);

        return !isNaN(result) ? result : 0;
    },

    // removeTrailingZeroes - удалять ли последние нули в копейках
    ForDisplay: function (value, scale, removeTrailingZeroes) {
        var result = "";
        if (!(value == undefined || isNaN(value) || value == null)) {
            result = ValueForDisplay(value, scale, removeTrailingZeroes);
        }

        return result;
    }
};