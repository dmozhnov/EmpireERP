var AccountingPriceList_Edit = {
    Init: function () {
        $(document).ready(function () {

            $('#LastDigitCalcRuleNumber, #LastDigitCalcRulePenny').bind("change disable submit", function () {
                ValidatePair('LastDigitCalcRuleNumber', 'LastDigitCalcRulePenny', function (x, y) { return ($('#rbLastDigitCalcRuleType_4:checked').val() == undefined || x != "" || y != "" || $('#rbLastDigitCalcRuleType_4:disabled').val() == "disabled"); }, 'Заполните хотя бы одно поле', "LastDigitCalcRuleError")
            });

            $('#EndDate').change(function () {
                var elem = $(this);
                if (elem.val() == "") {
                    $('#EndTime').val("");
                }
                else {
                    $('#EndTime').val("23:59:59");
                }
            });


            $(':input, select').bind('disable', function () {
                $(this).ValidationValid();
            });

            $('#form0').live('submit', function () {
                if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
                "Выберите все места хранения или не больше ", "messageAccountingPriceListEdit"))) {
                    scroll(0, $("#messageAccountingPriceListEdit").offset().top - 10);
                    return false;
                }
            });

            //  инициализация элементов формы
            if (IsTrue($("#AllowToEdit").val())) {
                if ($('#form0 input[name=AccountingPriceCalcRuleType]:checked').val() == 1) {
                    rbAccountingPriceCalcRuleType1();
                }
                else {
                    rbAccountingPriceCalcRuleType2();
                }
                var lastDigitRuleType = $('#form0 input[name=LastDigitCalcRuleType]:checked').val()
                AccountingPriceList_Edit.LastDigitCalcRuleType(lastDigitRuleType);
            }


            $('#Number').change(function () {

                // TODO: при редактировании готового надо-  где-то хранить значение номера до редактирования,
                // и если вводимый номер равен старому, считать его верным, а если отличается - вызывать IsNumberUnique все равно.
                // (сейчас просто проверка - редактируем или создаем)

                if ($('#AccountingPriceListId').val() == "00000000-0000-0000-0000-000000000000") {

                    var num = $('#Number').val();
                    $.ajax({
                        type: "GET",
                        url: "/AccountingPriceList/IsNumberUnique",
                        data: { number: num },
                        success: function (result) {
                            if (result == "False") {
                                $('#Number').addClass('input-validation-error');
                                $('#NumberIsUnique_validationMessage').removeClass('field-validation-valid').addClass('field-validation-error').text('Введите уникальный номер');
                                $('#NumberIsUnique').val(0);
                            }
                            else {
                                $('#NumberIsUnique_validationMessage').addClass('field-validation-valid').removeClass('field-validation-error').text('');
                                $('#NumberIsUnique').val(1);
                            }
                        }
                    });

                } // if
            });

            $('#btnClosePriceList').click(function () {
                window.location = $('#BackURL').val();
            });

            $('#rbAccountingPriceCalcRuleType_1').click(function () {
                rbAccountingPriceCalcRuleType1();
            });

            $('#rbAccountingPriceCalcRuleType_2').click(function () {
                rbAccountingPriceCalcRuleType2();
            });

            $('#rbMarkupPercentDeterminationRuleType_1, #rbMarkupPercentDeterminationRuleType_2').click(function () {
                $('#CustomMarkupValue').attr('disabled', 'disabled').trigger('disable');
            });

            $('#rbMarkupPercentDeterminationRuleType_3').click(function () {
                $('#CustomMarkupValue').removeAttr('disabled');
            });

            $('#rbAccountingPriceDeterminationRuleType_1').click(function () {
                $('#listAccountingPriceDeterminationRuleType1').removeAttr('disabled');
                $('#listAccountingPriceDeterminationRuleType2').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType3').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType4').attr('disabled', 'disabled').trigger('disable');
            });

            $('#rbAccountingPriceDeterminationRuleType_2').click(function () {
                $('#listAccountingPriceDeterminationRuleType1').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType2').removeAttr('disabled');
                $('#listAccountingPriceDeterminationRuleType3').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType4').attr('disabled', 'disabled').trigger('disable');
            });

            $('#rbAccountingPriceDeterminationRuleType_3').click(function () {
                $('#listAccountingPriceDeterminationRuleType1').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType2').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType3').removeAttr('disabled');
                $('#listAccountingPriceDeterminationRuleType4').attr('disabled', 'disabled').trigger('disable');
            });

            $('#rbAccountingPriceDeterminationRuleType_4').click(function () {
                $('#listAccountingPriceDeterminationRuleType1').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType2').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType3').attr('disabled', 'disabled').trigger('disable');
                $('#listAccountingPriceDeterminationRuleType4').removeAttr('disabled');
            });

            $('#rbMarkupValueRuleType_1').click(function () {
                $('#MarkupValuePercent').removeAttr('disabled');
                $('#DiscountValuePercent').attr('disabled', 'disabled').trigger('disable');
            });

            $('#rbMarkupValueRuleType_2').click(function () {
                $('#MarkupValuePercent').attr('disabled', 'disabled').trigger('disable');
                $('#DiscountValuePercent').removeAttr('disabled');
            });

            $('#rbLastDigitCalcRuleType_1, #rbLastDigitCalcRuleType_2').click(function () {
                AccountingPriceList_Edit.LastDigitCalcRuleType(1);
            });

            $('#rbLastDigitCalcRuleType_3').click(function () {
                AccountingPriceList_Edit.LastDigitCalcRuleType(3);
            });

            $('#rbLastDigitCalcRuleType_4').click(function () {
                AccountingPriceList_Edit.LastDigitCalcRuleType(4);
            });

            function rbAccountingPriceCalcRuleType1() {
                $("#divPurchaseCostDeterminationRuleType *").removeAttr("disabled");
                $("#divMarkupPercentDeterminationRuleType *").removeAttr("disabled");
                if ($("#divMarkupPercentDeterminationRuleType input:checked").attr("value") != 3) {
                    $('#CustomMarkupValue').attr("disabled", "disabled").trigger('disable');
                }

                $("#divAccountingPriceDeterminationRuleType *").attr("disabled", "disabled").trigger('disable');
                $("#divMarkupValueRuleType *").attr("disabled", "disabled").trigger('disable');
            }

            function rbAccountingPriceCalcRuleType2() {
                $("#divPurchaseCostDeterminationRuleType *").attr("disabled", "disabled").trigger('disable');
                $("#divMarkupPercentDeterminationRuleType *").attr("disabled", "disabled").trigger('disable');

                $("#divAccountingPriceDeterminationRuleType *").removeAttr("disabled");
                $("#divMarkupValueRuleType *").removeAttr("disabled");

                var value = $("#divAccountingPriceDeterminationRuleType input:checked").attr("value");

                switch (value) {
                    case "1":
                        $('#listAccountingPriceDeterminationRuleType2').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType3').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType4').attr('disabled', 'disabled').trigger('disable');
                        break;
                    case "2":
                        $('#listAccountingPriceDeterminationRuleType1').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType3').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType4').attr('disabled', 'disabled').trigger('disable');
                        break;
                    case "3":
                        $('#listAccountingPriceDeterminationRuleType1').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType2').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType4').attr('disabled', 'disabled').trigger('disable');
                        break;
                    case "4":
                        $('#listAccountingPriceDeterminationRuleType1').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType2').attr('disabled', 'disabled').trigger('disable');
                        $('#listAccountingPriceDeterminationRuleType3').attr('disabled', 'disabled').trigger('disable');
                        break;
                }

                value = $("#divMarkupValueRuleType input:checked").attr("value");
                switch (value) {
                    case "1":
                        $('#DiscountValuePercent').attr('disabled', 'disabled').trigger('disable');
                        break;
                    case "2":
                        $('#MarkupValuePercent').attr('disabled', 'disabled').trigger('disable');
                        break;
                }
            }
        });               // document ready        

        function ValidateInputWithRadioButton(input_id, radio_id, predicate, message, validationMessageId) {
            var input = $('#' + input_id);
            input.Validate(function (x) { return !($('#' + radio_id + ':checked').val() != undefined && !predicate(x) && ($('#' + radio_id + ':disabled').val() == undefined)); }, message, validationMessageId);
        }

        function BindValidateInputWithRadioButton(input_id, radio_id, predicate, message, validationMessageId) {
            $('#' + input_id).bind("change disable submit", function () { ValidateInputWithRadioButton(input_id, radio_id, predicate, message, validationMessageId); });
            $('#' + radio_id).bind('disable submit', function () { ValidateInputWithRadioButton(input_id, radio_id, predicate, message, validationMessageId); });
        }
    },

    OnFailCreatePriceList: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageAccountingPriceListEdit");
    },

    OnSuccessCreatePriceList: function (ajaxContext) {
        ShowSuccessMessage("Сохранено. Ждите загрузки страницы.", "messageAccountingPriceListEdit");
        var id = ajaxContext.Id;
        var url = "/AccountingPriceList/Details?id=" + id + GetBackUrlFromString($("#BackURL").val());
        window.location = url;
    },

    LastDigitCalcRuleType: function (value) {
        value = parseInt(value);
        switch (value) {
            case 1:
            case 2:
                $('#listLastDigitCalcRuleType, #LastDigitCalcRuleNumber, #LastDigitCalcRulePenny').attr('disabled', 'disabled').trigger('disable');
                break;
            case 3:
                $('#listLastDigitCalcRuleType').removeAttr('disabled');
                $('#LastDigitCalcRuleNumber, #LastDigitCalcRulePenny').attr('disabled', 'disabled').trigger('disable');
                break;
            case 4:
                $('#listLastDigitCalcRuleType').attr('disabled', 'disabled').trigger('disable');
                $('#LastDigitCalcRuleNumber, #LastDigitCalcRulePenny').removeAttr('disabled');
                break;
        }
    }
};