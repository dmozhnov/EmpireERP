<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.CommonPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessCommonPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messageCommonPermissionsEdit");
        scroll(0, $("#messageCommonPermissionsEdit").offset().top - 10);
    }

    function OnFailCommonPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageCommonPermissionsEdit");
        scroll(0, $("#messageCommonPermissionsEdit").offset().top - 10);
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveCommonPermissions", "Role", new AjaxOptions() { OnSuccess = "OnSuccessCommonPermissionsSave", OnFailure = "OnFailCommonPermissionsSave" })) %>
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
        
    <div id="messageCommonPermissionsEdit"></div>

    <div class="permission_group">
        <div class="title">Общие права - действия по всей системе</div>
        <table>
            <%= Html.Permission(Model.PurchaseCost_View_ForEverywhere_ViewModel)%>
            <%= Html.Permission(Model.PurchaseCost_View_ForReceipt_ViewModel)%>
            <%= Html.Permission(Model.AccountingPrice_NotCommandStorage_View_ViewModel)%>
        </table>
    </div>

    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnCommonPermissionsSave" value="Сохранить" />
    </div>

    <%} %>

<% } %>