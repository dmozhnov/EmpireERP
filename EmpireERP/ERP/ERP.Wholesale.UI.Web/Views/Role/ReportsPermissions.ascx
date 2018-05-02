<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.ReportsPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessReportsPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messageReportsPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageReportsPermissionsEdit").offset().top - 10);
        }
    }

    function OnFailReportsPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageReportsPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageReportsPermissionsEdit").offset().top - 10);
        }
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveReportsPermissions", "Role", new AjaxOptions()
   {
       OnSuccess = "OnSuccessReportsPermissionsSave", OnFailure = "OnFailReportsPermissionsSave" })) %>
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
    
    <%if (Model.AllowToEdit) { %>
        <div class="button_set">
            <input type="submit" id="btnReportsPermissionsSaveTop" value="Сохранить" />
        </div>
    <%} %>

    <div id="messageReportsPermissionsEdit"></div>

    <div class="permission_group">
        <div class="title">Наличие товаров на местах хранения</div>
        <table>
            <%= Html.Permission(Model.Report0001_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0001_Storage_List_ViewModel)%>            
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Реализация товаров</div>
        <table>
            <%= Html.Permission(Model.Report0002_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0002_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.Report0002_User_List_ViewModel)%>            
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Финансовый отчет</div>
        <table>            
            <%= Html.Permission(Model.Report0003_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0003_Storage_List_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Движение товара за период</div>
        <table>            
            <%= Html.Permission(Model.Report0004_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0004_Storage_List_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Карта движения товара</div>
        <table>            
            <%= Html.Permission(Model.Report0005_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0005_Storage_List_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Отчет по взаиморасчетам с клиентами</div>
        <table>
            <%= Html.Permission(Model.Report0006_View_ViewModel, true)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Отчет по взаиморасчетам по реализациям</div>
        <table>
            <%= Html.Permission(Model.Report0007_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0007_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.Report0007_Date_Change_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Реестр накладных</div>
        <table>
            <%= Html.Permission(Model.Report0008_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0008_Storage_List_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Отчет по поставкам</div>
        <table>
            <%= Html.Permission(Model.Report0009_View_ViewModel, true)%>
            <%= Html.Permission(Model.Report0009_Storage_List_ViewModel)%>
            <%= Html.Permission(Model.Report0009_User_List_ViewModel)%>            
        </table>
    </div>
    <div class="permission_group">
        <div class="title">Принятые платежи</div>
        <table>
            <%= Html.Permission(Model.Report0010_View_ViewModel, true)%>
        </table>
    </div>
     <div class="permission_group">
        <div class="title">Экспорт в 1С</div>
        <table>
            <%= Html.Permission(Model.ExportTo1C_ViewModel, true)%>
        </table>
    </div>

    <%if (Model.AllowToEdit) { %>
        <div class="button_set">
            <input type="submit" id="btnReportsPermissionsSaveBottom" value="Сохранить" />
        </div>
    <%} %>

<% } %>