<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill.ChangeOwnerWaybillChangeRecipientViewModel>" %>

<script type="text/javascript">
    function OnBeginChangeOwnerWaybillChangeRecipient() {
        StartButtonProgress($("#btnChangeRecipient"));
    }
</script>

<% using (Ajax.BeginForm("ChangeRecipient", "ChangeOwnerWaybill", new AjaxOptions() { OnBegin = "OnBeginChangeOwnerWaybillChangeRecipient",
       OnSuccess = "OnSuccessChangeOwnerWaybillChangeRecipient", OnFailure = "ChangeOwnerWaybill_Details.OnFailChangeOwnerWaybillChangeRecipient" }))%>
<%{ %>
    
    <%:Html.HiddenFor(model=>model.WaybillId) %>
    
    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>            

    <div style="padding: 10px 20px 5px; max-width: 500px;">
        <div id="messageChangeOwnerWaybillChangeRecipient"></div> 

         <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 60px">
                    <%: Html.LabelFor(model => model.RecipientId) %>:                    
                </td>
                <td>
                    <%:Html.DropDownListFor(model => model.RecipientId, Model.OrganizationList) %>
                    <%: Html.ValidationMessageFor(x => x.RecipientId) %>
                </td>                   
            </tr>
        </table>

        <div class="button_set">
            <%: Html.SubmitButton("btnChangeRecipient", "Сохранить")%>        
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>