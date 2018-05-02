<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Team.TeamSelectViewModel>" %>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Team_Select") %></div>
    <br />

    <%= Html.GridFilterHelper("filterTeam", Model.FilterData,
        new List<string>() { "gridSelectTeam" }, true) %>
    
    <div id="messageTeamSelectList"></div>    
    <div id="grid_team_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("TeamSelectGrid", Model.TeamsGrid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>