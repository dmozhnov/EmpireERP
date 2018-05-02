$(document).ready(function () {
    window.onresize = ResizeContentWrapper;
    window.onload = ResizeContentWrapper;

    function ResizeContentWrapper() {
        if ($("#height_div").length != 0) {

            var windowHeight = 0;
            if (typeof (window.innerWidth) == 'number') {
                //Non-IE
                windowHeight = window.innerHeight;
            } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
                //IE 6+ in 'standards compliant mode'
                windowHeight = document.documentElement.clientHeight;
            } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
                //IE 4 compatible
                windowHeight = document.body.clientHeight;
            }

            var minHeight = 165 + parseInt($("#sub_menu_content").css("height").toString().replace("px", "")) - 21;
            if (windowHeight >= minHeight) {
                if (windowHeight - minHeight < 300) {
                    windowHeight = 300 + minHeight;
                }
                document.getElementById("height_div").style.height = (windowHeight - minHeight).toString() + "px";
            }
        }
    }
});                    // document ready


var oldVal = $.fn.val;
$.fn.val = function (value) {
    var x = oldVal.apply(this, arguments);
    if (value != undefined) $(this).trigger("modify");
    return x;
};

//очищение выпадающего списка
//пример использования: $('#DropDown').clearSelect();
$.fn.clearSelect = function () {
    return this.each(function () {
        if (this.tagName == 'SELECT')
            this.options.length = 0;
    });
}

//заполнение выпадающего списка
// data - сериализованный в Json объект DropDownListData
// noEmptyOption - не добавлять пустой элемент
//пример использования: $('#DropDown').fillSelect(json_dropDownListData);
$.fn.fillSelect = function (data, noEmptyOption) {
    return this.clearSelect().each(function () {
        if (this.tagName == 'SELECT') {
            var dropdownList = this;

            if (!IsTrue(noEmptyOption)) {
                var emptyoption = new Option("", "");

                if ($.browser.msie) {
                    dropdownList.add(emptyoption);
                }
                else {
                    dropdownList.add(emptyoption, null);
                }
            }

            $.each(data.List, function (index, optionData) {
                var isSelected = optionData.Value == data.SelectedOption;
                var option = new Option(optionData.Text, optionData.Value, isSelected, isSelected);

                if ($.browser.msie) {
                    dropdownList.add(option);
                }
                else {
                    dropdownList.add(option, null);
                }
            });
        }
    });
}

//Для текстовых инпутов - инпут становится невидимым, а на его место ставится span с тем же содержимым. 
//При изменении содержимого инпута изменится и содержимое span.
$.fn.disableInput = function () {
    return this.each(function () {
        if ($(this).attr("type") == "text" && $(this).css("display") != "none") {
            var value = $(this).val();
            var id = $(this).attr("id");
            var name = $(this).attr("name");
            var width = $(this).css("width");

            var label = $("<span>").text(value).css({ "text-align": "left" });

            // label.css({ "width": width, "display": "inline-block" }); эта штука нужна была, чтобы у лэйбла ширина оставалась та же, что и у инпута. 
            //потом решили от этого отказаться, но может быть это будет опцией, поэтому оставляю.

            $(this).css("display", "none").before(label);

            $(this).bind('modify', { label: label }, syncFieldsHandler);
        }
    });
}

//Делает обратно видимым "задизабленный" ранее инпут. Соответствующий ему span удаляется.
$.fn.enableInput = function () {
    return this.each(function () {
        if ($(this).attr("type") == "text" && $(this).css("display") == "none") {

            var label = $(this).prev("span");

            label.remove();
            $(this).unbind('modify', syncFieldsHandler);
            $(this).css("display", "inline");
        }
    });
}

//Вспомогательная функция для работы $.fn.disable
function syncFieldsHandler(eventObject) {
    var textField = $(eventObject.target);
    var label = $(eventObject.data.label);
    label.text(textField.val());
}

//поиск по селектору ячейки в том же ряду
$.fn.findCell = function (selector) {
    var results = $(this).parent("td").parent("tr").find(selector);
    return results;
}

// управление отображением меню доп. функций
$("#feature_menu_pointer").live("click", function () {
    var expanded = true;

    if ($(this).text() == "◄") {
        $(this).text("►").removeClass("expanded").addClass("collapsed");
        $("#feature_menu_content").css("display", "none");
        $("#main_content_wrapper").css("margin-left", "20px");
        expanded = false;
    }
    else {
        $(this).text("◄").addClass("expanded").removeClass("collapsed");
        $("#feature_menu_content").css("display", "block");
        $("#main_content_wrapper").css("margin-left", "250px");
    }

    // устанавливаем состояние меню доп. функций
    $.ajax({
        type: "POST",
        url: "/User/SetFeatureMenuState/",
        data: { expanded: expanded }
    });
});


// Дополнение к стандарному Replace (стандартный не заменяет ВСЕ вхождения)
String.prototype.replaceAll = function (search, replace) {
    return this.split(search).join(replace);
}

// Функция проверки, является ли строка корректным числом
function IsNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

