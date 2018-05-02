$(document).ready(function () {
    window.onresize = ResizeContentWrapper;
    window.onload = ResizeContentWrapper;

    function ResizeContentWrapper() {
        CorrectHelpPosition();
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

    // добавление/сокрытие полосы прокрутки у окна просмотра комментария
    $('.comment').each(function () {
        if ($(this).height() < 60) {
            $(this).css('overflow-y', 'none');
        }
        else {
            $(this).css('overflow-y', 'auto');
        }
    });
});                         // document ready


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

    //Показываем затемнение и создаем модальное окно
    if ($('div.modal').length == 0) {
        //при изменении размеров окна корректируем позиции модальных окон, размеры тени и экрана
        $(window).bind("resize", CorrectModalOnWindowResize);
        MakePreparationToOpenDialog(elem, undefined);
        elem.attr("class", "modal").removeAttr('style');
    }
    else if ($('div.modal2').length == 0) {
        MakePreparationToOpenDialog(elem, $('.modal'));
        elem.attr("class", "modal2").removeAttr('style');
    }
    else if ($('div.modal3').length == 0) {
        MakePreparationToOpenDialog(elem, $('.modal2'));
        elem.attr("class", "modal3").removeAttr('style');
    }
    else if ($('div.modal4').length == 0) {
        MakePreparationToOpenDialog(elem, $('.modal3'));
        elem.attr("class", "modal4").removeAttr('style');
    }

    

    // если найден заголовок модальной формы - делаем форму перетаскиваемой
    if (elem.find('.modal_title').length != 0) {
        elem.draggable({
            start: CorrectHelpPosition,
            drag: CorrectHelpPosition,
            stop: CorrectHelpPosition,
            handle: '.modal_title',
            cancel: '.help-btn'
        });
    }

    SetModalBoundedPosition(elem, undefined, undefined);

    //отображаем окно
    elem.fadeIn("fast", function () {
        StopLinkProgress();
        StopButtonProgress();
        StopGridProgress();
    });
}

function CorrectModalOnWindowResize() {

    var lastWidth = $('#fade').width();
    var lastHeight = $('#fade').height();
    //Т.к. после того как у тени мы сделали position:absolute то она теперь является частью страницы и мешает ей сжиматься
    //Устанавливаем ей нулевую ширину и высоту и даем странице сжаться до нужных размеров
    $('#fade').width(0).height(0);
    $('.screen').width(0).height(0);

    //Запоминаем размеры страницы
    var maxHeight = $(".page").height();
    var maxWidth = $(".page").width();

    //Корректируем позицию модального окна когда оно выходит за пределы страницы (только при уменьшении окна)
    $(".modal, .modal2, .modal3, .modal4 ").each(function (index) {
        var newLeftPosition = undefined;
        var newTopPosition = undefined;

        if (($(this).width() + $(this).offset().left) > maxWidth && maxWidth < lastWidth) {
            newLeftPosition = $(this).position().left - ($(this).width() + $(this).offset().left - maxWidth);
        }

        if (($(this).height() + $(this).offset().top) > maxHeight && maxHeight < lastHeight) {
            newTopPosition = $(this).position().top - ($(this).height() + $(this).offset().top - maxHeight);
        }

        if (newLeftPosition != undefined || newTopPosition != undefined) {
            SetModalBoundedPosition($(this), newTopPosition, newLeftPosition);
        }
    });

    //Устанавливаем новые размеры для тени и экрана
    ShowFade($('#fade'));
    SetScreen($('.screen'));
    return;
}

//Функция корректировки положения хэлпа
function CorrectHelpPosition() {
    var btn = $(".help-btn-active").first();
    if (btn.length != 0) {
        var popup = $('#help_popup');
        var maxWidth = getScrollLeft() + getInnerWidth();
        var maxHeight = getScrollTop() + getInnerHeight();
        
        var top = btn.offset().top + 20;
        var left = btn.offset().left - 10;

        if (left + popup.width() > maxWidth || top + popup.height() > maxHeight) {
            closeHelp(); 
        }
        else {
            popup.css({ "top": top, "left": left });
        }
    }
}

