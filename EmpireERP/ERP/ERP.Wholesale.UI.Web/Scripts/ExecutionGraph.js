/// <reference path="jquery-1.4.4-vsdoc.js" />

//контрол графика исполнения заказа, isScaleOfOrder - если true маштабируем график по продолжительности всего заказа,
//иначе по продолжительности партии
var dayConsistsMsec = 3600 * 24 * 1000;

function drawExecutionGraph(idName, graphData, isScaleOfOrder) {
    
    if (isScaleOfOrder === undefined)
        isScaleOfOrder = false;

    var obj = graphData;
    var graph = $("#" + idName);
    graph.empty();
    $(".arrow_pointer").die();

    if (typeof obj == "string") {
        obj = $.parseJSON(obj);
    }

    var stages = obj.Stages;

    var factRow = $("<tr></tr>");
    var planRow = $("<tr></tr>");

    var factTotalDuration = 0; //фактическая продолжительность всего заказа
    var planTotalDuration = 0; //плановая продолжительность всего заказа

    //100% длины будет у отрезка бОльшей продолжительности из двух, второй будет иметь меньший процент
    var factTotalRatio; //процент длины для фактической продолжительности
    var planTotalRatio; //процент длины для плановой продолжительности

    var currentFactDate = getDateRoundToDay(obj.StartDate); //сюда будем прибавлять длины этапов, чтобы знать дату начала следующего этапа
    var currentPlanDate = getDateRoundToDay(obj.StartDate);

    //Подсчет длительности этапов
    $.each(stages, function (index, value) {
        value.startFactDate = dateToString(currentFactDate, 2);
        value.startPlanDate = dateToString(currentPlanDate, 2);

        if (value.state < 3) {
            factTotalDuration += Math.max(value.factDuration, 1);
        }
        if (value.state == 3) {
            factTotalDuration += Math.max(value.planDuration, value.factDuration, 1);
        }
        if (value.state == 4) {
            factTotalDuration += value.factDuration;
        }
        if (value.state == 5) {
            factTotalDuration += Math.max(value.factDuration, 1);
        }

        planTotalDuration += Math.max(value.planDuration, 1);

        if (value.state == 3) {
            currentFactDate.setDate(currentFactDate.getDate() + value.planDuration);
        }
        else {
            currentFactDate.setDate(currentFactDate.getDate() + value.factDuration);
        }
        currentPlanDate.setDate(currentPlanDate.getDate() + value.planDuration);
    });

    var startDate = getDateRoundToDay(obj.StartDate);//дата начала партии
    var endDate = getDateRoundToDay(obj.EndDate);//дата конца партии

    var maxLength;
    var marginLeftRatio;

    if (isScaleOfOrder === true) {
        //Получаем длительность заказа в днях
        var productionOrderStartDate = getDateRoundToDay(obj.ProductionOrderStartDate);
        var productionOrderEndDate = getDateRoundToDay(obj.ProductionOrderEndDate);
        maxLength = (productionOrderEndDate - productionOrderStartDate) / dayConsistsMsec;
        //на случай этапов с 0 длительностью
        maxLength = Math.max(maxLength, factTotalDuration);
        
        //Получаем смещение графика влево, если партия создана позже заказа.
        marginLeftRatio = (startDate - productionOrderStartDate )/ dayConsistsMsec / maxLength * 100
    }
    else {
        maxLength = Math.max(factTotalDuration, planTotalDuration); //за 100% принимается длина бОльшего отрезка
    }

    //ширина графиков
    planTotalRatio = planTotalDuration / maxLength * 100;
    factTotalRatio = factTotalDuration / maxLength * 100;

    //Из-за дробной части ширины графика и его смещения их сумма может немного превыщать 100%,
    //поэтому проверяем и исправляем если необходимо
    if (isScaleOfOrder === true) {
        if ((marginLeftRatio + planTotalRatio) > 100)
            marginLeftRatio = 100 - planTotalRatio;
        if ((marginLeftRatio + factTotalRatio) > 100)
            marginLeftRatio = 100 - factTotalRatio;
    }

    var factTable = $("<table width='100%'></table>");
    var planTable = $("<table width='100%'></table>");

    var planDayWidth = 100 / planTotalDuration; //ширина одного планируемого дня в процентах
    var factDayWidth = 100 / factTotalDuration; //ширина одного фактического дня в процентах

    $.each(stages, function (index, value) {
        if (value.state != 3) {
            value.factWidth = Math.max(value.factDuration, 1) * factDayWidth; //если длина этапа 0, то он все равно должен выводиться с длиной 1, для этого и Math.max()
        }
        else {
            value.factWidth = Math.max(value.planDuration, value.factDuration, 1) * factDayWidth;
        }

        value.planWidth = Math.max(value.planDuration, 1) * planDayWidth;
    });

    $.each(stages, function (index, value) {
        var factStage;

        switch (value.state) {
            case 1:
            case 2:
                factStage = getPastStage(value, factDayWidth); 
                break;
            case 3:
                factStage = getSuccessCurrentStage(value, factDayWidth); 
                break;
            case 4:
                factStage = getFailCurrentStage(value, factDayWidth); 
                break;
            case 5:
                factStage = getFutureStage(value, factDayWidth); 
                break;
        }

        var planStage = getPlannedStage(value, planDayWidth);

        factRow.append(factStage);
        planRow.append(planStage);
    });


    factTable.append(factRow);
    planTable.append(planRow);

    var factChart = $("<div class='chart'></div").append(factTable);
    var factDates = getFactDatePoints(stages, factTotalDuration, productionOrderStartDate, startDate, marginLeftRatio, factTotalRatio);
    var factBar = $("<div style='width:" + factTotalRatio + "%'></div>").append(factDates).append(factChart);
    
    var planChart = $("<div class='chart'></div").append(planTable);
    var planDates = getPlanDatePoints(stages, productionOrderStartDate, startDate, marginLeftRatio, planTotalRatio);
    var planBar = $("<div style='width:" + planTotalRatio + "%'></div>").append(planChart).append(planDates);

    //Если маштабируем по заказу, то смещаем графики партий которые начались позже заказа
    if (isScaleOfOrder === true) {
        graph.append('<div style="padding-left: 1%; margin-left:' + marginLeftRatio +
                                    '%; margin-bottom: 15px; font-size: 12px">' + obj.Name + '</div>');
        factBar.css("margin-left", marginLeftRatio + "%");
        planBar.css("margin-left", marginLeftRatio + "%");
    }

    graph.append(factBar);
    graph.append(planBar);

    $(".arrow_pointer").live('mouseover', function () {
        $(".date").each(function (index, value) {
            $(value).css("color", "#999999");
        });
        var date = $(this).parent().children(".date");
        var fontsize = $(date).css("font-size");
        var sizevalue = fontsize.slice(0, fontsize.length - 2);
        var newsizevalue = parseInt(sizevalue);

        date.css("color", "#FF0000").css("font-size", newsizevalue + "px").css("font-weight", "bold");

        if ($(this).hasClass("down")) {
            date.animate({ top: "-50%" }, 100);
        }
        else
            if ($(this).hasClass("up")) {
                date.animate({ bottom: "-50%" }, 100);
            }
    });

    $(".arrow_pointer").live('mouseout', function () {
        var date = $(this).parent().children(".date");
        var fontsize = date.css("font-size");
        var sizevalue = fontsize.slice(0, fontsize.length - 2);
        var newsizevalue = parseInt(sizevalue);

        if ($(this).hasClass("down")) {
            date.css("top", "-50%");
            date.animate({ top: "0%" }, 100);
        }
        else
            if ($(this).hasClass("up")) {
                date.css("bottom", "-50%");
                date.animate({ bottom: "0%" }, 100);
            }
        date.css("font-size", newsizevalue + "px").css("font-weight", "normal");

        $(".date").css("color", "#000000");
    });
}