// отображает сообщение об успехе message в контейнере cont_id
function ShowSuccessMessage(message, cont_id) {
    $("#" + cont_id).html('<div class="success_message" title="Скрыть">' + message + '</div>');
    $("#" + cont_id).children("div").delay(1500).animate({ backgroundColor: "white" }, 1500);
    //$("#" + cl).children("div").delay(1500).fadeTo(1000, 0.5);

    StopLinkProgress();
    StopButtonProgress();
    StopGridProgress();
}
// отображает сообщение об ошибке message в контейнере cont_id
function ShowErrorMessage(message, cont_id) {
    $("#" + cont_id).html('<div class="fail_message" title="Скрыть">' + message + '</div>');
    $("#" + cont_id).children("div").delay(1500).animate({ backgroundColor: "white" }, 1500);
    //$("#" + cl).children("div").delay(1500).fadeTo(1000, 0.5);

    StopLinkProgress();
    StopButtonProgress();
    StopGridProgress();
}
$("div.success_message, div.fail_message").live("click", function () {
    $(this).hide();
});


// отображает элемент cont_id в виде модального окна
function ShowModal(cont_id) {
    var elem = $("#" + cont_id);

    if ($('div.modal').length == 0) {
        $('body').append('<div id="fade"></div>');
        $('#fade').css({ 'filter': 'alpha(opacity=40)' }).fadeIn("fast");
        elem.attr("class", "modal").removeAttr('style');
    }
    else if ($('div.modal2').length == 0) {
        $('#fade').remove();
        $('.modal').append('<div id="fade"></div>');
        $('#fade').css({ 'filter': 'alpha(opacity=40)' }).show();
        elem.attr("class", "modal2").removeAttr('style');
    }
    else if ($('div.modal3').length == 0) {
        $('#fade').remove();
        $('.modal2').append('<div id="fade"></div>');
        $('#fade').css({ 'filter': 'alpha(opacity=40)' }).show();
        elem.attr("class", "modal3").removeAttr('style');
    }
    else if ($('div.modal4').length == 0) {
        $('#fade').remove();
        $('.modal3').append('<div id="fade"></div>');
        $('#fade').css({ 'filter': 'alpha(opacity=40)' }).show();
        elem.attr("class", "modal4").removeAttr('style');
    }

    elem.css("top", $(window).height() / 2 - elem.height() / 2);
    elem.css("left", $(window).width() / 2 - elem.width() / 2);
    elem.fadeIn("fast", function () {
        StopLinkProgress();
        StopButtonProgress();
        StopGridProgress();
    });
}
// Отображает всплывающее окно с левым верхним углом в указанной точке
function ShowPopup(cont_id, x, y) {
    $("#" + cont_id).show();
    $("#" + cont_id).css("left", x);
    $("#" + cont_id).css("top", y);

    StopLinkProgress();
    StopButtonProgress();
    StopGridProgress();
}
// Скрывает модальное окно
function HideModal(onComplete) {
    if ($('div.modal4').length != 0) {
        $('.modal4').fadeOut("fast", function () {
            $('#fade').remove();
            $('.modal').append('<div id="fade"></div>');
            $('#fade').css({ 'filter': 'alpha(opacity=40)' }).show();
            $('.modal4').html('').removeAttr('style');
            $('.modal4').removeAttr('class');
            if (onComplete != undefined)
                onComplete();
        });
    }
    else if ($('div.modal3').length != 0) {
        $('.modal3').fadeOut("fast", function () {
            $('#fade').remove();
            $('.modal').append('<div id="fade"></div>');
            $('#fade').css({ 'filter': 'alpha(opacity=40)' }).show();
            $('.modal3').html('').removeAttr('style');
            $('.modal3').removeAttr('class');
            if (onComplete != undefined)
                onComplete();
        });
    }
    else if ($('div.modal2').length != 0) {
        $('.modal2').fadeOut("fast", function () {
            $('#fade').remove();
            $('body').append('<div id="fade"></div>');
            $('#fade').css({ 'filter': 'alpha(opacity=40)' }).show();
            $('.modal2').html('').removeAttr('style');
            $('.modal2').removeAttr('class');
            if (onComplete != undefined)
                onComplete();
        });
    }
    else if ($('div.modal').length != 0) {
        $('.modal').fadeOut("fast", function () {
            $('#fade').remove();
            $('.modal').html('').removeAttr('style');
            $('.modal').removeAttr('class');
            if (onComplete != undefined)
                onComplete();
        });
    }
}
// Скрывает текущее всплывающее окно
function HidePopup() {
    $('.popup').fadeOut("fast");
}

