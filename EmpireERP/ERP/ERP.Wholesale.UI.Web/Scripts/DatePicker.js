$(function () {
    $(".datepicker").datepicker();
});

function CreateDatePicker(idName) {
    var dates = $("#" + idName + " .from, #" + idName + " .to").datepicker({
        //defaultDate: "+1w",
        changeMonth: true,
		changeYear: true,
        numberOfMonths: 1,
        onSelect: function (selectedDate) {
            var option = $(this).hasClass("from") ? "minDate" : "maxDate",
					instance = $(this).data("datepicker");
            date = $.datepicker.parseDate(
						instance.settings.dateFormat ||
						$.datepicker._defaults.dateFormat,
						selectedDate, instance.settings);
            dates.not(this).datepicker("option", option, date);
        },
        onClose: function (selectedDate) {
            $(this).trigger("blur");
        }
    });
}

$(document).ready(function () {
    $(".dateRangePicker").each(function () {
        CreateDatePicker($(this).attr("id"));
    });
});

$(function () {
    $.datepicker.regional['ru'] = {
        closeText: 'Закрыть',
        prevText: '&#x3c;Пред',
        nextText: 'След&#x3e;',
        currentText: 'Сегодня',
        monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
		'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
        monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн',
		'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
        dayNames: ['воскресенье', 'понедельник', 'вторник', 'среда', 'четверг', 'пятница', 'суббота'],
        dayNamesShort: ['вск', 'пнд', 'втр', 'срд', 'чтв', 'птн', 'сбт'],
        dayNamesMin: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
        weekHeader: 'Нед',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['ru']);
});

function GetDateRange(picker_id) {
    var from = "";
    var to = "";
    
    var date_from = $('#' + picker_id + " .from").datepicker("getDate");
    if (date_from != null) {
        from = date_from.format("dd.MM.yyyy");
        var from_split = from.split('.');
        from = from_split[0] + '.' + from_split[1] + '.' + from_split[2];
    }
    var date_to = $('#' + picker_id + " .to").datepicker("getDate");
    
    if (date_to != null) {
        to = date_to.format("dd.MM.yyyy");
        var to_split = to.split('.');
        to = to_split[0] + '.' + to_split[1] + '.' + to_split[2];
    }

    return from + '-' + to;
}


