<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.EconomicAgent.EconomicAgentTypeSelectorViewModel>" %>

<script type="text/javascript">
    function OnFailEconomicAgentTypeSelect(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageEconomicAgentCreate");
    }

    function OnBeginEconomicAgentTypeSelect() {
        StartButtonProgress($("#btnSelectEconomicAgentType"));
    }
</script>

<% using (Ajax.BeginForm("SelectType", "EconomicAgent", new AjaxOptions() { OnFailure = "OnFailEconomicAgentTypeSelect", OnSuccess = "OnSuccessEconomicAgentTypeSelect",
  OnBegin = "OnBeginEconomicAgentTypeSelect" }))%>
<%{ %>
    <%:Html.HiddenFor(x => x.ControllerName) %>
    <%:Html.HiddenFor(x => x.ActionNameForJuridicalPerson) %>
    <%:Html.HiddenFor(x => x.ActionNameForPhysicalPerson) %>
    <%:Html.HiddenFor(x => x.SuccessFunctionName) %>
    <%:Html.HiddenFor(x => x.ContractorId) %>

    <div style="width: 350px">
        <div class='modal_title'><%:Model.Title %><%: Html.Help("/Help/GetHelp_EconomicAgentType_Select") %></div>
        <div class="h_delim"></div>

        <div id="messageEconomicAgentCreate"></div>

        <div style='padding: 10px 10px 5px;'>
            <table class="editor_table">
                <tr>
                    <td>
                        <%:Html.RadioButtonFor(x => x.IsJuridicalPerson, true, new { id = "cbIsJuridicalPerson" })%>
                        <label for="cbIsJuridicalPerson"><%: Model.JuridicalPerson%></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%:Html.RadioButtonFor(x => x.IsJuridicalPerson, false, new { id = "cbIsPhysicalPerson" })%>
                        <label for="cbIsPhysicalPerson"><%: Model.PhysicalPerson%></label>
                    </td>
                </tr>
            </table>        
        
            <div class='button_set'>
                <input id="btnSelectEconomicAgentType" type="submit" value="Создать" />
                <input type="button" value="Закрыть" onclick="HideModal()" />
            </div>
        </div>
    </div>
<%} %>