function _refreshGrid(grid_id, currentPage, pageSize) {
    var grid_container = $("#" + grid_id).closest("div.grid").parent("div").attr("id");
    var view = $("#" + grid_id + " .view_action").attr("value");
    var parameters = $("#" + grid_id + " .parameters").attr("value");
    var filter = $("#" + grid_id + " .grid_filter").attr("value");
    var sort = $("#" + grid_id + " .grid_sort").attr("value");

    StartGridProgress($("#" + grid_id)); // останавливать не нужно, т.к. div с индикатором удаляется при обновлении грида

    $.ajax({
        type: "POST",
        url: view,
        data: { parameters: parameters, currentPage: currentPage, pageSize: pageSize, filter: filter, Sort: sort },
        success: function (result) {
            $('#' + grid_container).replaceWith(result);
        },
        error: function () {
            alert("Ошибка при обновлении списка.");
        }
    });
}

$(document).ready(function () {
    // переход на следующую странцу
    $(".grid_next").live('click', function () {
        var grid_id = $(this).closest("div.grid").attr("id");
        var currentPage = parseInt($("#" + grid_id + " .current_page").attr("value")) + 1;
        var pageSize = $("#" + grid_id + " .page_size").attr("value");

        _refreshGrid(grid_id, currentPage, pageSize);
    });
    // переход на предыдущую страницу
    $(".grid_back").live('click', function () {
        var grid_id = $(this).closest("div.grid").attr("id");
        var currentPage = parseInt($("#" + grid_id + " .current_page").attr("value")) - 1;
        var pageSize = $("#" + grid_id + " .page_size").attr("value");

        _refreshGrid(grid_id, currentPage, pageSize);
    });
    // переход на страницу (через номера страниц)
    $(".number_page").live('click', function () {
        var grid_id = $(this).closest("div.grid").attr("id");
        var currentPage = $(this).text();
        var pageSize = $("#" + grid_id + " .page_size").attr("value");

        _refreshGrid(grid_id, currentPage, pageSize);
    });
    // изменение количества строк в таблице
    $(".page_size").live('change', function () {
        var grid_id = $(this).closest("div.grid").attr("id");
        var currentPage = 1;
        var pageSize = $(this).attr("value");

        _refreshGrid(grid_id, currentPage, pageSize);
    });
    // переход на заданную страницу
    $(".go_to_page").live('change', function () {
        var grid_id = $(this).closest("div.grid").attr("id");
        var currentPage = $(this).attr("value");
        var pageSize = $("#" + grid_id + " .page_size").attr("value");

        _refreshGrid(grid_id, currentPage, pageSize);
    });

    // выделение строки грида
    $('.grid_row').live("click", function () {
        var grid_id = $(this).closest("div.grid").attr("id");

        var selected = $(this).children("td").hasClass("selectedNormal");
        DeleteSelectedStyle(grid_id);

        // снимаем выделение при повторном клике
        if ($(this).hasClass("gridNormalRow")) {
            if (!selected) {
                $(this).children("td").addClass("selectedNormal");
                $(this).children("td").addClass("selected");
            }
        }
        if ($(this).hasClass("gridSuccessRow"))
            $(this).children("td").addClass("selectedSuccess");
        if ($(this).hasClass("gridWarningRow"))
            $(this).children("td").addClass("selectedWarning");
        if ($(this).hasClass("gridErrorRow"))
            $(this).children("td").addClass("selectedError");
    });
});

//обновление грида
function RefreshGrid(grid_id, onSuccessFunction) {

    // если грид не найден - вызываем переданный метод и выходим
    if ($('#' + grid_id).length == 0) {
        if (onSuccessFunction != undefined)
            onSuccessFunction();

        return;   
    }
    
    var grid_container = $('#' + grid_id).parent("div").attr("id");
    var view = $("#" + grid_id + " .view_action").attr("value");
    var parameters = $("#" + grid_id + " .parameters").attr("value");
    var filter = $("#" + grid_id + " .grid_filter").attr("value");
    var sort = $("#" + grid_id + " .grid_sort").attr("value");
    var currentPage = parseInt($("#" + grid_id + " .current_page").attr("value"));
    var pageSize = $("#" + grid_id + " .page_size").attr("value");

    $.ajax({
        type: "POST",
        url: view,
        data: { Parameters: parameters, CurrentPage: currentPage, PageSize: pageSize, Filter: filter, Sort: sort },
        success: function (result) {
            $('#' + grid_container).replaceWith(result);
            if (onSuccessFunction != undefined)
                onSuccessFunction();    //Вызываем переданный метод
        }
    });
}