//получить дату округленную до дня
function getDateRoundToDay(jsonDate) {
    var date = new Date(parseInt(jsonDate.slice(6, 19)));
    var dateRoundToDay = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    return dateRoundToDay;
}

function getClassName(state) {
    switch (state) {
        case 1: return "successStage";
        case 2: return "failStage";
        case 3: return "successCurrentStage";
        case 4: return "failCurrentStage";
        case 5: return "futureStage"

        default: return "";
    }
}

//вывод текущего  этапа, если он еще не просрочен (фактическая часть)
function getSuccessCurrentStage(stageData, dayWidth) {
    var completedPart = $("<td class='completed_part'></td>"); //дней с начала этапа уже прошло (то, что выводится светло-зеленым или светло-красным)
    var uncompletedPart = $("<td></td>"); //дней до конца еще осталось по плану (выводится белым)

    completedPart.css("width", stageData.factDuration * dayWidth + "%");
    uncompletedPart.css("width", (stageData.planDuration - stageData.factDuration) * dayWidth + "%");

    completedPart.attr("title", stageData.name)
    uncompletedPart.attr("title", stageData.name)

    var className = getClassName(stageData.state);

    completedPart.addClass(className);

    if (stageData.planDuration == stageData.factDuration) {
        return completedPart;
    }
    else
        if (stageData.factDuration == 0) {
            return uncompletedPart;
        }
        else {
            return completedPart.add(uncompletedPart);
        }
}

//вывод текущего  этапа, если он не просрочен (фактическая часть)
function getFailCurrentStage(stageData, dayWidth) {
    var completedPart = $("<td>" + stageData.factDuration + "</td>"); 
    
    completedPart.css("width", stageData.factDuration * dayWidth + "%");
    completedPart.attr("title", stageData.name)
    
    var className = getClassName(stageData.state);
    completedPart.addClass(className);
    
    return completedPart;

}

//вывод этапа, который еще не наступил (фактическая часть)
function getFutureStage(stageData, dayWidth) {
    var stage = $("<td></td>");

    stage.css("width", (Math.max(stageData.factDuration, 1)) * dayWidth + "%");
    stage.attr("title", stageData.name);
    stage.addClass("futureStage");

    return stage;
}