//проверяем новые координаты модального окна(что не вышли за границу окна браузера) и устанавливаем их
function SetModalBoundedPosition(elem, top, left) {
    //максимальные значения top и left относительно краев документа
    var minTop = 0;
    var maxTop = $(document).height() - elem.height();
    var minLeft = 0;
    var maxLeft = $(document).width() - elem.width();
    var offsetParent = GetOffsetParent(elem);

    //Если координаты неизвестны
    //пытаемся поставить по центру экрана
    if (top == undefined) {
        top = getScrollTop() + getInnerHeight() / 2 - elem.height() / 2 - offsetParent.offset().top;
    }
    if (left == undefined) {
        left = getScrollLeft() + getInnerWidth() / 2 - elem.width() / 2 - offsetParent.offset().left;
    }

    //проверяем не вышли ли за пределы документа
    if ((top + offsetParent.offset().top) > maxTop) top = maxTop - offsetParent.offset().top;
    if ((top + offsetParent.offset().top) < minTop) top = minTop - offsetParent.offset().top;
    if ((left + offsetParent.offset().left) > maxLeft) left = maxLeft - offsetParent.offset().left;
    if ((left + offsetParent.offset().left) < minLeft) left = minLeft - offsetParent.offset().left;

    //Если у модалки есть дети модалки, то восстановить их позицию на экране
    var child = $(elem).find(".modal2, .modal3, .modal4").first();
    if (child.length != 0) {
        //Запоминаем позицию потомка
        var childOffset = child.offset();
        //Устанавливаем положение окна
        elem.css("top", top);
        elem.css("left", left);
        //Восстанавливаем позицию потомка
        child.css("top", childOffset.top - elem.offset().top - 1);
        child.css("left", childOffset.left - elem.offset().left - 1);
    }
    else {
        //Устанавливаем положение окна
        elem.css("top", top);
        elem.css("left", left);
    }
}

//Получить первого предка с абсолютным позиционированием или если его нет то body
function GetOffsetParent(elem) {
    var parent = elem.parents().filter(function () { return $(this).css("position") == 'absolute' }).first();
    if (parent.length == 0) return $('body');
    else return parent;
}

//Кросс браузерные методы для получения ScrollTop и ScrollLeft
function getScrollTop() {
    if (window.scrollY != undefined) {
        //most browsers
        return window.scrollY;
    }
    else {
        var B = document.body; //IE 'quirks'
        var D = document.documentElement; //IE with doctype
        D = (D.clientHeight) ? D : B;
        return D.scrollTop;
    }
}

function getScrollLeft() {
    if (window.scrollX != undefined) {
        //most browsers
        return window.scrollX;
    }
    else {
        var B = document.body; //IE 'quirks'
        var D = document.documentElement; //IE with doctype
        D = (D.clientWidth) ? D : B;
        return D.scrollLeft;
    }
}
//Кросс браузерные методы для определение window.innerheight и window.innerwidth
function getInnerHeight() {
    if (window.scrollX != undefined) {
        //Non-IE
        return window.innerHeight;
    }
    else {
        var B = document.body; //IE 'quirks'
        var D = document.documentElement; //IE with doctype
        D = (D.clientHeight) ? D : B;
        return D.clientHeight;
    }
}

function getInnerWidth() {
    var myWidth = 0;
    if (window.scrollX != undefined) {
        //Non-IE
        return window.innerWidth;
    }
    else {
        var B = document.body; //IE 'quirks'
        var D = document.documentElement; //IE with doctype
        D = (D.clientWidth) ? D : B;
        return D.clientWidth;
    }
}
//Кросс браузерные методы для определение document.width и document.height
function getDocumentHeight() {
    if (window.scrollX != undefined) {
        //Non-IE
        return $(document).height();
    }
    else {
        var B = document.body; //IE 'quirks'
        var D = document.documentElement; //IE with doctype
        D = (D.scrollHeight) ? D : B;
        return D.scrollHeight;
    }
}

function getDocumentWidth() {
    var myWidth = 0;
    if (window.scrollX != undefined) {
        //Non-IE
        return $(document).width();
    }
    else {
        var B = document.body; //IE 'quirks'
        var D = document.documentElement; //IE with doctype
        D = (D.scrollWidth) ? D : B;
        return D.scrollWidth;
    }
}


// Отображает всплывающее окно с левым верхним углом в указанной точке
function ShowPopup(cont_id, x, y) {
    var elem = $("#" + cont_id);

    elem.css("left", x).css("top", y).show();
    
    // если найден заголовок модальной формы - делаем форму перетаскиваемой
    if (elem.find('.modal_title').length != 0) {
        elem.draggable({ handle: '.modal_title', cancel: '.help-btn' });
    }

    StopLinkProgress();
    StopButtonProgress();
    StopGridProgress();
}

