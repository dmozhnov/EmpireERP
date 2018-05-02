<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Provider.ProviderContractEditViewModel>"%>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    Provider_ContractEdit.Init();

    function OnBeginContractEdit() {
        StartButtonProgress($("#btnSaveProviderContract"));
    }
</script>

<div style="width:540px; padding-left:10px; padding-right:10px;">

<% using (Ajax.BeginForm("EditContract", "Provider", new AjaxOptions() { OnSuccess = "OnSuccessContractEdit", OnFailure = "OnFailContractEdit",
   OnBegin = "OnBeginContractEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    <%: Html.HiddenFor(model => model.ProviderId)%>    

    <div class="modal_title"><%: Model.Title%><%: Html.Help("/Help/GetHelp_ProviderContract_Edit") %></div>
    <br />

    <div id="messageContractEdit"></div>

    <div class="group_title">Информация о договоре</div>
    <div class="h_delim"></div>
    <br />

    <table class="editor_table" style="width: 510px;">
        <tr>
            <td class="row_title" style="width: 90px">
                <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_ProviderContract_Edit_Number")%>:
            </td>
            <td>
                <%: Html.TextBoxFor(model => model.Number, new { style = "width:220px;", maxlength = "50" }, !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.Number)%>
            </td>
            <td class="row_title">
                <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_ProviderContract_Edit_Date")%>:
            </td>
            <td style="text-align: left;">
                <%= Html.DatePickerFor(model => model.Date, null, !Model.AllowToEdit, !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.Date)%>
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%: Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_ProviderContract_Edit_Name")%>:
            </td>
            <td colspan="3">
                <%: Html.TextBoxFor(model => model.Name, new { style = "width:400px;", maxlength = "200" }, !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.Name)%>
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
            </td>
            <td colspan="3">                
                <%: Html.CommentFor(model => model.Comment, new { style = "width:400px" }, !Model.AllowToEdit, rowsCount: 4)%>
                <%: Html.ValidationMessageFor(model => model.Comment)%>                
            </td>
        </tr>
    </table>

    <br />
    <div class="group_title">Между кем и кем</div>
    <div class="h_delim"></div>
    <br />

    <table class="editor_table" style="width:510px;">
        <tr>
            <td class="row_title" style="width:160px;">
                <%: Html.LabelFor(model => model.AccountOrganizationName)%>:
            </td>
            <td style="width:360px; padding-left:15px;">
                <span id="linkAccountOrganizationSelector" <% if (Model.AllowToChangeOrganizations) { %> class="select_link" <% } %> >
                    <span id="AccountOrganizationName"><%: Model.AccountOrganizationName%></span>
                </span>                
                <%: Html.HiddenFor(model => model.AccountOrganizationId)%>
                <%: Html.ValidationMessageFor(model => model.AccountOrganizationId)%>
            </td>
        </tr>
        <tr>
            <td class="row_title" style="width:160px;">
                <%: Html.LabelFor(model => model.ProviderOrganizationName)%>:
            </td>
            <td style="width:360px; padding-left:15px;">
                <span id="linkProviderOrganizationSelector" <% if (Model.AllowToChangeOrganizations) { %> class="select_link" <% } %> >
                    <span id="ProviderOrganizationName"><%: Model.ProviderOrganizationName %></span>
                </span>
                                
                <%: Html.HiddenFor(model => model.ProviderOrganizationId)%>
                <%: Html.ValidationMessageFor(model => model.ProviderOrganizationId)%>
            </td>
        </tr>
    </table>

    <br />

    <div class="button_set">
        <%: Html.SubmitButton("btnSaveProviderContract", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>        
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

</div>
