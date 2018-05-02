<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.User.UserSelectViewModel>" %>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_User_Select") %></div>
    <br />

    <%= Html.GridFilterHelper("filterUser", Model.FilterData,
        new List<string>() { "gridSelectUser" }, true) %>
    
    <div id="messageUserSelectList"></div>    
    <div id="grid_user_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("UserSelectGrid", Model.UsersGrid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>