// Устанавливаем валидацию для поля ввода на n знаков до запятой и m знаков после. При performValidation == true перезапускается inobtrusive validation для формы
// (если modalFormSelector задан, то это селектор формы, и для нее и перезапускается валидация).
function SetFieldScale(fieldSelector, n, m, modalFormSelector, performValidation) {
    if (modalFormSelector != undefined)
        fieldSelector = modalFormSelector + " " + fieldSelector;
    var decimalN = TryGetDecimal(n, 0);
    var decimalM = TryGetDecimal(m, 0);
    if (isNaN(decimalN) || isNaN(decimalM))
        return;
    if (decimalN < 1 || decimalN > 20 || decimalM < 0 || decimalM > 20)
        return;
    var maxlength;
    if (decimalM != 0) {
        maxlength = decimalN + decimalM + 1;
        $(fieldSelector).attr("data-val-regex-pattern", "[0-9]{1," + decimalN + "}([,.][0-9]{1," + decimalM + "})?");
        $(fieldSelector).attr("data-val-regex", "Укажите число до " + decimalN + " цифр до запятой и до " + decimalM + " после");
    } else {
        maxlength = decimalN;
        $(fieldSelector).attr("data-val-regex-pattern", "[0-9]{1," + decimalN + "}");
        $(fieldSelector).attr("data-val-regex", "Укажите целое число до " + decimalN + " цифр");
    }
    $(fieldSelector).attr("maxlength", maxlength);
    $(fieldSelector).attr("data-val-length-max", maxlength);
    $(fieldSelector).attr("data-val-length", "Не более " + maxlength + " символов");
    $(fieldSelector).attr("data-val", "true");

    if (modalFormSelector != undefined && IsTrue(performValidation)) {
        $.validator.unobtrusive.parse($(modalFormSelector));
    }
}

// проверяем число на соответствие знаков после запятой
// scale (обязательный параметр) - округляется до целых, должен быть от 0 до N. Задает максимально допустимое число знаков после запятой
// precision (необязательный параметр) - округляется до целых, должен быть от 0 до N. Задает максимально допустимое число знаков до запятой (не задан - не проверяется)
function CheckValueScale(value, scale, precision) {
    var decimalScale = TryGetDecimal(scale, 0);
    if (isNaN(decimalScale) || decimalScale < 0)
        return false;
    var decimalPrecision = TryGetDecimal(precision, 0);

    if (isNaN(decimalPrecision) || decimalPrecision < 0)
        return new RegExp(decimalScale > 0 ? "^[0-9]+([.,][0-9]{1," + decimalScale + "})?$" : "^[0-9]+$").test(value);
    else
        return new RegExp(decimalScale > 0 ? "^[0-9]{1," + decimalPrecision + "}([.,][0-9]{1," + decimalScale + "})?$" : "^[0-9]{1," + decimalPrecision + "}$").test(value);
}

// получить символ '&', параметр backURL и его значение (адрес текущей страницы в закодированном виде)
// если параметр omit передается, то символ '&' не включается в результат, иначе включается
function GetBackUrl(omit) {
    if (omit == undefined)
        return "&backURL=" + MyEncodeURIComponent(window.location);
    else
        return "backURL=" + MyEncodeURIComponent(window.location);
}

function MyEncodeURIComponent(str, maxCountBackUrl) {
    var maxCountBackUrl = 5;
    var myStr = "" + str;
    var count = myStr.split("backURL").length;
    var result = str;
    if (count > maxCountBackUrl) {
        var indexOfBackUrl = myStr.lastIndexOf("backURL");
        var tmpStr = myStr.substring(0, indexOfBackUrl);
        var indexOfPercent = tmpStr.lastIndexOf("%");
        result = myStr.substring(0, indexOfPercent);
    }

    return encodeURIComponent(result);
}

// получить символ '&', параметр backURL и его значение (передаваемая в функцию строка value в закодированном виде)
// если параметр omit передается, то символ '&' не включается в результат, иначе включается
function GetBackUrlFromString(value, omit) {
    if (omit == undefined)
        return "&backURL=" + encodeURIComponent(value);
    else
        return "backURL=" + encodeURIComponent(value);
}


// сделать кнопку недоступной
function DisableButton(button_id) {
    $("#" + button_id).attr("disabled", "disabled").addClass("disabled");
}
// сделать кнопку доступной
function EnableButton(button_id) {
    $("#" + button_id).removeAttr("disabled").removeClass("disabled");
}
// сделать кнопку доступной/недоступной в зависимости от значения булевской переменной
function UpdateButtonAvailability(button_id, state) {
    if (IsTrue(state)) {
        EnableButton(button_id);
    } else {
        DisableButton(button_id);
    }
}
// сменить надпись на кнопке
function UpdateButtonCaption(button_id, caption) {
    $("#" + button_id).attr("value", caption);
}
// сменить надпись на ссылке
function UpdateLinkCaption(element_id, caption) {
    $("#" + element_id).html(caption);
}
// сделать элемент видимым/невидимым в зависимости от значения булевской переменной
function UpdateElementVisibility(element_id, visibility) {
    if (IsTrue(visibility)) {
        $("#" + element_id).show();
    } else {
        $("#" + element_id).hide();
    }
}

// получение значений булевской переменной, переданной любым из способов: через Json или через модель
function IsTrue(value) {
    return ((value == "True") || (value == "true") || (value == true));
}
function IsFalse(value) {
    return (!((value == "True") || (value == "true") || (value == true)));
}

// Проверка строки на нулевое значение (пустой Guid или ноль)
function IsDefaultOrEmpty(value) {
    // Проверка одновременно на null и на undefined
    if (value == null)
        return true;

    var string = value.toString();
    if (string == "")
        return true;

    if (string == "0" || string == "00000000-0000-0000-0000-000000000000")
        return true;

    if (/^(-)?[0]+([.,][0]+)?$/.test(string))
        return true;

    return false;
}

