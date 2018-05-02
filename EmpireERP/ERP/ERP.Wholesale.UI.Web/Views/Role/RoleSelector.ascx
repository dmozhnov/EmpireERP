<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.RoleSelectViewModel>" %>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Role_Select") %></div>
    <br />

    <%= Html.GridFilterHelper("filterRole", Model.FilterData,
        new List<string>() { "gridSelectRole" }, true) %>
    
    <div id="messageRoleSelectList"></div>    
    <div id="grid_role_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("RoleSelectGrid", Model.RolesGrid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>