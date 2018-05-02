<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.User.LoginViewModel>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Добро пожаловать</title>
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    
    <link href="../../Content/Style/Site.css" rel="stylesheet" type="text/css" />
    <link href="../../Content/Style/Login.css" rel="stylesheet" type="text/css" />

    <script src="../../Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.unobtrusive-ajax.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.validate.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.validate.unobtrusive.min.js" type="text/javascript"></script>
    
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            if ($("#AccountNumber").length != 0) {
                $("#AccountNumber").focus();
            }
            else {
                $("#Login").focus();
            }

            $("#Password").val("<%: Model.Password %>");
        });
        
        function OnBeginUserLogin() {
            $("#messageUserLogin").hide();
            StartButtonProgress($("#btnLogin"));
        }
        
        function OnSuccessUserLogin() {
            window.location = "/User/Home";
        }

        function OnFailUserLogin(ajaxContext) {
            $("#messageUserLogin").show().text(ajaxContext.responseText);
            StopButtonProgress();
        }
    </script>
</head>
<body>
    <div id="login_content">
        <div id="login_header">
            <img id="logo" src="/Content/Img/Logo.png" alt="Bizpulse" />        
            
            <div id="login_header_content_wrapper">
                <div id="support_title">Техническая поддержка:</div>        
            
                <div class="clear"></div>

                <ul>
                    <li>
                        <img src="/Content/Img/phone.png" alt="" />
                        +7 8442 600-337</li>
                    <li>
                        <img src="/Content/Img/e-mail.png" alt="" />
                        <a href="mailto:support@bizpulse.ru">support@bizpulse.ru</a></li>
                    <li>
                        <img src="/Content/Img/skype.png" alt="" />
                        <a href="skype:Bizpulse?call">Bizpulse</a></li>
                </ul>               
            </div>

            <div class="clear"></div>
        </div>

        <div id="blue_line_wrapper">
            <div id="blue_line"></div>
        </div>

        <div id="main_content_wrapper">
            <div id="login_content_info">
                <h1>Управляйте Вашей компанией, где бы Вы не находились!</h1>
                <h2>Электронный менеджер Bizpulse предоставляется в виде интернет-сервиса - для его использования достаточно только зарегистрироваться и работать.</h2>
                <h1>Вы сможете с помощью Bizpulse:</h1>

                <ul>
                    <li>Управлять производством, товародвижением, продажами, сократив затраты на рутинные и ресурсоемкие работы.</li>
                    <li>Быстро получать любую интересующую Вас информацию о текущих делах Вашего бизнеса.</li>
                    <li>Гибко настраивать права доступа к информации для каждого сотрудника Вашей компании.</li>
                </ul>
            </div>

            <div id="login_content_login_bar_container">                
                <h1>Вход в аккаунт <b>Bizpulse</b></h1>

                <% using (Ajax.BeginForm("TryLogin", "User", new AjaxOptions() { OnBegin = "OnBeginUserLogin", OnSuccess = "OnSuccessUserLogin", OnFailure = "OnFailUserLogin" })) %>
                <%{ %>
                    <table>
                        <% if (Model.ShowAccountNumber) { %>
                            <tr>
                                <td>
                                    <span class="label_wrapper"><%: Html.LabelFor(model => model.AccountNumber)%>:</span>
                                </td>
                                <td>
                                    <%: Html.TextBoxFor(model => model.AccountNumber, new { maxlength = "8" })%>
                                    <%: Html.ValidationMessageFor(model => model.AccountNumber)%>
                                </td>
                            </tr>
                        <% } %>
                        <tr>
                            <td>
                                <span class="label_wrapper"><%: Html.LabelFor(model => model.Login) %>:</span>
                            </td>
                            <td>
                                <%: Html.TextBoxFor(model => model.Login, new { maxlength="30"}) %>
                                <%: Html.ValidationMessageFor(model => model.Login) %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="label_wrapper"><%: Html.LabelFor(model => model.Password) %>:</span>
                            </td>
                            <td>
                                <%: Html.PasswordFor(model => model.Password, new { maxlength="20"}) %>
                                <%: Html.ValidationMessageFor(model => model.Password) %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <span id="remember_me"><%: Html.LabelFor(model => model.RememberMe) %>:&nbsp;
                                <%: Html.YesNoToggleFor(model => model.RememberMe) %></span>
                
                                <span id="messageUserLogin" class="field-validation-error" style="display: none"></span>
                
                                <div id="login_button_wrapper">
                                    <input id="btnLogin" type="submit" value="Войти" />
                                </div>
                            </td>
                        </tr>
                    </table>
                <%} %>
                
                <% if(ERP.Wholesale.Settings.AppSettings.IsSaaSVersion) { %>
                    <a id="registrationLink" href='<%= ERP.Wholesale.Settings.AppSettings.AdminAppURL + "/Client/RegisterEasy" %>' >Регистрация</a>
                <% } %>
            </div>

            <div class="clear"></div>            
        </div>

        <div id="login_footer">
            &copy 2010-<%= DateTime.Now.Year.ToString() %>. Empiresoft.
        </div>

    </div>
</body>
</html>