// Возвращает строку, представляющую пустой Guid
function GetEmptyGuid() {
    return "00000000-0000-0000-0000-000000000000";
}

// Приведение числа к формату для отображения
// Параметр scale задает точность, если он не задан, то число обрезается до 6 знаков
function ValueForDisplay(value, scale) {
    var tmp_result = ValueForEdit(value, scale);
    if (tmp_result == "")
        return "";

    var delim_pos = tmp_result.lastIndexOf(".");
    var result = tmp_result;

    if (delim_pos != -1) {
        result = tmp_result.substr(0, delim_pos);
    }

    result = result.replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ');

    if (delim_pos != -1) {
        result = result.concat(".", tmp_result.substr(delim_pos + 1, 20));
    }

    return result;
}

// Приведение числа к формату для редактирования
// scale - округляется до целых, если получается число от 0 до 6, value округляется до данного количества знаков, иначе используется округление до 6 знаков.
// scale может быть не задан, тогда используется округление до 6 знаков.
function ValueForEdit(value, scale) {
    // Проверка одновременно на null и на undefined
    if (value == null)
        return "";

    if (value.toString() == "")
        return "";

    if (isString(value) && !/^(-)?[0-9]+([.,][0-9]+)?$/.test(value.toString()))
        return "";

    if (isString(value))
        value = parseFloat(+value.toString().replace(",", "."));
    if (isNaN(value))
        return "";

    var scaleUsed = 6;
    if (scale != undefined) {
        // Читаем точность через TryGetDecimal, т.е. через ValueForEdit
        scale = TryGetDecimal(scale);
        if (!isNaN(scale)) {
            if (scale >= 0 && scale <= 6)
                scaleUsed = scale;
        }
    }

    return value.toFixedString(scaleUsed, true);
}

// Приведение строки к числу. Возвращает NaN, если строка не является числом
// Параметр scale задает точность, если он не задан, то число обрезается до 6 знаков
function TryGetDecimal(value, scale) {
    var result = ValueForEdit(value, scale);

    return result != "" ? parseFloat(result) : NaN;
}

// "Банковское" ("Бухгалтерское") округление (к четному числу). При округлении до целых число 2,5 округлится до 2, а число 3,5 до 4.
// При округлении до миллионных число 2,0000005 округлится до 2,000000, а число 2,0000015 до 2,000002.
// scale - округляется до целых, если получается число от 0 до 6, value округляется до данного количества знаков, иначе используется округление до 6 знаков.
// scale может быть не задан, тогда используется округление до 6 знаков.
// Возвращает строку (пустую, если входные данные неверны)
function BankRound(value, scale) {
    // Проверка одновременно на null и на undefined
    if (value == null)
        return "";

    if (value.toString() == "")
        return "";

    if (isString(value) && !/^(-)?[0-9]+([.,][0-9]+)?$/.test(value.toString()))
        return "";

    if (isString(value))
        value = parseFloat(+value.toString().replace(",", "."));
    if (isNaN(value))
        return "";

    var scaleUsed = 6;
    if (scale != undefined) {
        // Читаем точность через TryGetDecimal, т.е. через ValueForEdit
        scale = TryGetDecimal(scale);
        if (!isNaN(scale)) {
            if (scale >= 0 && scale <= 6)
                scaleUsed = scale;
        }
    }

    // Округляем с дополнительной точностью +1 цифра, получаем последнюю цифру и идущую за последней.
    // Даже число из 1 цифры даст при этом минимум 2, так что предпоследняя цифра всегда будет существовать
    var extraDigitString = value.toFixedString(scaleUsed + 1, false).replaceAll(".", "").replaceAll("-", "");
    if (extraDigitString == "")
        return "";
    var beforeLastDigit = extraDigitString.substring(extraDigitString.length - 2, extraDigitString.length - 1);
    var lastDigit = extraDigitString.substring(extraDigitString.length - 1);

    // Проверка последней цифры на 4 и 5 нужна, чтобы при 0 не вылезало -0 за счет -eps
    var eps = lastDigit == "5" || lastDigit == "4" ? /0|2|4|6|8/.test(beforeLastDigit) ? -1e-9 : 1e-9 : 0;
    if (value < 0)
        eps = -eps;

    return (value + eps).toFixedString(scaleUsed, true);
}

