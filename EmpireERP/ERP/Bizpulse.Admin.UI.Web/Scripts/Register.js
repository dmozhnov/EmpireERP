var Register = {
    Init: function () {
        $(document).ready(function () {
            $('#ExtraUserCount').val('0');
            $('#ExtraStorageCount').val('0');
            $('#ExtraAccountOrganizationCount').val('0');
            $('#ExtraTeamCount').val('0');
            $('#ExtraGigabyteCount').val('0');

            var clientIsJuridicalPerson;

            function FillChildComboBox(parentId, childId, methodPath, parameterName) {
                var parentComboBox = $('#' + parentId); ;
                var childComboBox = $('#' + childId);

                parentComboBox.bind("keyup change", function () {
                    var selectedId = parentComboBox.val();
                    if (selectedId == "") {
                        childComboBox.clearSelect();
                    }
                    else {
                        var MskOrSPb = (parentComboBox.val() == "77" || parentComboBox.val() == "78");

                        if (!MskOrSPb) {
                            childComboBox.closest("div").show();
                            StartComboBoxProgress(childComboBox);
                        }
                        else {
                            childComboBox.closest("div").hide();
                        }


                        $.ajax({
                            type: "GET",
                            url: methodPath,
                            data: parameterName + '=' + selectedId,
                            success: function (result) {
                                if (result != 0) {
                                    childComboBox.fillSelect(result);

                                    if (!MskOrSPb) {
                                        StopComboBoxProgress(childComboBox);
                                    }
                                    else {
                                        childComboBox.val(childComboBox.children("option:last-child").val());
                                    }
                                }
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                if (!MskOrSPb) {
                                    StopComboBoxProgress(childComboBox);
                                }
                            }
                        });
                    }
                });
            }

            // клик по кнопке "Юр. лицо"
            $('#btn_juridical_person').on('click', function () {
                clientIsJuridicalPerson = true;

                $('#juridicalPersonInfo').show();
                // очищаем форму
                $('#physicalPersonInfo').html("");

                InitJuridicalPersonEvents();

                FillChildComboBox("JuridicalAddressRegionId", "JuridicalAddressCityId", "/City/GetList/", "regionId");
                FillChildComboBox("PostalAddressRegionId", "PostalAddressCityId", "/City/GetList/", "regionId");

                $('#client_organization_type_selection, #top_links, h3, h2').fadeOut(300, function () {
                    $('#registration_steps_wrapper').fadeIn(300, function () {
                        $("#AdminLastName").focus();
                    });
                });
            });

            // клик по кнопке "Физ. лицо"
            $('#btn_physical_person').on('click', function () {
                clientIsJuridicalPerson = false;

                $('#physicalPersonInfo').show();
                // очищаем форму
                $('#juridicalPersonInfo').html("");

                InitPhysicalPersonEvents();

                FillChildComboBox("RegistrationAddressRegionId", "RegistrationAddressCityId", "/City/GetList/", "regionId");
                FillChildComboBox("PostalAddressRegionId", "PostalAddressCityId", "/City/GetList/", "regionId");

                $('#client_organization_type_selection, #top_links, h3, h2').fadeOut(300, function () {
                    $('#registration_steps_wrapper').fadeIn(300, function () {
                        $("#AdminLastName").focus();
                    });
                });
            });

            // смена текущего этапа регистрации
            function setStep(stepId, callback) {
                var margin = -(stepId - 1) * 842;

                $('#registration_steps_form_container').animate({
                    'margin-left': margin
                },
                function () {
                    $('#registration_steps li').removeClass('selected').eq(stepId - 1).addClass('selected');
                    if (callback != undefined) {
                        callback();
                    }
                });
            }

            function scrollTop() {
                $('html, body').animate({
                    scrollTop: 0
                }, 1000);
            }

            $('#adminInfo_next').on('click', function () {
                var formIsValid = $("#adminInfo_form").validate().form();
                if (!formIsValid) return false;

                setStep(2, function () {
                    if (clientIsJuridicalPerson) {
                        $("#ShortName").focus();
                        scrollTop();
                    }
                    else {
                        if ($("#LastName").val() == "") {
                            $("#LastName").val($("#AdminLastName").val());
                        }

                        if ($("#FirstName").val() == "") {
                            $("#FirstName").val($("#AdminFirstName").val());
                        }

                        if ($("#Patronymic").val() == "") {
                            $("#Patronymic").val($("#AdminPatronymic").val());
                        }

                        $("#LastName").focus();
                        scrollTop();
                    }
                });
            });

            $('#juridicalPersonInfo_prev, #physicalPersonInfo_prev').on('click', function () {
                setStep(1, function () {
                    $("#AdminLastName").focus();
                    scrollTop();
                });
            });
            $('#juridicalPersonInfo_next').on('click', function () {
                var formIsValid = $("#juridicalPersonInfo_form").validate().form();
                if (!formIsValid) return false;

                setStep(3, function () {
                    $("#ExtraUserCount").focus();
                    scrollTop();
                });
            });
            $('#physicalPersonInfo_next').on('click', function () {
                var formIsValid = $("#physicalPersonInfo_form").validate().form();
                if (!formIsValid) return false;

                setStep(3, function () {
                    $("#ExtraUserCount").focus();
                    scrollTop();
                });
            });

            function InitJuridicalPersonEvents() {
                InitEvents('Juridical');
            }

            function InitPhysicalPersonEvents() {
                InitEvents('Registration');
            }

            function InitEvents(prefix) {
                $('#' + prefix + 'AddressRegionId').on('change', function () {
                    if ($('#PostalAddressEquals' + prefix).attr("checked") == "checked") {
                        $('#PostalAddressRegionId').val($(this).val());
                        $('#PostalAddressRegionId').trigger('change');
                    }
                });
                $('#' + prefix + 'AddressCityId').on('change', function () {
                    if ($('#PostalAddressEquals' + prefix).attr("checked") == "checked") {
                        $('#PostalAddressCityId').val($(this).val());
                    }
                });
                $('#' + prefix + 'AddressPostalIndex').on('change keyup', function () {
                    if ($('#PostalAddressEquals' + prefix).attr("checked") == "checked") {
                        $('#PostalAddressPostalIndex').val($(this).val());
                    }
                });
                $('#' + prefix + 'AddressLocalAddress').on('change keyup', function () {
                    if ($('#PostalAddressEquals' + prefix).attr("checked") == "checked") {
                        $('#PostalAddressLocalAddress').val($(this).val());
                    }
                });

                $('#PostalAddressEquals' + prefix).on('click', function () {
                    if ($(this).attr("checked") == "checked") {
                        $('#postalAddressContainer').slideUp(300).hide();
                    }
                    else {
                        $('#postalAddressContainer').slideDown(300).show();
                    }
                });
            }

            $('#configurationInfo_prev').on('click', function () {
                setStep(2, function () {
                    if (clientIsJuridicalPerson) {
                        $("#ShortName").focus();
                        scrollTop();
                    }
                    else {
                        $("#LastName").focus();
                        scrollTop();
                    }
                });
            });

            $('#RateId').on('change', function () {
                var rate = $.parseJSON($("#RateId option:selected").attr("param"));

                $('#BaseCostPerMonth span').text(ValueForDisplay(rate.BaseCostPerMonth));
                $('#ActiveUserCountLimit span').text(ValueForDisplay(rate.ActiveUserCountLimit));

                if (rate.StorageCountLimit == 32767) {
                    $('#StorageCountLimit span').text('∞');
                    $('#ExtraStorageCount').attr('disabled', 'disabled').val('0');
                }
                else {
                    $('#StorageCountLimit span').text(ValueForDisplay(rate.StorageCountLimit));
                    $('#ExtraStorageCount').removeAttr('disabled');
                }

                if (rate.AccountOrganizationCountLimit == 32767) {
                    $('#AccountOrganizationCountLimit span').text('∞');
                    $('#ExtraAccountOrganizationCount').attr('disabled', 'disabled').val('0');
                }
                else {
                    $('#AccountOrganizationCountLimit span').text(ValueForDisplay(rate.AccountOrganizationCountLimit));
                    $('#ExtraAccountOrganizationCount').removeAttr('disabled');
                }

                if (rate.TeamCountLimit == 32767) {
                    $('#TeamCountLimit span').text('∞');
                    $('#ExtraTeamCount').attr('disabled', 'disabled').val('0');
                }
                else {
                    $('#TeamCountLimit span').text(ValueForDisplay(rate.TeamCountLimit));
                    $('#ExtraTeamCount').removeAttr('disabled');
                }

                $('#GigabyteCountLimit span').text(ValueForDisplay(rate.GigabyteCountLimit));

                $('#ExtraUserCount').trigger('change');
                $('#ExtraStorageCount').trigger('change');
                $('#ExtraAccountOrganizationCount').trigger('change');
                $('#ExtraTeamCount').trigger('change');
                $('#ExtraGigabyteCount').trigger('change');
            });

            $('#clientAgree').on('click', function () {
                if ($(this).attr("checked") == "checked") {
                    UpdateButtonAvailability("btnRegister", true);
                }
                else {
                    UpdateButtonAvailability("btnRegister", false);
                }
            });

            $('#btnRegister').on('click', function () {
                if ($('#ExtraUserCount').val() == '') $('#ExtraUserCount').val('0');                
                if ($('#ExtraStorageCount').val() == '') $('#ExtraStorageCount').val('0');
                if ($('#ExtraAccountOrganizationCount').val() == '') $('#ExtraAccountOrganizationCount').val('0');
                if ($('#ExtraTeamCount').val() == '') $('#ExtraTeamCount').val('0');
                if ($('#ExtraGigabyteCount').val() == '') $('#ExtraGigabyteCount').val('0');

                var formIsValid = $("#configurationInfo_form").validate().form();
                if (!formIsValid) return false;

                var configurationInfo_form = $("#configurationInfo_form").serializeArray();

                if (clientIsJuridicalPerson) {
                    $.merge(configurationInfo_form, $('#juridicalPersonInfo_form').serializeArray());
                }
                else {
                    $.merge(configurationInfo_form, $('#physicalPersonInfo_form').serializeArray());
                }

                $.merge(configurationInfo_form, $('#adminInfo_form').serializeArray());

                StartButtonProgress($('#btnRegister'));
                $('#submitErrorMessage').text('');

                $.ajax({
                    type: "POST",
                    url: "/Client/" + (clientIsJuridicalPerson ? "RegisterJuridicalPerson" : "RegisterPhysicalPerson"),
                    data: configurationInfo_form,
                    success: function (accountNumber) {
                        // сразу пытаемся залогиниться                        
                        $("#loginForm #AccountNumber").val(accountNumber);
                        $("#loginForm #Login").val($("#adminInfo_form #AdminLogin").val());
                        $("#loginForm #Password").val($("#adminInfo_form #AdminPassword").val());
                        $("#loginForm").trigger("submit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        StopButtonProgress();
                        $('#submitErrorMessage').text(XMLHttpRequest.responseText);
                    }
                });
            });

            $('#ExtraUserCount').on('change keyup', function () {
                var userCount = TryGetDecimal($(this).val());
                var rate = $.parseJSON($("#RateId option:selected").attr("param"));

                if (isNaN(userCount)) {
                    userCount = 0;
                }

                $('#userCountTip').text(getPriceDetailsString(rate.ActiveUserCountLimit, userCount, rate.ExtraActiveUserOptionCostPerMonth));

                UpdateTotalCost();
            });
            
            $('#ExtraStorageCount').on('change keyup', function () {
                var storageCount = TryGetDecimal($(this).val());
                var rate = $.parseJSON($("#RateId option:selected").attr("param"));

                if (isNaN(storageCount)) {
                    storageCount = 0;
                }

                $('#storageCountTip').text(getPriceDetailsString(rate.StorageCountLimit, storageCount, rate.ExtraStorageOptionCostPerMonth));

                UpdateTotalCost();
            });

            $('#ExtraAccountOrganizationCount').on('change keyup', function () {
                var accountOrganizationCount = TryGetDecimal($(this).val());
                var rate = $.parseJSON($("#RateId option:selected").attr("param"));

                if (isNaN(accountOrganizationCount)) {
                    accountOrganizationCount = 0;
                }

                $('#accountOrganizationCountTip').text(getPriceDetailsString(rate.AccountOrganizationCountLimit, accountOrganizationCount, rate.ExtraAccountOrganizationOptionCostPerMonth));

                UpdateTotalCost();
            });

            $('#ExtraTeamCount').on('change keyup', function () {
                var teamCount = TryGetDecimal($(this).val());
                var rate = $.parseJSON($("#RateId option:selected").attr("param"));

                if (isNaN(teamCount)) {
                    teamCount = 0;
                }

                $('#teamCountTip').text(getPriceDetailsString(rate.TeamCountLimit, teamCount, rate.ExtraTeamOptionCostPerMonth));

                UpdateTotalCost();
            });

            $('#ExtraGigabyteCount').on('change keyup', function () {
                var gigabyteCount = TryGetDecimal($(this).val());
                var rate = $.parseJSON($("#RateId option:selected").attr("param"));

                if (isNaN(gigabyteCount)) {
                    gigabyteCount = 0;
                }

                $('#gigabyteCountTip').text(getPriceDetailsString(rate.GigabyteCountLimit, gigabyteCount, rate.ExtraGigabyteOptionCostPerMonth));

                UpdateTotalCost();
            });

            // получение строки расшифровки стоимости опции 
            // baseCount - кол-во по тарифному плану 
            // optionCount - кол-во, указанное пользователем 
            // optionCost - стоимость единицы опции в месяц
            function getPriceDetailsString(baseCount, optionCount, optionCost) {
                return '= ' + (baseCount == 32767 ? '∞' : ValueForDisplay(baseCount)) + ' × 0 руб. ' +
                (baseCount == 32767 ? '' : '+ ' + ValueForDisplay(optionCount) + ' × ' + ValueForDisplay(optionCost) + ' руб.') +
                ' = ' + ValueForDisplay(optionCount * optionCost) + ' руб./месяц';
            }

            function UpdateTotalCost() {
                var userCount = TryGetDecimal($('#ExtraUserCount').val());                
                var storageCount = TryGetDecimal($('#ExtraStorageCount').val());
                var accountOrganizationCount = TryGetDecimal($('#ExtraAccountOrganizationCount').val());
                var teamCount = TryGetDecimal($('#ExtraTeamCount').val());
                var gigabyteCount = TryGetDecimal($('#ExtraGigabyteCount').val());

                var rate = $.parseJSON($("#RateId option:selected").attr("param"));

                $('#total_cost').text('Итоговая стоимость: ' +
                ValueForDisplay(rate.BaseCostPerMonth +
                (isNaN(userCount) ? 0 : userCount * rate.ExtraActiveUserOptionCostPerMonth) +                
                (isNaN(storageCount) ? 0 : storageCount * rate.ExtraStorageOptionCostPerMonth) +
                (isNaN(accountOrganizationCount) ? 0 : accountOrganizationCount * rate.ExtraAccountOrganizationOptionCostPerMonth) +
                (isNaN(teamCount) ? 0 : teamCount * rate.ExtraTeamOptionCostPerMonth) +
                (isNaN(gigabyteCount) ? 0 : gigabyteCount * rate.ExtraGigabyteOptionCostPerMonth)
                ) + ' руб./месяц');
            }

            $('#RateId').trigger('change');
        });
    }
};