//Установка размеров и позиции затемнения
function ShowFade(fade) {
    var offsetParent = GetOffsetParent(fade);
    fade.css({ "width": getDocumentWidth(), "height": getDocumentHeight(), "top": -1 * offsetParent.offset().top , "left": -1 * offsetParent.offset().left  }).show();
}

//Установка позиции и размеров экрана
function SetScreen(screen) {
    var offsetParent = GetOffsetParent(screen);
    screen.css({ "top": -1 * offsetParent.offset().top , "left": -1 * offsetParent.offset().left , "width": getDocumentWidth(), "height": getDocumentHeight() });
}

//Приготовления перед открытием окна
function MakePreparationToOpenDialog(elem, prev) {
    if (prev != undefined) { //если это не первое диалоговое окно
        $('#fade').remove();
        if (elem != undefined) {
            var parentmodal = elem.closest(".modal, .modal2, .modal3, .modal4");
            if (parentmodal.length == 0) { //если у окна нет другого родительского модального окна, то ему нужен свой экран
                //Устанавливаем экран следующему окну
                elem.wrap('<div class="screen"></div>');
                //Устанавливаем размеры экрана
                SetScreen($('.screen'));
            }
            //Устанавливаем тень на предыдущий элемент
            prev.append('<div id="fade"></div>');
            //Отображаем тень
            ShowFade($('#fade'));
        }
    }
    else { //Если это первое модальное окно
        //Устанавливаем экран окну
        elem.wrap('<div class="screen"></div>');
        //Устанавливаем размеры экрана
        SetScreen($('.screen'));
        //Устанавливаем тень на тело
        $('body').append('<div id="fade"></div>');
        //Отображаем тень
        ShowFade($('#fade'));
    }
}

//Приготовления перед закрытием окна
function MakePreparationToCloseDialog(elem, next) {
    //Убираем затемнение
    $('#fade').remove();
    if (elem.parent().attr("class") == "screen") 
        elem.unwrap(); //Если у модального окна был свой экран, удаляем его
    if (next != undefined) { //Не последнее окно
        //Устанавливаем тень
        next.append('<div id="fade"></div>')
        //Отображаем тень
        ShowFade($('#fade'));
    }
}

// Скрывает модальное окно
function HideModal(onComplete) {
    if ($('div.modal4').length != 0) {
        $('.modal4').fadeOut("fast", function () {
            MakePreparationToCloseDialog($('.modal4'), $('.modal2'));
            $('.modal4').html('').removeAttr('style');
            $('.modal4').removeAttr('class');
            if (onComplete != undefined)
                onComplete();
        });
    }
    else if ($('div.modal3').length != 0) {
        $('.modal3').fadeOut("fast", function () {
            MakePreparationToCloseDialog($('.modal3'), $('.modal'));
            $('.modal3').html('').removeAttr('style');
            $('.modal3').removeAttr('class');
            if (onComplete != undefined)
                onComplete();
        });
    }
    else if ($('div.modal2').length != 0) {
        $('.modal2').fadeOut("fast", function () {
            MakePreparationToCloseDialog($('.modal2'), $('body'));
            $('.modal2').html('').removeAttr('style');
            $('.modal2').removeAttr('class');
            if (onComplete != undefined)
                onComplete();
        });
    }
    else if ($('div.modal').length != 0) {
        $('.modal').fadeOut("fast", function () {
            MakePreparationToCloseDialog($('.modal'), undefined);
            $('.modal').html('').removeAttr('style');
            $('.modal').removeAttr('class');
            $(window).unbind("resize");
            if (onComplete != undefined)
                onComplete();
        });
    }
}

// Скрывает текущее всплывающее окно
function HidePopup() {
    $('.popup').fadeOut("fast");
}