// Замена стандарного toFixed (стандартный содержит баг в IE6-8). Возвращает строку. Предполагается, что параметр scale целый, положительный, числовой (или не задан)
// Параметр scale должен лежать в диапазоне от 0 до 8 (знаков после запятой), хотя работает и при бОльших числах. Если он не задан, идет округление до целых.
// Параметр removeTrailingZeroes (по умолчанию true) заставляет убирать нули в конце дробной части
Number.prototype.toFixedString = function (scale, removeTrailingZeroes) {
    // Отсеиваем NaN
    if (isNaN(this))
        return "";

    // Проверка одновременно на null и на undefined. Если scale не задан, округляем до целых (scale = 0)
    if (scale == null)
        scale = 0;

    if (removeTrailingZeroes == null)
        removeTrailingZeroes = true;

    // Собственно округление
    var multiplier = Math.pow(10, scale);
    var number = Math.round(multiplier * this) / multiplier;

    // Отсюда можно было переписать весь метод, заменив деление на multiplier разбором строки, но надо было бы учесть все варианты: минус; числа меньше 1
    // Но так явно было бы проще

    var isNegative = (number < 0);
    number = !isNegative ? number : -number;

    // Добавляем 0,1 от последней цифры, чтобы избежать вида 0,49999...
    number += Math.pow(10, -scale - 1);

    // Выделяем целую часть и дробную, находим (дробную часть + 1), чтобы избежать проблемы 3.3333333333333e-7
    var numberInteger = Math.floor(number);
    if (new String(numberInteger).lastIndexOf("e") != -1) // Отсеиваем слишком большие числа
        return "";

    var numberFractional = number - numberInteger;
    var numberFractionalPlus1 = new String(numberFractional + 1);

    // Находим позицию точки, обрезаем лишние разряды
    var numberFractionalPlus1PointPosition = numberFractionalPlus1.lastIndexOf(".");
    var numberFractionalString = numberFractionalPlus1PointPosition != -1 ? numberFractionalPlus1.substring(numberFractionalPlus1PointPosition + 1) : "";
    if (numberFractionalString.length > scale)
        numberFractionalString = numberFractionalString.substring(0, scale);
    if (!removeTrailingZeroes) {
        while (numberFractionalString.length < scale) {
            numberFractionalString += "0";
        }
    }

    // Обрезаем лишние нули в дробной части и саму десятичную точку, если дробная часть нулевая
    if (removeTrailingZeroes) {
        while (numberFractionalString.length > 0 && numberFractionalString.charAt(numberFractionalString.length - 1) == "0") {
            numberFractionalString = numberFractionalString.substring(0, numberFractionalString.length - 1);
        }
    }

    return ((isNegative ? "-" : "") + new String(numberInteger) + (numberFractionalString != "" ? "." + numberFractionalString : "")).toString();
}

// Преобразование даты (объекта типа Date) в формат ДД.ММ.ГГ или ДД.ММ.ГГГГ
function dateToString(date, digits) {
    if (digits == undefined) digits = 4;
    yearStr = date.getFullYear().toString();
    monthStr = (date.getMonth() + 1).toString();
    dayStr = date.getDate().toString();

    if (digits == 2) yearStr = yearStr.slice(2);
    if (monthStr.length == 1) monthStr = "0" + monthStr;
    if (dayStr.length == 1) dayStr = "0" + dayStr;

    var dateStr = dayStr + "." + monthStr + "." + yearStr;

    return dateStr;
}

// приведение даты из строки в формате ToShortDateString() во внутренний формат JavaScript Date()
function stringToDate(value) {
    return new Date(value.replace(/(\d+).(\d+).(\d+)/, '$3/$2/$1'));
}

// Проверка, валидна ли дата во внутреннем формате
function isValidDate(d) {
    if (Object.prototype.toString.call(d) !== "[object Date]")
        return false;

    return !isNaN(d.getTime());
}

// Возвращает булевскую переменную, говорящую, является ли первый аргумент строкой
function isString() {
    if (typeof arguments[0] == 'string')
        return true;

    if (typeof arguments[0] == 'object') {
        var criterion = arguments[0].constructor.toString().match(/string/i);

        return (criterion != null);
    }

    return false;
}

// Расчет суммы, остающейся после взимания НДС (суммы без НДС). Возвращает NaN, если переданы неверные значения. Не округляет
// sumWithVat - сумма с НДС, vatPercent - ставка НДС
function CalculateWithoutVatSum(sumWithVat, vatPercent) {
    return TryGetDecimal(sumWithVat) / (1 + TryGetDecimal(vatPercent) / 100);
}

// Расчет суммы взимаемой НДС
// sumWithVat - сумма с НДС, vatPercent - ставка НДС. Возвращает NaN, если переданы неверные значения. Не округляет
function CalculateVatSum(sumWithVat, vatPercent) {
    return TryGetDecimal(sumWithVat) - CalculateWithoutVatSum(sumWithVat, vatPercent);
}

// клиентская валидация
// источник вызова - элемент input, чье содержание валидируется
// predicate - проверяемый предикат с 1 параметром - значением поля ввода
// message - выдаваемое сообщение об ошибке
// validationMessageId - id элемента, куда писать message. Если  элемент с этим id не будет найден, то валидация не произойдет.
// Если указать null, то будет автоматически создан span в конце родительского элемента с id = %input_id%_complexValidationMessage
$.fn.Validate = function (predicate, message, validationMessageId) {
    //    var defaultValMsg_id = this.attr('id') + '_validationMessage';
    //    var defaultValMsg = $('#' + defaultValMsg_id);

    //    if (defaultValMsg.length) { return false; }
    if (validationMessageId == null) { validationMessageId = this.AddValidationMessage(); }

    var validationMessage = $('#' + validationMessageId);

    if (!validationMessage.length) { return false; }

    var value = this.attr('value');
    if (predicate(value)) {
        if (validationMessage.text() == message) {
            validationMessage.removeClass('field-validation-error').addClass('field-validation-valid');
            this.removeClass('input-validation-error').addClass('input-validation-valid');
            validationMessage.text("");
        }
    }
    else {
        validationMessage.text(message);
        validationMessage.removeClass('field-validation-valid').addClass('field-validation-error');
        this.removeClass('input-validation-valid').addClass('input-validation-error');
    }
    //    return this;
}

