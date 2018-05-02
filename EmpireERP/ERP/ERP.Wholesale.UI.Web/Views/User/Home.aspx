<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.User.UserHomeViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Моя домашняя страница
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#administration_link_home').click(function () {
                $('#adminForm').trigger("submit");
            });
        });

        $("#btnCreateNewTask").live("click", function () {
            Task_NewTaskGrid.CreateNewTaskByCreatedBy();
        });
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% if (!string.IsNullOrEmpty(Model.Mode) && Model.Mode.Contains("welcome") && ERP.Wholesale.Settings.AppSettings.IsSaaSVersion) { %>
        <div style="padding: 10px 15px; line-height: 1.5">
            <%: ERP.Wholesale.UI.Web.Infrastructure.UserSession.CurrentUserInfo.DisplayName %>, поздравляем, Вы успешно зарегистрированы в Электронном менеджере Bizpulse.
            <br /><br />
            В течение <b>30 дней</b> Вы можете <b>бесплатно</b> использовать все его возможности.
            <br /><br />
            Номер Вашего аккаунта в Bizpulse: <b><%: ERP.Wholesale.UI.Web.Infrastructure.UserSession.CurrentUserInfo.ClientAccountId.ToString() %></b> (пригодится при следующем входе в систему).
            <br /><br />
            На указанный Вами при регистрации e-mail отправлено письмо с Вашими учетными данными для следующего входа в Bizpulse. 
            <br />Если Вы не видете письмо во входящих письмах, возможно, оно попало в папку «Спам».
            <br /><br />
            По умолчанию в Вашем аккаунте Вы включены в команду <a href="/Team/Details?id=1" target="_blank">«Основная команда»</a> и наделены ролью <a href="/Role/Details?id=1" target="_blank">«Администратор»</a>.
            <br /><br />
            Кроме того, мы заполнили для Вас несколько системных справочников, значения которых Вы в любой момент можете изменить:<br />
            <a href="/Currency/List" target="_blank">«Валюты»</a><br />
            <a href="/ValueAddedTax/List" target="_blank">«Ставки НДС»</a><br />
            <a href="/LegalForm/List" target="_blank">«Организационно-правовые формы»</a><br />
            <a href="/Country/List" target="_blank">«Страны»</a><br />
            <a href="/MeasureUnit/List" target="_blank">«Единицы измерения»</a><br />
    
            <br />Основные разделы Электронного менеджера содержат контекстную справку, доступную по нажатию на значок&nbsp;
            <img src="/Content/Img/question.png" style="position: relative; top: 3px;"/>.

            <br /><br />
            Панель администрирования аккаунта доступна по ссылке <a id="administration_link_home" href="#">«Администрирование»</a> в верхнем меню страницы.

            <br /><br />
            Желаем Вам приятной работы.

        </div>

    <%} else {%>
        <%= Html.PageTitle("User", "Моя домашняя страница", ERP.Wholesale.UI.Web.Infrastructure.UserSession.CurrentUserInfo.DisplayName) %>
        <br />

        <% Html.RenderPartial("~/Views/Task/NewTaskGrid.ascx", Model.UserAsCreatorGrid); %>
        <br />

        <% Html.RenderPartial("~/Views/Task/ExecutingTaskGrid.ascx", Model.UserAsExecutorGrid); %>
    <%} %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