//Модальный диалог потверждения
//в случае нажатия на Ок не закрывается, необходимо это делать в onOk.onSuccess
function ShowConfirm(title, message, btnOkTitle, btnCancelTitle, onOk, onCancel) {

    var btnConfirmOk_text = "";
    if (btnOkTitle !== undefined)
        btnConfirmOk_text = btnOkTitle;
    else
        btnConfirmOk_text= "Да";

    var btnConfirmCancel_text = "";
    if (btnCancelTitle !== undefined)
       btnConfirmCancel_text = btnCancelTitle;
    else
        btnConfirmCancel_text = "Отмена";

    var messageConfirm = "<div id=\"messageConfirm\">" + message + "</div>";
    var button_set = "<div class=\"button_set\">" +
        "<input id=\"btnConfirmOk\" type=\"button\" value=\"" + btnConfirmOk_text + "\"/>" +
        "<input id=\"btnConfirmCancel\" type=\"button\" value=\"" + btnConfirmCancel_text + "\"/>" +
        "</div>";

    var div_form = "<div style=\"padding: 10px 20px 5px; max-width: 300px;\">" + messageConfirm + button_set + "</div>";

    $("#confirm").append("<div class=\"modal_title\">" + title + "</div>");
    $("#confirm").append("<div class=\"h_delim\"></div>");

    $("#confirm").append(div_form);
    
    ShowModal("confirm");

    if (onOk !== undefined) {
        $("#btnConfirmOk").click(function () {
            StartButtonProgress($(this));
            onOk();
            $(this).unbind('click');
        });
    }

    $("#btnConfirmCancel").click(function () {
        if (onCancel !== undefined)
            onCancel();
        $(this).unbind('click');
        $("#btnConfirmOk").unbind('click');
        HideModal();
    });
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
// Параметр removeTrailingZeroes (по умолчанию true) заставляет убирать нули в конце дробной части
function ValueForDisplay(value, scale, removeTrailingZeroes) {
    var tmp_result = ValueForEdit(value, scale, removeTrailingZeroes);
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
// Параметр removeTrailingZeroes (по умолчанию true) заставляет убирать нули в конце дробной части
function ValueForEdit(value, scale, removeTrailingZeroes) {
    // Проверка одновременно на null и на undefined
    if (value == null)
        return "";

    if (removeTrailingZeroes === undefined) {
        removeTrailingZeroes = true;
    }

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

    return value.toFixedString(scaleUsed, removeTrailingZeroes);
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
// noEmptyOption - не добавлять пустой элемент
$.fn.FillChildComboBox = function (childId, methodPath, parameterName, errorMessageId, noEmptyOption, callback) {
    var parentId = this.attr('id');
    var parentComboBox = this;
    var childComboBox = $('#' + childId);
    this.bind("keyup change", function () {
        childComboBox.attr('disabled', 'disabled');

        var selectedId = parentComboBox.val();
        if (selectedId == "") {
            childComboBox.clearSelect();
            if (callback !== undefined)
                callback();
        }
        else {
            StartComboBoxProgress($("#" + childId));

            $.ajax({
                type: "GET",
                url: methodPath,
                data: parameterName + '=' + selectedId,
                success: function (result) {
                    if (result != 0) {
                        childComboBox.fillSelect(result, noEmptyOption);
                        childComboBox.removeAttr('disabled').removeClass('input-validation-error');
                        $('#' + childId + '_validationMessage').addClass('field-validation-valid').removeClass('field-validation-error');
                        StopComboBoxProgress($("#" + childId));
                        if (callback !== undefined)
                            callback();
                    }
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, errorMessageId);
                    StopComboBoxProgress($("#" + childId));
                    if (callback !== undefined)
                        callback();
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

/* PermissionModel
****************************************************************/
function GetLastPDPosition(possible_values, parent_value) {
    // 1111
    if (possible_values[1] == "1" && possible_values[2] == "1") {
        for (var i = parent_value; i >= 0; i--) {
            if (possible_values[i] == "1") {
                return i;
            }
        }
    }
    // 1001
    else if (possible_values[1] == "0" && possible_values[2] == "0") {
        if (parent_value == "0") {
            return 0;
        }
        else {
            return 3;
        }
    }
    // 1011
    else if (possible_values[1] == "0" && possible_values[2] == "1") {
        if (parent_value == "1") {
            return 2;
        }
        else {
            return parent_value;
        }
    }
    // 1101
    else if (possible_values[1] == "1" && possible_values[2] == "0") {
        if (parent_value == "2") {
            return 3;
        }
        else {
            return parent_value;
        }
    }
}
function SetPermissionDistribution0State(id, isActive, lastPDPosition) {
    var el = $("#" + id);

    el.nextAll(".pd0").removeClass("pd_0_passive").removeClass("pd_0_active").removeClass("pd_0_active_last");

    if (isActive) {
        if (lastPDPosition == 0) {
            el.nextAll(".pd0").addClass("pd_0_active_last");
        }
        else {
            el.nextAll(".pd0").addClass("pd_0_active");
        }
    }
    else {
        el.nextAll(".pd0").addClass("pd_0_passive");
    }
}
function SetPermissionDistribution1State(id, isActive, lastPDPosition) {
    var el = $("#" + id);

    el.nextAll(".pd1").removeClass("pd_1_passive").removeClass("pd_1_active").removeClass("pd_1_active_last").removeClass("pd_1_passive_last");

    if (lastPDPosition < "1") {
        el.nextAll(".pd1").css("display", "none");
    }
    else {
        el.nextAll(".pd1").css("display", "block");

        if (!el.nextAll(".pd1").hasClass("pd_empty")) {
            if (lastPDPosition == 1) {
                if (isActive) {
                    el.nextAll(".pd1").addClass("pd_1_active_last");
                }
                else {
                    el.nextAll(".pd1").addClass("pd_1_passive_last");
                }
            }
            else {
                if (isActive) {
                    el.nextAll(".pd1").addClass("pd_1_active");
                }
                else {
                    el.nextAll(".pd1").addClass("pd_1_passive");
                }
            }
        }
    }
}
function SetPermissionDistribution2State(id, isActive, lastPDPosition) {
    var el = $("#" + id);

    el.nextAll(".pd2").removeClass("pd_2_active").removeClass("pd_2_active_last").removeClass("pd_2_passive_last").removeClass("pd_2_passive");

    if (lastPDPosition < "2") {
        el.nextAll(".pd2").css("display", "none");
    }
    else {
        el.nextAll(".pd2").css("display", "block");

        if (!el.nextAll(".pd2").hasClass("pd_empty")) {
            if (lastPDPosition == 2) {
                if (isActive) {
                    el.nextAll(".pd2").addClass("pd_2_active_last");
                }
                else {
                    el.nextAll(".pd2").addClass("pd_2_passive_last");
                }
            }
            else {
                if (isActive) {
                    el.nextAll(".pd2").addClass("pd_2_active");
                }
                else {
                    el.nextAll(".pd2").addClass("pd_2_passive");
                }
            }
        }
    }
}
function SetPermissionDistribution3State(id, isActive, lastPDPosition) {
    var el = $("#" + id);

    el.nextAll(".pd3").removeClass("pd_3_passive_last").removeClass("pd_3_active_last");

    if (lastPDPosition < "3") {
        el.nextAll(".pd3").css("display", "none");
    }
    else {
        el.nextAll(".pd3").css("display", "block");

        if (isActive) {
            el.nextAll(".pd3").addClass("pd_3_active_last");
        }
        else {
            el.nextAll(".pd3").addClass("pd_3_passive_last");
        }
    }
}
function UpdatePermissionDistributionChildsState(parent_id) {
    var cur_el;
    var ar = $("#" + parent_id).attr("child_direct_relations").split(",");
    var parent_value = parseInt($("#" + parent_id).val());

    for (var i = 0; i < ar.length; i++) {
        if (ar[i] != "") {
            cur_el = $("#" + ar[i]);

            if (cur_el.length > 0) {
                // меняем значение элемента в зависимости от значения родителя
                if (cur_el.attr("possible_values")[1] == "1" && cur_el.attr("possible_values")[2] == "1") {
                    if (parseInt(cur_el.val()) > parent_value) {
                        cur_el.val(parent_value);
                    }
                }
                else {
                    if (parent_value == 0) {
                        cur_el.val(0);
                    }
                    else if (cur_el.attr("possible_values")[1] == "0" && cur_el.attr("possible_values")[2] == "1") {
                        if (parent_value == 2 && cur_el.val() > "2") {
                            cur_el.val(2);
                        }
                    }
                    else if (cur_el.attr("possible_values")[1] == "1" && cur_el.attr("possible_values")[2] == "0") {
                        if (parent_value == 1 && cur_el.val() > "1") {
                            cur_el.val(1);
                        }
                    }
                }

                var possible_values = cur_el.attr("possible_values");

                SetPermissionDistribution0State(cur_el.attr("id"), cur_el.val() == "0", GetLastPDPosition(possible_values, parent_value));
                SetPermissionDistribution1State(cur_el.attr("id"), cur_el.val() == "1", GetLastPDPosition(possible_values, parent_value));
                SetPermissionDistribution2State(cur_el.attr("id"), cur_el.val() == "2", GetLastPDPosition(possible_values, parent_value));
                SetPermissionDistribution3State(cur_el.attr("id"), cur_el.val() == "3", GetLastPDPosition(possible_values, parent_value));

                UpdatePermissionDistributionChildsState(cur_el.attr("id"));
            }
        }
    }
}
$(".pd0").live("click", function () {
    // в случае клика на активном элементе - выходим
    if ($(this).prevAll("input:hidden").val() == "0") return;
    // находим контейнер
    var pd_container = $(this).prevAll("input:hidden");
    // устанавливаем новое значение
    pd_container.val(0);

    $(this).removeClass("pd_0_passive");
    if ($(this).nextAll(".pd1").css("display") == "block") { $(this).addClass("pd_0_active"); }
    else { $(this).addClass("pd_0_active_last"); }

    $(this).nextAll(".pd1").removeClass("pd_1_active").removeClass("pd_1_active_last");
    if (pd_container.attr("possible_values")[1] == "1") {
        if ($(this).nextAll(".pd2").css("display") == "block") { $(this).nextAll(".pd1").addClass("pd_1_passive"); }
        else { $(this).nextAll(".pd1").addClass("pd_1_passive_last"); }
    }

    $(this).nextAll(".pd2").removeClass("pd_2_active").removeClass("pd_2_active_last");
    if (pd_container.attr("possible_values")[2] == "1") {
        if ($(this).nextAll(".pd3").css("display") == "block") { $(this).nextAll(".pd2").addClass("pd_2_passive"); }
        else { $(this).nextAll(".pd2").addClass("pd_2_passive_last"); }
    }

    $(this).nextAll(".pd3").removeClass("pd_3_active_last").addClass("pd_3_passive_last");

    // проходим по дочерним элементам с прямой зависимостью
    UpdatePermissionDistributionChildsState(pd_container.attr("id"));
});
$(".pd1").live("click", function () {
    // в случае клика на активном элементе или недоступном значении - выходим
    if ($(this).prevAll("input:hidden").val() == "1" || $(this).hasClass("pd_empty")) return;
    // находим контейнер
    var pd_container = $(this).prevAll("input:hidden");
    // устанавливаем новое значение
    pd_container.val(1);

    $(this).prevAll(".pd0").removeClass("pd_0_active").addClass("pd_0_passive");

    $(this).removeClass("pd_1_passive").removeClass("pd_1_passive_last");
    if ($(this).nextAll(".pd2").css("display") == "block") { $(this).addClass("pd_1_active"); }
    else { $(this).addClass("pd_1_active_last"); }

    $(this).nextAll(".pd2").removeClass("pd_2_active").removeClass("pd_2_active_last");
    if ($(this).nextAll(".pd3").css("display") == "block") { $(this).nextAll(".pd2").addClass("pd_2_passive"); }
    else { $(this).nextAll(".pd2").addClass("pd_2_passive_last"); }

    $(this).nextAll(".pd3").removeClass("pd_3_active_last").addClass("pd_3_passive_last");

    // проходим по дочерним элементам с прямой зависимостью
    UpdatePermissionDistributionChildsState(pd_container.attr("id"));

});
$(".pd2").live("click", function () {
    // в случае клика на активном элементе или недоступном значении - выходим
    if ($(this).prevAll("input:hidden").val() == "2" || $(this).hasClass("pd_empty")) return;
    // находим контейнер
    var pd_container = $(this).prevAll("input:hidden");
    // устанавливаем новое значение
    pd_container.val(2);

    $(this).prevAll(".pd0").removeClass("pd_0_active").addClass("pd_0_passive");

    if (pd_container.attr("possible_values")[1] == "1") { $(this).prevAll(".pd1").removeClass("pd_1_active").addClass("pd_1_passive"); }

    $(this).removeClass("pd_2_passive").removeClass("pd_2_passive_last");
    if (pd_container.attr("possible_values")[2] == "1") {
        if ($(this).nextAll(".pd3").css("display") == "block") { $(this).addClass("pd_2_active"); }
        else { $(this).addClass("pd_2_active_last"); }
    }

    $(this).nextAll(".pd3").removeClass("pd_3_active_last").addClass("pd_3_passive_last");

    // проходим по дочерним элементам с прямой зависимостью
    UpdatePermissionDistributionChildsState(pd_container.attr("id"));
});
$(".pd3").live("click", function () {
    // в случае клика на активном элементе - выходим
    if ($(this).prevAll("input:hidden").val() == "3") return;
    // находим контейнер
    var pd_container = $(this).prevAll("input:hidden");
    // устанавливаем новое значение
    pd_container.val(3);

    $(this).prevAll(".pd0").removeClass("pd_0_active").addClass("pd_0_passive");

    if (pd_container.attr("possible_values")[1] == "1") {
        $(this).prevAll(".pd1").removeClass("pd_1_active").addClass("pd_1_passive");
    }

    if ((pd_container.attr("possible_values")[2] == "1")) {
        $(this).prevAll(".pd2").removeClass("pd_2_active").addClass("pd_2_passive");
    }

    $(this).removeClass("pd_3_passive_last").addClass("pd_3_active_last");

    // проходим по дочерним элементам с прямой зависимостью
    UpdatePermissionDistributionChildsState(pd_container.attr("id"));
});

/* YesNoToggle
****************************************************************/
$(".yes_no_toggle").live("click", function () {
    ChangeYesNoToggleValue($(this));
});

function ChangeYesNoToggleValue(toggle) {
    if (toggle.next("input").val() == "0") {
        SetYesNoToggle(toggle);
    }
    else {
        ResetYesNoToggle(toggle);
    }
}
function SetYesNoToggle(toggle) {
    toggle.next("input").val("1");
    toggle.html(toggle.next("input").next("input").val());
    toggle.trigger('change');
}
function ResetYesNoToggle(toggle) {
    toggle.next("input").val("0");
    toggle.html(toggle.next("input").next("input").next("input").val());    
    toggle.trigger('change');
}
function EnableYesNoToggle(toggle) {
    toggle.addClass('yes_no_toggle link');
}
function DisableYesNoToggle(toggle) {
    toggle.removeClass('yes_no_toggle link');
}

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
// переключение пунктов главного меню
$('ul#menu li').live('click', function () {
    $(this).addClass('selected').siblings().removeClass('selected');

    var index = $(this).index();
    
    $('div#sub_menu_content div.submenu').eq(index).show().siblings().hide();
});

// клик на кнопке справки
$('.help-btn').live('click', function () {
    var btn = $(this);

    $('.help-btn').removeClass("help-btn-active");
    btn.addClass("help-btn-active");

    $('#help_popup').hide();
    $('#help_popup .close_button').prev('div').remove();

    var left = btn.offset().left;
    var top = btn.offset().top;

    $('#help_popup').css('left', left - 10);

    $.ajax({
        type: "GET",
        url: btn.attr('help-url'),
        success: function (result) {
            $('#help_popup .close_button').before(result);

            $('#help_popup').css('top', top + 20).fadeIn('fast');
        },
        error: function (XMLHttpRequest, textStatus, thrownError) {
            $('#help_popup .close_button').before('<div class="help_content"><p>Справка недоступна.</p></div>');

            $('#help_popup').css('top', top + 20).fadeIn('fast');
        }
    });
});

function closeHelp() {
    $('.help-btn').removeClass("help-btn-active");
    $('#help_popup').fadeOut('fast', function () {
        $('#help_popup .close_button').prev('div').remove();
    });
}
// закрытие окна справки при клике за его пределами
$("*", document.body).click(function (e) {
    if (!$(e.target).hasClass('help-btn')) {
        if ($(e.target).closest('#help_popup').length == 0) {
            closeHelp();
        }
    }
});
$('#help_popup .close_button').live('click', function () {
    closeHelp();
});

// Установка атрибутов ссылки на детали сущности
function SetEntityDetailsLink(allowToViewDetailsProperty, linkId, entityName, entityIdPropertyName) {
    if (IsTrue($("#" + allowToViewDetailsProperty).val()) || allowToViewDetailsProperty == null) {
        $("#" + linkId).removeClass("disabled").attr("href", "/" + entityName + "/Details?id=" + $("#" + entityIdPropertyName).val() + GetBackUrl());
    }
    else {
        $("#" + linkId).addClass("disabled").removeAttr("href");
    }
}