// клиентская валидация для пары связанных элементов
// input1_id - первое поле ввода
// input2_id - второе поле ввода
// predicate - проверяемый предикат с 2 параметрами - значениями полей ввода
// message - выдаваемое сообщение об ошибке
// validationMessageId - id элемента, куда писать message. Если  элемент с этим id не будет найден, то валидация не произойдет.
function ValidatePair(input1_id, input2_id, predicate, message, validationMessageId) {
    var validationMessage = $('#' + validationMessageId);

    if (!validationMessage.length) { return false; }

    var input1 = $('#' + input1_id);
    var input2 = $('#' + input2_id);
    var value1 = input1.attr('value');
    var value2 = input2.attr('value');

    if (predicate(value1, value2)) {
        if (validationMessage.text() == message) {
            validationMessage.removeClass('field-validation-error').addClass('field-validation-valid');
            input1.removeClass('input-validation-error').addClass('input-validation-valid');
            input2.removeClass('input-validation-error').addClass('input-validation-valid');
            validationMessage.text("");
        }
    }
    else {
        validationMessage.text(message);
        validationMessage.removeClass('field-validation-valid').addClass('field-validation-error');
        input1.removeClass('input-validation-valid').addClass('input-validation-error');
        input2.removeClass('input-validation-valid').addClass('input-validation-error');
    }
}

//внутренняя функция, используемая в Validate
// добавляет span в конце родительского элемента с id = %input_id%_complexValidationMessage, если уже есть - только возвращает его id
$.fn.AddValidationMessage = function () {
    var input_id = this.attr('id');
    var validationMessage_id = input_id + '_complexValidationMessage';
    if (!$('#' + validationMessage_id).length) {
        var validationMessage = $('<span/>', { id: input_id + '_complexValidationMessage' });
        this.parent().append(validationMessage);
    }

    return validationMessage_id;
}

//связывание комбобоксов
// источник - родительский комбобокс
// childId - id дочернего
// methodPath - путь к методу контроллера, который принимает выбранный элемент родительского и возвращает список значений для дочернего
// parameterName - имя параметра, передаваемого в метод контроллера
// errorMessageId - id элемента, в который писать сообщение об ошибке
$.fn.FillChildComboBox = function (childId, methodPath, parameterName, errorMessageId) {
    var parentId = this.attr('id');
    var parentComboBox = this;
    var childComboBox = $('#' + childId);
    this.bind("keyup change", function () {
        childComboBox.attr('disabled', 'disabled');

        var selectedId = parentComboBox.val();
        if (selectedId == "") {
            childComboBox.clearSelect();
        }
        else {
            StartComboBoxProgress($("#" + childId));

            $.ajax({
                type: "GET",
                url: methodPath,
                data: parameterName + '=' + selectedId,
                success: function (result) {
                    if (result != 0) {
                        childComboBox.fillSelect(result);
                        childComboBox.removeAttr('disabled').removeClass('input-validation-error');
                        $('#' + childId + '_validationMessage').addClass('field-validation-valid').removeClass('field-validation-error');
                        StopComboBoxProgress($("#" + childId));
                    }
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, errorMessageId);
                    StopComboBoxProgress($("#" + childId));
                }
            });
        }
    });
}

//сделать поле ввода красным, как при ошибке валидации
//сделать видимым сообщение ошибки валидации, записать в него сообщение message
//поле для сообщения валидации берется по умолчанию, генерируемое ASP.NET MVC и устанавливаемое на форму через Html.ValidationMessageFor(...)
$.fn.ValidationError = function (message) {
    var name = this.attr('name');
    var valMsg = $('[data-valmsg-for="' + name + '"]');
    this.addClass('input-validation-error').removeClass('input-validation-valid');
    if (valMsg.length) {
        valMsg.addClass('field-validation-error').removeClass('field-validation-valid').text(message);
    }

    return this;
}

//убрать с поля ввода выделение красным цветом
//сделать невидимым сообщение ошибки валидации, стереть его содержимое
//поле для сообщения валидации берется по умолчанию, генерируемое ASP.NET MVC и устанавливаемое на форму через Html.ValidationMessageFor(...)
$.fn.ValidationValid = function () {
    var name = this.attr('name');
    var valMsg = $('[data-valmsg-for="' + name + '"]');
    this.addClass('valid').removeClass('input-validation-error');
    if (valMsg.length) {
        valMsg.addClass('field-validation-valid').removeClass('field-validation-error').text("");
        valMsg.empty();
    }

    return this;
}

jQuery.validator.unobtrusive.adapters.addSingleVal("greaterorequalbyproperty", "valuetocompare");

