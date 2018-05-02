$("div.multiple_selector_item").live("click", function () {
    var id = $(this).parent().attr("multipleSelectorId");
    var divType = $(this).parent().attr("containerType");
    var selectedCountField = $("#" + id + "_selectedCount");

    $(this).remove();

    if (divType == 0) { //Клик по коллекции доступных
        $('#' + id + '_selected').append("<div class=\"multiple_selector_item link\" value=\"" + $(this).attr("value") + "\">" + $(this).html() + "</div>");
        $('#' + id).trigger('addElement', [new Array($(this).attr("value"))]);
        UpdateSelectedCount(id, 1);
    }
    else {//Клик по коллекции выбранных
        $('#' + id + '_available').append("<div class=\"multiple_selector_item link\" value=\"" + $(this).attr("value") + "\">" + $(this).html() + "</div>");
        $('#' + id).trigger('removeElement', [new Array($(this).attr("value"))]);
        UpdateSelectedCount(id, -1);
    }

    MultipleSelectorUpdate(id);
});

$("input.multiple_selector_add_button").live("click", function () {
    var id = $(this).attr("multipleSelectorId");
    var newHtml = $('#' + id + "_selected").html();

    var ids = new Array();
    $('#' + id + "_available").find('div.multiple_selector_item:visible').each(function (index, element) { ids.push($(element).attr("value")); });
    
    // выбираем только отфильтрованные элементы
    $('#' + id + "_selected").append($('#' + id + "_available").find('div.multiple_selector_item:visible'));
    $('#' + id + "_available").find('div.multiple_selector_item:visible').remove();

    $('#' + id).trigger('addElement', [ids]);

    SetSelectedCount(id, $("#" + id + "_selected .multiple_selector_item").length);
    MultipleSelectorUpdate(id);
});

$("input.multiple_selector_remove_button").live("click", function () {
    var id = $(this).attr("multipleSelectorId");
    var newHtml = $('#' + id + "_available").html();

    var ids = new Array();
    $('#' + id + "_selected").find('div.multiple_selector_item:visible').each(function (index, element) { ids.push($(element).attr("value")); });

    $('#' + id + "_available").html(newHtml + $('#' + id + "_selected").html());
    $('#' + id + "_selected").html("");

    $('#' + id).trigger('removeElement', [ids]);

    SetSelectedCount(id, "0");
    MultipleSelectorUpdate(id);
});

function MultipleSelectorUpdate(id) {
    $('#' + id + '_selected_values').val(MultipleSelectorGetParamsString(id));
}

function MultipleSelectorGetParamsString(id) {
    var str = "";
    var elements = $("#" + id + "_selected").children();
    for (var i = 0; i < elements.length; ++i) {
        str += $(elements[i]).attr("value");
        if (i < elements.length - 1) str += '_';
    }

    return str;
}

function SetMultipleSelectorSelectedValues(id_MultipleSelector, id_selectedValues, id_selectedAll) {
    if ($("#" + id_MultipleSelector + "_available .multiple_selector_item").length == 0) {
        $("#" + id_selectedAll).val("1");
    }
    else {
        var elements = $("#" + id_MultipleSelector + "_selected").children();
        var str = "";
        for (var i = 0; i < elements.length; ++i) {
            str += $(elements[i]).attr("value");
            if (i < elements.length - 1) str += '_';
        }
        $("#" + id_selectedValues).val(str);
    }
}

function UpdateSelectedCount(id, number) {
    var selectedCountField = $("#" + id + "_selectedCount");
    selectedCountField.text(TryGetDecimal(selectedCountField.text()) + number);

    CheckSelectedCount(id);
}

function SetSelectedCount(id, number) {
    var selectedCountField = $("#" + id + "_selectedCount");
    selectedCountField.text(number);

    CheckSelectedCount(id);
}

function CheckSelectedCount(id) {
    var selectedCountField = $("#" + id + "_selectedCount");
    var selectedCount = TryGetDecimal(selectedCountField.text());
    var maxSelectedCount = TryGetDecimal($("#" + id).attr("data-max-selected-count"));

    if (isNaN(maxSelectedCount)) return true;

    if (selectedCount > maxSelectedCount && $("#" + id + "_available .multiple_selector_item").length > 0) {
        selectedCountField.parent("font").attr("color", "red");
        return false;
    }
    else {
        selectedCountField.parent("font").attr("color", "gray");
        return true;
    }
}

$.fn.FormSelectedEntitiesUrlParametersString = function (allEntitiesParamName, listEntitiesParamName) {
    var selectorId = $(this).attr("id");
    if ($("#" + selectorId + "_available .multiple_selector_item").length == 0) {
        return allEntitiesParamName + "=1";
    }
    else {
        return listEntitiesParamName + "=" + $("#" + selectorId + "_selected_values").val();
    }
}

$.fn.FormSelectedEntityIDsUrlParametersString = function (listEntitiesParamName) {
    var selectorId = $(this).attr("id");
    return listEntitiesParamName + "=" + $("#" + selectorId + "_selected_values").val();
}

$.fn.CheckSelectedEntitiesCount = function (emptyMessage, overFlowMessage, messagePlaceHolderId) {
    var selectorId = $(this).attr("id");

    if ($("#" + selectorId + "_selected_values").val() == "") {
        ShowErrorMessage(emptyMessage, messagePlaceHolderId);

        return false;
    }

    var maxSelectedCount = TryGetDecimal($("#" + selectorId).attr("data-max-selected-count"));

    if (!CheckSelectedCount(selectorId)) {
        scroll(0, 205);
        ShowErrorMessage(overFlowMessage + maxSelectedCount + ".", messagePlaceHolderId);

        return false;
    }
    return true;
}

// независимый от регистра Contains 
jQuery.expr[':'].Contains = function (a, i, m) {
    return jQuery(a).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
};

$('.multiple_selector_wrapper input').live('change keyup', function () {    
    $(this).next('.multiple_selector').children('div').css('display', 'block');

    if ($(this).val() != "") {
        $(this).next('.multiple_selector').children('div:not(:Contains("' + $(this).val() + '"))').css('display', 'none');
    }
});