function DeleteSelectedStyle(grid_id) {
    $('#' + grid_id + ' .selectedNormal').removeClass("selectedNormal");
    $('#' + grid_id + ' .selectedSuccess').removeClass("selectedSuccess");
    $('#' + grid_id + ' .selectedWarning').removeClass("selectedWarning");
    $('#' + grid_id + ' .selectedError').removeClass("selectedError");
    $('#' + grid_id + ' .selected').removeClass("selected");
}

// ******************* ФИЛЬТР *******************************
// применение фильтра
$(".grid_filter #btnApplyFilter").live("click", function () {
    var grid = $(this).closest(".grid_filter");
    var filter_str = "";

    // запускаем индикатор
    StartButtonProgress($(this), grid.attr("id"));

    // для текстовых полей
    $("#" + grid.attr("id") + " .filter_table :text").each(function (i, el) {
        // ограничиваем dateRangePicker        
        if ($("#" + el.id.toString()).hasClass("from") == false && $("#" + el.id.toString()).hasClass("to") == false) {
            filter_str += (el.id.toString() + "=" + $("#" + grid.attr("id") + " #" + el.id).attr("value").toString() + ";")
        }
    });

    // для dateRangePicker
    $("#" + grid.attr("id") + " .filter_table .dateRangePicker").each(function (i, el) {
        filter_str += (el.id.toString() + "=" + GetDateRange(el.id.toString()) + ";");
    });

    // для select
    $("#" + grid.attr("id") + " .filter_table select").each(function (i, el) {
        filter_str += (el.id.toString() + "=" + $("#" + grid.attr("id") + " #" + el.id).attr("value").toString() + ";")
    });

    // для гиперссылки
    $("#" + grid.attr("id") + " .filter_table .select_link").each(function (i, el) {
        filter_str += (el.id.toString() + "=" + $("#" + grid.attr("id") + " #" + el.id).attr("selected_id").toString() + ";")
    });

    // для переключателя да/нет
    $("#" + grid.attr("id") + " .filter_table .yes_no_toggle").each(function (i, el) {
        var hiddenField = $(el).siblings("input");
        filter_str += (hiddenField.attr("id").toString() + "=" + hiddenField.attr("value").toString() + ";")
    });

    var gridNamesString = grid.find("#gridNames").attr("value");
    var gridNames = gridNamesString.split(";");

    // останавливаем индикатор, когда завершится последний ajax-запрос
    var k = 0;
    $(this).ajaxComplete(function () {
        k++;

        if (gridNames.length == k) {
            StopButtonProgress();
        }
    });

    for (var i = 0; i < gridNames.length; i++) {
        RefreshGridWithFilter(filter_str, gridNames[i]);
    }
});

// сброс фильтра
$(".grid_filter #btnResetFilter").live("click", function () {
    // запускаем индикатор
    StartButtonProgress($(this), $(this).closest(".grid_filter").attr("id"));
    
    $(this).closest(".grid_filter .filter_table").find(":text").val("");
    $(this).closest(".grid_filter .filter_table").find("select").val("");
    $(this).closest(".grid_filter .filter_table").find(".select_link").each(function (i, el) { var link = $('#' + el.id); link.text(link.attr("default_text")); link.attr("selected_id", ""); });

    var gridNamesString = $(this).closest(".grid_filter").find("#gridNames").attr("value");
    var gridNames = gridNamesString.split(";");

    // останавливаем индикатор, когда завершится последний ajax-запрос
    var k = 0;
    $(this).ajaxComplete(function () {
        k++;

        if (gridNames.length == k) {
            StopButtonProgress();
        }
    });

    for (var i = 0; i < gridNames.length; i++) {
        RefreshGridWithFilter("", gridNames[i]);
    }
});



// обновление грида с учетом фильтра
function RefreshGridWithFilter(filter, grid_id) {
    var grid_container = $('#' + grid_id).parent("div").attr("id");
    var view = $("#" + grid_id + " .view_action").attr("value");
    var page_size = $("#" + grid_id + " .page_size").attr("value");
    var parameters = $("#" + grid_id + " .parameters").attr("value");
    var sort = $("#" + grid_id + " .grid_sort").attr("value");

    //$("#" + grid_id + " .gridFilter").attr("value", filter);    //Запись фильтра в поле грида    
    $.ajax({
        type: "POST",
        url: view,
        data: { Parameters: parameters, CurrentPage: parseInt(1), PageSize: parseInt(page_size), Filter: filter, Sort: sort },        
        success: function (result) {
            $('#' + grid_container).replaceWith(result);
        }
    });
}

// открытие/закрытие панели фильтра
$(".filter_header").live('click', function () {
    var content = $(this).closest(".grid").children(".filter_content");

    if (content.hasClass("hidden")) {
        content.addClass("visibe").removeClass("hidden");
        $(this).children("b").text("▼");
    }
    else {
        content.addClass("hidden").removeClass("visibe");
        $(this).children("b").text("►");
    }
});
// ******************************************************************