jQuery.validator.addMethod("greaterorequalbyproperty", function (value, element, param) {
    var valueToCompare = param.replaceAll(',', '.');
    value = value.replaceAll(',', '.');

    return (parseFloat(value) >= parseFloat(valueToCompare));
});

jQuery.validator.unobtrusive.adapters.addSingleVal("greaterbyconst", "consttocompare");

jQuery.validator.addMethod("greaterbyconst", function (value, element, param) {
    var constToCompare = param.replaceAll(',', '.');
    value = value.replaceAll(',', '.');

    return (parseFloat(value) > parseFloat(constToCompare));
});



jQuery.validator.unobtrusive.adapters.add('requiredbyradiobutton', ['radiobuttongroupname', 'valuetocompare'], function (options) {
    options.rules["requiredbyradiobutton"] = options.params;
    options.messages["requiredbyradiobutton"] = options.message;
});

jQuery.validator.addMethod("requiredbyradiobutton", function (value, element, param) {
    var radiobuttongroupname = param['radiobuttongroupname'];
    var valuetocompare = param['valuetocompare'];

    var rbValue = $('[name="' + radiobuttongroupname + '"]:checked').val();

    return value != "" || rbValue != valuetocompare;
});



jQuery.validator.unobtrusive.adapters.add('rangebyradiobutton', ['radiobuttongroupname', 'valuetocompare', 'min', 'max'], function (options) {
    options.rules["rangebyradiobutton"] = options.params;
    options.messages["rangebyradiobutton"] = options.message;
});

jQuery.validator.addMethod("rangebyradiobutton", function (value, element, param) {
    var radiobuttongroupname = param['radiobuttongroupname'];
    var valuetocompare = param['valuetocompare'];
    var minimum = parseFloat(param['min'].replaceAll(",", "."));
    var maximum = parseFloat(param['max'].replaceAll(",", "."));
    value = parseFloat(value.replaceAll(",", "."));

    var rbValue = $('[name="' + radiobuttongroupname + '"]:checked').val();

    return (minimum <= value && value <= maximum) || rbValue != valuetocompare;
});



jQuery.validator.unobtrusive.adapters.add('regexbyradiobutton', ['radiobuttongroupname', 'valuetocompare', 'regex'], function (options) {
    options.rules["regexbyradiobutton"] = options.params;
    options.messages["regexbyradiobutton"] = options.message;
});

jQuery.validator.addMethod("regexbyradiobutton", function (value, element, param) {
    var radiobuttongroupname = param['radiobuttongroupname'];
    var valuetocompare = param['valuetocompare'];
    var pattern = param['regex'];

    var regexp = new RegExp(pattern);

    var rbValue = $('[name="' + radiobuttongroupname + '"]:checked').val();

    return regexp.test(value) || rbValue != valuetocompare;
});

jQuery.validator.unobtrusive.adapters.add('comparewithproperty', ['valuetocompare', 'comparetype'], function (options) {
    options.rules["comparewithproperty"] = options.params;
    options.messages["comparewithproperty"] = options.message;
});

jQuery.validator.addMethod("comparewithproperty", function (value, element, param) {
    var right = parseFloat(param['valuetocompare']);
    var comparetype = parseInt(param['comparetype']);
    var left = parseFloat(value.replaceAll(',', '.'));

    switch (comparetype) {
        case 1: return (left == right); break;
        case 2: return (left < right); break;
        case 3: return (left <= right); break;
        case 4: return (left >= right); break;
        case 5: return (left > right); break;
    }
});


/* Date validator
******************************************************************/
jQuery.validator.addMethod("isdate", function (value, element, param) {
    var regExp = new RegExp("[0-3]?[0-9].[0-1]?[0-9].[1-9][0-9]{3}");
    var arr = regExp.exec(value);

    var result = value == '' || (arr != null && arr[0] == value);

    return result;
});

jQuery.validator.unobtrusive.adapters.add('isdate', [], function (options) {
    options.rules["isdate"] = options.params;
    options.messages["isdate"] = options.message;
});

/* Tree
****************************************************************/
$(".tree_node_expander").live('click', function () {
    if (!$(this).hasClass("expanded")) {
        $(this).addClass("expanded").html("&#9660;");

        $(this).closest(".tree_node").next(".tree_node_childs").removeClass("hidden");
    }
    else {
        $(this).removeClass("expanded").html("&#9658;");

        $(this).closest(".tree_node").next(".tree_node_childs").addClass("hidden");
    }
});

$('.tree_node').live("click", function () {
    $(this).closest(".tree_table").find(".selected").removeClass("selected");
    $(this).addClass("selected");
});


/* YesNoToggle
****************************************************************/
$(".yes_no_toggle").live("click", function () {
    if ($(this).next("input").val() == "0") {
        $(this).next("input").val("1");
        $(this).html("Да");
    }
    else {
        $(this).next("input").val("0");
        $(this).html("Нет");
    }
});