//вывод этапа, который уже пройден (фактическая часть)
function getPastStage(stageData, dayWidth) {
    var factStage = $("<td>" + stageData.factDuration + "</td>");

    var factLength;

    if (stageData.state == 5) {
        factLength = stageData.planDuration;
    }
    else {
        factLength = stageData.factDuration;
    }

    factStage.css("width", stageData.factWidth + "%");

    var className = getClassName(stageData.state);

    factStage.addClass(className);

    factStage.attr("title", stageData.name);

    return factStage;
}

//вывод плановой части этапа
function getPlannedStage(stageData, dayWidth) {
    var planStage = $("<td>" + stageData.planDuration + "</td>");

    planStage.css("width", stageData.planWidth + "%");

    planStage.addClass("plannedStage");

    planStage.attr("title", stageData.name);

    return planStage;
}

//вывод указателей и дат для фактической части
function getFactDatePoints(stages, totalDuration, startOrder, startBatch, marginLeftRatio, totalRatio) {
    var stripDiv = $('<div class="dates" style="position:relative"></div>');

    var currentWidth = 0;

    $.each(stages, function (index, value) {

        //Если совпадает с началом заказа или 
        //мы показываем один график, то
        //не выводим дату начала
        if (index === 0 && ((Math.ceil((startOrder - startBatch) / dayConsistsMsec) === 0) || (startOrder === undefined))) {
            currentWidth += value.factWidth;
            return;
        }

        var arrowCode = getArrowCode(value.state);
        var datePoint = getDatePoint(index, currentWidth, value.startFactDate, arrowCode);

        stripDiv.append(datePoint);

        currentWidth += value.factWidth;
    });

    //Добавляем метку со временем в конце графика, если он не самый длинный
    if ( (marginLeftRatio !== undefined) && (marginLeftRatio + totalRatio !== 100)) {
        var lastIndex = stages.length - 1;

        var arrowCode = getArrowCode(stages[lastIndex].state);

        //данный код корректно работает только до 2099 г.
        //получаем дату начала последнего этапа
        var end = new Date(stages[lastIndex].startFactDate.replace(/(\d+).(\d+).(\d+)/, '20$3/$2/$1'));
        //добавляем его продолжительность
        end.setDate(end.getDate() + stages[lastIndex].factDuration);
        var endDate = dateToString(end, 2);
        
        var datePoint = getDatePoint(lastIndex, currentWidth, endDate, arrowCode);
        stripDiv.append(datePoint);
    }

    return stripDiv;
}

function getArrowCode(state) {
    var arrowCode;
    switch (state) {
        case 1:
        case 2:
        case 3:
        case 4:
            arrowCode = "&#9660"; 
            break;
        case 5:
            arrowCode = "&#9661"; 
            break;
    }
    return arrowCode;
}
//Получить элемент метки времени
function getDatePoint(index, currentWidth, date, arrowCode) {
    var itemContainer = $('<div class="dates_container" style="z-index:' + index + ';left:' + currentWidth + '%"</div>');
    var itemDiv = $('<div class="dates_item"></div>');
    var dateDiv = $('<div class="date">' + date + '</div>');
    
    var cssClass;
    if (arrowCode === "&#9650") {
        cssClass = "up";
        dateDiv.css("bottom", "0%");
    }
    else {
        cssClass = "down";
    }

    var arrowDiv = $('<div class="arrow_pointer '+ cssClass + '">' + arrowCode + '</div>');

    itemDiv.append(dateDiv);
    itemDiv.append(arrowDiv);
    itemContainer.append(itemDiv);
    return itemContainer;
}

//вывод указателей и дат для плановой части
function getPlanDatePoints(stages, startOrder, startBatch, marginLeftRatio, totalRatio) {
    var stripDiv = $('<div class="dates" style="position:relative"></div>');

    var currentWidth = 0;
    var arrowCode = "&#9650";

    $.each(stages, function (index, value) {

        //Если совпадает с началом заказа или 
        //мы показываем один график, то
        //не выводим дату начала
        if (index === 0 && ( (Math.ceil((startOrder - startBatch) / dayConsistsMsec) === 0) || (startOrder === undefined) )) {
            currentWidth += value.planWidth;
            return;
        }

        var datePoint = getDatePoint(index, currentWidth, value.startPlanDate, arrowCode)
        stripDiv.append(datePoint);
        
        currentWidth += value.planWidth;
    });

    //Добавляем метку со временем в конце графика, если он не самый длинный
    if ( (marginLeftRatio !== undefined) && (marginLeftRatio + totalRatio !== 100)) {
        var lastIndex = stages.length - 1;

        //данный код корректно работает только до 2099 г.
        //получаем дату начала последнего этапа
        var end = new Date(stages[lastIndex].startPlanDate.replace(/(\d+).(\d+).(\d+)/, '20$3/$2/$1'));
        //добавляем его продолжительность
        end.setDate(end.getDate() + stages[lastIndex].planDuration);
        var endDate = dateToString(end, 2);

        var datePoint = getDatePoint(lastIndex, currentWidth, endDate, arrowCode);
        stripDiv.append(datePoint);
    }

    return stripDiv;
}  