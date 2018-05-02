var User_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#LastName").focus();

            if ($("#Id").val() != "0") {
                ShowDisplayNameList();
            }

            $("#btnBack").live("click", function () {
                window.location = $('#BackURL').val();
            });

            $("#LastName, #FirstName, #Patronymic").bind("change", function () {
                ShowDisplayNameList();
            });

            $("#DisplayName").change(function () {
                $("#DisplayNameTemplate").val($(this).val());
            });

            $("#linkAddEmployeePost").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/EmployeePost/Create",
                    success: function (result) {
                        $("#employeePostEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#employeePostEdit"));
                        ShowModal("employeePostEdit");
                        $("#employeePostEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageUserEdit");
                    }
                });
            });

            $('#Login').bind("change", function () {
                var login = $('#Login').val();
                var id = $("#Id").val();

                $.ajax({
                    url: "/User/IsLoginUnique",
                    data: { login: login, id: id },
                    success: function (result) {
                        if (result == "False") {
                            $('#Login').removeClass('valid').addClass('input-validation-error');
                            $('[data-valmsg-for="Login"]').removeClass('field-validation-valid').addClass('field-validation-error').html("<span htmlfor='LoginIsUnique' generated='true'>Пользователь с данным логином уже существует</span>");
                            $('#LoginIsUnique').val(0);
                        }
                        else {
                            $('[data-valmsg-for="Login"]').removeClass('field-validation-error').addClass('field-validation-valid').html("");
                            $('#Login').removeClass('input-validation-error').addClass('valid');
                            $('#LoginIsUnique').val(1);
                        }
                    }
                });
            });
        });

        function GetDisplayNameByTemplate(template, lastName, firstName, patronymic) {
            var result = "";
            var splitSymbol = " ";

            for (var i = 0; i < template.length; i++) {
                if (result.length == 0) {
                    splitSymbol = "";
                }
                else {
                    splitSymbol = " ";
                }

                switch (template[i]) {
                    case 'L':
                        result += splitSymbol + lastName;
                        break;
                    case 'l':
                        result += splitSymbol + GetFirstSymbol(lastName);
                        break;
                    case 'F':
                        result += splitSymbol + firstName;
                        break;
                    case 'f':
                        result += splitSymbol + GetFirstSymbol(firstName);
                        break;
                    case 'P':
                        result += splitSymbol + patronymic;
                        break;
                    case 'p':
                        result += splitSymbol + GetFirstSymbol(patronymic);
                        break;
                }
            }

            return result;
        }

        function GetFirstSymbol(value) {
            var result = "";
            if (value.length > 0) {
                result = value[0] + '.';
            }

            return result;
        }

        function ShowDisplayNameList() {
            var lastName = $("#LastName").val();
            var firstName = $("#FirstName").val();
            var patronymic = $("#Patronymic").val();

            var currentTemplate = $("#DisplayNameTemplate").val();
            var dropdownList = $("#DisplayName");

            var templates = new Array()
            templates[0] = "LF";
            templates[1] = "Lfp";
            templates[2] = "LFP";
            templates[3] = "FL";
            templates[4] = "Lf";
            templates[5] = "FP";

            dropdownList.clearSelect();

            for (var i = 0; i < templates.length; i++) {
                var isSelected = templates[i] == currentTemplate;
                var text = GetDisplayNameByTemplate(templates[i], lastName, firstName, patronymic);

                var option = "<option value=" + templates[i];
                if (templates[i] == currentTemplate) {
                    option += " selected=\"selected\"";
                }
                option += ">" + text + "</option>";

                dropdownList.append(option);
            }
        }
    },

    OnSuccessUserSave: function (ajaxContext) {
        window.location = "/User/Details?id=" + ajaxContext + "&backURL=/User/List";
    },

    OnBeginUserSave: function () {
        StartButtonProgress($("#btnUserSave"));
    },

    OnFailUserSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageUserEdit");
    },

    OnSuccessEmployeePostSave: function (ajaxContext) {
        $.ajax({
            type: "POST",
            url: "/EmployeePost/GetEmployeePosts",
            success: function (result) {
                $("#EmployeePostId").fillSelect(result);
                $("#EmployeePostId").attr("value", ajaxContext.Id);
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageUserEdit");
            }
        });

        HideModal();
        ShowSuccessMessage("Должность добавлена.", "messageUserEdit");
    }
};