/* Loading Bar
****************************************************************/
// Отобразить индикатор выполнения задачи по нажатию на кнопку
function StartButtonProgress(btn, parent_id) {
    if (parent_id == undefined) {
        var _btn = $("#" + btn.attr("id"));
    }
    else {
        var _btn = $("#" + parent_id + " #" + btn.attr("id")); // сейчас используется для кнопок фильтра
    }

    _btn.before("<input type=\"button\" class=\"button_loading\" value=\" \" />"); // отображаем индикатор
    $(".button_loading").css("width", (_btn.outerWidth()) + "px");  // выставляем ширину индикатора = ширине кнопки
    $(".button_loading").css("height", (_btn.outerHeight()) + "px");  // выставляем высоту индикатора = высоте кнопки

    // если не ie
    if ($.browser.msie == undefined) {
        _btn.attr("old_type", _btn.attr("type"));   // запоминаем исходный тип кнопки
        _btn.get(0).type = "hidden"; // меняем тип кнопки на hidden
    }
    else {
        _btn.addClass("clear_button");
    }
}
// Скрыть активный индикатор выполнения задачи по нажатию на кнопку
function StopButtonProgress() {
    if ($(".button_loading").length != 0) {   // если есть активный индикатор для кнопки
        // если не ie
        if ($.browser.msie == undefined) {
            $(".button_loading").next("input").get(0).type = $(".button_loading").next("input").attr("old_type"); // восстанавливаем исходный тип кнопки            
        }
        else {
            $(".button_loading").next("input").removeClass("clear_button");
        }

        $(".button_loading").remove(); // удаляем индикатор прогресса
    }
}
// Отобразить индикатор выполнения задачи по нажатию на псевдоссылку
function StartLinkProgress(link) {
    var imgName = "";

    if (link.hasClass("main_details_action")) {
        imgName = "button_loading";
    }
    else {
        imgName = "link_loading";
    }
    link.before("<img src=\"/Content/Img/" + imgName + ".gif\" class=\"link_loading\" alt=\"загрузка\" />"); // отображаем индикатор   

    link.prev(".link_loading").css("padding-left", link.css("padding-left"));

    // если ширина ссылки больше ширины индикатора - то увеличиваем ширину индикатора
    var dif = link.outerWidth() - link.prev(".link_loading").outerWidth();
    if (dif > 0) {
        link.prev(".link_loading").css("margin-right", dif + "px");
    }

    link.hide(); // скрываем ссылку
}
// Скрыть индикатор выполнения задачи по нажатию на псевдоссылку
function StopLinkProgress() {
    if ($(".link_loading").length != 0) {   // если есть активный индикатор для ссылки
        $(".link_loading").next("span").show(); // отображаем ссылку
        $(".link_loading").remove();  // удаляем индикатор
    }
}
$(".select_link:not(.no_auto_progress), .edit_action:not(.no_auto_progress), .main_details_action:not(.no_auto_progress), .selector_link:not(.no_auto_progress)").live("click", function () {
    StartLinkProgress($(this));
});

// Отобразить индикатор выполения для выпадающего списка
function StartComboBoxProgress(combo) {
    if (combo.get(0).tagName == "SELECT") { // отображаем индикатор только для тега SELECT
        combo.before("<img src=\"/Content/Img/link_loading.gif\" class=\"link_loading\" alt=\"загрузка\" />"); // отображаем индикатор
        combo.prev(".link_loading").css("margin-top", ((TryGetDecimal(combo.outerHeight(), 0) - 6) / 2) + "px");

        var dif = combo.outerWidth() - combo.prev(".link_loading").outerWidth();
        if (dif > 0) {
            combo.prev(".link_loading").css("margin-right", dif + "px");
        }

        combo.hide();
    }
}
// Скрыть индикатор выполения для выпадающего списка
function StopComboBoxProgress(combo) {
    if (combo.get(0).tagName == "SELECT") { // скрываем индикатор только для тега SELECT
        combo.show();
        combo.prev(".link_loading").remove();
    }
}

// Отобразить индикатор выполения для грида
function StartGridProgress(grid) {
    var gridHeight = parseInt(grid.css("height").toString().replace("px", ""));

    grid.append("<div class=\"grid_loading_container\" style='" +
        "margin-top: -" + (gridHeight + 4) + "px;" +
        "height:" + (gridHeight + 4) + "px;" +
        "'>" +
            "<div class='grid_loading' style='" +
            "margin-top: -" + (gridHeight + 4) + "px;" +
            "height:" + (gridHeight + 4) + "px;" +
            "'></div>" +

            "<img src=\"/Content/Img/link_loading.gif\" style='display: block; z-index: 6000; position: relative; margin: 0 auto;" +
            "margin-top: -" + (gridHeight / 2 + 2) + "px;" +
            "' />" +
    "</div>");

}
// Скрыть индикатор выполения для грида
function StopGridProgress(grid) {
    if (grid != undefined) {
        grid.find(".grid_loading_container").remove();
    }
    else {
        $(".grid_loading_container").remove();
    }
}
// Автоматический запуск индикатора выполения для ссылок действий внутри грида
$(".grid_table b.link").live("click", function () {
    // если это не кнопка "Удалить"
    if ($(this).attr("class").toString().toLowerCase().indexOf('del') == -1 && $(this).attr("class").toString().toLowerCase().indexOf('rem') == -1) {
        StartGridProgress($(this).closest(".grid"));
    }
});
